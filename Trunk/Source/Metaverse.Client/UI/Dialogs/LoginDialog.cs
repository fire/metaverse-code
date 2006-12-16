using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;

namespace OSMP
{
    public class LoginDialog
    {
        [Widget]
        Entry entryusername = null;

        [Widget]
        Entry entrypassword = null;

        [Widget]
        Button btnok = null;

        [Widget]
        Window loginwindow = null;

        public string Login
        {
            get
            {
                return entryusername.Text;
            }
        }

        public string Password
        {
            get
            {
                return entrypassword.Text;
            }
        }

        void on_btnok_clicked(object o, EventArgs e)
        {
            loginwindow.Destroy();
            parent.Login(Login, Password);
        }

        ChatController parent;

        public LoginDialog( ChatController parent )
        {
            this.parent = parent;

            Glade.XML app = new Glade.XML("./metaverse.client.glade", "loginwindow", "");
            app.Autoconnect(this);

            entrypassword.Activated += new EventHandler(entrypassword_Activated);
            entryusername.Activated += new EventHandler(entryusername_Activated);
        }

        void entryusername_Activated(object sender, EventArgs e)
        {
            on_btnok_clicked(sender, e);
        }

        void entrypassword_Activated(object sender, EventArgs e)
        {
            on_btnok_clicked(sender, e);
        }
    }
}
