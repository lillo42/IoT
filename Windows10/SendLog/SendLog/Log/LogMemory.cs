using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SendLog.Log
{
    internal class LogMemory : ILog
    {
        private ConcurrentQueue<string> _queueLog = new ConcurrentQueue<string>();
        private bool dispose;
        private string _current;

        public string Current
        {
            get
            {
                return _current;
            }
        }

        public int Count
        {
            get
            {
                return _queueLog.Count;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Add(string log)
        {
            _queueLog.Enqueue(log);
        }

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            //return _queueLog.Count > 300;
            return true;
        }

        public void Reset()
        {

        }

        public IEnumerator<string> GetEnumerator()
        {
            while (_queueLog.TryDequeue(out _current)) //&& _queueLog.Count > 300)
                yield return _current;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
