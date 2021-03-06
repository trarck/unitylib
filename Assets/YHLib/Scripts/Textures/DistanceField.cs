﻿using UnityEngine;

namespace YH
{
    /* Class: DistanceField
	 * 
	 * Creates a distance field texture from an outline texture.
	 * 
	 * It is currently using a brute force algorithm taken from Joakim Hårsmanns C
	 * implementation https://bitbucket.org/harsman/distfield. It is quite slow for large images
	 * and requires a rather high  * resolution input image to give satisfactory results. 
	 */
    public class DistanceField
    {
        /* Constant: SearchRadius
		 * 
		 * The radius to search for an outline. Increasing this leads to a
		 * more blurred distance field which takes a longer time to compute
		 */
        static int SearchRadius = 20;

        public enum TextureChannel
        {
            BLUE = 0,
            GREEN = 1,
            RED = 2,
            ALPHA = 3
        }

        static void ShowBuffer(byte[] buff,int col,int row)
        {
            Debug.Log("show:" + col + "," + row+":"+buff.Length);
            string lines= "";
            for(int i = row-1; i>=0; --i)
            {
                for(int j = 0; j < col; ++j)
                {
                    lines += (buff[i * col + j]>0?"1":"0")+" ";
                }
                lines += "\n";
            }

            Debug.Log(lines);
        }

        public static Texture2D CreateDistanceFieldTexture(Texture2D inputTexture, TextureChannel channel, int outSize,Color defaultColor)
        {
            //Extract channel from input texture
            byte[] inputBuffer = GetTextureChannel(inputTexture, channel);

            //ShowBuffer(inputBuffer, inputTexture.width, inputTexture.height);

            //Create distance field
            byte[] outputBuffer = inputBuffer;//render(inputBuffer, inputTexture.width, inputTexture.height, outSize);

            //Put distance field into output texture
            Texture2D outputTexture = new Texture2D(outSize, outSize, TextureFormat.RGBA32, false);
            SetTextureChannel(outputTexture, TextureChannel.ALPHA, outputBuffer, defaultColor);

            return outputTexture;
        }

        private static byte[] GetTextureChannel(Texture2D tex, TextureChannel channel)
        {
            //GetPixels在unity5.3.x中有bug,每一行的前4个像素被放到后面了。
            //Color[] pixels = tex.GetPixels();

            Color32[] pixels = tex.GetPixels32();
            byte[] channelData = new byte[pixels.Length];

            for (int i = 0; i < pixels.Length; i++)
            {
                //channelData[i] = (byte)(255 * pixels[i][(int)channel]);
                switch (channel)
                {
                    case TextureChannel.RED:
                        channelData[i] = pixels[i].r;
                        break;
                    case TextureChannel.GREEN:
                        channelData[i] = pixels[i].g;
                        break;
                    case TextureChannel.BLUE:
                        channelData[i] = pixels[i].b;
                        break;
                    case TextureChannel.ALPHA:
                        channelData[i] = pixels[i].a;
                        break;
                }
            }

            return channelData;
        }

        private static void SetTextureChannel(Texture2D tex, TextureChannel channel, byte[] channelData,Color defaultColor)
        {
            if (tex.height * tex.width != channelData.Length)
            {
                throw new System.Exception("Invalid length of channel data");
            }

            //Convert channel to array of pixels
            Color[] pixels = new Color[channelData.Length];
            for (int i = 0; i < pixels.Length; i++)
            {
                Color pix = new Color(defaultColor.r,defaultColor.g,defaultColor.b,defaultColor.a);
                pix[(int)channel] = channelData[i] / 255f;
                //pix[0] = channelData[i] / 255f;
                //pix.a = 1;
                pixels[i] = pix;
            }

            //Update texture
            tex.SetPixels(pixels);
            tex.Apply();
        }


        /*
		 * mindist() and render() shamelessly stolen from Joakim Hårsmanns distfield calculator
		 * https://bitbucket.org/harsman/distfield
		 */

        static float mindist(byte[] buffer, int w, int h, int x, int y, int r, float maxdist)
        {
            int i, j, startx, starty, stopx, stopy;
            bool hit;
            float d, dx, dy;
            float mind = maxdist;
            byte p;

            p = buffer[y * w + x];
            bool outside = (p == 0);

            startx = Mathf.Max(0, x - r);
            starty = Mathf.Max(0, y - r);
            stopx = Mathf.Min(w, x + r);
            stopy = Mathf.Min(h, y + r);

            for (i = starty; i < stopy; i++)
            {
                for (j = startx; j < stopx; j++)
                {
                    p = buffer[i * w + j];
                    dx = j - x;
                    dy = i - y;
                    d = dx * dx + dy * dy;
                    hit = (p != 0) == outside;
                    if (hit && (d < mind))
                    {
                        mind = d;
                    }
                    if (d > maxdist)
                        Debug.LogWarning("Too big\n");
                }
            }

            if (outside)
                return Mathf.Sqrt(mind);
            else
                return -Mathf.Sqrt(mind);
        }

        static byte[] render(byte[] input, int w, int h, int outsize)
        {
            byte[] output = new byte[outsize * outsize];
            int x, y, ix, iy;
            float d;
            byte di;
            int sx = w / outsize;
            int sy = h / outsize;
            /* No sense of searching further with only 8-bits of output
			 * precision
			 */
            int r = SearchRadius;
            float maxsq = 2 * r * r;
            float max = Mathf.Sqrt(maxsq);

            for (y = 0; y < outsize; y++)
            {
                for (x = 0; x < outsize; x++)
                {
                    ix = (x * sx) + (sx / 2);
                    iy = (y * sy) + (sy / 2);
                    d = mindist(input, w, h, ix, iy, r, maxsq);
                    di = (byte)(127.5f + 127.5f * (-d / max));
                    {
                        //Debug.Log("Distance is " + d);
                    }
                    output[y * outsize + x] = di;
                }
            }

            return output;
        }

    }
}