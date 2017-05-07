using Bosel.Edition.Utils;
using Bosel.Model.Source;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bosel.Edition.Source
{
    public abstract class TextSource:IDisposable
    {
        public string Path { get; set; }
        private Dictionary<string, List<Property>> Library { get; set; }
        public string _fileName=string.Empty;
        public string FileName { get
            {
                if (string.IsNullOrEmpty(_fileName) && !string.IsNullOrEmpty(Path))
                    _fileName = System.IO.Path.GetFileName(Path);

                return _fileName;
            }
        }

        public TextSource()
        {
            Library = new Dictionary<string, List<Property>>();
        }

        public List<Property> InitProperties<T>() where T : class, new()
        {
            return InitProperties(typeof(T));
        }
        public List<Property> InitProperties(Type type)
        {
            string key = type.Name;
            List<Property> properties = GetProperties(key);

            if(properties == null)
            {
                properties = PropertyHelper.GetProperties(type);
                Library.Add(key, properties);
            }
            return properties;
        }
        public List<Property> GetProperties(string key)
        {
            List<Property> properties = null;

            if (Library.ContainsKey(key))
            {
                properties = Library[key];
            }
            return properties;
        }

        public MemoryStream GetStream(string path)
        {
            var stream = new MemoryStream();
            if (File.Exists(path))
            {
                using (var file = File.OpenRead(path))
                {
                    int localBufferSize = 4096;   // 4 KB
                    byte[] buffer = new byte[localBufferSize];
                    for (var i = 0; i < file.Length / localBufferSize; i++)
                    {    
                        file.Read(buffer, i * localBufferSize, localBufferSize);
                        stream.Write(buffer, i * localBufferSize, localBufferSize);
                    }
                }
            }

            return stream;
        }

        public abstract void CreateBook(string path);
        public abstract bool AddSheets<T>(List<IGrouping<string, T>> data, int rowIndex=0, int clnIndex=0) where T : class, new();
        public abstract void AddSheet<T>(List<T> data, string sheetName, int rowIndex = 0, int clnIndex = 0) where T : class, new();
        public abstract void AddSheet(Type type, IList data, string sheetName, int rowIndex = 0, int clnIndex = 0);
        public abstract void Save();
        public abstract string GetExtension();

        public abstract List<T> GetSheet<T>(int rowIndex = 0, int index = 0, List<Property> properties = null) where T : class, new();
        public abstract List<IGrouping<string, T>> GetSheets<T>(int rowIndex = 0) where T : class, new();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~TextSource() 
        {
            Dispose(false);
        }

        protected abstract void Dispose(bool disposing);
    }
}
