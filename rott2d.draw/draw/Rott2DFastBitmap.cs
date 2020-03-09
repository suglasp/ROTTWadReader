/*
 * ROTT2D
 * Unit: ROTT2D unsafe fast bitmap Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 08-02-2012
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
using System.Drawing;
using System.Drawing.Imaging;

//using ROTT2D.wad.palette;

/* 
 * Internet reference info:
 * 
 * The BitmapData class contains the following properties:
 *   Scan0 The address in memory of the fixed data array
 *   Stride The width, in bytes, of a single row of pixel data. This width is a multiple, or possiblysub-multiple, of the pixel dimensions of the image and may be padded out to include a few more bytes. I'll explain why shortly.
 *   PixelFormat The actual pixel format of the data. This is important for finding the right bytes
 *   Width The width of the locked image
 *   Height The height of the locked image 
 * 
 *   Finding the right byte offset:
 *   Format32BppArgb   Given X and Y coordinates,  the address of the first element in the pixel is Scan0+(y * stride)+(x*4). This Points to the blue byte. The following three bytes contain the green, red and alpha bytes.
 *   Format24BppRgb    Given X and Y coordinates, the address of the first element in the pixel is Scan0+(y*Stride)+(x*3). This points to the blue byte which is followed by the green and the red.
 *   Format8BppIndexed Given the X and Y coordinates the address of the byte is Scan0+(y*Stride)+x. This byte is the index into the image palette.
 *   Format4BppIndexed Given X and Y coordinates the byte containing the pixel data is calculated as Scan0+(y*Stride)+(x/2). The corresponding byte contains two pixels, the upper nibble is the leftmost and the lower nibble is the rightmost of two pixels. The four bits of the upper and lower nibble are used to select the colour from the 16 colour palette.
 *   Format1BppIndexed Given the X and Y coordinates, the byte containing the pixel is calculated by Scan0+(y*Stride)+(x/8). The byte contains 8 bits, each bit is one pixel with the leftmost pixel in bit 8 and the rightmost pixel in bit 0. The bits select from the two entry colour palette. 
 *  
 */

namespace ROTT2D.draw
{

    /* compile with "unsafe" in Visual Studio */

    #region Rott2D Fast drawing structures
    /// <summary>
    /// Rott2DPixelColor
    /// </summary>
    public struct Rott2DPixelColor
    {
        public byte B;
        public byte G;
        public byte R;
        //public byte A;
    }
    #endregion

    #region Rott2DFastBitmap
    /// <summary>
    /// Fast bitmap access
    /// </summary>
    public unsafe class Rott2DFastBitmap : IRott2DFastBitmap, IDisposable
    {

        /*
         * Class using pointers to fast read/write per pixel rendering to a Bitmap.
         * 
         */

        #region Private vars
        /// <summary>
        /// Private vars
        /// </summary>
        private Bitmap _bitmap;
        private int _width;
        private BitmapData _bitmapData = null;
        private byte* _pBase = null;
        private Rott2DPixelColor* _pInitPixel = null;
        private Point _size;
        private bool _locked = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DFastBitmap(Bitmap bmp)
        {
            if (bmp == null) throw new ArgumentNullException("bitmap");

            _bitmap = bmp;
            _size = new Point(bmp.Width, bmp.Height);

            LockBitmap();
        }
        #endregion

        #region Destructors
        /// <summary>
        /// Destructors
        /// </summary>
        ~Rott2DFastBitmap()
        {
            this.Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_locked) UnlockBitmap();
        }
        #endregion

        #region Indexers
        /// <summary>
        /// Indexer (readonly)
        /// </summary>
        public Rott2DPixelColor* this[int x, int y]
        {
            get { return (Rott2DPixelColor*)(_pBase + y * _width + x * sizeof(Rott2DPixelColor)); }
        }
        #endregion

        #region Getters & Setters
        /// <summary>
        /// Check if locked (readonly)
        /// </summary>
        public bool isLocked
        {
            get { return this._locked; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Lock bitmap
        /// </summary>
        private void LockBitmap()
        {
            if (_locked) throw new InvalidOperationException("Already locked");

            Rectangle bounds = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);

            // Figure out the number of bytes in a row. This is rounded up to be a multiple 
            // of 4 bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            _width = bounds.Width * sizeof(Rott2DPixelColor);
            if (_width % 4 != 0) _width = 4 * (_width / 4 + 1);

            _bitmapData = _bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            _pBase = (byte*)_bitmapData.Scan0.ToPointer();
            _locked = true;
        }

        /// <summary>
        /// Unlock bitmap
        /// </summary>
        private void UnlockBitmap()
        {
            if (!_locked) throw new InvalidOperationException("Not currently locked");

            _bitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
            _locked = false;
        }

        /// <summary>
        /// Set Pointer 
        /// </summary>
        private void InitCurrentPixel()
        {
            _pInitPixel = (Rott2DPixelColor*)_pBase;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Row size
        /// </summary>
        public Rott2DPixelColor* GetInitialPixelForRow(int rowNumber)
        {
            return (Rott2DPixelColor*)(_pBase + rowNumber * _width);
        }

        /// <summary>
        /// Get color from pixel pos
        /// </summary>
        public Color GetColor(int x, int y)
        {
            Rott2DPixelColor* data = this[x, y];
            return Color.FromArgb(data->R, data->G, data->B);
            //return Color.FromArgb(data->A, data->R, data->G, data->B);
        }

        /// <summary>
        /// Set pixel color on pos
        /// </summary>
        public void SetColor(int x, int y, Color color)
        {
            Rott2DPixelColor* data = this[x, y];
            data->R = color.R;
            data->G = color.G;
            data->B = color.B;
            //data->A = color.A;
        }

        /// <summary>
        /// Fill image with one color
        /// </summary>
        public void SetFill(Color color)
        {
            for (int x = 0; x < this._bitmap.Width; x++)
                for (int y = 0; y < this._bitmap.Height; y++)
                {
                    Rott2DPixelColor* data = this[x, y];
                    data->R = color.R;
                    data->G = color.G;
                    data->B = color.B;
                    //data->A = color.A;
                }
        }
        #endregion

    }
    #endregion

}
