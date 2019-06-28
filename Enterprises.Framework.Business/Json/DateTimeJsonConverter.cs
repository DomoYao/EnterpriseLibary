using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Enterprises.Framework.Json
{

    /// <summary>
    /// 短日期Json格式化
    /// </summary>
    public class ShortDateTimeConverter : IsoDateTimeConverter
    {
        public ShortDateTimeConverter() : base()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }

        /// <summary>
        /// 时间少于1900-01-01 设置为空 Add by yaolifeng
        /// 主要解决系统中时间默认值为0001/1/1的统一处理，根据系统实际情况这里把年份小于1900-01-01时间全部设置为空。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                if (dateTime < new DateTime(1900, 1, 1))
                {
                    text = "";
                }
                else
                {
                    if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                        || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    {
                        dateTime = dateTime.ToUniversalTime();
                    }

                    text = dateTime.ToString(DateTimeFormat, Culture);
                }

                
            }

            #if !NET20
            else if (value is DateTimeOffset)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                if (dateTimeOffset < new DateTimeOffset(new DateTime(1900, 1, 1)))
                {
                    text = "";
                }
                else
                {
                    if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                        || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    {
                        dateTimeOffset = dateTimeOffset.ToUniversalTime();
                    }

                    text = dateTimeOffset.ToString(DateTimeFormat, Culture);
                }
            }
            #endif
            else
            {
                throw new JsonSerializationException("短日期Json格式化发送错误!" + nameof(WriteJson));
            }

            writer.WriteValue(text);
        }

    }

    public class CustomyyyyMMddHHmmssConverter : IsoDateTimeConverter
    {
        public CustomyyyyMMddHHmmssConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }

        /// <summary>
        /// 时间少于1900-01-01 设置为空 Add by yaolifeng
        /// 主要解决系统中时间默认值为0001/1/1的统一处理，根据系统实际情况这里把年份小于1900-01-01时间全部设置为空。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                if (dateTime < new DateTime(1900, 1, 1))
                {
                    text = "";
                }
                else
                {
                    if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                        || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    {
                        dateTime = dateTime.ToUniversalTime();
                    }

                    text = dateTime.ToString(DateTimeFormat, Culture);
                }


            }

#if !NET20
            else if (value is DateTimeOffset)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                if (dateTimeOffset < new DateTimeOffset(new DateTime(1900, 1, 1)))
                {
                    text = "";
                }
                else
                {
                    if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                        || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    {
                        dateTimeOffset = dateTimeOffset.ToUniversalTime();
                    }

                    text = dateTimeOffset.ToString(DateTimeFormat, Culture);
                }
            }
#endif
            else
            {
                throw new JsonSerializationException("短日期Json格式化发送错误!" + nameof(WriteJson));
            }

            writer.WriteValue(text);
        }
    }

    public class CustomyyyyTMMddHHmmssConverter : IsoDateTimeConverter
    {
        public CustomyyyyTMMddHHmmssConverter()
        {
            DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        }
    }

    public class CustomHHmmssConverter : IsoDateTimeConverter
    {
        public CustomHHmmssConverter()
        {
            DateTimeFormat = "HH:mm:ss";
        }
    }

    public class CustomyyyyTMMddConverter : IsoDateTimeConverter
    {
        public CustomyyyyTMMddConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }

    public class CustomToMinuteConverter : IsoDateTimeConverter
    {
        public CustomToMinuteConverter()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm";
        }

        /// <summary>
        /// 时间少于1900-01-01 设置为空 Add by yaolifeng
        /// 主要解决系统中时间默认值为0001/1/1的统一处理，根据系统实际情况这里把年份小于1900-01-01时间全部设置为空。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime) value;
                if (dateTime < new DateTime(1900, 1, 1))
                {
                    text = "";
                }
                else
                {
                    if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                        || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    {
                        dateTime = dateTime.ToUniversalTime();
                    }

                    text = dateTime.ToString(DateTimeFormat, Culture);
                }


            }

#if !NET20
            else if (value is DateTimeOffset)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset) value;
                if (dateTimeOffset < new DateTimeOffset(new DateTime(1900, 1, 1)))
                {
                    text = "";
                }
                else
                {
                    if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                        || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    {
                        dateTimeOffset = dateTimeOffset.ToUniversalTime();
                    }

                    text = dateTimeOffset.ToString(DateTimeFormat, Culture);
                }
            }
#endif
            else
            {
                throw new JsonSerializationException("短日期Json格式化发送错误!" + nameof(WriteJson));
            }

            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            try
            {
                var v = reader.Value.ToString();
                if (string.IsNullOrWhiteSpace(v))
                {
                    return null;
                }

                return Convert.ToDateTime(reader.Value.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("不能将时间字符串{1}的值{0}转换为Json格式.", reader.Value, objectType));
            }

        }
    }

    public class ContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName;
        }
    }
    public class ContractLowerResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }

    public class NullToEmptyConverter : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }


    /// <summary>
    /// null值处理 Add by yaolifeng
    /// </summary>

    public class NullableValueProvider : IValueProvider
    {
        private readonly object _defaultValue;
        private readonly IValueProvider _underlyingValueProvider;

        public NullableValueProvider(MemberInfo memberInfo, Type underlyingType)
        {
            _underlyingValueProvider = new DynamicValueProvider(memberInfo);
            _defaultValue = Activator.CreateInstance(underlyingType);
        }

        public void SetValue(object target, object value)
        {
            _underlyingValueProvider.SetValue(target, value);
        }

        public object GetValue(object target)
        {
            return _underlyingValueProvider.GetValue(target) ?? _defaultValue;
        }
    }

    /// <summary>
    /// null值处理 Add by yaolifeng
    /// </summary>
    public class StringProvider : IValueProvider
    {
        private readonly string _defaultValue = string.Empty;
        private readonly IValueProvider _underlyingValueProvider;


        public StringProvider(MemberInfo memberInfo, Type underlyingType)
        {
            _underlyingValueProvider = new DynamicValueProvider(memberInfo);
        }

        public void SetValue(object target, object value)
        {
            _underlyingValueProvider.SetValue(target, value);
        }

        public object GetValue(object target)
        {
            return _underlyingValueProvider.GetValue(target) ?? _defaultValue;
        }
    }

    /// <summary>
    /// 枚举类型的转换 Add by yaolifeng
    /// T 泛型必须是枚举类型
    /// </summary>
    public class EnumJsonConvert<T>:JsonConverter where T:  struct, IConvertible
    {
        public void EnumConverter()
        {
            if (!typeof (T).IsEnum)
            {
                throw new ArgumentException("T 必须是枚举类型");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            try
            {
                return reader.Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("不能将枚举{1}的值{0}转换为Json格式.", reader.Value, objectType));
            }
           
        }

        /// <summary>
        /// 判断是否为Bool类型
        /// </summary>
        /// <param name="objectType">类型</param>
        /// <returns>为bool类型则可以进行转换</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }


        public bool IsNullableType(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return t.BaseType != null && (t.BaseType.FullName == "System.ValueType" && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
                string bValue = value.ToString();
                int isNo;
                if (int.TryParse(bValue, out isNo))
                {
                    bValue= GetEnumDescription(typeof(T), isNo);
                }
                else
                {
                    bValue= GetEnumDescription(typeof(T), value.ToString());
                }
            

            writer.WriteValue(bValue);
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="value">枚举名称</param>
        /// <returns></returns>
        private  string GetEnumDescription(Type type, string value)
        {
            try
            {
                FieldInfo field = type.GetField(value);
                if (field == null)
                {
                    return "";
                }

                var desc = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (desc != null) return desc.Description;

                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="value">枚举hasecode</param>
        /// <returns></returns>
        private  string GetEnumDescription(Type type, int value)
        {
            try
            {

                FieldInfo field = type.GetField(Enum.GetName(type, value));
                if (field == null)
                {
                    return "";
                }

                var desc = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (desc != null) return desc.Description;

                return "";
            }
            catch
            {
                return "";
            }
        }
    }

   
}
