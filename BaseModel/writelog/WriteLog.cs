using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseModel.log
{
    public class WriteLog
    {
        private static string LogPath = AppDomain.CurrentDomain.BaseDirectory;
        private static object objLock = new object();
        public static void WriteLogFile(string text)
        {
            string saveLogPath = Path.Combine(LogPath, "log", DateTime.Now.ToString("yyyy-MM-dd"));
            string savelogFile = Path.Combine(saveLogPath, DateTime.Now.Hour + ".log");
            lock (objLock)
            {
                if (Directory.Exists(saveLogPath))
                {
                    File.AppendAllText(savelogFile, DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss") + Environment.NewLine + text + Environment.NewLine);
                }
                else
                {
                    Directory.CreateDirectory(saveLogPath);
                    WriteLogFile(text);
                }
            }
        }
        public static void ConsoleWritelog(string text)
        {
            //text.ToCharArray().ToList().ForEach(s => { Console.WriteLine(s);Thread.Sleep(50); });
            Console.WriteLine(text);
            WriteLogFile(text);
        }
    }
}
