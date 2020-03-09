/*
 * ROTT2D
 * Unit: ROTT2D Marker Controller sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 14-01-2012
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

#if DEBUG
using System.Windows.Forms;
#endif

using ROTT2D.WAD.data;         //General lump data
using ROTT2D.WAD.palette;      //Palette lump data

namespace ROTT2D.WAD.marker
{

    #region Marker controller class
    /// <summary>
    /// WAD Marker controller class
    /// </summary>
    public sealed class Rott2DMarkerController : IRott2DMarkerController
    {
        /*
         * Class to "cache" all marker lumps.
         * Other lumps can be defined to what data type they belong.
         * 
         */

        #region Private vars
        /// <summary>
        /// Private vars
        /// </summary>
        private List<Rott2DMarker> _markers = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructors
        /// </summary>
        public Rott2DMarkerController()
        {
            this.initMarkersList();
        }
        #endregion

        #region Destructors
        /// <summary>
        /// Destructors
        /// </summary>
        ~Rott2DMarkerController()
        {
            this.Dispose();    
        }

        /// <summary>
        /// Dispose
        /// </summary>
        private void Dispose()
        {
            if (_markers != null)
            {
                this.clearMarkers();
                this._markers = null; //Free pointer location
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Init the list
        /// </summary>
        private void initMarkersList()
        {
            if (_markers == null)
            {
                _markers = new List<Rott2DMarker>();
            }
        }

        /// <summary>
        /// Clear the markers list
        /// </summary>
        public void clearMarkers()
        {
            if (_markers != null)
            {
                this._markers.Clear();
            }
        }

        /// <summary>
        /// Get lump type (by data)
        /// </summary>
        public Rott2DMarkerType getLumpType(byte[] data)
        {
            Rott2DMarkerType mkrType = Rott2DMarkerType.mtUnknown;

            /*
             * If data is larger then zero, start processing.
             * 
             * First we process all simple data structure = fast
             * Then, we also keep in mind what data types are most common (in WAD and for loading).
             * 
             * Remarks:
             * Since c# doesn't support ranges in a switch-case statement,
             * we'll have to do it with slower processing if-then statements.
             * 
             */

            if (data.Length > 0)
            {
                //okay, larger the zero -> start processing
                
                //sky
                if (Rott2DSky.isSkyTexture(data))
                {
                    mkrType = Rott2DMarkerType.mtSky;
                }
                else
                {
                    //pic_t
                    if (Rott2DPic.isPicTexture(data))
                    {
                        mkrType = Rott2DMarkerType.mtPic;
                    }
                    else
                    {
                        //patch (lpic_t)                            
                        if (Rott2DPatch.isPatchTexture(data))
                        {
                            mkrType = Rott2DMarkerType.mtPatch;
                        }
                        else
                        {
                            //colormap
                            if (Rott2DColormap.isColormap(data))
                            {
                                mkrType = Rott2DMarkerType.mtColormap;
                            }
                            else
                            {
                                //masked
                                if (Rott2DMasked.isMaskedTexture(data))
                                {
                                    mkrType = Rott2DMarkerType.mtMasked;
                                }
                                else
                                {
                                    //transmasked
                                    if (Rott2DTransMasked.isTransMaskedTexture(data))
                                    {
                                        mkrType = Rott2DMarkerType.mtTransMasked;
                                    }
                                    else
                                    {
                                        //flat (moved checking of flat and palette after 
                                        //      all header type lumps, to prevent incorrect checking.)
                                        if (Rott2DFlat.isFlatTexture(data))
                                        {
                                            mkrType = Rott2DMarkerType.mtFlat;
                                        }
                                        else
                                        {
                                            //palette
                                            if (Rott2DPalette.isPalette(data))
                                            {
                                                mkrType = Rott2DMarkerType.mtPalette;
                                            }
                                            else
                                            {
                                                //sound (voc data)
                                                if (Rott2DVoc.isSoundLump(data))
                                                {
                                                    mkrType = Rott2DMarkerType.mtSound;
                                                }
                                                else
                                                {
                                                    //music (midi data)
                                                    if (Rott2DMidi.isMidiLump(data))
                                                    {
                                                        mkrType = Rott2DMarkerType.mtMusic;
                                                    } //midi
                                                } //sound                                            
                                            } //palette
                                        } //flat
                                    } //trans masked
                                } //masked
                            } //colormap
                        } //patch
                    }  //pic
                } //sky

            } //else unknown (unknown = ascii, pcspeaker, fonts, ...)

#if DEBUG
            MessageBox.Show(mkrType.ToString()); //fast dirty DEBUG
#endif

            return mkrType;
        }

        /// <summary>
        /// Get lump type (by name and id)
        /// </summary>
        public Rott2DMarkerType isASCIIType(string name, int id)
        {
            Rott2DMarkerType mkrType = Rott2DMarkerType.mtUnknown;

            /*
             * Remarks:
             * Since c# doesn't support ranges in a switch-case statement,
             * we'll have to do it with slower processing if-then statements.
             * 
             */

            if ((name == "VENDOR") || (name == "TABLES") || (name == "GUSMIDI") || (name == "SHARTITL") || (name == "SHARTIT2") || (name == "LICENSE"))
            {
                mkrType = Rott2DMarkerType.mtASCII;  //text
            }

            return mkrType;
        }


        /// <summary>
        /// Search for a marker offset by name
        /// </summary>
        public int getMarkerIDByName(string name)
        {
            int foundID = -1;

            foreach (Rott2DMarker marker in this._markers)
            {
                if (marker.Name == name.ToUpper())
                {
                    foundID = marker.ID;
                    break;
                }
            }

            return foundID;
        }

        /// <summary>
        /// Search for a marker by name
        /// </summary>
        public Rott2DMarker getMarkerByName(string name)
        {
            Rott2DMarker foundMarker = null;

            foreach (Rott2DMarker marker in this._markers)
            {
                if (marker.Name == name.ToUpper())
                {
                    foundMarker = marker;
                }
            }

            return foundMarker;
        }

        /// <summary>
        /// Add a marker to list by params
        /// </summary>
        public void addMarker(int id, string name, int offset, int size)
        {
            Rott2DMarker markerlump = new Rott2DMarker(id, name, offset, size);

            this._markers.Add(markerlump);
        }

        /// <summary>
        /// Add a marker to list by Marker
        /// </summary>
        public void addMarker(Rott2DMarker markerlump)
        {
            this._markers.Add(markerlump);
        }

        /// <summary>
        /// Delete marker from list
        /// </summary>
        public void removeMarker(Rott2DMarker markerlump)
        {
            foreach (Rott2DMarker marker in this._markers.ToArray()) //Implement ToArray to exclude a InvalidOperationException
            {
                if (marker.Name == markerlump.Name)
                {
                    this._markers.Remove(marker);
                }
            }
        }

        /// <summary>
        /// Delete marker from list
        /// </summary>
        public void removeMarker(string name)
        {
            foreach (Rott2DMarker marker in this._markers.ToArray()) //Implement ToArray to exclude a InvalidOperationException
            {
                if (marker.Name == name.ToUpper())
                {
                    this._markers.Remove(marker);
                }
            }
        }

        /// <summary>
        /// Get total size of (markers-) list
        /// </summary>
        public int getSize()
        {
            return this._markers.Count;
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Method to identify a marker in a ROTT wad file.
        /// This method is static so we don't have to create every a object.
        /// This is the only method that scan's on lump name.
        /// </summary>
        public static bool isMarker(string name)
        {
            /*
             * Most safe way is to indicate a marker by name.
             * 
             * If the lump size is zero, chances are great that you have a marker,
             * but this isn't always the case for "The Hunt Begins" WAD file.
             * 
             */

            bool _isMarker = false;

            switch (name)
            {
                case "WALLSTRT":  //static wall textures
                case "WALLSTOP":

                case "ANIMSTRT":  //animated wall textures

                case "EXITSTRT":  //exit arch textures
                case "EXITSTOP":

                case "ABVMSTRT":  //above door textures

                case "ABVWSTRT":  //above door textures

                case "HMSKSTRT":  //switch textures

                case "GUNSTART":   //gun textures

                case "ELEVSTRT":  //Elevator textures
                case "ELEVSTOP":

                case "DOORSTRT":  //Door masked textures
                case "DOORSTOP":

                case "SIDESTRT":  //Door side pannel (key markings, ...) textures
                case "SIDESTOP":

                case "MASKSTRT":  //Masked textures
                case "MASKSTOP":

                case "UPDNSTRT":  //floor and ceiling textures
                case "UPDNSTOP":

                case "SKYSTART":   //Sky textures
                case "SKYSTOP":

                case "ORDRSTRT":   //Order images
                case "ORDRSTOP":

                case "SPECMAPS":   //some data

                case "PLAYMAPS":   //color maps, overlay and menu textures

                case "SHAPSTRT":   //character textures
                case "SHAPSTOP":

                case "DIGISTRT":   //voc sample sounds
                case "DIGISTOP":

                case "SONGSTRT":   //midi music

                case "PCSTART":    //pc speaker sound stuff
                case "PCSTOP":

                case "ADSTART":    //Adblib sound card stuff
                case "ADSTOP":

                    _isMarker = true;
                    break;
                default:
                    break;
            }

            return _isMarker;
        }
        #endregion

    }
    #endregion

}
