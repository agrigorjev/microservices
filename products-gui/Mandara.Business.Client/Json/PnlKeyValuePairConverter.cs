using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace Mandara.Business.Json
{
    public class PnlKeyValuePairConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type type = value.GetType();
            PropertyInfo property1 = type.GetProperty("Key");
            PropertyInfo property2 = type.GetProperty("Value");
            writer.WriteStartObject();
            writer.WritePropertyName("PortfolioId");
            serializer.Serialize(writer, ReflectionUtils.GetMemberValue((MemberInfo)property1, value));
            writer.WritePropertyName("Pnls");
            serializer.Serialize(writer, ReflectionUtils.GetMemberValue((MemberInfo)property2, value));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool flag = ReflectionUtils.IsNullableType(objectType);
            if (reader.TokenType == JsonToken.Null)
            {
                return (object)null;
            }
            else
            {
                Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
                IList<Type> list = (IList<Type>)type.GetGenericArguments();
                Type objectType1 = list[0];
                Type objectType2 = list[1];
                object obj1 = (object)null;
                object obj2 = (object)null;
                reader.Read();
                while (reader.TokenType == JsonToken.PropertyName)
                {
                    switch (reader.Value.ToString())
                    {
                        case "PortfolioId":
                            reader.Read();
                            obj1 = serializer.Deserialize(reader, objectType1);
                            break;
                        case "Pnls":
                            reader.Read();
                            obj2 = serializer.Deserialize(reader, objectType2);
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                    reader.Read();
                }
                return ReflectionUtils.CreateInstance(type, obj1, obj2);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            Type type = ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            if (type.IsValueType && type.IsGenericType)
                return type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
            else
                return false;
        }
    }
}