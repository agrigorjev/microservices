using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mandara.Business.Bus.Messages.ProductBreakdown;

namespace Mandara.ProductGUI
{
    public partial class BreakdownToolHolidaysForm : DevExpress.XtraEditors.XtraForm
    {
        public BreakdownToolHolidaysForm()
        {
            InitializeComponent();
        }

        public Holidays Holidays { get; set; }

        private void BreakdownToolHolidaysForm_Load(object sender, EventArgs e)
        {
            lbLeg1.DataBindings.Clear();
            lbLeg2.DataBindings.Clear();
            lbCalendarLeg1.DataBindings.Clear();
            lbCalendarLeg2.DataBindings.Clear();
            lbCommonPricing.DataBindings.Clear();

            gcLeg2.Visible = false;

            if (Holidays != null)
            {
                gcLeg1.Visible = Holidays.HolidaysList != null;
                gcLeg2.Visible = Holidays.HolidaysList != null && Holidays.CommonPricing;

                gcLeg1Exp.Visible = Holidays.ExpirationDays != null && Holidays.ExpirationDays.Any(it => it.Leg1ExpirationDate != null);
                gcLeg2Exp.Visible = Holidays.ExpirationDays != null && Holidays.ExpirationDays.Any(it => it.Leg2ExpirationDate != null);
                gcMonth.Visible = gcLeg1Exp.Visible || gcLeg2Exp.Visible;


                gcLegs.DataSource = GetGridItems(Holidays.HolidaysList, Holidays.ExpirationDays);

                lbLeg1.DataBindings.Add("Text", Holidays, "Leg1ProductName", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbLeg2.DataBindings.Add("Text", Holidays, "Leg2ProductName", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbLeg2.DataBindings.Add("Enabled", Holidays, "CommonPricing", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbLeg2Title.DataBindings.Add("Enabled", Holidays, "CommonPricing", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbCalendarLeg1.DataBindings.Add("Text", Holidays, "Leg1CalendarName", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbCalendarLeg2.DataBindings.Add("Text", Holidays, "Leg2CalendarName", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbCalendarLeg2.DataBindings.Add("Enabled", Holidays, "CommonPricing", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbCalendarLeg2Title.DataBindings.Add("Enabled", Holidays, "CommonPricing", false,
                    DataSourceUpdateMode.OnPropertyChanged);
                lbCommonPricing.DataBindings.Add("Text", Holidays, "CommonPricing", false,
                    DataSourceUpdateMode.OnPropertyChanged);
            }

        }

        private List<GridItem> GetGridItems(List<ProductHoliday> holidays, List<ProductExpirationDay> expirationDays)
        {
            int holidaysCount = holidays == null ? 0 : holidays.Count;
            int expirationCount = expirationDays == null ? 0 : expirationDays.Count;
            int length = Math.Max(holidaysCount, expirationCount);

            List<GridItem> result = new List<GridItem>();

            for (int i = 0; i < length; i++)
            {
                var item = new GridItem();
                if (i < holidaysCount)
                {
                    item.Leg1Holiday = holidays[i].Leg1Holiday;
                    item.Leg2Holiday = holidays[i].Leg2Holiday;
                }

                if (i < expirationCount)
                {
                    item.ExpirationMonth = expirationDays[i].ExpirationMonth.ToString("MMMyy");
                    item.Leg1ExpirationDate = expirationDays[i].Leg1ExpirationDate;
                    item.Leg2ExpirationDate = expirationDays[i].Leg2ExpirationDate;
                }

                result.Add(item);
            }

            return result;
        }

        private class GridItem
        {
            public DateTime? Leg1Holiday { get; set; }
            public DateTime? Leg2Holiday { get; set; }
            public string ExpirationMonth { get; set; }
            public DateTime? Leg1ExpirationDate { get; set; }
            public DateTime? Leg2ExpirationDate { get; set; }
        }
    }
}
