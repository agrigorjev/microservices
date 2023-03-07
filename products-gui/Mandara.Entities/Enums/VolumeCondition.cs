using System.ComponentModel;

namespace Mandara.Entities.Enums
{
    public enum VolumeCondition
    {
        [Description("<")]
        Lt = 0,
        [Description(">")]
        Gt = 1
    }
}