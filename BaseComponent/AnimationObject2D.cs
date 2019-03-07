using System;
using System.Collections.Generic;
namespace BaseComponent
{
    ///<summary>
    ///アニメーションを描画するクラス
    ///</summary>
    public class AnimationObject2D : asd.TextureObject2D, ICloneable
    {
        protected List<asd.Texture2D> _textures;
        IEnumerator<int> _iterator;

        int _interval;
        /// <summary>
        /// 切り替え間隔
        /// </summary>
        public int Interval 
        {
            set
            {
                if (value < 0) _interval = 1;
                else _interval = value;
            }
            get
            {
                return _interval;
            }
        }
        
        /// <summary>
        /// 一周で元の状態に戻るか否か
        /// </summary>
        public bool IsOneLoop { set; get; }

        public AnimationObject2D()
        {
            _textures = new List<asd.Texture2D>();
            _iterator = Animate();
            Interval = 1;
        }

        ///<summary>
        ///連番画像を読み込む
        ///</summary>
        ///<param name="animationGroup">ファイル名</param>
        ///<param name="extension">拡張子</param>
        ///<param name="sheets">枚数</param>
        public void LoadAnimationFile(string animationGroup, string extension, int sheets)
        {
            for (int i = 0; i < sheets; i++)
            {
                var texture = TextureManager.LoadTexture(animationGroup + i.ToString() + "." + extension);
                if (texture == null) return;
                _textures.Add(texture);
            }
            Texture = _textures[0];
        }

        IEnumerator<int> Animate()
        {
            for (int i = 0; i < _textures.Count; i++)
            {
                Texture = _textures[i];
                yield return i;
                for (int l = 0; l < Interval - 1; l++)
                {
                    yield return i;
                }
            }
            yield return -1;
        }

        protected override void OnUpdate()
        {
            if (!_iterator.MoveNext())
            {
                if (IsOneLoop)
                {
                    Dispose();
                    return;
                }
                _iterator = Animate();
            }
            base.OnUpdate();
        }

        /// <summary>
        /// インスタンスの複製
        /// </summary>
        /// <returns>複製されたインスタンス</returns>
        public object Clone()
        {
            AnimationObject2D clone = new AnimationObject2D();
            clone._textures = new List<asd.Texture2D>(_textures);
            clone.IsOneLoop = IsOneLoop;
            clone.Interval = Interval;
            return clone;
        }
    }
}
