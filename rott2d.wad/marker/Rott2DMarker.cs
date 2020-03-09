/*
 * ROTT2D
 * Unit: ROTT2D Marker sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 14-01-2012
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

using ROTT2D.WAD.data;

namespace ROTT2D.WAD.marker
{

    #region marker structs
    /// <summary>
    /// Marker data type
    /// </summary>
    public enum Rott2DMarkerType
    { 
        mtUnknown,
        mtPalette,
        mtColormap,
        mtFlat,
        mtPic,
        mtPatch,
        mtSky,
        mtMasked,
        mtTransMasked,
        mtASCII,
        mtSound,
        mtMusic,
        mtPCSpeaker
    }
    #endregion

    #region marker lump class
    public sealed class Rott2DMarker : Rott2DLump, IRott2DMarker
    {

        /*
         * ROTT uses a type of lump that mostly is a zero data length lump type.
         * Not all of these zero data length lumps are in fact "markers" inside the WAD file.
         * 
         * A "marker" lump, is a sort of lump that is only present in a WAD to indicate other types
         * of lumps. Most other data lumps are stored between these marker lumps,
         * and serve only the purpose to "mark" a certain type of data lump.
         * 
         * For example:
         * Between the marker lumps "DIGISTRT" and "DIGISTOP" are all 
         * sound lumps stored. Note that not all "start" markers have a "STOP" marker.
         * 
         */

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DMarker(int id, string name, int offset, int size)
        {
            this.ID = id;
            this.Name = name;
            this.Offset = offset;
            this.Size = size;
        }
        #endregion

        #region Destructors
        /// <summary>
        /// Desctructor
        /// </summary>
        ~Rott2DMarker()
        {
            this.Dispose();
        }
        #endregion

    }
    #endregion

}
