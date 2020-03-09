/*
 * ROTT2D
 * Unit: ROTT2D "snea" Small Picture sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 17-01-2012
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

    #region Small Picture Header structs
    public struct Rott2DPicHeader //ROTT WAD pic structure (= small GUI textures)
    {
        /*
         * each property in the header is a byte value = 1 bytes (8bit)
         * 2 bytes header ==> 2 times a byte value
         * 
         */

        /// <summary>
        /// Public consts
        /// </summary>
        public const ushort PIC_HEADER_MULTIPLIER = 4; //pic multiplier
        public const byte PIC_HEADER_SIZE = 2; //pic header is 2 bytes in size
        public const byte PIC_TRAIL_SIZE = 2; //pic trailing is 2 bytes in size
        public const byte PIC_UPPER_PLANE_BOUNDS = 80;  //max plane size of 80 (80*4 = 320)

        /// <summary>
        /// Private vars
        /// </summary>
        //texture bounding box size
        private ushort _picWidth;  //width in byte (width needs to be ushort because _picPlaneSize * 4 can result in value larger then byte)
        private byte _picHeight; //height in byte
        private byte _picPlaneSize; //plane size in byte (this is my own parameter for col off's)

        /// <summary>
        /// Width
        /// </summary>
        public ushort PicWidth
        {
            get { return this._picWidth; }
            private set { this._picWidth = value; }  //private, can only be set inside the header structure
        }

        /// <summary>
        /// Height
        /// </summary>
        public byte PicHeight
        {
            get { return this._picHeight; }
            set { this._picHeight = value; }
        }

        /// <summary>
        /// Plane Size
        /// </summary>
        public byte PicPlaneSize
        {
            get { return this._picPlaneSize; }
            set { 
                this._picPlaneSize = value;

                //auto set the correct pic_t Width value
                if (this._picPlaneSize > 0)
                {
                    this.PicWidth = (ushort)(this.PicPlaneSize * PIC_HEADER_MULTIPLIER);
                }
            }
        }
    }
    #endregion

    #region Small Picture class
    public sealed class Rott2DPic : Rott2DTextureLump, IRott2DPic
    {

        /*
         * -- date 16-01-2012:
         * I found this lump structure when browsing through the ROTT source code
         * in header file "lumpy.h". This type is not defined on the hacker.txt document from Apogee.
         * typedef struct
         * {
         *   byte     width,height;
         *   byte     data;
         * } pic_t;
         * 
         * Since there is the patch structure that holds a short value for width and height (+ offset values),
         * some textures have a smaller header with only width and height as byte values.
         * 
         * These are all tiny textures in ROTT mainly used for GUI overlay stuff and menu.
         * 
         * 
         * 
         * -- update: date 18-01-2012:
         * In the original released Doom source code (by ID-soft), this type is defined as pic_t.
         * Found in file "doomtype.h". But only it's definition remained from Wolf3D. The type defined in the ROTT
         * code thus is exactly the same as in the Doom header source code:
         * //
         * // Flats?
         * //
         * // a pic is an unmasked block of pixels
         * typedef struct
         * {
         *    byte width;
         *    byte height;
         *    byte data;
         * } pic_t;
         * 
         * Doom v0.3 also used this format, but abondoned the format later on in alpha releases, to be replaced
         * with the LBE masked and transmasked image formats (that ROTT also uses!). Deutex introduced this
         * pic_t format under the name "Snea", other editors inflated the same code from Deutex.
         * 
         * -- Date 24-01-2012:
         * Finally got the time to figure out the algoritm in C#.
         * 
         * -- Date 28-01-2012:
         * This is a nasty data format! I was able to figure out how the algoritm works,
         * by reverse engineering the output images with Raster Image software
         * and try to figure out how the raw data was used to build to a Bitmap.
         * First a pic of 8xn worked, but then trouble arose if i would read a pic_t
         * larger then 8 pix in width. So now i've added a new algortim to figure out
         * if the col-offset's are divadable by four.
         * In a meantime, i also found that Wolf3D introduced this picture format before Doom.
         * Later, Doom adopted the format and again dropped it. ROTT adopted all format's
         * that Doom used. Only this pic_t format in ROTT is only used for GUI overlay stuff.
         * 
         * -- Header struct is:
         *  "pixel plane size" = raw value
         *  "height" = raw value
         *  
         * then we can calulate the width as:
         *  "width"  = "pixel plane size" * 4
         * 
         * -- Notes:
         * Note that ROTT uses two trailing bytes (each value 0) in this format.
         * Also, ROTT supports this format as a 'transparent' picture format where Doom doesn't.
         * 
         * Since ROTT and Doom in there original source code call this type pic_t,
         * i chose the class to be name as Rott2DPic.
         * 
         * 
         */

        #region Public consts
        /// <summary>
        /// Private consts
        /// </summary>      
        public const byte PIC_TRAILING_SIZE = 2; //trailing bytes are 2 bytes in size
        public const byte PIC_TRANS_COLOR = 255; //pic trans color index

        #endregion

        #region Private Vars
        /// <summary>
        /// Private vars
        /// </summary>
        private bool _headerReady = false; //header is ready
        private Rott2DPicHeader _picHeader; //small picture texture header
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DPic(ref byte[] picLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this._rawData = picLumpData;
            this._palette = palette;

            //process header data first
            this._picHeader = new Rott2DPicHeader();
            this.ProcessPicHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
            {
                this._buffer = new Bitmap(this._picHeader.PicWidth, this._picHeader.PicHeight, PixelFormat.Format24bppRgb);
            }

            //generate buffer bitmap
            this.ProcessLumpData();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DPic(string name, ref byte[] picLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this.Name = name;
            this._rawData = picLumpData;
            this._palette = palette;

            //process header data first
            this._picHeader = new Rott2DPicHeader();
            this.ProcessPicHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
            {
                this._buffer = new Bitmap(this._picHeader.PicWidth, this._picHeader.PicHeight, PixelFormat.Format24bppRgb);
            }

            //generate buffer bitmap
            this.ProcessLumpData();
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DPic()
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
        private void ProcessPicHeader()
        {
            if (this.GetDataSize() >= Rott2DPicHeader.PIC_HEADER_SIZE)
            {
                //read pic header width & height
                this._picHeader.PicPlaneSize = this._rawData[0];                        //plane size
                //this._picHeader.PicWidth = (ushort)(this._picHeader.PicPlaneSize * PIC_HEADER_MULTIPLIER);  //convert width to byte
                this._picHeader.PicHeight = this._rawData[1];                   //read height as byte
                
                //for information, you can apply algortim for bound checking:
                //int heightCRC = (this._rawData.Length - PIC_HEADER_SIZE - PIC_TRAILING_SIZE) / this._picHeader.PicWidth;
                //int widthCRC  = (this._rawData.Length - PIC_HEADER_SIZE - PIC_TRAILING_SIZE) / this._picHeader.PicHeight;

                this.isHeaderReady = true;

                //this.dumpRawStreamToPic();
            }
        }

        /// <summary>
        /// DEBUGGING STUFF, TO BE REMOVED FROM SOURCE
        /// </summary>
        /*private void DumpRawStreamToPic()
        {
            if (this.isHeaderReady)
            {
                using (Bitmap dumper = new Bitmap(this._rawData.Length, 1))
                { 
                    for (int p = 0; p < this._rawData.Length; p++)
                    {                    
                        short index = this._rawData[p];

                        Color colorDump = this._palette[index];

                        dumper.SetPixel(p, 0, colorDump);
                    }

                    dumper.Save(Environment.CurrentDirectory + @"\data\cache\dump.bmp");
                }
            }
        }*/

        /// <summary>
        /// Generate the buffer texture
        /// </summary>
        protected override void ProcessLumpData()
        {
            if (this.GetDataSize() > Rott2DPicHeader.PIC_HEADER_SIZE)
            {
                if ((this.isHeaderReady) && (this._buffer != null) && (this._palette.isReady))
                 {
                     using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                     {
                         //first, fill buffer with transparency color from palette 
                         Color textureTransparencyColor = this._palette.GetMaskedColor();
                         fastBuffer.SetFill(textureTransparencyColor);
                         //this.fillBuffer(ref textureTransparencyColor);

                         //convert pic_t raw data stream to Bitmap
                         int iLumpIndex = Rott2DPicHeader.PIC_HEADER_SIZE;                                    //start pixels processing from last header byte
                         ushort iPaletteColorIndex = Rott2DPalette.DEFAULT_MASKED_COLOR;       //value for lump palette color index (0-255)
                         int iLastLumpIndex = iLumpIndex;                                     //remeber last LumpIndex start offset
                         int iColoffFactor = 1;                                               //set starting factor not zero.

                         //go for all pic_t columns
                         //(this is a modex image format)
                         for (ushort x = 0; x < this._picHeader.PicWidth; x++)  //rows (horz)
                         {
                             //After the first three rows, start checking if we have a col off's
                             //that is dividable by four. If so, shift index by one in the raw data
                             //bytes stream and start reading from that position on.
                             //For images of 16 pix width, this happens once. Larger width then 16 results in more shifts.
                             if (x > (Rott2DPicHeader.PIC_HEADER_MULTIPLIER - 1))  //x > 3
                             {                                 
                                 iColoffFactor = x % Rott2DPicHeader.PIC_HEADER_MULTIPLIER;  //x % 4

                                 if (iColoffFactor == 0)
                                 {
                                     iLumpIndex = iLastLumpIndex = iLastLumpIndex + 1;
                                 }
                             }

                             //go down the pic_t colums and read pixels
                             for (ushort y = 0; y < this._picHeader.PicHeight; y++)  //cols (vert)
                             {
                                 //get color and decide if is transparent color or not
                                 iPaletteColorIndex = this._rawData[iLumpIndex]; //get color from palette

                                 if (iPaletteColorIndex != 255)
                                 {
                                     //draw palette color
                                     Color rott2dImgColor = this._palette[iPaletteColorIndex];
                                     fastBuffer.SetColor(x, y, rott2dImgColor);
                                     //this._buffer.SetPixel(x, y, rott2dImgColor);
                                 }

                                 iLumpIndex += this._picHeader.PicPlaneSize; //move ahead in the data stream by supplied multiplier
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
            return "pic_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Figure if we have a small picture (pic_t; GUI overlay textures)
        /// </summary>
        public static bool isPicTexture(byte[] lumpdata)
        {
            /*
             * small picture header is 2 bytes
             * the actual header structure only contains width and height in byte (= 2 bytes)
             * 
             * on a key image (8x16 in size) are the first foud bytes always:
             * x * 4  ==> width. You have to multiply by four. (value * 4)
             * y      ==> Height
             * 
             * A second paramter to check for are the two trailing bytes. 
             * These two trailing bytes are a left over from Doom alpha versions.
             * 
             */

            bool isPic = false;

            if (lumpdata.Length > Rott2DPicHeader.PIC_HEADER_SIZE)
            {
                //PIC_UPPER_PLANE_BOUNDS is 80, reason:
                //80 * 4 = 320. This is the largest pic format found in ROTT wad files.
                //so this explains why upper plane bounds size is set to be 80.

                Rott2DPicHeader header = new Rott2DPicHeader();
                header.PicPlaneSize = lumpdata[0];            //plane size
                header.PicHeight = lumpdata[1];               //read height as byte

                byte trailing1 = lumpdata[lumpdata.Length - 2];  //trailing bytes
                byte trailing2 = lumpdata[lumpdata.Length - 1];

                if (( ( ( (header.PicWidth * header.PicHeight) + (Rott2DPicHeader.PIC_HEADER_SIZE + Rott2DPicHeader.PIC_TRAIL_SIZE) ) == lumpdata.Length)) && (header.PicPlaneSize <= Rott2DPicHeader.PIC_UPPER_PLANE_BOUNDS))
                {
                    if ((trailing1 == 0) && (trailing2 == 0))
                        isPic = true;
                }
            }

            return isPic;
        }
        #endregion

    }
    #endregion

}
