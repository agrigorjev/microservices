using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Mandara.Business;
using Mandara.Business.Audit;
using Mandara.Business.Managers.Calendars;
using Mandara.Entities;
using Mandara.Entities.Enums;
using Mandara.Extensions.Collections;
using NLog;
using TextEdit = DevExpress.XtraEditors.TextEdit;

namespace Mandara.ProductGUI.Calendars
{
    public partial class ExpiryDatesControl : XtraUserControl
    {
        private class NeighbourDates
        {
            public (DateTime date, int month) Previous { get; }
            public (DateTime date, int month) Next { get; }

            public static readonly (DateTime date, int month) DefaultPrevious = (DateTime.MinValue, 0);
            public static readonly (DateTime date, int month) DefaultNext = (DateTime.MaxValue, 0);

            public NeighbourDates((DateTime, int) previous, (DateTime, int) next)
            {
                Previous = previous;
                Next = next;
            }
        }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private const string CALENDAR_ID = "CALENDAR_ID";
        private const string NAME = "NAME";
        private const int CalendarNameColumn = 1;
        private const string TIMEZONE = "TIMEZONE";
        private const string CORRECTION = "CORRECTION";

        private const string DATE_FORMAT = "dd/MM";
        private int currentYear = -1;

        private ProductManager _productManager;
        private string[] monthNames = MonthNames.GetMonthNames();
        private List<TimeZoneInfo> _timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();

        private readonly CalendarClashCheck _holidays;
        private const int InvalidColumn = -1;
        private int _calendarCol = InvalidColumn;
        private const int NonDateInitialItems = 4;
        private const int MonthsInYear = 12;
        private HashSet<int> _invalidDates = new HashSet<int>();
        private static readonly Color InvalidExpiry = Color.Yellow;

        public event EventHandler ExpiryDateChanged;

        private int CalendarIdCol
        {
            get
            {
                if (InvalidColumn == _calendarCol)
                {
                    _calendarCol = GridDataTable?.Columns?.IndexOf(CALENDAR_ID) ?? InvalidColumn;

                }

                return _calendarCol;
            }
        }

        public ExpiryDatesControl()
        {
            InitializeComponent();
            _holidays = new CalendarClashCheck();
            SetGrid();
        }

        public void SetProductManager(ProductManager productManager)
        {
            _productManager = productManager;
            _productManager.RegisterForHolidaysSaved(OnCalendarHolidaysSaved);
        }

        public void SetYear(int year)
        {
            expiryDates.DataSource = GetGridDataTable(year);
            currentYear = year;
        }

        public bool Save()
        {
            return HasChanges && SaveChanges();
        }

        private bool SaveChanges()
        {
            CalendarChanges<NewExpiryCalendar> calendarUpdates = GetExpiryDatesFromGrid();
            AuditContext audit = ProductListForm.CreateAuditContext("Expiry dates");

            _productManager.UpdateExpiryCalendars(calendarUpdates, currentYear, audit);
            expiryDates.DataSource = GetGridDataTable(currentYear);
            return true;
        }

        public void RejectChanges()
        {
            GridDataTable?.RejectChanges();
        }

        public bool HasChanges => GridDataTable != null && GridDataTable.GetChanges() != null;

        /// <summary>
        /// Indicates if editing available for control
        /// </summary>
        public bool AllowChanges { get; set; }

        public bool IsValid =>
            !_invalidDates.Any()
            && (Enumerable.Range(0, expiryDatesDisplay.RowCount).All(
                product =>
                {
                    DataRow expiryCalendar = expiryDatesDisplay.GetDataRow(product)
                                             ?? expiryDatesDisplay.GetDataRow(GridControl.NewItemRowHandle);

                    return null != expiryCalendar
                           && !expiryCalendar.HasErrors
                           && !String.IsNullOrWhiteSpace(expiryCalendar.ItemArray[CalendarNameColumn].ToString());
                }));

        private CalendarChanges<NewExpiryCalendar> GetExpiryDatesFromGrid()
        {
            List<NewExpiryCalendar> added = new List<NewExpiryCalendar>();
            List<int> deleted = new List<int>();
            List<StockCalendar> modified = new List<StockCalendar>();

            foreach (DataRow expiryData in GridDataTable.Rows)
            {
                switch (expiryData.RowState)
                {
                    case DataRowState.Added:
                    {
                        added.Add(AddCalendar(expiryData));
                    }
                    break;

                    case DataRowState.Deleted:
                    {
                        deleted.Add((int)expiryData[CALENDAR_ID, DataRowVersion.Original]);
                    }
                    break;

                    case DataRowState.Modified:
                    {
                        modified.Add(UpdateExistingCalendar(expiryData));
                    }
                    break;
                }
            }

            return new CalendarChanges<NewExpiryCalendar>(modified, added, deleted);
        }

        private NewExpiryCalendar AddCalendar(DataRow expiryData)
        {
            List<CalendarExpiryDate> expiryDates = GetExpiryDates(expiryData);
            int? correctionInfo = null;

            if (!expiryData.IsNull(CORRECTION))
            {
                correctionInfo = (int)expiryData[CORRECTION];
            }

            return new NewExpiryCalendar(
                expiryData[NAME].ToString(),
                correctionInfo,
                expiryData[TIMEZONE] as TimeZoneInfo,
                expiryDates);
        }

        private List<CalendarExpiryDate> GetExpiryDates(DataRow productExpiriesForYear)
        {
            return Enumerable
                   .Range(1, MonthsInYear)
                   .Where(
                       monthIndex =>
                           !productExpiriesForYear.IsNull(GetUppercaseExpiryGridCol(monthIndex)))
                   .Select(monthIndex => GetCalendarExpiryDate(productExpiriesForYear, monthIndex))
                   .Where(expiry => null != expiry)
                   .ToList();
        }

        private CalendarExpiryDate GetCalendarExpiryDate(DataRow expiryData, int monthIndex)
        {
            string expiryDateColumnName = GetUppercaseExpiryGridCol(monthIndex);

            return new CalendarExpiryDate()
            {
                ExpiryDate = (DateTime)expiryData[expiryDateColumnName],
                FuturesDate = new DateTime(currentYear, monthIndex, 1)
            };
        }

        private StockCalendar UpdateExistingCalendar(DataRow productExpiriesForYear)
        {
            int? correctionInfo = null;
            TimeZoneInfo expiryDateTimeZoneInfo = productExpiriesForYear[TIMEZONE] as TimeZoneInfo;

            if (!productExpiriesForYear.IsNull(CORRECTION))
            {
                correctionInfo = (int)productExpiriesForYear[CORRECTION];
            }

            StockCalendar calendar = new StockCalendar()
            {
                Name = productExpiriesForYear[NAME].ToString(),
                CalendarType = CalendarType.Expiry,
                Timezone = expiryDateTimeZoneInfo?.Id,
                CalendarId = (int)productExpiriesForYear[CALENDAR_ID],
                Correction = correctionInfo
            };

            calendar.FuturesExpiries = GetExpiryDates(productExpiriesForYear);
            return calendar;
        }

        private DataTable GetGridDataTable(int year)
        {
            DataTable calendarData = CreateGridDataTable();
            List<StockCalendar> calendarsList = _productManager.GetExpiryCalendars();

            foreach (StockCalendar calendar in calendarsList)
            {
                DataRow calendarExpiryDates = calendarData.NewRow();

                calendarExpiryDates[CALENDAR_ID] = calendar.CalendarId;
                calendarExpiryDates[NAME] = calendar.Name;

                if (calendar.Correction.HasValue)
                {
                    calendarExpiryDates[CORRECTION] = calendar.Correction.Value;
                }
                else
                {
                    calendarExpiryDates[CORRECTION] = DBNull.Value;
                }

                calendarExpiryDates[TIMEZONE] = string.IsNullOrEmpty(calendar.Timezone)
                    ? null
                    : TimeZoneInfo.FindSystemTimeZoneById(calendar.Timezone);

                IEnumerable<CalendarExpiryDate> expiryDatesFromCalendar =
                    from CalendarExpiryDate expiryDate in calendar.FuturesExpiries
                    where expiryDate.FuturesDate.Year == year
                    select expiryDate;

                foreach (CalendarExpiryDate expiryDate in expiryDatesFromCalendar)
                {
                    calendarExpiryDates[GetUppercaseExpiryGridCol(expiryDate.FuturesDate.Month)] = expiryDate.ExpiryDate;
                }

                foreach (CalendarExpiryDate expiryDate in expiryDatesFromCalendar)
                {
                    calendarExpiryDates[GetExpiryDateValidFieldName(expiryDate.FuturesDate.Month)] = true;
                }

                calendarData.Rows.Add(calendarExpiryDates);
            }

            calendarData.AcceptChanges();
            return calendarData;
        }

        private DataTable CreateGridDataTable()
        {
            DataTable calendarData = new DataTable();

            calendarData.Columns.Add(CALENDAR_ID, typeof(int));
            calendarData.Columns.Add(NAME, typeof(string));
            calendarData.Columns.Add(CORRECTION, typeof(int));
            calendarData.Columns.Add(TIMEZONE, typeof(TimeZoneInfo));

            ForAllMonthsOfTheYear(AddMonth, 1);
            ForAllMonthsOfTheYear(AddIsMonthValid, 1);
            return calendarData;

            void AddMonth(int monthOfYear)
            {
                string month = GetUppercaseExpiryGridCol(monthOfYear);

                calendarData.Columns.Add(month, typeof(DateTime));
            }

            void AddIsMonthValid(int monthOfYear)
            {
                string expiryDateValid = GetExpiryDateValidFieldName(monthOfYear);

                calendarData.Columns.Add(expiryDateValid, typeof(bool));
            }
        }

        private void ForAllMonthsOfTheYear(Action<int> actForMonth, int startMonth)
        {
            Enumerable.Range(startMonth, MonthsInYear).ForEach(actForMonth);
        }

        private string GetExpiryDateValidFieldName(int monthOfYear)
        {
            return $"{GetUppercaseExpiryGridCol(monthOfYear)}Valid";
        }

        private void SetGrid()
        {
            expiryDatesDisplay.Columns.Add(CreateTimeZoneColumn());
            AddMonthColumnsToGrid(CreateMonthColumns());
        }

        private List<(GridColumn, GridColumn)> CreateMonthColumns()
        {
            return Enumerable.Range(1, MonthsInYear).Select(
                monthOfYear => (CreateExpiryDateColumn(monthOfYear),
                    CreateExpiryDateValidColumn(monthOfYear))).ToList();
        }

        private void AddMonthColumnsToGrid(List<(GridColumn month, GridColumn monthValid)> months)
        {
            months.Select(
                monthColumns =>
                {
                    expiryDatesDisplay.Columns.Add(monthColumns.month);

                    return (monthValid: monthColumns.monthValid,
                        formatting: CreateMonthFormattingRules(monthColumns.month, monthColumns.monthValid));
                }).ForEach(
                monthValidAndFormatting =>
                {
                    expiryDatesDisplay.Columns.Add(monthValidAndFormatting.monthValid);
                    expiryDatesDisplay.FormatRules.AddRange(monthValidAndFormatting.formatting);
                });
        }

        private GridColumn CreateTimeZoneColumn()
        {
            return new GridColumn
            {
                Name = "timezone",
                Caption = "Time Zone",
                Width = 80,
                Visible = true,
                VisibleIndex = dateOffset.VisibleIndex + 2,
                FieldName = TIMEZONE,
                ColumnEdit = CreateTimezoneRepositoryItem()
            };
        }

        private GridColumn CreateExpiryDateColumn(int monthOfYear)
        {
            GridColumn month = new GridColumn
            {
                Name = GetLowercaseExpiryGridCol(monthOfYear),
                Caption = monthNames[monthOfYear - 1],
                Width = 10,
                Visible = true
            };

            month.DisplayFormat.FormatType = FormatType.DateTime;
            month.DisplayFormat.FormatString = DATE_FORMAT;
            month.VisibleIndex = dateOffset.VisibleIndex + 1 + monthOfYear;
            month.FieldName = GetUppercaseExpiryGridCol(monthOfYear);

            month.ColumnEdit = CreateExpiryDateRepositoryItem();

            return month;
        }

        private RepositoryItemComboBox CreateTimezoneRepositoryItem()
        {
            var repositoryItem = new RepositoryItemComboBox
            {
                DropDownRows = 10,
                TextEditStyle = TextEditStyles.DisableTextEditor
            };

            _timeZones.ForEach(tz => repositoryItem.Items.Add(tz));

            return repositoryItem;
        }

        private RepositoryItemDateEdit CreateExpiryDateRepositoryItem()
        {
            RepositoryItemDateEdit expiryDateEditor = new RepositoryItemDateEdit
            {
                ShowClear = false,
                ShowToday = false
            };

            FormatType formatType = FormatType.DateTime;

            expiryDateEditor.DisplayFormat.FormatType = formatType;
            expiryDateEditor.DisplayFormat.FormatString = DATE_FORMAT;
            expiryDateEditor.HighlightHolidays = true;
            expiryDateEditor.TextEditStyle = TextEditStyles.DisableTextEditor;
            expiryDateEditor.EditFormat.FormatType = formatType;
            expiryDateEditor.EditFormat.FormatString = DATE_FORMAT;

            expiryDateEditor.Spin += OnExpiryDateSelectorSpin;
            expiryDateEditor.KeyDown += OnExpiryDateKeyDown;
            expiryDateEditor.QueryPopUp += OnExpiryDatePopUp;
            expiryDateEditor.DrawItem += OnRepositoryDateEditDraw;
            expiryDateEditor.EditValueChanging += ExpiryDateChanging;
            expiryDateEditor.EditValueChanged += OnExpiryDateChanged;
            return expiryDateEditor;
        }

        private GridColumn CreateExpiryDateValidColumn(int monthOfYear)
        {
            GridColumn validExpiry = new GridColumn
            {
                Name = $"{GetLowercaseExpiryGridCol(monthOfYear)}Valid",
                Caption = " ",
                Width = 0,
                Visible = false,
            };

            validExpiry.FieldName = GetExpiryDateValidFieldName(monthOfYear);
            return validExpiry;
        }

        private List<GridFormatRule> CreateMonthFormattingRules(GridColumn month, GridColumn monthValid)
        {
            FormatConditionRuleExpression validExpiryDateBackColour = new FormatConditionRuleExpression();

            GridFormatRule validExpiryDateBackColourRule = new GridFormatRule
            {
                Name = $"{month.Name}ValidFormat",
                ApplyToRow = false,
                Column = month
            };
            validExpiryDateBackColourRule.Column = monthValid;
            validExpiryDateBackColourRule.ColumnApplyTo = month;
            validExpiryDateBackColour.Appearance.BackColor = DefaultBackColor;
            validExpiryDateBackColour.Appearance.Options.UseBackColor = true;
            validExpiryDateBackColour.Expression = $"[{monthValid.FieldName}] = True";
            validExpiryDateBackColourRule.Rule = validExpiryDateBackColour;

            FormatConditionRuleExpression invalidExpiryDateBackColour = new FormatConditionRuleExpression();
            GridFormatRule invalidExpiryDateBackColourRule = new GridFormatRule
            {
                Name = $"{month.Name}InvalidFormat",
                ApplyToRow = false,
                Column = month
            };
            validExpiryDateBackColourRule.Column = monthValid;
            validExpiryDateBackColourRule.ColumnApplyTo = month;
            invalidExpiryDateBackColour.Appearance.BackColor = InvalidExpiry;
            invalidExpiryDateBackColour.Appearance.Options.UseBackColor = true;
            invalidExpiryDateBackColour.Expression = $"[{monthValid.FieldName}] = False";
            invalidExpiryDateBackColourRule.Rule = invalidExpiryDateBackColour;

            return new List<GridFormatRule>() { validExpiryDateBackColourRule, invalidExpiryDateBackColourRule };
        }

        private void OnCalendarHolidaysSaved(object sender, HolidaysSavedEventArgs changedHolidays)
        {
            _holidays.UpdateHolidayCalendars(changedHolidays.Changes);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            expiryDatesDisplay.ValidatingEditor += ValidateExpiryDate;
            _holidays.InitHolidayCalendars(_productManager.GetHolidaysCalendars());
        }

        private void ExpiryDateEditorShown(object sender, EventArgs e)
        {
            if (expiryDatesDisplay.ActiveEditor is DateEdit)
            {
                expiryDatesDisplay.ValidateEditor();
            }
        }

        private void ExpiryDateChanging(object sender, ChangingEventArgs changedDate)
        {
            List<string> clashingCalendars = CheckForClashingHolidays(
                changedDate,
                GetRelatedHolidayCalendars(GetCurrentCalendarId(expiryDatesDisplay.FocusedRowHandle)));

            if (clashingCalendars.Any())
            {
                MessageBox.Show(
                    $"Expiry coincides with holiday(s) [{String.Join(", ", clashingCalendars)}]. Changes cancelled.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                changedDate.Cancel = true;
            }
        }

        private void OnExpiryDateChanged(object sender, EventArgs e)
        {
            expiryDatesDisplay.ValidateEditor();
            ExpiryDateChanged?.Invoke(this, new EventArgs());

            HackToApplyConditionalFormattingImmediately();
        }

        private void HackToApplyConditionalFormattingImmediately()
        {
            GridColumn selectedMonth = expiryDatesDisplay.FocusedColumn;

            expiryDatesDisplay.FocusedColumn = expiryDatesDisplay.Columns[0];
            expiryDatesDisplay.FocusedColumn = selectedMonth;
        }

        private void ValidateExpiryDate(object sender, BaseContainerValidateEditorEventArgs ignoreValidationArgs)
        {
            if (!(expiryDatesDisplay.ActiveEditor is DateEdit)
                || (expiryDatesDisplay.ActiveEditor as DateEdit).EditValue is DBNull)
            {
                return;
            }

            DateEdit expiryEditor = (expiryDatesDisplay.ActiveEditor as DateEdit);
            int selectedProduct = expiryDatesDisplay.FocusedRowHandle;
            List<(object date, int index)> updatedExpiryDates = GetUpdatedExpiryDates(selectedProduct, expiryEditor);

            _invalidDates = UpdateInvalidExpiryDates(updatedExpiryDates, selectedProduct);
            ForAllMonthsOfTheYear(SetIsMonthValid, NonDateInitialItems);
            SetCurrentEditorBackground();

            void SetIsMonthValid(int monthColumn)
            {
                int expiryKey = GetExpiryCellKey(selectedProduct, monthColumn);

                expiryDatesDisplay.GetDataRow(selectedProduct).SetField(
                    monthColumn + MonthsInYear,
                    !_invalidDates.Contains(expiryKey));
            }

            void SetCurrentEditorBackground()
            {
                if (_invalidDates.Contains(
                    GetExpiryCellKey(selectedProduct, expiryDatesDisplay.FocusedColumn.ColumnHandle)))
                {
                    expiryEditor.BackColor = InvalidExpiry;
                }
                else
                {
                    expiryEditor.ResetBackColor();
                }
            }
        }

        private List<(object date, int index)> GetUpdatedExpiryDates(int selectedProduct, DateEdit expiryEditor)
        {
            int dateBeingUpdated = expiryDatesDisplay.FocusedColumn.ColumnHandle - NonDateInitialItems;

            return expiryDatesDisplay.GetDataRow(selectedProduct).ItemArray.Skip(NonDateInitialItems).Take(MonthsInYear)
                                     .Select(GetExpiryDate).ToList();

            (object date, int index) GetExpiryDate(object date, int index)
            {
                return index == dateBeingUpdated
                    ? ((DateTime)expiryEditor.EditValue, index)
                    : (date, index);
            }
        }

        private HashSet<int> UpdateInvalidExpiryDates(
            List<(Object date, int month)> updatedExpiryDates,
            int selectedProduct)
        {
            HashSet<int> invalidExpiries = new HashSet<int>();

            updatedExpiryDates.ForEachWithIndex(
                (monthIndex, expiryDate) =>
                {
                    NeighbourDates neighbours = GetNeighbours(updatedExpiryDates, monthIndex);
                    int cellKey = GetExpiryCellKey(selectedProduct, expiryDate.month + NonDateInitialItems);
                    bool isDateValid = expiryDate.date is DBNull || IsDateValid(neighbours, (DateTime)expiryDate.date);

                    if (!isDateValid)
                    {
                        invalidExpiries.Add(cellKey);
                    }
                    else
                    {
                        invalidExpiries.Remove(cellKey);
                    }
                });

            return invalidExpiries;
        }

        private int GetCurrentCalendarId(int selectedRow)
        {
            if (InvalidColumn == CalendarIdCol || GridControl.NewItemRowHandle == selectedRow)
            {
                return Product.NoCalendar;
            }

            object idInRow = GridDataTable.Rows[selectedRow].ItemArray[CalendarIdCol];

            return idInRow is DBNull ? Product.NoCalendar : (int)idInRow;
        }

        private int GetExpiryCellKey(int selectedProduct, int selectedMonth)
        {
            return (selectedProduct != GridControl.NewItemRowHandle ? selectedProduct : expiryDatesDisplay.RowCount)
                   * 1000
                   + selectedMonth;
        }

        private NeighbourDates GetNeighbours(List<(Object date, int month)> dates, int selectedMonth)
        {
            return selectedMonth == 0
                ? new NeighbourDates(NeighbourDates.DefaultPrevious, GetNext(selectedMonth + 1))
                :
                selectedMonth == (dates.Count - 1)
                    ?
                    new NeighbourDates(GetPrevious(selectedMonth - 1), NeighbourDates.DefaultNext)
                    : new NeighbourDates(GetPrevious(selectedMonth - 1), GetNext(selectedMonth + 1));

            (DateTime, int) GetPrevious(int prevMonth)
            {
                if (prevMonth < 0)
                {
                    return NeighbourDates.DefaultPrevious;
                }

                (DateTime date, int month) = Neighbour(prevMonth, NeighbourDates.DefaultPrevious);

                return date == NeighbourDates.DefaultPrevious.date ? GetPrevious(prevMonth - 1) : (date, month);
            }

            (DateTime, int) GetNext(int nextMonth)
            {
                if (nextMonth > dates.Count - 1)
                {
                    return NeighbourDates.DefaultNext;
                }

                (DateTime date, int month) = Neighbour(nextMonth, NeighbourDates.DefaultNext);

                return date == NeighbourDates.DefaultNext.date ? GetNext(nextMonth + 1) : (date, month);
            }

            (DateTime, int) Neighbour(int targetMonth, (DateTime date, int month) defaultDate)
            {
                (object date, int month) = dates[targetMonth];

                return date is DBNull ? defaultDate : ((DateTime)date, month);
            }
        }

        private bool IsDateValid(NeighbourDates neighbours, DateTime value)
        {
            return value > neighbours.Previous.date && value < neighbours.Next.date;
        }

        private List<int> GetRelatedHolidayCalendars(int currentCalendarId)
        {
            return _productManager.GetProducts()
                                  .Where(product => currentCalendarId == product.ExpiryCalendar.CalendarId)
                                  .Select(product => product.HolidaysCalendar.CalendarId).ToHashSet().ToList();
        }

        private List<string> CheckForClashingHolidays(ChangingEventArgs changedDate, List<int> holidayCalendars)
        {
            List<string> clashingCalendars = holidayCalendars.Aggregate(
                new List<string>(),
                (clashes, holidayCalendar) =>
                {
                    (bool hasClash, string clashingCalendar) = _holidays.HasHolidayDate(
                        (DateTime)changedDate.NewValue,
                        holidayCalendar);

                    if (hasClash)
                    {
                        clashes.Add(clashingCalendar);
                    }

                    return clashes;
                });

            return clashingCalendars;
        }

        private static string GetLowercaseExpiryGridCol(int month)
        {
            return $"month{month}";
        }

        private static string GetUppercaseExpiryGridCol(int month)
        {
            return $"MONTH{month}";
        }

        private DataTable GridDataTable => expiryDates.DataSource as DataTable;

        private void OnRepositoryDateEditDraw(
            object sender,
            DevExpress.XtraEditors.Calendar.CustomDrawDayNumberCellEventArgs cellDraw)
        {
            if (IsWeekendOrHoliday(cellDraw.Date))
            {
                cellDraw.Style.ForeColor = Color.Gray;
            }
        }

        private void OnExpiryDatePopUp(object sender, CancelEventArgs cancelEvt)
        {
            if (!AllowChanges)
            {
                cancelEvt.Cancel = true;
            }
        }

        private void OnExpiryDateKeyDown(object sender, KeyEventArgs key)
        {
            key.Handled = true;
        }

        private void OnExpiryDateSelectorSpin(object sender, SpinEventArgs expiryDateSpinner)
        {
            expiryDateSpinner.Handled = true;
        }

        private void OnExpiryDateRowValidate(
            object sender,
            DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs rowValidation)
        {
            DataRowView dataToValidate = (DataRowView)rowValidation.Row;

            rowValidation.Valid = !expiryDates.Enabled
                                  || !string.IsNullOrEmpty(dataToValidate[CalendarNameColumn].ToString());

            if (!rowValidation.Valid)
            {
                rowValidation.ErrorText = "Calendar name empty.\n";
            }
        }

        private void OnProductNameValidating(object sender, CancelEventArgs cancelArgs)
        {
            if (string.IsNullOrEmpty((sender as TextEdit).EditValue.ToString()))
            {
                cancelArgs.Cancel = true;
                (sender as TextEdit).ErrorText = "Empty value not allowed";
            }
            else
            {
                (sender as TextEdit).ErrorText = "";
            }

            ExpiryDateChanged?.Invoke(this, new EventArgs());
        }

        private void OnProductNameChanged(object sender, EventArgs e)
        {
            if (GridControl.NewItemRowHandle == expiryDatesDisplay.FocusedRowHandle)
            {
                expiryDatesDisplay.SetFocusedRowCellValue(
                  expiryDatesDisplay.Columns[0],
                  (sender as TextEdit).EditValue.ToString());
            }

            ExpiryDateChanged?.Invoke(this, new EventArgs());
        }

        private void OnCalendarAddOrRemove(
            object sender,
            NavigatorButtonClickEventArgs navigationArgs)
        {
            if (navigationArgs.Button.ButtonType == NavigatorButtonType.Remove)
            {
                DataRow current = expiryDatesDisplay.GetDataRow(expiryDatesDisplay.FocusedRowHandle);

                if (current.IsNull(CALENDAR_ID) || _productManager.CalendarMayBeRemoved((int)current[CALENDAR_ID]))
                {
                    if (MessageBox.Show(
                            string.Format("Are you sure you want to delete {0} calendar?", current[NAME]),
                            "Confirm deletion",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question)
                        == DialogResult.No)
                    {
                        navigationArgs.Handled = true;
                    }
                    else
                    {
                        expiryDatesDisplay.SelectRow(GridControl.InvalidRowHandle);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Calendar cannot be deleted because there are products or exchanges associated with it.",
                        "Delete not allowed",
                        MessageBoxButtons.OK);
                    navigationArgs.Handled = true;
                }
            }
        }

        private bool IsWeekendOrHoliday(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private void ExpiryDatesDisplay_InitNewRow(object sender, InitNewRowEventArgs newRow)
        {
            expiryDatesDisplay.SelectRow(expiryDatesDisplay.RowCount - 1);
            expiryDatesDisplay.FocusedRowHandle = GridControl.NewItemRowHandle;
            expiryDatesDisplay.GetDataRow(newRow.RowHandle).ItemArray[CalendarIdCol] = 0;
            ForAllMonthsOfTheYear(SetDateValid, 0);
            expiryDatesDisplay.FocusedColumn = expiryDatesDisplay.Columns[0];
            // This doesn't do anything, it seems.  The hope was that it would, you know, show the editor.
            expiryDatesDisplay.ShowEditor();

            ExpiryDateChanged?.Invoke(this, new EventArgs());

            void SetDateValid(int dateValidIndex)
            {
                expiryDatesDisplay.SetRowCellValue(newRow.RowHandle, GetExpiryDateValidFieldName(dateValidIndex), true);
            }
        }
    }
}
