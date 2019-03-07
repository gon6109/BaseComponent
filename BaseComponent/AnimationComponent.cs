using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    class AnimationComponent : asd.Object2DComponent
    {
        Dictionary<int, IEnumerator<object>> animations;

        /// <summary>
        /// アニメーションしているか
        /// </summary>
        public bool IsAnimating { get; private set; }

        /// <summary>
        /// 更新処理
        /// </summary>
        protected override void OnUpdate()
        {
            List<int> removeAnimationKeys = new List<int>();
            foreach (var item in animations)
            {
                if (!item.Value.MoveNext()) removeAnimationKeys.Add(item.Key);
            }

            foreach (var item in removeAnimationKeys)
            {
                animations.Remove(item);
            }

            IsAnimating = animations.Count() > 0;
            base.OnUpdate();
        }

        /// <summary>
        /// アニメーションを追加する
        /// </summary>
        /// <param name="animation">アニメーション</param>
        /// <param name="slot">スロット</param>
        public void AddAnimation(Animation animation, int slot = 0)
        {
            if (Owner is asd.DrawnObject2D object2D)
                animations.Add(slot, animation.GetAnimationCoRutine(object2D));
        }
    }
}
