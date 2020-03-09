/*
 * ROTT2D
 * Unit: ROTT2D WinMM Simple Wave Player sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 13-01-2012
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
using System.Media;
using System.IO;

namespace ROTT2D.media
{

    /*
     * Uses C# integrated SoundPlayer class.
     * This class can play Wave RIFF files.
     * 
     */

    #region simple waveplayer class
    /// <summary>
    /// WinMM Wave player
    /// </summary>
    public sealed class Rott2DSimpleWavePlayer
    {

        #region Methods
        public static void playWave(string file)
        {
            if (File.Exists(file))
            {
                SoundPlayer player = new SoundPlayer(file);
                player.Play();
            }
        }


        public static void playWaveLoop(string file)
        {
            if (File.Exists(file))
            {
                SoundPlayer player = new SoundPlayer(file);
                player.PlayLooping();
            }
        }
        #endregion

    }
    #endregion

}
