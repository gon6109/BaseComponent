﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent.UI
{

    /// <summary>
    /// UI用レイヤー
    /// </summary>
    public class UILayer2D : ScalingLayer2D
    {
        private UIElement _focusedUIElement;

        /// <summary>
        /// フォーカスされたオブジェクトが変更された時に発火するイベント
        /// </summary>
        public event Action OnChangedFocusedUIElement = delegate { };

        /// <summary>
        /// フォーカスされたオブジェクト
        /// </summary>
        public UIElement FocusedUIElement
        {
            get => _focusedUIElement;
            set
            {
                if (_focusedUIElement == value) return;
                if (value != null)
                    value.IsFocused = true;
                if (_focusedUIElement != null) _focusedUIElement.IsFocused = false;
                _focusedUIElement = value;
                OnChangedFocusedUIElement();
            }
        }

        public bool IsMoveFocus { get; set; }

        public UILayer2D()
        {
            IsMoveFocus = true;
        }

        protected override void OnAdded()
        {
            base.OnAdded();
            ConnectUIElements();
        }

        /// <summary>
        /// 要素同士の関係を構築
        /// </summary>
        public void ConnectUIElements()
        {
            var elements = Objects.Select(obj => obj.GetComponent("ui")).OfType<UIElement>().ToList();

            foreach (var item in elements)
            {
                item.ResetConnection();
            }
            FocusedUIElement = null;

            elements.Sort((a, b) => Math.Sign(a.Size.X * a.Size.Y - b.Size.X * b.Size.Y));
            foreach (var item in elements.Where(obj => obj.IsEnable))
            {
                foreach (var item2 in elements.Where(obj => obj.IsEnable && obj != item))
                {
                    var angle = (item2.Position - item.Position).Degree;
                    if (angle >= (-item.Size).Degree && angle < new asd.Vector2DF(item.Size.X / 2, -item.Size.Y / 2).Degree)
                    {
                        if (item.Up == null) item.Up = item2;
                        else
                        {
                            if ((item.Up.Position - item.Position).Length >= (item2.Position - item.Position).Length) item.Up = item2;
                        }
                    }
                    else if (angle >= new asd.Vector2DF(item.Size.X / 2, -item.Size.Y / 2).Degree && angle < item.Size.Degree)
                    {
                        if (item.Right == null) item.Right = item2;
                        else
                        {
                            if ((item.Right.Position - item.Position).Length >= (item2.Position - item.Position).Length) item.Right = item2;
                        }
                    }
                    else if (angle >= item.Size.Degree && angle < new asd.Vector2DF(-item.Size.X / 2, item.Size.Y / 2).Degree)
                    {
                        if (item.Down == null) item.Down = item2;
                        else
                        {
                            if ((item.Down.Position - item.Position).Length >= (item2.Position - item.Position).Length) item.Down = item2;
                        }
                    }
                    else if ((angle >= new asd.Vector2DF(-item.Size.X / 2, item.Size.Y / 2).Degree && angle <= 180)
                        || (angle >= -180 && angle < (-item.Size).Degree))
                    {
                        if (item.Left == null) item.Left = item2;
                        else
                        {
                            if ((item.Left.Position - item.Position).Length >= (item2.Position - item.Position).Length) item.Left = item2;
                        }
                    }
                }
            }

            FocusedUIElement = elements.Where(obj => obj.IsEnable).LastOrDefault();
        }

        protected override void OnUpdating()
        {
            if (IsMoveFocus)
            {
                if (GetIsPushedUpFunc()) FocusedUIElement = FocusedUIElement.Up ?? FocusedUIElement;
                if (GetIsPushedRightFunc()) FocusedUIElement = FocusedUIElement.Right ?? FocusedUIElement;
                if (GetIsPushedLeftFunc()) FocusedUIElement = FocusedUIElement.Left ?? FocusedUIElement;
                if (GetIsPushedDownFunc()) FocusedUIElement = FocusedUIElement.Down ?? FocusedUIElement;
            }
            base.OnUpdating();
        }

        /// <summary>
        /// 上押下判定
        /// </summary>
        public static Func<bool> GetIsPushedUpFunc = delegate { return Input.GetInputState(Inputs.Up) == 1; };

        /// <summary>
        /// 下押下判定
        /// </summary>
        public static Func<bool> GetIsPushedDownFunc = delegate { return Input.GetInputState(Inputs.Down) == 1; };

        /// <summary>
        /// 左押下判定
        /// </summary>
        public static Func<bool> GetIsPushedLeftFunc = delegate { return Input.GetInputState(Inputs.Left) == 1; };

        /// <summary>
        /// 右押下判定
        /// </summary>
        public static Func<bool> GetIsPushedRightFunc = delegate { return Input.GetInputState(Inputs.Right) == 1; };
    }
}
