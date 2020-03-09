/*
 * ROTT2D
 * Unit: ROTT2D Midi sealed Class
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

    #region midi class
    /// <summary>
    /// ROTT2D class for loading Midi files from a lump data
    /// </summary>
    public sealed class Rott2DMidi : Rott2DLump, IRott2DMidi
    {

        /*
         * ROTT music data lump's are actually fully stored Midi files,
         * stored as lump into a WAD file.
         * 
         * Originally, Doom used GUS files (General Midi) for it's music.
         * - GUS supports 8 channels, with 1 sample per track.
         * - Midi supports 16 channels, with 3 samples per track.
         * 
         * Midi is a derived file format from the GUS file format standard.
         * 
         */

        #region Public consts
        public const int MUSIC_MIDI_HEADER_SIZE  = 4;        //header size of a midi file
        public const string MUSIC_MIDI_HEADER_ID = "MThd";   //header ID
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DMidi(ref byte[] midiLumpData)
        {
            this.isReady = false;
            this._rawData = midiLumpData;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DMidi(string name, ref byte[] midiLumpData)
        {
            this.isReady = false;
            this.Name = name;
            this._rawData = midiLumpData;
        }
        #endregion
                
        #region Destructors
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DMidi()
        {
            this.Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Export the Midi lump to a midi file
        /// </summary>
        public void ExportMidiToFile(string filename)
        {
            if (this._rawData != null)
            {
                Rott2DLumpWriter exportMidi = new Rott2DLumpWriter(ref this._rawData);
                this.isReady = exportMidi.ExportLumpToFile(filename, Rott2DLumpWriter.MIDI_LUMP_EXT);
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "midi_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Try figure out if we have a ROTT music lump (MIDI)
        /// </summary>
        public static bool isMidiLump(byte[] lumpdata)
        {
            /*
             * For a Midi file, the first bytes of header are as follow:
             * 4D 54 68 64 00 00 00 06 ff ff nn nn dd dd
             * 
             * The ACSII equivalent of the first 4 bytes is "MThd". After "MThd" comes the 4-byte size of the Midi header. 
             * This will always be 00 00 00 06, because the actual header information will always be 6 bytes.
             * 
             */

            bool midiLump = false;

            if (lumpdata.Length > MUSIC_MIDI_HEADER_SIZE)
            {
                System.Text.Encoding encoding = System.Text.ASCIIEncoding.ASCII;
                string description = encoding.GetString(lumpdata, 0, MUSIC_MIDI_HEADER_SIZE);   //"MThd" is first 4 bytes

                //check header writing of a Midi lump, do the compare of "MThd" in lower case
                if (description.ToLower() == MUSIC_MIDI_HEADER_ID.ToLower())   //check the first 4 bytes MThd, to lower case
                    midiLump = true;
            }

            return midiLump;
        }
        #endregion

    }
    #endregion

}
