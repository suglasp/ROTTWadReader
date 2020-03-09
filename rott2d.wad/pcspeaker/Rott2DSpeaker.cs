/*
 * ROTT2D
 * Unit: ROTT2D pc speaker sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 09-02-2012
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
using System.Text;

using ROTT2D.media;

namespace ROTT2D.WAD.data
{
    /// <summary>
    /// PC Speaker class
    /// </summary>
    public sealed class Rott2DSpeaker : Rott2DSoundLump, IRott2DSpeaker
    {

        /*
         * Currently a very early experimental class that tries
         * to read and play the PC Speaker sound lumps in ROTT a WAD file.
         * 
         * This needs more research from the ROTT source code.
         * 
         */

        #region Private vars
        /// <summary>
        /// Private vars
        /// </summary>        
        //header size of a pc speaker sound
        private short PCSFX_HEADER_SIZE = 4; //four bytes

        //header stuff from the ROTT source code
        private short _length;
        private short _priority;
        private byte[] _speakerSFXData;

        //Frequency data table
        private float[] frequencies = { 0, 175.00f, 180.02f, 185.01f, 190.02f, 196.02f, 202.02f, 208.01f, 214.02f, 220.02f,
    226.02f, 233.04f, 240.02f, 247.03f, 254.03f, 262.00f, 269.03f, 277.03f, 285.04f,
    294.03f, 302.07f, 311.04f, 320.05f, 330.06f, 339.06f, 349.08f, 359.06f, 370.09f,
    381.08f, 392.10f, 403.10f, 415.01f, 427.05f, 440.12f, 453.16f, 466.08f, 480.15f,
    494.07f, 508.16f, 523.09f, 539.16f, 554.19f, 571.17f, 587.19f, 604.14f, 622.09f,
    640.11f, 659.21f, 679.10f, 698.17f, 719.21f, 740.18f, 762.41f, 784.47f, 807.29f,
    831.48f, 855.32f, 880.57f, 906.67f, 932.17f, 960.69f, 988.55f, 1017.20f, 1046.64f,
    1077.85f, 1109.93f, 1141.79f, 1175.54f, 1210.12f, 1244.19f, 1281.61f, 1318.43f,
    1357.42f, 1397.16f, 1439.30f, 1480.37f, 1523.85f, 1569.97f, 1614.58f, 1661.81f,
    1711.87f, 1762.45f, 1813.34f, 1864.34f, 1921.38f, 1975.46f, 2036.14f, 2093.29f,
    2157.64f, 2217.80f, 2285.78f, 2353.41f, 2420.24f, 2490.98f, 2565.97f, 2639.77f };
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DSpeaker(ref byte[] data)
        {
            this.isReady = false;
            this._rawData = data;

            this.ProcessSpeakerHeader();
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DSpeaker()
        {
            this.Dispose();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Read pc speaker data fields
        /// </summary>
        private void ProcessSpeakerHeader()
        {
            /*
             * Checking the original ROTT source code and Doom FAQ,
             * i suspect as with other code, ROTT uses for pc speaker sounds
             * the same format from Doom, except the first two zero's.
             * 
             */

            //header data
            this._length = BitConverter.ToInt16(this._rawData, 0);
            this._priority = BitConverter.ToInt16(this._rawData, 2);

            this._speakerSFXData = new byte[this._rawData.Length - PCSFX_HEADER_SIZE];

            //read sound data
            for (int sdi = 0; sdi < (this._rawData.Length - PCSFX_HEADER_SIZE); sdi++)
            {
                this._speakerSFXData[sdi] = this._rawData[PCSFX_HEADER_SIZE + sdi];
            }

            //flag ready
            this.isReady = true;
        }

        /// <summary>
        /// Play a pc speaker sound
        /// </summary>
        private void PlaySpeakerValue(int val)
        {
            int ms;

            ms = (this._length * 1000) / 140;

            //ms = (this._length) / 140;

            //if (val > 0)
            //{
            //    Rott2DSimplePCspeakerPlayer.Beep((uint)frequencies[val], (uint)ms);
            //}
            
            if (val == 0)
            {
                System.Threading.Thread.Sleep(ms);
            }
            else
            {
                //Rott2DSimplePCspeakerPlayer.Beep((uint)val, (uint)ms);
                Rott2DSimplePCspeakerPlayer.Beep((uint)frequencies[val], (uint)ms);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Play the pc speaker sound
        /// </summary>
        public void PlaySpeaker()
        {
            if (this._isReady)
            {
                if (this._speakerSFXData.Length > 0)
                {
                    for (int i = 0; i < this._speakerSFXData.Length; i++)
                    {
                        this.PlaySpeakerValue(this._speakerSFXData[i]);
                    }
                }
            }
        }
        #endregion


    }
}
