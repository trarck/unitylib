using UnityEngine;
using System.Threading;
using System.IO;

namespace YH
{
	public class TextureSave
	{
		//保存贴图到文件，贴图经过Shader处理
        public static bool SaveTextureToPNG(Texture inputTex, Shader shader, string contents, string pngName)
        {
            RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);
            Material mat = new Material(shader);
            Graphics.Blit(inputTex, temp, mat);
            bool ret = SaveRenderTextureToPNG(temp, contents, pngName);
            RenderTexture.ReleaseTemporary(temp);
            return ret;
        }

        //将RenderTexture保存成一张png图片  
        public static bool SaveRenderTextureToPNG(RenderTexture rt, string contents, string pngName)
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();
            if (!Directory.Exists(contents))
                Directory.CreateDirectory(contents);
            FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
            Texture2D.DestroyImmediate(png);
            png = null;
            RenderTexture.active = prev;
            return true;
        }
	}
}