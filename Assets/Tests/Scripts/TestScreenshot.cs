using UnityEngine;

using YH;

public class TestScreenshot
{
    public string GetPhotoPath(string name)
    {
        return Application.persistentDataPath + "/" + name + ".png";
    }

    public string GetSavePhotoPath(string name)
    {
#if UNITY_EDITOR
        return Application.persistentDataPath + "/" + name + ".png";
#else  //UNITY_IOS || UNITY_ANDROID
        return name + ".png";
#endif
    }

    public void SaveScreenshot()
    {
        Rect captureRect = new Rect(0, 0, Camera.main.pixelWidth , Camera.main.pixelHeight);

        string saveFile = GetPhotoPath("screenshot.png");

        Texture2D texture= Capture.GetScreenshotAndSave(captureRect, saveFile);
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
        float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
}
