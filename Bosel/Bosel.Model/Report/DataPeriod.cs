using System;
using System.ComponentModel;

namespace Bosel.Model.Report
{
    public class DataPeriod
    {
        [DisplayName("Date value")]
        public DateTime DateVal { get; set; }
        [DisplayName("Total in")]
        public decimal TotalIn { get; set; }
        [DisplayName("Total out")]
        public decimal TotalOut { get; set; }
    }
}
