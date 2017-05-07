using System;
using System.Reflection;

namespace Bosel.Model.Source
{
    public class Property
    {
        public string Id { get; set; }
        public string ColumnIn { get; set; }
        public string ColumnOut { get; set; }
        public TypeCode Type { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public string Format { get; set; }
        public string GroupSeparator { get; set; }
        public string DecimalSeparator { get; set; }

        public Property(string id, string columnIn, string columnOut, TypeCode type, PropertyInfo propertyInfo, string groupSeparator, string decimalSeparator, string format = null)
        {
            Id = id;
            ColumnIn = columnIn;
            ColumnOut = columnOut;
            Type = type;
            PropertyInfo = propertyInfo;
            Format = format;
            GroupSeparator = groupSeparator;
            DecimalSeparator = decimalSeparator;
        }
    }
}
