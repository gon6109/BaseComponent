using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// ログ表示用インターフェース
    /// </summary>
    public interface ILogPrinter
    {
        void OnAddedLog(Logger.Status status, string message);
    }
}
