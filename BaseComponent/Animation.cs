using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    public class Animation
    {
        abstract class BaseAnimationElement
        {
            public bool isRequireFrom;
            public int frame;
            public Easing easing;
        }

        List<BaseAnimationElement> animationElements;

        public Animation()
        {
            animationElements = new List<BaseAnimationElement>();
        }

        public IEnumerator<object> GetAnimationCoRutine(asd.DrawnObject2D object2D)
        {
            foreach (var item in animationElements)
            {
                switch (item)
                {
                    case MoveAnimationElement move:
                        yield return GetMoveCoRutine(object2D, move);
                        break;
                    case ScaleAnimationElement scale:
                        yield return GetScaleCoRutine(object2D, scale);
                        break;
                    case RotateAnimationElement rotate:
                        yield return GetRotateCoRutine(object2D, rotate);
                        break;
                    case AlphaAnimationElement alpha:
                        yield return GetAlphaCoRutine(object2D, alpha);
                        break;
                    case SleepAnimationElement sleep:
                        for (int i = 0; i < sleep.frame; i++)
                        {
                            yield return null;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        IEnumerator<object> GetMoveCoRutine(asd.DrawnObject2D object2D, MoveAnimationElement move)
        {
            asd.Vector2DF start = move.isRequireFrom ? move.from : object2D.Position;
            for (int i = 0; i < move.frame; i++)
            {
                object2D.Position = new asd.Vector2DF(GetEasing(move.easing, i, start.X, move.to.X, move.frame), GetEasing(move.easing, i, start.Y, move.to.Y, move.frame));
                yield return null;
            }
        }

        IEnumerator<object> GetScaleCoRutine(asd.DrawnObject2D object2D, ScaleAnimationElement scale)
        {
            asd.Vector2DF start = scale.isRequireFrom ? scale.from : object2D.Scale;
            for (int i = 0; i < scale.frame; i++)
            {
                object2D.Scale = new asd.Vector2DF(GetEasing(scale.easing, i, start.X, scale.to.X, scale.frame), GetEasing(scale.easing, i, start.Y, scale.to.Y, scale.frame));
                yield return null;
            }
        }

        IEnumerator<object> GetRotateCoRutine(asd.DrawnObject2D object2D, RotateAnimationElement rotate)
        {
            float start = rotate.isRequireFrom ? rotate.from : object2D.Angle;
            for (int i = 0; i < rotate.frame; i++)
            {
                object2D.Angle = GetEasing(rotate.easing, i, start, rotate.to, rotate.frame);
                yield return null;
            }
        }

        IEnumerator<object> GetAlphaCoRutine(asd.DrawnObject2D object2D, AlphaAnimationElement alpha)
        {
            Byte start = alpha.isRequireFrom ? alpha.from : object2D.Color.A;
            for (int i = 0; i < alpha.frame; i++)
            {
                var color = object2D.Color;
                color.A = (byte)GetEasing(alpha.easing, i, start, alpha.to, alpha.frame);
                object2D.Color = color;
                yield return null;
            }
        }

        float GetEasing(Easing easing, int current, float start, float end, int frame)
        {
            float s = 0.2f;

            float t = (float)current / frame;
            if (current > frame) t = 1;
            else if (current < 0) t = 0;

            float d = end - start;

            switch (easing)
            {
                case Easing.Linear:
                    return start + d * t;
                case Easing.InSine:
                    return -d * (float)Math.Cos(t * asd.MathHelper.DegreeToRadian(90)) + end + start;
                case Easing.OutSine:
                    return d * (float)Math.Sin(t * asd.MathHelper.DegreeToRadian(90)) + start;
                case Easing.InOutSine:
                    return -d / 2 * ((float)Math.Cos(t * Math.PI) - 1) + start;
                case Easing.InQuad:
                    return -d * t * t + start;
                case Easing.OutQuad:
                    return -d * t * (t - 2) + start;
                case Easing.InOutQuad:
                    if (t / 2 < 1)
                        return d / 2 * t * t + start;
                    --t;
                    return -d * (t * (t - 2) - 1) + start;
                case Easing.InCubic:
                    return d * t * t * t + start;
                case Easing.OutCubic:
                    --t;
                    return d * (t * t * t + 1) + start;
                case Easing.InOutCubic:
                    if (t / 2 < 1)
                        return d / 2 * t * t * t + start;
                    t -= 2;
                    return d / 2 * (t * t * t + 2) + start;
                case Easing.InQuart:
                    return d * t * t * t * t + start;
                case Easing.OutQuart:
                    --t;
                    return -d * (t * t * t * t - 1) + start;
                case Easing.InOutQuart:
                    if (t / 2 < 1)
                        return d / 2 * t * t * t * t + start;
                    t -= 2;
                    return -d / 2 * (t * t * t * t - 2) + start;
                case Easing.InQuint:
                    return d * t * t * t * t * t + start;
                case Easing.OutQuint:
                    --t;
                    return d * (t * t * t * t * t + 1) + start;
                case Easing.InOutQuint:
                    if (t / 2 < 1)
                        return d / 2 * t * t * t * t * t + start;
                    t -= 2;
                    return d / 2 * (t * t * t * t * t + 2) + start;
                case Easing.InExpo:
                    return t == 0.0f ? start : d * (float)Math.Pow(2, 10 * (t - 1)) + start;
                case Easing.OutExpo:
                    return t == 1 ? d + start : d * (-(float)Math.Pow(2, -10 * t) + 1) + start;
                case Easing.InOutExpo:
                    if (t == 0.0)
                        return start;
                    if (t == end)
                        return end;
                    if (t / 2 < 1)
                        return d / 2 * (float)Math.Pow(2, 10 * (t - 1)) + start;
                    --t;
                    return d / 2 * (-(float)Math.Pow(2, -10 * t) + 2) + start;
                case Easing.InCirc:
                    return -d * ((float)Math.Sqrt(1 - t * t) - 1) + start;
                case Easing.OutCirc:
                    --t;
                    return d * (float)Math.Sqrt(1 - t * t) + start;
                case Easing.InOutCirc:
                    if (t / 2 < 1)
                        return -d / 2 * ((float)Math.Sqrt(1 - t * t) - 1) + start;
                    t -= 2;
                    return d / 2 * ((float)Math.Sqrt(1 - t * t) + 1) + start;
                case Easing.InBack:
                    return d * t * t * ((s + 1) * t - s) + start;
                case Easing.OutBack:
                    --t;
                    return d * (t * t * ((s + 1) * t * s) + 1) + start;
                case Easing.InOutBack:
                    s *= 1.525f;
                    if (t / 2 < 1)
                    {
                        return d * (t * t * ((s + 1) * t - s)) + start;
                    }
                    t -= 2;
                    return d / 2 * (t * t * ((s + 1) * t + s) + 2) + start;
                case Easing.InElastic:
                    return d * t * t * t * t * (float)Math.Sin(t * Math.PI * 4.5) + start;
                case Easing.OutElastic:
                    {
                        float t2 = (t - 1) * (t - 1);
                        return d * (1 - t2 * t2 * (float)Math.Cos(t * Math.PI * 4.5)) + start;
                    }
                case Easing.InOutElastic:
                    {
                        float t2;
                        if (t < 0.45)
                        {
                            t2 = t * t;
                            return d * 8 * t2 * t2 * (float)Math.Sin(t * Math.PI * 9) + start;
                        }
                        else if (t < 0.55)
                        {
                            return d * (0.5f + 0.75f * (float)Math.Sin(t * Math.PI * 4)) + start;
                        }
                        else
                        {
                            t2 = (t - 1) * (t - 1);
                            return d * (1 - 8 * t2 * t2 * (float)Math.Sin(t * Math.PI * 9)) + start;
                        }
                    }
                case Easing.InBounce:
                    return end - GetEasing(Easing.OutBounce, current, 0, d, frame) + start;
                case Easing.OutBounce:
                    if (t < 1 / 2.75)
                        return d * (7.5625f * t * t) + start;
                    else if (t < 2 / 2.75)
                    {
                        t -= 1.5f / 2.75f;
                        return d * (7.5625f * t * t + 0.75f) + start;
                    }
                    else if (t < 2.5 / 2.75)
                    {
                        t -= 2.25f / 2.75f;
                        return d * (7.5625f * t * t + 0.9375f) + start;
                    }
                    else
                    {
                        t -= 2.625f / 2.75f;
                        return d * (7.5625f * t * t + 0.984375f) + start;
                    }
                case Easing.InOutBounce:
                    if (t < 0.5f)
                        return GetEasing(Easing.InBounce, current * 2, d, end, frame) * 0.5f + start;
                    else
                        return GetEasing(Easing.OutBounce, current * 2 - frame, 0, d, frame) * 0.5f + start + d * 0.5f;
                default:
                    return 0;
            }
        }

        public void Move(asd.Vector2DF from, asd.Vector2DF to, int frame, Easing easing = Easing.Linear)
        {
            var element = new MoveAnimationElement();
            element.from = from;
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = true;
            animationElements.Add(element);
        }

        public void MoveTo(asd.Vector2DF to, int frame, Easing easing = Easing.Linear)
        {
            var element = new MoveAnimationElement();
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = false;
            animationElements.Add(element);
        }

        class MoveAnimationElement : BaseAnimationElement
        {
            public asd.Vector2DF to;
            public asd.Vector2DF from;
        }

        public void Scale(asd.Vector2DF from, asd.Vector2DF to, int frame, Easing easing = Easing.Linear)
        {
            var element = new ScaleAnimationElement();
            element.from = from;
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = true;
            animationElements.Add(element);
        }

        public void ScaleTo(asd.Vector2DF to, int frame, Easing easing = Easing.Linear)
        {
            var element = new ScaleAnimationElement();
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = false;
            animationElements.Add(element);
        }

        class ScaleAnimationElement : BaseAnimationElement
        {
            public asd.Vector2DF to;
            public asd.Vector2DF from;
        }

        public void Rotate(float from, float to, int frame, Easing easing = Easing.Linear)
        {
            var element = new RotateAnimationElement();
            element.from = from;
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = true;
            animationElements.Add(element);
        }

        public void RotateTo(float to, int frame, Easing easing = Easing.Linear)
        {
            var element = new RotateAnimationElement();
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = false;
            animationElements.Add(element);
        }

        class RotateAnimationElement : BaseAnimationElement
        {
            public float to;
            public float from;
        }

        public void Alpha(byte from, byte to, int frame, Easing easing = Easing.Linear)
        {
            var element = new AlphaAnimationElement();
            element.from = from;
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = true;
            animationElements.Add(element);
        }

        public void AlphaTo(byte to, int frame, Easing easing = Easing.Linear)
        {
            var element = new AlphaAnimationElement();
            element.to = to;
            element.frame = frame > 0 ? frame : 1;
            element.easing = easing;
            element.isRequireFrom = false;
            animationElements.Add(element);
        }

        class AlphaAnimationElement : BaseAnimationElement
        {
            public byte to;
            public byte from;
        }

        public void Sleep(int frame)
        {
            var element = new ScaleAnimationElement();
            element.frame = frame > 0 ? frame : 1;
            element.isRequireFrom = false;
            animationElements.Add(element);
        }

        class SleepAnimationElement : BaseAnimationElement
        {
        }

        public enum Easing
        {
            Linear,
            InSine,
            OutSine,
            InOutSine,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InBack,
            OutBack,
            InOutBack,
            InElastic,
            OutElastic,
            InOutElastic,
            InBounce,
            OutBounce,
            InOutBounce,
        }
    }
}
