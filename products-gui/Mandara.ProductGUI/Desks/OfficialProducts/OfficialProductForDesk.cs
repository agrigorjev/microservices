using System.Collections.Generic;

namespace Mandara.ProductGUI.Desks.OfficialProducts
{
    public class OfficialProductKey : IEqualityComparer<OfficialProductKey>
    {
        public int OfficialProductId { get; }
        public int DeskId { get; }
        public StripStructure ProductType { get; }

        public OfficialProductKey(int offProdId, int deskId, StripStructure prodType)
        {
            OfficialProductId = offProdId;
            DeskId = deskId;
            ProductType = prodType;
        }

        public override bool Equals(object rhs)
        {
            if (ReferenceEquals(this, rhs))
            {
                return true;
            }

            OfficialProductKey testKey = rhs as OfficialProductKey;

            return testKey != null
                   && OfficialProductId == testKey.OfficialProductId
                   && DeskId == testKey.DeskId
                   && ProductType == testKey.ProductType;
        }

        public bool Equals(OfficialProductKey lhs, OfficialProductKey rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            return lhs == null ? false : lhs.Equals(rhs);
        }

        public int GetHashCode(OfficialProductKey obj)
        {
            return (OfficialProductId * 37) ^ (DeskId * 41) ^ ((int)ProductType * 111);
        }
    }

    public class OfficialProductForDesk
    {
        public const int DefaultProductId = 0;
        public int OfficialProductId { get; set; }
        public int DeskId { get; set; }
        public const StripStructure DefaultProductType = StripStructure.Flat;
        public StripStructure ProductType { get; set; }
        public const int DefaultPricePosition = int.MaxValue;
        public int PricePosition { get; set; }
        public const decimal DefaultPriceFactor = decimal.MaxValue;
        public decimal PriceFactor { get; set; }
        public const int DefaultCacheLifetime = int.MaxValue;
        public int CacheLifeTimeMinutes { get; set; }
        public const int DefaultShowPriceAsAged = int.MaxValue;
        public int ShowPriceAsAged { get; set; }
        public const int NoAlertGroup = -1;
        public int AlertGroupId { get; set; } = NoAlertGroup;
        public const int DefaultOrder = int.MaxValue;
        public int Order { get; set; }

        public static OfficialProductForDesk Default = new OfficialProductForDesk(
            DefaultProductId,
            Desk.DefaultId,
            DefaultProductType,
            DefaultPricePosition,
            DefaultPriceFactor,
            DefaultCacheLifetime,
            DefaultShowPriceAsAged,
            NoAlertGroup,
            DefaultOrder);

        public bool IsDefault()
        {
            return DefaultProductId == OfficialProductId && Desk.DefaultId == DeskId;
        }

        public OfficialProductKey Key { get; private set; }

        public OfficialProductForDesk()
        {
            //OfficialProductId = DefaultProductId;
            ProductType = StripStructure.Flat;
            PricePosition = 0;
            PriceFactor = 1.0M;
            CacheLifeTimeMinutes = 60;
            ShowPriceAsAged = 10;
            AlertGroupId = NoAlertGroup;
            Order = 0;
            UpdateKey();
        }

        public OfficialProductForDesk(
            int officialProductId,
            int deskId,
            StripStructure productType,
            int pricePosition,
            decimal priceFactor,
            int cacheLifeTimeMinutes,
            int showPriceAsAged,
            int alertGroupId,
            int order)
        {
            OfficialProductId = officialProductId;
            DeskId = deskId;
            ProductType = productType;
            PricePosition = pricePosition;
            PriceFactor = priceFactor;
            CacheLifeTimeMinutes = cacheLifeTimeMinutes;
            ShowPriceAsAged = showPriceAsAged;
            AlertGroupId = alertGroupId;
            Order = order;
            UpdateKey();
        }

        public OfficialProductForDesk UpdateKey()
        {
            Key = new OfficialProductKey(OfficialProductId, DeskId, ProductType);
            return this;
        }

        public bool IsKeyChanged()
        {
            OfficialProductKey currentKey = new OfficialProductKey(OfficialProductId, DeskId, ProductType);

            if (currentKey.Equals(Key))
            {
                return false;
            }

            return true;
        }

        public OfficialProductForDesk Copy()
        {
            return new OfficialProductForDesk(
                OfficialProductId,
                DeskId,
                ProductType,
                PricePosition,
                PriceFactor,
                CacheLifeTimeMinutes,
                ShowPriceAsAged,
                AlertGroupId,
                Order);
        }
    }
}