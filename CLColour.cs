using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColourfulLib
{
    public class CLColour
    {
        public byte R { get; private set; } = 0;
        public byte G { get; private set; } = 0;
        public byte B { get; private set; } = 0;
        public byte A { get; private set; } = 255;

        /// <summary>
        /// Initialise a new colour using RGBA byte values.
        /// </summary>
        /// <param name="r">Red value (0 to 255)</param>
        /// <param name="g">Green value (0 to 255)</param>
        /// <param name="b">Blue value (0 to 255)</param>
        /// <param name="a">Alpha/Transparency value (0 to 255)</param>
        public CLColour(byte r, byte g, byte b, byte a = 255)
        {
            r = (byte)CLMath.Clamp(0, r, 255);
            g = (byte)CLMath.Clamp(0, g, 255);
            b = (byte)CLMath.Clamp(0, b, 255);
            a = (byte)CLMath.Clamp(0, a, 255);

            R = r;
            G = g;
            B = b;
            A = a;
        }
        /// <summary>
        /// Initialise a new colour using RGBA float values.
        /// </summary>
        /// <param name="r">Red value (0 to 1)</param>
        /// <param name="g">Green value (0 to 1)</param>
        /// <param name="b">Blue value (0 to 1)</param>
        /// <param name="a">Alpha/Transparency value (0 to 1)</param>
        public CLColour(float r, float g, float b, float a = 1f)
        {
            r = CLMath.Clamp(0, r, 1);
            g = CLMath.Clamp(0, g, 1);
            b = CLMath.Clamp(0, b, 1);
            a = CLMath.Clamp(0, a, 1);

            R = (byte)(255 * r);
            G = (byte)(255 * g);
            B = (byte)(255 * b);
            A = (byte)(255 * a);
        }
        /// <summary>
        /// Initialise a new colour using Microsoft's Color.
        /// </summary>
        /// <param name="color">Microsoft colour struct</param>
        public CLColour(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }
        /// <summary>
        /// Initialise a new colour using hex.
        /// </summary>
        /// <param name="hex">Hex value, with or without a hashtag at start.</param>
        public CLColour(string hex)
        {
            if (hex.Contains("#"))
            {
                hex = hex.Trim('#');
            }

            hex = hex.ToUpper();

            if (hex.Length < 6 || hex.Length % 2 == 1 || string.IsNullOrWhiteSpace(hex) || !hex.All("0123456789ABCDEF".Contains))
            {
                R = 0;
                G = 0;
                B = 0;
                A = 0;
                return;
            }

            if (hex.Length > 8)
            {
                hex = hex.Substring(0, 8);
            }

            byte[] hexResult = new byte[hex.Length / 2];

            int chunk = 0;

            for (int i = 0; i < hex.Length; i += 2)
            {
                string hexChunk = hex.Substring(i, 2);

                hexResult[chunk] = Convert.ToByte(hexChunk, 16);

                chunk++;
            }

            R = (byte)CLMath.Clamp(0, hexResult[0], 255);
            G = (byte)CLMath.Clamp(0, hexResult[1], 255);
            B = (byte)CLMath.Clamp(0, hexResult[2], 255);
            A = 255;
            if (hexResult.Length > 3)
            {
                A = (byte)CLMath.Clamp(0, hexResult[3], 255);
            }
        }
        /// <summary>
        /// Initialise new CLColour using RGBA string separated by a splitChr.
        /// </summary>
        /// <param name="RgbaStr">Byte values joint with splitChr</param>
        /// <param name="splitChr">Char that separates byte values</param>
        /// <param name="isHSVA">Optional, the RgbaStr will be treated as HSVA</param>
        public CLColour(string RgbaStr, char splitChr, bool isHSVA = false)
        {
            string mainStr = new string(RgbaStr.Where(c => char.IsDigit(c) || c.Equals(splitChr)).ToArray());

            string[] strArray = mainStr.Split(splitChr);

            if (strArray.Length < 3)
            {
                R = 0;
                G = 0;
                B = 0;
                A = 0;
                return;
            }

            byte[] rgbaBytes = new byte[4]
            {
                Convert.ToByte(strArray[0]),
                Convert.ToByte(strArray[1]),
                Convert.ToByte(strArray[2]),
                255
            };

            if (isHSVA)
            {
                int[] hsvaArray = new int[4]
                {
                    Convert.ToInt32(strArray[0]),
                    Convert.ToInt32(strArray[1]),
                    Convert.ToInt32(strArray[2]),
                    100
                };

                if (strArray.Length > 3)
                {
                    hsvaArray[3] = Convert.ToInt32(strArray[3]);
                }

                rgbaBytes = HSVAToRGBA(hsvaArray[0], hsvaArray[1], hsvaArray[2], hsvaArray[3]);

                R = rgbaBytes[0];
                G = rgbaBytes[1];
                B = rgbaBytes[2];
                A = rgbaBytes[3];

                return;
            }

            if (strArray.Length > 3)
            {
                rgbaBytes[3] = Convert.ToByte(strArray[3]);
            }

            R = rgbaBytes[0];
            G = rgbaBytes[1];
            B = rgbaBytes[2];
            A = rgbaBytes[3];
        }
        /* ================================================== */
        /// <summary>
        /// Get Colour's RGBA.
        /// </summary>
        /// <returns>Byte array [R,G,B,A]</returns>
        public byte[] GetRGBA()
        {
            return new byte[4] { R, G, B, A };
        }
        /// <summary>
        /// Get string of colour's RGBA.
        /// </summary>
        /// <returns>String of RGBA as "R,G,B,A"</returns>
        public string GetRGBAString()
        {
            return $"{R},{G},{B},{A}";
        }
        /// <summary>
        /// Get Hex of this colour.
        /// </summary>
        /// <param name="withHashtag">Should the return array include hastag.</param>
        /// <returns>Hex as string</returns>
        public string GetHEX(bool withHashtag = false)
        {
            if (withHashtag)
            {
                return $"#{R:X2}{G:X2}{B:X2}{A:X2}";
            }
            return $"{R:X2}{G:X2}{B:X2}{A:X2}";
        }
        /// <summary>
        /// Get Hex as array of this colour.
        /// </summary>
        /// <returns>string array [R,G,B,A] with hex as values</returns>
        public string[] GetHEXArray()
        {
            return new string[4] { R.ToString("X2"), G.ToString("X2"), B.ToString("X2"), A.ToString("X2") };
        }
        /// <summary>
        /// Get colour HSVA as int array.
        /// </summary>
        /// <returns>Int array [H,S,V,A]</returns>
        public int[] GetHSVA()
        {
            float normalisedR = CLMath.NormaliseByte(R);
            float normalisedG = CLMath.NormaliseByte(G);
            float normalisedB = CLMath.NormaliseByte(B);
            float normalisedA = CLMath.NormaliseByte(A);

            float[] rgbArray = new float[3] { normalisedR, normalisedG, normalisedB };

            rgbArray.ToList().Sort();

            float minRGB = rgbArray.First();

            float maxRGB = rgbArray.Last();

            float difference = maxRGB - minRGB;

            float value = maxRGB * 100;

            float saturation = 0f;
            if (maxRGB != 0)
            {
                saturation = difference / maxRGB * 100;
            }

            float hue = 0f;
            if (maxRGB == normalisedR)
            {
                hue = 60 * (((normalisedG - normalisedB) / difference) % 6);
            }
            if (maxRGB == normalisedG)
            {
                hue = 60 * (((normalisedB - normalisedR) / difference) + 2);
            }
            if (maxRGB == normalisedB)
            {
                hue = 60 * (((normalisedR - normalisedG) / difference) + 4);
            }

            int[] HSVA = new int[4]
            {
                (int)Math.Round(hue, MidpointRounding.AwayFromZero),
                (int)Math.Round(saturation, MidpointRounding.AwayFromZero),
                (int)Math.Round(value, MidpointRounding.AwayFromZero),
                (int)Math.Round(normalisedA, MidpointRounding.AwayFromZero) * 100
            };

            return HSVA;
        }
        /* ================================================== */
        /// <summary>
        /// Normalise RGBA colours.
        /// </summary>
        /// <returns>Float array [R,G,B,A] with values between 0 and 1</returns>
        public float[] NormaliseSelf()
        {
            return new float[4] { (R / 255f), (G / 255f), (B / 255f), (A / 255f) };
        }
        /// <summary>
        /// Convert to Microsoft's color.
        /// </summary>
        /// <returns>Microsoft's color</returns>
        public Color GetMSColour()
        {
            return Color.FromArgb(A, R, G, B);
        }
        /* ================================================== */
        /// <summary>
        /// Convert HSVA to RGBA.
        /// </summary>
        /// <param name="hue">Hue value (0 to 360)</param>
        /// <param name="sat">Saturation value (0 to 100)</param>
        /// <param name="val">Value value (0 to 100)</param>
        /// <param name="alp">Alpha/Transparency value (0 to 100)</param>
        /// <returns>Byte array [R,G,B,A]</returns>
        public static byte[] HSVAToRGBA(int hue, int sat, int val, int alp = 100)
        {
            float h = CLMath.Clamp(0, hue, 360);
            float s = CLMath.Clamp(0, sat, 100);
            float v = CLMath.Clamp(0, val, 100);
            float a = CLMath.Clamp(0, alp, 100);

            s /= 100;
            v /= 100;

            float chroma = v * s;

            float inter = chroma * (1 - Math.Abs(((h / 60) % 2) - 1));

            float match = v - chroma;

            float intR = 0f;
            float intG = 0f;
            float intB = 0f;
            if (0 <= h && h < 60)
            {
                intR = chroma;
                intG = inter;
                intB = 0;
            }
            if (60 <= h && h < 120)
            {
                intR = inter;
                intG = chroma;
                intB = 0;
            }
            if (120 <= h && h < 180)
            {
                intR = 0;
                intG = chroma;
                intB = inter;
            }
            if (180 <= h && h < 240)
            {
                intR = 0;
                intG = inter;
                intB = chroma;
            }
            if (240 <= h && h < 300)
            {
                intR = inter;
                intG = 0;
                intB = chroma;
            }
            if (300 <= h && h <= 360)
            {
                intR = chroma;
                intG = 0;
                intB = inter;
            }

            byte[] rgba = new byte[4]
            {
                (byte)Math.Round((intR + match) * 255, MidpointRounding.AwayFromZero),
                (byte)Math.Round((intG + match) * 255, MidpointRounding.AwayFromZero),
                (byte)Math.Round((intB + match) * 255, MidpointRounding.AwayFromZero),
                (byte)Math.Round((double)((a / 100) * 255), MidpointRounding.AwayFromZero)
            };

            return rgba;
        }
        /// <summary>
        /// Get Bitmap's average RGBA.
        /// </summary>
        /// <param name="bmp">Target Bitmap</param>
        /// <returns>CLColour</returns>
        public static CLColour AverageImage(Bitmap bmp)
        {
            if (bmp == null)
            {
                return new CLColour(0, 0, 0, 0);
            }
            
            int[] RgbaSum = new int[4] { 0, 0, 0, 0 };

            int pixelsAmount = bmp.Width * bmp.Height;

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;

                byte[] RgbaValues = new byte[bytes];

                Marshal.Copy(ptr, RgbaValues, 0, bytes);

                for (int i = 0; i < RgbaValues.Length; i += 4)
                {
                    RgbaSum[2] += RgbaValues[i];
                    RgbaSum[1] += RgbaValues[i + 1];
                    RgbaSum[0] += RgbaValues[i + 2];
                    RgbaSum[3] += RgbaValues[i + 3];
                }

                byte[] RGBA = new byte[4]
                {
                    (byte)(RgbaSum[0] / pixelsAmount),
                    (byte)(RgbaSum[1] / pixelsAmount),
                    (byte)(RgbaSum[2] / pixelsAmount),
                    (byte)(RgbaSum[3] / pixelsAmount)
                };

                return new CLColour(RGBA[0], RGBA[1], RGBA[2], RGBA[3]);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
        }
        /// <summary>
        /// Get Bitmap's average HSVA colour.
        /// </summary>
        /// <param name="bmp">Target Bitmap</param>
        /// <returns>int array [H,S,V,A]</returns>
        public static int[] AverageImageHSVA(Bitmap bmp)
        {
            byte[] RGBA = AverageImage(bmp).GetRGBA();

            return new CLColour(RGBA[0], RGBA[1], RGBA[2], RGBA[3]).GetHSVA();
        }
    }
}