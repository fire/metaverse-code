
using System;
using System.Windows.Forms;

namespace OSMP
{
    // its a singleton, but no particular reason for this at this time
    class DialogHelper
    {
        static DialogHelper instance = new DialogHelper();
        public static DialogHelper GetInstance()
        {
            return instance;
        }
        public void ShowInfoMessage( string title, string message )
        {
            MessageBox.Show( message, title );
        }
    }
}
