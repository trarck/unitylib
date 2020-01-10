using UnityEngine;
using System.Collections;

namespace YH
{
	public class Capture
	{
		public static void SaveScreenshot(string file)
		{
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            System.IO.File.WriteAllBytes(file, texture.EncodeToPNG());
            // cleanup
            UnityEngine.Object.Destroy(texture);
        }

        /// <summary>
        /// 最好在一帧最后截图
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Texture2D GetScreenshot(Rect rect)
		{
			Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

			//读取屏幕像素信息并存储为纹理数据。
			screenShot.ReadPixels(rect, 0, 0);
			screenShot.Apply();

			return screenShot;
		}

		public static Texture2D GetScreenshotAndSave(Rect rect, string file)
		{
			Texture2D screenShot = GetScreenshot(rect);

			// 然后将这些纹理数据，成一个png图片文件
			byte[] bytes = screenShot.EncodeToPNG();

			System.IO.File.WriteAllBytes(file, bytes);

			return screenShot;
		}
	}
}