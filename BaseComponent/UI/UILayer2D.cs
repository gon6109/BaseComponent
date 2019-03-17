using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent.UI
{

    /// <summary>
    /// UI用レイヤー
    /// </summary>
    public abstract class UILayer2D : asd.Layer2D
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
                value.IsFocused = true;
                if (_focusedUIElement != null) _focusedUIElement.IsFocused = false;
                _focusedUIElement = value;
                OnChangedFocusedUIElement();
            }
        }

        public bool IsMoveFocus { get; set; }

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

            elements.Sort((a, b) => Math.Sign(a.Size.X * a.Size.Y - b.Size.X * b.Size.Y));
            foreach (var item in elements)
            {
                foreach (var item2 in elements.Where(obj => obj != item)) 
                {
                    var angle = (item2.Owner.Position - item.Owner.Position).Degree;
                    if (angle >= (-item.Size).Degree && angle < new asd.Vector2DF(item.Size.X / 2, -item.Size.Y / 2).Degree)
                    {
                        if (item.Up == null) item.Up = item2;
                        else
                        {
                            if ((item.Up.Owner.Position - item.Owner.Position).Length >= (item2.Owner.Position - item.Owner.Position).Length) item.Up = item2;
                        }
                    }
                    if (angle >= new asd.Vector2DF(item.Size.X / 2, -item.Size.Y / 2).Degree && angle < item.Size.Degree)
                    {
                        if (item.Right == null) item.Right = item2;
                        else
                        {
                            if ((item.Right.Owner.Position - item.Owner.Position).Length >= (item2.Owner.Position - item.Owner.Position).Length) item.Right = item2;
                        }
                    }
                    if (angle >= item.Size.Degree && angle < new asd.Vector2DF(-item.Size.X / 2, item.Size.Y / 2).Degree)
                    {
                        if (item.Down == null) item.Down = item2;
                        else
                        {
                            if ((item.Down.Owner.Position - item.Owner.Position).Length >= (item2.Owner.Position - item.Owner.Position).Length) item.Down = item2;
                        }
                    }
                    if ((angle >= new asd.Vector2DF(-item.Size.X / 2, item.Size.Y / 2).Degree && angle <= 180)
                        || (angle >= -180 && angle < (-item.Size).Degree))
                    {
                        if (item.Left == null) item.Left = item2;
                        else
                        {
                            if ((item.Left.Owner.Position - item.Owner.Position).Length >= (item2.Owner.Position - item.Owner.Position).Length) item.Left = item2;
                        }
                    }
                }
            }

            _focusedUIElement = elements.LastOrDefault();
        }

        protected override void OnUpdating()
        {
            if (!IsMoveFocus) return;

            if (Input.GetInputState(Inputs.Up) == 1) FocusedUIElement = FocusedUIElement.Up ?? FocusedUIElement;
            if (Input.GetInputState(Inputs.Right) == 1) FocusedUIElement = FocusedUIElement.Right ?? FocusedUIElement;
            if (Input.GetInputState(Inputs.Left) == 1) FocusedUIElement = FocusedUIElement.Left ?? FocusedUIElement;
            if (Input.GetInputState(Inputs.Down) == 1) FocusedUIElement = FocusedUIElement.Down ?? FocusedUIElement;
            base.OnUpdating();
        }
    }
}
