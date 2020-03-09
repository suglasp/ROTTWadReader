/*
 * ROTT2D
 * Unit: ROTT2D Sky sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 11-01-2012
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

    #region Sky class
    public sealed class Rott2DSky : Rott2DRaw, IRott2DSky
    {
        /*
         * A ROTT Sky texture is very near the same as a Flat (= Wall) texture.
         * They have no header. The only way to discover if the lump is a Sky texture, is by calculating
         * the size of the lump (51200 bytes) or by search between the marker lumps SKYSTART/SKYEND.
         * Sky textures are 256x200 in dimension.
         * 
         * Raw format is derived from the LBM format.
         * 
         */
         
        #region Public consts
        /// <summary>
        /// Private consts
        /// </summary>
        public const ushort SKY_TEXTURE_WIDTH = 256;
        public const ushort SKY_TEXTURE_HEIGHT = 200;
        public const ushort SKY_DATA_SIZE = SKY_TEXTURE_WIDTH * SKY_TEXTURE_HEIGHT; //256x200 = 51200 bytes large
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DSky(ref byte[] skyLumpData, ref Rott2DPalette palette) : base(SKY_TEXTURE_WIDTH, SKY_TEXTURE_HEIGHT, ref skyLumpData, ref palette)
        {
            this.ProcessLumpData(); //generate !
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DSky(string name, ref byte[] skyLumpData, ref Rott2DPalette palette) : base(name, SKY_TEXTURE_WIDTH, SKY_TEXTURE_HEIGHT, ref skyLumpData, ref palette)
        {
            this.ProcessLumpData(); //generate !
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DSky()
        {
            base.Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "sky_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Figure if we have a Sky (skies)
        /// </summary>
        public static bool isSkyTexture(byte[] lumpdata)
        {

            /*
             * Skies are always 256x200 pixels = 51200 bytes.
             * 
             */

            bool isSky = false;

            isSky = ((lumpdata.Length > 50000) ? ((lumpdata.Length == SKY_DATA_SIZE) ? true : false) : false);

            return isSky;
        }

        #endregion

    }
    #endregion

}
