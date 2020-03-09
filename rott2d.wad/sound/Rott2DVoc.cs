/*
 * ROTT2D
 * Unit: ROTT2D voc sealed Class
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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ROTT2D.WAD.data
{

    #region voc structures
    /// <summary>
    /// Creative voc block types (compression method)
    /// </summary>
    public enum VocDataType : short
    {
        /*
         * 0x00  8 bits unsigned PCM
         * 0x01  4 bits to 8 bits Creative ADPCM
         * 0x02  3 bits to 8 bits Creative ADPCM (AKA 2.6 bits)
         * 0x03  2 bits to 8 bits Creative ADPCM
         * 0x04  16 bits signed PCM
         * 0x06  alaw
         * 0x07  ulaw
         * 0x0200  4 bits to 16 bits Creative ADPCM (only valid in block nine type)
         * 
         */

        VocPCM8Bit       = 0x0,    //mono uncompressed
        VocADPCM4TO8Bit  = 0x1,    //Creative ADPCM
        VocADPCM3TO8Bit  = 0x2,    //Creative ADPCM, 2.6 bits
        VocADPCM2TO8Bit  = 0x3,    //Creative ADPCM
        VocPCM16Bit      = 0x4,    //stereo uncompressed
        VocALAW          = 0x06,   //alaw compressed
        VocULAW          = 0x07,   //ulaw compressed
        VocADPCM4TO16Bit = 0x0200  //Creative ADPCM, only valid in block "nine" type 
    }

    /// <summary>
    /// Creative Labs voc file header
    /// </summary>
    public struct Rott2DVocHeader
    {
        /*
         * bytes 0-18   Identifier string containing: Creative Voice File
         * byte  19     0x1A (EOF). This is belived to be used to abort printing of file
         * bytes 20-21  Total size of this main header (usually 0x001A)
         * bytes 22-23  Version number, calculated as (major<<8)|minor
         *                major is usually 0x01
         *                minor is usually 0x0A or 0x14
         * bytes 24-25  Validity check. This must be equal to ~version + 0x1234
         * 
         */

        /// <summary>
        /// Public constants
        /// </summary>
        public const int    SOUND_VOC_HEADER_IDSIZE = 19;                      //header ID size of a voc file
        public const string SOUND_VOC_HEADER_ID     = "Creative Voice File";   //header ID

        /// <summary>
        /// Private vars
        /// </summary>
        private string _description;  //Length of 18 bytes string containing "Creative Voice File"
        private ushort _headerSize;   //Size of a voc header (value should be 26 in most cases)
        private ushort _version;      //Version of the voc   (value should be 266 in most cases)
        private ushort _IDCode;       //voc IDCode (should be Version + 0x1234)

        private bool _correct;        //my own flag to ensure this is a correct Creative voc file!

        /// <summary>
        /// Description, should always be "Creative Voice File"
        /// </summary>
        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }

        /// <summary>
        /// Total HeaderSize
        /// </summary>
        public ushort HeaderSize
        {
            get { return this._headerSize; }
            set { this._headerSize = value; }
        }

        /// <summary>
        /// VOC file version
        /// </summary>
        public ushort Version
        {
            get { return this._version; }
            set { this._version = value; }
        }

        /// <summary>
        /// VOC file IDCode
        /// </summary>
        public ushort IDCode
        {
            get { return this._IDCode; }
            set { this._IDCode = value; }
        }

        /// <summary>
        /// Header is not corrupted?
        /// </summary>
        public bool isCorrect
        {
            get { return this._correct; }
            set { this._correct = value; }
        }
    }

    /// <summary>
    /// Creative Labs voc file general block "header"
    /// </summary>
    public struct Rott2DVocBlock
    {
        /* 
         * All the different data blocks begin with a common header:
         *      byte  0      block type
         *      bytes 1-3    block size (NOT including this common header)
         *      
         */

        /// <summary>
        /// Private vars
        /// </summary>
        private int _length;      //block length
        private byte _blockType;  //defines the type of a block

        /// <summary>
        /// Length
        /// </summary>
        public int Length
        {
            get { return this._length; }
            set { this._length = value; }
        }

        /// <summary>
        /// Length
        /// </summary>
        public byte BlockType
        {
            get { return this._blockType; }
            set { this._blockType = value; }
        }
    }

    /// <summary>
    /// Creative Labs voc file block "One" type
    /// </summary>
    public struct Rott2DVocBlockOneHeader
    {
        /*
         * byte  0      frequency divisor
         * byte  1      codec id
         * bytes 2..n   the audio data
         * 
         * The sample rate is defined as 1000000/(256 - frequency_divisor)
         * 
         */

        /// <summary>
        /// Private vars
        /// </summary>
        private int _len;
        private uint _tc;
        private byte _pack;

        /// <summary>
        /// Len
        /// </summary>
        public int Len
        {
            get { return this._len; }
            set { this._len = value; }
        }

        /// <summary>
        /// Tc
        /// </summary>
        public uint Tc
        {
            get { return this._tc; }
            set { this._tc = value; }
        }

        /// <summary>
        /// Pack
        /// </summary>
        public byte Pack
        {
            get { return this._pack; }
            set { this._pack = value; }
        }
    }

    /// <summary>
    /// Creative Labs voc file block "Nine" type
    /// </summary>
    public struct Rott2DVocBlockNineHeader
    {
        /* 
         * This block type is probably only valid in version 1.20 (0x0114) or greater files.
         * bytes 0-3    sample rate
         * byte  4      bits per sample
         * byte  5      channels number
         * bytes 6-7    codec_id
         * bytes 8-11   reserved
         * bytes 12..n  the audio data
         * 
         */

        /// <summary>
        /// Private vars
        /// </summary>
        private int _samplerate;
        private byte _bitspersample;
        private byte _numchannels;
        private short _codec;
        private byte _reserved0;
        private byte _reserved1;
        private byte _reserved2;

        /// <summary>
        /// SampleRate
        /// </summary>
        public int SampleRate
        {
            get { return this._samplerate; }
            set { this._samplerate = value; }
        }

        /// <summary>
        /// BitsPerSample
        /// </summary>
        public byte BitsPerSample
        {
            get { return this._bitspersample; }
            set { this._bitspersample = value; }
        }

        /// <summary>
        /// Channels
        /// </summary>
        public byte Channels
        {
            get { return this._numchannels; }
            set { this._numchannels = value; }
        }

        /// <summary>
        /// Codec
        /// </summary>
        public short Codec
        {
            get { return this._codec; }
            set { this._codec = value; }
        }

        /// <summary>
        /// Reserved0
        /// </summary>
        public byte Reserved0
        {
            get { return this._reserved0; }
            set { this._reserved0 = value; }
        }

        /// <summary>
        /// Reserved1
        /// </summary>
        public byte Reserved1
        {
            get { return this._reserved1; }
            set { this._reserved1 = value; }
        }

        /// <summary>
        /// Reserved2
        /// </summary>
        public byte Reserved2
        {
            get { return this._reserved2; }
            set { this._reserved2 = value; }
        }
    }

    /// <summary>
    /// Creative Labs voc data file block (this contains the sound data!)
    /// </summary>
    public struct Rott2DVocBlockData
    {
        /// <summary>
        /// Private vars
        /// </summary>
        private int _pcmtype;
        private int _samplerate;
        private byte[] _sample;
        private int _bits;
        private byte _channel;  //0 = mono, 1 = stereo
        
        /// <summary>
        /// ReadOnly Indexer to get a sample byte from the array
        /// </summary>
        public byte this[ushort sampleIndex]
        {
            get
            {
                return this._sample[sampleIndex];
            }
            set
            {
                this._sample[sampleIndex] = value;
            }
        }

        /// <summary>
        /// init sample array size
        /// </summary>
        public void initSampleData(int size)
        {
            if (this._sample == null)
            {
                this._sample = new byte[size];
            }
        }

        /// <summary>
        /// PCMType
        /// </summary>
        public int PCMType
        {
            get { return this._pcmtype; }
            set { this._pcmtype = value; }
        }

        /// <summary>
        /// SampleRate
        /// </summary>
        public int SampleRate
        {
            get { return this._samplerate; }
            set { this._samplerate = value; }
        }


        /// <summary>
        /// Sample
        /// </summary>
        public byte[] Sample
        {
            get { return this._sample; }
            set { this._sample = value; }
        }

        /// <summary>
        /// Bits
        /// </summary>
        public int Bits
        {
            get { return this._bits; }
            set { this._bits = value; }
        }

        /// <summary>
        /// ChannelType
        /// </summary>
        public byte ChannelType
        {
            get { return this._channel; }
            set { this._channel = value; }
        }
    }
    #endregion

    #region voc class (sound samples)
    /// <summary>
    /// ROTT2D Class to load sound data from lump
    /// </summary>
    public sealed class Rott2DVoc : Rott2DSoundLump, IRott2DVoc
    {
        /*
         * ROTT sound data lump's, are fully stored VOC files into the WAD file.
         * 
         * Original Doom uses stripped wave files for it's sound samples.
         * 
         * When i was in search for the sound samples used in ROTT,
         * it turned out that the sound lumps where full voc files.
         * 
         * These lump types are not stored with a compression algorithm,
         * probably because a VOC file it self has a compression algorithm already used.
         * 
         */

        #region Public consts
        /// <summary>
        /// Public consts
        /// </summary>
        public const int DEFAULT_VOC_SAMPLERATE = 11025;
        public const int DEFAULT_VOC_CHANNELS = 1;
        public const int DEFAULT_VOC_BITS = 8;

        public const byte VOC_SAMPLE_8BIT = 0x0;
        public const byte VOC_SAMPLE_16BIT = 0x4;

        public const int VOC_MONO = 0x0;
        public const int VOC_STEREO = 0x1;


        #endregion

        #region Private vars
        private List<Rott2DVocBlockData> blocks = null;
        private MemoryStream _vocSoundStream = null;
        private Rott2DVocHeader _vocHeader;
        private byte[] _vocSoundData = null;

        private int _sampleRate = 0;
        private int _channels = 0;
        private int _bits = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DVoc(ref byte[] vocLumpData)
        {
            this.isReady = false;
            this._rawData = vocLumpData;

            this._vocHeader = new Rott2DVocHeader();

            //read voc file header
            if (this.ReadVocHeader())
            {
                //read voc body data
                this.blocks = new List<Rott2DVocBlockData>();

                this.GenerateVocData();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Rott2DVoc(string name, ref byte[] vocLumpData)
        {
            this.isReady = false;
            this.Name = name;
            this._rawData = vocLumpData;

            this._vocHeader = new Rott2DVocHeader();

            //read voc file header
            if (this.ReadVocHeader())
            {
                //read voc body data
                this.blocks = new List<Rott2DVocBlockData>();

                this.GenerateVocData();
            }
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DVoc()
        {
            this.Dispose(false);
        }


        /// <summary>
        /// Dispose method
        /// </summary>
        public new void Dispose()
        {
            //de-allocate sample blocks from memory
            if (blocks != null)
            {
                if (blocks.Count > 0)
                {
                    blocks.Clear();
                    blocks = null;
                }
            }

            //de-allocate stream from memory
            if (this._vocSoundStream != null)
            {
                this._vocSoundStream.Dispose();
                this._vocSoundStream = null;
            }

            GC.SuppressFinalize(this); //call garbage collector

            base.Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Read the voc data header
        /// </summary>
        private bool ReadVocHeader()
        {
            //init with default values
            this._vocHeader.Description = string.Empty;
            this._vocHeader.HeaderSize = 0;
            this._vocHeader.Version = 0;
            this._vocHeader.IDCode = 0;
            this._vocHeader.isCorrect = true;

            //read "Creative Voice File"
            System.Text.Encoding encoding = System.Text.ASCIIEncoding.ASCII;
            this._vocHeader.Description = encoding.GetString(this._rawData, 0, Rott2DVocHeader.SOUND_VOC_HEADER_IDSIZE);   //"Creative Voice File" is first 19 bytes

            //parse header data "Creative Voice File"
            if (this._vocHeader.Description.ToLower() == Rott2DVocHeader.SOUND_VOC_HEADER_ID.ToLower())   //check the first 19 bytes
            {
                if (this._rawData[20] == 0x1A)  //value 0x1A is byte 19. If incorrect, we don't have a voc! 
                {
                    //Read first bytes
                    this._vocHeader.HeaderSize = BitConverter.ToUInt16(this._rawData, 20); //read header size, usally 0x001A (= 26)
                    this._vocHeader.Version = BitConverter.ToUInt16(this._rawData, 22);    //read voc version
                    this._vocHeader.IDCode = BitConverter.ToUInt16(this._rawData, 24);     //read voc ID-code

                    //header version checking
                    if (this._vocHeader.Version != 0x010A && this._vocHeader.Version != 0x0114)
                    {
                        /* 
                         * 0x010A = 166
                         * 0x0114 = 276
                         * 
                         * Version number, calculated as (major<<8)|minor
                         * major is usually 0x01 (1)
                         * minor is usually 0x0A (10) or 0x14 (20)
                         */

                        this._vocHeader.isCorrect = false;  //woops, no supported voc file version!

                        //Supports version 110 and 120
                        throw new Exception("readVocHeader :: VOC file version not supported!");
                    }

                   // CRC check (checksum) 
                    /*if (_vocHeader.Version + 0x1234 != _vocHeader.IDCode)
                    {
                        _vocHeader.Correct = false;  //woops, CRC is incorrect!

                        //   throw new Exception("readVocHeader :: VOC file CRC failed!");
                    }*/

                    // Header seems to be okay
                   //_vocHeader.Correct = true;  
                }
            }

            return this._vocHeader.isCorrect;
        }

        /// <summary>
        /// Extract the voc data into one large stream
        /// (This can be written to wave or back to a voc.)
        /// </summary>
        private void GenerateVocData()
        {
            if ((this._vocHeader.HeaderSize != 0) && (this.GetDataSize() > this._vocHeader.HeaderSize))
            {
                bool _EOF = false;   //End Of File
                int startPos = this._vocHeader.HeaderSize;    //skip header data

                while (startPos < this.GetDataSize() && !_EOF)
                {
                    //start reading first block
                    Rott2DVocBlock _vocBlock = new Rott2DVocBlock();
                    _vocBlock.BlockType = this._rawData[startPos++]; //blocktype

                    //calculate block offset
                    long Offset = (this.GetDataSize() - startPos);

                    //in case of blocktype 0, terminate reading further
                    if (_vocBlock.BlockType == 0 || (Offset < 10))
                    {
                        break;
                    }

                    //else, go on, calculate the block Length
                    ulong blockLength = (BitConverter.ToUInt64(this._rawData, startPos) & 0x00FFFFFF);  //ToUInt64 = 8 bytes
                    _vocBlock.Length = (int)blockLength; //set length

                    startPos = startPos + 3; //current startpos + 3 bytes offset

                    //get our block types
                    //--> we only support block 0, 1 and 9 voc file code blocks!
                    switch (_vocBlock.BlockType)
                    {
                        case 0:
                            break;
                        case 1:
                            this.ParseBlockTypeOne(ref startPos, _vocBlock.Length);  //Type One
                            break;
                        case 9:
                            this.ParseBlockTypeNine(ref startPos, _vocBlock.Length); //Type Nine
                            break;
                        default:
                            {
                                //throw new Exception("generateVocData :: Unsupported block type");
                                startPos = startPos + _vocBlock.Length;  //skip data block, goto next block
                                continue;
                            }
                    } //switch
                } //while


                //buffer all extracted data blocks to a stream
                if (blocks.Count > 0)
                {
                    if (this._vocSoundStream == null)
                    {
                        this._vocSoundStream = new MemoryStream();
                    }

                    //convert all samples to one large memorystream block
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        this._vocSoundStream.Write(blocks[i].Sample, 0, blocks[i].Sample.Length);
                    }

                    //fill private buffer as byte array
                    this._vocSoundData = new byte[this._vocSoundStream.GetBuffer().Length];
                    this._vocSoundData = this._vocSoundStream.GetBuffer();

                    //ready !
                    this.isReady = true;
                }

            }
        }


        /// <summary>
        /// block type one
        /// </summary>
        private void ParseBlockTypeOne(ref int pos, int len)
        {
            Rott2DVocBlockOneHeader _blockheader = new Rott2DVocBlockOneHeader();
            _blockheader.Len = len;
            _blockheader.Tc = (uint)(this._rawData[pos] << 8);
            _blockheader.Pack = this._rawData[pos = pos + 1];

            Rott2DVocBlockData _block = new Rott2DVocBlockData();
            _block.SampleRate = (int)((ulong)256000000L / (65536 - _blockheader.Tc));
            _block.PCMType = _blockheader.Pack & 0xFF;
            _block.Bits = 8;
            _block.ChannelType = VOC_MONO; //mono tone only

            //set general settings
            this._sampleRate = _block.SampleRate;
            this._channels = 1;
            this._bits = _block.Bits;

            //fill sound data buffer (this is the real sound data)
            _block.initSampleData(len);

            ushort samplePos = 0;
            int datarest = this._rawData.Length - this._vocHeader.HeaderSize - len -1;  //calculate other rubbish to be removed

            for(int block = 0; block < len; block++)
            {
                _block[samplePos] = this._rawData[(this._vocHeader.HeaderSize + datarest) + block];
                //_block[samplePos] = this._rawData[block + this._vocHeader.HeaderSize + 4];
                //_block[samplePos] = this._rawData[this._vocHeader.HeaderSize + block];
                //_block[samplePos] = this._rawData[block];
                samplePos++;
            }

            //add sound data to generics list
            this.blocks.Add(_block);
        }

        /// <summary>
        /// block type nine
        /// </summary>
        private void ParseBlockTypeNine(ref int pos, int len)
        {
            Rott2DVocBlockNineHeader _blockheader = new Rott2DVocBlockNineHeader();
            _blockheader.SampleRate = BitConverter.ToInt32(this._rawData, pos);  //Int32 (int)
            _blockheader.BitsPerSample = this._rawData[(pos = pos + sizeof(int))];       //Byte
            _blockheader.Channels = this._rawData[(pos = pos + sizeof(byte))];            //Byte
            _blockheader.Codec = BitConverter.ToInt16(this._rawData, (pos = pos + sizeof(ushort)));           //Int16 (short)
            _blockheader.Reserved0 = this._rawData[pos = pos + sizeof(byte)];           //Byte
            _blockheader.Reserved1 = this._rawData[pos = pos + sizeof(byte)];           //Byte
            _blockheader.Reserved2 = this._rawData[pos = pos + sizeof(byte)];           //Byte

            Rott2DVocBlockData _block = new Rott2DVocBlockData();
            _block.SampleRate = _blockheader.SampleRate;
            _block.PCMType = 0;
            _block.Bits = _blockheader.BitsPerSample;

            //set general settings
            this._sampleRate = _block.SampleRate;
            this._channels = _blockheader.Channels;
            this._bits = _block.Bits;

            if (_blockheader.Channels > 1 && _blockheader.Codec == VOC_SAMPLE_16BIT)
            {
                _block.ChannelType = VOC_STEREO; //16 bit sample = stereo
            }
            else if (_blockheader.Channels == 1 && _blockheader.Codec == VOC_SAMPLE_8BIT)
            {
                _block.ChannelType = VOC_MONO;  //8 bit sample = mono
            }
            else
            {
                throw new Exception("ParseBlockTypeNine :: BlockNine invalid voc block");
            }

            //fill sound data buffer (-> this is the "real" sound data)
            _block.initSampleData(len);

            ushort samplePos = 0; //block index
            int datarest = this._rawData.Length - this._vocHeader.HeaderSize - len -1;  //calculate other rubbish to be removed

            for (int block = 0; block < len; block++)
            {
                _block[samplePos] = this._rawData[(this._vocHeader.HeaderSize + datarest) + block];
                //_block[samplePos] = this._rawData[block + this._vocHeader.HeaderSize + 4];
                //_block[samplePos] = this._rawData[this._vocHeader.HeaderSize + block];
                //_block[samplePos] = this._rawData[block];
                samplePos++;
            }

            //add sound data to generics list
            this.blocks.Add(_block);
        }

        /// <summary>
        /// Export the voc lump to a voc file
        /// </summary>
        public void ExportVocToFile(string filename)
        {
            // plain raw bytes dump to disk
            if (this._rawData != null)
            {
                using (Rott2DLumpWriter exportVoc = new Rott2DLumpWriter(ref this._rawData))
                {
                    this.isReady = exportVoc.ExportLumpToFile(filename, Rott2DLumpWriter.VOC_LUMP_EXT);
                }
            }
        }

        /// <summary>
        /// Export the voc lump to a wav file
        /// I implemented this because almost any OS or Sound API can play out of the box (RIFF) Wave files.
        /// </summary>
        public bool ExportVocToWaveFile(string filename)
        {
            bool isVocDone = false;

            if (this._vocSoundStream != null)
            {
                //create wave file from voc data
                /*WaveFile wav = new WaveFile(DEFAULT_SOUND_CHANNELS, DEFAULT_SOUND_BITS, DEFAULT_SOUND_SAMPLERATE);
                wav.SetData(this._vocSoundStream.GetBuffer(), this._vocSoundStream.GetBuffer().Length);
                wav.WriteFile(filename);*/

                //create wave file from voc data
                //Rott2DWave rottWav = new Rott2DWave(DEFAULT_VOC_CHANNELS, DEFAULT_VOC_SAMPLERATE, DEFAULT_VOC_BITS, this._vocSoundStream.GetBuffer());

                //using (Rott2DWave rottWav = new Rott2DWave(this._vocSoundStream.GetBuffer()))
                using (Rott2DWave rottWav = new Rott2DWave((short)this._channels, this._sampleRate, (short)this._bits, this._vocSoundStream.GetBuffer()))
                {
                    isVocDone = rottWav.saveWave(filename);
                }

            }

            return isVocDone;
        }

        /// <summary>
        /// Get raw bytes stream
        /// </summary>
        public override byte[] GetStreamRaw()
        {
            if (this._vocSoundData != null)
            {
                return this._vocSoundData;
            }
            else
            {
                return this._rawData;
            }
        }

        /// <summary>
        /// Get memorystream bytes
        /// </summary>
        public override MemoryStream GetStreamMemory()
        {
            if (this._vocSoundStream != null)
            {
                return this._vocSoundStream;
            }
            else
            {
                MemoryStream sfxStream = null;

                if (this._rawData != null)
                {
                    sfxStream = new MemoryStream(this._rawData);
                }

                return sfxStream;
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "voc_t";
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Try figure out if we have ROTT sound lump (voc file)
        /// </summary>
        public static bool isSoundLump(byte[] lumpdata)
        {

            /*
             * A valid VOC files first offset bytes always contain the string value
             * "Creative Voice File"
             * 
             * This is 19 bytes in length
             * 
             * In binary this looks like:
             * 43 72 65 61 74 69 76 65 20 56 6F 69 63 65 20 46 69 6C 65
             * 
             */

            bool vocLump = false;

            if (lumpdata.Length > Rott2DVocHeader.SOUND_VOC_HEADER_IDSIZE)
            {
                System.Text.Encoding encoding = System.Text.ASCIIEncoding.ASCII;
                string description = encoding.GetString(lumpdata, 0, Rott2DVocHeader.SOUND_VOC_HEADER_IDSIZE);   //"Creative Voice File" is first 19 bytes

                //check header writing of a voc == "Creative Voice File"
                if (description.ToLower() == Rott2DVocHeader.SOUND_VOC_HEADER_ID.ToLower())   //check the first 19 bytes
                    vocLump = true;
            }

            return vocLump;
        }
        #endregion

    }
    #endregion

}
