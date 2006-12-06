// Copyright Hugh Perkins 2004,2005,2006
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
using System.Windows.Forms;

namespace OSMP
{
    // provides a couple of functions to help manipulate menus at runtime
    public class MenuHelper
    {
        public static MenuItem CreateMenuItemInTree( string[] contextmenupath, Menu.MenuItemCollection thesemenuitems )
        {
            for( int i = 0; i < contextmenupath.GetUpperBound(0) + 1 - 1; i++ )
            {
                bool bAlreadyCreated = false;
                for( int j = 0; j < thesemenuitems.Count && !bAlreadyCreated; j++ )
                {
                    if( thesemenuitems[j].Text == contextmenupath[i] )
                    {
                        bAlreadyCreated = true;
                        thesemenuitems = thesemenuitems[j].MenuItems;
                    }
                }
                if( !bAlreadyCreated )
                {
                    MenuItem newitem = new MenuItem( contextmenupath[i] );
                    thesemenuitems.Add( newitem );
                    thesemenuitems = newitem.MenuItems;
                }
            }
            MenuItem commanditem = new MenuItem( contextmenupath[ contextmenupath.GetUpperBound(0) ]);
            thesemenuitems.Add( commanditem );
            return commanditem;
        }
    }
}
