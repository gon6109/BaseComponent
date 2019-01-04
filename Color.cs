using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// HSV系
    /// </summary>
    public class HsvColor
    {
        private float _h;
        /// <summary>
        /// 色相 (Hue)
        /// </summary>
        public float H
        {
            get => _h;
            set
            {
                if (_h >= 0f && 360f >= _h) _h = value;
            }
        }

        private float _s;
        /// <summary>
        /// 彩度 (Saturation)
        /// </summary>
        public float S
        {
            get => _s;
            set
            {
                if (_s >= 0f && 1f >= _s) _s = value;
            }
        }

        private float _v;
        /// <summary>
        /// 明度 (Value, Brightness)
        /// </summary>
        public float V
        {
            get => _v;
            set
            {
                if (_v >= 0f && 1f >= _v) _v = value;
            }
        }

        public HsvColor(float hue, float saturation, float brightness)
        {
            _h = hue;
            _s = saturation;
            _v = brightness;
        }

        private HsvColor()
        {
            _h = 0;
            _s = 0;
            _v = 0;
        }

        /// <summary>
        /// 指定したColorからHsvColorを作成する
        /// </summary>
        /// <param name="rgb">Color</param>
        /// <returns>HsvColor</returns>
        public static HsvColor FromRgb(asd.Color rgb)
        {
            float r = rgb.R / 255f;
            float g = rgb.G / 255f;
            float b = rgb.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            float brightness = max;

            float hue, saturation;
            if (max == min)
            {
                //undefined
                hue = 0f;
                saturation = 0f;
            }
            else
            {
                float c = max - min;

                if (max == r)
                {
                    hue = (g - b) / c;
                }
                else if (max == g)
                {
                    hue = (b - r) / c + 2f;
                }
                else
                {
                    hue = (r - g) / c + 4f;
                }
                hue *= 60f;
                if (hue < 0f)
                {
                    hue += 360f;
                }

                saturation = c / max;
            }

            return new HsvColor(hue, saturation, brightness);
        }

        /// <summary>
        /// 指定したHsvColorからColorを作成する
        /// </summary>
        /// <returns>Color</returns>
        public asd.Color ToRgb()
        {
            float h = H;
            float s = S;
            float v = V;
            float r = v;
            float g = v;
            float b = v;
            if (s > 0)
            {
                int Hi = (int)(Math.Floor(h / 60.0f) % 6.0f);
                float f = (h / 60.0f) - Hi;

                float p = v * (1 - s);
                float q = v * (1 - f * s);
                float t = v * (1 - (1 - f) * s);

                switch (Hi)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                    default:
                        break;
                }
            }

            return new asd.Color(
                (byte)Math.Round(r * 255f),
                (byte)Math.Round(g * 255f),
                (byte)Math.Round(b * 255f));
        }
    }
}
