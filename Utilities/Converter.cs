using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftID.Utilities
{
    public static class Converter
    {
        #region ToNullable
        /// <summary>
        /// This is the core of ToNullable and ToStruct generic methods.
        /// All other overloading ToNullable and Tostruct methods call this method.
        /// This method convert string or boxing value type to value type.
        /// If type T is enum then Enum.Parse method is called,
        /// else if value and type T implement IConvertible then Convert.ChangeType is called,
        /// else then unboxing the value to type T.
        /// </summary>
        /// <typeparam name="T">Convert to this struct type.</typeparam>
        /// <param name="value">Value to convert to type T.</param>
        /// <param name="provider">Fomatting that used by value parameter.</param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public static Nullable<T> ToNullable<T>(this object value, IFormatProvider provider, bool throwOnError)
            where T : struct
        {
            Nullable<T> retValue = new Nullable<T>();
            if (value == null || value is DBNull || (value is string && string.IsNullOrWhiteSpace(value as string)))
                return retValue;
            Type typeT = typeof(T);
            try
            {
                if (typeT.IsEnum)
                    retValue = (T)Enum.Parse(typeT, (value ?? string.Empty).ToString(), true);
                else if (value is IConvertible && typeT.GetInterface(typeof(IConvertible).FullName) != null)
                    retValue = provider == null ? (T)Convert.ChangeType(value, typeT)
                        : (T)Convert.ChangeType(value, typeT, provider);
                else
                    retValue = (T)value;
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
            }
            return retValue;
        }

        public static Nullable<T> ToNullable<T>(this object value, IFormatProvider provider) where T : struct
        {
            return Converter.ToNullable<T>(value, provider, true);
        }

        public static Nullable<T> ToNullable<T>(this object value, bool throwOnError) where T : struct
        {
            return Converter.ToNullable<T>(value, null, throwOnError);
        }

        public static Nullable<T> ToNullable<T>(this object value) where T : struct
        {
            return Converter.ToNullable<T>(value, null, true);
        }
        #endregion

        #region ToStruct with Default Value
        public static T ToStruct<T>(this object value, T defaultValue, IFormatProvider provider, bool throwOnError)
            where T : struct
        {
            return Converter.ToNullable<T>(value, provider, throwOnError).GetValueOrDefault(defaultValue);
        }

        public static T ToStruct<T>(this object value, T defaultValue, IFormatProvider provider) where T : struct
        {
            return Converter.ToStruct<T>(value, defaultValue, provider, true);
        }

        public static T ToStruct<T>(this object value, T defaultValue, bool throwOnError) where T : struct
        {
            return Converter.ToStruct<T>(value, defaultValue, null, throwOnError);
        }

        public static T ToStruct<T>(this object value, T defaultValue) where T : struct
        {
            return Converter.ToStruct<T>(value, defaultValue, null, true);
        }
        #endregion

        #region ToStruct without Default Value
        public static T ToStruct<T>(this object value, IFormatProvider provider, bool throwOnError) where T : struct
        {
            return Converter.ToNullable<T>(value, provider, throwOnError).GetValueOrDefault();
        }

        public static T ToStruct<T>(this object value, IFormatProvider provider) where T : struct
        {
            return Converter.ToStruct<T>(value, provider, true);
        }

        public static T ToStruct<T>(this object value, bool throwOnError) where T : struct
        {
            return Converter.ToStruct<T>(value, null, throwOnError);
        }

        public static T ToStruct<T>(this object value) where T : struct
        {
            return Converter.ToStruct<T>(value, null, true);
        }
        #endregion
    }
}