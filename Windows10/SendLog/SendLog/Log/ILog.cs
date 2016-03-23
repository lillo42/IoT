using System.Collections.Generic;

namespace SendLog.Log
{
    internal interface ILog : IEnumerator<string>, IEnumerable<string>
    {
        void Add(string log);
    }
}
