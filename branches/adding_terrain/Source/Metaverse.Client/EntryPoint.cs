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

                System.Environment.SetEnvironmentVariable( "PATH", System.Environment.GetEnvironmentVariable( "PATH" ) +
                    ";" + EnvironmentHelper.GetExeDirectory(), EnvironmentVariableTarget.Process );

                if( arguments.Unnamed.Contains("clientonly") )
                {
                    LogFile.WriteLine("User requested client only");
                    MetaverseClient.GetInstance().Go(args);
                }
                else if (arguments.Unnamed.Contains("serveronly"))
                {
                    LogFile.WriteLine("User requested server only");
                    MetaverseServer.GetInstance().Init(args);
                    while (true)
                    {
                        MetaverseServer.GetInstance().Tick();
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    MetaverseServer.GetInstance().Init(args);
                    MetaverseClient.GetInstance().Tick += new MetaverseClient.TickHandler(EntryPoint_Tick);
                    MetaverseClient.GetInstance().Go(args);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine( e );
                string errorlogpath = EnvironmentHelper.GetExeDirectory() + "/error.log";
                StreamWriter sw = new StreamWriter( errorlogpath, false );
                sw.WriteLine( LogFile.GetInstance().logfilecontents.Replace("\n","\n\r" ) );
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

        static void EntryPoint_Tick()
        {
            MetaverseServer.GetInstance().Tick();
        }
    }

}
