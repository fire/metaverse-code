// Copyright Hugh Perkins 2004,2005,2006
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License version 2 as published by the
// Free Software Foundation;
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
//  more details.
//
// You should have received a copy of the GNU General Public License along
// with this program in the file licence.txt; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-
// 1307 USA
// You can find the licence also on the web at:
// http://www.opensource.org/licenses/gpl-license.php
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Metaverse.Utility;
using Metaverse.Controller;
using Metaverse.Common.Controller;

namespace OSMP
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            try
            {
                //new TestBinaryPacker().Go();
                //TestNetworkUdp.Go(args);
                //TestLevel2.Go(args);
                //TestNetRpc.Go(args);
                //new TestReplicationAttributes().Go();
                //return;

                LogFile.GetInstance().Init( EnvironmentHelper.GetExeDirectory() + "/osmplog_" + new Random().Next(1000) + ".log");

                Arguments arguments = new Arguments(args);

                LogFile.WriteLine("here1");
                System.Environment.SetEnvironmentVariable( "PATH", System.Environment.GetEnvironmentVariable( "PATH" ) +
                    ";" + EnvironmentHelper.GetExeDirectory(), EnvironmentVariableTarget.Process );

                LogFile.WriteLine("here1");
                if( arguments.Unnamed.Contains("clientonly") )
                {
                	LogFile.WriteLine("here2");
                    LogFile.WriteLine("User requested client only");
                    ClientController.Instance.Initialize(args);
                }
                else if (arguments.Unnamed.Contains("serveronly"))
                {
                	LogFile.WriteLine("here3");
                    LogFile.WriteLine("User requested server only");
                    ServerController.Instance.Initialize( args );
                }
                else
                {
                	LogFile.WriteLine("here4");
                	ClientController.Instance.InitializeWithServer( args );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine( e );
                string errorlogpath = EnvironmentHelper.GetExeDirectory() + "/error.log";
                StreamWriter sw = new StreamWriter( errorlogpath, false );
                sw.WriteLine( LogFile.GetInstance().logfilecontents );
                sw.WriteLine( e.ToString() );
                sw.Close();

                if (System.Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    ProcessStartInfo psi = new ProcessStartInfo( "notepad.exe", errorlogpath );
                    psi.UseShellExecute = true;
                    Process process = new Process();
                    process.StartInfo = psi;
                    process.Start();
                }
            }
        }
    }

}
