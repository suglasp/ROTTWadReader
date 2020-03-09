/*
 * ROTT2D
 * Unit: ROTT2D Textured lump abstract Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 13-01-2012
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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using ROTT2D.draw;
using ROTT2D.WAD.palette;

namespace ROTT2D.WAD.data
{

    #region Texture (GFX) Lump basic class
    /// <summary>
    /// Rott2D class for Textured lump's
    /// </summary>
    public abstract class Rott2DTextureLump : Rott2DLump, IDisposable
    {

        /*
         * Base class for textures in ROTT2D.
         * 
         * Some textures are stored in (MS-Dos) Modex compatibility mode.
         * 
         * Modex was a Dos graphics library written in ASM (= Assembly, Low level machine code)
         * to render 2D and 3D images in backbuffer of a VGA/VESA localbus videocard.
         * 
         * For example, the Flat data type uses the Modex compatibility mode.
         * This means all pixels are stored in Y to X loops, instead of X to Y.
         * for(int Y; ...)
         *    for(int X; ...)
         *       setPixel(x, y, Bitmap);
         * 
         * In the end, we need to rotate the bitmap 90 degrees.
         * 
         * Again, not all textures are in this format.
         * 
         */

        #region Private Vars
        private bool _disposed = false;
        #endregion

        #region Protected vars
        /// <summary>
        /// Protected vars
        /// </summary>
        protected Bitmap _buffer = null;          //backbuffer bitmap image
        protected Rott2DPalette _palette = null;  //256 color palette
        #endregion

        #region Destructors
        /// <summary>
        /// Dispose memory
        /// </summary>
        protected override void Dispose(bool disposing) 
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Unload();
                }

                this._disposed = true;

                base.Dispose(disposing);
            }
        }
        #endregion

        #region Getters & setters
        /// <summary>
        /// Real width
        /// </summary>
        public int TextureWidth
        {
            get {
                /*if (this._buffer != null)
                {
                    return this._buffer.Width;
                }
                else
                    return 0;
                */

                return (this._buffer != null) ? this._buffer.Width : 0;
            }
        }


        /// <summary>
        /// Real height
        /// </summary>
        public int TextureHeight
        {
            get
            {
                /*if (this._buffer != null)
                {
                    return this._buffer.Height;
                }
                else
                    return 0;
                */

                return (this._buffer != null) ? this._buffer.Height : 0;
            }
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Unload the buffer
        /// </summary>
        protected virtual void Unload()
        {
            //release backbuffer
            if (this._buffer != null)
            {
                this._buffer.Dispose();
                this._buffer = null;
            }

            //release color palette
            if (this._palette != null)
            {
                this._palette.Dispose();
                this._palette = null;
            }
        }

        /// <summary>
        /// Rotate buffer 90 deg (for Modex compatibility)
        /// </summary>
        protected void RotateBuffer()
        {
            //rotate bitmap 90 degrees
            if (this._buffer != null)
            {
                this._buffer.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            }
        }

        /// <summary>
        /// Fill whole texture buffer with a "clear" color
        /// (Handy to pre-set a transparency color.)
        /// </summary>
        public void FillBuffer(ref Color color)
        {
            if (this._buffer != null)
            {
                using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                {
                    //set all pixels to transparent color (index 255)
                    for (int y = 0; y < this.TextureHeight; y++)
                    {
                        for (int x = 0; x < this.TextureWidth; x++)
                        {
                            fastBuffer.SetColor(x, y, color);
                            //this._buffer.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// clear buffer (parameter by value!)
        /// </summary>
        public void FillBuffer(int width, int height, ref Color color)
        {
            if (this._buffer != null)
            {
                using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                {
                    //set all pixels to transparent color (index 255)
                    for (int y = 0; y < width; y++)
                    {
                        for (int x = 0; x < height; x++)
                        {
                            fastBuffer.SetColor(x, y, color);
                            //this._buffer.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the buffer Texture
        /// </summary>
        public virtual Bitmap GetTexture()
        {
            if ((this.isReady) && (this._buffer != null))
            {
                return this._buffer;
            }
            else
            {
                return null;
            }
        }

        /*
        /// <summary>
        /// Real width
        /// </summary>
        public int GetTextureWidth()
        {
            int width = 0;

            if (this._buffer != null)
                width = this._buffer.Width;

            return width;
        }

        /// <summary>
        /// Real height
        /// </summary>
        public int GetTextureHeight()
        {
            int height = 0;

            if (this._buffer != null)
                height = this._buffer.Height;

            return height;
        }
         */

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "gfxlump_t";
        }
        #endregion
        
        #region Abstract Methods
        /// <summary>
        /// Define an abstract "processLumpData" method
        /// This method will "process" the lump data to a workable image.
        /// </summary>
        protected abstract void ProcessLumpData();

        #endregion

    }
    #endregion

}
