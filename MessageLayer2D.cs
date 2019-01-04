using System.Collections.Generic;
using System.IO;

namespace BaseComponent
{
    /// <summary>
    /// メッセージ出力系
    /// </summary>
    public class MessageLayer2D : asd.Layer2D
    {
        static public int FontSize { set; get; } = 12;
        static public asd.Color TextColor = new asd.Color(255, 255, 255);

        /// <summary>
        /// インスタンスを返す
        /// </summary>
        public static MessageLayer2D Instance { get; private set; } = new MessageLayer2D();

        List<MessageBox> messageBoxes;
        public asd.Font Font { get; set; }
        public float Margin { get; set; }
        public int WindowSpeed { get; set; } = 20;
        public int TextSpeed { get; set; } = 3;

        public MessageLayer2D()
        {
            messageBoxes = new List<MessageBox>();
            DrawingPriority = 2;
            Margin = 5;
        }

        protected override void OnUpdated()
        {
            for (int i = 0; i < messageBoxes.Count; i++)
            {
                if (messageBoxes[i].MessageBoxState == MessageBoxState.Closed)
                {
                    Instance.RemoveObject(messageBoxes[i]);
                    messageBoxes.Remove(messageBoxes[i]);
                    i--;
                }
            }
            base.OnUpdated();
        }

        public static int Count
        {
            get => Instance.messageBoxes.Count;
        }

        /// <summary>
        /// メッセージボックスを開く
        /// </summary>
        /// <param name="rect">表示範囲</param>
        /// <returns>メっセージボックスへのハンドル</returns>
        public static MessageBox Open(asd.RectF rect)
        {
            MessageBox box = new MessageBox();
            box.DrawingArea = rect;
            Instance.messageBoxes.Add(box);
            Instance.AddObject(box);
            return box;
        }

        public static void Close(MessageBox messageBox)
        {
            if (messageBox.MessageBoxState == MessageBoxState.Closing
                 || messageBox.MessageBoxState == MessageBoxState.Closing) return;
            messageBox.MessageBoxState = MessageBoxState.Closing;
        }

        public static void Reset()
        {
            Instance = new MessageLayer2D();
            Instance.Font = asd.Engine.Graphics.CreateDynamicFont(Base.MainFont, FontSize, TextColor, 0, new asd.Color());
        }
    }

    public class MessageBox : asd.GeometryObject2D
    {
        public asd.RectF DrawingArea
        {
            get
            {
                return new asd.RectF(Position, Size);
            }
            set
            {
                Position = value.Position;
                Size = value.Size;
            }
        }

        public asd.Vector2DF Size { get; set; }

        asd.RectangleShape back;
        Queue<List<TextLine>> messages;
        List<TextLine> texts;
        int counter;
        IEnumerator<int> iterator;

        MessageBoxState messageBoxState;
        public MessageBoxState MessageBoxState
        {
            get => messageBoxState;
            set
            {
                messageBoxState = value;
                switch (messageBoxState)
                {
                    case MessageBoxState.Opening:
                        counter = MessageLayer2D.Instance.WindowSpeed;
                        break;
                    case MessageBoxState.Opened:
                        break;
                    case MessageBoxState.Messaging:
                        break;
                    case MessageBoxState.Messaged:
                        break;
                    case MessageBoxState.Closing:
                        counter = MessageLayer2D.Instance.WindowSpeed;
                        break;
                    case MessageBoxState.Closed:
                        break;
                    default:
                        break;
                }
            }
        }

        public MessageBox()
        {
            MessageBoxState = MessageBoxState.Opening;
            Color = new asd.Color(0, 0, 200);
            texts = new List<TextLine>();
            back = new asd.RectangleShape();
            back.DrawingArea = new asd.RectF();
            Shape = back;
            messages = new Queue<List<TextLine>>();
            iterator = ShowText();
        }

        protected override void OnUpdate()
        {
            switch (MessageBoxState)
            {
                case MessageBoxState.Opening:
                    Open();
                    break;
                case MessageBoxState.Opened:
                    if (messages.Count != 0)
                    {
                        foreach (var item in texts)
                        {
                            Layer.RemoveObject(item);
                        }
                        texts = messages.Dequeue();
                        foreach (var item in texts)
                        {
                            Layer.AddObject(item);
                        }
                        iterator = ShowText();
                        MessageBoxState = MessageBoxState.Messaging;
                    }
                    break;
                case MessageBoxState.Messaging:
                    iterator.MoveNext();
                    break;
                case MessageBoxState.Messaged:
                    if (Input.GetInputState(Inputs.A) == 1)
                    {
                        foreach (var item in texts)
                        {
                            Layer.RemoveObject(item);
                        }
                        if (messages.Count == 0)
                        {
                            MessageBoxState = MessageBoxState.Closing;
                            break;
                        }
                        texts = messages.Dequeue();
                        foreach (var item in texts)
                        {
                            Layer.AddObject(item);
                        }
                        iterator = ShowText();
                        MessageBoxState = MessageBoxState.Messaging;
                    }
                    break;
                case MessageBoxState.Closing:
                    Close();
                    break;
                case MessageBoxState.Closed:
                    break;
                default:
                    break;
            }

            base.OnUpdate();
        }

        void Open()
        {
            back.DrawingArea = new asd.RectF(new asd.Vector2DF(), Size * (MessageLayer2D.Instance.WindowSpeed - counter) / MessageLayer2D.Instance.WindowSpeed);
            if (counter == 0)
            {
                MessageBoxState = MessageBoxState.Opened;
                return;
            }
            counter--;
        }

        void Close()
        {
            back.DrawingArea = new asd.RectF(new asd.Vector2DF(), Size * counter / MessageLayer2D.Instance.WindowSpeed);
            if (counter == 0)
            {
                MessageBoxState = MessageBoxState.Closed;
                return;
            }
            counter--;
        }

        IEnumerator<int> ShowText()
        {
            int count = 0;
            int total = 0;
            while (true)
            {
                if (count % MessageLayer2D.Instance.TextSpeed == 0)
                {
                    bool isEnd = true;
                    MessageBoxState = MessageBoxState.Messaging;
                    foreach (var item in texts)
                    {
                        if (item.Text == item.Line) continue;
                        item.Text = item.Line.Substring(0, count / MessageLayer2D.Instance.TextSpeed - total);
                        if (item.Text == item.Line) total += item.Line.Length;
                        isEnd = false;
                        break;
                    }
                    if (isEnd) break;
                }
                count++;
                yield return 0;
            }
            MessageBoxState = MessageBoxState.Messaged;
            yield return -1;
        }

        /// <summary>
        /// テキストを表示させる
        /// </summary>
        /// <param name="text">表示するテキスト</param>
        public void SetMessage(string text)
        {
            string temp = "";
            int l = 0;
            List<TextLine> textLines = new List<TextLine>();
            TextLine textObject;
            foreach (var item in text)
            {
                if (item == '\n')
                {
                    textObject = new TextLine();
                    textObject.Font = MessageLayer2D.Instance.Font;
                    textObject.Position = DrawingArea.Position +
                        new asd.Vector2DF(MessageLayer2D.Instance.Margin,
                        MessageLayer2D.Instance.Margin + MessageLayer2D.Instance.Font.CalcTextureSize(" ", asd.WritingDirection.Horizontal).Y * 1.2f * (l++));
                    textObject.Line = temp;
                    temp = "";
                    textLines.Add(textObject);
                }
                else if (MessageLayer2D.Instance.Font.CalcTextureSize(temp + item, asd.WritingDirection.Horizontal).X < DrawingArea.Width - MessageLayer2D.Instance.Margin * 2)
                    temp += item;
                else
                {
                    textObject = new TextLine();
                    textObject.Font = MessageLayer2D.Instance.Font;
                    textObject.Position = DrawingArea.Position +
                        new asd.Vector2DF(MessageLayer2D.Instance.Margin,
                        MessageLayer2D.Instance.Margin + MessageLayer2D.Instance.Font.CalcTextureSize(" ", asd.WritingDirection.Horizontal).Y * 1.2f * (l++));
                    textObject.Line = temp;
                    temp = item.ToString();
                    textLines.Add(textObject);
                }
            }
            textObject = new TextLine();
            textObject.Font = MessageLayer2D.Instance.Font;
            textObject.Position = DrawingArea.Position +
                new asd.Vector2DF(MessageLayer2D.Instance.Margin,
                MessageLayer2D.Instance.Margin + MessageLayer2D.Instance.Font.CalcTextureSize(" ", asd.WritingDirection.Horizontal).Y * 1.2f * (l++));
            textObject.Line = temp;
            textLines.Add(textObject);
            messages.Enqueue(textLines);
        }

        /// <summary>
        /// テキストを表示させる
        /// </summary>
        /// <param name="stream">表示するテキストのストリーム</param>
        public void SetMessage(Stream stream)
        {
            SetMessage(stream.ToString());
        }

        class TextLine : asd.TextObject2D
        {
            public string Line { get; set; }
        }
    }

    public enum MessageBoxState
    {
        Opening,
        Opened,
        Messaging,
        Messaged,
        Closing,
        Closed,
    }
}
