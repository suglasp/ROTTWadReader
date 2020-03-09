/*
 * ROTT2D
 * Unit: ROTT2D Flat sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 15-02-2012
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

    #region raw class (wall & sky textures)
    public abstract class Rott2DRaw : Rott2DTextureLump, IRott2DRaw
    {
        /*
         * ROTT Raw textures data format.
         * This format is used to store Sky and Wall textures in a WAD.
         * 
         */

        #region Public consts
        private const int RAW_MINIMAL_DATA_SIZE = 4096;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        protected Rott2DRaw(ushort width, ushort height, ref byte[] rawLumpData, ref Rott2DPalette palette)
        {
            this.isReady = false;
            this._rawData = rawLumpData;
            this._palette = palette;

            if (this._buffer == null)
            {
                this._buffer = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            }

            this.ProcessLumpData(); //generate !
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected Rott2DRaw(string name, ushort width, ushort height, ref byte[] rawLumpData, ref Rott2DPalette palette)
        {
            this.isReady = false;
            this.Name = name;
            this._rawData = rawLumpData;
            this._palette = palette;

            if (this._buffer == null)
                this._buffer = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            this.ProcessLumpData(); //generate !
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DRaw()
        {
            base.Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generate the buffer texture
        /// </summary>
        protected override void ProcessLumpData()
        {
            if (this.GetDataSize() >= RAW_MINIMAL_DATA_SIZE)
            {
                 if ((this._buffer != null) && (this._palette.isReady))
                 {
                     using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                     {
                         int iLumpIndex = 0;          //index for lump data array to byte (wall 0-4096, sky 0-512000)
                         ushort iPaletteColorIndex = 0;  //return value for lump palette color index (0-256)

                         //generate pixels in bitmap, starting with Y value (for ROTT's "modex" compatibility mode)
                         for (int y = 0; y < this.TextureHeight; y++)
                         {
                             for (int x = 0; x < this.TextureWidth; x++)
                             {
                                 //convert lump data to Palette Index values
                                 iPaletteColorIndex = this._rawData[iLumpIndex++];

                                 //get Palette Index and create color value
                                 Color rott2dImgColor = this._palette[iPaletteColorIndex];
                                 //rott2dImgColor = Color.FromArgb(colormap[iPaletteColorIndex].a, colormap[iPaletteColorIndex].r, colormap[iPaletteColorIndex].g, colormap[iPaletteColorIndex].b);

                                 //draw color pixels to bitmap texture
                                 fastBuffer.SetColor(x, y, rott2dImgColor);
                                 //this._buffer.SetPixel(x, y, rott2dImgColor);
                             }
                         }
                     }

                    //rotate bitmap 90 degrees
                    this.RotateBuffer();

                    //flag ready
                    this.ForceIsReady();
                 }
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "raw_t";
        }
        #endregion

    }
    #endregion

}
