using System;
using System.Collections.Generic;
using System.Text;

namespace Arvin.Extensions
{
    public static class ModelExtension
    {
        public static T Clone<T>(this T model) where T : class
        {
            string cloneStr = model.XmlSerialize();
            return cloneStr.XmlDeserialize<T>();
        }

        public static T CopyTo<T>(this object source) where T : new()
        {
            return source.CopyTo<T>(new T());
        }

        public static T CopyTo<T>(this object source, T target) where T : new()
        {
            if (source == null)
                return target;
            Type targetType = typeof(T);
            CopyPropList(source, target, targetType);
            return target;
        }

        /// <param name="filterFields">指定不需要自动转换的字段</param>
        /// <returns></returns>
        public static T CopyTo<T>(this object source, string[] filterFields = null) where T : new()
        {
            T target = new T();
            if (source == null)
                return target;
            Type targetType = typeof(T);
            Type sourceType = source.GetType();
            CopyPropList(source, target, targetType, filterFields);
            return target;
        }

        public static object CopyTo(this object source, Type targetType, string[] filterFields = null)
        {
            Object target = Activator.CreateInstance(targetType);
            if (source == null)
                return target;
            Type sourceType = source.GetType();
            CopyPropList(source, target, targetType, filterFields);
            return target;
        }

        public static void CopyPropList<T>(this object source, T target, Type targetType, string[] filterFields = null)
        {
            foreach (var prop in targetType.GetProperties())
            {
                if (filterFields.Contains(prop.Name))
                    continue;
                var sourceProp = source.GetType().GetProperty(prop.Name);
                if (sourceProp == null)
                    continue;
                prop.SetValue(target, Convert.ChangeType(sourceProp.GetValue(source), prop.PropertyType));
            }
        }
    }
}
