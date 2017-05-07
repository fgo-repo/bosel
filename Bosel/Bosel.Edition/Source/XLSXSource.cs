using Bosel.Edition.Utils;
using Bosel.Model.Source;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Bosel.Edition.Source
{
    public class XLSXSource : TextSource
    {
        public ExcelPackage Package { get; set; }
        private object Lock = new object();

        public XLSXSource() : base()
        {
            
        }

        public override void CreateBook(string path)
        {
            FileInfo newFile = new FileInfo(path);
            Package = new ExcelPackage(newFile);
        }

        public override bool AddSheets<T>(List<IGrouping<string, T>> data, int rowIndex=0, int clnIndex=0)
        {            
            try
            {
                if (data?.Count > 0)
                {
                    foreach (IGrouping<string, T> group in data)
                    {
                        AddSheet(group.ToList(), group.Key, rowIndex, clnIndex);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Logic.Report.XlsWorker.AddSheets] {0} Error when generating a report based on {1}:\n{2}", ex.GetType().Name, typeof(T).Name, ex);
                return false;
            }
        }

        public override void AddSheet<T>(List<T> data, string sheetName, int rowIndex = 0, int clnIndex = 0)
        {
            AddSheet(typeof(T), data, sheetName, rowIndex, clnIndex);
        }

        public override void AddSheet(Type type, IList data, string sheetName, int rowIndex = 0, int clnIndex = 0)
        {
            var properties = base.InitProperties(type);

            if (string.IsNullOrWhiteSpace(sheetName))
                return;

            lock (Lock)
            {
                ExcelWorksheet worksheet = Package.Workbook.Worksheets.Where(ws => ws.Name == sheetName).FirstOrDefault();
                if (worksheet == null)
                    worksheet = Package.Workbook.Worksheets.Add(sheetName);

                AddHeader(worksheet, properties, rowIndex, clnIndex);
                AddData(data, worksheet, properties, ++rowIndex, clnIndex);
                //AddBeauty(data, worksheet, properties, rowIndex, clnIndex); 
            }
        }

        private void AddHeader(ExcelWorksheet worksheet, List<Property> properties, int rowIndex, int clnIndex)
        {
            //title row
            for (int i = 0; i < properties.Count; i++)
            {
                worksheet.Cells[rowIndex +1, clnIndex + i + 1].Value = properties[i].ColumnOut;
            }
            using (var range = worksheet.Cells[rowIndex +1, clnIndex + 1, rowIndex +1, clnIndex + properties.Count])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                range.Style.Font.Color.SetColor(Color.White);
                range.AutoFilter = true;
            }
        }

        private void AddData(IList data, ExcelWorksheet worksheet, List<Property> properties, int rowIndex, int clnIndex)
        {
            object val;
            int cnt = properties.Count;
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < cnt; j++)
                {
                    if (properties[j].PropertyInfo.CanRead)
                    {
                        val = properties[j].PropertyInfo.GetValue(data[i], null);
                        if (val != null)
                        worksheet.SetVal(i + rowIndex + 1, j + clnIndex + 1, val, properties[j].Type);
                    }
                }
            }
            worksheet.Cells.AutoFitColumns();
        }
        private void AddBeauty(IList data, ExcelWorksheet worksheet, List<Property> properties, int rowIndex, int clnIndex)
        {
            int cnt = properties.Count;
            //border and color for all cells
            using (var range = worksheet.Cells[rowIndex + 1, clnIndex + 1, data.Count + rowIndex, clnIndex + cnt])
            {
                var border = range.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                //range.StyleName = "Medium2";// OfficeOpenXml.Table.TableStyles.Medium2;
            }
            //for (int i = rowIndex + 1; i < data.Count + clnIndex + 1; i++)
            //{
            //    if (i % 2 == 0)
            //    {
            //        using (var range = worksheet.Cells[i, 1, i, cnt])
            //        {
            //            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            //            range.Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
            //        }
            //    }
            //}

            //printersettings
            worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
            worksheet.PrinterSettings.FitToPage = true;
        }

        public override void Save()
        {
            if (Package != null && Package.Workbook.Worksheets.Count > 0)
            {
                try
                {
                    Package.Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Logic.Report.XlsWorker.AddSheets] {0} Error when saving a report:\n{1}", ex.GetType().Name, ex);
                }
                Package.Dispose();
            }
        }

        public override string GetExtension()
        {
            return ".xlsx";
        }

        public override List<T> GetSheet<T>(int rowIndex = 0, int index = 0, List<Property> properties = null)
        {
            if (properties == null)
            {
                properties = base.InitProperties<T>();
            }
            List<T> lst = new List<T>();


            ExcelWorksheet worksheet = Package.Workbook.Worksheets[index + 1];
            ExcelCellAddress startCell = worksheet.Dimension.Start;
            ExcelCellAddress endCell = worksheet.Dimension.End;

            var header = new Dictionary<int, string>();
            for (int col = startCell.Column; col <= endCell.Column; col++)
            {
                header.Add(col, worksheet.Cells[startCell.Row, col].Value.ToString());
            }

            var headerColumns = (from h in header
                                 join p in properties on h.Value equals p.ColumnIn
                                 select new KeyValuePair<int, Property>(h.Key, p)).ToDictionary(k => k.Key, k => k.Value);

            if (headerColumns != null && headerColumns.Count() > 0)
            {
                for (int row = startCell.Row + 1 + rowIndex; row <= endCell.Row; row++)
                {
                    T cls = Activator.CreateInstance<T>();
                    foreach (var cln in headerColumns)
                    {
                        Property property = headerColumns[cln.Key];
                        property.PropertyInfo.SetValue(cls, property.ConvertVal(worksheet.Cells[row, cln.Key].Value, property.Format), null);
                    }

                    lst.Add(cls);
                }
            }
            return lst;
        }

        public override List<IGrouping<string, T>> GetSheets<T>(int rowIndex = 0)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {                
                //Package.Dispose();
                Package = null;
            }
        }
    }

    public static class XlsxExtensions
    {
        public static Lazy<List<string>> ColumnsLetter = new Lazy<List<string>>(() => GetExcelStrings().ToList(), true);

        public static void SetVal(this ExcelWorksheet ws, int rowIndex, int clnIndex, object val)
        {
            ws.SetVal(rowIndex, clnIndex, val, TypeCode.String);
        }
        public static void SetVal(this ExcelWorksheet ws, int rowIndex, int clnIndex, object val, TypeCode Type)
        {
            ExcelRange cell;

            switch (Type)
            {
                case TypeCode.Char:
                case TypeCode.String:
                    ws.SetValue(rowIndex, clnIndex, val.ToString());
                    break;
                
                case TypeCode.DBNull:
                    break;
                case TypeCode.DateTime:
                    DateTime date;
                    if (DateTime.TryParse(val.ToString(), out date))
                    {
                        if (date == DateTime.MinValue) break;

                        ws.SetValue(rowIndex, clnIndex, date);
                        cell = ws.Cells[rowIndex, clnIndex];
                        cell.Style.Numberformat.Format = "dd/mm/yyyy";
                    }
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Decimal:
                    decimal dec;
                    if (decimal.TryParse(val.ToString(), out dec))
                        ws.SetValue(rowIndex, clnIndex, dec);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Double:
                    double dou;
                    if (double.TryParse(val.ToString(), out dou))
                        ws.SetValue(rowIndex, clnIndex, dou);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Empty:
                    break;
                case TypeCode.Int16:
                    Int16 int16;
                    if (Int16.TryParse(val.ToString(), out int16))
                        ws.SetValue(rowIndex, clnIndex, int16);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Int32:
                    Int32 int32;
                    if (Int32.TryParse(val.ToString(), out int32))
                        ws.SetValue(rowIndex, clnIndex, int32);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Int64:
                    Int64 int64;
                    if (Int64.TryParse(val.ToString(), out int64))
                        ws.SetValue(rowIndex, clnIndex, int64);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Boolean:
                    bool b;
                    if (bool.TryParse(val.ToString(), out b))
                        ws.SetValue(rowIndex, clnIndex, b);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Byte:
                    byte by;
                    if (byte.TryParse(val.ToString(), out by))
                        ws.SetValue(rowIndex, clnIndex, by);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.SByte:
                    sbyte sb;
                    if (SByte.TryParse(val.ToString(), out sb))
                        ws.SetValue(rowIndex, clnIndex, sb);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.Single:
                    Single single;
                    if (Single.TryParse(val.ToString(), out single))
                        ws.SetValue(rowIndex, clnIndex, single);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;                
                case TypeCode.UInt16:
                    UInt16 uint16;
                    if (UInt16.TryParse(val.ToString(), out uint16))
                        ws.SetValue(rowIndex, clnIndex, uint16);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.UInt32:
                    UInt32 uint32;
                    if (UInt32.TryParse(val.ToString(), out uint32))
                        ws.SetValue(rowIndex, clnIndex, uint32);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                case TypeCode.UInt64:
                    UInt64 uint64;
                    if (UInt64.TryParse(val.ToString(), out uint64))
                        ws.SetValue(rowIndex, clnIndex, uint64);
                    else
                        ws.SetVal(rowIndex, clnIndex, val);
                    break;
                default:
                    break;
            }
        }
        public static IEnumerable<string> GetExcelStrings()
        {
            var alphabet = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => ((Char)i).ToString());
            var lst = from c1 in alphabet
                      from c2 in alphabet
                      select c1 + c2;

            return alphabet.Concat(lst);
        }
        public static string GetColumnNme(int index)
        {
            int dividend = index;
            string rtnVal = string.Empty;
            int modulo = 0;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                rtnVal = string.Concat(Convert.ToChar(65 + modulo), rtnVal);
                dividend = Convert.ToInt32(Math.Round((dividend - modulo) / 26f, 0));
            }
            return rtnVal;
        }

        //public static double GetWidth(string font, int fontSize, string text)
        //{
        //    System.Drawing.Font stringFont = new System.Drawing.Font(font, fontSize);
        //    return GetWidth(stringFont, text);
        //}

        //public static double GetWidth(System.Drawing.Font stringFont, string text)
        //{
        //    // This formula is based on this article plus a nudge ( + 0.2M )
        //    // http://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet.column.width.aspx
        //    // Truncate(((256 * Solve_For_This + Truncate(128 / 7)) / 256) * 7) = DeterminePixelsOfString

        //    Size textSize = System.Windows.Forms.TextRenderer.MeasureText(text, stringFont);
        //    double width = (double)(((textSize.Width / (double)7) * 256) - (128 / 7)) / 256;
        //    width = (double)decimal.Round((decimal)width + 0.2M, 2);

        //    return width;
        //}
    }
}
