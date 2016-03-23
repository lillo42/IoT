using SendLog.Log.File;
using SendLog.Log.Memory;
using System.Collections.Concurrent;

namespace SendLog.Log
{
    public class LogAsync
    {
        private static LogAsync _instance;
        private static object locker = new object();
        private ConcurrentQueue<string> _queueLog = new ConcurrentQueue<string>();
        private readonly LogMemory _logInMemory;
        private readonly LogFile _logInFile;
        private const string _filename = "log.txt";

        public static LogAsync Instance
        {
            get
            {
                //Double check-locking patterns
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                            _instance = new LogAsync();
                    }
                }
                return _instance;
            }
        }

        internal LogMemory LogInMemory
        {
            get
            {
                return _logInMemory;
            }
        }

        internal LogFile LogInFile
        {
            get
            {
                return _logInFile;
            }
        }

        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        private LogAsync()
        {
            _logInMemory = new LogMemory();
            _logInFile = new LogFile(Filename);
        }

        public void AddLog(string log)
        {
            LogInMemory?.Add(log);
            LogInFile?.Add(log);
        }
    }
}
