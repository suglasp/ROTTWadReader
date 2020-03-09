/*
 * ROTT2D
 * Unit: ROTT2D Logging class
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
using System.IO;
using System.Text;

namespace ROTT2D.log
{
    /// <summary>
    /// Log type structure
    /// </summary>
    public enum Rott2DLoggerMessageType : byte
    { 
        LOG_INFO = 0,
        LOG_WARNING,
        LOG_ERROR
    }

    /// <summary>
    /// Rott2D Logging class
    /// </summary>
    public sealed class Rott2DLogger
    {

        /*
         * Internal logging class
         * 
         */

        /// <summary>
        /// public constants
        /// </summary>
        public const string DEFAULT_LOG_OPFILENAME = "rott2d";

        /// <summary>
        /// private vars
        /// </summary>
        private static readonly Rott2DLogger _singletonInstance = new Rott2DLogger(); //create singleton instance
        private StreamWriter _logWriter = null;
        private string _logPath, _logFile, _logFullPath;
        private bool _logReady = false;

        /// <summary>
        /// Constructor (PRIVATE!!!)
        /// </summary>
        private Rott2DLogger()
        {
            this._logReady = false;

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Rott2DLogger()
        {

        }

        /// <summary>
        /// singleton instance
        /// </summary>
        public static Rott2DLogger Instance
        {
            get
            {
                return _singletonInstance;
            }
        }

        /// <summary>
        /// Getter for status
        /// </summary>
        public bool isReady
        {
            get { return this._logReady; }
        }

        /// <summary>
        /// Set logging path en filename
        /// </summary>
        public void setOutput(string strPath, string strName = DEFAULT_LOG_OPFILENAME)
        {
            this._logPath = strPath;
            this._logFile = strName;

            DateTime timestamp = DateTime.Now;
            this._logFullPath = _logPath + @"\" + _logFile + "_" + timestamp.ToString("ddMMyyyy") + ".log";

            if (!File.Exists(this._logFullPath))
            {
                FileStream fs = File.Create(this._logFullPath);
                fs.Close();
            }

            if (_logWriter == null)
            {
                _logWriter = new StreamWriter(this._logFullPath, true);

                this._logReady = true;
            }
        }

        /// <summary>
        /// Write to the output file log
        /// </summary>
        public bool writeLog(string strMessage, Rott2DLoggerMessageType msgType = Rott2DLoggerMessageType.LOG_INFO)
        {
            if (this.isReady)
            {

                try
                {

                    if (this._logWriter != null)
                    {
                        //_logWriter = File.AppendText(this._logFullPath);

                        if ((strMessage.Length > 0) || (strMessage != string.Empty))
                        {
                            DateTime timestamp = DateTime.Now;

                            switch (msgType)
                            {
                                default:
                                case Rott2DLoggerMessageType.LOG_INFO:
                                    _logWriter.WriteLine(timestamp.ToString("hh:mm") + " INFO: " + strMessage + "\r\n");
                                    break;
                                case Rott2DLoggerMessageType.LOG_WARNING:
                                    _logWriter.WriteLine(timestamp.ToString("hh:mm") + " WARNING: " + strMessage + "\r\n");
                                    break;
                                case Rott2DLoggerMessageType.LOG_ERROR:
                                    _logWriter.WriteLine(timestamp.ToString("hh:mm") + " ERROR: " + strMessage + "\r\n");
                                    break;
                            }

                        }
                        else
                        {
                            //write empty line
                            _logWriter.WriteLine(strMessage);
                        }

                        _logWriter.Flush();
                        _logWriter.Close();
                    }
                }
                catch (Exception exRest)
                {
                    Console.WriteLine("loggin error\n" + exRest.ToString()); //output "junk" to console to get rid of it
                }
                /*finally {
                    if ((this.isReady) && (this._logWriter != null))
                    {
                        _logWriter.Flush();
                        _logWriter.Close();
                    }
                }*/
            }

            return this.isReady;
        }

    }
}
