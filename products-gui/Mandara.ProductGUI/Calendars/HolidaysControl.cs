using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Mandara.Business;
using Mandara.Business.Audit;
using Mandara.Business.Managers.Calendars;
using Mandara.Entities;

namespace Mandara.ProductGUI.Calendars
{
    public partial class HolidaysControl : XtraUserControl
    {
        private const string CALENDAR_ID = "CALENDAR_ID";
        private const string NAME = "NAME";
        private const string GRID_COLUMN = "GRID_COLUMN";

        private int currentYear = -1;
        private int currentMonthNumber = -1;

        private ProductManager _productManager;
        private readonly Brush holidayBrush = Brushes.Red;
        private readonly Brush weekendBrush = Brushes.LightGray;

        private readonly CalendarClashCheck _expiryDates;
        private const int InvalidColumn = -1;
        private int _calendarCol = InvalidColumn;
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

        public HolidaysControl()
        {
            InitializeComponent();

            InitGrid();

            _expiryDates = new CalendarClashCheck();
        }

        public void SetProductManager(ProductManager productManager)
        {
            _productManager = productManager;
            _productManager.RegisterForExpiriesSaved(OnCalendarExpiryDatesSaved);
        }

        public void SetMonth(int year, int month)
        {

            HideGridDayColumns(year, month);
            gcHolidays.DataSource = GetGridDataTable(year, month);
            currentYear = year;
            currentMonthNumber = month;
        }

        public bool Save()
        {
            if (HasChanges && AllowChanges)
            {
                CalendarChanges<NewHolidayCalendar> calendarHolidays = GetHolydaysFromGrid();
                AuditContext audit = ProductListForm.CreateAuditContext("Holidays");

                _productManager.UpdateHolidayCalendars(calendarHolidays, currentYear, currentMonthNumber, audit);
                gcHolidays.DataSource = GetGridDataTable(currentYear, currentMonthNumber);
                return true;
            }

            return false;
        }

        public void RejectChanges()
        {
            GridDataTable?.RejectChanges();
        }

        public bool HasChanges => GridDataTable?.GetChanges() != null;

        /// <summary>
        /// Indicates if editing available for control
        /// </summary>
        public bool AllowChanges { get; set; }
        private CalendarChanges<NewHolidayCalendar> GetHolydaysFromGrid()
        {
            List<int> deleted = new List<int>();
            List<NewHolidayCalendar> added = new List<NewHolidayCalendar>();
            List<StockCalendar> modified = new List<StockCalendar>();

            foreach (DataRow holidayData in GridDataTable.Rows)
            {
                switch (holidayData.RowState)
                {
                    case DataRowState.Added:
                        {
                            added.Add(AddHolidayToCalendar(holidayData));
                        }
                        break;

                    case DataRowState.Deleted:
                        {
                            deleted.Add((int)holidayData[CALENDAR_ID, DataRowVersion.Original]);
                        }
                        break;

                    case DataRowState.Modified:
                        {
                            modified.Add(ModifyHolidayCalendar(holidayData));
                        }
                        break;
                }
            }

            return new CalendarChanges<NewHolidayCalendar>(modified, added, deleted);
        }

        private NewHolidayCalendar AddHolidayToCalendar(DataRow holidayData)
        {
            List<CalendarHoliday> temp = new List<CalendarHoliday>();

            for (int dayIndex = 1; dayIndex <= DateTime.DaysInMonth(currentYear, currentMonthNumber); dayIndex++)
            {
                if (holidayData[GetDayCol(dayIndex)].Equals(true))
                {
                    CalendarHoliday newHoliday = new CalendarHoliday
                    {
                        HolidayDate = new DateTime(currentYear, currentMonthNumber, dayIndex)
                    };

                    temp.Add(newHoliday);
                }
            }

            return new NewHolidayCalendar(holidayData[NAME].ToString(), temp);
        }

        private StockCalendar ModifyHolidayCalendar(DataRow holidayData)
        {
            StockCalendar calendar = new StockCalendar()
            {
                Name = holidayData[NAME].ToString(),
                CalendarId = (int)holidayData[CALENDAR_ID],
            };

            for (int dayIndex = 1; dayIndex <= DateTime.DaysInMonth(currentYear, currentMonthNumber); dayIndex++)
            {
                if (holidayData[GetDayCol(dayIndex)].Equals(true))
                {
                    CalendarHoliday newHoliday = new CalendarHoliday
                    {
                        CalendarId = (int)holidayData[CALENDAR_ID],
                        HolidayDate = new DateTime(currentYear, currentMonthNumber, dayIndex)
                    };
                    calendar.Holidays.Add(newHoliday);
                }
            }

            return calendar;
        }

        private DataTable GetGridDataTable(int year, int month)
        {
            DataTable result = CreateGridDataTable(year, month);

            List<StockCalendar> calendarsList = _productManager.GetHolidaysCalendars();

            foreach (var eachCalendar in calendarsList)
            {
                DataRow newDataRow = result.NewRow();
                newDataRow[CALENDAR_ID] = eachCalendar.CalendarId;
                newDataRow[NAME] = eachCalendar.Name;

                var holidays = from CalendarHoliday holiday in eachCalendar.Holidays
                               where holiday.HolidayDate.Year == year && holiday.HolidayDate.Month == month
                               select holiday.HolidayDate;

                foreach (var eachHol in holidays)
                {
                    newDataRow[GetDayCol(eachHol.Day)] = true;
                }
                result.Rows.Add(newDataRow);
            }
            result.AcceptChanges();

            return result;
        }

        private void HideGridDayColumns(int year, int month)
        {
            int daysInMoth = DateTime.DaysInMonth(year, month);

            //hide invalid day columns
            for (int i = 29; i <= 31; i++)
            {
                var col = gvHolidays.Columns[GetDayCol(i)];
                col.Visible = (i <= daysInMoth);
                if (col.Visible && col.VisibleIndex != i)
                {
                    col.VisibleIndex = i + 1;
                }
            }
        }

        private DataTable CreateGridDataTable(int year, int month)
        {
            int daysInMoth = DateTime.DaysInMonth(year, month);

            DataTable result = new DataTable();
            result.Columns.Add(CALENDAR_ID, typeof(int));
            result.Columns.Add(NAME, typeof(string));
            for (int i = 1; i <= daysInMoth; i++)
            {
                DataColumn newColumn = new DataColumn(GetDayCol(i), typeof(bool))
                {
                    AllowDBNull = false,
                    DefaultValue = false
                };

                result.Columns.Add(newColumn);
                newColumn.SetOrdinal(i + 1);
            }

            return result;
        }

        private void InitGrid()
        {
            for (int i = 1; i <= 31; i++)
            {
                GridColumn newGridColumn = new GridColumn
                {
                    Name = GetDayGridCol(i),
                    Caption = i.ToString(),
                    Width = 10,
                    Visible = true,
                    VisibleIndex = gcName.VisibleIndex + i,
                    FieldName = GetDayCol(i)
                };
                newGridColumn.OptionsColumn.AllowEdit = false;
                newGridColumn.OptionsColumn.AllowFocus = false;

                gvHolidays.Columns.Add(newGridColumn);
            }

            GridColumn emptyColumn = new GridColumn
            {
                Width = 1,
                Visible = true,
                VisibleIndex = gvHolidays.Columns.Count
            };
            emptyColumn.OptionsColumn.AllowEdit = false;
            emptyColumn.OptionsColumn.AllowFocus = false;
            gvHolidays.Columns.Add(emptyColumn);

            gvHolidays.CustomDrawCell += new RowCellCustomDrawEventHandler(gvHolidays_CustomDrawCell);
        }

        private void OnCalendarExpiryDatesSaved(object sender, ExpiriesSavedEventArgs changedExpiries)
        {
            _expiryDates.UpdateExpiryCalendars(changedExpiries.Changes);
        }

        private void HolidaysControl_Load(object sender, EventArgs e)
        {
            _expiryDates.InitExpiryCalendars(_productManager.GetExpiryCalendars());
        }

        private string GetDayGridCol(int dayOfMonth)
        {
            return string.Format("{0}{1}", GRID_COLUMN, dayOfMonth);
        }

        private string GetDayCol(int dayOfMonth)
        {
            return string.Format("COLUMN{0}", dayOfMonth);
        }

        private DataTable GridDataTable => gcHolidays.DataSource as DataTable;

        private void gvHolidays_DoubleClick(object sender, EventArgs e)
        {
            if (AllowChanges)
            {
                GridView view = (GridView)sender;
                Point mousePos = view.GridControl.PointToClient(MousePosition);
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo mouseHit = view.CalcHitInfo(mousePos);

                if (!mouseHit.InRowCell
                    || !mouseHit.Column.Name.Contains("GRID_COLUMN")
                    || isWeekEnd(mouseHit.Column.AbsoluteIndex))
                {
                    return;
                }

                bool isHoliday = gvHolidays.GetRowCellValue(mouseHit.RowHandle, mouseHit.Column).Equals(true);
                DateTime checkDate = new DateTime(currentYear, currentMonthNumber, mouseHit.Column.AbsoluteIndex);
                List<string> clashingCalendars = !isHoliday
                    ? CheckForClashingExpiries(checkDate, GetRelatedExpiryCalendars(GetCurrentCalendarId(mouseHit.RowHandle)))
                    : new List<string>();

                if (clashingCalendars.Any())
                {
                    MessageBox.Show(
                        $"Holiday coincides with expiry calendar(s) [{string.Join(", ", clashingCalendars)}]",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    gvHolidays.SetRowCellValue(mouseHit.RowHandle, mouseHit.Column, !isHoliday);
                }
            }
        }

        private int GetCurrentCalendarId(int selectedRow)
        {
            if (InvalidColumn == CalendarIdCol)
            {
                return Product.NoCalendar;
            }

            return GridDataTable.Rows[selectedRow].Field<int>(CalendarIdCol);
        }

        private List<int> GetRelatedExpiryCalendars(int currentCalendarId)
        {
            return _productManager.GetProducts()
                                  .Where(product => currentCalendarId == product.HolidaysCalendar.CalendarId)
                                  .Select(product => product.ExpiryCalendar.CalendarId).ToHashSet().ToList();
        }

        private List<string> CheckForClashingExpiries(DateTime checkDate, List<int> expiryCalendars)
        {
            List<string> clashingCalendars = expiryCalendars.Aggregate(
                new List<string>(),
                (clashes, expiryCalendar) =>
                {
                    (bool hasClash, string clashingCalendar) =
                        _expiryDates.HasExpiryDate(checkDate, expiryCalendar);

                    if (hasClash)
                    {
                        clashes.Add(clashingCalendar);
                    }

                    return clashes;
                });

            return clashingCalendars;
        }

        private void ColumnEdit_Click(object sender, EventArgs e)
        {
            if (AllowChanges)
            {
                var col = gvHolidays.FocusedColumn;
                var row = gvHolidays.FocusedRowHandle;
                if (col.Name.Contains("GRID_COLUMN"))
                {
                    object cellValue = gvHolidays.GetRowCellValue(row, col);
                    bool boolCellValue = cellValue.Equals(true);
                    gvHolidays.SetRowCellValue(row, col, !boolCellValue);

                }
                ((Control)sender).Visible = false;
            }
        }

        private void gvHolidays_CustomDrawCell(object sender, RowCellCustomDrawEventArgs cellDraw)
        {
            GridView currentView = sender as GridView;
            Rectangle cellBounds = cellDraw.Bounds;

            if (!cellDraw.Column.Name.Contains("GRID_COLUMN"))
            {
                return;
            }

            if (isWeekEnd(cellDraw.Column.AbsoluteIndex))
            {
                cellDraw.Graphics.FillRectangle(weekendBrush, cellBounds);
            }

            bool isHoliday = currentView.GetRowCellValue(
                cellDraw.RowHandle,
                currentView.Columns[cellDraw.Column.FieldName]).Equals(true);

            if (isHoliday)
            {
                cellDraw.Graphics.FillRectangle(holidayBrush, cellBounds);
            }

            cellDraw.Handled = true;
        }

        private bool isWeekEnd(int dayIndex)
        {
            var date = new DateTime(currentYear, currentMonthNumber, dayIndex);
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private void gcHolidays_EmbeddedNavigator_ButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button.ButtonType == NavigatorButtonType.Remove)
            {
                DataRow current = gvHolidays.GetDataRow(gvHolidays.FocusedRowHandle);

                if (current.IsNull(CALENDAR_ID) || _productManager.CalendarMayBeRemoved((int)current[CALENDAR_ID]))
                {
                    if (MessageBox.Show(
                        string.Format("Are you sure you want to delete {0} calendar?"
                        , current[NAME])
                        , "Confirm deletion"
                        , MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Calendar cannot be deleted because there are products or exchanges associated with it.",
                        "Delete not allowed",
                        MessageBoxButtons.OK);
                    e.Handled = true;
                }
            }
        }

        private void gvHolidays_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            e.Valid = !gcHolidays.Enabled || !string.IsNullOrEmpty(((DataRowView)e.Row)[1].ToString());
            if (!e.Valid)
            {
                e.ErrorText = "Calendar name empty.\n";
            }
        }
    }
}
