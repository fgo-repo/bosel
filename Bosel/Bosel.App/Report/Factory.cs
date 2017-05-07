using Bosel.Edition.Source;
using Bosel.Edition.Utils;
using Bosel.Model.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Utils;
using Bosel.Model.Common.Source;

namespace Bosel.App.Report
{
    public class Factory
    {
        /// <summary>
        /// The application path
        /// </summary>
        public static string AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        /// <summary>
        /// The configuration path
        /// </summary>
        public static string ConfigPath;
        private static List<Bank> _banks;
        /// <summary>
        /// Gets the banks settings.
        /// </summary>
        /// <value>
        /// The banks.
        /// </value>
        public static List<Bank> Banks
        {
            get
            {
                if (_banks == null)
                {
                    _banks = Extensions.GetJson<BankList>(Path.Combine(ConfigPath, "Banks.txt")).Banks;
                }
                return _banks;
            }
        }
        private static CategoryList _categoriesDef;
        /// <summary>
        /// Gets the categories definitions settings.
        /// </summary>
        /// <value>
        /// The categories definition.
        /// </value>
        public static CategoryList CategoriesDef
        {
            get
            {
                if (_categoriesDef == null)
                {
                    _categoriesDef = Extensions.GetJson<CategoryList>(Path.Combine(ConfigPath, "Categories.txt"));
                }
                return _categoriesDef;
            }
        }
        private static Dictionary<string, string> _categories;
        /// <summary>
        /// Gets the categories in a dictionnary (key = category code, value = category key).
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public static Dictionary<string, string> Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new Dictionary<string, string>();
                    CategoriesDef.Categories.ForEach(c => c.CategoryCodes.ForEach(t => _categories.Add(t.Code, c.Code)));
                }
                return _categories;
            }
        }
        /// <summary>
        /// Gets the data for all the customers.
        /// </summary>
        /// <value>
        /// The data global.
        /// </value>
        public static List<Data> DataGlobal { get; private set; }

        /// <summary>
        /// Gets the right reader/writer TextSource depending of the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static TextSource GetSource(string extension)
        {
            TextSource source = null;
            switch (extension.ToLower().Remove(0, 1))
            {
                case "csv":
                    source = new CSVSource();
                    break;
                case "xlsx":
                    source = new XLSXSource();
                    break;
                default:
                    break;
            }
            return source;
        }
        [Obsolete]
        public static Type GetType(string code)
        {
            switch (code)
            {
                case "BNP":
                    return typeof(DataBNP);
                case "ING":
                    return typeof(DataING);
                default:
                    return typeof(Data);
            }
        }

        /// <summary>
        /// Adds the data of customers in the global data.
        /// </summary>
        /// <param name="data">The data.</param>
        public static void AddGlobalData(List<Data> data)
        {
                if (DataGlobal == null)
                {
                    DataGlobal = new List<Data>();
                }

                DataGlobal.AddRange(data);
        }

        /// <summary>
        /// Gets the data grouped by month.
        /// </summary>
        /// <returns></returns>
        public static List<Data> GetDataGlobalGrouped()
        {
            //var dataMonths = from p in source
            //  let k = new
            //  {
            //      DateVal = p.DateVal > DateTime.MinValue ? p.DateVal.Date.AddDays(-1 * p.DateVal.Day - 1) : DateTime.MinValue,
            //      Category = p.Category,
            //      Sign = p.Sign
            //  }
            //  group p by k into t
            //  select new Data()
            //  {
            //      DateVal = t.Key.DateVal,
            //      Category = t.Key.Category,
            //      Sign = t.Key.Sign,
            //      Value = t.Sum(p => p.Value)
            //  };

            return (from p in DataGlobal
                    group p by new
                    {
                        DateVal = p.DateVal > DateTime.MinValue ? p.DateVal.Date.AddDays(-1 * p.DateVal.Day + 1) : DateTime.MinValue,
                        Category = p.Category,
                        Sign = p.Sign
                    } into t
                    select new Data()
                    {
                        DateVal = t.Key.DateVal,
                        Category = t.Key.Category,
                        Sign = t.Key.Sign,
                        Value = t.Sum(p => p.Value)
                    }).ToList();
        }

        /// <summary>
        /// Gets the data grouped by month.
        /// </summary>
        /// <returns></returns>
        public static List<DataPeriod> GetDataGlobalPeriod()
        {
            return (from p in DataGlobal
                    group p by new
                    {
                        DateVal = p.DateVal > DateTime.MinValue ? p.DateVal.Date.AddDays(-1 * p.DateVal.Day + 1) : DateTime.MinValue
                    } into t
                    select new DataPeriod()
                    {
                        DateVal = t.Key.DateVal,
                        TotalIn = t.Where(s => s.Value > 0).Sum(v => v.Value),
                        TotalOut = t.Where(s => s.Value < 0).Sum(v => v.Value) * -1
                    }).ToList();
        }

        [Obsolete]
        public static List<DataING> GetINGSheet(TextSource source, int rowIndex = 0, int index = 0)
        {
            var data = source.GetSheet<DataING>(rowIndex, index);
            foreach (var item in data)
            {
                item.Sign = item.Value > 0;
                string key = Factory.Categories.Keys.FirstOrDefault(k => (item.Customer + item.Detail).Contains(k));
                item.Category = !string.IsNullOrEmpty(key) ? Factory.Categories[key] : "Autres";
            }

            return data;
        }
        [Obsolete]
        public static List<DataBNP> GetBNPSheet(TextSource source, int rowIndex = 0, int index = 0)
        {
            var data = source.GetSheet<DataBNP>(rowIndex, index);
            foreach (var item in data)
            {
                item.Sign = item.Value > 0;
                string key = Factory.Categories.Keys.FirstOrDefault(k => (item.Customer + item.Detail).Contains(k));
                item.Category = !string.IsNullOrEmpty(key) ? Factory.Categories[key] : "Autres";
            }

            return data;
        }
        /// <summary>
        /// Gets the data sheet based on the default format.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="bankName">Name of the bank.</param>
        /// <returns></returns>
        public static List<Data> GetDataSheet(TextSource source, BankSetting settings, string bankName)
        {
            var data = source.GetSheet<Data>(settings.FileSetting?.RowIndex ?? 0, settings.FileSetting?.SheetIndex ?? 0);
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.Sign = item.Value > 0;
                    item.Bank = bankName ?? "Default";
                    if (string.IsNullOrWhiteSpace(item.Category))
                    {
                        string key = Factory.Categories.Keys.FirstOrDefault(k => (item.Customer + item.Detail).Contains(k));
                        item.Category = !string.IsNullOrEmpty(key) ? Factory.Categories[key] : "Autres";
                    }
                } 
            }

            return data;
        }
        /// <summary>
        /// Gets the data sheet based on the custom format.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="bankName">Name of the bank.</param>
        /// <returns></returns>
        public static List<Data> GetCustomDataSheet(TextSource source, BankSetting settings, string bankName)
        {
            var properties = source.GetProperties(settings.DataSetting.Pattern);
            List<Data> data = null;
            Func<Data, string> getKey = item => item.Customer + item.Detail;

            if (properties == null && settings.DataMapping?.MappingFields?.Count > 0)
            {
                properties = PropertyHelper.GetProperties<Data>(settings.DataMapping);
            }

            if (properties != null)
            {
                data = source.GetSheet<Data>(settings.FileSetting?.RowIndex ?? 0, settings.FileSetting?.SheetIndex ?? 0, properties);
                if (data != null)
                {
                    if(settings.DataMapping?.KeyFields?.Count > 0)
                    {
                        var keyProperties = source.InitProperties<Data>();
                        var usedProperties = new List<PropertyInfo>();

                        foreach (var prop in settings.DataMapping.KeyFields)
                        {
                            var property = keyProperties.FirstOrDefault(v => v.ColumnIn == prop);
                            if(property != null)
                            {
                                usedProperties.Add(property.PropertyInfo);
                            }
                        }
                        if (usedProperties.Count > 0)
                        {
                            getKey = item =>
                            {
                                var rtnVal = string.Empty;

                                foreach (var prop in usedProperties)
                                {
                                    rtnVal = string.Concat(rtnVal, (prop.GetValue(item, null) ?? string.Empty));
                                }

                                return settings.DataMapping.IsKeyUpper 
                                    ? rtnVal.ToUpper() 
                                    : rtnVal;
                            };
                        }
                    }

                    foreach (var item in data)
                    {
                        item.Sign = item.Value > 0;
                        item.Bank = bankName ?? "Custom";
                        if (string.IsNullOrWhiteSpace(item.Category))
                        {
                            string key = Factory.Categories.Keys.FirstOrDefault(k => getKey(item).Contains(k));
                            item.Category = !string.IsNullOrEmpty(key) ? Factory.Categories[key] : "Autres";
                        }
                    }  
                }
            }

            return data;
        }
        [Obsolete]
        public static void AddSheetsByClassMapping(TextSource output, IEnumerable<string> paths)
        {
            foreach (var bank in Factory.Banks)
            {
                var dico = new Dictionary<string, Data>();
                var lst = new List<Data>();
                Type t = GetType(bank.Name);

                foreach (var bankSetting in bank.Settings)
                {
                    var filesBank = paths.Where(f => f.Contains(bankSetting.DataSetting.Pattern));
                    if (filesBank?.Count() > 0)
                    {
                        foreach (var path in filesBank)
                        {
                            Console.WriteLine("Using the mapping {0} to read the file {1}", bank.Name, Path.GetFileName(path));
                            using (TextSource input = Factory.GetSource(Path.GetExtension(path)))
                            {
                                switch (bank.Name)
                                {
                                    case "BNP":
                                        input.CreateBook(path);
                                        GetBNPSheet(input).ForEach(i =>
                                        {
                                            var key = string.Concat(i.DateVal.ToString("yyyyMMdd"), i.Id);
                                            if (!dico.ContainsKey(key))
                                                dico.Add(key, i);
                                        });
                                        break;
                                    case "ING":
                                        input.CreateBook(path);
                                        GetINGSheet(input).ForEach(i =>
                                        {
                                            var key = string.Concat(i.DateVal.ToString("yyyyMMdd"), i.Id);
                                            if (!dico.ContainsKey(key))
                                                dico.Add(key, i);
                                        });
                                        break;
                                    default:
                                        input.CreateBook(path);
                                        GetDataSheet(input, bankSetting, bank.Name).ForEach(i =>
                                        {
                                            var key = string.Concat(i.DateVal.ToString("yyyyMMdd"), i.Id);
                                            if (!dico.ContainsKey(key))
                                                dico.Add(key, i);
                                        });
                                        break;
                                }
                            }
                        }
                    }

                    if (dico.Count > 0)
                    {
                        foreach (var item in dico.Values)
                        {
                            lst.Add(item);
                        }
                        lst.Sort();

                        Func<Data, bool> filter = v => true;
                        if (bankSetting.DataSetting.MaxVal > 0)
                            filter = v => Math.Abs(v.Value).IsBetween(bankSetting.DataSetting.MinVal, bankSetting.DataSetting.MaxVal);
                        else if (bankSetting.DataSetting.MinVal > 0)
                            filter = v => Math.Abs(v.Value) > bankSetting.DataSetting.MinVal;

                        lst = lst.Where(filter).ToList(); 
                    }
                }
                output.AddSheet(t, lst, bank.Name);
                Factory.AddGlobalData(lst);
            }
        }

        /// <summary>
        /// Adds all the data from the paths in a file.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="paths">The paths.</param>
        public static void AddSheets(TextSource output, IEnumerable<string> paths)
        {
            foreach (var bank in Factory.Banks)
            {
                var lst = new List<Data>();

                foreach (var bankSetting in bank.Settings)
                {
                    var filesBank = paths.Where(f => f.Contains(bankSetting.DataSetting.Pattern));
                    if (filesBank?.Count() > 0)
                    {
                        foreach (var path in filesBank)
                        {
                            Console.WriteLine("Using the mapping {0} to read the file {1}", bank.Name, Path.GetFileName(path));
                            using (TextSource input = Factory.GetSource(Path.GetExtension(path)))
                            {
                                input.CreateBook(path);
                                List<Data> data = null;
                                if (bankSetting == null)
                                {
                                    data = GetDataSheet(input, bankSetting, bank.Name); 
                                }
                                else
                                {
                                    data = GetCustomDataSheet(input, bankSetting, bank.Name);
                                }
                                if(data != null)
                                {
                                    lst.AddRange(data);
                                }
                            }
                        }
                    }
                    
                    if(lst.Count > 0)
                    {
                        lst = lst.GroupBy(x => string.Concat(x.DateVal.ToString("yyyyMMdd"), x.Id)).Select(g => g.First()).ToList();
                        lst.Sort();

                        Func<Data, bool> filter = v => true;
                        if (bankSetting.DataSetting.MaxVal > 0)
                            filter = v => Math.Abs(v.Value).IsBetween(bankSetting.DataSetting.MinVal, bankSetting.DataSetting.MaxVal);
                        else if (bankSetting.DataSetting.MinVal > 0)
                            filter = v => Math.Abs(v.Value) > bankSetting.DataSetting.MinVal;

                        lst = lst.Where(filter).ToList();
                    } 
                }

                if (bank.IncludeBankSheet)
                {
                    output.AddSheet(lst, bank.Name);
                }
                Factory.AddGlobalData(lst);
            }
        }


        //public static void AddSheetsAsync2(TextSource output, IEnumerable<string> paths)
        //{
        //    int cnt = Factory.Banks.Count;
        //    if (cnt == 0)
        //        return;

        //    var _lock = new object();
        //    var _countdown = new CountdownEvent(cnt);
        //    using (var concurrencySemaphore = new SemaphoreSlim(Environment.ProcessorCount))
        //    {
        //        foreach (var bank in Factory.Banks)
        //        {
        //            var filesBank = paths.Where(f => f.Contains(bank.Pattern));
        //            if (filesBank?.Count() > 0)
        //            {
        //                Task.Factory.StartNew(fileBankList =>
        //                {
        //                    Type t = GetType(bank.Name);
        //                    //var listType = typeof(List<>);
        //                    //var constructedListType = listType.MakeGenericType(t);
        //                    //var lst = (IList)Activator.CreateInstance(constructedListType);

        //                    //var constructedDicoType = typeof(ConcurrentDictionary<,>).MakeGenericType(typeof(string), t);
        //                    //var dico = (IDictionary)Activator.CreateInstance(constructedDicoType);
        //                    //var dico = new ConcurrentDictionary<string, Data>();
        //                    var dico = new Dictionary<string, Data>();
        //                    var filteredFilesBank = fileBankList as IEnumerable<string>;
        //                    var countdownFilesBank = new CountdownEvent(filteredFilesBank.Count());

        //                    var _lockTsk = new object();

        //                    foreach (var path in filteredFilesBank)
        //                    {
        //                        Task.Factory.StartNew(fileBankPath =>
        //                        {
        //                            concurrencySemaphore.Wait();
        //                            var currentFileBankPath = fileBankPath.ToString();
        //                            Console.WriteLine("Using the mapping {0} to read the file {1}", bank.Name, Path.GetFileName(currentFileBankPath));
        //                            using (TextSource input = Factory.GetSource(Path.GetExtension(currentFileBankPath)))
        //                            {
        //                                try
        //                                {
        //                                    switch (bank.Name)
        //                                    {
        //                                        case "BNP":
        //                                            input.CreateBook(currentFileBankPath);
        //                                            //GetBNPSheet(input).ForEach(i => { if (!dico.Contains(i.Id)) dico.Add(i.Id, i); });
        //                                            //GetBNPSheet(input).ForEach(i => { if (!dico.ContainsKey(i.Id)) dico.TryAdd(i.Id, i); });
        //                                            var lstBNP = GetBNPSheet(input);
        //                                            lock (_lockTsk)
        //                                                lstBNP.ForEach(i => {
        //                                                    var key = string.Concat(i.DateVal.ToString("yyyyMMdd"), i.Id);
        //                                                    if (!dico.ContainsKey(key))
        //                                                        dico.Add(key, i);
        //                                                });

        //                                            break;
        //                                        case "ING":
        //                                            input.CreateBook(currentFileBankPath);
        //                                            //GetINGSheet(input).ForEach(i => { if (!dico.Contains(i.Id)) dico.Add(i.Id, i); });
        //                                            //GetINGSheet(input).ForEach(i => { if (!dico.ContainsKey(i.Id)) dico.TryAdd(i.Id, i); });
        //                                            var lstING = GetINGSheet(input);
        //                                            lock (_lockTsk)
        //                                                lstING.ForEach(i => {
        //                                                    var key = string.Concat(i.DateVal.ToString("yyyyMMdd"), i.Id);
        //                                                    if (!dico.ContainsKey(key))
        //                                                        dico.Add(key, i);
        //                                                });
        //                                            break;
        //                                        default:

        //                                            input.CreateBook(currentFileBankPath);
        //                                            //GetDataSheet(input).ForEach(i => { if (!dico.Contains(i.Id)) dico.Add(i.Id, i); });
        //                                            //GetDataSheet(input).ForEach(i => { if (!dico.ContainsKey(i.Id)) dico.TryAdd(i.Id, i); });
        //                                            var lst = GetDataSheet(input);
        //                                            lock (_lockTsk)
        //                                                lst.ForEach(i => {
        //                                                    var key = string.Concat(i.DateVal.ToString("yyyyMMdd"), i.Id);
        //                                                    if (!dico.ContainsKey(key))
        //                                                        dico.Add(key, i);
        //                                                });
        //                                            break;
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    Console.WriteLine(string.Format("[{0}] Code {1}: error during generation", currentFileBankPath, bank.Name), ex);
        //                                }
        //                                finally
        //                                {
        //                                    concurrencySemaphore.Release();
        //                                    countdownFilesBank.Signal();
        //                                }
        //                            }
        //                        }, path, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
        //                    }

        //                    countdownFilesBank.Wait();                            

        //                    try
        //                    {
        //                        var lst = new List<Data>();
        //                        foreach (var item in dico.Values)
        //                        {
        //                            lst.Add(item);
        //                        }
        //                        lst.Sort();

        //                        Func<Data, bool> filter = v => true;
        //                        if (bank.MaxVal > 0)
        //                            filter = v => Math.Abs(v.Value).IsBetween(bank.MinVal, bank.MaxVal);
        //                        else if (bank.MinVal > 0)
        //                            filter = v => Math.Abs(v.Value) > bank.MinVal;

        //                        lock (_lock)
        //                            output.AddSheet(t, lst, bank.Name);
        //                        Factory.AddGlobalData(lst.Where(filter).ToList());

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine(string.Format("Code {0}: error during generation: {1}", bank.Name, ex.ToString()));
        //                    }
        //                    finally
        //                    {
        //                        _countdown.Signal();
        //                    }
        //                }, filesBank);
        //            }
        //            else
        //            {
        //                _countdown.Signal();
        //            }
        //        }
        //        _countdown.Wait();
        //    }
        //}

        //public static void AddSheetsAsync(TextSource output, IEnumerable<string> paths)
        //{
        //    int cnt = Factory.Banks.Count;
        //    if (cnt == 0)
        //        return;

        //    var _globalLock = new object();
        //    var _countdown = new CountdownEvent(cnt);
        //    using (var concurrencySemaphore = new SemaphoreSlim(Environment.ProcessorCount))
        //    {
        //        foreach (var bank in Factory.Banks)
        //        {
        //            var filesBank = paths.Where(f => f.Contains(bank.Pattern));
        //            if (filesBank?.Count() > 0)
        //            {
        //                Task.Factory.StartNew(fileBankList =>
        //                {
        //                    Type t = GetType(bank.Name);
        //                    //var listType = typeof(List<>);
        //                    //var constructedListType = listType.MakeGenericType(t);
        //                    //var lst = (IList)Activator.CreateInstance(constructedListType);

        //                    //var constructedDicoType = typeof(ConcurrentDictionary<,>).MakeGenericType(typeof(string), t);
        //                    //var dico = (IDictionary)Activator.CreateInstance(constructedDicoType);
        //                    //var dico = new ConcurrentDictionary<string, Data>();
        //                    var lst = new List<Data>();
        //                    var filteredFilesBank = fileBankList as IEnumerable<string>;
        //                    var countdownFilesBank = new CountdownEvent(filteredFilesBank.Count());

        //                    var _lockTsk = new object();

        //                    foreach (var path in filteredFilesBank)
        //                    {
        //                        Task.Factory.StartNew(fileBankPath =>
        //                        {
        //                            concurrencySemaphore.Wait();
        //                            var currentFileBankPath = fileBankPath.ToString();
        //                            Console.WriteLine("Using the mapping {0} to read the file {1}", bank.Name, Path.GetFileName(currentFileBankPath));
        //                            using (TextSource input = Factory.GetSource(Path.GetExtension(currentFileBankPath)))
        //                            {
        //                                try
        //                                {
        //                                    switch (bank.Name)
        //                                    {
        //                                        case "BNP":
        //                                            input.CreateBook(currentFileBankPath);
        //                                            //GetBNPSheet(input).ForEach(i => { if (!dico.Contains(i.Id)) dico.Add(i.Id, i); });
        //                                            //GetBNPSheet(input).ForEach(i => { if (!dico.ContainsKey(i.Id)) dico.TryAdd(i.Id, i); });
        //                                            var lstBNP = GetBNPSheet(input);
        //                                            lock (_lockTsk)
        //                                                lst.AddRange(lstBNP);
        //                                            break;
        //                                        case "ING":
        //                                            input.CreateBook(currentFileBankPath);
        //                                            //GetINGSheet(input).ForEach(i => { if (!dico.Contains(i.Id)) dico.Add(i.Id, i); });
        //                                            //GetINGSheet(input).ForEach(i => { if (!dico.ContainsKey(i.Id)) dico.TryAdd(i.Id, i); });
        //                                            var lstING = GetINGSheet(input);
        //                                            lock (_lockTsk)
        //                                                lst.AddRange(lstING);
        //                                            break;
        //                                        default:

        //                                            input.CreateBook(currentFileBankPath);
        //                                            //GetDataSheet(input).ForEach(i => { if (!dico.Contains(i.Id)) dico.Add(i.Id, i); });
        //                                            //GetDataSheet(input).ForEach(i => { if (!dico.ContainsKey(i.Id)) dico.TryAdd(i.Id, i); });
        //                                            var lstData = GetDataSheet(input);
        //                                            lock (_lockTsk)
        //                                                lst.AddRange(lstData);
        //                                            break;
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    Console.WriteLine(string.Format("[{0}] Code {1}: error during generation", currentFileBankPath, bank.Name), ex);
        //                                }
        //                                finally
        //                                {
        //                                    concurrencySemaphore.Release();
        //                                    countdownFilesBank.Signal();
        //                                }
        //                            }
        //                        }, path, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
        //                    }

        //                    countdownFilesBank.Wait();

        //                    try
        //                    {
        //                        lst = lst.GroupBy(x => string.Concat(x.DateVal.ToString("yyyyMMdd"), x.Id)).Select(g => g.First()).ToList();
        //                        lst.Sort();

        //                        Func<Data, bool> filter = v => true;
        //                        if (bank.MaxVal > 0)
        //                            filter = v => Math.Abs(v.Value).IsBetween(bank.MinVal, bank.MaxVal);
        //                        else if (bank.MinVal > 0)
        //                            filter = v => Math.Abs(v.Value) > bank.MinVal;

        //                        lock (_globalLock)
        //                        {
        //                            output.AddSheet(t, lst, bank.Name);
        //                            Factory.AddGlobalData(lst.Where(filter).ToList());
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.WriteLine(string.Format("Code {0}: error during generation: {1}", bank.Name, ex.ToString()));
        //                    }
        //                    finally
        //                    {
        //                        _countdown.Signal();
        //                    }
        //                }, filesBank);
        //            }
        //            else
        //            {
        //                _countdown.Signal();
        //            }
        //        }
        //        _countdown.Wait();
        //    }
        //}
    }
}