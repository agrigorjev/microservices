using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;

namespace Mandara.ProductGUI
{
    internal static class EditMask
    {
        private const string NonZeroMask = @"(0\.0*?[1-9][0-9]*)|([1-9][0-9]*\.[0-9]+)";
        private const string TwoDecimalFloatMask = "f2";

        public static void SetNonZeroFloatMask(TextEdit editor)
        {
            editor.Properties.Mask.IgnoreMaskBlank = false;
            editor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            editor.Properties.Mask.ShowPlaceHolders = false;
            editor.Properties.Mask.EditMask = NonZeroMask;
        }

        public static void SetTwoDecimalFloatMask(TextEdit editor)
        {
            editor.Properties.DisplayFormat.FormatString = TwoDecimalFloatMask;
            editor.Properties.EditFormat.FormatString = TwoDecimalFloatMask;
            editor.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
        }

    }
}
