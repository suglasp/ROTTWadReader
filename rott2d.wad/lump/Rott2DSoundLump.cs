/*
 * ROTT2D
 * Unit: ROTT2D Sound lump abstract Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 12-01-2012
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
using System.IO;
using System.Text;

namespace ROTT2D.WAD.data
{

    #region Sound & Music (SFX) Lump basic class
    /// <summary>
    /// Rott2D class for sound and music lump's
    /// </summary>
    public abstract class Rott2DSoundLump : Rott2DLump
    {
        /*
         * Base class for sound stuff in ROTT2D.
         * 
         */

        #region Destructors
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DSoundLump()
        {
            base.Dispose(false);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raw Array Memory stream
        /// </summary>
        public virtual byte[] GetStreamRaw()
        {
            return this._rawData;
        }

        /// <summary>
        /// MemoryStream object Memory stream
        /// </summary>
        public virtual MemoryStream GetStreamMemory()
        {
            MemoryStream sfxStream = null;

            if (this._rawData != null)
            {
                sfxStream = new MemoryStream(this._rawData);
            }

            return sfxStream;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "sfxlump_t";
        }
        #endregion

    }
    #endregion

}
