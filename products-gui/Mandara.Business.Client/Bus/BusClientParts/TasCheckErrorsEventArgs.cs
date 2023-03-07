using System;
using System.Collections.Generic;
using Mandara.Entities.Trades;

namespace Mandara.Business.Client.Bus.BusClientParts
{
    public class TasCheckErrorsEventArgs : EventArgs
    {
        private List<TasCheckDetail> _tasErrors = new List<TasCheckDetail>();

        public List<TasCheckDetail> TasCheckErrors
        {
            get => _tasErrors;
            private set => _tasErrors = value;
        }
        
        public TasCheckErrorsEventArgs(List<TasCheckDetail> tasErrors)
        {
            TasCheckErrors = tasErrors ?? _tasErrors;
        }
    }
}