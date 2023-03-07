using System;

namespace Mandara.Entities.Calculation
{
    public class ClosingPosition
    {
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}