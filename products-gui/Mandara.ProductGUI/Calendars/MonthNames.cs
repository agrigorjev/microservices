using System;
using System.Globalization;

namespace Mandara.ProductGUI.Calendars
{
    internal class MonthNames
    {
        internal static string[] GetMonthNames()
        {
            string[] result = new string[12];

            var usCultureInfo = CultureInfo.CreateSpecificCulture("en-US");
            //usCultureInfo.DateTimeFormat.MonthNames.Count == 13!!!
            Array.Copy(usCultureInfo.DateTimeFormat.MonthNames, result, 12);
            return result;
        }
    }
}
