using System;
using System.Collections.Generic;

namespace Mandara.ProductGUI.Calendars
{
    public partial class SelectYearControl : SelectControlBase
    {
        public SelectYearControl()
        {
            InitializeComponent();

            SetValues();
        }

        public int SelectedYear
        {
            get => (int)CurrentValue;
            set
            {
                if (Items.Contains(value))
                {
                    CurrentValue = value;
                }
            }
        }

        private void SetValues()
        {
            List<int> years = new List<int>();
            for (int i = DateTime.Now.Year - 10; i < DateTime.Now.Year + 10; i++)
            {
                years.Add(i);
            }
            SetItems(years);
            CurrentValue = DateTime.Now.Year;
        }
    }
}
