/*
 * ROTT2D
 * Unit: ROTT2D 256 Palette Sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 09-01-2012
 * 
 * This file is part of ROTT2D.
 *
 * ROTT2D is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ROTT2D is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with ROTT2D.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

//using ROTT2D.draw;
using ROTT2D.WAD.data;

namespace ROTT2D.WAD.palette
{

    #region RGBA color struct
    /// <summary>
    /// Palette RGBA color structure
    /// </summary>
    public struct Rott2DPaletteColor
    {
        /// <summary>
        /// Private values
        /// </summary>
        private byte _r, _g, _b, _a;  // RGBA color byte values

        /// <summary>
        /// RED
        /// </summary>
        public byte r
        {
            get {
                return this._r;
            }
            set {
                //sanity check for larger then 255
                /*if (value < 0xFF)                
                    this._r = value;
                else 
                    this._r = 0xFF;*/

                this._r = (value < 0xFF) ? value : (byte)0xFF;
            }
        }

        /// <summary>
        /// GREEN
        /// </summary>
        public byte g
        {
            get
            {
                return this._g;
            }
            set
            {
                //sanity check for larger then 255
                /*if (value < 0xFF)                
                    this._g = value;                
                else                
                    this._g = 0xFF;*/
                

                this._g = (value < 0xFF) ? value : (byte)0xFF;
            }
        }

        /// <summary>
        /// BLUE
        /// </summary>
        public byte b
        {
            get
            {
                return this._b;
            }
            set
            {
                //sanity check for larger then 255
                /* if (value < 0xFF)                
                    this._b = value;                
                else                
                    this._b = 0xFF; */
                
                this._b = (value < 0xFF) ? value : (byte)0xFF;
            }
        }

        /// <summary>
        /// ALPHA
        /// </summary>
        public byte a
        {
            get
            {
                return this._a;
            }
            set
            {
                //sanity check for larger then 255
                /* if (value < 0xFF)                
                    this._a = value;                
                else                
                    this._a = 0xFF; */

                this._a = (value < 0xFF) ? value : (byte)0xFF;
            }
        }
    }
    #endregion

    #region Color Palette Class
    /// <summary>
    /// Class for a 265 Color Palette
    /// In fact this class generates the full 256 spectrum ROTT colormap.
    /// </summary>
    public sealed class Rott2DPalette : Rott2DTextureLump, IRott2DPalette
    {
        /*
         * Notes:
         * ROTT's HUNTBGIN.WAD and DARKWAR.WAD files contain each two palette lump's,
         * named "PAL" and "AP_PAL". Both are 768 bytes (3x256 bytes = RGB) in size.
         * 
         * Palettes:
         * - "PAL" is the default palette used for nearly every texture in a WAD.
         * - "AP_PAL" is only used for two larger textures that serve for the original ROTT intro animation.
         *     --> texture AP_TITL : Apogee trademark logo
         *     --> texture AP_WRLD : Earth world image behind the Apogee logo
         * 
         * 
         */

        #region Public consts
        /// <summary>
        /// Private consts
        /// </summary>
        public const short PALETTE_LUMP_SIZE    = 768;     //size of the "pal" lump (raw format)
        public const short PALETTE_SIZE         = 256;     //size of one (colormap) palette
        public const byte  DEFAULT_ALPHA_VALUE  = 0xFF;    //255 = fully transparency

        public const byte  DEFAULT_MASKED_COLOR = 0xFF;   //255 index of the transparency color

        public readonly string PALETTE_DEFAULT_NAME;
        #endregion

        #region Private Vars
        /// <summary>
        /// private vars
        /// </summary>        
        private Rott2DPaletteColor[] _colorMap; //color map (array of colors)
        private byte _paletteMaskColor = DEFAULT_MASKED_COLOR;   //(default) masked transparent color
        private byte _paletteAlphaColor = DEFAULT_ALPHA_VALUE;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DPalette(ref byte[] paletteLumpData)
        {
            this.PALETTE_DEFAULT_NAME = "PAL";

            this.isReady = false;
            this.Name = this.PALETTE_DEFAULT_NAME;      //default name to PAL, should always be PAL since there is only one palette lump in ROTT
            this._rawData = paletteLumpData;
        
            this._colorMap = new Rott2DPaletteColor[PALETTE_SIZE]; //init our 256 colors color palette

            this.ProcessLumpData(); //generate palette data!
            this.ProcessPaletteBitmap(); //create bitmap buffer, only used for map editor & wad reader actually
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DPalette()
        {
            this.Dispose();
        }
        #endregion

        #region Getters & Setters
        /// <summary>
        /// Masked color (index)
        /// </summary>
        public byte MaskColorIndex
        {
            get { return this._paletteMaskColor; }
            set {
                /*if (value > 0xFF)                
                    this._transColor = 0xFF;                
                else                
                    this._transColor = value;*/

                this._paletteMaskColor = (value < 0xFF) ? value : (byte)0xFF;

                this.ProcessLumpData(); //recalc Alpha value
            }
        }

        /// <summary>
        /// Palette Alpha value color
        /// </summary>
        public byte AlphaColor
        {
            get { return this._paletteAlphaColor; }
            set
            {                
                this._paletteAlphaColor = (value < DEFAULT_ALPHA_VALUE) ? value : DEFAULT_ALPHA_VALUE;

                this.ProcessLumpData(); //recalc Alpha value
            }
        }
        #endregion

        #region Indexer
        /// <summary>
        /// ReadOnly Indexer for the palette
        /// </summary>
        public Color this[ushort paletteIndex]
        {
            get
            {
                return this.PalToColor(paletteIndex);
            }
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Blend two colors
        /// </summary>
        /*public Rott2DPaletteColor BlendColors(this Rott2DPaletteColor srcColor, Rott2DPaletteColor backColor, double percent)
        {
            Rott2DPaletteColor blendColor = new Rott2DPaletteColor();

            blendColor.r = (byte)((srcColor.r * percent) + backColor.r * (1 - percent));
            blendColor.g = (byte)((srcColor.g * percent) + backColor.g * (1 - percent));
            blendColor.b = (byte)((srcColor.b * percent) + backColor.b * (1 - percent));

            return blendColor;
        }*/


        /// <summary>
        /// Build the colormap
        /// </summary>
        protected override void ProcessLumpData()
        {
            if ((this.GetDataSize() > 0) || (this.GetDataSize() == PALETTE_LUMP_SIZE))
            {
                //build the colormap from palette
                for (int i = 0; i < this._colorMap.Length; i++)
                {
                    
                    //informational:
                    //a RGB value AND (0 to 255) defines it's real color darkness
                    //example: this._colorMap[i].r & 0xFF = full color (255)  = no change
                    //         this._colorMap[i].r & 0x00 = black color (0)   = all black
                    //         this._colorMap[i].r & 0x7F = dark red (127)
                    //
                    //a RGB value OR (0 to 255) defines it's real color brightness
                    //example: this._colorMap[i].r & 0xFF = white color (255) = all white
                    //         this._colorMap[i].r & 0x00 = full color (0)    = no change
                    //         this._colorMap[i].r & 0x7F = bright red (127)


                    //Generate colormap from the loaded palette data lump
                    int indexToProcess = 3 * i; //get 3 bytes for 3 color values (RGB)

                    if ((AlphaColor == DEFAULT_ALPHA_VALUE) || (i == DEFAULT_MASKED_COLOR))
                    {
                        //no darkenss at all on texture, we also skip the masked color!
                        this._colorMap[i].r = this._rawData[indexToProcess];
                        this._colorMap[i].g = this._rawData[indexToProcess + 1];
                        this._colorMap[i].b = this._rawData[indexToProcess + 2];
                        this._colorMap[i].a = DEFAULT_ALPHA_VALUE;
                    }
                    else
                    {
                        /*
                        //old code, just for a reminder in comment
                        Rott2DPaletteColor sourceColor = new Rott2DPaletteColor();
                        sourceColor.r = this._rawData[indexToProcess];
                        sourceColor.g = this._rawData[indexToProcess + 1];
                        sourceColor.b = this._rawData[indexToProcess + 2];

                        Rott2DPaletteColor backColor = new Rott2DPaletteColor();
                        sourceColor.r = this.AlphaColor;
                        sourceColor.g = this.AlphaColor;
                        sourceColor.b = this.AlphaColor;

                        Rott2DPaletteColor blended = this.blendColors(sourceColor, backColor, 100);

                        this._colorMap[i].a = this.AlphaColor;
                        this._colorMap[i].r = blended.r;
                        this._colorMap[i].g = blended.g;
                        this._colorMap[i].b = blended.b;*/

                        //calculate palette with daskness (Alpha channel)
                        this._colorMap[i].a = this.AlphaColor;
                        this._colorMap[i].r = (byte)(this._rawData[indexToProcess] & this._colorMap[i].a);
                        this._colorMap[i].g = (byte)(this._rawData[indexToProcess + 1] & this._colorMap[i].a);
                        this._colorMap[i].b = (byte)(this._rawData[indexToProcess + 2] & this._colorMap[i].a);
                    }
                }

                this.ForceIsReady(); //flag success
            }
        }

        /// <summary>
        /// Get a Color from the palette index in RGBA
        /// </summary>
        private Color PalToColor(ushort index)
        {
            Color rott2dPalColor = new Color();
            
            if ((this.isReady) && (index > 0) && (index < PALETTE_SIZE))
            {
                rott2dPalColor = Color.FromArgb(this._colorMap[index].a, this._colorMap[index].r, this._colorMap[index].g, this._colorMap[index].b);
            }

            return rott2dPalColor;
        }

        /// <summary>
        /// Palette to bitmap
        /// </summary>
        private void ProcessPaletteBitmap()
        {            
            if (this.isReady)
            {
                //init buffer
                if (this._buffer == null)
                {
                    this._buffer = new Bitmap(PALETTE_SIZE, 1, PixelFormat.Format24bppRgb);  //create output image
                }

                //using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                //{
                    //fill palette
                    for (ushort x = 0; x < PALETTE_SIZE; x++)
                    {
                        //write pixels to output bitmap
                        this._buffer.SetPixel(x, 0, this.PalToColor(x));
                        //fastBuffer.SetColor(x, 0, this.palToColor(x));
                    }
                //}
            }
        }

        /// <summary>
        /// Masked color
        /// </summary>
        public Color GetMaskedColor()
        {
            return this.PalToColor(this.MaskColorIndex);
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "pal_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Figure if we have a valid palette
        /// </summary>
        public static bool isPalette(byte[] lumpdata)
        {
            /*
             * Palette is always 768 in bytes.
             * (There are other lumps with the same size, so it's not always true.)
             * 
             */

            bool isPal = false;

            isPal = ((lumpdata.Length > 760) ? ((lumpdata.Length == PALETTE_LUMP_SIZE) ? true : false) : false);

            return isPal;
        }
        #endregion

    }
    #endregion

}
