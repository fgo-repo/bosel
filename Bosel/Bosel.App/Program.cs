using Bosel.App.Report;
using Bosel.Model.Common.Source;
using Bosel.Model.Report;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utils;

namespace Bosel.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //log all the stuffs from the console in a log file
            var logger = new ConsoleLog();
            var paths = GetPaths();
            var outPath = Extensions.CreateDirForFile(paths.PathOut, string.Format("output_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-ddTHHmmsszz")));
            Factory.ConfigPath = paths.PathSettings;
            var files = Extensions.GetFiles(paths.PathIn).OrderBy(n => n);
            
            if (files?.Count() > 0)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var output = new BankReport();
                output.CreateBook(outPath);
                Factory.AddSheets(output, files);

                if (Factory.DataGlobal?.Count > 0)
                {
                    Factory.DataGlobal.Sort();
                    output.AddSheetTotal<Data>(Factory.DataGlobal, "Total");
                    //output.AddSheetTotal<Data>(Factory.GetDataGlobalGrouped(), "TotalGrouped");
                    output.AddSheetTotal<DataPeriod>(Factory.GetDataGlobalPeriod(), "Period");
                }
                else
                {
                    Console.WriteLine("No data found, check your mappings");
                }

                output.Save();

                stopwatch.Stop();
                Console.WriteLine("Time elapsed: {0:00}:{1:00}:{2:00} {3:000}", stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds);

                OpenFile(outPath);
            }
            else
            {
                Console.WriteLine("No files found in {0}. Add your csv, xlsx files in this directory.", paths.PathIn);
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Opens the file and kill the other useless instances of Excel.
        /// </summary>
        /// <param name="path">The path.</param>
        private static void OpenFile(string path)
        {
            if (!File.Exists(path)) return;

            var processes = Process.GetProcessesByName("EXCEL");

            Process.Start(path);

            if (processes?.Length > 0)
            {
                Process process = (from p in Process.GetProcessesByName("EXCEL")
                                   where !(processes.Any(a => a.Id == p.Id))
                                   select p).SingleOrDefault();
                
                if (process != null && !process.HasExited)
                    process.Kill();
            }
        }

        /// <summary>
        /// Gets the paths from the config file.
        /// </summary>
        /// <returns></returns>
        private static Paths GetPaths()
        {
            var paths = new Paths();
            var settingsPath = Path.Combine(Factory.AppPath, "Config", "Paths.txt");
            if(File.Exists(settingsPath))
            {
                paths = settingsPath.GetJson<Paths>();
            }
            if(!Directory.Exists(paths.PathIn))
            {
                Console.WriteLine("Input Path '{0}' doesn't exist. It'll be replaced by the default path '{1}'", paths.PathIn, Path.Combine(Factory.AppPath, "in"));
                paths.PathIn = string.Empty;
            }
            paths.PathIn = !string.IsNullOrEmpty(paths.PathIn) ? paths.PathIn : Path.Combine(Factory.AppPath, "in");
            paths.PathOut = !string.IsNullOrEmpty(paths.PathOut) ? paths.PathOut : Path.Combine(Factory.AppPath, "out");
            paths.PathSettings = !string.IsNullOrEmpty(paths.PathSettings) ? paths.PathSettings : Path.Combine(Factory.AppPath, "Config");

            TestPath(paths.PathIn);
            TestPath(paths.PathSettings);

            return paths;
        }

        /// <summary>
        /// Tests the path if exists, unless warn the user.
        /// </summary>
        /// <param name="path">The path.</param>
        private static void TestPath(string path)
        {
            if(!Directory.Exists(path))
            {
                Console.WriteLine("WARNING: setting path '{0}' not found.", path);
            }
        }       
    }
}
