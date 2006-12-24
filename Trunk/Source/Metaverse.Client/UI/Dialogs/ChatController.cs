// Copyright Hugh Perkins 2006
// hughperkins@gmail.com http://manageddreams.com
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
using System.Collections;
//using System.Windows.Forms;
using System.Drawing;
using Gtk;
using Glade;

namespace OSMP
{
    public class ChatController
    {
        //Panel IMPanel;

        [Widget]
        Gtk.Label chathistory = null;

        [Widget]
        Entry chatentry = null;

        [Widget]
        Window chatwindow = null;
        
        //static ChatController instance = new ChatController();
        //public static ChatController GetInstance(){ return instance; }

        //ChatWindow chatwindow;
        
        public ChatController()
        {
            LoginDialog logindialog = new LoginDialog( this );
        }

        public void Login(string username, string password)
        {
            LogFile.WriteLine(this.GetType().ToString() + " trying to login as " + username + " ... ");
            ImImplementationFactory.GetInstance().Login( username, password);
            //logindialog.Destroy();

            Glade.XML app = new Glade.XML( EnvironmentHelper.GetExeDirectory() + "/metaverse.client.glade", "chatwindow", "");
            app.Autoconnect(this);

            ImImplementationFactory.GetInstance().MessageReceived += new MessageReceivedHandler(MessageReceived);
            MetaverseClient.GetInstance().Tick += new MetaverseClient.TickHandler(ChatController_Tick);

            CommandCombos.GetInstance().RegisterCommand(
                "activatechat", new KeyCommandHandler(EnterChat));
        }

        void ChatController_Tick()
        {
            ImImplementationFactory.GetInstance().CheckMessages();
        }

        void on_btnSend_clicked(object o, EventArgs e)
        {
            LogFile.WriteLine("send clicked");
            if (chatentry.Text != "")
            {
                ImImplementationFactory.GetInstance().SendMessage(chatentry.Text);
                chatentry.Text = "";
            }
        }

        public void EnterChat( string command, bool down )
        {
            if( down )
            {
                LogFile.WriteLine("EnterChat");
                //chatentry.Focus();
                chatwindow.RootWindow.Hide();
                chatwindow.RootWindow.Show();
            }
        }
        
        //int numlines = 0;
        void MessageReceived( object source, MessageReceivedArgs e )
        {
            LogFile.WriteLine("message received: " + e.MessageText );
            chathistory.Text += Environment.NewLine + e.MessageText;

            //chathistory.SelectionStart = chathistory.Text.Length;
            //chathistory.ScrollToCaret();
        }
    }
}
