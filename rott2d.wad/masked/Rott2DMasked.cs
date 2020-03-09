/*
 * ROTT2D
 * Unit: ROTT2D Masked sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date:   11-01-2012
 * Updated on data: 17-02-2012
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

    #region Masked structs
    /// <summary>
    /// ROTT WAD Masked structure (= Transparent textures)
    /// </summary>
    public struct Rott2DMaskedHeader
    {
        /*
         * Each property in the Masked header is a short value = 2 bytes (16bit)
         * 10 bytes header ==> 5 times a short value
         * 
         * _collumnOfs is an array that contains the pointer offset positions of 
         * each column. The first index [0] is the same as _width.
         * 
         * This format is very nearly the same as Doom's picture format when it was
         * in Beta development status.
         * 
         */

        /// <summary>
        /// Public consts
        /// </summary>
        public const byte MASKED_HEADER_SIZE = 10; //masked header is 10 bytes in size

        /// <summary>
        /// Private vars
        /// </summary>
        private ushort _origSize;    // the orig size of "grabbed" gfx
        private ushort _width;       // bounding box size
        private ushort _height;
        private short _leftOffset;  // pixels to the left of origin
        private short _topOffset;   // pixels above the origin
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
    public struct Rott2DMaskedPost
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
        public const byte MASKED_POST_TERMINATOR = 0xFF;  //row ending

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
        /// Mthod that returns next value
        /// </summary>
        public int getNextOffset()
        {
            this.Offset++;

            return this.Offset;
        }
    }
    #endregion

    #region Masked class
    public sealed class Rott2DMasked : Rott2DTextureLump, IRott2DMasked
    {

        /*
         * Trans textures.
         * Mostly used for ingame actor and weapon textures.
         * 
         */

        #region Public consts
        /// <summary>
        /// Private consts
        /// </summary>
        public const ushort MASKED_MIN_TEXTURE_BOUNDS = 3;     //smallest supported texture size
        public const ushort MASKED_MAX_TEXTURE_BOUNDS = 2048;  //largest supported texture size
        #endregion

        #region Private Vars
        /// <summary>
        /// Private vars
        /// </summary>
        private bool _headerReady = false; //header is ready
        private Rott2DMaskedHeader _maskedHeader; //patch header
        private int _dataSize = 0;  //image data size, without the header
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DMasked(ref byte[] maskedLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this._rawData = maskedLumpData;
            this._palette = palette;
            this._dataSize = this.GetDataSize() - Rott2DMaskedHeader.MASKED_HEADER_SIZE;

            //process header data first
            this._maskedHeader = new Rott2DMaskedHeader();
            this.ProcessMaskedHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
            {
                this._buffer = new Bitmap(this._maskedHeader.Width, this._maskedHeader.Height, PixelFormat.Format24bppRgb);
            }

            //generate buffer bitmap
            this.ProcessLumpData();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DMasked(string name, ref byte[] maskedLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this.Name = name;
            this._rawData = maskedLumpData;
            this._palette = palette;
            this._dataSize = this.GetDataSize() - Rott2DMaskedHeader.MASKED_HEADER_SIZE;

            //process header data first
            this._maskedHeader = new Rott2DMaskedHeader();
            this.ProcessMaskedHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
            {
                this._buffer = new Bitmap(this._maskedHeader.Width, this._maskedHeader.Height, PixelFormat.Format24bppRgb);
            }

            //generate buffer bitmap
            this.ProcessLumpData();
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DMasked()
        {
            base.Dispose();
        }
        #endregion

        #region Getters & Setters
        /// <summary>
        /// Mark if header is ready (in cache)
        /// </summary>
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
        private void ProcessMaskedHeader()
        {
            if (this.GetDataSize() > Rott2DMaskedHeader.MASKED_HEADER_SIZE)
            {
                //read masked header
                this._maskedHeader.OrigSize = BitConverter.ToUInt16(this._rawData, 0);     //read original size
                this._maskedHeader.Width = BitConverter.ToUInt16(this._rawData, 2);        //read width
                this._maskedHeader.Height = BitConverter.ToUInt16(this._rawData, 4);       //read height
                this._maskedHeader.LeftOffset = BitConverter.ToInt16(this._rawData, 8);    //read left offset
                this._maskedHeader.TopOffset = BitConverter.ToInt16(this._rawData, 10);    //read top offset

                //this._maskedHeader.CollumnOfs = new ushort[this._maskedHeader.Width];

                this.isHeaderReady = true;
            }
        }

        /// <summary>
        /// Generate the buffer texture
        /// </summary>
        protected override void ProcessLumpData()
        {
            if (this.GetDataSize() == (this._dataSize + Rott2DMaskedHeader.MASKED_HEADER_SIZE))
            {
                if ((this.isHeaderReady) && (this._buffer != null) && (this._palette.isReady))
                {
                    using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                    {
                        //generate pixels to bitmap
                        ushort iPaletteColorIndex = 0;       //return value for lump palette color index (0-256)

                        //read column offset pointers
                        int pos = Rott2DMaskedHeader.MASKED_HEADER_SIZE;

                        for (int col = 0; col < this._maskedHeader.Width; col++)
                        {
                            this._maskedHeader[col] = BitConverter.ToUInt16(this._rawData, pos); //store pointer position in colofs
                            pos += sizeof(ushort); //move two bytes further  
                        }

                        //first, fill buffer with default transparency color from palette 
                        Color textureTransparencyColor = this._palette.GetMaskedColor();
                        fastBuffer.SetFill(textureTransparencyColor);
                        //this.fillBuffer(ref textureTransparencyColor);
 
                        //generate pixels in bitmap
                        for (ushort iXpos = 0; iXpos < this._maskedHeader.Width; iXpos++)
                        {
                            Rott2DMaskedPost postData = new Rott2DMaskedPost(); //create post data

                            postData.Offset = this._maskedHeader[iXpos]; //get post data Offset

                            ushort iYpos = this._rawData[postData.Offset]; //read first position in row

                            while (iYpos != Rott2DMaskedPost.MASKED_POST_TERMINATOR)  //continue until row is terminated
                            {
                                postData.NumberOfPixels = this._rawData[postData.getNextOffset()];  //read number of pixels in post

                                if (iYpos + postData.NumberOfPixels > this._maskedHeader.Height)
                                    break;  //skip is we reached Height of bitmap!

                                //read a full post (= row of pixels to be drawn)
                                for (byte iPost = 0; iPost < postData.NumberOfPixels; iPost++)
                                {
                                    iPaletteColorIndex = this._rawData[postData.getNextOffset()]; //read a pixel value

                                    Color rott2dImgColor = this._palette[iPaletteColorIndex];     //convert to Color

                                    fastBuffer.SetColor(iXpos, iYpos + iPost, rott2dImgColor);
                                    //this._buffer.SetPixel(iXpos, iYpos + iPost, rott2dImgColor);  //draw
                                }

                                iYpos = this._rawData[postData.getNextOffset()]; //goto next row offset
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
            return "mask_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Try to figure out if this is a masked texture
        /// </summary>
        public static bool isMaskedTexture(byte[] lumpdata)
        {
            /*
             * masked & transmasked have the same first 6 bytes (= 3x short) in common:
             *   origsize
             *   width
             *   height
             * 
             * Masked textures are very alike to the Doom masked textures.
             * Only the two extra bits (before and after each post) are not present in ROTT Masked textures.
             * Doom used these two extra bits for, non x86, NExT machines.
             * 
             */

            bool maskedTexture = false;

            if (lumpdata.Length > Rott2DMaskedHeader.MASKED_HEADER_SIZE)
            {
                Rott2DMaskedHeader header = new Rott2DMaskedHeader();
                header.Width = BitConverter.ToUInt16(lumpdata, 2);        //width
                header.Height = BitConverter.ToUInt16(lumpdata, 4);       //height

                
                if ((header.Width >= MASKED_MIN_TEXTURE_BOUNDS) && (header.Width < MASKED_MAX_TEXTURE_BOUNDS))
                {
                    if ((header.Height >= MASKED_MIN_TEXTURE_BOUNDS) && (header.Height < MASKED_MAX_TEXTURE_BOUNDS))
                    {
                        maskedTexture = true;  //probably a masked textures

                        //do an extra check to be sure this is not a transmasked
                        ushort lastheaderByte = BitConverter.ToUInt16(lumpdata, 10); //standard masked has no translevel, so cannot be 21 or 34

                        if (((lastheaderByte == 21) || (lastheaderByte == 34)) && ((header.Width >= 64) && (header.Height >= 64)))
                        { maskedTexture = false; } //probably this is a transmasked texture, mark it as none                    
                    }
                }                
            }

            return maskedTexture;
        }
        #endregion

    }
    #endregion

}
