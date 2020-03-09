/*
 * ROTT2D
 * Unit: ROTT2D bitmap tools Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 02-07-2012
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ROTT2D.draw
{
    public class Rott2DBitmapTools
    {
        /// <summary>
        /// Copy a Bitmap from file
        /// </summary>
        static public Bitmap deepCopyBitmap(string originalBitmap)
        {
            Bitmap deepCopy = null;

            try
            {
                using (FileStream fs = new FileStream(@originalBitmap, FileMode.Open))
                {
                    int len = (int)fs.Length;
                    byte[] buf = new byte[len];
                    fs.Read(buf, 0, len);

                    using (MemoryStream ms = new MemoryStream(buf))
                    {
                        deepCopy = new Bitmap(ms);
                    }
                }
            }
            catch (Exception ex)
            { }

            return deepCopy;
        }

        /// <summary>
        /// Copy a Bitmap in memory
        /// </summary>
        static public Bitmap deepCopyBitmap(Bitmap originalBitmap)
        {
            Bitmap deepCopy = null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    originalBitmap.Save(ms, ImageFormat.MemoryBmp);
                    deepCopy = new Bitmap(ms);
                }
            }
            catch (Exception ex)
            { }

            return deepCopy;
        }

    }
}
