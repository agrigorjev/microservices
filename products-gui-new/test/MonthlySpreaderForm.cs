using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Mandara.Business.Spreader;
using Mandara.RiskMgmtTool.Spreader.Commands;
using Mandara.RiskMgmtTool.Spreader.Common;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mandara.RiskMgmtTool.Spreader.MonthlySpreader
{
    public partial class MonthlySpreaderForm : SpreaderFormBase, IMonthlySpreaderView
    {
        public override string FormId => $"SpreaderForm{Tag ?? ""}";

        public override int ExitRow
        {
            get => _exitRow ?? SpreadsTotals.NoExitMonth;
            set
            {
                _exitRow = value;

                if (SpreadsTotals.NoExitMonth == value)
                {
                    exitRowTitle.Caption = string.Empty;
                }
                else
                {
                    string month = calculatedSpreadsDisplay.GetRowCellValue(
                        ExitRow,
                        calculatedSpreadsDisplay.Columns[nameof(MonthlySpread.Month)]).ToDataString();

                    exitRowTitle.Caption = $"Exit on {month}";
                }
            }
        }

        private int? _exitRow;

        public override int SplitterPosition
        {
            get => splitContainerControl1.SplitterPosition;
            set => splitContainerControl1.SplitterPosition = value;
        }

        private MonthlySpreaderPresenter _spreaderPresenter;
        private readonly ILogger _logger = new NLogLogger(typeof(MonthlySpreaderPresenter));

        private readonly DXMenuItem _setExitItem = new DXMenuItem() { Caption = "Exit here" };
        private readonly DXMenuItem _clearExitItem = new DXMenuItem() { Caption = "Clear selection" };

        public MonthlySpreaderForm(MainForm form) : base(form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            InitializeComponent();

            _spreaderPresenter = new MonthlySpreaderPresenter(
                form.BusClientStreams,
                form.GetMessageBox.Invoke(),
                this,
                form.SettingsSaver,
                form.SchedulerProvider);

            _spreaderPresenter.Initialize();
        }

        public void SetInputGrid(BindingList<SimulatedPosition> manualInputPositions)
        {
            manualPositions.BeginUpdate();
            manualPositions.DataSource = manualInputPositions;
            manualPositions.EndUpdate();
        }

        public void SetCalculatedGrid(BindingList<MonthlySpread> manualInputPositions)
        {
            calculatedSpreads.BeginUpdate();
            calculatedSpreads.DataSource = manualInputPositions;
            calculatedSpreads.EndUpdate();
        }

        protected override void AddControlsToSettings()
        {
            MainForm.SettingsSaver.AddObject(manualPositionsDisplay, "InputGrid", FormId);
            MainForm.SettingsSaver.AddObject(calculatedSpreadsDisplay, "OutputGrid", FormId);
        }

        protected override void FormatFigures(string quantityFormatter)
        {
            if (manualPositionsDisplay.Columns.Count > 0)
            {
                configuredSwapQuantities.DisplayFormat.FormatString = quantityFormatter;
                configuredFuturesQuantities.DisplayFormat.FormatString = quantityFormatter;
            }

            if (calculatedSpreadsDisplay.Columns.Count > 0)
            {
                futuresEquivalents.DisplayFormat.FormatString = quantityFormatter;
                jalSpreads.DisplayFormat.FormatString = quantityFormatter;
                bookFuturesEquivalents.DisplayFormat.FormatString = quantityFormatter;
                totalJalSpreads.DisplayFormat.FormatString = quantityFormatter;
            }
        }

        public override void UpdateView(object arg)
        {
            calculatedSpreads.RefreshDataSource();
            manualPositions.RefreshDataSource();
        }

        private void manualPositionsDisplay_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            CommandStream.OnNext(new RecalculateFuturesInputPosChanged());
        }

        protected override void Dispose(bool disposing)
        {
            _logger.Trace($"Disposing {FormId} {disposing}.");
            if (disposing)
            {
                CommandStream?.Dispose();
                _spreaderPresenter?.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void CalculatedSpreadsPositionsRowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            e.Appearance.BackColor = IsExitRow(e.RowHandle) ? SpreaderDisplayDefaults.ExitRowBackground : calculatedSpreadsDisplay.Appearance.Row.BackColor;
        }

        private void CalculatedSpreadsPositionsPopupMenuShowing(object sender, PopupMenuShowingEventArgs popupMenu)
        {
            if (!popupMenu.HitInfo.InDataRow)
            {
                return;
            }

            calculatedSpreadsDisplay.FocusedRowHandle = popupMenu.HitInfo.RowHandle;
            popupMenu.Menu.Items.Clear();
            popupMenu.Menu.Items.Add(IsExitRow(popupMenu.HitInfo.RowHandle) ? _clearExitItem : _setExitItem);
        }

        private bool IsExitRow(int handle)
        {
            return ExitRow == handle;
        }

        private void MonthlySpreaderForm_Load(object sender, EventArgs e)
        {
            _setExitItem.Click += OnExitRowSet;
            _clearExitItem.Click += OnExitRowCleared;
        }

        private void OnExitRowSet(object sender, EventArgs e)
        {
            ExitRow = calculatedSpreadsDisplay.FocusedRowHandle;
            calculatedSpreadsDisplay.RefreshRow(ExitRow);
            CommandStream.OnNext(new RecalculateSpreadsExitRowChanged());
        }

        private void OnExitRowCleared(object sender, EventArgs e)
        {
            calculatedSpreadsDisplay.RefreshRow(ExitRow);
            ExitRow = SpreadsTotals.NoExitMonth;
            CommandStream.OnNext(new RecalculateSpreadsExitRowChanged());
        }
    }
}