/*
 * ROTT2D
 * Unit: ROTT2D 256 Palette Interface
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 09-01-2012
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
using System.Drawing;

namespace ROTT2D.WAD.palette
{
    /// <summary>
    /// Interface for a Rott2DPalette
    /// </summary>
    public interface IRott2DPalette
    {
        //Request a readonly indexer to get a nullable Color struct
        Color this[ushort paletteIndex]
        {
            get;
        }

        //Request a readonly getter for the palette status
        bool isReady
        {
            get;
        }
    }
}
