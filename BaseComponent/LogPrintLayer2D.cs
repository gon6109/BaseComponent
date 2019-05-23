using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// ログ表示レイヤー
    /// </summary>
    public class LogPrintLayer2D : asd.Layer2D, ILogPrinter
    {
        private int displayFrame;

        class LogTextObject : asd.TextObject2D
        {
            public AnimationComponent Animation => GetComponent("animation") as AnimationComponent;

            public LogTextObject()
            {
                AddComponent(new AnimationComponent(), "animation");
            }
        }

        List<IEnumerator<object>> Coroutines { get; }

        asd.Font Font { get; }

        /// <summary>
        /// 表示時間
        /// </summary>
        public int DisplayFrame
        {
            get => displayFrame;
            set
            {
                if (value > 0)
                    displayFrame = value;
            }
        }

        bool IsRequireMove { get; set; }

        public LogPrintLayer2D()
        {
            Coroutines = new List<IEnumerator<object>>();
            DisplayFrame = 300;
            Font = asd.Engine.Graphics.CreateDynamicFont("", 15, new asd.Color(255, 255, 255), 0, new asd.Color());
        }

        public void OnAddedLog(Logger.Status status, string message)
        {
            Coroutines.Add(Print(status, message));
        }

        protected override void OnUpdated()
        {
            base.OnUpdated();

            var remove = new List<IEnumerator<object>>();
            foreach (var item in Coroutines)
            {
                if (!item.MoveNext())
                    remove.Add(item);
            }

            foreach (var item in remove)
            {
                Coroutines.Remove(item);
            }

            if (IsRequireMove)
            {
                foreach (var (item, i) in Objects.OfType<LogTextObject>().Select((obj, i) => (obj, i)))
                {
                    Animate(item, i);
                }
            }

            IsRequireMove = remove.Count > 0;

            void Animate(LogTextObject textObject, int index)
            {
                var anm = new Animation();
                anm.MoveTo(new asd.Vector2DF(15, index * 17), 15, Animation.Easing.InOutSine);
                textObject.Animation.AddAnimation(textObject, anm, 1);
            }
        }

        IEnumerator<object> Print(Logger.Status status, string message)
        {
            var text = new LogTextObject();

            switch (status)
            {
                case Logger.Status.Error:
                    text.Color = new asd.Color(255, 0, 0, 0);
                    break;
                case Logger.Status.Warning:
                    text.Color = new asd.Color(255, 255, 0, 0);
                    break;
                case Logger.Status.Debug:
                    text.Color = new asd.Color(255, 255, 255, 0);
                    break;
                default:
                    break;
            }

            text.Text = message;
            text.Font = Font;
            text.Position = new asd.Vector2DF(15, (Objects.Count(obj => obj is LogTextObject) + 1) * 17);

            AddObject(text);

            yield return null;

            var anm = new Animation();
            anm.AlphaTo(255, 30);
            anm.Sleep(displayFrame);
            anm.AlphaTo(0, 30);

            text.Animation.AddAnimation(text, anm);

            while (text.Animation.IsAnimating)
            {
                yield return null;
            }

            text.Dispose();
        }
    }
}
