/*
 * ROTT2D
 * Unit: ROTT2D wave sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 28-01-2012
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

namespace ROTT2D.WAD.data
{

    /*
     * -- RIFF Wave specs  (from the time when Windows For Workgroups 3.1 was common) --
     * 
     * RIFF Wave Data fields and Data types:
     * -------------------------------------------------------------------
     * File description header      4 bytes (DWord)     The ASCII text string "RIFF" The procedure converiting 4-symbol string into dword is included in unit Wave. (0x52494646)
     * Size of file                 4 bytes (DWord)     The file size not including the "RIFF" description (4 bytes) and file description (4 bytes). This is file size - 8.
     * WAV description header       4 bytes (DWord)     The ASCII text string "WAVE". (0x57415645)
     * Format description header    4 bytes (DWord)     The ASCII text string "fmt ", the space char included. (0x666D7420)
     * Size of WAVE section chunck  4 bytes (DWord)     The size of the WAVE type format (2 bytes) + mono/stereo flag (2 bytes) + sample rate (4 bytes) + bytes per sec (4 bytes) + block alignment (2 bytes) + bits per sample (2 bytes). This is usually 16.
     * WAVE type format             2 bytes (Word)      Type of WAVE format. This is a PCM header = $01 (linear quntization). Other values indicates some forms of compression.
     * Number of channels           2 bytes (Word)      mono (0x01) or stereo (0x02). You may try more channels...
     * Samples per second           4 bytes (DWord)     The frequency of quantization (usually 44100 Hz, 22050 Hz, ...)
     * Bytes per second             4 bytes (DWord)     Speed of data stream = Number_of_channels * Samples_per_second * Bits_per_Sample / 8
     * Block alignment              2 bytes (Word)      Number of bytes in elementary quantization = Number_of_channels * Bits_per_Sample / 8
     * Bits per sample              2 bytes (Word)      Digits of quantization (usually 32, 24, 16, 8). I wonder if this is, for example, 5 ..?
     * Data description header      4 bytes (DWord)     The ASCII text string "data". (0x64617461)
     * Size of data                 4 bytes (DWord)     Number of bytes of data is included in the data section.
     * Data                         -?-                 Wave stream data itself. Be careful with format of stereo.
     *
     * RIFF Wave Compression types:
     * -------------------------------------------------------------------
     * 0        0x0000  Unknown
     * 1        0x0001 	PCM/uncompressed
     * 2        0x0002 	Microsoft ADPCM
     * 6        0x0006 	ITU G.711 a-law
     * 7        0x0007 	ITU G.711 u-law
     * 17       0x0011 	IMA ADPCM
     * 20       0x0016 	ITU G.723 ADPCM (Yamaha)
     * 49       0x0031 	GSM 6.10
     * 64       0x0040 	ITU G.721 ADPCM
     * 80       0x0050 	MPEG
     * 65,536   0xFFFF 	Experimental
     * 
     * Most common frequencies:
     * -------------------------------------------------------------------
     * 8000  Hz
     * 11025 Hz (Mostly DOS)
     * 16000 Hz
     * 22050 Hz
     * 32000 Hz
     * 44100 Hz (CD)
     * 48000 Hz (DVD)
     * 96000 Hz
     * 
     */


    #region RIFF Wave structures

    /// <summary>
    /// RIFF Wave file header
    /// </summary>
    public struct WaveFileHeader
    {
        public const string WAVE_HEADERCHUNKID = "RIFF";
        public const string WAVE_HEADERTYPE     = "WAVE";

        private string _chunkID;    //should be "RIFF"
        private int _chunkDataSize; //data size
        private string _riffType;   //should be "WAVE" for wave files. Can also be "AVI", ...

        #region Getters & Setters
        /// <summary>
        /// Chunk ID
        /// </summary>
        public string ChunkID
        {
            get { return this._chunkID.ToUpper(); }
            set { this._chunkID = value.ToUpper(); }
        }

        /// <summary>
        /// Chunck Data Size
        /// </summary>
        public int ChunkDataSize
        {
            get { return this._chunkDataSize; }
            set { this._chunkDataSize = value; }
        }

        /// <summary>
        /// RIFF (file) type
        /// </summary>
        public string RIFFType
        {
            get { return this._riffType.ToUpper(); }
            set { this._riffType = value.ToUpper(); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Default init Wave header
        /// </summary>
        public void defaultInitHeader(int dataSize, int fmtSize)
        {
            this.ChunkID = WAVE_HEADERCHUNKID;
            this.ChunkDataSize = 4 + (8 + fmtSize) + (8 + dataSize);  //total 20 + fmtSize + dataSize
            this.RIFFType = WAVE_HEADERTYPE;
        }
        #endregion

    }

    /// <summary>
    /// RIFF Wave file data chunk
    /// </summary>
    public struct WaveFileDataChunk
    {
        public const string WAVE_DATACHUNKID = "data";

        private string _chunkID;      //should be "data"
        private int _chunkDataSize;   //data size,  NumSamples * NumChannels * (BitsPerSample / 8)
        private byte[] _chunkRawData; //raw data stream (array of bytes)

        #region Indexers
        /// <summary>
        /// Indexer
        /// </summary>
        public byte this[int index]
        {
            get
            {
                return this._chunkRawData[index];
            }

            set
            {
                this._chunkRawData[index] = value;
            }
        }
        #endregion

        #region Getters & Setters
        /// <summary>
        /// Chunk ID
        /// </summary>
        public string ChunkID
        {
            get { return this._chunkID.ToLower(); }
            set { this._chunkID = value.ToLower(); }
        }

        /// <summary>
        /// Chunck Data Size
        /// </summary>
        public int ChunkDataSize
        {
            get { return this._chunkDataSize; }
            set {
                this._chunkDataSize = value;
                this._chunkRawData = new byte[this._chunkDataSize]; //init the raw data stream                
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Default init Wave data chunk
        /// </summary>
        public void defaultInitData(int dataSize, ref byte[] rawdata)
        {
            this.ChunkID = WAVE_DATACHUNKID;
            this.ChunkDataSize = dataSize;
            this.setData(ref rawdata);
        }

        /// <summary>
        /// Assign a full data stream
        /// </summary>
        public void setData(ref byte[] rawData)
        {
            this._chunkRawData = (byte[])rawData.Clone();
        }

        /// <summary>
        /// Retrieve full data stream
        /// </summary>
        public byte[] getData()
        {
            return this._chunkRawData;
        }
        #endregion 

    }

    /// <summary>
    /// RIFF Wave fmt chunk
    /// </summary>
    public struct WaveFileFmtChunk
    {
        public const string WAVE_FMTCHUNKID = "fmt ";

        public const int WAVE_MONO          = 0x01;  //1
        public const int WAVE_STEREO        = 0x02;  //2

        public const int WAVE_SIZE          = 0x10;  //16
        public const int WAVE_PCM           = 0x10;  //16
        public const int WAVE_AUDIOFORMAT   = 0x01;  //PCM
        public const int WAVE_BITSPERSAMPLE = 0x08;  //this is default 16 (0x10), but ROTT uses 8 (0x08) bits per sample

        private string _subChunk1ID;             //should be "fmt "
        private int _subChunk1Size;              //should be 0x10 (16) for uncompressed PCM
        private short _audioFormat;              //should be 0x01 (1) for PCM
        private short _numChannels;              //should be 0x01 (1) for Mono, 0x02 (2) for Stereo
        private int _sampleRate;                 //default 11025 for ROTT sound samples
        private int _avgBytesPerSecond;          //(SampleRate * BlockAlign) or (SampleRate * NumChannels * (SignificantBitsPerSample / 8))
        private short _blockAlign;               //(SignificantBitsPerSample / 8 * NumChannels) or (NumChannels * (BitsPerSample / 8))
        private short _significantBitsPerSample; //default 0x10 (16). The value specifies the number of bits used to define each sample. This value is usually 8, 16, 24 or 32. 
        private int _extraFormatBytes;           //Not needed for uncompressed PCM

        #region Getters & Setters
        /// <summary>
        /// Sub Chunk (1) ID
        /// </summary>
        public string SubChunkID
        {
            get { return this._subChunk1ID.ToLower(); }
            set { this._subChunk1ID = value.ToLower(); }
        }

        /// <summary>
        /// Sub Chunk (1) Size
        /// </summary>
        public int SubChunkSize
        {
            get { return this._subChunk1Size; }
            set { this._subChunk1Size = value; }
        }

        /// <summary>
        /// Audio format
        /// </summary>
        public short AudioFormat
        {
            get { return this._audioFormat; }
            set { this._audioFormat = value; }
        }

        /// <summary>
        /// Number of channels
        /// </summary>
        public short NumberOfChannels
        {
            get { return this._numChannels; }
            set { this._numChannels = value; }
        }

        /// <summary>
        /// Sample rate
        /// </summary>
        public int SampleRate
        {
            get { return this._sampleRate; }
            set { this._sampleRate = value; }
        }

        /// <summary>
        /// Average bytes per sec
        /// </summary>
        public int AverageBytesPerSecond
        {
            get { return this._avgBytesPerSecond; }
            //set { this._avgBytesPerSecond = value; }
        }

        /// <summary>
        /// Block align
        /// </summary>
        public short BlockAlign
        {
            get { return this._blockAlign; }
            //set { this._blockAlign = value; }
        }

        /// <summary>
        /// Bits Per Sample
        /// </summary>
        public short BitsPerSample
        {
            get { return this._significantBitsPerSample; }
            set { this._significantBitsPerSample = value; }
        }

        /// <summary>
        /// Extra format bytes
        /// </summary>
        public int ExtraFormatBytes
        {
            get { return this._extraFormatBytes; }
            set { this._extraFormatBytes = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Default init Wave fmt chunk
        /// </summary>
        public void defaultInitFMT(short channels = WAVE_MONO, short bitsPerSamples = WAVE_BITSPERSAMPLE, int sampleRate = 11025)
        {
            this.SubChunkID = WAVE_FMTCHUNKID;        //default for RIFF Wave
            this.SubChunkSize = WAVE_SIZE;            //default for RIFF Wave
            this.AudioFormat = WAVE_AUDIOFORMAT; 
            this.NumberOfChannels = channels;        
            this.SampleRate = sampleRate;                 
            this.BitsPerSample = bitsPerSamples;   
            this.ExtraFormatBytes = 0;

            this.calcWaveData();
        }

        /// <summary>
        /// Calculate the Avarage bytes and block align
        /// </summary>
        public void calcWaveData()
        {
            this._avgBytesPerSecond = this._sampleRate * this._numChannels * (this._significantBitsPerSample / 8);
            this._blockAlign = (short)(this._numChannels * (this._significantBitsPerSample / 8));
        }
        #endregion

    }

    #endregion

    #region RIFF Wave class (uncompressed PCM only)
    /// <summary>
    /// ROTT2D Class to load and store wave sound data
    /// </summary>
    public sealed class Rott2DWave : IRott2DWave, IDisposable
    {

        /*
         * Remarks:
         * A wave file (in case of ROTT) doesn't inherit from the class Rott2DSoundLump.
         * 
         * This is because a voc file (Rott2DVoc) is actually lump data stored inside a ROTT WAD file
         * and thus inherits from Rott2DSoundLump. ROTT doesn't have any wave data in it's WAD files,          
         * therefore is this class not bound to a lump. This class is only used to convert a voc to a wave,
         * so we can playback the sound samples with any compatible sound engine (Even Windows API Call's).
         * 
         */


        #region Public consts
        public const int WAVE_HEADER_SIZE = 44;   //every byte before the actual wave data is total 44 bytes (header, ftm, data).
        public const int DEFAULT_SAMPLE_RATE = 11025;  //default for all ROTT sound samples
        #endregion

        #region Private vars
        private string _waveName;                 //our name ID
        private WaveFileHeader _waveHeader;       //header
        private WaveFileFmtChunk _waveFmtChunk;   //fmt chunk
        private WaveFileDataChunk _waveDataChunk; //data chunk

        private byte[] _waveMemoryData = null;   //full wave file in memory
        private bool _waveReady = false;
        #endregion

        #region Constructors
        public Rott2DWave(byte[] data)
        {
            this.isReady = false;
            this.Name = string.Empty;

            //ready for default ROTT sounds
            this.defaultInitWave(WaveFileFmtChunk.WAVE_MONO, DEFAULT_SAMPLE_RATE, WaveFileFmtChunk.WAVE_BITSPERSAMPLE, data);

            //create memory buffer
            if (this.isReady)
                this.buildMemoryWave();
        }

        public Rott2DWave(string name, byte[] data)
        {
            this.isReady = false;
            this.Name = name;

            //ready for default ROTT sounds
            this.defaultInitWave(WaveFileFmtChunk.WAVE_MONO, DEFAULT_SAMPLE_RATE, WaveFileFmtChunk.WAVE_BITSPERSAMPLE, data);

            //create memory buffer
            if (this.isReady)
                this.buildMemoryWave();
        }


        public Rott2DWave(short channels, int samplerate, short bitspersample, byte[] data)
        {
            this.isReady = false;
            this.Name = string.Empty;

            //ready for custom ROTT sounds
            this.defaultInitWave(channels, samplerate, bitspersample, data);
            
            //create memory buffer
            if (this.isReady)
                this.buildMemoryWave();
        }

        public Rott2DWave(string name, short channels, int samplerate, short bitspersample, byte[] data)
        {
            this.isReady = false;
            this.Name = name;

            //ready for custom ROTT sounds
            this.defaultInitWave(channels, samplerate, bitspersample, data);

            //create memory buffer
            if (this.isReady)
                this.buildMemoryWave();
        }
        #endregion
                     
        #region Destructors
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DWave()
        {
            this.Dispose();
        }

        /// <summary>
        /// Dispose memory
        /// </summary>
        public void Dispose()
        {
            Dispose(true); //dispose the class

            GC.SuppressFinalize(this); //call garbage collector
        }

        /// <summary>
        /// Dispose memory
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.isReady = false;

                if (this._waveMemoryData != null)
                    this._waveMemoryData = null;
            }
        }
        #endregion

        #region Getters & setters
        /// <summary>
        /// Ready wave
        /// </summary>
        public bool isReady
        {
            get { return this._waveReady; }
            private set { this._waveReady = value; }
        }

        /// <summary>
        /// Name ID
        /// </summary>
        public string Name
        {
            get { return this._waveName; }
            set { this._waveName = value; }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Init method
        /// </summary>
        private void defaultInitWave(short channels, int samplerate, short bitspersample, byte[] data)
        {
            //fmt chunk
            this._waveFmtChunk = new WaveFileFmtChunk();
            this._waveFmtChunk.defaultInitFMT(channels, bitspersample, samplerate);

            //data chunk
            this._waveDataChunk = new WaveFileDataChunk();
            this._waveDataChunk.defaultInitData(data.Length, ref data);

            //init header (header must be done last, because we need the size of both fmt and data!)
            this._waveHeader = new WaveFileHeader();
            this._waveHeader.defaultInitHeader(this._waveDataChunk.ChunkDataSize, this._waveFmtChunk.SubChunkSize);

            //flag as ready
            this.isReady = true;
        }

        /// <summary>
        /// Write small part to the memory buffer stream
        /// </summary>
        private void toMemoryStream(byte[] data, ref int offset)
        {
            if ((this.isReady) && (this._waveMemoryData != null))
                foreach (byte b in data)
                {
                    this._waveMemoryData[offset] = b;
                    offset++;
                }
        }

        /// <summary>
        /// String to byte array
        /// </summary>
        private byte[] toBytes(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        /// <summary>
        /// Integer to byte array
        /// </summary>
        private byte[] toBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Short to byte array
        /// </summary>
        private byte[] toBytes(short value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Construct the full wave "file" in memory buffer
        /// </summary>
        private void buildMemoryWave()
        {

            /*
             * This methods builds the whole wave file in memory as byte stream.
             * 
             */

            if ((this.isReady) && (this._waveMemoryData == null))
            {
                this._waveMemoryData = new byte[WAVE_HEADER_SIZE + this._waveDataChunk.ChunkDataSize];  //init memory stream
                int iWaveOffset = 0;  //offset in stream for building

                /* -- the header -- */
                //Header ID
                byte[] riffID = this.toBytes(this._waveHeader.ChunkID);
                this.toMemoryStream(riffID, ref iWaveOffset);

                //Header Chunk Size
                byte[] riffChunkSize = this.toBytes(this._waveHeader.ChunkDataSize);
                this.toMemoryStream(riffChunkSize, ref iWaveOffset);

                //Header Data Type
                byte[] riffType = this.toBytes(this._waveHeader.RIFFType);
                this.toMemoryStream(riffType, ref iWaveOffset);



                /* -- the fmt chunk -- */
                //fmt Chunk ID
                byte[] subchunk1ID = this.toBytes(this._waveFmtChunk.SubChunkID);
                this.toMemoryStream(subchunk1ID, ref iWaveOffset);

                //fmt Chunk Size
                byte[] subchunk1Size = this.toBytes(this._waveFmtChunk.SubChunkSize);
                this.toMemoryStream(subchunk1Size, ref iWaveOffset);

                //fmt Audio Format (PCM)
                byte[] audioFormat = this.toBytes(this._waveFmtChunk.AudioFormat);
                this.toMemoryStream(audioFormat, ref iWaveOffset);

                //fmt No.Channels
                byte[] numChannels = this.toBytes(this._waveFmtChunk.NumberOfChannels);
                this.toMemoryStream(numChannels, ref iWaveOffset);

                //fmt Sample Rate
                byte[] sampleRate = this.toBytes(this._waveFmtChunk.SampleRate);
                this.toMemoryStream(sampleRate, ref iWaveOffset);

                //fmt Average bytes
                byte[] avgBytes = this.toBytes(this._waveFmtChunk.AverageBytesPerSecond);
                this.toMemoryStream(avgBytes, ref iWaveOffset);

                //fmt Block Align
                byte[] blockAlign = this.toBytes(this._waveFmtChunk.BlockAlign);
                this.toMemoryStream(blockAlign, ref iWaveOffset);

                //fmt Bits Per Sample
                byte[] bitsPerSample = this.toBytes(this._waveFmtChunk.BitsPerSample);
                this.toMemoryStream(bitsPerSample, ref iWaveOffset);


                
                /* -- the data chunk -- */
                //Chunk Data ID
                byte[] dataChunk2ID = this.toBytes(this._waveDataChunk.ChunkID);
                this.toMemoryStream(dataChunk2ID, ref iWaveOffset);

                //Chunk Data Size
                byte[] dataChunk2Size = this.toBytes(this._waveDataChunk.ChunkDataSize);
                this.toMemoryStream(dataChunk2Size, ref iWaveOffset);


                //Actual sound data bytes (always on the outer ending of a PCM RIFF Wave)
                //WaveFileDataChunk already contains a function to return the sound data as a byte array
                this.toMemoryStream(this._waveDataChunk.getData(), ref iWaveOffset);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// return stream from memory buffer
        /// </summary>
        public byte[] getWaveStream()
        {
            if ((this.isReady) && (this._waveMemoryData == null))
                return this._waveMemoryData;
            else 
                return null;            
        }

        /// <summary>
        /// Wave data length
        /// </summary>
        /// <returns></returns>
        public int getWaveLength()
        { 
            return (this._waveMemoryData != null) ? this._waveMemoryData.Length : 0;
        }

        /// <summary>
        /// output memory buffer wave to file on disk
        /// </summary>
        public bool saveWave(string filename)
        {
            bool isSuccess = false;

            if ((this.isReady) && (this._waveMemoryData != null))
            {
                if (this.getWaveLength() > 0)
                {
                    using (FileStream waveFile = new FileStream(filename.ToLower(), FileMode.Create))
                    {
                        using (BinaryWriter waveBinary = new BinaryWriter(waveFile))
                        {
                            foreach (byte b in this._waveMemoryData)
                                waveBinary.BaseStream.WriteByte(b);                            

                            waveBinary.Flush();  //force flush so we are sure eveything is written to disk!
                        }
                    }

                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "wave_t";
        }
        #endregion

    }
    #endregion

}
