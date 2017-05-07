using Bosel.Edition.Source;
using Bosel.Model.Source;
using Bosel.Model.Report;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Table.PivotTable;
using System.Collections.Generic;
using System.Linq;

namespace Bosel.App.Report
{
    public class BankReport : XLSXSource
    {
        public void AddSheetTotal<T>(List<T> data, string sheetName, int rowIndex = 0, int clnIndex = 0) where T : class, new()
        {
            if (data?.Count == 0)
                return;
            string type = typeof(T).Name;
            var properties = base.InitProperties<T>();

            switch (type)
            {
                case "Data":       
                    var defaultDate = (data.First() as Data).DateVal;
                    var lst = new List<Data>();
                    Factory.CategoriesDef.Categories.ForEach(c =>
                        lst.Add(new Data()
                        {
                            Category = c.Code,
                            Value = 0,
                            DateVal = defaultDate
                        })
                    );
                    lst = lst.Concat<Data>(data.Cast<Data>()).ToList();

                    base.AddSheet<Data>(lst, sheetName, rowIndex, clnIndex);
                    var worksheet = base.Package.Workbook.Worksheets[base.Package.Workbook.Worksheets.Count];
                    AddDataChart<Data>(worksheet, properties, lst, rowIndex, clnIndex);
                    break;
                case "DataPeriod":
                    base.AddSheet<T>(data, sheetName, rowIndex, clnIndex);
                    var worksheetPeriod = base.Package.Workbook.Worksheets[base.Package.Workbook.Worksheets.Count];
                    AddDataPeriodChart<T>(worksheetPeriod, properties, data, rowIndex, clnIndex);
                    break;
                default:
                    base.AddSheet<T>(data, sheetName, rowIndex, clnIndex);
                    break;
            }
        }

        private void AddDataChart<T>(ExcelWorksheet worksheet, List<Property> properties, List<T> data, int rowIndex =0, int clnIndex=0) where T: class, new()
        {
            using (ExcelRange dataRange = worksheet.Cells[string.Format("{0}{1}:{2}{3}",
                        XlsxExtensions.ColumnsLetter.Value[clnIndex],
                        rowIndex + 1,
                        XlsxExtensions.ColumnsLetter.Value[clnIndex + properties.Count - 1],
                        rowIndex + 1 + data.Count
                    )
                ])
            {
                #region pivot
                var wsPivot = base.Package.Workbook.Worksheets.Add("Pivot" + worksheet.Name);
                var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A3"], dataRange, "PerCategory");

                //pivotTable.Fields.Select(f => f.Name).ToList().ForEach(l => Console.WriteLine(l));

                pivotTable.ColumnFields.Add(pivotTable.Fields["Category"]);
                //Add a rowfield
                var rowField = pivotTable.RowFields.Add(pivotTable.Fields["Date value"]);
                //This is a date field so we want to group by Years and quaters. This will create one additional field for years.
                rowField.AddDateGrouping(eDateGroupBy.Years | eDateGroupBy.Quarters | eDateGroupBy.Months);
                //Get the Quaters field and change the texts
                var quaterField = pivotTable.Fields.GetDateGroupField(eDateGroupBy.Quarters);
                quaterField.Items[0].Text = "<"; //Values below min date, but we use auto so its not used
                quaterField.Items[1].Text = "Q1";
                quaterField.Items[2].Text = "Q2";
                quaterField.Items[3].Text = "Q3";
                quaterField.Items[4].Text = "Q4";
                quaterField.Items[5].Text = ">"; //Values above max date, but we use auto so its not used                

                //Add a pagefield
                pivotTable.PageFields.Add(pivotTable.Fields["Sign"]);
                //pivotTable.PageFields.Add(pivotTable.Fields["Category"]);

                //Add the data fields and format them
                var dataField = pivotTable.DataFields.Add(pivotTable.Fields["Amount"]);
                dataField.Format = "#,##0";

                //We want the datafields to appear in columns
                pivotTable.DataOnRows = false;

                pivotTable.WorkSheet.View.FreezePanes(5, 4); 
                #endregion

                #region chart
                var chart = wsPivot.Drawings.AddChart("Chart" + worksheet.Name, eChartType.BarStacked, pivotTable);
                //chart.DataLabel.ShowCategory = true;
                //chart.DataLabel.ShowPercent = true;
                chart.SetPosition(5, 0, 5 + Factory.Categories.Values.Distinct().Count(), 0);
                chart.SetSize(600, 800); 
                #endregion


                #region chart Full Sheet
                var sheetForChart = base.Package.Workbook.Worksheets.Add("Chart" + worksheet.Name);
                var chartFullSheet = sheetForChart.Drawings.AddChart("ChartFull" + worksheet.Name, eChartType.ColumnStacked, pivotTable);
                chartFullSheet.Legend.Position = OfficeOpenXml.Drawing.Chart.eLegendPosition.Right;
                chartFullSheet.Legend.Add();
                chartFullSheet.SetPosition(1, 0, 1, 0);
                chartFullSheet.SetSize(1500, 900);
                chartFullSheet.Title.Text = "Pivot chart";
                #endregion
            }
        }

        private void AddDataPeriodChart<T>(ExcelWorksheet worksheet, List<Property> properties, List<T> data, int rowIndex = 0, int clnIndex = 0) where T : class, new()
        {
            using (ExcelRange dataRange = worksheet.Cells[string.Format("{0}{1}:{2}{3}",
                        XlsxExtensions.ColumnsLetter.Value[clnIndex],
                        rowIndex + 1,
                        XlsxExtensions.ColumnsLetter.Value[clnIndex + properties.Count - 1],
                        rowIndex + 1 + data.Count
                    )
                ])
            {
                #region pivot
                var wsPivot = base.Package.Workbook.Worksheets.Add("Pivot" + worksheet.Name);
                var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A1"], dataRange, "PerPeriod");

                //pivotTable.Fields.Select(f => f.Name).ToList().ForEach(l => Console.WriteLine(l));

                //pivotTable.ColumnFields.Add(pivotTable.Fields["Category"]);
                //Add a rowfield
                var rowField = pivotTable.RowFields.Add(pivotTable.Fields["Date value"]);
                //This is a date field so we want to group by Years and quaters. This will create one additional field for years.
                rowField.AddDateGrouping(eDateGroupBy.Years | eDateGroupBy.Quarters | eDateGroupBy.Months);
                //Get the Quaters field and change the texts
                var quaterField = pivotTable.Fields.GetDateGroupField(eDateGroupBy.Quarters);
                quaterField.Items[0].Text = "<"; //Values below min date, but we use auto so its not used
                quaterField.Items[1].Text = "Q1";
                quaterField.Items[2].Text = "Q2";
                quaterField.Items[3].Text = "Q3";
                quaterField.Items[4].Text = "Q4";
                quaterField.Items[5].Text = ">"; //Values above max date, but we use auto so its not used                

                //Add a pagefield
                //var pageField = pivotTable.PageFields.Add(pivotTable.Fields["Sign"]);

                //Add the data fields and format them
                var dataField = pivotTable.DataFields.Add(pivotTable.Fields["Total in"]);
                dataField.Format = "#,##0";
                dataField = pivotTable.DataFields.Add(pivotTable.Fields["Total out"]);
                dataField.Format = "#,##0";

                //We want the datafields to appear in columns
                pivotTable.DataOnRows = false;

                pivotTable.WorkSheet.View.FreezePanes(3, 4);
                #endregion

                #region chart
                var chart = wsPivot.Drawings.AddChart("Chart" + worksheet.Name, eChartType.BarClustered, pivotTable);
                //chart.DataLabel.ShowCategory = true;
                //chart.DataLabel.ShowPercent = true;
                chart.SetPosition(3, 0, 5, 0);
                chart.SetSize(600, 800); 
                #endregion
            }
        }
    }
}
