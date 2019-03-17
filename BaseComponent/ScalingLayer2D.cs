using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// スクリーンサイズに自動調整するレイヤー
    /// </summary>
    public class ScalingLayer2D : asd.Layer2D
    {
        private bool _isFixAspectRatio;

        /// <summary>
        /// 表示する範囲
        /// </summary>
        public static asd.Vector2DF OriginDisplaySize { get; set; } = new asd.Vector2DF(1920, 1080);

        /// <summary>
        /// カメラ
        /// </summary>
        public asd.CameraObject2D Camera { get; protected set; }

        /// <summary>
        /// 縦横比を固定するか
        /// </summary>
        public bool IsFixAspectRatio
        {
            get => _isFixAspectRatio;
            set
            {
                _isFixAspectRatio = value;
                UpdateScaling();
            }
        }

        /// <summary>
        /// Updating時自動的にスケーリングするか
        /// </summary>
        public bool IsUpdateScalingAuto { get; set; }

        public ScalingLayer2D()
        {
            Camera = new asd.CameraObject2D();
            AddObject(Camera);
        }

        protected override void OnAdded()
        {
            UpdateScaling();
            base.OnAdded();
        }

        protected override void OnUpdating()
        {
            if (IsUpdateScalingAuto)
                UpdateScaling();
            base.OnUpdating();
        }

        /// <summary>
        /// スケーリングを更新
        /// </summary>
        public void UpdateScaling()
        {
            if (IsFixAspectRatio)
            {
                if ((float)asd.Engine.WindowSize.X / asd.Engine.WindowSize.Y > OriginDisplaySize.X / OriginDisplaySize.Y)
                {
                    Camera.Dst = new asd.RectI(
                        new asd.Vector2DI((int)((asd.Engine.WindowSize.X - OriginDisplaySize.X * asd.Engine.WindowSize.Y / OriginDisplaySize.Y) / 2), 0),
                        new asd.Vector2DI((int)(OriginDisplaySize.X * asd.Engine.WindowSize.Y / OriginDisplaySize.Y), asd.Engine.WindowSize.Y));
                }
                else
                {
                    Camera.Dst = new asd.RectI(
                       new asd.Vector2DI(0, (int)((asd.Engine.WindowSize.Y - OriginDisplaySize.Y * asd.Engine.WindowSize.X / OriginDisplaySize.X) / 2)),
                       new asd.Vector2DI(asd.Engine.WindowSize.X, (int)(OriginDisplaySize.Y * asd.Engine.WindowSize.X / OriginDisplaySize.X)));
                }
            }
            else
            {
                Camera.Dst = new asd.RectI(new asd.Vector2DI(), asd.Engine.WindowSize);
            }
            Camera.Src = new asd.RectI(Camera.Src.Position, OriginDisplaySize.To2DI());
        }
    }
}
