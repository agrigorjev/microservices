using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using Mandara.Business;
using Mandara.Business.Audit;
using Mandara.Business.Authorization;
using Mandara.Business.Bus.Messages.TradeAdd;
using Mandara.Business.TradeAdd;
using Mandara.Date;
using Mandara.Date.Time;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Entities.ErrorDetails;
using Mandara.Extensions.Collections;
using Mandara.Extensions.Nullable;
using Mandara.ProductGUI.Calendars;
using Mandara.ProductGUI.Models;
using ComboBox = DevExpress.XtraEditors.ComboBox;
using Unit = Mandara.Entities.Unit;

namespace Mandara.ProductGUI
{
    internal partial class ProductDetailsForm : XtraForm
    {
        private class RolloffSettings
        {
            public bool UseRollOffTime { get; set; }
            public DateTime? RollOffTime { get; set; }
            public string RollOffTimezoneId { get; set; }
            public bool UseFuturesExpireTime { get; set; }
            public DateTime? TasActivateTime { get; set; }
            public string TasTimezoneId { get; set; }
            public DateTime? FuturesExpireTime { get; set; }

            /// <summary>
            /// Referenced in a control data binding.
            /// </summary>
            public TimeZoneInfo TasTimezone
            {
                get => string.IsNullOrEmpty(TasTimezoneId)
                        ? null
                        : TimeZoneInfo.FindSystemTimeZoneById(TasTimezoneId);
                set => TasTimezoneId = value?.Id;
            }

            /// <summary>
            /// Referenced in a control data binding.
            /// </summary>
            public TimeZoneInfo RollOffTimezone
            {
                get => string.IsNullOrEmpty(RollOffTimezoneId)
                        ? null
                        : TimeZoneInfo.FindSystemTimeZoneById(RollOffTimezoneId);
                set => RollOffTimezoneId = value?.Id;
            }

            /// <summary>
            /// Referenced in a control data binding.
            /// </summary>
            public string LocalRollOffTime
            {
                get
                {
                    if (!RollOffTime.HasValue || null == RollOffTimezoneId)
                    {
                        return null;
                    }

                    DateTime now = SystemTime.Now();

                    DateTime rolloffTimeWithoutTimezone = new DateTime(
                        now.Year,
                        now.Month,
                        now.Day,
                        RollOffTime.Value.Hour,
                        RollOffTime.Value.Minute,
                        RollOffTime.Value.Second,
                        DateTimeKind.Unspecified);

                    DateTime localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                        rolloffTimeWithoutTimezone,
                        RollOffTimezoneId,
                        TimeZoneInfo.Local.Id);

                    return localTime.ToShortTimeString();
                }
            }
        }

        public int? UpdatedProductId;

        private readonly ProductManager _productManager = new ProductManager();
        private readonly Product _productToEdit;
        private BindingList<ProductAlias> _productAliases;
        private BindingList<IceProductMapping> _iceProductsMappings;
        private BindingList<ProductBrokerage> _companiesBrokerage;
        private BindingList<ABNMappings> _productAbnmAppings;
        private BindingList<GmiBalmoCode> _gmiBalmoCodes;
        private BindingList<IceBalmoMapping> _iceBalmoMappings;
        private TradeAddDetails TradeAddDetails { get; set; }
        private TaskScheduler _uiSynchronizationContext;
        private readonly StripGenerator _stripGenerator = new StripGenerator();
        private const int EmptyOfficialProductId = 0;
        private IDisposable _observeImpact;

        private readonly RolloffSettings _rolloffSettings = new RolloffSettings();
        private CalendarClashCheck _calendarCheck;
        private static readonly DateTime DefaultMinDate = new DateTime(1900, 1, 1);

        public ProductDetailsForm(Product product = null)
        {
            InitializeComponent();

            List<Currency> currencies = _productManager.GetCurrencies();

            if (product == null)
            {
                Currency defaultCurrency = currencies.Find(x => x.IsoName == CurrencyCodes.USD);

                _productToEdit = CreateNewProduct(defaultCurrency);
            }
            else
            {
                _productToEdit = _productManager.GetProduct(product.ProductId);

                _productToEdit.UseRolloffSettings = _productToEdit.UseRolloffSettings.True();
                _productToEdit.UseExpiryCalendar = _productToEdit.UseExpiryCalendar.True();

                isRollingBackward.Checked = _productToEdit.RollingMethod == RollingMethod.Backward;
                expiryTypeGivenDateSelector.Checked = _productToEdit.ExpirationType == ExpirationType.GivenDate;
                expTypeNumOfDaysSelector.Checked = _productToEdit.ExpirationType == ExpirationType.NumberOfDays;
            }

            TradeImpact_Load();
        }

        private void SetProductDataBindings(List<Currency> currencies)
        {
            if (_productToEdit.Type == ProductType.Futures)
            {
                _rolloffSettings.UseFuturesExpireTime = _productToEdit.UseRolloffSettings.True();
                _rolloffSettings.TasTimezoneId = _productToEdit.TimezoneId;
                _rolloffSettings.TasActivateTime = _productToEdit.RolloffTime;
            }
            else
            {
                if (TasType.NotTas == _productToEdit.TasType)
                {
                    _rolloffSettings.UseRollOffTime = _productToEdit.UseRolloffSettings.True();
                    _rolloffSettings.RollOffTime = _productToEdit.RolloffTime;
                    _rolloffSettings.RollOffTimezoneId = _productToEdit.TimezoneId;
                }
                else
                {
                    _rolloffSettings.UseRollOffTime = false;
                    _rolloffSettings.TasActivateTime = _productToEdit.RolloffTime;
                    _rolloffSettings.TasTimezoneId = _productToEdit.TimezoneId;
                }
            }

            _rolloffSettings.FuturesExpireTime = _productToEdit.FuturesExpireTime;

            productName.DataBindings.Add("EditValue", _productToEdit, "Name", true);

            BindProductType();
            SetExpirationSettingsPanelsVisibility(_productToEdit.ExpirationType);

            List<OfficialProduct> officialProducts = _productManager.GetOfficialProducts();

            BindOfficialProducts(officialProducts);
            BindRelatedFutures(officialProducts);
            BindCalendars();
            SetCurrencies(currencies);
            BindCategories();
            BindProperties();
            SetTimezones();

            companySelector.DataSource = new BrokerManager().GetCompanies();

            BindUnits();
            BindBalmoFields();
            BindExchange();
            BindRollOffFields();
            BindFactors();

            contractSize.DataBindings.Add("EditValue", _productToEdit, "ContractSize", true);

            BindCurrencies();
            BindCategory();
            BindProductValidityDates();

            definitionLink.DataBindings.Add("EditValue", _productToEdit, "DefinitionLink", true);
            allowedForManualTrades.DataBindings.Add("EditValue", _productToEdit, "IsAllowedForManualTrades", true);

            BindContractCodes();
            BindFees();
            BindAliases();
            BindAbnMappings();
            BindCompanyBrokerage();
            BindBalmoCodes();

            isPhysicallySettled.DataBindings.Add("EditValue", _productToEdit, "IsPhysicallySettled");
            isInternalTransferProduct.DataBindings.Add("EditValue", _productToEdit, "IsInternalTransferProduct");
            priceConversionFactor.DataBindings.Add("EditValue", _productToEdit, "PriceConversionFactor");
            treatTimeSpreadStripAsLegs.DataBindings.Add("EditValue", _productToEdit, "TreatTimespreadStripAsLegs");
            calculatePnlFromLegs.DataBindings.Add("EditValue", _productToEdit, "CalculatePnlFromLegs");

            SetComplexControls(_productToEdit.Type);

            productTabs.SelectedTabPageIndex = 0;

            BindDailyDiffMonthShift();

            enableRiskDecomposition.DataBindings.Add(
                "EditValue",
                _productToEdit,
                "IsEnabledRiskDecomposition",
                true);

            BindContractSizeMultipliers();
        }

        private void UpdateTasActivationTimeControls()
        {
            if (_productToEdit.TasType == TasType.NotTas)
            {
                DisableTasActivationTime();
            }
            else
            {
                EnableTasActivationTime();
            }
        }

        private void DisableTasActivationTime()
        {
            tasActivationTime.Enabled = false;
            tasTimezoneSelector.Enabled = false;
            tasOfficialProductSelector.Enabled = false;

            if (_productToEdit.Type == ProductType.Futures)
            {
                tasActivationTime.EditValue = _rolloffSettings.TasActivateTime = null;
                _rolloffSettings.TasTimezoneId = null;
                tasTimezoneSelector.SelectedIndex = -1;
            }
        }

        private void EnableTasActivationTime()
        {
            tasActivationTime.Enabled = true;
            tasTimezoneSelector.Enabled = true;
            tasOfficialProductSelector.Enabled = true;
        }

        private void UpdateMocActivationTimeControls()
        {
            if (_productToEdit.TasType == TasType.NotTas)
            {
                DisableMocActivationTime();
            }
            else
            {
                EnableMocActivationTime();
            }
        }

        private void DisableMocActivationTime()
        {
            mocActivationTime.Enabled = false;
            //mocActivationTime.EditValue = _rolloffSettings.TasActivateTime = null;
        }

        private void EnableMocActivationTime()
        {
            mocActivationTime.Enabled = true;
        }

        private void BindDailyDiffMonthShift()
        {
            dailyDiffMonthShift.Properties.DataSource = DailyDiffMonthShift.GetList();
            dailyDiffMonthShift.DataBindings.Add("EditValue", _productToEdit, "DailyDiffMonthShift", true);
        }

        private void BindCompanyBrokerage()
        {
            companiesBrokerage.DataSource = _companiesBrokerage =
                new BindingList<ProductBrokerage>(new List<ProductBrokerage>(_productToEdit.CompaniesBrokerages));
            _productToEdit.CompaniesBrokerages.SyncWithBindingList(_companiesBrokerage);
        }

        private void BindAbnMappings()
        {
            abnMappings.DataSource = _productAbnmAppings =
                new BindingList<ABNMappings>(new List<ABNMappings>(_productToEdit.ABNMappings));
            _productToEdit.ABNMappings.SyncWithBindingList(_productAbnmAppings);
        }

        private void BindAliases()
        {
            productAliases.DataSource = _productAliases =
                new BindingList<ProductAlias>(new List<ProductAlias>(_productToEdit.Aliases));
            _productToEdit.Aliases.SyncWithBindingList(_productAliases);

            iceAliases.DataSource = _iceProductsMappings =
                new BindingList<IceProductMapping>(new List<IceProductMapping>(_productToEdit.IceProductsMappings));
            _productToEdit.IceProductsMappings.SyncWithBindingList(_iceProductsMappings);
        }

        private void BindProductValidityDates()
        {
            validFrom.DataBindings.Add("EditValue", _productToEdit, "ValidFrom", true);
            validTo.DataBindings.Add("EditValue", _productToEdit, "ValidTo", true);
        }

        private void BindCategory()
        {
            productCategorySelector.DataBindings.Add("EditValue", _productToEdit, "Category", true);
            categoryOverride.DataBindings.Add("EditValue", _productToEdit, "CategoryOverride", true);
            categoryOverrideDate.DataBindings.Add("EditValue", _productToEdit, "CategoryOverrideAt", true);
        }

        private void BindProductType()
        {
            productTypeSelector.Properties.DataSource = ProductTypeBindingHelper.GetAllInstances();
            productTypeSelector.DataBindings.Add("EditValue", _productToEdit, "Type", true);
        }

        private void BindFactors()
        {
            pnlFactor.DataBindings.Add("EditValue", _productToEdit, "PnlFactor", true);
            positionFactor.DataBindings.Add("EditValue", _productToEdit, "PositionFactor", true);
        }

        private void BindRollOffFields()
        {
            if (ProductType.Swap == _productToEdit.Type && TasType.NotTas != _productToEdit.TasType)
            {
                mocActivationTime.DataBindings.Add("Text", _rolloffSettings, "LocalRollOffTime", true);
            }
            else
            {
                localRollOffTime.DataBindings.Add("Text", _rolloffSettings, "LocalRollOffTime", true);
            }

            futuresExpiryTime.DataBindings.Add(
                "EditValue",
                _rolloffSettings,
                "FuturesExpireTime",
                true,
                DataSourceUpdateMode.OnPropertyChanged,
                new DateTime(1900, 1, 1));
            pricingEndTime.DataBindings.Add(
                "EditValue",
                _productToEdit,
                "PricingEndTime",
                true,
                DataSourceUpdateMode.OnPropertyChanged,
                new DateTime(1900, 1, 1));
            givenDate.DataBindings.Add("EditValue", _productToEdit, "GivenDate", true);
            expirationMonthSelector.DataBindings.Add("EditValue", _productToEdit, "ExpirationMonth", true);
            numberOfDaysSelector.DataBindings.Add("EditValue", _productToEdit, "NumberOfDays", true);
        }

        private void BindExchange()
        {
            exchangeSelector.Properties.DataSource = _productManager.GetExchanges();
            exchangeSelector.DataBindings.Add("EditValue", _productToEdit, "Exchange");
        }

        private void BindBalmoFields()
        {
            if (_productToEdit.Type == ProductType.Balmo)
            {
                balmoCrudeSwapsSelector.Properties.DataSource = _productManager.GetProducts_CrudeSwapsAndGasoilSwaps();
                balmoComplexProductsSelector.Properties.DataSource = _productManager.GetProducts_Complex();
            }

            balmoCrudeSwapsSelector.DataBindings.Add("EditValue", _productToEdit, "BalmoOnCrudeProduct", true);
            balmoComplexProductsSelector.DataBindings.Add("EditValue", _productToEdit, "BalmoOnComplexProduct", true);
        }

        private void BindUnits()
        {
            unitsSelector.Properties.DataSource = _productManager.GetUnits();
            unitsSelector.DataBindings.Add("EditValue", _productToEdit, "Unit");
        }

        private void SetTimezones()
        {
            List<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();

            rolloffTimezoneSelector.Properties.Items.Clear();
            mocActivationTimezoneSelector.Properties.Items.Clear();

            timeZones.ForEach(
                timeZone =>
                {
                    AddTimeZoneToSelectors(new ComboBoxItem(timeZone));
                });
        }

        private void AddTimeZoneToSelectors(ComboBoxItem timeZone)
        {
            rolloffTimezoneSelector.Properties.Items.Add(timeZone);
            mocActivationTimezoneSelector.Properties.Items.Add(timeZone);
            tasTimezoneSelector.Properties.Items.Add(timeZone);
        }

        private void BindProperties()
        {
            List<Product> productsSource = _productManager.GetProducts();
            leg1ProductSelector.Properties.DataSource = productsSource;
            leg2ProductSelector.Properties.DataSource = productsSource.ToList();
        }

        private void BindRelatedFutures(List<OfficialProduct> officialProducts)
        {
            OfficialProduct emptyOfficialProduct =
                new OfficialProduct() { OfficialProductId = EmptyOfficialProductId, Name = "[None]" };
            List<OfficialProduct> underlyingFuturesSource = new List<OfficialProduct> { emptyOfficialProduct }
                .Union(officialProducts.ToList())
                .ToList();
            underlyingFuturesSelector.Properties.DataSource = underlyingFuturesSource;
            underlyingFuturesOverride.Properties.DataSource = underlyingFuturesSource.ToList();
            underlyingFuturesSelector.DataBindings.Add("EditValue", _productToEdit, "UnderlyingFutures", true);
            underlyingFuturesOverride.DataBindings.Add(
                "EditValue",
                _productToEdit,
                "UnderlyingFuturesOverride",
                true);
        }

        private void BindCategories()
        {
            List<ProductCategory> productCategories = _productManager.GetGroups();
            productCategorySelector.Properties.DataSource = productCategories;
            categoryOverride.Properties.DataSource = productCategories.ToList();
        }

        private void BindCalendars()
        {
            useExpiryCalendar.DataBindings.Add("EditValue", _productToEdit, "UseExpiryCalendar", true);
            expiryCalendar.Properties.DataSource = _productManager.GetExpiryCalendars();
            holidaysCalendar.Properties.DataSource = _productManager.GetHolidaysCalendars();
            expiryCalendar.DataBindings.Add("EditValue", _productToEdit, "ExpiryCalendar", true);
            holidaysCalendar.DataBindings.Add("EditValue", _productToEdit, "HolidaysCalendar", true);
        }

        private void BindOfficialProducts(List<OfficialProduct> officialProducts)
        {
            officialProductSelector.Properties.DataSource = officialProducts;
            tasOfficialProductSelector.Properties.DataSource = officialProducts.ToList();
            monthlyOfficialProductSelector.Properties.DataSource = officialProducts.ToList();
            officialProductSelector.DataBindings.Add("EditValue", _productToEdit, "OfficialProduct", true);
            tasOfficialProductSelector.DataBindings.Add("EditValue", _productToEdit, "TasOfficialProduct", true);
            monthlyOfficialProductSelector.DataBindings.Add("EditValue", _productToEdit, "MonthlyOfficialProduct", true);
        }

        private void SetCurrencies(List<Currency> currencies)
        {
            List<Currency> currenciesPlusNull = new List<Currency>();
            currenciesPlusNull.AddRange(currencies);
            currenciesPlusNull.Insert(0, new Currency() { CurrencyId = 0, IsoName = "Non" });

            currency1Selector.Properties.DataSource = currenciesPlusNull;
            currency2Selector.Properties.DataSource = currenciesPlusNull;

            blockCurrencySelector.Properties.DataSource = currencies;
            cashCurrencySelector.Properties.DataSource = currencies;
            plattsCurrencySelector.Properties.DataSource = currencies;
            primeBrokerCurrencySelector.Properties.DataSource = currencies;
            nfaCurrencySelector.Properties.DataSource = currencies;
            clearingCurrencySelector.Properties.DataSource = currencies;
            exchangeCurrencySelector.Properties.DataSource = currencies;
        }

        private void BindCurrencies()
        {
            currency1Selector.DataBindings.Add("EditValue", _productToEdit, "Currency1", true);
            currency2Selector.DataBindings.Add("EditValue", _productToEdit, "Currency2", true);
            exchangeCurrencySelector.DataBindings.Add("EditValue", _productToEdit, "FeeExchangeCurrency", true);
            clearingCurrencySelector.DataBindings.Add("EditValue", _productToEdit, "FeeClearingCurrency", true);
            cashCurrencySelector.DataBindings.Add("EditValue", _productToEdit, "FeeCashCurrency", true);
            plattsCurrencySelector.DataBindings.Add("EditValue", _productToEdit, "FeePlattsCurrency", true);
            blockCurrencySelector.DataBindings.Add("EditValue", _productToEdit, "FeeBlockCurrency", true);
            primeBrokerCurrencySelector.DataBindings.Add("EditValue", _productToEdit, "FeeCommissionCurrency", true);
            nfaCurrencySelector.DataBindings.Add("EditValue", _productToEdit, "FeeNfaCurrency", true);
        }

        private void BindContractCodes()
        {
            contractCodeOne.DataBindings.Add("EditValue", _productToEdit, "BalmoContractCode1");
            contractCodeTwo.DataBindings.Add("EditValue", _productToEdit, "BalmoContractCode2");
            contractCodeThree.DataBindings.Add("EditValue", _productToEdit, "BalmoContractCode3");
            contractCodeOneFirstLetter.DataBindings.Add("EditValue", _productToEdit, "BalmoCodeFirstLetter");
            exchangeContractCode.DataBindings.Add("EditValue", _productToEdit, "ExchangeContractCode", true);
        }

        private void BindFees()
        {
            clearingFee.DataBindings.Add("EditValue", _productToEdit, "FeeClearing", true);
            commisionFee.DataBindings.Add("EditValue", _productToEdit, "FeeCommission", true);
            exchangeFee.DataBindings.Add("EditValue", _productToEdit, "FeeExchange", true);
            nfaFee.DataBindings.Add("EditValue", _productToEdit, "FeeNfa", true);
            blockFee.DataBindings.Add("EditValue", _productToEdit, "FeeBlockTrade", true);
            plattsFee.DataBindings.Add("EditValue", _productToEdit, "FeePlattsTrade", true);

            cashFee.DataBindings.Add("EditValue", _productToEdit, "FeeCash", true);
        }

        private void BindBalmoCodes()
        {
            gmiBalmoCodes.DataSource = _gmiBalmoCodes =
                new BindingList<GmiBalmoCode>(new List<GmiBalmoCode>(_productToEdit.GmiBalmoCodes));
            _productToEdit.GmiBalmoCodes.SyncWithBindingList(_gmiBalmoCodes);

            iceBalmoMappings.DataSource = _iceBalmoMappings =
                new BindingList<IceBalmoMapping>(new List<IceBalmoMapping>(_productToEdit.IceBalmoMappings));
            _productToEdit.IceBalmoMappings.SyncWithBindingList(_iceBalmoMappings);
        }

        private void BindContractSizeMultipliers()
        {
            InitialiseContractSizeMultipliers();
            contractSizeMultiplier.DataBindings.Add("EditValue", _productToEdit, "ContractSizeMultiplier", true);
        }

        private static Product CreateNewProduct(Currency defaultCurrency)
        {
            return new Product
            {
                Name = "New product",
                Type = ProductType.Futures,
                UseRolloffSettings = false,
                FeeBlockCurrency = defaultCurrency,
                FeeCashCurrency = defaultCurrency,
                FeeExchangeCurrency = defaultCurrency,
                FeeCommissionCurrency = defaultCurrency,
                FeePlattsCurrency = defaultCurrency,
                FeeNfaCurrency = defaultCurrency,
                FeeClearingCurrency = defaultCurrency
            };
        }

        private void InitialiseContractSizeMultipliers()
        {
            Array contractSizeMultiplierValues = Enum.GetValues(typeof(ContractSizeMultiplier));
            contractSizeMultiplier.Properties.Items.AddRange(contractSizeMultiplierValues);

            int index = 0;

            foreach (ContractSizeMultiplier multiplier in contractSizeMultiplierValues)
            {
                if (multiplier == ContractSizeMultiplier.Monthly)
                {
                    break;
                }

                ++index;
            }

            contractSizeMultiplier.SelectedIndex = index;
            SetContractSizeMultiplierEnabledState();
        }

        private void SetContractSizeMultiplierEnabledState()
        {
            Unit currentUnit = unitsSelector.EditValue as Unit;

            if (null != currentUnit)
            {
                contractSizeMultiplier.Enabled = !currentUnit.AllowOnlyMonthlyContractSize;
            }
            else
            {
                contractSizeMultiplier.Enabled = false;
            }
        }

        private bool AreAllExpiryDatesOnBusinessDays()
        {
            Dictionary<DateTime, CalendarHoliday> holidays =
                _productToEdit.HolidaysCalendar.Holidays.ToDictionary(it => it.HolidayDate);
            StringBuilder errors = _productToEdit.ExpiryCalendar.FuturesExpiries.Aggregate(
                new StringBuilder(),
                (invalidExpiries, expiry) =>
                {
                    if (holidays.ContainsKey(expiry.ExpiryDate))
                    {
                        invalidExpiries.AppendLine(ConstructInvalidExpiryError(expiry, "holiday"));
                    }
                    else if (expiry.ExpiryDate.IsWeekendDay())
                    {
                        invalidExpiries.AppendLine(ConstructInvalidExpiryError(expiry, "weekend"));
                    }

                    return invalidExpiries;
                });

            if (0 == errors.Length)
            {
                return true;
            }

            string errorMessage = $"The product cannot be saved because the following problems "
                                  + $"with its calendars have been detected:{Environment.NewLine}{errors}";

            MessageBox.Show(errorMessage, "Invalid Expiry Calendar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private string ConstructInvalidExpiryError(CalendarExpiryDate expiry, string dayType)
        {
            return $"- expiry date {expiry.ExpiryDate.ToShortDateString()} "
                   + $"for {expiry.FuturesDate:MM/yy} falls on a {dayType}";
        }

        private void onSave(object sender, EventArgs e)
        {
            UpdateRollOffSettings();
            _productToEdit.FuturesExpireTime = _rolloffSettings.FuturesExpireTime;

            if (!ValidateForm() || (!CheckForWarnings() && !IgnoreWarnings()))
            {
                productTabs.SelectedTabPageIndex = 0;
                return;
            }

            if (!ConfirmSave())
            {
                return;
            }

            SetCurrenciesBasedOnProductType(_productToEdit.Type);

            _productToEdit.Aliases?.Remove(p => string.IsNullOrWhiteSpace(p.Name));
            _productToEdit.IceProductsMappings?.Remove(p => p.IceProductId == 0);
            _productToEdit.CompaniesBrokerages?.Remove(p => p.CompanyId == 0);
            _productToEdit.ABNMappings?.Remove(abnMapping => !abnMapping.IsValid());
            _productToEdit.GmiBalmoCodes?.Remove(balmoCode => !balmoCode.IsValid());
            _productToEdit.IceBalmoMappings?.Remove(balmoMapping => !balmoMapping.IsValid());

            ClearZeroIdReferences();
            TrySaveProduct(ProductListForm.CreateAuditContext("Product Details"));
        }

        private void UpdateRollOffSettings()
        {
            _productToEdit.UsesRollOffSettings = _rolloffSettings.UseRollOffTime || _rolloffSettings.UseFuturesExpireTime;
            _productToEdit.RolloffTime = GetSelectedRollOffTime();
            _productToEdit.TimezoneId = GetSelectedTimeZoneId();
        }

        private DateTime? GetSelectedRollOffTime()
        {
            return _productToEdit.UsesRollOffSettings ? GetRollOffTimeForPersistence() : null;

            DateTime? GetRollOffTimeForPersistence()
            {
                return _productToEdit.TasType == TasType.NotTas ? GetRollOffTime() : _rolloffSettings.TasActivateTime;
            }

            DateTime? GetRollOffTime()
            {
                return ProductType.Futures == _productToEdit.Type ? null : _rolloffSettings.RollOffTime;
            }
        }

        private string GetSelectedTimeZoneId()
        {
            return ((_productToEdit.Type == ProductType.Futures || ProductType.Swap == _productToEdit.Type)
                    && _productToEdit.TasType != TasType.NotTas)
                ? _rolloffSettings.TasTimezoneId
                : (_productToEdit.UsesRollOffSettings ? _rolloffSettings.RollOffTimezoneId : null);
        }

        private bool ValidateForm()
        {
            productName.Focus();

            bool isValid = AllCurrenciesValid();

            isValid &= IsOfficialProductValid();
            isValid &= AreUnitsValid();
            isValid &= IsExchangeValid();
            //TODO is it required for the Underlying Futures
            isValid &= IsExpiryCalendarValid();
            isValid &= IsHolidaysCalendarValid();
            isValid &= IsProductCategoryValid();
            isValid &= IsContractSizeValid();
            isValid &= AreLegsValid();
            isValid &= AreFuturesSettingsValid();
            isValid &= AreRollOffSettingsValid();
            isValid &= IsTradeMonthSwapWithAllBusinessDayExpiries_SpecialForSomeReason();
            isValid &= AreBalmoSettingsValid();

            return isValid;
        }

        private bool IsOfficialProductValid()
        {
            officialProductSelector.ErrorText = _productToEdit.OfficialProduct == null
                ? "Official product is required"
                : null;
            return string.IsNullOrWhiteSpace(officialProductSelector.ErrorText);
        }

        private bool AreUnitsValid()
        {
            unitsSelector.ErrorText = _productToEdit.Unit == null ? "Units are required" : null;
            return string.IsNullOrWhiteSpace(unitsSelector.ErrorText);
        }

        private bool IsExchangeValid()
        {
            exchangeSelector.ErrorText = _productToEdit.Exchange == null ? "Exchange is required" : null;
            return string.IsNullOrWhiteSpace(exchangeSelector.ErrorText);
        }

        private bool IsExpiryCalendarValid()
        {
            expiryCalendar.ErrorText = _productToEdit.ExpiryCalendar == null ? "Expiry Calendar is required" : null;
            return string.IsNullOrWhiteSpace(expiryCalendar.ErrorText);
        }

        private bool IsHolidaysCalendarValid()
        {
            holidaysCalendar.ErrorText = _productToEdit.HolidaysCalendar == null
                ? "Holidays Calendar is required"
                : null;
            return string.IsNullOrWhiteSpace(holidaysCalendar.ErrorText);
        }

        private bool IsProductCategoryValid()
        {
            productCategorySelector.ErrorText = _productToEdit.Category == null ? "Category is required" : null;
            return string.IsNullOrWhiteSpace(productCategorySelector.ErrorText);
        }

        private bool IsContractSizeValid()
        {
            contractSize.ErrorText = contractSize.Text == string.Empty ? "Contract size is required" : null;
            return string.IsNullOrWhiteSpace(contractSize.ErrorText);
        }

        private bool AreLegsValid()
        {
            if (_productToEdit.Type != ProductType.Diff)
            {
                return true;
            }

            leg1ProductSelector.ErrorText = _productToEdit.ComplexProduct.ChildProduct1 == null
                ? "Leg 1 product is required"
                : null;
            leg2ProductSelector.ErrorText = _productToEdit.ComplexProduct.ChildProduct2 == null
                ? "Leg 2 product is required"
                : null;
            leg1PositionFactor.ErrorText = leg1PositionFactor.Text == string.Empty ? "Leg 1 factor is required" : null;
            leg2PositionFactor.ErrorText = leg2PositionFactor.Text == string.Empty ? "Leg 2 factor is required" : null;

            return string.IsNullOrWhiteSpace(leg1ProductSelector.ErrorText)
                   && string.IsNullOrWhiteSpace(leg2ProductSelector.ErrorText)
                   && string.IsNullOrWhiteSpace(leg1PositionFactor.ErrorText)
                   && string.IsNullOrWhiteSpace(leg2PositionFactor.ErrorText);
        }

        private bool AreFuturesSettingsValid()
        {
            if (_productToEdit.Type != ProductType.Futures)
            {
                return true;
            }

            if (expiryTypeGivenDateSelector.Checked)
            {
                givenDate.ErrorText = _productToEdit.GivenDate.HasValue ? null : "Given Date is required";
            }

            if (expTypeNumOfDaysSelector.Checked)
            {
                numberOfDaysSelector.ErrorText = _productToEdit.NumberOfDays.HasValue ? null : "Number of Days is required";
            }

            if (useFuturesExpirationSettings.Checked)
            {
                futuresExpiryTime.ErrorText = _rolloffSettings.FuturesExpireTime.HasValue
                    ? null
                    : "Futures expiry time is required";
            }

            if (_productToEdit.TasType != TasType.NotTas)
            {
                tasActivationTime.ErrorText = _rolloffSettings.TasActivateTime.HasValue
                    ? null
                    : "TAS activation time is required";
                tasTimezoneSelector.ErrorText = _rolloffSettings.TasTimezoneId != null ? null : "Timezone is required";
            }

            bool hasValidRollOffTime = _productToEdit.TasType != TasType.NotTas
                                       || (!_rolloffSettings.TasActivateTime.HasValue
                                           && null == _rolloffSettings.TasTimezoneId);

            return string.IsNullOrWhiteSpace(givenDate.ErrorText)
                   && string.IsNullOrWhiteSpace(numberOfDaysSelector.ErrorText)
                   && string.IsNullOrWhiteSpace(futuresExpiryTime.ErrorText)
                   && string.IsNullOrWhiteSpace(pricingEndTime.ErrorText)
                   && string.IsNullOrWhiteSpace(tasActivationTime.ErrorText)
                   && string.IsNullOrWhiteSpace(tasTimezoneSelector.ErrorText)
                   && hasValidRollOffTime;
        }

        private bool AreRollOffSettingsValid()
        {
            if (_productToEdit.Type == ProductType.Futures
                || (ProductType.Swap == _productToEdit.Type
                    || TasType.NotTas == _productToEdit.TasType
                    && !useRolloffSettings.Checked)
                || !useRolloffSettings.Checked)
            {
                return true;
            }

            rolloffTime.ErrorText = _rolloffSettings.RollOffTime.HasValue ? null : "Roll Off time is required";
            rolloffTimezoneSelector.ErrorText = GetTimeZoneErrorText();

            return string.IsNullOrWhiteSpace(rolloffTime.ErrorText)
                   && string.IsNullOrWhiteSpace(rolloffTimezoneSelector.ErrorText);
        }

        private string GetTimeZoneErrorText()
        {
            return GetTimeZoneId(_productToEdit) == null
                ? "Timezone is required"
                : null;
        }

        private string GetTimeZoneId(Product product)
        {
            return product.TasType == TasType.NotTas
                ? _rolloffSettings.RollOffTimezoneId
                : _rolloffSettings.TasTimezoneId;
        }

        private bool IsTradeMonthSwapWithAllBusinessDayExpiries_SpecialForSomeReason()
        {
            return _productToEdit.Type != ProductType.TradeMonthSwap || AreAllExpiryDatesOnBusinessDays();
        }

        private bool AreBalmoSettingsValid()
        {
            bool balmoUnderyingsValid = !(_productToEdit.Type == ProductType.Balmo
                                          && _productToEdit.BalmoOnCrudeProduct != null
                                          && _productToEdit.BalmoOnComplexProduct != null);
            string bothBalmoUnderlyingsErrorText = balmoUnderyingsValid
                ? string.Empty
                : "Balmo cannot have both underlyings. Please select either "
                  + "crude swap or complex product, or non of them.";

            balmoCrudeSwapsSelector.ErrorText = bothBalmoUnderlyingsErrorText;
            balmoComplexProductsSelector.ErrorText = bothBalmoUnderlyingsErrorText;
            return balmoUnderyingsValid;
        }

        private bool CheckForWarnings()
        {
            errorProvider.ClearErrors();

            bool allClear = AreUnitAndPositionFactorsValid();

            allClear &= null == _productToEdit.ComplexProduct || AreUnitAndLegFactorsValid();
            allClear &= AreMatchingExpiryAndRollOffTimeZones();
            return allClear;
        }

        private bool AreUnitAndPositionFactorsValid()
        {
            if (!UnitAndPositionFactorMatch())
            {
                decimal? defaultPosFactor = _productToEdit.Unit.DefaultPositionFactor;

                string unitsNotMatchPositionFactorErrorText = defaultPosFactor.HasValue
                    ? $"Selected Units default [{defaultPosFactor}] doesn't correspond to this conversion factor"
                    : "Selected Units default position factor doesn't correspond to this conversion factor";

                //errorProvider.SetError(unitsSelector, unitsNotMatchPositionFactorErrorText, ErrorType.Warning);
                errorProvider.SetError(positionFactor, unitsNotMatchPositionFactorErrorText, ErrorType.Warning);
                return false;
            }

            return true;
        }

        private bool UnitAndPositionFactorMatch()
        {
            return (_productToEdit.PositionFactor ?? 1M) == (_productToEdit.Unit.DefaultPositionFactor ?? 1M);
        }

        // I'd prefer to have a Dictionary<Control, Func<bool>> and Dictionary<Control, Action> for this.  Iterate over
        // the first and gather a collection of controls that are invalid.  Then execute the error handling Action for
        // each invalid control, separating the setting of the warning messages from determining whether there are
        // possible errors.
        private bool AreUnitAndLegFactorsValid()
        {
            bool conversionFactorsEqual = AreLegConversionFactorsEqual();

            if (!conversionFactorsEqual)
            {
                errorProvider.SetError(
                    leg1PnlFactor,
                    "Leg conversion factors are missing or not equal.",
                    ErrorType.Warning);
                errorProvider.SetError(
                    leg2PnlFactor,
                    "Leg conversion factors are missing or not equal.",
                    ErrorType.Warning);
            }

            bool leg1PositionFactorsMatchProductUnit = ProductUnitPositionFactorMatchesLeg1();

            if (!leg1PositionFactorsMatchProductUnit)
            {
                errorProvider.SetError(
                    leg1PositionFactor,
                    "Leg position factor doesn't correspond to product units position factor.",
                    ErrorType.Warning);
            }

            return conversionFactorsEqual && leg1PositionFactorsMatchProductUnit;
        }

        private bool AreLegConversionFactorsEqual()
        {
            return _productToEdit.ComplexProduct.ConversionFactor1 == _productToEdit.ComplexProduct.ConversionFactor2;
        }

        private bool ProductUnitPositionFactorMatchesLeg1()
        {
            return _productToEdit.Unit.DefaultPositionFactor == _productToEdit.ComplexProduct.ConversionFactor1;
        }

        private bool AreMatchingExpiryAndRollOffTimeZones()
        {
            if (!ExpiryCalendarTimezoneMatchRolloffTimezone())
            {
                string controlName = _productToEdit.Type == ProductType.Futures ? "TAS" : "roll-off";
                string message = $"Calendar and {controlName} timezones don't match.";

                errorProvider.SetError(expiryCalendar, message, ErrorType.Warning);
                errorProvider.SetError(
                    _productToEdit.Type == ProductType.Futures ? tasTimezoneSelector : rolloffTimezoneSelector,
                    message,
                    ErrorType.Warning);
                return false;
            }

            return true;
        }

        private bool IgnoreWarnings()
        {
            return MessageBox.Show("Ignore warnings?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                   == DialogResult.Yes;
        }

        private bool ExpiryCalendarTimezoneMatchRolloffTimezone()
        {
            if (!string.IsNullOrEmpty(_productToEdit.ExpiryCalendar?.Timezone)
                && !string.IsNullOrEmpty(_productToEdit.TimezoneId))
            {
                return _productToEdit.ExpiryCalendar.Timezone.Equals(_productToEdit.TimezoneId);
            }

            return true;
        }

        private void ClearZeroIdReferences()
        {
            if (_productToEdit.Currency1?.CurrencyId == 0)
            {
                _productToEdit.Currency1 = null;
            }

            if (_productToEdit.Currency2?.CurrencyId == 0)
            {
                _productToEdit.Currency2 = null;
            }

            if (_productToEdit.FeeClearingCurrency?.CurrencyId == 0)
            {
                _productToEdit.FeeClearingCurrency = null;
            }

            if (_productToEdit.FeeCommissionCurrency?.CurrencyId == 0)
            {
                _productToEdit.FeeCommissionCurrency = null;
            }

            if (_productToEdit.FeeExchangeCurrency?.CurrencyId == 0)
            {
                _productToEdit.FeeExchangeCurrency = null;
            }

            if (_productToEdit.FeeNfaCurrency?.CurrencyId == 0)
            {
                _productToEdit.FeeNfaCurrency = null;
            }

            if (_productToEdit.FeeCashCurrency?.CurrencyId == 0)
            {
                _productToEdit.FeeCashCurrency = null;
            }

            if (_productToEdit.FeeBlockCurrency?.CurrencyId == 0)
            {
                _productToEdit.FeeBlockCurrency = null;
            }

            if (_productToEdit.FeePlattsCurrency?.CurrencyId == 0)
            {
                _productToEdit.FeePlattsCurrency = null;
            }

            if (_productToEdit.UnderlyingFuturesOverride?.OfficialProductId == EmptyOfficialProductId)
            {
                _productToEdit.UnderlyingFuturesOverride = null;
            }

            if (_productToEdit.UnderlyingFutures?.OfficialProductId == EmptyOfficialProductId)
            {
                _productToEdit.UnderlyingFutures = null;
            }
        }

        private void TrySaveProduct(AuditContext auditContext)
        {
            try
            {
                UpdatedProductId = _productManager.SaveProduct(_productToEdit, auditContext);
                Close();
            }
            catch (IceProductMappingExistException ex)
            {
                XtraMessageBox.Show(
                    this,
                    $"Ice Product Mapping for Ice Product Id [{ex.IceProductId}] already exists: "
                    + $"mapped to a product [{ex.ProductName}].",
                    "Mapping Exists",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    this,
                    $"An error occured during product saving: {ex.Message}",
                    "Product Save Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool ConfirmSave()
        {
            return IsProductCodeKnownFromTradeInfo() && HasTraderConfirmedProduct();
        }

        private static bool IsProductCodeKnownFromTradeInfo()
        {
            return MessageBox.Show(
                       "Have you received the product code from the trade information?",
                       "Confirmation",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question)
                   == DialogResult.Yes;
        }

        private static bool HasTraderConfirmedProduct()
        {
            return MessageBox.Show(
                       "Have you confirm the product details with a trader?",
                       "Confirmation",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question)
                   == DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ProductTypeChanged(object sender, EventArgs e)
        {
            if (productTypeSelector.EditValue is DBNull)
            {
                // HACK - something is setting the value to DBNull when changing the pricing end time and, sometimes, 
                // when changing the futures expiry time.  Possibly other changes as well.
                // Allowing this to be left as DBNull results in AllCurrenciesValid returning false under circumstances 
                // for which it should return true.
                productTypeSelector.EditValue = _productToEdit.Type;
                return;
            }

            ProductType productType = (ProductType)productTypeSelector.EditValue;
            ProductType prevProdType = _productToEdit.Type;

            _productToEdit.Type = productType;

            if (productType != prevProdType)
            {
                _productToEdit.TasType = TasType.NotTas;
                isPlain.Checked = true;
            }

            SetDailyControls(productType);
            SetComplexControls(productType);
            SetExpirationControls(productType, prevProdType);
            SetCurrenciesControls(productType);

            if (productType == ProductType.Balmo)
            {
                balmoCrudeSwapsSelector.Enabled = true;
                balmoComplexProductsSelector.Enabled = true;

                if (balmoCrudeSwapsSelector.Properties.DataSource == null)
                {
                    balmoCrudeSwapsSelector.Properties.DataSource = _productManager.GetProducts_CrudeSwapsAndGasoilSwaps();
                }

                if (balmoComplexProductsSelector.Properties.DataSource == null)
                {
                    balmoComplexProductsSelector.Properties.DataSource = _productManager.GetProducts_Complex();
                }

                if (!testTradeStartDateTitle.Visible)
                {
                    testTradeStartDate.DataBindings.Clear();
                    testTradeStripSelector.DataBindings.Clear();

                    testTradeStartDateTitle.Visible = true;
                    testTradeStartDate.Visible = true;

                    testTradeStripTitle.Visible = false;
                    testTradeStripSelector.Visible = false;

                    TradeAddDetails.StripDetail1.Strip =
                        new Strip { IsBalmoStrip = true, StringValue = "Balmo", StartDate = DateTime.Today };

                    testTradeStartDate.DataBindings.Add("EditValue", TradeAddDetails.StripDetail1.Strip, "StartDate");
                }
            }
            else
            {
                _productToEdit.BalmoOnComplexProduct = null;
                _productToEdit.BalmoOnCrudeProduct = null;

                if (!testTradeStripTitle.Visible)
                {
                    testTradeStripSelector.DataBindings.Clear();
                    testTradeStartDate.DataBindings.Clear();

                    testTradeStripSelector.SelectedIndex = 0;

                    testTradeStartDateTitle.Visible = false;
                    testTradeStartDate.Visible = false;

                    testTradeStripTitle.Visible = true;
                    testTradeStripSelector.Visible = true;

                    TradeAddDetails.StripDetail1.Strip = testTradeStripSelector.EditValue as Strip;

                    testTradeStripSelector.DataBindings.Add("EditValue", TradeAddDetails.StripDetail1, "Strip");
                }
            }

            underlyingFuturesTitle.Text = "Underlying product:";
            underlyingFuturesTitle.Location = new Point(281, 53);

            enableRiskDecomposition.Enabled = productType == ProductType.TradeMonthSwap;

            if (productType != ProductType.TradeMonthSwap && _productToEdit.Type != productType)
            {
                _productToEdit.IsEnabledRiskDecomposition = false;
                enableRiskDecomposition.Checked = false;
            }
        }

        private void SetCurrenciesControls(ProductType productType)
        {
            bool isSpot = productType == ProductType.Spot;

            currency1Selector.Enabled = isSpot;
            currency1Title.Enabled = isSpot;
            currency2Selector.Enabled = isSpot;
            currency2Title.Enabled = isSpot;

            SetCurrenciesBasedOnProductType(productType);
            AllCurrenciesValid();
        }

        private bool AllCurrenciesValid()
        {
            if (productTypeSelector.EditValue == null || productTypeSelector.EditValue is DBNull)
            {
                return false;
            }

            if ((ProductType)productTypeSelector.EditValue != ProductType.Spot)
            {
                currency1Selector.ErrorText = null;
                currency2Selector.ErrorText = null;
                return true;
            }

            return AreSpotTradeCurrenciesValid();
        }

        private bool AreSpotTradeCurrenciesValid()
        {
            Currency currency1 = this.currency1Selector.EditValue as Currency;
            Currency currency2 = currency2Selector.EditValue as Currency;

            return IsCurrencyValid(this.currency1Selector, "1")
                   && IsCurrencyValid(currency2Selector, "2")
                   && !AreSpotCurrenciesTheSame(currency1, currency2);
        }

        private bool IsCurrencyValid(LookUpEdit currEdit, string currNumber)
        {
            Currency currency = currEdit.EditValue as Currency;

            return null
                   == (currEdit.ErrorText = !IsNullOrEmptyCurrency(currency)
                       ? $"Currency{currNumber} is required for Spot products"
                       : null);
        }

        private bool IsNullOrEmptyCurrency(Currency currency)
        {
            return currency == null || 0 == currency.CurrencyId;
        }

        private bool AreSpotCurrenciesTheSame(Currency currency1, Currency currency2)
        {
            if (currency1 == null || currency2 == null)
            {
                return false;
            }

            bool currenciesAreTheSame = currency1.Equals(currency2);

            this.currency1Selector.ErrorText = currency2Selector.ErrorText =
                currenciesAreTheSame ? "Currencies must be different" : null;
            return currenciesAreTheSame;
        }

        private void SetCurrenciesBasedOnProductType(ProductType productType)
        {
            if (productType == ProductType.Spot)
            {
                _productToEdit.Currency1 = currency1Selector.EditValue as Currency;
                _productToEdit.Currency2 = currency2Selector.EditValue as Currency;
            }
            else
            {
                _productToEdit.Currency1 = null;
                _productToEdit.Currency2 = null;
                currency1Selector.EditValue = null;
                currency2Selector.EditValue = null;
            }
        }

        private void SetDailyControls(ProductType productType)
        {
            monthlyOfficialProductTitle.Visible = productType == ProductType.DailySwap;
            monthlyOfficialProductSelector.Visible = productType == ProductType.DailySwap;

            bool isDailyOrWeeklyDiff = productType.IsDailyOrWeeklyDiff();
            dailyDiffMonthShift.Visible = isDailyOrWeeklyDiff;
            dailyDiffMonthShiftTitle.Visible = isDailyOrWeeklyDiff;
            if (!isDailyOrWeeklyDiff)
            {
                _productToEdit.DailyDiffMonthShift = Product.NoDailyDiffMonthShift;
            }
        }

        private void SetExpirationControls(ProductType productTp, ProductType prevProductTp)
        {
            SetCalendarDaySwapControls(productTp);
            SetProductTypeSpecificControls(productTp);
            SetRollOffAndExpiryControls(prevProductTp, productTp);
        }

        private void SetCalendarDaySwapControls(ProductType productTp)
        {
            bool isSwap = productTp == ProductType.Swap;

            isCalendarDaySwap.Visible = isSwap;
            isCalendarDaySwap.DataBindings.Clear();

            if (isSwap)
            {
                isCalendarDaySwap.DataBindings.Add("EditValue", _productToEdit, "IsCalendarDaySwap", true);
            }
            else
            {
                _productToEdit.IsCalendarDaySwap = false;
            }
        }

        private void SetProductTypeSpecificControls(ProductType productTp)
        {
            bool isFutures = productTp == ProductType.Futures;

            if (isFutures)
            {
                SetForFutures();
            }
            else
            {
                SetForNonFutures();
            }

            bool isSwap = ProductType.Swap == productTp;

            tasTypeSelectors.Enabled = isFutures || isSwap || productTp == ProductType.DailySwap;

            if (!tasTypeSelectors.Enabled)
            {
                isPlain.Checked = true;
            }
            else
            {
                isTas.Enabled = isMinuteMarker.Enabled = isMops.Enabled = !isSwap;
                isMoc.Enabled = isSwap;
            }
        }

        private void SetForFutures()
        {
            rolloffSettings.Visible = false;
            futuresExpirationSettings.Visible = true;
            alsoIsTas.Enabled = true;
        }

        private void SetForNonFutures()
        {
            rolloffSettings.Visible = true;
            futuresExpirationSettings.Visible = false;
            alsoIsTas.Enabled = false;
        }

        private void SetRollOffAndExpiryControls(ProductType prevProductTp, ProductType productTp)
        {
            bool isFutures = productTp == ProductType.Futures;

            tasActivationTime.DataBindings.Clear();
            rolloffTime.DataBindings.Clear();
            tasTimezoneSelector.DataBindings.Clear();
            rolloffTimezoneSelector.DataBindings.Clear();
            mocActivationTime.DataBindings.Clear();
            mocActivationTimezoneSelector.DataBindings.Clear();

            if (isFutures)
            {
                SwitchRollOffAndExpiryToFutures();
                SwitchToTasActivationTime();
            }
            else
            {
                SwitchRollOffAndExpiryToNonFutures();
                SwitchToRollOffTime();
            }
        }

        private void SwitchToTasActivationTime()
        {
            rolloffTime.EditValue = null;
            rolloffTimezoneSelector.EditValue = null;
            tasActivationTime.EditValue = _rolloffSettings.TasActivateTime;

            tasActivationTime.DataBindings.Add(
                "EditValue",
                _rolloffSettings,
                "TasActivateTime",
                true,
                DataSourceUpdateMode.OnPropertyChanged,
                new DateTime(1900, 1, 1));
            tasTimezoneSelector.DataBindings.Add(
                "EditValue",
                _rolloffSettings,
                "TasTimezone",
                true,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void SwitchToRollOffTime()
        {
            tasActivationTime.EditValue = null;
            tasTimezoneSelector.EditValue = null;
            rolloffTime.EditValue = _rolloffSettings.RollOffTime;

            rolloffTime.DataBindings.Add(
                "EditValue",
                _rolloffSettings,
                "RollOffTime",
                true,
                DataSourceUpdateMode.OnPropertyChanged,
                new DateTime(1900, 1, 1));
            rolloffTimezoneSelector.DataBindings.Add(
                "EditValue",
                _rolloffSettings,
                "RollOffTimezone",
                true,
                DataSourceUpdateMode.OnPropertyChanged);
            mocActivationTimezoneSelector.DataBindings.Add(
                "EditValue",
                _rolloffSettings,
                "TasTimezone",
                true,
                DataSourceUpdateMode.OnPropertyChanged);
            mocActivationTime.DataBindings.Add(
                "EditValue",
                _rolloffSettings,
                "TasActivateTime",
                true,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void SwitchRollOffAndExpiryToFutures()
        {
            _rolloffSettings.UseRollOffTime = false;
            useRolloffSettings.Checked = false;
            useFuturesExpirationSettings.Checked = _rolloffSettings.UseFuturesExpireTime;
        }

        private void SwitchRollOffAndExpiryToNonFutures()
        {
            _rolloffSettings.UseFuturesExpireTime = false;
            useFuturesExpirationSettings.Checked = false;
            useRolloffSettings.Checked = _rolloffSettings.UseRollOffTime;
        }

        private void SetComplexControls(ProductType productType)
        {
            bool isComplex = productType.IsDiff();

            if (!isComplex)
            {
                leg1PositionFactor.DataBindings.Clear();
                leg2PositionFactor.DataBindings.Clear();
                leg1PnlFactor.DataBindings.Clear();
                leg2PnlFactor.DataBindings.Clear();
                leg1ProductSelector.DataBindings.Clear();
                leg2ProductSelector.DataBindings.Clear();
                useCommonPricing.DataBindings.Clear();

                _productToEdit.ClearComplexProductReference();
            }
            else
            {
                _productToEdit.SetComplexProductReference();

                if (leg1PositionFactor.DataBindings.Count == 0)
                {
                    leg1PositionFactor.DataBindings.Add(
                        "EditValue",
                        _productToEdit.ComplexProduct,
                        "ConversionFactor1",
                        true);
                    leg2PositionFactor.DataBindings.Add(
                        "EditValue",
                        _productToEdit.ComplexProduct,
                        "ConversionFactor2",
                        true);
                    leg1PnlFactor.DataBindings.Add("EditValue", _productToEdit.ComplexProduct, "PnlFactor1", true);
                    leg2PnlFactor.DataBindings.Add("EditValue", _productToEdit.ComplexProduct, "PnlFactor2", true);
                    leg1ProductSelector.DataBindings.Add("EditValue", _productToEdit.ComplexProduct, "ChildProduct1", true);
                    leg2ProductSelector.DataBindings.Add("EditValue", _productToEdit.ComplexProduct, "ChildProduct2", true);
                    useCommonPricing.DataBindings.Add(
                        "EditValue",
                        _productToEdit.ComplexProduct,
                        "IsCommonPricing",
                        true);
                }
            }

            leg1PositionFactor.Enabled = isComplex;
            leg2PositionFactor.Enabled = isComplex;
            leg1PnlFactor.Enabled = isComplex;
            leg2PnlFactor.Enabled = isComplex;
            leg1ProductSelector.Enabled = isComplex;
            leg2ProductSelector.Enabled = isComplex;
            useCommonPricing.Enabled = isComplex;
            treatTimeSpreadStripAsLegs.Enabled = isComplex;
            calculatePnlFromLegs.Enabled = isComplex;

            if (isComplex)
            {
                bool isDaily = productType.IsDailyOrWeeklyDiff();

                if (isDaily)
                {
                    List<Product> productsSource = _productManager
                        .GetProducts()
                        .Where(p => p.Type == ProductType.DailySwap)
                        .ToList();
                    leg1ProductSelector.Properties.DataSource = productsSource;
                    leg2ProductSelector.Properties.DataSource = productsSource.ToList();
                }
                else
                {
                    List<Product> productsSource = _productManager.GetProducts();
                    leg1ProductSelector.Properties.DataSource = productsSource;
                    leg2ProductSelector.Properties.DataSource = productsSource.ToList();
                }
            }
        }

        private void gvAliases_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            ProductAlias newAlias = (ProductAlias)productAliasesDisplay.GetRow(e.RowHandle);
            newAlias.Product = _productToEdit;
        }

        private void ProductDetailsForm_Load(object sender, EventArgs e)
        {
            _calendarCheck = new CalendarClashCheck();
            SetProductDataBindings(_productManager.GetCurrencies());

            if (!AuthorizationService.IsUserAuthorizedTo(
                ProductListForm.AuthorizedUser,
                PermissionType.ProductMgmtToolWriteAccess))
            {
                productAliasesDisplay.OptionsBehavior.Editable = false;
                MakeEditControlsReadOnly(Controls);

                save.Visible = false;
                cancel.Text = "Close";

                return;
            }

            UpdateTasActivationTimeControls();
            UpdateRollOffSettingsControls();
            UpdateMocActivationTimeControls();
            testTradeStripSelector.SelectedIndex = 0;

            IObservable<EventPattern<EventArgs>> observalbeImpact = Observable
                .FromEventPattern<EventArgs>(TradeAddDetails, "ImpactChange")
                .Throttle(TimeSpan.FromSeconds(.7));

            _observeImpact = observalbeImpact.ObserveOn(SynchronizationContext.Current)
                                             .Subscribe(
                                                 _ =>
                                                 {
                                                     if (IsProductValidForImpactCalculation())
                                                     {
                                                         TradeAddDetails.OfficialProductId =
                                                             _productToEdit.OfficialProduct.OfficialProductId;
                                                         ProductListForm.BusClient.GetTradeAddImpact(
                                                             TradeAddDetails,
                                                             ImpactLoaded,
                                                             ImpactLoadFailed);
                                                     }
                                                 });

            SetUpTas();
            SetNumericMasks();
        }

        private void SetUpTas()
        {
            tasTypeSelectors.Visible = true;
            alsoIsTas.Visible = false;
            tasOfficialProductTitle.Visible = true;
            tasOfficialProductSelector.Visible = true;

            switch (_productToEdit.TasType)
            {
                case TasType.Tas:
                {
                    if (ProductType.Swap == _productToEdit.Type)
                    {
                        isMoc.Checked = true;
                    }
                    else
                    {
                        isTas.Checked = true;
                    }
                }
                break;

                case TasType.Mops:
                {
                    isMops.Checked = true;
                }
                break;

                case TasType.Mm:
                {
                    isMinuteMarker.Checked = true;
                }
                break;

                case TasType.NotTas:
                {
                    isPlain.Checked = true;
                }
                break;
            }
        }

        private void SetNumericMasks()
        {
            EditMask.SetNonZeroFloatMask(pnlFactor);
            EditMask.SetNonZeroFloatMask(leg1PnlFactor);
            EditMask.SetNonZeroFloatMask(leg2PnlFactor);

            EditMask.SetNonZeroFloatMask(positionFactor);
            EditMask.SetNonZeroFloatMask(leg1PositionFactor);
            EditMask.SetNonZeroFloatMask(leg2PositionFactor);

            EditMask.SetNonZeroFloatMask(priceConversionFactor);
            EditMask.SetNonZeroFloatMask(contractSize);

            EditMask.SetTwoDecimalFloatMask(commisionFee);
            EditMask.SetTwoDecimalFloatMask(exchangeFee);
            EditMask.SetTwoDecimalFloatMask(nfaFee);
            EditMask.SetTwoDecimalFloatMask(clearingFee);
            EditMask.SetTwoDecimalFloatMask(blockFee);
            EditMask.SetTwoDecimalFloatMask(cashFee);
            EditMask.SetTwoDecimalFloatMask(plattsFee);
        }

        private bool IsProductValidForImpactCalculation()
        {
            if (_productToEdit.OfficialProduct == null || _productToEdit.ExpiryCalendar == null
                || _productToEdit.HolidaysCalendar == null || _productToEdit.Category == null
                || _productToEdit.Exchange == null)
            {
                return false;
            }

            if (_productToEdit.ContractSize == 0M || TradeAddDetails.StripDetail1.Volume == 0M)
            {
                return false;
            }

            if (_productToEdit.Type == ProductType.Diff)
            {
                if (_productToEdit.ComplexProduct?.ConversionFactor1 == null
                    || _productToEdit.ComplexProduct.ConversionFactor2 == null)
                {
                    return false;
                }

                if (_productToEdit.ComplexProduct.ChildProduct1 == null
                    || _productToEdit.ComplexProduct.ChildProduct2 == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void TradeImpact_Load()
        {
            _uiSynchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

            testTradeStripSelector.Properties.Items.Clear();
            testTradeStripSelector.Properties.Items.AddRange(
                _stripGenerator.GenerateStrips(StripPeriodsToInclude.QsAndCals));

            TradeAddDetails = new TradeAddDetails
            {
                TradeType = TradeTypeControl.Manual,
                Side = SideControl.Buy,
                Broker = "Internal",
                StripTypeControl = StripTypeControl.Flat,
                Product = _productToEdit,
            };

            TradeAddDetails.StripDetail1 = new StripDetail(TradeAddDetails);
            BindTradeImpactUi();
        }

        private void BindTradeImpactUi()
        {
            TradeAddDetails.StripDetail1.Unit = _productToEdit.Unit;

            testTradePrice.DataBindings.Add(
                "Text",
                TradeAddDetails.StripDetail1,
                "Price",
                true,
                DataSourceUpdateMode.OnPropertyChanged);
            testTradeLivePrice.DataBindings.Add("EditValue", TradeAddDetails.StripDetail1, "LivePrice");
            testTradeVolume.DataBindings.Add(
                "Text",
                TradeAddDetails.StripDetail1,
                "Volume",
                true,
                DataSourceUpdateMode.OnPropertyChanged);
            testTradeStripSelector.DataBindings.Add(
                "EditValue",
                TradeAddDetails.StripDetail1,
                "Strip",
                true,
                DataSourceUpdateMode.OnPropertyChanged);
        }

        private void MakeEditControlsReadOnly(Control.ControlCollection controls)
        {
            foreach (object control in controls)
            {
                MakeControlReadOnly(control);
            }
        }

        private void MakeControlReadOnly(object control)
        {
            switch (control)
            {
                case BaseEdit baseEdit:
                {
                    baseEdit.Properties.ReadOnly = true;
                }
                break;

                case GroupControl groupControl:
                {
                    MakeEditControlsReadOnly(groupControl.Controls);
                }
                break;
            }
        }

        private void OnUseRollOffSettingsChanged(object sender, EventArgs e)
        {
            _rolloffSettings.UseRollOffTime = useRolloffSettings.Checked;

            UpdateRollOffSettingsControls();
        }

        private void UpdateRollOffSettingsControls()
        {
            DisableRollOffSettings();

            if (TasType.NotTas == _productToEdit.TasType)
            {
                EnableForRollOffTime();
            }
            else
            {
                EnableForMocActivationTime();
            }
        }

        private void DisableRollOffSettings()
        {
            bool isTasType = _productToEdit.TasType != TasType.NotTas;

            if (isTasType)
            {
                useRolloffSettings.Checked = false;
                useRolloffSettings.Enabled = false;
                isCalendarDaySwap.Checked = false;
                isCalendarDaySwap.Enabled = false;
                _productToEdit.IsCalendarDaySwap = false;
            }

            DisableRollOffTimeZone();

            rolloffTime.Enabled = false;
            localRollOffTimeTitle.Visible = false;
            localRollOffTime.Visible = false;
        }

        private void DisableRollOffTimeZone()
        {
            rolloffTimezoneSelector.Enabled = false;
            SetSelectedRollOffTimeZone(GetTimeZoneSelector());
        }

        private ComboBoxEdit GetTimeZoneSelector()
        {
            return TasType.NotTas == _productToEdit.TasType ? rolloffTimezoneSelector : mocActivationTimezoneSelector;
        }

        private void SetSelectedRollOffTimeZone(ComboBoxEdit timeZoneSelector)
        {
            timeZoneSelector.SelectedItem = GetSelectedRollOffTimeZone();
        }

        private TimeZoneInfo GetSelectedRollOffTimeZone()
        {
            return (TasType.NotTas == _productToEdit.TasType
                       ? _rolloffSettings.RollOffTimezone
                       : _rolloffSettings.TasTimezone)
                   ?? TimeZoneInfo.Local;
        }

        private void EnableForRollOffTime()
        {
            useRolloffSettings.Checked = _rolloffSettings.UseRollOffTime;
            useRolloffSettings.Enabled = true;
            isCalendarDaySwap.Checked = _productToEdit.IsCalendarDaySwap;
            isCalendarDaySwap.Enabled = true;
            rolloffTime.Enabled = _rolloffSettings.UseRollOffTime;
            _rolloffSettings.RollOffTime = _rolloffSettings.RollOffTime ?? DefaultMinDate;
            rolloffTime.EditValue = _rolloffSettings.RollOffTime;
            localRollOffTimeTitle.Visible = true;
            localRollOffTime.Visible = true;
            rolloffTimezoneSelector.Enabled = _rolloffSettings.UseRollOffTime;
            rolloffTimezoneSelector.Visible = true;
            mocActivationTimezoneSelector.Enabled = false;
            mocActivationTimezoneSelector.Visible = false;
            mocActivationTime.Enabled = false;

            if (_rolloffSettings.UseRollOffTime)
            {
                EnableRollOffTimeZone();
            }
        }

        private void EnableRollOffTimeZone()
        {
            rolloffTimezoneSelector.Enabled = true;
            SetSelectedRollOffTimeZone(GetTimeZoneSelector());
        }

        private void EnableForMocActivationTime()
        {
            useRolloffSettings.Checked = false;
            useRolloffSettings.Enabled = false;
            rolloffTimezoneSelector.Visible = false;
            mocActivationTimezoneSelector.Enabled = true;
            mocActivationTimezoneSelector.Visible = true;
            mocActivationTime.Enabled = true;
            _rolloffSettings.TasActivateTime = _rolloffSettings.TasActivateTime ?? DefaultMinDate;
            mocActivationTime.EditValue = _rolloffSettings.TasActivateTime;
            EnableRollOffTimeZone();
        }

        private void gridView1_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            ProductBrokerage brokerage = (ProductBrokerage)companyBrokerageDisplay.GetRow(e.RowHandle);

            brokerage.Product = _productToEdit;
        }

        private void rbExpirationType_CheckedChanged(object sender, EventArgs e)
        {
            if (expiryTypeCalendarSelector.Checked)
            {
                _productToEdit.ExpirationType = ExpirationType.Calendar;
            }

            if (expiryTypeGivenDateSelector.Checked)
            {
                _productToEdit.ExpirationType = ExpirationType.GivenDate;
            }

            if (expTypeNumOfDaysSelector.Checked)
            {
                _productToEdit.ExpirationType = ExpirationType.NumberOfDays;
            }

            SetExpirationSettingsPanelsVisibility(_productToEdit.ExpirationType);
        }

        private void SetExpirationSettingsPanelsVisibility(ExpirationType expirationType)
        {
            switch (expirationType)
            {
                case ExpirationType.Calendar:
                expirationMonthSection.Visible = true;
                givenDateSection.Visible = false;
                numberOfDaysSection.Visible = false;
                break;

                case ExpirationType.GivenDate:
                givenDateSection.Visible = true;
                expirationMonthSection.Visible = false;
                numberOfDaysSection.Visible = false;
                break;

                case ExpirationType.NumberOfDays:
                expirationMonthSection.Visible = true;
                numberOfDaysSection.Visible = true;
                givenDateSection.Visible = false;
                break;
            }
        }

        private void rbRollingMethod_CheckedChanged(object sender, EventArgs e)
        {
            if (isRollingForward.Checked)
            {
                _productToEdit.RollingMethod = RollingMethod.Forward;
            }

            if (isRollingBackward.Checked)
            {
                _productToEdit.RollingMethod = RollingMethod.Backward;
            }
        }

        private void gvMappings_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            ABNMappings newMapping = (ABNMappings)abnMappingsDisplay.GetRow(e.RowHandle);
            newMapping.Product = _productToEdit;
        }

        private void gvGmiBalmoCodes_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            GmiBalmoCode code = (GmiBalmoCode)gmiBalmoCodesDisplay.GetRow(e.RowHandle);
            code.Product = _productToEdit;
        }

        private void ImpactLoadFailed(FailureCallbackInfo info)
        {
            Invoke(
                new Action(
                    () =>
                    {
                        XtraMessageBox.Show(
                            this,
                            "Cannot load trade impact.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }));
        }

        private void ImpactLoaded(TradeAddImpactResponseMessage message)
        {
            if (message?.TradesImpact == null)
            {
                return;
            }

            TradeAddImpact impact = message.TradesImpact;

            Task.Factory.StartNew(
                () =>
                {
                    TradeAddDetails.StripDetail1.LivePrice = impact.LivePrice1;

                    impact.Positions.ForEach(x => x.Amount = x.Amount * 0.001M);

                    testTradePnL.EditValue = impact.Pnl;

                    testTradeImpact.DataSource = impact.Positions;
                    testTradeImpact.CollapseAllColumns();
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                _uiSynchronizationContext);
        }

        private void txtVol_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void cmbStrip_SelectedIndexChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void txtPrice_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void deStartDate_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void leExchange_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.Exchange = exchangeSelector.Text;
        }

        private void txtContractSize_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void HolidaysCalendarChanged(object sender, EventArgs e)
        {
            (sender as LookUpEdit).DoValidate();
        }

        private void SelectedCalendarValidating(object sender, CancelEventArgs cancelEvent)
        {
            bool calendarsClash = AreExpiryAndHolidayCalendarsClashing();

            cancelEvent.Cancel = calendarsClash;
            (sender as LookUpEdit).ErrorText = calendarsClash ? "The selected expiry and holiday calendars clash" : "";
        }

        private void ExpiryCalendarChanged(object sender, EventArgs ignoreArgs)
        {
            LookUpEdit expiryCalendar = sender as LookUpEdit;

            if (expiryCalendar.DoValidate())
            {
                TradeAddDetails.FireImpactChange();
            }
        }

        private bool AreExpiryAndHolidayCalendarsClashing()
        {
            StockCalendar holidays = holidaysCalendar.EditValue as StockCalendar ?? StockCalendar.Default;
            StockCalendar expiries = expiryCalendar.EditValue as StockCalendar ?? StockCalendar.Default;

            return _calendarCheck.DoCalendarsClash(holidays, expiries);
        }

        private void txtPnlFactor_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void txtPositionFactor_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void leProductLeg1_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void leProductLeg2_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void txtLeg1Factor_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void txtLeg2Factor_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void gridView2_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            IceProductMapping newAlias = (IceProductMapping)iceAliasesDisplay.GetRow(e.RowHandle);
            newAlias.UpdatedAt = DateTime.UtcNow;

            newAlias.Product = _productToEdit;
        }

        private void leBalmoCrudeSwaps_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void leBalmoComplexProducts_EditValueChanged(object sender, EventArgs e)
        {
            TradeAddDetails.FireImpactChange();
        }

        private void chkUseFuturesExpirationSettings_CheckedChanged(object sender, EventArgs e)
        {
            _rolloffSettings.UseFuturesExpireTime = useFuturesExpirationSettings.Checked;

            if (useFuturesExpirationSettings.Checked)
            {
                EnableFuturesExpirationSettings();
            }
            else
            {
                DisableFuturesExpirationSettings();
            }
        }

        private void EnableFuturesExpirationSettings()
        {
            futuresExpiryTime.Enabled = true;
            pricingEndTime.Enabled = true;
        }

        private void DisableFuturesExpirationSettings()
        {
            _rolloffSettings.FuturesExpireTime = null;
            _productToEdit.PricingEndTime = null;

            futuresExpiryTime.Enabled = false;
            pricingEndTime.Enabled = false;
            futuresExpiryTime.EditValue = new DateTime(1900, 1, 1);
            pricingEndTime.EditValue = new DateTime(1900, 1, 1);
        }

        private void ChangeContractSubType(object sender, EventArgs e)
        {
            if (isTas.Checked)
            {
                _productToEdit.TasType = TasType.Tas;
            }

            if (isMops.Checked)
            {
                _productToEdit.TasType = TasType.Mops;
            }

            if (isMinuteMarker.Checked)
            {
                _productToEdit.TasType = TasType.Mm;
            }

            if (isPlain.Checked)
            {
                _productToEdit.TasType = TasType.NotTas;
                UpdateRollOffSettingsControls();
            }

            if (isMoc.Checked)
            {
                _productToEdit.TasType = TasType.Tas;
                UpdateRollOffSettingsControls();
                //DisableRollOffSettings();
                EnableRollOffTimeZone();
            }

            UpdateTasActivationTimeControls();
            UpdateMocActivationTimeControls();
        }

        private void UnitsChanged(object sender, EventArgs e)
        {
            if (unitsSelector.EditValue is DBNull)
            {
                unitsSelector.EditValue = _productToEdit.Unit;
                return;
            }

            Unit selectedUnit = unitsSelector.EditValue as Unit;

            TradeAddDetails.StripDetail1.Unit = _productToEdit.Unit;

            if (null != selectedUnit && selectedUnit.AllowOnlyMonthlyContractSize)
            {
                contractSizeMultiplier.EditValue = ContractSizeMultiplier.Monthly;
            }

            SetContractSizeMultiplierEnabledState();
        }

        private void gvIceBalmoMappings_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            IceBalmoMapping mapping = (IceBalmoMapping)iceBalmoMappingsDisplay.GetRow(e.RowHandle);
            mapping.Product = _productToEdit;
        }

        private void leCurrency_EditValueChanged(object sender, EventArgs e)
        {
            AllCurrenciesValid();
        }
    }
}