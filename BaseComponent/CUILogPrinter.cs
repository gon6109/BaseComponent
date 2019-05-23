using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// CUIログプリンター
    /// </summary>
    public class CUILogPrinter : ILogPrinter
    {
        public void OnAddedLog(Logger.Status status, string message)
        {
            switch (status)
            {
                case Logger.Status.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Logger.Status.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Logger.Status.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                default:
                    break;
            }
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
