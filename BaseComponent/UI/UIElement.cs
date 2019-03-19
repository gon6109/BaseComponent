using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent.UI
{
    /// <summary>
    /// UIを構成する要素の基底クラス
    /// </summary>
    public class UIElement : asd.Object2DComponent
    {
        private bool _isFocused;

        /// <summary>
        /// フォーカスされているか
        /// </summary>
        public virtual bool IsFocused
        {
            get => _isFocused;
            set
            {
                _isFocused = value;
                OnChangedFocus(_isFocused);
            }
        }

        /// <summary>
        /// 有効か否か
        /// </summary>
        public virtual bool IsEnable { get; set; }

        /// <summary>
        /// 上への参照
        /// </summary>
        public UIElement Up { get; set; }

        /// <summary>
        /// 下への参照
        /// </summary>
        public UIElement Down { get; set; }

        /// <summary>
        /// 右への参照
        /// </summary>
        public UIElement Right { get; set; }

        /// <summary>
        /// 左への参照
        /// </summary>
        public UIElement Left { get; set; }

        /// <summary>
        /// 要素のサイズ
        /// </summary>
        public asd.Vector2DF Size { get; set; }

        /// <summary>
        /// フォーカス変更時のイベント
        /// </summary>
        public event Action<bool> OnChangedFocus = delegate { };

        /// <summary>
        /// 選択されたか
        /// </summary>
        public bool IsSelected
        {
            get
            {
                if (Owner?.Layer is UILayer2D layer)
                    return layer.IsMoveFocus && IsEnable && IsFocused && Input.GetInputState(Inputs.A) == 1;
                return false;
            }
        }

        /// <summary>
        /// 接続を消去
        /// </summary>
        public void ResetConnection()
        {
            Up = null;
            Down = null;
            Left = null;
            Right = null;
            IsFocused = false;
        }

        public UIElement()
        {
            IsEnable = true;
        }
    }
}
