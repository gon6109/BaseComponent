using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BaseComponent
{
    /// <summary>
    /// ログ
    /// </summary>
    public static class Logger
    {
        class Log
        {
            public Status Status { get; }
            public string Message { get; }

            public Log(Status status, string message)
            {
                Status = status;
                Message = message;
            }
        }

        public enum Status
        {
            Error,
            Warning,
            Debug
        }

        static List<Log> Logs { get; } = new List<Log>();

        public static void Error(object obj)
        {
            AddLog(Status.Error, obj.ToString());
        }

        public static void Warning(object obj)
        {
            AddLog(Status.Warning, obj.ToString());
        }

        [Conditional("DEBUG")]
        public static void Debug(object obj)
        {
            AddLog(Status.Debug, obj.ToString());
        }

        public static void AddLog(Status status, string message)
        {
            var log = new Log(status, "[" + DateTime.Now.ToLongTimeString() + "] [" + status.ToString() + "]: " + message);
            Logs.Add(log);
        }

        public static void Save(string path)
        {
            using (var writer = new StreamWriter(path, false))
            {
                foreach (var item in Logs)
                {
                    writer.WriteLine(item.Message);
                }
            }
        }

        public static async Task SaveAsync(string path)
        {
            using (var writer = new StreamWriter(path, false))
            {
                foreach (var item in Logs)
                {
                    await writer.WriteLineAsync(item.Message);
                }
            }
        }
    }
}
