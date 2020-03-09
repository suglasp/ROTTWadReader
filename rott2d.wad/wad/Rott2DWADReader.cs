/*
 * ROTT2D
 * Unit: ROTT2D WadReader Class
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
 * 
 * 
 * 
 * Rise Of The Triad (also know as "ROTT") WAD file format reader.
 * This class can read a ROTT WAD file and load all lump information that a WAD carries.
 * 
 * -- ROTT came with two WAD files when released for public audience:
 *  "The Dark Ward" wad file   : darkwar.wad  (= Full version).
 *  "The Hunt Begins" wad file : huntbgin.wad (= Shareware version).
 * 
 * -- ROTT Game Notes:
 * Doom originally uses two versions of WAD files (IWAD an PWAD). ROTT uses a derived WAD format from Doom's IWAD.
 *   IWAD = "Internal WAD", original game WAD's.
 *   PWAD = "Patched WAD", modification game WAD's. They have priority over IWAD's.
 *   In Doom terminology, these identifiers are called "Magic" Identifiers.
 * 
 * 
 * -- Lump types in a ROTT WAD file:
 * Marker lumps are idendentically the same as in Doom's IWAD/PWAD format.
 * Palette lumps are the same as in Doom IWAD format.
 *   (Some functions in the source of the ROTT engine are identical to Doom's "wadutils" source code.)
 * Texture lumps in ROTT are very nearly the same as early versions of the Doom beta source code.
 * there is one type found "pic_t" in ROTT that was used only in early Doom alpha versions and Wolf3D.
 *   (Except for a few details. origsize is not found in Doom patch format, and the NExT pixels are
 *   not found in the ROTT texture patches). * 
 * Sounds lumps are fully stored Creative Labs Voice (.voc) files.
 * Music lumps are fully stored Midi (.mid) files.
 * Text lumps are plain ASCII data characters.
 * PC Speaker lumps are raw bytes that represent Frequency and Duration.
 *    (I know by testing that the pc speaker format in ROTT lump's is not equal to
 *    Doom's PC Speaker formatted lumps. But i don't go further digging those out.)
 * All other lump formats found in the ROTT WAD files have not been examined,
 * as don't need them for my ROTT2D Project.
 * 
 */

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;

namespace ROTT2D.WAD.reader
{

    #region WAD file structures
    /// <summary>
    /// WAD Header structure
    /// </summary>
    public struct Rott2DWadHeader //Header of a WAD file
    {
        private string _wadFileId; //this must be the value of declared constant IWAD_ID
        private int _numEntries; //number of entry's in the allocated WAD file
        private int _dirOffset; //LUMP directory offset in a WAD file

        public bool _isWADFile; //declare our own flag: is this a correct ROTT WAD file?

        /// <summary>
        /// IsWADFile
        /// </summary>
        public string WadFileId
        {
            get { return this._wadFileId; }
            set { this._wadFileId = value; }
        }

        /// <summary>
        /// NumEntries
        /// </summary>
        public int NumEntries
        {
            get { return this._numEntries; }
            set { this._numEntries = value; }
        }

        /// <summary>
        /// DirOffset
        /// </summary>
        public int DirOffset
        {
            get { return this._dirOffset; }
            set { this._dirOffset = value; }
        }

        /// <summary>
        /// IsWADFile
        /// </summary>
        public bool IsWADFile
        {
            get { return this._isWADFile; }
            set { this._isWADFile = value; }
        }
    }


    /// <summary>
    /// WAD Directory Entry structure
    /// </summary>
    public struct Rott2DWadDirectoryEntry  //Directory entry is 16 bytes
    {
        /// <summary>
        /// Private vars
        /// </summary>
        private int _id;      //this is my own added flag
        private string _name; //name of the lump (8 byte)
        private int _offset;  //offset of lump in wad file (4 byte)
        private int _size;    //size of the lump (4 byte)

        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get {
                //normally in a regulare ROTT WAD lump names are uppercase
                return this._name.ToUpper();
            } 
            set {
                //only support a length of up to 8 characters uppercase
                /*if (value.Length > 8)
                {
                    this._name = value.Substring(0, 8).ToUpper(); 
                }
                else
                {
                    this._name = value.ToUpper();
                }
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

        
        /// <summary>
        /// IEnumerable.GetEnumerator
        /// </summary>
        /*IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this.Name;   
        }

        
        public PeopleEnum GetEnumerator()
        {
            return new PeopleEnum(_people);
        }
        */
    }

    #endregion
    
    #region Rott2DWADReader Class
    /// <summary>
    /// Wad reader class
    /// </summary>
    public class Rott2DWADReader : IRott2DWADReader, IDisposable
    {

        /*
         * This class can read an original valid ROTT WAD file.
         * 
         */

        #region Public consts
        /// <summary>
        /// Public consts
        /// </summary>
        public const string IWAD_ID = "IWAD"; //ROTT only uses "IWAD"-WAD files, "PWAD"-WAD files are unsupported.
        #endregion

        #region Private Vars
        /// <summary>
        /// Private Vars
        /// </summary>
        private string _strWADFilename = string.Empty;
        private bool _bWADFileFound = false;
        private bool _bWADFileAllocated = false;
        private bool _bWADFileHeaderRead = false;
        private bool _bWADIsRottWad = false;

        private FileStream _WADFile = null;
        private BinaryReader _WADReader = null;

        private Rott2DWadHeader _WADHeader;
        private Rott2DWadDirectoryEntry[] _WADEntries;

        private bool _disposed = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DWADReader(string strWADFile)
        {
            this._strWADFilename = strWADFile; //initialize

            this._bWADFileAllocated = false;  
            this._bWADFileHeaderRead = false;
            this._bWADFileFound = false;

            this.OpenRottWad();   
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DWADReader()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.CloseRottWad();
                }

                this._disposed = true;
            }
        }
        #endregion

        #region Getters and Setters
        /// <summary>
        /// File name
        /// </summary>
        public string WADFilename
        {
            get { return this._strWADFilename; }
            private set { this._strWADFilename = value; }
        }

        /// <summary>
        /// Wad is found
        /// </summary>
        public bool WADFound
        {
            get { return this._bWADFileFound; }
            private set { this._bWADFileFound = value; }
        }

        /// <summary>
        /// Wad allocated by system
        /// </summary>
        public bool WADAllocated
        {
            get { return this._bWADFileAllocated; }
            private set { this._bWADFileAllocated = value; }
        }

        /// <summary>
        /// Valid ROTT WAD?
        /// </summary>
        public bool WADValid
        {
            get { return this._bWADIsRottWad; }
            private set { this._bWADIsRottWad = value; }
        }

        /// <summary>
        /// WAD Reader struct
        /// </summary>
        public Rott2DWadHeader WADHeader
        {
            get { return this._WADHeader; }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Open (Allocate) a ROTT WAD file
        /// </summary>
        private bool OpenRottWad()
        {
            this.WADFound = false;

            if (File.Exists(this._strWADFilename))
            {
                bool failedload = false;

                this.WADFound = true;
                
                try
                {
                    this._WADFile = File.OpenRead(this._strWADFilename);
                    this._WADReader = new BinaryReader(this._WADFile, System.Text.Encoding.ASCII);
                    this.WADAllocated = true;

                    if (!this._bWADFileHeaderRead)
                    {
                        this.ReadRottWadEntries();
                    }
                }
                catch (Exception ex)
                {
                    failedload = true;

                    System.Diagnostics.Debug.WriteLine("Error in OpenRottWad");
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }
                finally
                {
                    if (failedload)
                    {
                        this.CloseRottWad();
                    }
                }
            }

            return this._bWADFileFound;
        }

        /// <summary>
        /// Read Wad file LUMP's
        /// </summary>
        private bool ReadRottWadEntries()
        {
            //Entries can only be read when the file is allocated
            if (this.WADAllocated)
            {
                try
                {
                    //Create header for WAD
                    this._WADHeader = new Rott2DWadHeader();
                    this._WADHeader.IsWADFile = false;  //initialize

                    //Go to beginning of WAD file
                    this._WADFile.Seek(0, SeekOrigin.Begin);

                    //read first 4 bytes for the IWAD_ID
                    if (this._WADFile.Length != 0)
                    {
                        this._WADHeader.WadFileId = System.Text.Encoding.ASCII.GetString(this._WADReader.ReadBytes(4));
                    }

                    //read WAD header data
                    if (this._WADHeader.WadFileId.CompareTo(IWAD_ID) == 0)
                    {
                        this._WADHeader.IsWADFile = true;
                        this._WADHeader.NumEntries = BitConverter.ToInt32(this._WADReader.ReadBytes(4), 0);
                        this._WADHeader.DirOffset = BitConverter.ToInt32(this._WADReader.ReadBytes(4), 0);
                    }

                    if (this._WADHeader.IsWADFile)
                    {
                        //start reading WAD entries
                        this._WADEntries = new Rott2DWadDirectoryEntry[this._WADHeader.NumEntries];
                        this._WADFile.Seek(this._WADHeader.DirOffset, SeekOrigin.Begin);

                        for (int i = 0; i < this._WADHeader.NumEntries; i++)
                        {
                            this._WADEntries[i] = new Rott2DWadDirectoryEntry();
                            this._WADEntries[i].ID = i;    //Set our own ID
                            this._WADEntries[i].Offset = BitConverter.ToInt32(this._WADReader.ReadBytes(4), 0);
                            this._WADEntries[i].Size = BitConverter.ToInt32(this._WADReader.ReadBytes(4), 0);
                            //this._WADEntries[i].Name = System.Text.Encoding.ASCII.GetString(this._WADReader.ReadBytes(8)).ToUpper();
                            this._WADEntries[i].Name = System.Text.Encoding.ASCII.GetString(this._WADReader.ReadBytes(8)).Trim('\0').ToUpper();
                        }

                        //flag ready if this is a rott wad file
                        //the lump in a ROTT WAD file, always reads "WALLSTRT" by name and is a lump marker!
                        if (this._WADEntries.Length > 0)
                        {
                            Rott2DWadDirectoryEntry wde = this.GetRottWadEntryByIndex(0);

                            if (wde.Name == "WALLSTRT")
                            {
                                this._bWADFileHeaderRead = true;
                                this.WADValid = true;
                            }
                        }
                        else
                        {
                            this.CloseRottWad(); //not a ROTT WAD file!
                        }
                    }
                    else
                    {
                        this.CloseRottWad(); //close on fail?
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error in ReadRottWadEntries");
                    System.Diagnostics.Debug.Write(ex.ToString());

                    throw new Exception("Error in ReadRottWadEntries");                    
                }

            } //if end

            return WADHeader.IsWADFile;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Close (free) a ROTT WAD file
        /// </summary>
        public void CloseRottWad()
        {
            if ((this._bWADFileFound) && (this._bWADFileAllocated))
            {
                //close the binary reader
                if (this._WADReader != null)
                {
                    this._WADReader.Close();
                    this._WADReader = null;
                }

                //close the file
                if (this._WADFile != null)
                {
                    this._WADFile.Close();
                    this._WADFile = null;
                }

                //this._strWADFilename = string.Empty;
                this._bWADFileAllocated = false;
            }
        }

        /// <summary>
        /// Get byte array from lump name
        /// </summary>
        public byte[] GetWadLumpDataByName(string strSearchName)
        {
            Rott2DWadDirectoryEntry lump = this.GetWadEntryByName(strSearchName);
            byte[] data = null;

            if (lump.Size > 0)
            {
                this._WADFile.Seek(lump.Offset, SeekOrigin.Begin);
                data = this._WADReader.ReadBytes(lump.Size);
            }

            return data;
        }

        /// <summary>
        /// Get byte array from lump
        /// </summary>
        public byte[] GetRottWadLumpDataByLump(ref Rott2DWadDirectoryEntry lump)
        {
            byte[] data = null;

            if (lump.Size > 0)
            {
                this._WADFile.Seek(lump.Offset, SeekOrigin.Begin);
                data = this._WADReader.ReadBytes(lump.Size);
            }

            return data;
        }

        /// <summary>
        /// Get byte array from lump
        /// </summary>
        public byte[] GetRottWadLumpDataByLump(Rott2DWadDirectoryEntry lump)
        {
            byte[] data = null;

            if (lump.Size > 0)
            {
                this._WADFile.Seek(lump.Offset, SeekOrigin.Begin);
                data = this._WADReader.ReadBytes(lump.Size);
            }

            return data;
        }

        /// <summary>
        /// Get all WAD entries
        /// </summary>
        public Rott2DWadDirectoryEntry[] GetAllWadEntries()
        {
            return this._WADEntries;
        }

        /// <summary>
        /// Get WAD entry by Name
        /// </summary>
        public Rott2DWadDirectoryEntry GetWadEntryByName(string strSearchName)
        {
            Rott2DWadDirectoryEntry rweReturn = new Rott2DWadDirectoryEntry();
            rweReturn.Name = "EMPTY";
            rweReturn.Offset = 0;
            rweReturn.Size = 0;

            foreach (Rott2DWadDirectoryEntry rwe in this._WADEntries)
            {
                if (rwe.Name == strSearchName)
                {
                    rweReturn = rwe;
                    /*rweReturn.Name = rwe.Name;
                    rweReturn.Offset = rwe.Offset;
                    rweReturn.Size = rwe.Size;*/
                }
            }

            return rweReturn;
        }

        /// <summary>
        /// Get WAD entry by Index
        /// </summary>
        public Rott2DWadDirectoryEntry GetRottWadEntryByIndex(int iIndex)
        {
            return this._WADEntries[iIndex];
        }

        /// <summary>
        /// Get WAD index by Name
        /// </summary>
        public int GetRottWadIndexByName(string strSearchName)
        {
            int iPosition = -1;

            for (int pos = 0; pos < this._WADEntries.Length; pos++)
            {
                if (this._WADEntries[pos].Name == strSearchName)
                {
                    iPosition = pos;
                }
            }
            return iPosition;
        }

        /// <summary>
        /// Get WAD entry by file Offset position
        /// </summary>
        public Rott2DWadDirectoryEntry GetRottWadEntryByOffset(int intSearchOffset)
        {
            Rott2DWadDirectoryEntry rweFound = rweFound = new Rott2DWadDirectoryEntry(); ;
            rweFound.Name = "EMPTY";
            rweFound.Offset = 0;
            rweFound.Size = 0;

            foreach (Rott2DWadDirectoryEntry rwe in this._WADEntries)
            {
                if (rwe.Offset == intSearchOffset)
                {                    
                    rweFound = rwe;
                }
            }

            return rweFound;
        }

        /// <summary>
        /// Get WAD entry by file size
        /// </summary>
        public Rott2DWadDirectoryEntry GetRottWadEntryBySize(int intSearchSize)
        {
            Rott2DWadDirectoryEntry rweFound = new Rott2DWadDirectoryEntry();
            rweFound.Name = "EMPTY";
            rweFound.Offset = 0;
            rweFound.Size = 0;

            foreach (Rott2DWadDirectoryEntry rwe in this._WADEntries)
            {
                if (rwe.Size == intSearchSize)
                {
                    rweFound = rwe;
                }
            }

            return rweFound;
        }
        #endregion

    }
    #endregion

}