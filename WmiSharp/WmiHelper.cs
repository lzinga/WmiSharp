using System;
using System.Collections.Generic;
using System.Management;
using System.Reflection;
using System.Text;
using WmiSharp.Attributes;
using System.Runtime.CompilerServices;
using System.Linq;

[assembly: InternalsVisibleTo("WmiSharp.Test")]
namespace WmiSharp
{
    internal class WmiHelper
    {
        /// <summary>
        /// Returns the wmi class name from <see cref="WmiClassAttribute"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static string GetClassName<T>()
        {
            var attr = typeof(T).GetCustomAttribute<WmiClassAttribute>();

            return attr == null
                ? string.Empty
                : attr.Name;
        }

        /// <summary>
        /// Returns the wmi namespace from <see cref="WmiClassAttribute"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static string GetNamespace<T>()
        {
            var attr = typeof(T).GetCustomAttribute<WmiClassAttribute>();

            return attr == null
                ? string.Empty
                : attr.Namespace;
        }

        /// <summary>
        /// Gets all properties from <typeparamref name="T"/> that are queryable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static string GetSearchableProperties<T>()
        {
            List<string> properties = new List<string>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                WmiIgnoreAttribute ignoreProp = propertyInfo.GetCustomAttribute<WmiIgnoreAttribute>();

                if (ignoreProp == null)
                {
                    WmiPropertyAttribute propAtt = propertyInfo.GetCustomAttribute<WmiPropertyAttribute>();

                    if (propAtt == null)
                    {
                        properties.Add(propertyInfo.Name.ToUpper());
                    }
                    else
                    {
                        properties.Add(propAtt.Name.ToUpper());
                    }
                }
            }

            return String.Join(",", properties);
        }

        /// <summary>
        /// Loads the <see cref="ManagementObject"/> into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wmiObject"></param>
        /// <returns></returns>
        internal static T LoadType<T>(ManagementObject wmiObject)
        {
            var o = Activator.CreateInstance<T>();

            foreach (PropertyInfo p in o.GetType().GetProperties())
            {
                WmiIgnoreAttribute ignore = p.GetCustomAttribute<WmiIgnoreAttribute>();
                if (ignore != null)
                {
                    continue;
                }

                WmiPropertyAttribute propAtt = p.GetCustomAttribute<WmiPropertyAttribute>();
                string propertyName = p.Name;
                if (propAtt != null)
                {
                    propertyName = propAtt.Name;
                }
                var value = wmiObject.Properties[propertyName].Value;

                SetPropertyValue(p, wmiObject, o);
            }

            return o;
        }

        internal static void SetPropertyValue(PropertyInfo p, object value, object o)
        {
            // If the value to be checked is null, just set the property value to null.
            if (value == null)
            {
                p.SetValue(o, null);
            }

            // If the property type is a DateTime or its a Nullable<DateTime> and it is a string try to set the value.
            else if ((p.PropertyType == typeof(DateTime) || p.PropertyType.GetGenericArguments().FirstOrDefault() == typeof(DateTime)) && value is string s)
            {
                var val = ToDateTime(s);

                // If it is nullable.
                if (val == default && p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    p.SetValue(o, null);
                }
                else
                {
                    p.SetValue(o, val);
                }
            }
            else if (p.PropertyType.IsEnum || p.PropertyType.GetGenericArguments().FirstOrDefault().IsEnum)
            {
                if (int.TryParse(value.ToString(), out int enumVal))
                {
                    if(Enum.IsDefined(p.PropertyType, enumVal))
                    {
                        p.SetValue(o, Convert.ChangeType(enumVal, Enum.GetUnderlyingType(p.PropertyType)));
                    }
                    else
                    {
                        p.SetValue(o, default);
                    }
                }
                else if (string.IsNullOrEmpty(value.ToString()))
                {
                    p.SetValue(o, default);
                }
                else
                {
                    p.SetValue(o, Enum.Parse(p.PropertyType, value.ToString()));
                }
            }
            else
            {
                p.SetValue(o, Convert.ChangeType(value, p.PropertyType), null);
            }
        }

        private static DateTime ToDateTime(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return default;
            }

            return DateTime.TryParse(obj, out DateTime date) ? date : ManagementDateTimeConverter.ToDateTime(obj);
        }
    }
}
