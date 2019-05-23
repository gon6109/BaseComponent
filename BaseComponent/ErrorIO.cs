using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// エラー出力系
    /// </summary>
    [Obsolete]
    public static class ErrorIO
    {
        private static asd.Scene s_scene;

        static List<string> Errors { get; set; } = new List<string>();

        public static asd.Scene Scene
        {
            get => s_scene;
            set
            {
                if (!ErrorText.layer.IsAlive) ErrorText.layer = new asd.Layer2D();
                if (ErrorText.layer.Scene == s_scene) s_scene?.RemoveLayer(ErrorText.layer);
                s_scene = value;
                s_scene.AddLayer(ErrorText.layer);
            }
        }

        /// <summary>
        /// 例外をログ
        /// </summary>
        /// <param name="exception">例外</param>
        public static void AddError(Exception exception)
        {
            Errors.Add(exception.ToString());
            if (Scene != null && Scene.IsAlive) ErrorText.Add(new ErrorText(exception.Message));
            else
            {
                Scene = asd.Engine.CurrentScene;
                ErrorText.Add(new ErrorText(exception.Message));
            }
        }

        /// <summary>
        /// ログを出力
        /// </summary>
        /// <param name="path"></param>
        public static void SaveError(string path)
        {
            StreamWriter streamWriter = new StreamWriter(path, false);
            foreach (var item in Errors)
            {
                streamWriter.WriteLine(item);
            }
            streamWriter.Close();
            Errors.Clear();
        }
    }

    /// <summary>
    /// エラー表示用オブジェクト
    /// </summary>
    [Obsolete]
    class ErrorText : asd.TextObject2D
    {
        internal static asd.Layer2D layer = new asd.Layer2D();
        int count = 0;
        IEnumerator<int> enumerator;
        int index;

        public ErrorText(string message)
        {
            Text = message;
            Font = asd.Engine.Graphics.CreateDynamicFont("", 12, new asd.Color(255, 100, 100), 0, new asd.Color());
            DrawingPriority = 10;
        }

        protected override void OnAdded()
        {
            Open();
            count = -10 * layer.Objects.Count(obj => obj is ErrorText);
            index = Environment.TickCount;
            Layer.DrawingPriority = 15;
            base.OnAdded();
        }

        public void Open()
        {
            enumerator = AnimateTransition(true);
        }

        IEnumerator<int> AnimateTransition(bool isFadeIn)
        {
            if (isFadeIn) Color = new asd.Color(255, 255, 255, 0);
            yield return 0;
            while (isFadeIn ? Color.A < 245 : Color.A > 10)
            {
                var temp = Color;
                if (isFadeIn) temp.A += 5;
                else temp.A -= 5;
                Color = temp;
                yield return 0;
            }
            if (isFadeIn) Color = new asd.Color(255, 255, 255);
            else Dispose();
            yield return 0;
        }

        public void Close()
        {
            enumerator = AnimateTransition(false);
        }

        protected override void OnUpdate()
        {
            Position = new asd.Vector2DF(10, 10 + layer.Objects.OfType<ErrorText>().OrderBy(obj => obj.index).ToList().IndexOf(this) * 15);
            count++;
            if (count == 600) Close();
            enumerator?.MoveNext();
            base.OnUpdate();
        }

        public static void Add(ErrorText errorText)
        {
            layer.AddObject(errorText);
        }
    }
}
