using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bosel.Model.Report
{
    public class Data:IEquatable<Data>, IEqualityComparer<Data>, IComparable<Data>
    {
        [Description("BANK")]
        public virtual string Bank { get; set; }
        [Description("ID")]
        public virtual string Id { get; set; }
        [Description("DATEVAL")]
        [DisplayName("Date value")]
        public virtual DateTime DateVal { get; set; }
        [Description("CUSTOMER")]
        public virtual string Customer { get; set; }
        [Description("DETAIL")]
        public virtual string Detail { get; set; }
        [Description("MESSAGE")]
        public virtual string Message { get; set; }
        [Description("VALUE")]
        [DisplayName("Amount")]
        public virtual decimal Value { get; set; }
        [Description("ACCOUNT")]
        [DisplayName("Account number")]
        public virtual string AccountNbr { get; set; }
        #region internal
        [Description("CATEGORY")]
        public string Category { get; set; }
        [Description("SIGN")]
        public bool Sign { get; set; }
        #endregion

        public bool Equals(Data other)
        {
            return string.Concat(other.DateVal.ToString("yyyyMMdd"), other.Id) == string.Concat(this.DateVal.ToString("yyyyMMdd"), this.Id);
        }

        public bool Equals(Data x, Data y)
        {
            return string.Concat(x.DateVal.ToString("yyyyMMdd"), x.Id) == string.Concat(y.DateVal.ToString("yyyyMMdd"), y.Id);
        }

        public int GetHashCode(Data obj)
        {
            return string.Concat(obj.DateVal.ToString("yyyyMMdd"), obj.Id).GetHashCode();
        }

        public int CompareTo(Data other)
        {
            return string.Compare(string.Concat(this.DateVal.ToString("yyyyMMdd"), this.Id), string.Concat(other.DateVal.ToString("yyyyMMdd"), other.Id), true, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
