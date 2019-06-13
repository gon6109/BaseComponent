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
    /// ログ管理クラス
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// ログ
        /// </summary>
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

        /// <summary>
        /// ログ状態
        /// </summary>
        public enum Status
        {
            Error,
            Warning,
            Debug
        }

        /// <summary>
        /// ログリスト
        /// </summary>
        static List<Log> Logs { get; } = new List<Log>();

        public static ILogPrinter Printer { get; set; }

        /// <summary>
        /// エラー
        /// </summary>
        /// <param name="obj">メッセージオブジェクト</param>
        public static void Error(object obj)
        {
            AddLog(Status.Error, obj.ToString());
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="obj">メッセージオブジェクト</param>
        public static void Warning(object obj)
        {
            AddLog(Status.Warning, obj.ToString());
        }

        /// <summary>
        /// デバッグ
        /// </summary>
        /// <param name="obj">メッセージオブジェクト</param>
        [Conditional("DEBUG")]
        public static void Debug(object obj)
        {
            AddLog(Status.Debug, obj.ToString());
        }

        /// <summary>
        /// ログを記録
        /// </summary>
        /// <param name="status">ログ状態</param>
        /// <param name="message">メッセージ</param>
        public static void AddLog(Status status, string message)
        {
            string _message = "[" + DateTime.Now.ToLongTimeString() + "] [" + status.ToString() + "]: " + message;
            var log = new Log(status, _message);
            Logs.Add(log);
            Printer?.OnAddedLog(status, _message);
        }

        /// <summary>
        /// ログをほ存する
        /// </summary>
        /// <param name="path">パス</param>
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

        /// <summary>
        /// ログを非同期で保存する
        /// </summary>
        /// <param name="path">パス</param>
        /// <returns>タスク</returns>
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
