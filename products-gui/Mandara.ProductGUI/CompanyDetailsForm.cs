using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Mandara.Business;
using Mandara.Entities;
using Mandara.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Mandara.Extensions.Collections;

namespace Mandara.ProductGUI
{
    public partial class CompanyDetailsForm : XtraForm
    {
        public bool? CompanyUpdated { get; private set; }

        private readonly Company _company;
        private readonly BrokerManager _brokerManager;
        private BindingList<CompanyAlias> _companyAliases;
        private BindingList<CompanyBrokerage> _companyBrokerages;

        public CompanyDetailsForm(Company company = null)
        {
            InitializeComponent();

            _company = company;
            _brokerManager = new BrokerManager();

            if (_company == null)
            {
                _company = new Company
                {
                    CompanyName = "New company"
                };
            }
            else
            {
                _company = _brokerManager.GetCompanyById(_company.CompanyId) ?? company;
            }


            leRegion.Properties.DataSource = _brokerManager.GetRegions();

            txtCompanyName.DataBindings.Add("EditValue", _company, "CompanyName", true);
            leRegion.DataBindings.Add("EditValue", _company, "Region", true);
            txtAbbr.DataBindings.Add("EditValue", _company, "AbbreviationName", true);
            txtDefaultBrokerage.DataBindings.Add("EditValue", _company, "DefaultBrokerage", true);
            txtDefaultBrokerageKt.DataBindings.Add("EditValue", _company, "DefaultBrokerageKt", true);

            gcAliases.DataSource = _companyAliases = new BindingList<CompanyAlias>(new List<CompanyAlias>(_company.CompanyAliases));
            _company.CompanyAliases.SyncWithBindingList(_companyAliases);


            riUnitLookup.DataSource = new ProductManager().GetUnits();
            riConditionLookup.DataSource = GetListOfEnums(typeof(VolumeCondition));

            gcBrokerages.DataSource = _companyBrokerages = new BindingList<CompanyBrokerage>(new List<CompanyBrokerage>(_company.Brokerages));
            _company.Brokerages.SyncWithBindingList(_companyBrokerages);
        }

        private IEnumerable<EnumModel> GetListOfEnums(Type type)
        {
            var values = Enum.GetValues(type);

            foreach (Enum value in values)
            {
                yield return new EnumModel
                {
                    Value = value,
                    Description = value.GetDescription()
                };
            }
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            if (ValidateForm())
            {
                if (_company.CompanyAliases != null)
                {
                    _company.CompanyAliases.Remove(a => string.IsNullOrEmpty(a.AliasName));
                }

                var auditContext = ProductListForm.CreateAuditContext("Company Details");

                try
                {
                    _brokerManager.SaveCompany(_company, auditContext);
                    CompanyUpdated = true;
                    Close();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateForm()
        {
            txtCompanyName.Focus();

            Boolean isValid = true;

            isValid = isValid && !string.IsNullOrEmpty(_company.CompanyName);
            txtCompanyName.ErrorText = string.IsNullOrEmpty(_company.CompanyName) ? "Company voice name could not be blank" : null;

            isValid = isValid && _company.Region != null;
            leRegion.ErrorText = _company.Region == null ? "Region is required" : null;

            // check that company aliases are unique
            if (_company.CompanyAliases != null)
            {
                List<string> aliases = _company.CompanyAliases.Where(a => !string.IsNullOrEmpty(a.AliasName)).Select(a => a.AliasName.Trim()).ToList();

                bool checkAliasesUnique = _brokerManager.CheckAliasesUnique(aliases, _company.CompanyId);

                isValid = isValid && checkAliasesUnique;

                if (!checkAliasesUnique)
                {
                    XtraMessageBox.Show(this, "Either there are two identical aliases for the company \r\nor one of the specified aliases is already mapped to another company.", "Aliases not unique", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // check Brokerage settings
            if (_company.Brokerages != null)
            {
                var units = _company.Brokerages.Select(x => x.Unit).ToLookup(key => key, el => 1);

                var moreThanOneUnitRow = units.FirstOrDefault(x => x.Count() > 1);

                if (moreThanOneUnitRow != null)
                {
                    isValid = false;

                    XtraMessageBox.Show(this, "There are more than one row per unit in the brokerage settings.", "Units are not unique", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return isValid;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void CompanyDetailsForm_Load(object sender, EventArgs e)
        {

        }

        private void riConditionLookup_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            if (e.Value == null)
                return;

            var volumeCondition = (VolumeCondition)e.Value;
            e.DisplayText = volumeCondition.GetDescription();
        }

        private void gvAliases_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            CompanyAlias alias = (CompanyAlias)gvAliases.GetRow(e.RowHandle);
            alias.Company = _company;
        }

        private void gvBrokerages_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            CompanyBrokerage companyBrokerage = (CompanyBrokerage)gvBrokerages.GetRow(e.RowHandle);
            companyBrokerage.Company = _company;
        }
    }

    internal class EnumModel
    {
        public Enum Value { get; set; }
        public string Description { get; set; }
    }
}