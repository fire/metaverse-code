// Copyright Hugh Perkins 2006
// hughperkins@gmail.com http://manageddreams.com
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURVector3E. See the GNU General Public License for
//  more details.
//
// You should have received a copy of the GNU General Public License along
// with this program in the file licence.txt; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-
// 1307 USA
// You can find the licence also on the web at:
// http://www.opensource.org/licenses/gpl-license.php
//
// ======================================================================================
//

using System;
using System.IO;

    // file logging functions
    // ====================
    //
    // just call WriteLog( "my message" ) to write to the log
    // for now, we just store the logged messages, that we could write to a crash dump file later
    public class LogFile
    {
        static LogFile instance = new LogFile();
        public static LogFile GetInstance(){ return instance; } // Singleton pattern

        string filename;

        public string Filename
        {
            get { return filename; }
        }

        LogFile() // protected constructor to force Singleton instantiation
        {
        }
        ~LogFile()
        {
        }
        
        //StreamWriter sw;
        
        public bool AutoFlushOn = false;

        public string logfilecontents = "";

        public LogFile Init()
        {
            return this;
        }

        //string prefix = "";

        public LogFile Init( string logfilepath )
        {
            this.filename = logfilepath;
            //sw = new StreamWriter(logfilepath, false);            
            //CSAI.GetInstance().RegisterVoiceCommand( "flushlog", new CSAI.VoiceCommandHandler( this.VCFlushLog ) );
            return this;
        }
        
        public void Flush()
        {
            //sw.Flush();
        }
                    
        // arguably we shouldnt auto-flush. because it slows writes, up to you
        public void WriteLine( string message )
        {
            //string finalmessage = prefix + " " + message;
            logfilecontents += message + "\n";
            Console.WriteLine(message);
            //sw.WriteLine(DateTime.Now.ToString("hh:mm:ss.ff") + ": " + message);
            //sw.WriteLine( message);
            //Console.WriteLine(message);
            //if( AutoFlushOn )
            //{
              //  sw.Flush();
            //}
            //sw.Flush();
        }
        
        public void Shutdown()
        {
            //sw.Flush();
            //sw.Close();
        }
    }
