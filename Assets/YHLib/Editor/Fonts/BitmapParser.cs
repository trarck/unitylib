using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;

namespace YH.Fonts
{
    public class BitmapParser
    {
        public void Parse(BitmapFont fnt, string fontFile)
        {
            ParsePages(fnt, fontFile);
        }

        public void ParsePages(BitmapFont fnt,string fontFile)
        {
            string[] pages = fnt.pages;
            Texture2D[] texturePages = new Texture2D[pages.Length *  (fnt.packed?4:1)];
            int index = 0;
            //用在带通道的字体上，提取通道值后的颜色应该是白色的
            Color defaultColor = Color.white;

            foreach (string pageImagePath in pages)
            {
                //Find original font texture
                string imagePath = Path.GetDirectoryName(fontFile) + "/" + pageImagePath;
                Texture2D inputTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(imagePath, typeof(Texture2D));
                //Make sure font texture is readable
                TextureImporter inputTextureImp = (TextureImporter)AssetImporter.GetAtPath(imagePath);
                inputTextureImp.textureType = TextureImporterType.Default;
                inputTextureImp.isReadable = true;
                inputTextureImp.maxTextureSize = 4096;
                inputTextureImp.mipmapEnabled = false;
                inputTextureImp.textureFormat = TextureImporterFormat.RGBA32;

                AssetDatabase.ImportAsset(imagePath, ImportAssetOptions.ForceSynchronousImport);

                //Create distance field from texture
                //处理通道
                if (fnt.packed)
                {
                    Texture2D distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.BLUE, inputTexture.width,defaultColor);
                    //byte[] buff = distanceField.EncodeToPNG();
                    //File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", buff);
                    texturePages[index++] = distanceField;     

                    distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.GREEN, inputTexture.width, defaultColor);
                    //buff = distanceField.EncodeToPNG();
                    //File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", buff);
                    texturePages[index++] = distanceField;
                    
                    distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.RED, inputTexture.width, defaultColor);
                    //buff = distanceField.EncodeToPNG();
                    //File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", buff);
                    texturePages[index++] = distanceField;

                    distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.ALPHA, inputTexture.width, defaultColor);
                    //buff = distanceField.EncodeToPNG();
                    //File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", buff);
                    texturePages[index] = distanceField;
                }
                else
                {
                    Texture2D distanceField = inputTexture; //DistanceField.CreateDistanceFieldTexture(inputTexture, InputTextureChannel, inputTexture.width / DistanceFieldScaleFactor);
                    texturePages[index] = distanceField;
                }               

                index++;
            }

            //Create texture atlas
            if (texturePages.Length > 1)
            {
                Texture2D pageAtlas = new Texture2D(0, 0);
                fnt.pageOffsets = pageAtlas.PackTextures(texturePages, 0);

                //foreach(Rect r in fnt.pageOffsets)
                //{
                //    Debug.Log(r);
                //}

                //Save atlas as png
                byte[] pngData = pageAtlas.EncodeToPNG();
                string outputPath = fontFile.Substring(0, fontFile.LastIndexOf('.')) + "_pak.png";
                File.WriteAllBytes(outputPath, pngData);
                AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceSynchronousImport);

                //Set correct texture format
                TextureImporter texImp = (TextureImporter)TextureImporter.GetAtPath(outputPath);
                texImp.textureType = TextureImporterType.Default;
                texImp.isReadable = true;
                texImp.textureFormat = TextureImporterFormat.RGBA32;
                texImp.mipmapEnabled = false;
                AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceSynchronousImport);

                //Load the saved texture atlas
                fnt.pageAtlas = AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath);
            }
            else
            {
                fnt.pageAtlas = texturePages[0];
            }            
        }        
    }
}