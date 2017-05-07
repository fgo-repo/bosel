using Bosel.Model.Common.Source;
using Bosel.Model.Source;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Utils;

namespace Bosel.Edition.Utils
{
    public static class PropertyHelper
    {
        public static List<Property> GetProperties<T>() where T : class, new()
        {
            return GetProperties(typeof(T));
        }
        public static List<Property> GetProperties(Type type)
        {
            var lst = new List<Property>();
            var properties = type.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                var source = properties[i].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                var desc = properties[i].GetCustomAttributes(typeof(DisplayNameAttribute), false) as DisplayNameAttribute[];

                lst.Add(new Property(
                        properties[i].Name,
                        source.Length > 0 ? source[0].Description : "UNMAPPED",
                        desc.Length > 0 ? desc[0].DisplayName : properties[i].Name,
                        Type.GetTypeCode(properties[i].PropertyType),
                        properties[i],
                        NumberGroupSeparator.Value,
                        NumberDecimalSeparator.Value
                    ));
            }
            return lst;
        }
        public static List<Property> GetProperties<T>(BankMapping mappings)
        {
            return GetProperties(mappings, typeof(T));
        }
        public static List<Property> GetProperties(BankMapping mappings, Type type)
        {
            var lst = new List<Property>();
            var properties = type.GetProperties();
            var dico = mappings.MappingFields.ToDictionary(x => x.Out, x => new { x.In, x.Format });
            var numberGroupSeparator = !string.IsNullOrWhiteSpace(mappings.NumberGroupSeparator) ? mappings.NumberGroupSeparator : NumberGroupSeparator.Value;
            var numberDecimalSeparator = !string.IsNullOrWhiteSpace(mappings.NumberDecimalSeparator) ? mappings.NumberDecimalSeparator : NumberDecimalSeparator.Value;


            for (int i = 0; i < properties.Length; i++)
            {
                var source = properties[i].GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
                var desc = properties[i].GetCustomAttributes(typeof(DisplayNameAttribute), false) as DisplayNameAttribute[];

                lst.Add(new Property(
                        properties[i].Name,
                        source.Length > 0 && dico.ContainsKey(source[0].Description) ? dico[source[0].Description].In : "UNMAPPED",
                        desc.Length > 0 ? desc[0].DisplayName : properties[i].Name,
                        Type.GetTypeCode(properties[i].PropertyType),
                        properties[i],
                        numberGroupSeparator,
                        numberDecimalSeparator,
                        source.Length > 0 && dico.ContainsKey(source[0].Description) ? dico[source[0].Description].Format : null
                    ));
            }
            return lst;
        }

        public static Lazy<string> NumberDecimalSeparator = new Lazy<string>(() => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        public static Lazy<string> NumberGroupSeparator = new Lazy<string>(() => CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
        public static Lazy<string> WrongDecimalSeparator = new Lazy<string>(() => NumberDecimalSeparator.Value == "." ? "," : ".");
        
        private static Random random = new Random(0);
        public static object ConvertVal(this Property property, object obj, string format = null) 
        {
            object rtnVal = null;
            if (obj == null)
            {
                rtnVal = null;
            }
            else
            {
                switch (property.Type)
                {
                    case TypeCode.String:
                        rtnVal = obj.ToString();
                        break;
                    case TypeCode.DateTime:
                        try
                        {
                            rtnVal = DateTime.ParseExact(obj.ToString(), format ?? "dd/MM/yyyy", null);
                        }
                        catch
                        {
                            rtnVal = Convert.ChangeType(obj, property.PropertyInfo.PropertyType);
                        }                       
                        break;
                    case TypeCode.Int32:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        try
                        {
                            decimal dVal = 0;
                            if(decimal.TryParse(obj.ToString(), NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out dVal))
                            {
                                rtnVal = dVal;
                            }
                            else
                            {
                                rtnVal = Convert.ToDecimal(obj.ToString()
                                        .Replace(property.GroupSeparator, NumberGroupSeparator.Value)
                                        .Replace(property.DecimalSeparator, NumberDecimalSeparator.Value), 
                                    CultureInfo.CurrentCulture);
                            }
                        }
                        catch
                        {
                            rtnVal = Convert.ChangeType(obj.ToString().Replace(WrongDecimalSeparator.Value, NumberDecimalSeparator.Value), property.PropertyInfo.PropertyType);
                        }
                        //random value for demo
                        //rtnVal = (decimal)random.NextDouble() * Convert.ToDecimal(rtnVal);
                        break;
                    case TypeCode.Boolean:
                        rtnVal = obj.ToString().ToLower().In("true", "1");
                        break;
                    case TypeCode.Char:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                        rtnVal = Convert.ChangeType(obj, property.PropertyInfo.PropertyType);
                        break;
                    case TypeCode.DBNull:
                        rtnVal = null;
                        break;
                    case TypeCode.Empty:
                        rtnVal = string.Empty;
                        break;
                    case TypeCode.Object:
                        rtnVal = obj;
                        break;
                    default:
                        break;
                }
            }
            return rtnVal;
        }
    }
}
