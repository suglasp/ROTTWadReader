/*
 * ROTT2D
 * Unit: ROTT2D Colormap sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 15-02-2012
 * Last updated : 17-02-2012
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

using ROTT2D.WAD.palette;

using ROTT2D.draw;

namespace ROTT2D.WAD.data
{

    #region Colormap class
    /// <summary>
    /// Colormap class
    /// </summary>
    public sealed class Rott2DColormap : Rott2DTextureLump, IRott2DColormap
    {

        /*
         * Colormaps are used for image shading tables.
         * 
         * In ROTT, i suppose some of the colormaps are used for fogging, darkness and gas mask (green) effects.
         * 
         * ROTT also has several other non-standard colormaps like GRAYMAP and such. These are used for
         * the player colors in COMM-BAT mode (= Multiplayer).
         * 
         * There are many colormaps stored in the standard (Darkwar & Huntbegins) ROTT WAD files.
         * 
         */

        #region Public consts
        /// <summary>
        /// Private consts
        /// </summary>
        public const ushort COLORMAP_DATA_SIZE = 8192; //8192 in size
        public const byte COLORMAP_COUNT = 31;  //0..31 = 32 colormaps
        public const byte COLORMAP_SIZE = 255; //256 colors
        #endregion

        #region Private vars

        byte[,] _colorMaps;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DColormap(ref byte[] colormapLumpData, ref Rott2DPalette palette)
        {
            this.isReady = false;
            this._rawData = colormapLumpData;
            this._palette = palette;
            this._colorMaps = new byte[COLORMAP_COUNT, COLORMAP_SIZE];
            //this._colorMaps = new byte[COLORMAP_SIZE, COLORMAP_COUNT];

            //create buffer from size
            if ((this._buffer == null) && (!this.isReady))
            {
                //this._buffer = new Bitmap(COLORMAP_COUNT, COLORMAP_SIZE, PixelFormat.Format24bppRgb);
                this._buffer = new Bitmap(COLORMAP_SIZE, COLORMAP_COUNT, PixelFormat.Format24bppRgb);
            }

            this.ProcessLumpData(); //generate !
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DColormap(string name, ref byte[] colormapLumpData, ref Rott2DPalette palette)
        {
            this.isReady = false;
            this._rawData = colormapLumpData;
            this._palette = palette;
            this._colorMaps = new byte[COLORMAP_COUNT,COLORMAP_SIZE];
            //this._colorMaps = new byte[COLORMAP_SIZE, COLORMAP_COUNT];
            
            //create buffer from size
            if ((this._buffer == null) && (!this.isReady))
            {
                //this._buffer = new Bitmap(COLORMAP_COUNT, COLORMAP_SIZE, PixelFormat.Format24bppRgb);
                this._buffer = new Bitmap(COLORMAP_SIZE, COLORMAP_COUNT, PixelFormat.Format24bppRgb);
            }

            this.ProcessLumpData(); //generate !
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DColormap()
        {
            this.Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._colorMaps != null)
                {
                    this._colorMaps = null; //cannot dispose, GC needs to collect
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Indexers
        /// <summary>
        /// Get a color map data
        /// </summary>
        public byte[] this[byte index]
        {
            get {
                return this.GetColorMapData(index);
            }
        }

        /*
        /// <summary>
        /// Get a color map bitmap
        /// </summary>
        public Bitmap this[byte index]
        {
            get
            {
                return this.getColorMapBmp(index);
            }
        }*/
        #endregion

        #region Methods
        /// <summary>
        /// Generate the buffer texture
        /// </summary>
        protected override void ProcessLumpData()
        {
            if (this.GetDataSize() == (COLORMAP_DATA_SIZE))
            {
                if ((!this.isReady) && (this._buffer != null) && (this._palette.isReady))
                {                    
                    //build colormap in memory, per color
                    for (ushort colMap = 0; colMap < COLORMAP_COUNT; colMap++)
                    {
                        for (byte i = 0; i < COLORMAP_SIZE; i++)
                        {
                            this._colorMaps[colMap, i] = this._rawData[colMap * 0xFF + i];
                        }
                    }

                    /*
                    for (int i = 0; i < COLORMAP_SIZE; i += 3)
                    {
                        for (ushort colMap = 0; colMap < COLORMAP_COUNT; colMap++)
                        {
                            int indexToProcess = 3 * i; //get 3 bytes for 3 color values (RGB)

                            this._colorMaps[colMap, i] = this._rawData[indexToProcess];
                            this._colorMaps[colMap, i +1] = this._rawData[indexToProcess + 1];
                            this._colorMaps[colMap, i +2] = this._rawData[indexToProcess + 2];
                        }
                        
                    }
                    */

                    //build buffer bitmap (just for output to user)
                    using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                    {
                        for (ushort palIndex = 0; palIndex < COLORMAP_COUNT; palIndex++)
                        {
                            for (ushort colOfs = 0; colOfs < COLORMAP_SIZE; colOfs++)
                            {
                                byte iPaletteColorIndex = this._colorMaps[palIndex, colOfs];      //read a pixel value
                                Color rott2dImgColor = this._palette[iPaletteColorIndex];     //convert to Color
                                fastBuffer.SetColor(colOfs, palIndex, rott2dImgColor);
                            }
                        }
                    }

                    //flag ready
                    this.ForceIsReady();
                }
            }
        }

        /// <summary>
        /// return the selected colormap data
        /// </summary>
        private byte[] GetColorMapData(byte index)
        {            
            byte[] arrColorMap = new byte[COLORMAP_SIZE];

            if ((this.isReady) && (this._colorMaps != null))
            {
                if (this._colorMaps.Length > 0)
                {
                    for (byte i = 0; i < COLORMAP_SIZE; i++)
                    {
                        arrColorMap[i] = this._colorMaps[index, i];
                    }
                } //length check
            } //is ready

            return arrColorMap;
        }

        /// <summary>
        /// return the selected colormap data
        /// </summary>
        public Bitmap GetColorMapBmp(byte palIndex)
        {
            Bitmap colormapBmp = new Bitmap(COLORMAP_SIZE, 1, PixelFormat.Format24bppRgb);

            if ((this.isReady) && (this._colorMaps != null))
            {
                if (this._colorMaps.Length > 0)
                {
                    using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                    {                        
                        for (ushort colOfs = 0; colOfs < COLORMAP_SIZE; colOfs++)
                        {
                            byte iPaletteColorIndex = this._colorMaps[palIndex, colOfs];      //read a pixel value
                            Color rott2dImgColor = this._palette[iPaletteColorIndex];     //convert to Color
                            fastBuffer.SetColor(colOfs, 0, rott2dImgColor);
                        }   
                    }  //using fastBuffer
                } //length check
            } //is ready

            return colormapBmp;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "colormap_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Figure if we have a Colormap
        /// </summary>
        public static bool isColormap(byte[] lumpdata)
        {

            /*
             * Colormaps are always 8192 bytes large
             * And containt 32 maps of shades (dividable by 256).
             * 
             */

            bool isColormap = false;

            isColormap = ((lumpdata.Length == COLORMAP_DATA_SIZE) ? ((lumpdata.Length % 256 == 0) ? true : false) : false);

            return isColormap;
        }
        #endregion

    }
    #endregion

}
