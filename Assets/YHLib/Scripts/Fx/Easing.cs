using UnityEngine;
using System.Collections;

namespace YH
{

    public class Easing
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t">经过的时间</param>
        /// <param name="b">初始位置</param>
        /// <param name="c"></param>
        /// <param name="d">总的时间</param>
        /// <returns></returns>
        public static float EaseLinear(float t, float b, float c, float d)
        {
            return b + c * t / d;
        }

        public static float EaseSwing(float t, float b, float c, float d)
        {
            return ((-Mathf.Cos(t / d * Mathf.PI) / 2.0f) + 0.5f) * c + b;
        }

        public static float EaseInQuad(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }

        public static float EaseOutQuad(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }

        public static float EaseInOutQuad(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t + b;
            return -c / 2 * ((--t) * (t - 2) - 1) + b;
        }

        public static float EaseInCubic(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }

        public static float EaseOutCubic(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }

        public static float EaseInOutCubic(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }

        public static float EaseInQuart(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t + b;
        }

        public static float EaseOutQuart(float t, float b, float c, float d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }

        public static float EaseInOutQuart(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }

        public static float EaseInQuint(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }

        public static float EaseOutQuint(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }

        public static float EaseInOutQuint(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }

        public static float EaseInSine(float t, float b, float c, float d)
        {
            return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
        }

        public static float EaseOutSine(float t, float b, float c, float d)
        {
            return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
        }

        public static float EaseInOutSine(float t, float b, float c, float d)
        {
            return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
        }

        public static float EaseInExpo(float t, float b, float c, float d)
        {
            return (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b;
        }

        public static float EaseOutExpo(float t, float b, float c, float d)
        {
            return (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
        }

        public static float EaseInOutExpo(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            if (t == d) return b + c;
            if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
            return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
        }

        public static float EaseInCirc(float t, float b, float c, float d)
        {
            return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
        }

        public static float EaseOutCirc(float t, float b, float c, float d)
        {
            return c * Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
        }

        public static float EaseInOutCirc(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
            return c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;
        }

        public static float EaseInElastic(float t, float b, float c, float d)
        {
            float s = 1.70158f, p = d * 0.3f, a = c;
            if (t == 0) return b;
            if ((t /= d) == 1) return b + c;
            if (a < Mathf.Abs(c))
            {
                a = c;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(c / a);
            }
            return -(a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
        }

        public static float EaseOutElastic(float t, float b, float c, float d)
        {
            float s = 1.70158f, p = d * 0.3f, a = c;
            if (t == 0) return b;
            if ((t /= d) == 1) return b + c;
            if (a < Mathf.Abs(c))
            {
                a = c;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(c / a);
            }
            return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b;
        }

        public static float EaseInOutElastic(float t, float b, float c, float d)
        {
            float s = 1.70158f, p = d * (0.3f * 1.5f), a = c;
            if (t == 0) return b;
            if ((t /= d / 2) == 2) return b + c;
            if (a < Mathf.Abs(c))
            {
                a = c;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(c / a);
            }
            if (t < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
            return a * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) * 0.5f + c + b;
        }

        public static float EaseInBack(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            return c * (t /= d) * t * ((s + 1) * t - s) + b;
        }

        public static float EaseOutBack(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
        }

        public static float EaseInOutBack(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= 1.525f) + 1) * t - s)) + b;
            return c / 2 * ((t -= 2) * t * (((s *= 1.525f) + 1) * t + s) + 2) + b;
        }

        public static float EaseInBounce(float t, float b, float c, float d)
        {
            return c - EaseOutBounce(d - t, 0, c, d) + b;
        }

        public static float EaseOutBounce(float t, float b, float c, float d)
        {
            if ((t /= d) < (1 / 2.75f))
            {
                return c * (7.5625f * t * t) + b;
            }
            else if (t < (2 / 2.75f))
            {
                return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + b;
            }
            else if (t < (2.5f / 2.75f))
            {
                return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .935f) + b;
            }
            else
            {
                return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
            }
        }

        public static float EaseInOutBounce(float t, float b, float c, float d)
        {
            if (t < d / 2) return EaseInBounce(t * 2.0f, 0, c, d) * 0.5f + b;
            return EaseOutBounce(t * 2.0f - d, 0, c, d) * 0.5f + c * 0.5f + b;
        }
    }
}