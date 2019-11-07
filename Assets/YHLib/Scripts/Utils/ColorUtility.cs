using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH
{
    public class ColorUtility
    {
        public static Vector3 Frac(Vector3 v)
        {
            v.x -= Mathf.Floor(v.x);
            v.y -= Mathf.Floor(v.y);
            v.z -= Mathf.Floor(v.z);
            return v;
        }

        public static Vector3 Abs(Vector3 v)
        {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.z = Mathf.Abs(v.z);
            return v;
        }

        public static Vector3 Saturate(Vector3 v)
        {
            v.x = Mathf.Max(0, Mathf.Min(1, v.x));
            v.y = Mathf.Max(0, Mathf.Min(1, v.y));
            v.z = Mathf.Max(0, Mathf.Min(1, v.z));
            return v;
        }

        public static Color HsvToRgb(Vector3 hsv)
        {
            Vector3 K = new Vector3(1.0f, 2.0f / 3.0f, 1.0f / 3.0f);
            Vector3 Kx = new Vector3(K.x, K.x, K.x);
            Vector3 Kw = new Vector3(3.0f,3.0f,3.0f);
            Vector3 Hx = new Vector3(hsv.x, hsv.x, hsv.x);

            Vector3 P = Abs(Frac(Hx + K) * 6.0f - Kw);
            //Debug.Log(P);
            //P = Saturate(P - Kx);
            //P.x = Mathf.Lerp(K.x, P.x, hsv.y);
            //P.y = Mathf.Lerp(K.x, P.y, hsv.y);
            //P.z = Mathf.Lerp(K.x, P.z, hsv.y);
            Vector3 c = hsv.z * Vector3.LerpUnclamped(Kx, Saturate(P-Kx), hsv.y);
            return new Color(c.x, c.y, c.z);
        }

        public static Vector3 RgbToHsv(Color color)
        {
            Vector4 K = new Vector4(0.0f, -1.0f / 3.0f, 2.0f / 3.0f, -1.0f);
            Vector4 P1=new Vector4(color.b,color.g,K.w,K.z);
            Vector4 P2 = new Vector4(color.g, color.b, K.x, K.y);
            Vector4 P= color.g >= color.b?P2:P1;
            Vector4 Q1 = new Vector4(P.x, P.y, P.w, color.r);
            Vector4 Q2 = new Vector4(color.r, P.y, P.z, P.x);
            Vector4 Q = color.r>P.x ? Q2 : Q1;
            float D = Q.x - Mathf.Min(Q.w, Q.y);
            float E = 1e-10f;
            return new Vector3(Mathf.Abs(Q.z + (Q.w - Q.y) / (6.0f * D + E)), D / (Q.x + E), Q.x);
        }
    }
}
