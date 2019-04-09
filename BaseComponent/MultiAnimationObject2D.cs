using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading.Tasks;

namespace BaseComponent
{
    /// <summary>
    /// 複数パターンのアニメーションを描画する
    /// </summary>
    public class MultiAnimationObject2D : asd.TextureObject2D, ICloneable
    {
        Dictionary<string, AnimationPart> animationPart;
        /// <summary>
        /// アニメーションパターン
        /// </summary>
        public Dictionary<string, AnimationPart> AnimationPart { get => animationPart; private set => animationPart = value; }

        string state;
        /// <summary>
        /// 現在のアニメーションパート
        /// </summary>
        public string State
        {
            set
            {
                PreState = state;
                state = value;
                if (AnimationPart.ContainsKey(value)) Texture = AnimationPart[value].CurrentTexture;
            }
            get
            {
                return state;
            }
        }

        public int Interval
        {
            get => AnimationPart[State].Interval;
            set
            {
                AnimationPart[State].Interval = value;
            }
        }

        bool isOneLoop;
        /// <summary>
        /// 一周で元の状態に戻るか否か
        /// </summary>
        public bool IsOneLoop
        {
            get
            {
                return isOneLoop;
            }

            set
            {
                isOneLoop = value;
                if (isOneLoop) AnimationPart[State].Reset();
            }
        }

        string preState;
        public string PreState { get => preState; private set => preState = value; }

        /// <summary>
        /// アニメーションするか否か
        /// </summary>
        public bool IsAnimate { get; set; }

        public MultiAnimationObject2D()
        {
            AnimationPart = new Dictionary<string, AnimationPart>();
            IsAnimate = true;
            TextureFilterType = asd.TextureFilterType.Linear;
            state = "";
            PreState = "";
            IsOneLoop = false;
        }

        /// <summary>
        /// アニメーションスクリプトをロード
        /// </summary>
        /// <param name="path">スクリプトへのパス</param>
        public ScriptRunner<object> LoadAnimationScript(string path)
        {
            try
            {
                var script = CSharpScript.Create(IO.GetStream(path), globalsType: this.GetType());
                script.Compile();
                var thread = script.RunAsync(this);
                thread.Wait();
                return script.CreateDelegate();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// アニメーションスクリプトをロード
        /// </summary>
        /// <param name="path">スクリプトへのパス</param>
        public async Task<ScriptRunner<object>> LoadAnimationScriptAsync(string path)
        {
            try
            {
                var script = CSharpScript.Create(IO.GetStream(path), globalsType: this.GetType());
                script.Compile();
                await script.RunAsync(this);
                return script.CreateDelegate();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// アニメーションパーツを追加
        /// </summary>
        /// <param name="animationGroup">ファイル名</param>
        /// <param name="extension">拡張子</param>
        /// <param name="sheets">枚数</param>
        /// <param name="partName">パーツ名</param>
        /// <param name="interval">切り替え間隔</param>
        public void AddAnimationPart(string animationGroup, string extension, int sheets, string partName, int interval)
        {
            AnimationPart part = new AnimationPart();
            part.LoadAnimationFile(animationGroup, extension, sheets);
            part.Interval = interval;
            AnimationPart[partName] = part;

            if (AnimationPart.Count == 0) State = partName;
        }

        /// <summary>
        /// アニメーションパーツを追加
        /// </summary>
        /// <param name="animationGroup">ファイル名</param>
        /// <param name="extension">拡張子</param>
        /// <param name="sheets">枚数</param>
        /// <param name="partName">パーツ名</param>
        /// <param name="interval">切り替え間隔</param>
        public void AddAnimationPartAsync(string animationGroup, string extension, int sheets, string partName, int interval)
        {
            AnimationPart part = new AnimationPart();
            part.LoadAnimationFileAsync(animationGroup, extension, sheets);
            part.Interval = interval;
            AnimationPart[partName] = part;

            if (AnimationPart.Count == 0) State = partName;
        }

        /// <summary>
        /// コピーする
        /// </summary>
        /// <param name="multiAnimationObject">コピー元</param>
        public void Copy(MultiAnimationObject2D multiAnimationObject)
        {
            foreach (var item in multiAnimationObject.AnimationPart)
            {
                AnimationPart.Add(item.Key, (AnimationPart)item.Value.Clone());
            }
        }

        protected override void OnUpdate()
        {
            if (!AnimationPart.ContainsKey(State) || !IsAnimate) return;
            if (AnimationPart[State].Update() && IsOneLoop)
            {
                State = PreState;
                IsOneLoop = false;
            }
            if (AnimationPart[State].IsUpdated)
            {
                Texture = AnimationPart[State].CurrentTexture;
                AnimationPart[State].IsUpdated = false;
            }
            base.OnUpdate();
        }

        /// <summary>
        /// インスタンスの複製
        /// </summary>
        /// <returns>複製されたインスタンス</returns>
        public object Clone()
        {
            MultiAnimationObject2D clone = new MultiAnimationObject2D();
            foreach (var item in AnimationPart)
            {
                clone.AnimationPart.Add(item.Key, (AnimationPart)item.Value.Clone());
            }
            return clone;
        }
    }

    public class AnimationPart : ICloneable
    {
        IEnumerator<int> iterator;

        public bool IsUpdated { set; get; }

        public asd.Texture2D CurrentTexture { set; get; }
        public List<asd.Texture2D> Textures { set; get; }

        int interval;
        public int Interval
        {
            set
            {
                if (value < 0) interval = 1;
                else interval = value;
            }
            get
            {
                return interval;
            }
        }

        public AnimationPart()
        {
            iterator = Animate();
            Textures = new List<asd.Texture2D>();
        }

        public void LoadAnimationFile(string animationGroup, string extension, int sheets)
        {
            for (int i = 0; i < sheets; i++)
            {
                var texture = TextureManager.LoadTexture(animationGroup + i.ToString() + "." + extension);
                if (texture == null) return;
                Textures.Add(texture);
                if (i == 0) CurrentTexture = texture;
            }
        }

        public void LoadAnimationFileAsync(string animationGroup, string extension, int sheets)
        {
            Parallel.For(0, sheets, async (i) =>
            {
                var texture = await TextureManager.LoadTextureAsync(animationGroup + i.ToString() + "." + extension);
                if (texture == null) return;
                lock (this)
                {
                    Textures.Add(texture);
                    if (i == 0) CurrentTexture = texture;
                }
            });
        }

        public void Reset()
        {
            iterator = Animate();
        }

        public IEnumerator<int> Animate()
        {
            for (int i = 0; i < Textures.Count; i++)
            {
                CurrentTexture = Textures[i];
                IsUpdated = true;
                yield return i;
                for (int l = 0; l < Interval - 1; l++)
                {
                    yield return i;
                }
            }
            yield return -1;
        }

        public bool Update()
        {
            if (!iterator.MoveNext())
            {
                iterator = Animate();
                return true;
            }
            return false;
        }

        public object Clone()
        {
            AnimationPart clone = new AnimationPart();
            clone.Textures = new List<asd.Texture2D>(Textures);
            clone.Interval = Interval;
            return clone;
        }
    }
}
