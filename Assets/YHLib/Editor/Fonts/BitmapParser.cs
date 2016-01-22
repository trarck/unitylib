using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.IO;

namespace YH.Font
{
    public class BitmapParser
    {
        public BitmapFont Parse(string fontFile)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(fontFile))
            {
                doc.Load(fontFile);
            }
            else
            {
                TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(fontFile);
                if (text)
                {
                    doc.LoadXml(text.text);
                }
                else
                {
                    Debug.LogError("No font file find. " + fontFile);
                }
            }

            return Parse(doc, fontFile);
        }

        public BitmapFont Parse(XmlDocument doc, string fontFile)
        {
            BitmapXMLReader bitmapXmlReader = new BitmapXMLReader();

            BitmapFont fnt = bitmapXmlReader.Load(doc);
            
            ParsePages(fnt, fontFile);
            return fnt;
        }

        public void ParsePages(BitmapFont fnt,string fontFile)
        {
            string[] pages = fnt.pages;
            Texture2D[] texturePages = new Texture2D[pages.Length *  (fnt.packed?4:1)];
            int index = 0;
            foreach (string pageImagePath in pages)
            {
                //Find original font texture
                string imagePath = Path.GetDirectoryName(fontFile) + "/" + pageImagePath;
                Texture2D inputTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(imagePath, typeof(Texture2D));

                //Make sure font texture is readable
                TextureImporter inputTextureImp = (TextureImporter)AssetImporter.GetAtPath(imagePath);
                inputTextureImp.textureType = TextureImporterType.Advanced;
                inputTextureImp.isReadable = true;
                inputTextureImp.maxTextureSize = 4096;
                inputTextureImp.mipmapEnabled = false;
                AssetDatabase.ImportAsset(imagePath, ImportAssetOptions.ForceSynchronousImport);

                //Create distance field from texture
                //处理通道
                if (fnt.packed)
                {
                    Texture2D distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.RED, inputTexture.width);

                    byte[] bytes = distanceField.EncodeToPNG();
                    File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", bytes);

                    texturePages[index++] = distanceField;
                    distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.GREEN, inputTexture.width);
                    bytes = distanceField.EncodeToPNG();
                    File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", bytes);
                    texturePages[index++] = distanceField;
                    distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.BLUE, inputTexture.width);
                    bytes = distanceField.EncodeToPNG();
                    File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", bytes);
                    texturePages[index++] = distanceField;
                    distanceField = DistanceField.CreateDistanceFieldTexture(inputTexture, DistanceField.TextureChannel.ALPHA, inputTexture.width);
                    bytes = distanceField.EncodeToPNG();
                    File.WriteAllBytes(Path.GetDirectoryName(fontFile) + "/index_" + index + ".png", bytes);
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

                //Save atlas as png
                byte[] pngData = pageAtlas.EncodeToPNG();
                string outputPath = fontFile.Substring(0, fontFile.LastIndexOf('.')) + "_pak.png";
                File.WriteAllBytes(outputPath, pngData);
                AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceSynchronousImport);

                //Set correct texture format
                TextureImporter texImp = (TextureImporter)TextureImporter.GetAtPath(outputPath);
                texImp.textureType = TextureImporterType.Advanced;
                texImp.isReadable = true;
                texImp.textureFormat = TextureImporterFormat.Alpha8;
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