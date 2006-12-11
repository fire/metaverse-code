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
using System.Windows.Forms;

namespace OSMP
{
    // example keyboard and menu plugin
    // This class provides a Quit functionality via the Escape key, context menu, and main menu
    public class HelpAbout
    {
        static HelpAbout instance = new HelpAbout(); // instantiate handler
        public static HelpAbout GetInstance()
        {
            return instance;
        }
        
        public HelpAbout()
        {
            Console.WriteLine("instantiating HelpAbout" );
            MenuController.GetInstance().RegisterMainMenu(new string[]{ "&Help","&About..." }, new MainMenuCallback( About ) );
        }
        public void About()
        {
            DialogHelper.GetInstance().ShowInfoMessage("About OSMP",
                "Written by Hugh Perkins hughperkins@gmail.com"  + Environment.NewLine +
                ""  + Environment.NewLine +
                "Website at http://metaverse.sf.net by Zenaphex" + Environment.NewLine +
                ""  + Environment.NewLine +
                "Thanks to everyone who has provided support, advice, code:"  + Environment.NewLine +
                "Jorge Lima, Christopha Omega, Jack Didgeridoo, Carnildo, Francis Chang, Peter Amstutz, Reed Hedges" + Environment.NewLine +
                "William Knight, Morgaine, Khamon, Jarod, Nick Merrill, Dan Miller, Sadie Wang, and many others who I've missed."
            );
        }
    }
}