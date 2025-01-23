using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arvin.Extensions
{
    public static class AttributeExtension
    {        
        public static MaxLengthAttribute GetMaxLengthAttr(this PropertyInfo property)
        {
            if (property == null)
                return null;
            return property.GetCustomAttribute<MaxLengthAttribute>();
        }

        public static RequiredAttribute GetRequiredAttr(this PropertyInfo property)
        {
            if (property == null)
                return null;
            return property.GetCustomAttribute<RequiredAttribute>();
        }
        public static TableAttribute GetTableAttr(this Type type)
        {
            if (type == null)
                return null;
            return type.GetCustomAttribute<TableAttribute>();
        }
        public static string GetTableName(this Type type)
        {
            var tableAttr = GetTableAttr(type);
            if (tableAttr != null)
                return tableAttr.Name;
            return type.Name;
        }
    }
}
