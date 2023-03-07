using Mandara.Entities;
using System.Collections.Generic;

namespace Mandara.ProductGUI.Models
{
    public class DailyDiffMonthShift
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static List<DailyDiffMonthShift> GetList()
        {
            List<DailyDiffMonthShift> result = new List<DailyDiffMonthShift>();
            result.Add(null);
            result.Add(new DailyDiffMonthShift() { Value = Product.NoDailyDiffMonthShift });
            result.Add(new DailyDiffMonthShift() { Value = 1 });
            result.Add(new DailyDiffMonthShift() { Value = 2 });
            result.Add(new DailyDiffMonthShift() { Value = 3 });
            result.Add(new DailyDiffMonthShift() { Value = 4 });
            return result;
        }
    }
}
