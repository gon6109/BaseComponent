using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent.UI
{
    /// <summary>
    /// ボタン
    /// </summary>
    public class UIButton : asd.TextureObject2D
    {
        /// <summary>
        /// UIの情報
        /// </summary>
        UIElement UI => GetComponent("ui") as UIElement;

        /// <summary>
        /// テクスチャ
        /// </summary>
        public new asd.Texture2D Texture
        {
            get => base.Texture;
            set
            {
                base.Texture = value;
                CenterPosition = base.Texture.Size.To2DF() / 2;
                UI.Size = base.Texture.Size.To2DF();
            }
        }

        /// <summary>
        /// 選択された時に発火するイベント
        /// </summary>
        public event Action OnSelected = delegate { };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UIButton()
        {
            AddComponent(new UIElement(), "ui");
        }

        protected override void OnUpdate()
        {
            if (UI.IsSelected)
                OnSelected();
            base.OnUpdate();
        }
    }
}
