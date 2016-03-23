using SendLog.Log;
using System.Threading.Tasks;

namespace SendLog.Productor
{
    internal class Productor
    {
        public Task BeginCreateLog(bool memory, bool file)
        {
            return new TaskFactory().StartNew(() => GenerateLog(memory, file));
        }

        private async void GenerateLog(bool memory, bool file)
        {
            string pattern = "Teste{0}";
            uint i = 0;
            bool canLog = true;
            while (true)
            {
                if (canLog)
                {
                    string log = string.Format(pattern, i);
                    if (memory && LogAsync.Instance.LogInMemory.Count < 10)
                        LogAsync.Instance.LogInMemory.Add(log);
                    else
                        await Task.Delay(1000);
                    if (file)
                        LogAsync.Instance.LogInFile.Add(log);
                    i++;
                }
                await Task.Delay(100);
            }
        }
    }
}
