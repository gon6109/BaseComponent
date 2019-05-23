using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        static ConcurrentQueue<Tuple<asd.Texture2D, string>> requets = new ConcurrentQueue<Tuple<asd.Texture2D, string>>();

        /// <summary>
        /// テクスチャをロードする
        /// </summary>
        /// <param name="path">パス</param>
        /// <returns>テクスチャ</returns>
        public static asd.Texture2D LoadTexture(string path)
        {
            var syncObj = new object();

            asd.Texture2D texture;
            lock (syncObj)
            {
                texture = asd.Engine.Graphics.CreateTexture2D(path);
            }
            if (texture == null)
            {
                lock (syncObj)
                {
                    texture = asd.Engine.Graphics.CreateTexture2D("Static/error.png");
                }
                Logger.Error(new FileNotFoundException(path + "が見つかりません"));
            }
            return texture;
        }

        /// <summary>
        /// テクスチャを非同期ロードする
        /// </summary>
        /// <param name="path">パス</param>
        /// <returns>テクスチャ</returns>
        public static async Task<asd.Texture2D> LoadTextureAsync(string path)
        {
            var syncObj = new object();
            asd.Texture2D texture;

            lock (syncObj)
            {
                texture = asd.Engine.Graphics.CreateTexture2DAsync(path);
            }

            if (texture == null)
            {
                lock (syncObj)
                {
                    texture = asd.Engine.Graphics.CreateTexture2D("Static/error.png");
                }
                Logger.Error(new FileNotFoundException(path + "が見つかりません"));
            }

            await Task.Run(() =>
            {
                while (texture.LoadState == asd.LoadState.Loading) { }
            });

            return texture;
        }
    }
}
