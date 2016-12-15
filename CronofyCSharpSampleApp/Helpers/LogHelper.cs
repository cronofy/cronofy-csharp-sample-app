using System;
using System.IO;

namespace CronofyCSharpSampleApp
{
    public static class LogHelper
    {
        public static void Log(string line)
        {
            using (StreamWriter w = File.AppendText($"{Environment.CurrentDirectory}/Logs/info.txt"))
            {
                w.WriteLine($"{DateTime.Now} - {line}");
            }
        }
    }
}
