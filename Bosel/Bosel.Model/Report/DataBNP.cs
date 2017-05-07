using System;
using System.ComponentModel;

namespace Bosel.Model.Report
{
    public class DataBNP:Data
    {        
        [Description("ANNEE + REFERENCE")]
        public override string Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }
        [Description("DATE DE L'EXECUTION")]
        [DisplayName("Date execution")]
        public DateTime DateExec { get; set; }
        [Description("DATE VALEUR")]
        [DisplayName("Date value")]
        public override DateTime DateVal 
        {
            get { return base.DateVal; }
            set { base.DateVal = value; }
        }
        [Description("MONTANT")]
        [DisplayName("Amount")]
        public override decimal Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
        [Description("DEVISE DU COMPTE")]
        [DisplayName("Currency")]
        public string CodeDevise { get; set; }
        [Description("CONTREPARTIE DE L'OPERATION")]
        public override string Customer
        {
            get { return base.Customer; }
            set { base.Customer = value; }
        }
        [Description("DETAIL")]
        public override string Detail
        {
            get { return base.Detail; }
            set { base.Detail = value; }
        }
        [Description("NUMERO DE COMPTE")]
        [DisplayName("Account number")]
        public override string AccountNbr
        {
            get { return base.AccountNbr; }
            set { base.AccountNbr = value; }
        }
    }
}
