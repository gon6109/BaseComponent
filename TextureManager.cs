using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// 画像管理
    /// </summary>
    public static class TextureManager
    {
        public static asd.Texture2D LoadTexture(string path)
        {
            var texture = asd.Engine.Graphics.CreateTexture2D(path);
            if (texture == null) 
            {
                texture = asd.Engine.Graphics.CreateTexture2D("Static/error.png");
                ErrorIO.AddError(new FileNotFoundException(path + "が見つかりません"));
            }
            return texture;
        }
    }
}
