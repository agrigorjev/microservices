using System;
using System.Collections.Generic;
using Mandara.Entities;
using Mandara.Entities.Trades;

namespace Mandara.Business.Bus
{
    public class FxExposureDetailChangedEventArgs : EventArgs
    {
        public FxExposureDetail FxExposureDetail { get; set; }

        public FxExposureDetailChangedEventArgs(FxExposureDetail detail)
        {
            FxExposureDetail = detail;
        }
    }
}