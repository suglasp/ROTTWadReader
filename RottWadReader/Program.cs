/*
 * ROTT2D
 * Unit: ROTT2D Program - WAD Reader
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 10-01-2012
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
using System.Windows.Forms;


namespace RottWadReader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //check DLL's
            if (File.Exists(Environment.CurrentDirectory + @"\rott2d.draw.dll"))
            {
                if (File.Exists(Environment.CurrentDirectory + @"\rott2d.wad.dll"))
                {
                    if (File.Exists(Environment.CurrentDirectory + @"\rott2d.media.dll"))
                    {
                        Application.Run(new frmWADReader());
                    }
                }
            }
            

        }
    }
}
