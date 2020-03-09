/*
 * ROTT2D
 * Unit: ROTT2D Unknown sealed Class
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
using System.Text;

namespace ROTT2D.WAD.data
{

    #region Unknown lump class
    /// <summary>
    /// Sealed class for a WAD Unknown Lump
    /// </summary>
    public sealed class Rott2DUnknown : Rott2DLump, IRott2DUnknown
    {

        /*
         * A unknown lump can be anything from a pc speaker to gusmidi, ... lumps.
         * 
         * It can hold the original lump name (or any "ID") and the raw bytes.
         * 
         */

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DUnknown(ref byte[] unknownRawData)
        {
            this._rawData = unknownRawData;

            if (this.GetDataSize() > 0)
            {
                this.isReady = true;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DUnknown(string name, ref byte[] unknownRawData)
        {
            this.Name = name;
            this._rawData = unknownRawData;

            if (this.GetDataSize() > 0)
            {
                this.isReady = true;
            }
        }
        #endregion
                     
        #region Destructors
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DUnknown()
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
            return "unknown_t";
        }
        #endregion

    }
    #endregion

}
