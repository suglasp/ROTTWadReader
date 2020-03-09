/*
 * ROTT2D
 * Unit: ROTT2D WinMM Simple Midi Player sealed Class
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
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace ROTT2D.media
{

    #region simple midiplayer class
    /// <summary>
    /// WinMM Midi mediaplayer
    /// </summary>
    public sealed class Rott2DSimpleMidiPlayer
    {
        /*
         * MS info for the mciSendString play command:
         * http://msdn.microsoft.com/en-us/library/windows/desktop/dd743667%28v=vs.85%29.aspx
         */

        #region DLL Imports
        /// <summary>
        /// MIDI player through WinMM
        /// </summary>
        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);
        #endregion

        #region Methods
        public static long playMidi(string file, string name)
        {
            long lRet = -1;

            if (File.Exists(file))
            {
                lRet = mciSendString("open " + file + " alias " + name, null, 0, IntPtr.Zero);  //play once
                lRet = mciSendString("play track", null, 0, IntPtr.Zero);
            }

            return lRet;                
        }


        public static long playMidiLoop(string file, string name)
        {
            long lRet = -1;

            if (File.Exists(file))
            {
                lRet = mciSendString("open " + file + " alias " + name, null, 0, IntPtr.Zero);  //play once
                lRet = mciSendString("play " + name + " loop", null, 0, IntPtr.Zero);
            }

            return lRet;
        }

        public static long stopMidi(string name)
        {
            long lRet = -1;
            lRet = mciSendString("stop " + name, null, 0, IntPtr.Zero);
            lRet = mciSendString("close " + name, null, 0, IntPtr.Zero);

            return lRet;
        }

        public static long stopAllMidi()
        {
            long lRet = -1;
            lRet = mciSendString("stop all", null, 0, IntPtr.Zero);
            lRet = mciSendString("close all", null, 0, IntPtr.Zero);

            return lRet;
        }
        #endregion

    }
    #endregion

}
