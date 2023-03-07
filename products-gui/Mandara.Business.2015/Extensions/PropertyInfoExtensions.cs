using System;
using System.Reflection;

namespace Mandara.Business.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static Func<object, object> GetValueGetter(this PropertyInfo propertyInfo)
        {
            return x => propertyInfo.GetValue(x, null);
        }
    }
}