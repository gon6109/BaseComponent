using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// ファイル入出力系
    /// </summary>
    public class IO
    {
        public static MemoryStream GetStream(string path)
        {
            if (path == null) throw new ArgumentNullException("path is null");
            var temp = asd.Engine.File.CreateStaticFile(path);
            if (temp == null) throw new FileNotFoundException(path + "が見つかりません");
            var result = temp.Buffer;
            return new MemoryStream(result);
        }
    }
}
