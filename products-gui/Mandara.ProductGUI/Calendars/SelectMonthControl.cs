using System;

namespace Mandara.ProductGUI.Calendars
{
    public partial class SelectMonthControl : SelectControlBase
    {
        public SelectMonthControl()
        {
            InitializeComponent();

            SetItems(MonthNames.GetMonthNames());
            Cycle = true;
            CurrentIndex = DateTime.Now.Month - 1;
        }

        public int SelectedMonthIndex
        {
            get { return CurrentIndex + 1; }
        }
    }
}
