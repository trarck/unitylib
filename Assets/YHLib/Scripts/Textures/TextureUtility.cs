using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH.Textures
{
    public class TextureUtility
    {
        /// <summary>
        /// 这里的sourceTexture会作为mat的mainTexture.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="sourceTexure"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static Texture2D MergeTexture(int width, int height, Texture sourceTexure, Material mat)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(sourceTexure, tempRT, mat);

            Texture2D output = new Texture2D(tempRT.width, tempRT.height, TextureFormat.RGBA32, false);
            RenderTexture.active = tempRT;

            output.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            output.Apply();
            output.filterMode = FilterMode.Bilinear;

            RenderTexture.ReleaseTemporary(tempRT);
            RenderTexture.active = null;

            return output;
        }

    }
}
