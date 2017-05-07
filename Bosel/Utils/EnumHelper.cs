using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace Utils
{
    public class EnumHelper
    {
        /// <summary>
        /// Gets the string values associated with the enum.
        /// </summary>
        /// <returns>IEnumerable of strings</returns>
        public static IEnumerable<string> GetStringValues(Type enumType)
        {
            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in enumType.GetFields())
            {
                //Check for our custom attribute
                DescriptionAttribute[] stringValueAttributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (stringValueAttributes.Length > 0)
                {
                    yield return stringValueAttributes[0].Description;
                }
            }
        }

        /// <summary>
        /// Gets the values as a IEnumerable datasource.
        /// </summary>
        /// <returns>IEnumerable for data binding</returns>
        public static IEnumerable<DictionaryEntry> GetEnumsAsDictionary(Type enumType)
        {
            Type underlyingType = Enum.GetUnderlyingType(enumType);

            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fieldInfo in enumType.GetFields())
            {
                //Check for our custom attribute
                DescriptionAttribute[] stringValueAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (stringValueAttributes.Length > 0)
                {
                    //yield return new DictionaryEntry(Convert.ChangeType(Enum.Parse(enumType, fieldInfo.Name, false), underlyingType, System.Globalization.CultureInfo.InvariantCulture), stringValueAttributes[0].Description);
                    yield return new DictionaryEntry(fieldInfo.Name, stringValueAttributes[0].Description);
                }
            }
        }

        /// <summary>
        /// Gets the values as a dictionary  
        /// </summary>
        /// <param name="enumType">Dictionary of string</param>
        /// <returns></returns>
        public static Dictionary<int, string> GetEnumsAsStrictDictionary(Type enumType)
        {
            Dictionary<int, string> dico = new Dictionary<int, string>(enumType.GetFields().GetLength(0));
            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fieldInfo in enumType.GetFields())
            {
                //Check for our custom attribute
                DescriptionAttribute[] stringValueAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                if (stringValueAttributes.Length > 0)
                {
                    dico.Add(Convert.ToInt32(Enum.Parse(enumType, fieldInfo.Name, false)), stringValueAttributes[0].Description);
                }
            }
            return dico;
        }
    }
}
