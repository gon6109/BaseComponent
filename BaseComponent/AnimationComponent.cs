using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    class AnimationComponent : asd.Object2DComponent
    {
        IEnumerator<object> animation;

        /// <summary>
        /// アニメーションしているか
        /// </summary>
        public bool IsAnimating { get; private set; }

        /// <summary>
        /// 更新処理
        /// </summary>
        protected override void OnUpdate()
        {
            IsAnimating = animation?.MoveNext() ?? false;
            base.OnUpdate();
        }

        /// <summary>
        /// アニメーションを追加する
        /// </summary>
        /// <param name="animation">アニメーション</param>
        /// <param name="slot">スロット</param>
        public void AddAnimation(Animation animation, int slot = 0)
        {

        }
    }
}
