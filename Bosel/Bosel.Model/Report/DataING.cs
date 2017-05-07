using System;
using System.ComponentModel;

namespace Bosel.Model.Report
{
    public class DataING : Data
    {
        [Description("Numéro de compte")]
        [DisplayName("Account number")]
        public override string AccountNbr
        {
            get { return base.AccountNbr; }
            set { base.AccountNbr = value; }
        }
        [Description("Nom du compte")]
        [DisplayName("Account name")]
        public string AccountName { get; set; }
        [Description("Compte partie adverse")]
        public override string Customer
        {
            get { return base.Customer; }
            set { base.Customer = value; }
        }
        [Description("Numéro de mouvement")]
        public override string Id
        {
            get { return base.Id; }
            set { base.Id = value; }
        }
        [Description("Date comptable")]
        [DisplayName("Date execution")]
        public DateTime DateExec { get; set; }
        [Description("Date valeur")]
        [DisplayName("Date value")]
        public override DateTime DateVal
        {
            get { return base.DateVal; }
            set { base.DateVal = value; }
        }
        [Description("Montant")]
        [DisplayName("Amount")]
        public override decimal Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
        [Description("Devise")]
        [DisplayName("Currency")]
        public string CodeDevise { get; set; }

        [Description("Libellés")]
        public override string Detail
        {
            get { return base.Detail; }
            set { base.Detail = value; }
        }

        [Description("Détails du mouvement")]
        [DisplayName("Movement detail")]
        public string DetailMvt { get; set; }
        [Description("Message")]
        public override string Message
        {
            get { return base.Message; }
            set { base.Message = value; }
        }
    }
}
