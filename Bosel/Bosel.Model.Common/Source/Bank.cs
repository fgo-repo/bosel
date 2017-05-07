using System.Collections.Generic;

namespace Bosel.Model.Common.Source
{
    /// <summary>
    /// sheet parameters
    /// </summary>
    public class BankFileSetting
    {
        /// <summary>
        /// Gets or sets the index of the first row where we start to read the file. 0 based
        /// </summary>
        /// <value>
        /// The index of the row.
        /// </value>
        public int RowIndex { get; set; }
        /// <summary>
        /// Gets or sets the index of the sheet (default: 0).
        /// </summary>
        /// <value>
        /// The index of the sheet.
        /// </value>
        public int SheetIndex { get; set; }
    }

    /// <summary>
    /// bank parameters
    /// </summary>
    public class BankDataSetting
    {
        /// <summary>
        /// Gets or sets the pattern used to identify the bank by the file path.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        public string Pattern { get; set; }
        /// <summary>
        /// Gets or sets the minimum value used to filter the data on the absolute value (default: 0).
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public decimal MinVal { get; set; }
        /// <summary>
        /// Gets or sets the maximum value used to filter the data on the absolute value (default: 0).
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public decimal MaxVal { get; set; }       
    }

    public class BankMapping
    {
        /// <summary>
        /// Gets or sets a value indicating whether the category codes are upper.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the value indicating whether the category codes are upper otherwise, <c>false</c>.
        /// </value>
        public bool IsKeyUpper { get; set; }
        /// <summary>
        /// Gets or sets the number group separator.
        /// </summary>
        /// <value>
        /// The number group separator.
        /// </value>
        public string NumberGroupSeparator { get; set; }
        /// <summary>
        /// Gets or sets the number decimal separator.
        /// </summary>
        /// <value>
        /// The number decimal separator.
        /// </value>
        public string NumberDecimalSeparator { get; set; }

        /// <summary>
        /// Gets or sets the fields used to identify the key (based on the class Data).
        /// </summary>
        /// <value>
        /// The key fields.
        /// </value>
        public List<string> KeyFields { get; set; }
        public List<Mapping> MappingFields { get; set; }
    }
    public class Mapping
    {
        /// <summary>
        /// Gets or sets the column of the source file.
        /// </summary>
        /// <value>
        /// The in.
        /// </value>
        public string In { get; set; }
        /// <summary>
        /// Gets or sets the output field in the class Data.
        /// </summary>
        /// <value>
        /// The out.
        /// </value>
        public string Out { get; set; }
        /// <summary>
        /// Gets or sets the format used for the dates (dd/MM/yyyy by default).
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public string Format { get; set; }
    }

    /// <summary>
    /// bank settings
    /// </summary>
    public class BankSetting
    {
        public BankDataSetting DataSetting { get; set; }
        public BankMapping DataMapping { get; set; }
        public BankFileSetting FileSetting { get; set; }
    }

    public class Bank
    {
        /// <summary>
        /// Gets or sets the name of the bank.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether we include an extra sheet with the data of the bank.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include bank sheet]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeBankSheet { get; set; }
        /// <summary>
        /// Gets or sets the settings by bank. A bank may have several inputs due to different formats
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public List<BankSetting> Settings { get; set; }
    }
    public class BankList
    {
        public List<Bank> Banks { get; set; }
    }
}
