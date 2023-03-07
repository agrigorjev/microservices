using System;

namespace Mandara.Entities.Positions
{
    [Serializable]
    public class DateQuantity
    {
        public DateTime PosDate { get; }

        public decimal Quantity { get; private set; }

        public DateQuantity(DateTime posDate, decimal quantity)
        {
            PosDate = posDate;
            Quantity = quantity;
        }

        public void Add(decimal toAdd)
        {
            Quantity += toAdd;
        }

        public void Subtract(decimal toSubtract)
        {
            Quantity -= toSubtract;
        }

        public override string ToString()
        {
            return $"{nameof(PosDate)}: {PosDate:d}, {nameof(Quantity)}: {Quantity:F2}";
        }
    }

}
