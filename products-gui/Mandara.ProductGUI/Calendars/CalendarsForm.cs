using System;
using System.ComponentModel;
using System.Windows.Forms;
using Mandara.Business;
using Mandara.Business.Authorization;
using Mandara.Entities;

namespace Mandara.ProductGUI
{
    public partial class CalendarsForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly ProductManager _productManager;

        /// <summary>
        /// Initialize form
        /// </summary>
        /// <param name="currentUser">Logged in user object</param>
        public CalendarsForm(User currentUser)
        {
            InitializeComponent();

            expiryYearSelector.BorderStyle = BorderStyle.None;
            selectHolydatsYearMonthControl.BorderStyle = BorderStyle.None;

            //Respect user's permissions
            AllowChanges = AuthorizationService.IsUserAuthorizedTo(currentUser, PermissionType.SuperAdministrator)
                           || AuthorizationService.IsUserAuthorizedTo(currentUser, PermissionType.Administrator)
                           ||
                           AuthorizationService.IsUserAuthorizedTo(currentUser,
                               PermissionType.ProductMgmtToolWriteAccess);

            _productManager = new ProductManager();

            expiryDatesControl.SetProductManager(_productManager);
            expiryDatesControl.ExpiryDateChanged += OnExpiryDateChanged;
            holidaysControl.SetProductManager(_productManager);
        }

        private void OnExpiryDateChanged(object sender, EventArgs e)
        {
            bool areExpiryDatesValid = expiryDatesControl.IsValid;

            ok.Enabled = areExpiryDatesValid;
            apply.Enabled = areExpiryDatesValid;
            expiryYearSelector.Enabled = areExpiryDatesValid;
        }

        private bool Save()
        {
            try
            {
                if (holidaysControl.Save())
                {
                    SetCurrentYear();
                }

                if (expiryDatesControl.Save())
                {
                    SetCurrentHolidayMonth();
                }

                return true;
            }
            catch (Exception anythingCouldGoWrong)
            {
                MessageBox.Show(
                    $"Something bad happened: {anythingCouldGoWrong.Message}",
                    "Save Calendar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private DialogResult AskAndSaveChangesIfAny()
        {
            DialogResult dialogResult = DialogResult.Yes;

            if (holidaysControl.HasChanges)
            {
                dialogResult = MessageBox.Show(
                    "Save holidays changes?",
                    "Save changes.",
                    MessageBoxButtons.YesNoCancel);

                if (dialogResult == DialogResult.Yes)
                {
                    if (holidaysControl.Save())
                    {
                        SetCurrentHolidayMonth();
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    holidaysControl.RejectChanges();
                }
            }
            else if (expiryDatesControl.HasChanges)
            {
                dialogResult = MessageBox.Show("Save expiry changes?", "Save changes.", MessageBoxButtons.YesNoCancel);

                if (dialogResult == DialogResult.Yes)
                {
                    if (expiryDatesControl.Save())
                    {
                        SetCurrentYear();
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    expiryDatesControl.RejectChanges();
                }
            }

            return dialogResult;
        }

        private void SetCurrentHolidayMonth()
        {
            holidaysControl.SetMonth(
                selectHolydatsYearMonthControl.SelectedYear,
                selectHolydatsYearMonthControl.SelectedMonthIndex);
        }

        private void SetCurrentYear()
        {
            expiryDatesControl.SetYear(expiryYearSelector.SelectedYear);
        }

        private void SetControls()
        {
            expiryYearSelector.SelectedYear = DateTime.Now.Year;
            SetCurrentHolidayMonth();
            SetCurrentYear();

            //Respect user's permissions
            apply.Visible =
                holidaysControl.AllowChanges =
                    expiryDatesControl.AllowChanges = AllowChanges;
        }


        private bool AllowChanges { get; set; }


        private void CalendarsForm_Load(object sender, EventArgs e)
        {
            SetControls();

            selectHolydatsYearMonthControl.ValueChanging += selectedHolidaysYearMonthChanging;
            selectHolydatsYearMonthControl.ValueChanged += selectedHolidaysYearMonthChanged;

            expiryYearSelector.ValueChanging += ExpiryYearSelectorValueChanging;
            expiryYearSelector.ValueChanged += ExpiryYearSelectorValueChanged;

            calendarTabControl.SelectedPageChanging += calendarTabControl_SelectedPageChanging;

        }

        private void selectedHolidaysYearMonthChanged(object sender, EventArgs ignore)
        {
            SetCurrentHolidayMonth();
        }

        private void selectedHolidaysYearMonthChanging(object sender, CancelEventArgs cancelEvt)
        {
            cancelEvt.Cancel = (AskAndSaveChangesIfAny() == DialogResult.Cancel);
        }

        private void calendarTabControl_SelectedPageChanging(
            object sender,
            DevExpress.XtraTab.TabPageChangingEventArgs pageChanging)
        {
            if (calendarTabControl.SelectedTabPage.Name == expiryDatesTabPage.Name && !expiryDatesControl.IsValid)
            {
                pageChanging.Cancel = true;
                return;
            }

            pageChanging.Cancel = (AskAndSaveChangesIfAny() == DialogResult.Cancel);
        }

        private void ExpiryYearSelectorValueChanged(object sender, EventArgs ignore)
        {
            SetCurrentYear();
        }

        private void ExpiryYearSelectorValueChanging(object sender, CancelEventArgs cancelEvt)
        {
            cancelEvt.Cancel = (AskAndSaveChangesIfAny() == DialogResult.Cancel);
        }

        private void OkClicked(object sender, EventArgs ignore)
        {
            if (expiryDatesControl.IsValid && AllowChanges)
            {
                if (Save())
                {
                    Close();
                }
            }
        }

        private void ApplyClicked(object sender, EventArgs ignore)
        {
            if (expiryDatesControl.IsValid)
            {
                Save();
            }
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            Close();
        }

        private bool AreExpiryDatesValid()
        {
            if (!expiryDatesControl.IsValid)
            {
                MessageBox.Show(
                    "Non-sequential expiry dates, please check highlighted cell(s)",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CalendarsForm_FormClosing(object sender, FormClosingEventArgs closingArgs)
        {
            if (closingArgs.CloseReason == CloseReason.UserClosing)
            {
                closingArgs.Cancel = DialogResult.Cancel != DialogResult
                                     && (AllowChanges && AskAndSaveChangesIfAny() == DialogResult.Cancel);
            }
        }

        private void calendarTabControl_SelectedPageChanging_1(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            if (calendarTabControl.SelectedTabPage.Name == expiryDatesTabPage.Name && !expiryDatesControl.IsValid)
            {
                e.Cancel = true;
            }
        }
    }
}