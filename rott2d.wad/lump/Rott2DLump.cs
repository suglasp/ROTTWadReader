/*
 * ROTT2D
 * Unit: ROTT2D Lump Abstract Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 11-01-2012
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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ROTT2D.WAD.data
{

    #region Lump basic class
    /// <summary>
    /// Abstract class for a WAD Lump
    /// </summary>
    public abstract class Rott2DLump : IRott2DLump, IDisposable
    {

        /*
         * Base class for all lumps in a (ROTT) WAD file.
         * 
         */

        #region Private vars
        /// <summary>
        /// Private vars
        /// </summary>
        private int _id = 0;
        private string _name = "UNKNOWN";
        private int _size = 0;
        private int _offset = 0;

        private bool _disposed = false;
        #endregion

        #region Protected vars
        /// <summary>
        /// Protected vars
        /// </summary>
        protected bool _isReady = false;
        protected byte[] _rawData = null;
        #endregion

        #region Destructors
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DLump()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Dispose memory
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true); //dispose the class

            GC.SuppressFinalize(this); //call garbage collector
        }

        /// <summary>
        /// Dispose memory
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.isReady = false;

                    if (this._rawData != null)
                    {
                        this._rawData = null;
                    }
                }

                this._disposed = true;
            }
        }
        #endregion

        #region Getters and setters
        /// <summary>
        /// ReadOnly Property to check if lump is ready
        /// </summary>
        public bool isReady
        {
            get
            {
                return this._isReady;
            }

            protected set
            {
                this._isReady = value;
            }
        }

        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <summary>
        /// Property for name (sort of "ID" for the lump)
        /// </summary>
        public string Name
        {
            get {
                //always uppercase
                return this._name.ToUpper();
            }
            set {
                //only support a length of up to 8 characters uppercase
                /*if (value.Length > 8)
                    this._name = value.Substring(0, 8).ToUpper();
                else                
                    this._name = value.ToUpper();
                */

                this._name = (value.Length > 8) ? value.Substring(0, 8).ToUpper() : value.ToUpper();
            }
        }

        /// <summary>
        /// Offset
        /// </summary>
        public int Offset
        {
            get { return this._offset; }
            set { this._offset = value; }
        }

        /// <summary>
        /// Size
        /// </summary>
        public int Size
        {
            get { return this._size; }
            set { this._size = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method to force ready status
        /// (Can only be called internal and from derived classes)
        /// </summary>
        protected void ForceIsReady()
        {
            //only if we have valid data!
            if (this._rawData != null)
            {
                this.isReady = true;
            }
        }

        /// <summary>
        /// get the RAW data
        /// </summary>
        /// <returns></returns>
        public byte[] GetInternalData()
        {
            return this._rawData;
        }

        /// <summary>
        /// get the RAW data size
        /// </summary>
        public int GetDataSize()
        {
            int _datasize = -1;

            if (this._rawData != null)
            {
                _datasize = this._rawData.Length;
            }

            return _datasize;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "lump_t";
        }
        #endregion

    }
    #endregion

}
