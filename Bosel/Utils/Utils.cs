using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class Extensions
    {
        public static string SetJson(this object obj, bool indented = true)
        {
            return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
        }

        public static T GetJson<T>(this string path) where T : class, new()
        {

            List<string> errors = new List<string>();
            T jsonData = JsonConvert.DeserializeObject<T>(File.ReadAllText(path),
                                            new JsonSerializerSettings()
                                            {
                                                Error = (sender, args) =>
                                                {
                                                    errors.Add(args.ErrorContext.Error.Message);
                                                    args.ErrorContext.Handled = true; //true: continue
                                                }
                                            });
            if (errors.Count > 0)
            {
                Console.WriteLine("error in Json: \n{0}", string.Join(Environment.NewLine, errors));
            }

            return jsonData;
        }

        private static Regex MinifyJsonSpace = new Regex("(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", RegexOptions.Compiled | RegexOptions.Multiline);

        public static string MinifyJson(string json)
        {
            return MinifyJsonSpace.Replace(json, "$1");
        }

        public static string Dump(this object element)
        {
            return SetJson(element, true);
        }

        private static void Backup(string dirRoot, string dirDest)
        {
            Console.WriteLine("{0} -- {1}", dirRoot, dirDest);

            if (!Directory.Exists(dirDest))
                Directory.CreateDirectory(dirDest);

            foreach (var file in Directory.GetFiles(dirRoot))
            {
                string outputFile = Path.Combine(dirDest, Path.GetFileName(file));
                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                File.Copy(file, outputFile);
            }
            foreach (var dir in Directory.GetDirectories(dirRoot))
            {
                Backup(dir, string.Concat(dirDest, dir.Replace(dirRoot, string.Empty)));
            }
        }

        public static void Merge(string dirRoot, string outputFile)
        {
            var newFile = Path.Combine(dirRoot, outputFile);
            var filter = string.Concat("*", Path.GetExtension(newFile));

            foreach (var file in Directory.GetFiles(dirRoot, filter).Where(nf => !nf.EndsWith(outputFile)))
            {
                File.AppendAllText(newFile, File.ReadAllText(file));
                File.Delete(file);
            }
        }

        public static string CreateDirForFile(string dir, string file)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return Path.Combine(dir, file);
        }

        public static IEnumerable<string> GetFiles(string path)
        {
            var files = Directory.GetFiles(path).ToList();
            var subFiles = Directory.GetDirectories(path).SelectMany(d => GetFiles(d));
            return files.Concat(subFiles);
        }

        public static bool In<T>(this T obj, params T[] values)
        {
            foreach (T val in values)
            {
                if (obj.Equals(val))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable
        {
            return min.CompareTo(value) < 0 && value.CompareTo(max) < 0;
        }
    }
}