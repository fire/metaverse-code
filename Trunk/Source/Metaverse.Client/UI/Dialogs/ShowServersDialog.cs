// Copyright Hugh Perkins 2006
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
using Glade;
using Gtk;

namespace OSMP
{
    public class ShowServersDialog
    {
        static ShowServersDialog instance = new ShowServersDialog();
        public static ShowServersDialog GetInstance() { return instance; }

        [Widget]
        ScrolledWindow serversscrolledwindow = null;

        [Widget]
        TreeView serverstreeview = null;

        [Widget]
        Button btnconnect = null;

        [Widget]
        Button btnclose = null;

        [Widget]
        Gtk.Window availableserversdialog = null;

        ShowServersDialog()
        {
        }

        public void Show( string[] serverlist )
        {
            Console.WriteLine( "showserversdialog.Show()" );

            if (availableserversdialog != null)
            {
                availableserversdialog.Destroy();
            }

            Glade.XML app = new Glade.XML( EnvironmentHelper.GetExeDirectory() + "/metaverse.client.glade", "availableserversdialog", "" );
            app.Autoconnect( this );

            btnclose.Clicked += new EventHandler( btnclose_Clicked );
            btnconnect.Clicked += new EventHandler( btnconnect_Clicked );
            btnconnect.Hide(); // placeholder

            ListStore liststore = new ListStore( typeof( string ) );
            serverstreeview.Model = liststore;

            serverstreeview.AppendColumn( "Server:", new CellRendererText(), "text", 0 );

            serverstreeview.ShowAll();

            foreach (string server in serverlist)
            {
                liststore.AppendValues( server );
            }
        }

        void btnconnect_Clicked( object sender, EventArgs e )
        {
        }

        void btnclose_Clicked( object sender, EventArgs e )
        {
            Hide();
        }

        public void Hide()
        {
            availableserversdialog.Destroy();
            availableserversdialog = null;
        }
    }
}
