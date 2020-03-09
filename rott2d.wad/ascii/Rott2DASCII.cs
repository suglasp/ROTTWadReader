/*
 * ROTT2D
 * Unit: ROTT2D ASCII Text sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 15-01-2012
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

    #region ascii text class
    public sealed class Rott2DASCII : Rott2DLump, IRott2DASCII
    {

        /*
         * ROTT WAD files contain lump's with simple ASCII text.
         * These are totally non-encoded data structures, just plain ASCII text lumps
         * written to the WAD in a serial byte order.
         * 
         */

        #region Private vars
        /// <summary>
        /// Private vars
        /// </summary>
        private string _textMessage = string.Empty;  //text buffer for the converted message
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DASCII(ref byte[] textLumpData)
        {
            this.isReady = false;
            this._rawData = textLumpData;

            this.ProcessLumpData(); //convert to text ascii !
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DASCII(string name, ref byte[] textLumpData)
        {
            this.isReady = false;
            this.Name = name;
            this._rawData = textLumpData;

            this.ProcessLumpData(); //convert to text ascii !
        }        
        #endregion
                     
        #region Destructors
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DASCII()
        {
            base.Dispose();
        }
        #endregion

        #region Getters & Setters
        /// <summary>
        /// Get the text message property
        /// </summary>
        public string TextMessage
        {
            get { return this._textMessage; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Generate the acii text
        /// </summary>
        private void ProcessLumpData()
        {
            if (this.GetDataSize() > 0)
            {
                Encoding encEncoder = ASCIIEncoding.ASCII;
                this._textMessage = encEncoder.GetString(this._rawData);
            }
        }

        /// <summary>
        /// Get the text message
        /// </summary>
        public string GetText()
        {
            return this._textMessage;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "ascii_t";
        }
        #endregion

        #region Static Methods

        #endregion

    }
    #endregion

}
