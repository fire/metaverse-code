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
using System.Windows.Forms;
using System.Drawing;

namespace OSMP
{
    class LoginCredentialsForm : Form
    {
        public string Login{
            get{ return login.Text; }
        }
        public string Password{
            get{ return password.Text; }
        }
        
        TextBox login;
        TextBox password;
        
        public LoginCredentialsForm()
        {
            LayoutManager.LayoutControl border = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
            border.TabStop = false;
            border.Size = new Size( 100, 100 );
            
            LayoutManager.LayoutControl layout = new LayoutManager.LayoutControl( new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Vertical ) );
            layout.TabStop = false;
            layout.Bounds = new Rectangle( 5, 5, 90, 90 );
            border.Controls.Add( layout );

            LayoutManager.LayoutControl loginrowlayout = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
            loginrowlayout.TabStop = false;
            loginrowlayout.Size = new Size(100,100);
            
            Label intro = new Label();
            intro.TabStop = false;
            intro.Text = "Welcome to OSMP!";
            
            Label description = new Label();
            description.TabStop = false;
            description.Text = "Please enter a login, and (optionally) a password.  This will be used to connect you to FreeNet IRC";
            
            Label loginlabel = new Label();
            loginlabel.TabStop = false;
            loginlabel.Text = "Login:";
            loginlabel.Bounds = new Rectangle( 0, 0, 30, 100 );
            loginrowlayout.Controls.Add( loginlabel );
            
            login = new TextBox();
            login.Bounds = new Rectangle( 30, 0, 70, 100 );
            loginrowlayout.Controls.Add( login );
            
            LayoutManager.LayoutControl passwordrowlayout = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
            passwordrowlayout.TabStop = false;
            passwordrowlayout.Size = new Size(100,100);
            
            Label passwordlabel = new Label();
            passwordlabel.TabStop = false;
            passwordlabel.Text = "Password:";
            passwordlabel.Bounds = new Rectangle( 0, 0, 30, 100 );
            passwordrowlayout.Controls.Add( passwordlabel );
            
            password = new TextBox();
            password.Bounds = new Rectangle( 30, 0, 70, 100 );
            passwordrowlayout.Controls.Add( password );
            
            LayoutManager.LayoutControl buttonlayout = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
            buttonlayout.TabStop = false;
            buttonlayout.Size = new Size(100,100);
            Button submitbutton = new Button();
            submitbutton.Text = "Submit";
            submitbutton.Bounds = new Rectangle( 15, 0, 50, 100 );
            submitbutton.Click += new EventHandler( submitbutton_click );
            buttonlayout.Controls.Add( submitbutton );
            AcceptButton = submitbutton;
            
            layout.Controls.Add( intro );
            layout.Controls.Add( description );
            layout.Controls.Add( loginrowlayout );
            layout.Controls.Add( passwordrowlayout );
            layout.Controls.Add( buttonlayout );
            
            border.Location = new Point( 0, 0 );
            border.Dock = DockStyle.Fill;
            Controls.Add( border );
            
            Text = "Login";
            Size = new Size( 450, 200 );
            
            TopMost = true;
        }
        
        public void submitbutton_click( object source, EventArgs e )
        {
            Hide();
        }
    }
    
    public class ChatController
    {
        Panel IMPanel;
        TextBox chathistory;
        TextBox chatentry;
        
        //static ChatController instance = new ChatController();
        //public static ChatController GetInstance(){ return instance; }
        
        public ChatController()
        {
            MetaverseClient.GetInstance().Tick += new MetaverseClient.TickHandler(ChatController_Tick);

            KeyFilterComboKeys.GetInstance().RegisterCombo(new string[]{"activatechat"},null, new KeyComboHandler( EnterChat ) );
            
            IMPanel = RendererFactory.GetInstance().IMPanel;
            
            Login();
            
            LayoutManager.LayoutControl layout = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
            layout.TabStop = false;
            layout.Size = new Size(100,100);
            
            //Panel chatpanel = new Panel();
            //chatpanel.Bounds = new Rectangle( 0, 0, 100, 80 );
            //chatpanel.AutoScroll = true;
            chathistory = new TextBox();
            chathistory.TabStop = false;
            //chathistory.Dock = DockStyle.Fill;
            chathistory.Bounds = new Rectangle( 0, 0, 100 , 80 );
            chathistory.Multiline = true;
            chathistory.ReadOnly = true;
            chathistory.AcceptsReturn = true;
            chathistory.ScrollBars = ScrollBars.Vertical;
            //chatpanel.Controls.Add( chathistory );
            //layout.Controls.Add( chatpanel );
            layout.Controls.Add( chathistory );
            
            chatentry = new TextBox();
            chatentry.Bounds = new Rectangle( 0, 80, 90, 20 );
            chatentry.KeyDown += new KeyEventHandler( this._KeyDown );
            layout.Controls.Add( chatentry );
            
            Button submitbutton = new Button();
            submitbutton.Bounds = new Rectangle( 90, 80, 10, 20 );
            submitbutton.Text = "Submit";
           // submitbutton.IsDefault = true;
            submitbutton.Click += new EventHandler( SubmitButton_Click );
            layout.Controls.Add( submitbutton );
                        
            layout.Location = new Point( 0, 0 );
            layout.Dock = DockStyle.Fill;
            IMPanel.Controls.Add( layout );
            
            IMPanel.KeyDown +=  new KeyEventHandler( this._KeyDown );
            
            ImImplementationFactory.GetInstance().MessageReceived += new MessageReceivedHandler( MessageReceived );
        }

        void ChatController_Tick()
        {
            ImImplementationFactory.GetInstance().CheckMessages();
        }
        
        public void EnterChat( object source, ComboKeyEventArgs e )
        {
            if( e.IsComboDown )
            {
                chatentry.Focus();
            }
        }
        
        void Login()
        {
            LoginCredentialsForm logincredentialsform = new LoginCredentialsForm();
            logincredentialsform.ShowDialog();
            Console.WriteLine( this.GetType().ToString() + " trying to login as " + logincredentialsform.Login + " ... " );
            ImImplementationFactory.GetInstance().Login( logincredentialsform.Login, logincredentialsform.Password );
        }
        
        void _KeyDown( object source, KeyEventArgs e )
        {
            if( e.KeyCode == Keys.Enter )
            {
                if( chatentry.Text != "" )
                {
                    _Submit();                
                }
                else
                {
                    RendererFactory.GetInstance().MakeRendererActiveControl();
                }
            }
        }
        
        void _Submit()
        {
            ImImplementationFactory.GetInstance().SendMessage( chatentry.Text );
            chatentry.Text = "";
        }
        
        void SubmitButton_Click( object source, EventArgs e )
        {
            _Submit();
        }
        //int numlines = 0;
        void MessageReceived( object source, MessageReceivedArgs e )
        {
            Console.WriteLine("message received: " + e.MessageText );
            chathistory.Text += Environment.NewLine + e.MessageText;
            //numlines++;
            //chathistory.Size = new Size( chathistory.PreferredHeight * numlines, chathistory.Size.Width );
            
            //chathistory.AutoSize = true;
            //chathistory.ScrollToCaret();
            chathistory.SelectionStart = chathistory.Text.Length;
            chathistory.ScrollToCaret();
        }
    }
}
