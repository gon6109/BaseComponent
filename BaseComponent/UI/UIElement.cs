﻿using System;
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
        private bool _isEnable;

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
        public virtual bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                OnChangedEnable(_isEnable);
            }
        }

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
        /// IsEnable変更時のイベント
        /// </summary>
        public event Action<bool> OnChangedEnable = delegate { };

        /// <summary>
        /// 選択されたか
        /// </summary>
        public bool IsSelected
        {
            get
            {
                if (Owner?.Layer is UILayer2D layer)
                    return layer.IsMoveFocus && IsEnable && IsFocused && GetIsSelectedFunc();
                return false;
            }
        }

        /// <summary>
        /// UIの基準座標
        /// </summary>
        public asd.Vector2DF Position
        {
            get
            {
                if (Owner is asd.TextObject2D text)
                    return Owner.Position + (text.Font?.CalcTextureSize(text.Text, asd.WritingDirection.Horizontal).To2DF() ?? new asd.Vector2DF()) / 2;
                else if (Owner is asd.GeometryObject2D geo)
                    if (geo.Shape is asd.RectangleShape rect)
                        return Owner.Position + rect.DrawingArea.Position + rect.DrawingArea.Size / 2;
                    else if (geo.Shape is asd.CircleShape circle)
                        return Owner.Position + circle.Position;
                    else
                        return Owner.Position;
                else
                    return Owner.Position;
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

        /// <summary>
        /// 選択条件
        /// </summary>
        public static Func<bool> GetIsSelectedFunc　= delegate { return Input.GetInputState(Inputs.A) == 1; };
    }
}
