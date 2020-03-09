/*
 * ROTT2D
 * Unit: ROTT2D Flat sealed Class
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

    #region flat class (wall)
    public sealed class Rott2DFlat : Rott2DRaw, IRott2DFlat
    {

        /*
         * A Flat, are textures used for only (static) wall's. With static, i mean non-shadered wall's.
         * 
         * Note:
         * Instead of calling this class Rott2DWall, i've called it Rott2DFlat in analogy to
         * Doom's flat lump's. Doom uses the same structure for it's basic wall textures.
         * These lump's are always placed first in the ROTT WAD.
         * 
         * In ROTT, Flat data has no header structure. All Flat textures are 64x64 in size.
         * If you would import larger textures as Flat, ROTT would only read them up to 64x64 in size.
         * 
         * For custom or larger textures, the DIP team used patches (refer to Rott2DPatch class).
         * 
         */

        #region Public consts
        /// <summary>
        /// Private consts
        /// </summary>
        public const ushort FLAT_TEXTURE_WIDTH = 64;
        public const ushort FLAT_TEXTURE_HEIGHT = 64;
        public const ushort FLAT_DATA_SIZE = FLAT_TEXTURE_WIDTH * FLAT_TEXTURE_HEIGHT; //64x64 = 4096 bytes large
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DFlat(ref byte[] flatLumpData, ref Rott2DPalette palette) : base(FLAT_TEXTURE_WIDTH, FLAT_TEXTURE_HEIGHT, ref flatLumpData, ref palette)
        {     
            this.ProcessLumpData(); //generate !
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DFlat(string name, ref byte[] flatLumpData, ref Rott2DPalette palette) : base(name, FLAT_TEXTURE_WIDTH, FLAT_TEXTURE_HEIGHT, ref flatLumpData, ref palette)
        {
            this.ProcessLumpData(); //generate !
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DFlat()
        {
            this.Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "flat_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Figure if we have a Flat (wall texures)
        /// </summary>
        public static bool isFlatTexture(byte[] lumpdata)
        {

            /*
             * Flats are always 64x64 pixels = 4096 bytes.
             * 
             * There are other lump's in the ROTT wad files that are 4006 in size,
             * and are masked textures. So we have to becareful.
             * 
             */

            bool isFlat = false;

            isFlat = ((lumpdata.Length > 4000) ? ((lumpdata.Length == FLAT_DATA_SIZE) ? true : false) : false);

            return isFlat;
        }
        #endregion

    }
    #endregion

}
