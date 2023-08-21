using System.ComponentModel;
using Mandara.Entities;

namespace Mandara.ProductGUI.Models
{
    public class CurrencyModel : IEditableObject
    {
        private Currency _currency;
        private Currency _backupCurrency;
        private bool inTxn = false;

        public CurrencyModel(Currency currency)
        {
            _currency = currency;
            _backupCurrency = new Currency();
        }

        public string IsoName { get { return _currency.IsoName; } set { _currency.IsoName = value; } }

        public Currency Currency { get { return _currency; } }

        // Implements IEditableObject
        void IEditableObject.BeginEdit()
        {
            if (!inTxn)
            {
                _backupCurrency.IsoName = _currency.IsoName;
                inTxn = true;
            }
        }

        void IEditableObject.CancelEdit()
        {
            if (inTxn)
            {
                _currency.IsoName = _backupCurrency.IsoName;
                inTxn = false;
            }
            
        }

        void IEditableObject.EndEdit()
        {
            if (inTxn)
            {
                inTxn = false;
            }
        }
    }
}
