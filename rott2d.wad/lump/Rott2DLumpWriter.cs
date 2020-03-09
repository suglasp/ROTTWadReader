/*
 * ROTT2D
 * Unit: ROTT2D Lump Writer Sealed Class
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

namespace ROTT2D.WAD.data
{
    public sealed class Rott2DLumpWriter : IRott2DLumpWriter, IDisposable
    {

        /* lump writer class */

        #region public consts
        /// <summary>
        /// Public consts
        /// </summary>
        public const string DEFAULT_LUMP_EXT = ".lmp";
        public const string MIDI_LUMP_EXT    = ".mid";
        public const string VOC_LUMP_EXT     = ".voc";
        public const string WAVE_LUMP_EXT    = ".wav";
        public const string BITMAP_LUMP_EXT  = ".bmp";
        public const string ASCII_LUMP_EXT   = ".txt";
        #endregion

        #region Private vars
        /// <summary>
        /// Private vars
        /// </summary>
        private bool _isReady = false;
        private byte[] _lumpExportData = null;  //databuffer to write

        private bool _disposed = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DLumpWriter(ref byte[] lumpExportData)
        {
            this._lumpExportData = lumpExportData;

            if (this._lumpExportData != null)
                this.isReady = true;
        }
        #endregion

        #region Destructors
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DLumpWriter()
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
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.isReady = false;

                    if (this._lumpExportData != null)
                    {
                        this._lumpExportData = null;
                    }
                }

                this._disposed = true;
            }
        }
        #endregion

        #region Getters & Setters
        /// <summary>
        /// ReadOnly Getter to check if data is ready
        /// </summary>
        public bool isReady
        {
            get
            {
                return this._isReady;
            }

            private set
            {
                this._isReady = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lump buffer data size
        /// </summary>
        public int GetDataSize()
        {
            int size = 0;

            if (this.isReady)
            {
                size = this._lumpExportData.Length;
            }

            return size;
        }

        /// <summary>
        /// Export a lump to a raw binary file
        /// </summary>
        public bool ExportLumpToFile(string filename, string ext = DEFAULT_LUMP_EXT)
        {
            bool exportSuccess = false;

            if (this.isReady)
            {
                if (this.GetDataSize() > 0)
                {
                    using (FileStream lumpFile = new FileStream(filename.ToLower() + ext.ToLower(), FileMode.Create))
                    {
                        using (BinaryWriter lumpBinary = new BinaryWriter(lumpFile))
                        {
                            foreach (byte b in this._lumpExportData)
                                lumpBinary.BaseStream.WriteByte(b);                            

                            lumpBinary.Flush();  //force flush so we are sure eveything is written to disk!
                        }
                    }

                    exportSuccess = true;
                }
            }

            return exportSuccess;
        } //exportLumpToFile
        #endregion

    }
}
