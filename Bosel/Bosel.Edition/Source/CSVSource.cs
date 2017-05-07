using Bosel.Edition.Utils;
using Bosel.Model.Source;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bosel.Edition.Source
{
    public class CSVSource : TextSource
    {
        private Char Delimiter;
        private const Char Quote = '"';        

        public CSVSource():base()
        {
            
        }

        public override void CreateBook(string path)
        {
            Path = path;
        }

        public override bool AddSheets<T>(List<IGrouping<string, T>> data, int rowIndex=0, int clnIndex=0)
        {
            throw new NotImplementedException();
        }

        public override void AddSheet<T>(List<T> data, string sheetName, int rowIndex = 0, int clnIndex = 0)
        {           
            throw new NotImplementedException();
        }

        public override void AddSheet(Type type, IList data, string sheetName, int rowIndex = 0, int clnIndex = 0)
        {

        }

        public override void Save()
        {
            throw new NotImplementedException();
        }

        public override string GetExtension()
        {
            return ".csv";
        }

        public override List<T> GetSheet<T>(int rowIndex = 0, int index = 0, List<Property> properties = null)
        {
            if (properties == null)
            {
                properties = base.InitProperties<T>(); 
            }

            List<T> lst = new List<T>();
            //var file = string.Empty;
            IDictionary<int, string> header;

            if (File.Exists(Path))
            {
                using (StreamReader sr = new StreamReader(Path, Encoding.GetEncoding("iso-8859-1")))
                {
                    for (int i = 0; i < rowIndex; i++)
                    {
                        sr.ReadLine();
                    }
                    header = this.ParseLine(sr.ReadLine());
                    //file = ReadFile(sr);
                
                    var headerColumns = (from h in header
                                         let i = 0
                                        join p in properties on h.Value.Trim() equals p.ColumnIn
                                        select new KeyValuePair<int, Property>(h.Key, p)).ToList();

                    if (headerColumns?.Count > 0)
                    {                        
                        int importedLine = 0;
                        //foreach (var line in this.ReadLine(file, file.Length))
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            importedLine++;
                            var lineValues = this.ParseLine(line);
                            if (lineValues.Count <= 1)
                            {
                                Console.WriteLine("{0}: Line {1} skipped: The line is empty", FileName, importedLine);
                                continue;
                            } 
                            T cls = Activator.CreateInstance<T>();
                            for (int i = 0; i < headerColumns.Count; i++)
                            {
                                Property property = headerColumns[i].Value;                                
                                property.PropertyInfo.SetValue(cls, property.ConvertVal(lineValues[headerColumns[i].Key], property.Format), null);
                            }
                            lst.Add(cls);
                        }
                    }
                }
            }
            return lst;
        }

        public override List<IGrouping<string, T>> GetSheets<T>(int rowIndex = 0)
        {
            throw new NotImplementedException();
        }

        private String ReadFile(StreamReader reader)
        {
            var contentToProcess = new StringBuilder();
            var buffer = new char[1024 * 1024];
            var charsRead = 0;

            while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                contentToProcess.Append(new string(buffer, 0, charsRead));

            return contentToProcess.ToString();
        }

        private IEnumerable<String> ReadLine(String file, int fileLength)
        {
            var line = new StringBuilder();
            for (Int32 i = 0; i < file.Length; i++)
            {
                if (i + 1 < file.Length && file[i] == '\r' && file[i + 1] == '\n') //Windows eof format
                {
                    i++;
                    yield return line.ToString();
                    line.Clear();
                }
                else if (fileLength > i)
                    line.Append(file[i]);
            }
            yield return line.ToString();
        }

        private Dictionary<int, string> ParseLine(String line)
        {
            var res = new Dictionary<int, string>();
            var l = line;
            var colNbr = 0;
            var sb = new StringBuilder();
            var isInQuotes = false;
            char c;
            for (Int32 i = 0; i < l.Length; i++)
            {
                c = l[i];

                if (Delimiter == 0 && (c == ';' || c == ',' || c == '|')) //Delimiter is first ; or , found in header line
                    Delimiter = c;

                if (c == Quote)
                {
                    if (isInQuotes)
                    {
                        if (i < l.Length - 1 && l[i + 1] == Quote)
                        {
                            sb.Append(c);
                            i++;
                        }
                        else
                            isInQuotes = false;
                    }
                    else
                    {
                        if (sb.Length == 0)
                        {
                            isInQuotes = true;
                        }
                        else
                            sb.Append(c);
                    }
                }
                else if (c == Delimiter)
                {
                    if (isInQuotes)
                        sb.Append(c);
                    else
                    {
                        res.Add(colNbr, sb.ToString());
                        sb.Length = 0;
                        colNbr++;
                    }
                }
                else
                    sb.Append(c);
            }
            res.Add(colNbr, sb.ToString());

            return res;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }
    }
}
