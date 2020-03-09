/*
 * ROTT2D
 * Unit: ROTT2D Trans Masked sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date:   15-01-2012
 * Updated on data: 17-02-2012 
 * 
 * 
 * On 06-02-2012: Found a bug myself in my own code i was looking for for days
 *                but i also want to thank Birger N. A. of the WinRott port project for sharing code.
 * 
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

    #region TransMasked structs
    /// <summary>
    /// ROTT WAD Trans Masked structure (= Transparent textures)
    /// </summary>
    public struct Rott2DTransMaskedHeader
    {
        /*
         * Each property in the Trans Masked header is a short value = 2 bytes (16bit)
         * 12 bytes header ==> 5 times a short value
         * 
         * _collumnOfs is an array that contains the pointer offset positions of 
         * each column. The first index [0] is the same as _width.
         * 
         * This format is very nearly the same as Doom's picture format when it was
         * in Beta development status. For more info on this transmasked format,
         * see description in the class Rott2DTransMaskedPost.
         * 
         */

        /// <summary>
        /// Private consts
        /// </summary>
        public const byte TRANSMASKED_HEADER_SIZE = 12; //transmasked header is 12 bytes in size

        /// <summary>
        /// Private vars
        /// </summary>
        private ushort _origSize;    // the orig size of "grabbed" gfx
        private ushort _width;       // bounding box size
        private ushort _height;
        private short _leftOffset;  // pixels to the left of origin
        private short _topOffset;   // pixels above the origin
        private ushort _translevel;       // transparency level
        private ushort[] _collumnOfs;  // only [_width] used, the [0] is &collumnofs[_width]

        /// <summary>
        /// Indexer that returns or sets a pointer value in the CollumnOfs
        /// </summary>
        public ushort this[int colIndex]
        {
            get
            {
                return this.CollumnOfs[colIndex];
            }

            set
            {
                if (this.CreateColumnArray())
                {
                    this.CollumnOfs[colIndex] = value;
                }
            }
        }

        /// <summary>
        /// Original Size, should be the total of Width x Height
        /// </summary>
        public ushort OrigSize
        {
            get { return this._origSize; }
            set { this._origSize = value; }
        }

        /// <summary>
        /// Width
        /// </summary>
        public ushort Width
        {
            get { return this._width; }
            set { this._width = value; }
        }

        /// <summary>
        /// Height
        /// </summary>
        public ushort Height
        {
            get { return this._height; }
            set { this._height = value; }
        }

        /// <summary>
        /// LeftOffset
        /// </summary>
        public short LeftOffset
        {
            get { return this._leftOffset; }
            set { this._leftOffset = value; }
        }

        /// <summary>
        /// TopOffset
        /// </summary>
        public short TopOffset
        {
            get { return this._topOffset; }
            set { this._topOffset = value; }
        }

        /// <summary>
        /// TransLevel
        /// </summary>
        public ushort TransLevel
        {
            get { return this._translevel; }
            set { this._translevel = value; }
        }

        /// <summary>
        /// CollumnOfs
        /// </summary>
        public ushort[] CollumnOfs
        {
            get { return this._collumnOfs; }
            set {
                if (CreateColumnArray())
                {
                    this._collumnOfs = value;
                }
            }
        }

        /// <summary>
        /// Create the _collumnOfs array
        /// </summary>
        private bool CreateColumnArray()
        {
            bool created = false;

            if ((this._collumnOfs == null) && (this.Width > 0))
            {
                this._collumnOfs = new ushort[this.Width];
                created = true;
            }
            else {
                created = true;
            }


            return created;
        }
    }

    /// <summary>
    /// ROTT texture image post data
    /// </summary>
    public struct Rott2DTransMaskedPost
    {
        /*
         * A Masked texture contains columns.
         * Reach row containts area's where no pixels are drawn,
         * and some where there are pixels.
         * 
         * A post is a number of pixels "down the row" that follow
         * eachother. One row can contain several posts.
         * 
         * A full row ends with 255 (0xFF).
         * 
         */

        /// <summary>
        /// Public consts
        /// </summary>
        public const byte TRANSMASKED_POST_TERMINATOR  = 0xFF;  //row ending (255)
        public const byte TRANSMASKED_POST_SPECIAL     = 0xFE;  //special post (254)

        /// <summary>
        /// Private vars
        /// </summary>
        private int _offset;
        private byte _nPixels;

        /// <summary>
        /// Post data offset position
        /// </summary>
        public int Offset
        {
            get { return this._offset; }
            set { this._offset = value; }
        }

        /// <summary>
        /// Number of pixels in a post
        /// </summary>
        public byte NumberOfPixels
        {
            get { return this._nPixels; }
            set { this._nPixels = value; }
        }

        /// <summary>
        /// Method that returns previous value
        /// </summary>
        public int GetPrevOffset()
        {
            this.Offset--;
            return this.Offset;
        }

        /// <summary>
        /// Method that returns next value
        /// </summary>
        public int GetNextOffset()
        {
            this.Offset++;
            return this.Offset;
        }
    }
    #endregion

    #region TransMasked class
    public sealed class Rott2DTransMasked : Rott2DTextureLump, IRott2DTransMasked
    {

        /*
         * Transmasked textures
         * 
         * These textures are the same as masked textures, but contain extra data posts
         * with colorshading values. ROTT's engine needed this extra data for use with glass textures,
         * where the glass needed to be blended with the background (behind the glass) from player perspective.
         * 
         * For ROTT2D we just read both kind of posts, but skip over the colorshading.
         * 
         */

        #region Public Consts
        /// <summary>
        /// Public consts
        /// </summary>
        public const ushort TRANSMASKED_MIN_TEXTURE_BOUNDS = 3;     //smallest texture size supported
        public const ushort TRANSMASKED_MAX_TEXTURE_BOUNDS = 2048;  //largest texture size supported
        #endregion

        #region Private Vars
        /// <summary>
        /// Private vars
        /// </summary>
        private bool _headerReady = false; //header is ready
        private Rott2DTransMaskedHeader _transMaskedHeader; //patch header
        private int _dataSize = 0;  //image data size, without the header
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DTransMasked(ref byte[] transmaskedLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this._rawData = transmaskedLumpData;
            this._palette = palette;
            this._dataSize = this.GetDataSize() - Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE;

            //process header data first
            this._transMaskedHeader = new Rott2DTransMaskedHeader();
            this.ProcessTransMaskedHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
            {
                this._buffer = new Bitmap(this._transMaskedHeader.Width, this._transMaskedHeader.Height, PixelFormat.Format24bppRgb);
            }

            //generate buffer bitmap
            this.ProcessLumpData();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DTransMasked(string name, ref byte[] transmaskedLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this.Name = name;
            this._rawData = transmaskedLumpData;
            this._palette = palette;
            this._dataSize = this.GetDataSize() - Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE;

            //process header data first
            this._transMaskedHeader = new Rott2DTransMaskedHeader();
            this.ProcessTransMaskedHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
                this._buffer = new Bitmap(this._transMaskedHeader.Width, this._transMaskedHeader.Height, PixelFormat.Format24bppRgb);

            //generate buffer bitmap
            this.ProcessLumpData();
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DTransMasked()
        {
            base.Dispose();
        }
        #endregion

        #region Getters & Setters
        public bool isHeaderReady
        {
            get { return this._headerReady; }
            private set { this._headerReady = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Read header data fields
        /// </summary>
        private void ProcessTransMaskedHeader()
        {
            if (this.GetDataSize() > Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE)
            {
                //read masked header
                this._transMaskedHeader.OrigSize = BitConverter.ToUInt16(this._rawData, 0);      //read original size
                this._transMaskedHeader.Width = BitConverter.ToUInt16(this._rawData, 2);         //read width
                this._transMaskedHeader.Height = BitConverter.ToUInt16(this._rawData, 4);        //read height
                this._transMaskedHeader.LeftOffset = BitConverter.ToInt16(this._rawData, 8);    //read left offset
                this._transMaskedHeader.TopOffset = BitConverter.ToInt16(this._rawData, 10);    //read top offset
                this._transMaskedHeader.TransLevel = BitConverter.ToUInt16(this._rawData, 12);   //read trans level
                
                //flag header ready
                this.isHeaderReady = true;
            }
        }

        /// <summary>
        /// Generate the buffer texture
        /// </summary>
        protected override void ProcessLumpData()
        {
            if (this.GetDataSize() == (this._dataSize + Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE))
            {
                 if ((this.isHeaderReady) && (this._buffer != null) && (this._palette.isReady))
                 {
                     //generate pixels to bitmap
                     using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                     {
                         ushort iPaletteColorIndex = 0;       //return value for lump palette color index (0-256)

                         //read column offset pointers
                         int pos = Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE;

                         for (int col = 0; col < this._transMaskedHeader.Width; col++)
                         {
                             this._transMaskedHeader[col] = BitConverter.ToUInt16(this._rawData, pos); //store pointer position in colofs
                             pos += sizeof(ushort); //move two bytes further  
                         }

                         //first, fill buffer with default transparency color from palette 
                         //Color textureTransparencyColor = this._palette[Rott2DPalette.PALETTE_TRANSPARENT_COLOR];
                         Color textureTransparencyColor = this._palette.GetMaskedColor();
                         fastBuffer.SetFill(textureTransparencyColor);
                         //this.fillBuffer(this._transMaskedHeader.Width, this._transMaskedHeader.Height, textureTransparencyColor);

                         //generate pixels in bitmap
                         for (ushort iXpos = 0; iXpos < this._transMaskedHeader.Width; iXpos++)
                         {
                             Rott2DTransMaskedPost postData = new Rott2DTransMaskedPost(); //create a post header data
                             postData.Offset = this._transMaskedHeader[iXpos]; //get post data Offset

                             ushort iYpos = (ushort)this._rawData[postData.Offset]; //read first position in row
                             byte transbyte = 0;  //this is our "special" post pre-reading byte value

                             while (iYpos != Rott2DTransMaskedPost.TRANSMASKED_POST_TERMINATOR)  //continue until row is terminated
                             {
                                 postData.NumberOfPixels = this._rawData[postData.GetNextOffset()]; //read number of pixels in post

                                 if (iYpos + postData.NumberOfPixels > this._transMaskedHeader.Height)
                                 {
                                     break;  //skip is we reached Height of bitmap!                                 
                                 }

                                 //read first post byte if number of pixels is larger then zero
                                 //we need to do this check, or we go out of limits of the stream (SOLVED 06-02-2012).
                                 if (postData.NumberOfPixels > 0)
                                 {
                                     transbyte = this._rawData[postData.GetNextOffset()];
                                 }

                                 //optimized routine for speed (less if-then-else checking)
                                 Color rott2dImgColor = this._palette.GetMaskedColor(); //default masked color from palette

                                 //get byte value, draw correct color if not transparent                            
                                 if (transbyte != Rott2DTransMaskedPost.TRANSMASKED_POST_SPECIAL)
                                 {
                                     //we have a regular post (such as in a masked texture), draw it!
                                     for (byte iPost = 0; iPost < postData.NumberOfPixels; iPost++)
                                     {
                                         //transbyte pre-reads a value that can be 254 (= TRANSMASKED_POST_SPECIAL).
                                         //this is the same as the first post byte value.
                                         //in other words: if iPost is zero, copy the transbyte value.
                                         iPaletteColorIndex = (iPost == 0) ? transbyte : this._rawData[postData.GetNextOffset()];

                                         rott2dImgColor = this._palette[iPaletteColorIndex];  //convert to Color

                                         fastBuffer.SetColor(iXpos, iYpos + iPost, rott2dImgColor);
                                         //this._buffer.SetPixel(iXpos, iYpos + iPost, rott2dImgColor); //draw
                                     }
                                 }
                                 else
                                 {
                                     /* special post,
                                      * draw cracked/remaining glass pieces on the edges of the transmasked pictures
                                      *
                                      * notes:
                                      * in ROTT this is calculated with an ALPHA value for transparency
                                      * most of this can be found in rt_scale.c
                                      * int alpha = (((translevel+64)>>2)<<8)
                                      * 
                                      * in WinRottGL this is calculated as (thanks to Birger):
                                      * int mask = (63 - translevel) << (ALPHA_SHIFT+2)
                                      * 
                                      */

                                     for (byte iPost = 0; iPost < postData.NumberOfPixels; iPost++)
                                     {
                                         iPaletteColorIndex = 32; //set dark value
                                         rott2dImgColor = this._palette[iPaletteColorIndex];  //convert to Color
                                         fastBuffer.SetColor(iXpos, iYpos + iPost, rott2dImgColor);
                                         //this._buffer.SetPixel(iXpos, iYpos + iPost, rott2dImgColor); //draw
                                     }
                                 }

                                 //finally, goto next row offset
                                 iYpos = this._rawData[postData.GetNextOffset()];
                             }

                         }

                     }
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
            return "transmask_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Try to figure out if this is a transmasked texture
        /// </summary>
        public static bool isTransMaskedTexture(byte[] lumpdata)
        {
            /* 
             * masked & transmasked have the same first 6 bytes (= 3x short) in common:
             *   origsize
             *   width
             *   height
             *  
             * This is a format Doom, Strife or Heretic don't know about. Only ROTT does!
             * 
             */

            bool trmaskedTexture = false;

            if (lumpdata.Length > Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE)
            {
                Rott2DTransMaskedHeader header = new Rott2DTransMaskedHeader();
                header.Width = BitConverter.ToUInt16(lumpdata, 2);        //width
                header.Height = BitConverter.ToUInt16(lumpdata, 4);       //height
                header.TransLevel = BitConverter.ToUInt16(lumpdata, 10);  //read trans level

                if ((header.Width >= TRANSMASKED_MIN_TEXTURE_BOUNDS) && (header.Width < TRANSMASKED_MAX_TEXTURE_BOUNDS))
                {
                    if ((header.Height >= TRANSMASKED_MIN_TEXTURE_BOUNDS) && (header.Height < TRANSMASKED_MAX_TEXTURE_BOUNDS))
                    {
                        if ((header.TransLevel == 21) || (header.TransLevel == 34))  //check to be sure this is a transmasked.                       
                        {                            
                            //we have proably a transmasked
                            trmaskedTexture = true;
                         
                            //extra sanity check on the columns data
                            if (lumpdata.Length < (Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE + (header.Width * sizeof(ushort))))
                                trmaskedTexture = false;

                            for (int c = 0; c < header.Width; c++)
                            {
                                if ((lumpdata[Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE + c] > lumpdata.Length) ||
                                    (lumpdata[Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE + c] < ((header.Width) + Rott2DTransMaskedHeader.TRANSMASKED_HEADER_SIZE)))
                                { trmaskedTexture = true; }
                                
                            }

                        }
                    }
                }
                
            }

            return trmaskedTexture;
        }
        #endregion

    }
    #endregion

}
