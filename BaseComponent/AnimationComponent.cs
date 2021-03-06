﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    public class AnimationComponent : asd.Object2DComponent
    {
        Dictionary<int, IEnumerator<object>> animations;

        public AnimationComponent()
        {
            animations = new Dictionary<int, IEnumerator<object>>();
        }

        /// <summary>
        /// アニメーションしているか
        /// </summary>
        public bool IsAnimating => animations.Count > 0;

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
            base.OnUpdate();
        }

        /// <summary>
        /// アニメーションを追加する
        /// </summary>
        /// <param name="object2D">アニメーションするオブジェクト</param>
        /// <param name="animation">アニメーション</param>
        /// <param name="slot">スロット</param>
        public void AddAnimation(asd.DrawnObject2D object2D, Animation animation, int slot = 0)
        {
            animations[slot] = animation.GetAnimationCoroutine(object2D);
        }
    }
}
