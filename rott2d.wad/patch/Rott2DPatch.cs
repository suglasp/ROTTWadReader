/*
 * ROTT2D
 * Unit: ROTT2D Patch sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date:     11-01-2012
 * Last changed date: 16-02-2012
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

    #region Patch Header structs
    public struct Rott2DPatchHeader //ROTT WAD Patch structure (= Floor, Ceiling and menu/intro textures)
    {
        /*
         * Reference:
         * File lumpy.h in the original ROTT source code, this "patch" format is defined as lpic_t (large picture).
         * 
         * Description ROTT2D:
         * Each property in the header is a unsigned short value = 2 bytes (16bit)
         * 8 bytes header ==> 4 times a (u)short value
         * 
         * A standard ROTT patch lump would be 16392, this minus the header = 16384 bytes remaining
         * 128x128 = 16384 bytes
         * These are the floor and ceiling title textures.
         * 
         * 
         * 
         * Update 15-02-2012:
         * There are also larger patches in the ROTT WAD files.
         * Lumps TRILOGO, PLANE and AP_WRLD are now supported.
         * So in other words, these are patches that are 320x200 in size.
         * These would be 64004, this minus the header = 64000 bytes.
         * 
         * Extra note on this update: needs AP_PAL palette to be loaded in order to display AP_WRLD correctly.
         *
         * 
         */

        /// <summary>
        /// Private consts
        /// </summary>
        public const byte PATCH_HEADER_SIZE = 8;  //patch_t header is 8 bytes in size

        /// <summary>
        /// Private vars
        /// </summary>
        private ushort _patchWidth, _patchHeight; // texture bounding box size
        private short _patchOrgX, _patchOrgY;     // texture origin

        /// <summary>
        /// Width
        /// </summary>
        public ushort PatchWidth
        {
            get { return this._patchWidth; }
            set { this._patchWidth = value; }
        }

        /// <summary>
        /// Height
        /// </summary>
        public ushort PatchHeight
        {
            get { return this._patchHeight; }
            set { this._patchHeight = value; }
        }

        /// <summary>
        /// Origin X
        /// </summary>
        public short PatchOrgX
        {
            get { return this._patchOrgX; }
            set { this._patchOrgX = value; }
        }

        /// <summary>
        /// Origin Y
        /// </summary>
        public short PatchOrgY
        {
            get { return this._patchOrgY; }
            set { this._patchOrgY = value; }
        }

        public ushort PatchSize()
        { 
            return (ushort)(this.PatchWidth * this.PatchHeight);
        }
    }
    #endregion

    #region Patch class
    public sealed class Rott2DPatch : Rott2DTextureLump, IRott2DPatch
    {

        /*
         * Patches are mainly used for Floor and Ceiling textures.
         * These are mostly 128x128 pixels in size.
         * 
         * There are also larger patches, mainly used for the intro and menu textures.
         * In most cases, those textures are 320x200 pixels in size.
         * 
         */

        #region Public consts
        /// <summary>
        /// Private consts
        /// </summary>
        public const ushort PATCH_FC_SIZE      = 16384; //Floor & ceiling, 128x128 = 16384 bytes large
        public const ushort PATCH_PLANE_SIZE   = 45504; //plane lump, 288x158 = 45504 bytes large
        public const ushort PATCH_TRILOGO_SIZE = 64000; //trilogo and ap_wrld lumps, 320x200 = 64000 bytes large
        #endregion

        #region Private Vars
        /// <summary>
        /// Private vars
        /// </summary>
        private bool _headerReady = false; //header is ready
        private Rott2DPatchHeader _patchHeader; //patch header
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DPatch(ref byte[] patchLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this._rawData = patchLumpData;
            this._palette = palette;

            //process header data first
            this._patchHeader = new Rott2DPatchHeader();
            this.ProcessPatchHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
            {
                this._buffer = new Bitmap(this._patchHeader.PatchWidth, this._patchHeader.PatchHeight, PixelFormat.Format24bppRgb);
            }

            //generate buffer bitmap
            this.ProcessLumpData();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DPatch(string name, ref byte[] patchLumpData, ref Rott2DPalette palette)
        {
            this.isHeaderReady = false;
            this.isReady = false;
            this.Name = name;
            this._rawData = patchLumpData;
            this._palette = palette;

            //process header data first
            this._patchHeader = new Rott2DPatchHeader();
            this.ProcessPatchHeader();

            //create buffer from size
            if ((this._buffer == null) && (this.isHeaderReady))
                this._buffer = new Bitmap(this._patchHeader.PatchWidth, this._patchHeader.PatchHeight, PixelFormat.Format24bppRgb);

            //generate buffer bitmap
            this.ProcessLumpData();
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DPatch()
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
        private void ProcessPatchHeader()
        {
            if (this.GetDataSize() >= Rott2DPatchHeader.PATCH_HEADER_SIZE)
            {
                //read patch header
                this._patchHeader.PatchWidth = BitConverter.ToUInt16(this._rawData, 0);  //read width
                this._patchHeader.PatchHeight = BitConverter.ToUInt16(this._rawData, 2); //read height
                this._patchHeader.PatchOrgX = BitConverter.ToInt16(this._rawData, 4);   //read orgx
                this._patchHeader.PatchOrgY = BitConverter.ToInt16(this._rawData, 8);   //read orgy

                this.isHeaderReady = true;
            }
        }

        /// <summary>
        /// Generate the buffer texture
        /// </summary>
        protected override void ProcessLumpData()
        {
            if (this.GetDataSize() >= Rott2DPatchHeader.PATCH_HEADER_SIZE)
            {
                if ((this.isHeaderReady) && (this._buffer != null) && (this._palette.isReady))
                 {
                     using (Rott2DFastBitmap fastBuffer = new Rott2DFastBitmap(this._buffer))
                     {
                         //generate pixels to bitmap
                         int iLumpIndex = Rott2DPatchHeader.PATCH_HEADER_SIZE; //start pixels processing from last header byte
                         ushort iPaletteColorIndex = 0;       //return value for lump palette color index (0-256)

                         //generate pixels in bitmap

                         for (int x = 0; x < this._patchHeader.PatchWidth; x++)
                         {
                             for (int y = 0; y < this._patchHeader.PatchHeight; y++)
                             {
                                 //convert lump data to Palette Index values
                                 iPaletteColorIndex = this._rawData[iLumpIndex++];

                                 //get Palette Index and create color value
                                 Color rott2dImgColor = this._palette[iPaletteColorIndex];
                    
                                 //draw color pixels to bitmap texture
                                 fastBuffer.SetColor(x, y, rott2dImgColor);
                                 //this._buffer.SetPixel(x, y, rott2dImgColor);
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
            return "patch_t (lpic_t)";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Figure if we have a Patch (lpic_t; floor, ceiling and menu texures)
        /// </summary>
        public static bool isPatchTexture(byte[] lumpdata)
        {
            /*
             * Patches start always with 4 bytes (2x short) data for width and height.
             * Pixel count is mostly 16384 in size (total 16392 including the 4 bytes header data).
             * 
             */

            bool isPatch = false;
            
            if (lumpdata.Length > Rott2DPatchHeader.PATCH_HEADER_SIZE)
            {
                Rott2DPatchHeader header = new Rott2DPatchHeader();

                header.PatchWidth = BitConverter.ToUInt16(lumpdata, 0);   //width of texture
                header.PatchHeight = BitConverter.ToUInt16(lumpdata, 2);  //height of texture

                //measure total data size
                ushort pixelCount = (ushort)(header.PatchWidth * header.PatchHeight);  //calculate size of actual pixels, should be 16384

                
                 
                
                //Note: needed strikt bounds checking or else some larger masked textures where recognized as patches.
                if (((pixelCount == PATCH_FC_SIZE) || (pixelCount == PATCH_PLANE_SIZE) || (pixelCount == PATCH_TRILOGO_SIZE)) && (header.PatchWidth <= 320) && (header.PatchHeight <= 200))
                {

                    //16-02-2012 new bug introduced again: some 128x128 masked textures are recognized as patches.
                    //we need to check first of we have a valid patch by calculating the byte size
                    //a (trans-)masked would result in a larger value
                    if (header.PatchWidth * header.PatchHeight + 8 == lumpdata.Length)
                    {
                        isPatch = true;
                    }                                       
                }
            }
           
            return isPatch;
        }
        #endregion

    }
    #endregion

}
