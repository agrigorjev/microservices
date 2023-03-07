using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandara.Entities.Entities
{
    [Table("fxOfficialProductPnLMap")]
    public class FxOfficialProductPnLMap : ICloneable
    {
        public static int DefaultId = -1;

        [Column("currencyId")]
        [Required]
        [Key]
        public int CurrencyId { get; set; }

        [Column("officialProductId")]
        [Required]
        public int OfficialProductId { get; set; }

        [Column("holidayCalendarId")]
        [Required]
        public int HolidayCalendarId { get; set; }

        public static FxOfficialProductPnLMap DefaultFxOfficialProductPnLMap(int currencyId)
        {
            FxOfficialProductPnLMap defaultForCurrency = new FxOfficialProductPnLMap()
            {
                CurrencyId = DefaultId,
                OfficialProductId = DefaultId,
                HolidayCalendarId = DefaultId
            };

            defaultForCurrency.CurrencyId = currencyId;
            return defaultForCurrency;
        }

        public bool IsDefaultFxOfficialProductPnLMap()
        {
            return this.OfficialProductId == DefaultId && this.HolidayCalendarId == DefaultId;
        }

        public object Clone()
        {
            return CloneInstance();
        }

        public FxOfficialProductPnLMap CloneInstance()
        {
            return new FxOfficialProductPnLMap()
            {
                CurrencyId = this.CurrencyId,
                OfficialProductId = this.OfficialProductId,
                HolidayCalendarId = this.HolidayCalendarId,
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is FxOfficialProductPnLMap)
            {
                return Equals(obj as FxOfficialProductPnLMap);
            }

            return false;
        }

        public bool Equals(FxOfficialProductPnLMap rhs)
        {
            if (null == rhs)
            {
                return false;
            }

            return CurrencyId == rhs.CurrencyId
                && OfficialProductId == rhs.OfficialProductId
                && HolidayCalendarId == rhs.HolidayCalendarId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CurrencyId;
                hashCode = (hashCode * 397) ^ OfficialProductId;
                hashCode = (hashCode * 397) ^ HolidayCalendarId;
                return hashCode;
            }
        }

        public static bool operator ==(FxOfficialProductPnLMap left, FxOfficialProductPnLMap right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FxOfficialProductPnLMap left, FxOfficialProductPnLMap right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return String.Format(
                "Currency {0} maps to official product {1} and holiday calendar {2}",
                CurrencyId,
                OfficialProductId,
                HolidayCalendarId);
        }
    }
}
