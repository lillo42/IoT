using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace SendLog.Log
{
    internal class LogFile : ILog
    {
        private readonly string _fileName;
        private string _current;
        private StorageFile _file;
        private readonly CancellationToken _cancel;
        private long lastLog;
        public LogFile(string fileName )
        {
            _fileName = fileName;
        }

        public string Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public async void Add(string log)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.GetFileAsync(_fileName);
            await FileIO.WriteTextAsync(sampleFile, string.Concat(log, Environment.NewLine));
        }

        public void Dispose()
        {
            return;
        }

        public IEnumerator<string> GetEnumerator()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            IAsyncOperation<StorageFile> result = storageFolder.GetFileAsync("sample.txt");
            result.AsTask().Wait();
            StorageFile sampleFile = result.GetResults();

            IAsyncOperation<IList<string>> resultFile = FileIO.ReadLinesAsync(sampleFile);
            resultFile.AsTask().Wait();

            foreach (string s in resultFile.GetResults())
                yield return s;
        }

        private string NextString(StreamReader reader)
        {
            Task<string> taskString = reader.ReadLineAsync();
            taskString.Wait();
            return taskString.Result;
        }

        public bool MoveNext()
        {
            return true;
        }

        public void Reset()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
