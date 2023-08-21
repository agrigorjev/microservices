using System;

namespace Mandara.Entities.Calculation
{
    [Serializable]
    public class CoeffEntityId
    {
        public CoeffEntity CoeffEntity { get; }
        public int EntityId { get; }

        public CoeffEntityId(CoeffEntity coeffEntity, int entityId)
        {
            CoeffEntity = coeffEntity;
            EntityId = entityId;
        }

        public override int GetHashCode()
        {
            return (CoeffEntity == CoeffEntity.SecurityDefinition ? 0 : 1) * 1000000000 + EntityId;
        }
        
        public override bool Equals(object obj)
        {
            return Object.ReferenceEquals(this, obj)
                   || (obj is CoeffEntityId rhs && (GetHashCode() == rhs.GetHashCode()));
        }

        public override string ToString()
        {
            return $"{CoeffEntity}: {EntityId}";
        }

        public const int DefaultEntityId = Int32.MinValue;

        public static readonly CoeffEntityId Default = new CoeffEntityId(
            CoeffEntity.SecurityDefinition,
            DefaultEntityId);

        public bool IsDefault() => CoeffEntity == CoeffEntity.SecurityDefinition && EntityId == DefaultEntityId;
    }

    public enum CoeffEntity
    {
        SecurityDefinition = 1,
        TradeCapture = 2
    }
}