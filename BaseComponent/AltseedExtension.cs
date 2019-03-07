using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// Altseed拡張
    /// </summary>
    public static class AltseedExtension
    {
        /// <summary>
        /// 座標が矩形領域に含まれているか
        /// </summary>
        /// <param name="rect">領域</param>
        /// <param name="vector">座標</param>
        /// <returns>真偽</returns>
        public static bool Contains(this asd.RectF rect, asd.Vector2DF vector)
        {
            float cross = 0;
            for (int i = 0; i < 4; i++)
            {
                var temp = asd.Vector2DF.Cross(rect.Vertexes[i] - rect.Vertexes[i == 0 ? 3 : i - 1], vector - rect.Vertexes[i == 0 ? 3 : i - 1]);
                if (temp == 0) return true;
                if (Math.Sign(cross) == Math.Sign(temp) || cross == 0) cross = temp;
                else return false;
            }
            return true;
        }
    }
}
