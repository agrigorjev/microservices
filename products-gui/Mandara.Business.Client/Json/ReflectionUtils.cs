using System;
using System.Globalization;
using System.Reflection;

namespace Mandara.Business.Json
{
    public static class ReflectionUtils
    {
        public static bool IsNullableType(Type t)
        {
            if (t.IsGenericType)
                return t.GetGenericTypeDefinition() == typeof(Nullable<>);
            else
                return false;
        }

        public static object GetMemberValue(MemberInfo member, object target)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(target);
                case MemberTypes.Property:
                    try
                    {
                        return ((PropertyInfo)member).GetValue(target, (object[])null);
                    }
                    catch (TargetParameterCountException ex)
                    {
                        throw new ArgumentException(String.Format("MemberInfo '{0}' has index parameters", (IFormatProvider)CultureInfo.InvariantCulture, (object)member.Name), (Exception)ex);
                    }
                default:
                    throw new ArgumentException(String.Format("MemberInfo '{0}' is not of type FieldInfo or PropertyInfo", (IFormatProvider)CultureInfo.InvariantCulture, (object)CultureInfo.InvariantCulture, (object)member.Name), "member");
            }
        }

        public static object CreateInstance(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }

    }
}