/*
 * ROTT2D
 * Unit: ROTT2D WAD Reader tool
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 10-01-2012
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using ROTT2D.draw;             //fast rendering
using ROTT2D.log;              //logging
using ROTT2D.media;            //general media players

using ROTT2D.WAD.reader;       //WAD file
using ROTT2D.WAD.data;         //WAD files data layers
using ROTT2D.WAD.marker;       //WAD Marker lumps
using ROTT2D.WAD.palette;      //WAD Palette lump

namespace RottWadReader
{
    public partial class frmWADReader : Form
    {
        #region constants
        /* -- consts -- */
        private const int ICON_SIZE = 16;   //listview icon(s) size

        /* -- indices for the icon images in the view list -- */
        private const int ICON_UNKNOWN = 0;
        private const int ICON_MARKER = 1;
        private const int ICON_PAL = 2;
        private const int ICON_TEXTURE = 3;
        private const int ICON_SOUND = 4;
        private const int ICON_MUSIC = 5;
        private const int ICON_ASCII = 6;
        #endregion

        #region private vars
        /* -- private vars -- */
        private ImageList _lstWADEntriesImages;
        private Rott2DWADReader _wad = null;
        private Rott2DMarkerController _controller = null;
        private Rott2DLogger log = Rott2DLogger.Instance; //singleton instance
        private bool _isInited = false;
        #endregion

        #region CTOR
        public frmWADReader()
        {
            InitializeComponent();
        }
        #endregion

        /// <summary>
        /// Init the wad reader
        /// </summary>
        public void initWADReader()
        {
            /* -- form icon -- */
            if (File.Exists(Application.StartupPath + @"\data\icons\rott2d.ico"))
            {
                Icon icon = new Icon(Environment.CurrentDirectory + @"\data\icons\rott2d.ico");
                this.Icon = icon;
            }

            /* -- set default WAD file */
            this.txtWADPath.Text = Environment.CurrentDirectory + @"\data\wad\huntbgin.wad";

            /* -- logging stuff -- */
            this.log.setOutput(Environment.CurrentDirectory + @"\data\log");

            /* -- load image list for listview -- */
            this._lstWADEntriesImages = new ImageList();
            this._lstWADEntriesImages.ColorDepth = ColorDepth.Depth32Bit;
            this._lstWADEntriesImages.ImageSize = new Size(ICON_SIZE, ICON_SIZE);

            if (File.Exists(Environment.CurrentDirectory + @"\data\icons\rott_filetype_unknown16.png"))
            {
                Image unknownImgIcon = new Bitmap(Environment.CurrentDirectory + @"\data\icons\rott_filetype_unknown16.png");
                this._lstWADEntriesImages.Images.Add(unknownImgIcon);
            }

            if (File.Exists(Environment.CurrentDirectory + @"\data\icons\rott_filetype_marker16.png"))
            {
                Image markerImgIcon = new Bitmap(Environment.CurrentDirectory + @"\data\icons\rott_filetype_marker16.png");
                this._lstWADEntriesImages.Images.Add(markerImgIcon);
            }

            if (File.Exists(Environment.CurrentDirectory + @"\data\icons\rott_filetype_pal16.png"))
            {
                Image markerImgIcon = new Bitmap(Environment.CurrentDirectory + @"\data\icons\rott_filetype_pal16.png");
                this._lstWADEntriesImages.Images.Add(markerImgIcon);
            }

            if (File.Exists(Environment.CurrentDirectory + @"\data\icons\rott_filetype_texture16.png"))
            {
                Image textureImgIcon = new Bitmap(Environment.CurrentDirectory + @"\data\icons\rott_filetype_texture16.png");
                this._lstWADEntriesImages.Images.Add(textureImgIcon);
            }

            if (File.Exists(Environment.CurrentDirectory + @"\data\icons\rott_filetype_sound16.png"))
            {
                Image soundImgIcon = new Bitmap(Environment.CurrentDirectory + @"\data\icons\rott_filetype_sound16.png");
                this._lstWADEntriesImages.Images.Add(soundImgIcon);
            }

            if (File.Exists(Environment.CurrentDirectory + @"\data\icons\rott_filetype_music16.png"))
            {
                Image musicImgIcon = new Bitmap(Environment.CurrentDirectory + @"\data\icons\rott_filetype_music16.png");
                this._lstWADEntriesImages.Images.Add(musicImgIcon);
            }

            if (File.Exists(Environment.CurrentDirectory + @"\data\icons\rott_filetype_ascii16.png"))
            {
                Image asciiImgIcon = new Bitmap(Environment.CurrentDirectory + @"\data\icons\rott_filetype_ascii16.png");
                this._lstWADEntriesImages.Images.Add(asciiImgIcon);
            }

#if DEBUG
            /* -- hide debug GUI stuff -- */
            this.btnListMarkers.Visible = true;
#endif

            /* -- correcting some other GUI stuff -- */
            this.nudDarkness.Enabled = this.chkDarkness.Checked;  //enable or disable numeric up down for darkness

            /* -- assign the image list to the view list -- */
            this.lstWADEntries.MultiSelect = false;
            this.lstWADEntries.AutoArrange = false;
            this.lstWADEntries.HeaderStyle = ColumnHeaderStyle.None;
            this.lstWADEntries.SmallImageList = this._lstWADEntriesImages;
            this.lstWADEntries.View = View.SmallIcon;

            /* init controller */
            if (this._controller == null)
            {
                this._controller = new Rott2DMarkerController();
            }
            else {
                this._controller.clearMarkers();
            }

            /* -- open the wad -- */
            if (File.Exists(this.txtWADPath.Text))
            {
                this.log.writeLog("startup wad exists " + this.txtWADPath.Text);

                this.ReadWAD(this.txtWADPath.Text);
            }
            else
            {
                this.log.writeLog("startup wad not found " + txtWADPath.Text, Rott2DLoggerMessageType.LOG_ERROR);

                this.txtWADPath.Text = String.Empty;                
            }

            this._isInited = true;
        }

        /// <summary>
        /// Main form load event
        /// </summary>
        private void frmWADReader_Load(object sender, EventArgs e)
        {
            this.initWADReader();
        }

        /// <summary>
        /// Double mouse click event
        /// </summary>
        private void lstWADEntries_DoubleClick(object sender, EventArgs e)
        {
            this.ProcessSelectedLump();

            if (this.chkAutoScale.Checked)
            {
                this.UpscaleTexture();
            }
        }

        /// <summary>
        /// Event for list change
        /// </summary>
        private void lstWADEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._isInited)
            {
                this.ProcessSelectedLump();
            }


            if (this.chkAutoScale.Checked)
            {
                this.UpscaleTexture();
            }
        }

        /// <summary>
        /// Upscale the openened texture image in the picturebox
        /// </summary>
        private void UpscaleTexture()
        {
            if (picWADView.Image != null)
            {
                Bitmap bmpOriginal = (Bitmap)picWADView.Image.Clone();

                int scaleFactor = 3;

                /*int scaleFactor = 2;

                for (int fact = 0; fact <= 4; fact++)
                { 
                    if ((bmpOriginal.Width * fact) > picWADView.Width)
                        scaleFactor = fact;

                    if ((bmpOriginal.Height * scaleFactor) > picWADView.Height)
                        scaleFactor = fact--;
                }*/
                
                if (bmpOriginal.Height * scaleFactor <= picWADView.Height)
                {
                    Bitmap bmpResized = new Bitmap(picWADView.Image.Width * scaleFactor, picWADView.Image.Height * scaleFactor);

                    using (Graphics g = Graphics.FromImage(bmpResized))
                    {
                        //keep old-skool "pixellated" effect of low bound textures
                        g.CompositingQuality = CompositingQuality.HighSpeed;
                        g.SmoothingMode = SmoothingMode.HighSpeed;
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.SmoothingMode = SmoothingMode.None;
                        /*g.PixelOffsetMode = PixelOffsetMode.None;
                        g.PageUnit = GraphicsUnit.Pixel;*/

                        g.DrawImage(bmpOriginal, 0, 0, bmpResized.Width, bmpResized.Height);
                    }

                    this.picWADView.Image = bmpResized;
                }

            }
        }

        /// <summary>
        /// Read the opened WAD file
        /// </summary>
        private void ReadWAD(string filename)
        {
            //close first on reopen
            if (this._wad != null)
            {
                this._wad.CloseRottWad();
                this._wad = null;

                this.log.writeLog("wad closed");
            }

            //open a wad and read data
            if (this._wad == null)
            {
                this.log.writeLog("opening wad " + filename);

                //open the ROTT wad
                this._wad = new Rott2DWADReader(filename);

                //if valid ROTT wad file, go on and read lumps
                if (this._wad.WADValid)
                {
                    //clear the gui list
                    this.lstWADEntries.Items.Clear();

                    //Clear all markers from cache
                    if (this._controller != null)
                    {
                        this._controller.clearMarkers();
                    }

                    Rott2DWadDirectoryEntry[] wadEntries = this._wad.GetAllWadEntries();  //retrieve all directoy entries once

                    //process marker controller list
                    foreach (Rott2DWadDirectoryEntry mrkrlump in wadEntries)
                    {
                        Application.DoEvents();

                        if (Rott2DMarkerController.isMarker(mrkrlump.Name))
                        {
                            Rott2DMarker marker = new Rott2DMarker(mrkrlump.ID, mrkrlump.Name, mrkrlump.Offset, mrkrlump.Size);
                            this._controller.addMarker(marker);
                        }
                    }

                    //process all lumps
                    foreach (Rott2DWadDirectoryEntry lump in wadEntries)
                    {
                        Application.DoEvents();

                        ListViewItem item = new ListViewItem(lump.Name);
                        item.Name = lump.Name;
                        item.Tag = lump.Offset.ToString() + ";" + lump.Size.ToString();

                        if (Rott2DMarkerController.isMarker(lump.Name))
                        {
                            item.ImageIndex = ICON_MARKER;
                            this.lstWADEntries.Items.Add(item);
                        }
                        else
                        {
                            //byte[] data = this._wad.getRottWadLumpDataByLump(lump);

                            /*
                            //define icons in list
                            Rott2DMarkerType lumpType = this._controller.getLumpType(lump.Name, lump.ID);

                            switch (lumpType)
                            { 
                                case Rott2DMarkerType.mtUnknown:
                                    item.ImageIndex = ICON_UNKNOWN;
                                    break;
                                case Rott2DMarkerType.mtASCII:
                                    item.ImageIndex = ICON_ASCII;
                                    break;
                                case Rott2DMarkerType.mtPatch:
                                case Rott2DMarkerType.mtFlat:
                                case Rott2DMarkerType.mtSky:
                                case Rott2DMarkerType.mtMasked:
                                case Rott2DMarkerType.mtPic:
                                case Rott2DMarkerType.mtTransMasked:
                                    item.ImageIndex = ICON_TEXTURE;
                                    break;
                                case Rott2DMarkerType.mtMusic:
                                    item.ImageIndex = ICON_MUSIC;
                                    break;
                                case Rott2DMarkerType.mtSound:
                                    item.ImageIndex = ICON_SOUND;
                                    break;
                                case Rott2DMarkerType.mtPalette:
                                    item.ImageIndex = ICON_PAL;
                                    break;
                                default:
                                    item.ImageIndex = ICON_UNKNOWN;
                                    break;
                            }
                            */

                            this.lstWADEntries.Items.Add(item);
                        }


                        //this.log.writeLog("added lump " + lump.Name + " (offset: " + lump.Offset + ", size: " + lump.Size + ")");

                        //Application.DoEvents();
                    }

                    lstWADEntries.Focus();

                    //select first item in list
                    if (this.lstWADEntries.Items.Count > 0)
                    {
                        this.lstWADEntries.Items[0].Selected = true;

                        this.lblTotalLumps.Text = "total lumps: " + this.lstWADEntries.Items.Count.ToString();

                        //this.log.writeLog("total lumps: " + lstWADEntries.Items.Count.ToString());
                    }

#if (!DEBUG)
                    /* -- show "default" picture (only in prod release, not in DEBUG version) -- */

                    //set as default the image MMBK (= loading screen background in ROTT)
                    byte[] paldata = this._wad.GetWadLumpDataByName("PAL");  //default palette
                    Rott2DPalette pal = null;

                    if (paldata.Length == 768)
                    {
                        pal = new Rott2DPalette(ref paldata);
                    }

                    byte[] mmbk = this._wad.GetWadLumpDataByName("MMBK");  //MMBK
                    Rott2DPic mmbkpic = new Rott2DPic(ref mmbk, ref pal);

                    this.picWADView.Image = mmbkpic.GetTexture();

                    pal.Dispose();
                    pal = null;
                    //mmbkpic.Dispose();

#endif

                    /*
                    //pc speaker testing (experimental)
                    byte[] spekdata = this._wad.getWadLumpDataByName("PCSP45");
                    Rott2DSpeaker spek = new Rott2DSpeaker(ref spekdata);
                    spek.playSpeaker();
                    */

                } //Valid ROTT wad?
            }
        }

        /// <summary>
        /// Process selected lump
        /// </summary>
        private void ProcessSelectedLump()
        {
            try
            {

                if (this.lstWADEntries.Items.Count > 0)
                {
                    //get the selected item(s)
                    ListView.SelectedListViewItemCollection selectedItems = this.lstWADEntries.SelectedItems;

                    //clear image view
                    if (this.picWADView.Image != null)
                    {
                        this.picWADView.Image.Dispose();
                        this.picWADView.Image = null;
                    }

                    //load selected lump from list
                    foreach (ListViewItem selectedItem in selectedItems)
                    {
                        Rott2DWadDirectoryEntry lump = this._wad.GetWadEntryByName(selectedItem.Name);

                        string paletteName = "PAL";

                        //select custom palette for "Apogee" company logo
                        if ((selectedItem.Name == "AP_WRLD") || (selectedItem.Name == "AP_TITL"))
                        {
                            paletteName = "AP_PAL"; //palette for AP_TITL and AP_WRLD
                        }

                        // read palette
                        byte[] paldata = this._wad.GetWadLumpDataByName(paletteName);  //default palette
                        //byte[] paldata = this._wad.getWadLumpDataByName("AP_PAL"); //palette for AP_TITL and AP_WRLD
                        Rott2DPalette pal = null;

                        if (paldata.Length == 768)
                        {
                            pal = new Rott2DPalette(ref paldata);

                            if ((selectedItem.Name == "AP_WRLD") || (selectedItem.Name == "AP_TITL"))
                            {
                                pal.MaskColorIndex = 0;
                            }

                            if (this.chkDarkness.Checked)
                                pal.AlphaColor = (byte)this.nudDarkness.Value;  //try palette darkness
                        }

                        //parse lump
                        if (lump.Size != 0)
                        {
                            byte[] data = this._wad.GetRottWadLumpDataByLump(ref lump);


                            // -- DEBUG --
#if DEBUG                           

                            /*if (this._controller.isSoundLump(data))
                                MessageBox.Show("voc sound detected");

                            if (this._controller.isMidiLump(data))
                                MessageBox.Show("midi music detected");

                            if (this._controller.isPicTexture(data))
                                MessageBox.Show("small picture texture detected");

                            if (this._controller.isPalette(data))
                                MessageBox.Show("palette data detected");

                            if (this._controller.isPatchTexture(data))
                                MessageBox.Show("patch texture detected");

                            if (this._controller.isSkyTexture(data))
                                MessageBox.Show("sky texture detected");

                            if (this._controller.isMaskedTextureLump(data))
                                MessageBox.Show("masked texture detected");*/

#endif
                            // -- DEBUG --


                            Rott2DMarkerType lumpType = this._controller.getLumpType(data);  //findout if we have a specific type

                            if (lumpType == Rott2DMarkerType.mtUnknown)
                            {
                                //in case of unknown, try ascii and "fixes"
                                lumpType = this._controller.isASCIIType(lump.Name, lump.ID);


                                //--> code integrated into Controller <--
                                //still unknown, try masked
                                /*if (this._controller.isMaskedTexture(data))
                                {
                                    lumpType = Rott2DMarkerType.mtMasked;  //masked
                                }
                                else
                                {
                                    if (this._controller.isTransMaskedTexture(data))
                                        lumpType = Rott2DMarkerType.mtTransMasked;  //transmasked
                                } */
                                //--> code integrated into Controller <--


                                //here we leave it by unknown...
                            }

                            /*
                            Bitmap tex = new Bitmap(64, 64);
                            this.picWADView.Image = tex;
                            */
                            
                            //this.picWADView.Image = null;

                            switch (lumpType)
                            {
                                default:
                                case Rott2DMarkerType.mtUnknown:
                                    //unknown, we don't do anything
                                    break;
                                case Rott2DMarkerType.mtPic:
                                    Rott2DPic smallPic = new Rott2DPic(ref data, ref pal);
                                    this.picWADView.Image = smallPic.GetTexture();
                                    break;
                                case Rott2DMarkerType.mtFlat:
                                    Rott2DFlat flat = new Rott2DFlat(ref data, ref pal);
                                    this.picWADView.Image = flat.GetTexture();
                                    break;
                                case Rott2DMarkerType.mtPatch:
                                    Rott2DPatch patch = new Rott2DPatch(ref data, ref pal);
                                    this.picWADView.Image = patch.GetTexture();
                                    break;
                                case Rott2DMarkerType.mtColormap:
                                    Rott2DColormap colmap = new Rott2DColormap(ref data, ref pal);
                                    this.picWADView.Image = colmap.GetTexture();
                                    //this.picWADView.Image = colmap.getColorMapBmp(15);
                                    break;
                                case Rott2DMarkerType.mtSky:
                                    Rott2DSky sky = new Rott2DSky(ref data, ref pal);
                                    this.picWADView.Image = sky.GetTexture();
                                    break;
                                case Rott2DMarkerType.mtMasked:
                                    Rott2DMasked masked = new Rott2DMasked(ref data, ref pal);

                                    //System.Threading.Tasks.Task.Factory.StartNew(() => this.picWADView.Image = masked.getTexture());
                                                                       
                                    this.picWADView.Image = masked.GetTexture();
                                    break;
                                case Rott2DMarkerType.mtTransMasked:
                                    Rott2DTransMasked transmasked = new Rott2DTransMasked(ref data, ref pal);
                                    this.picWADView.Image = transmasked.GetTexture();
                                    break;
                                case Rott2DMarkerType.mtMusic:
                                    Rott2DMidi midi = new Rott2DMidi(ref data);

                                    midi.ExportMidiToFile(Environment.CurrentDirectory + @"\data\cache\out");

                                    if (midi.isReady)
                                    {
                                        long ret = -1;
                                        ret = Rott2DSimpleMidiPlayer.playMidi(Environment.CurrentDirectory + @"\data\cache\out.mid", "track");

                                        if (ret == 0)
                                        {
                                            MessageBox.Show("ROTT music '" + lump.Name + "' should be playing now...\nClick OK to stop.");
                                            Rott2DSimpleMidiPlayer.stopAllMidi();
                                        }
                                    }

                                    break;
                                case Rott2DMarkerType.mtSound:
                                    string outwave = Environment.CurrentDirectory + @"\data\cache\" + lump.Name.ToLower() + ".wav";

                                    //parse voc file data and convert to wave format
                                    Rott2DVoc voc = new Rott2DVoc(ref data);
                                    if (voc.ExportVocToWaveFile(outwave))
                                    {
                                        Rott2DSimpleWavePlayer.playWave(outwave);  //play wave sample
                                    }
                                    break;
                                case Rott2DMarkerType.mtPalette:
                                    Bitmap palCopy = (Bitmap)pal.GetTexture().Clone();  //our palette is freed at the end of the statement, so copy it.
                                    this.picWADView.Image = palCopy;  //(Show copy of) Palette texture 
                                    break;
                                case Rott2DMarkerType.mtASCII:
                                    Rott2DASCII asciiText = new Rott2DASCII(ref data);
                                    
                                    MessageBox.Show(asciiText.GetText()); //dirty display method. But it works...
                                    break;
                            }

                            //clear the palette data
                            if (pal != null)
                            {
                                pal.Dispose();
                                pal = null;
                            }

                        }
                        else
                        {
                            if (this.chkWarnOnZeroLump.Checked)
                                MessageBox.Show("lump size is zero!", "error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Open new WAD
        /// </summary>
        private void btnWADOpen_Click(object sender, EventArgs e)
        {
            //provide used a dialg selection window hnd
            using (OpenFileDialog openfiledlg = new OpenFileDialog())
            {
                openfiledlg.Title = "open a WAD file...";
                openfiledlg.Filter = "wad files (*.wad)|*.wad";
                openfiledlg.InitialDirectory = Environment.CurrentDirectory + @"\data\wad\";

                if (openfiledlg.ShowDialog() == DialogResult.OK)
                {
                    //set selected wad filename & path
                    this.txtWADPath.Text = openfiledlg.FileName;

                    //open the wad
                    if (File.Exists(this.txtWADPath.Text))
                    {
                        log.writeLog("custom wad exists " + this.txtWADPath.Text);

                        this.ReadWAD(this.txtWADPath.Text);
                    }
                }
            }

            
        }

        /// <summary>
        /// Item selection change in lump list
        /// </summary>
        private void lstWADEntries_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (lstWADEntries.Items.Count > 0)
            {
                ListView.SelectedListViewItemCollection selectedItems = this.lstWADEntries.SelectedItems;

                foreach (ListViewItem selectedItem in selectedItems)
                {
                    lblLumpName.Text = "Name: " + selectedItem.Name;

                    string data_a = selectedItem.Tag.ToString();
                    string[] data_b = data_a.Split(';');

                    lblLumpOffset.Text = "Offset: " + data_b[0];
                    lblLumpSize.Text = "Size : " + data_b[1];
                }
            }
        }

        /// <summary>
        /// Debug for listing all null lumps
        /// </summary>
        private void btnListMarkers_Click(object sender, EventArgs e)
        {
            string sb = string.Empty;

            int counter = 0;
            foreach (Rott2DWadDirectoryEntry entry in this._wad.GetAllWadEntries())
            {
                if (entry.Size == 0)
                {
                    sb = sb + entry.Name + " ";
                    
                    counter++;

                    if (counter == 3)
                    {
                        sb = sb + "\n";
                        counter = 0;
                    }
                }

            }

            MessageBox.Show(sb.ToString());
        }

        /// <summary>
        /// checkbox to allow darkness rendering on the palette
        /// </summary>
        private void chkDarkness_CheckedChanged(object sender, EventArgs e)
        {
            this.nudDarkness.Enabled = this.chkDarkness.Checked;
        }

    }
}
