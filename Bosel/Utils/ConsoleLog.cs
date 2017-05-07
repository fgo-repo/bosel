using System;
using System.IO;
using System.Threading;

namespace Utils
{
    public class ConsoleLog : TextWriter
    {
        #region Events
        public event EventHandler<BeforWriteEventArgs> BeforeWrite;
        public event EventHandler<AfterWriteEventArgs> AfterWrite;
        protected void OnAfterWrite(string value)
        {
            if (AfterWrite != null)
                AfterWrite(this, new AfterWriteEventArgs(value));
        }
        protected BeforWriteEventArgs OnBeforWrite(string value)
        {
            BeforWriteEventArgs arg = new BeforWriteEventArgs(value);
            if (BeforeWrite != null)
                BeforeWrite(this, arg);
            return arg;
        }
        #endregion

        #region Variables
        TextWriter _out;
        private Mutex _Mutex;
        private string _path;
        #endregion

        #region Constructors
        public ConsoleLog() : this("ConsoleLog.log") { }
        public ConsoleLog(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _Mutex = new Mutex(false, System.Reflection.Assembly.GetExecutingAssembly().FullName);
                _path = path;
                _out = Console.Out;
                Console.SetOut(this);
            }
        }
        ~ConsoleLog()
        {
            Dispose(false);
        }
        protected override void Dispose(bool disposing)
        {
            _Mutex.Close();
            base.Dispose(disposing);
            Console.SetOut(_out);
        }
        #endregion

        #region Write Methode
        public override void Write(char[] buffer, int index, int count)
        {
            BeforWriteEventArgs arg = OnBeforWrite(new string(buffer));
            string value = string.Format("[{0}] {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), arg.Value);

            if (arg.WriteToLog)
            {
                _Mutex.WaitOne();
                using (StreamWriter _StreamWriter = new StreamWriter(_path, true))
                {
                    _StreamWriter.Write(value);
                    _StreamWriter.Flush();
                    _StreamWriter.Close();
                }
                _Mutex.ReleaseMutex();
            }

            if (arg.ShowInConsole)
                this._out.Write(value);
            if (arg.WriteToLog || arg.ShowInConsole)
                OnAfterWrite(value);
        }
        #endregion

        #region TextWriter Override
        public override System.Text.Encoding Encoding
        { get { return this._out.Encoding; } }
        #endregion
    }

    #region Class EventHandler
    public class AfterWriteEventArgs : EventArgs
    {
        private string _value;
        public string Value { get { return this._value; } set { _value = value; } }
        internal AfterWriteEventArgs(string value) { this._value = value; }
    }
    public class BeforWriteEventArgs : AfterWriteEventArgs
    {
        private bool _writeToLog = true;
        public bool WriteToLog { get { return _writeToLog; } set { _writeToLog = value; } }
        private bool _showInConsole = true;
        public bool ShowInConsole { get { return _showInConsole; } set { _showInConsole = value; } }
        internal BeforWriteEventArgs(string value) : base(value) { }
    }
    #endregion
}
