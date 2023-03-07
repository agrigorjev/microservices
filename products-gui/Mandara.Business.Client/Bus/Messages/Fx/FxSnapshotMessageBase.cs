using Mandara.Business.Bus.Messages.Base;
using System;

namespace Mandara.Business.Bus.Messages.Fx
{
    public abstract class FxSnapshotMessageBase : SnapshotMessageBase
    {
        internal void ValidateParameter<T>(Predicate<T> validator, T valueToTest, string errorMessageFormat)
        {
            if (!validator(valueToTest))
            {
                throw new ArgumentException(String.Format(errorMessageFormat, valueToTest));
            }
        }
    }
}
