using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WmiSharp.Test.Helpers
{
    public static class PropertyHelper
    {
        public static T SetPropertyValue<T>(string propertyName, object value)
        {
            var item = Activator.CreateInstance<T>();

            var pInfo = typeof(T).GetProperty(propertyName);
            WmiHelper.SetPropertyValue(pInfo, value, item);
            return item;
        }

    }
}
