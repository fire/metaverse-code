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
using System.Collections;
using System.Windows.Forms;

namespace OSMP
{
    public class ContextMenuArgs : EventArgs
    {
        public int MouseX;
        public int MouseY;
        public Entity Entity;
        public ContextMenuArgs( int mousex, int mousey, Entity entity )
        {
            MouseX = mousex; MouseY = mousey; this.Entity = entity;
        }
    }
    
    public delegate void ContextMenuHandler( object source, ContextMenuArgs e );
    
    // Manages the context menu for us
    // hooks into Renderer's ContextMenuPopup event, and provides its own
    // uses MouseCache to populate its own event with mousex and mousey
    public class ContextMenuController
    {
        public event ContextMenuHandler ContextMenuPopup;
        public event ContextMenuHandler ContextMenuClick;
        
        ContextMenu contextmenu;
        
        int iMouseX;
        int iMouseY;
        Entity entity;
        
        ArrayList contextmenucommanditems = new ArrayList();
        ArrayList contextmenucallbacks = new ArrayList();
                
        ArrayList persistentcontextmenupaths = new ArrayList();
        ArrayList persistentcontextmenucallbacks = new ArrayList();
                
        static ContextMenuController instance = new ContextMenuController();
        public static ContextMenuController GetInstance()
        {
            return instance;
        }
        
        public ContextMenuController()
        {
            IRenderer renderer = RendererFactory.GetInstance();
            contextmenu = renderer.ContextMenu;
            renderer.ContextMenuPopup += new EventHandler( _ContextMenuPopup );
        }

        void _RegisterContextMenu( string[] contextmenupath, ContextMenuHandler handler )
        {
            Menu.MenuItemCollection thesemenuitems = contextmenu.MenuItems;
            
            MenuItem commanditem = MenuHelper.CreateMenuItemInTree( contextmenupath, thesemenuitems );
            commanditem.Click += new  EventHandler( _ContextMenuClick );
            
            contextmenucommanditems.Add( commanditem );
            contextmenucallbacks.Add( handler );
            
            return;
        }

        void AddPersistentItems()
        {
            for( int i = 0; i < persistentcontextmenupaths.Count; i++ )
            {
                _RegisterContextMenu( (string[])persistentcontextmenupaths[i], (ContextMenuHandler)persistentcontextmenucallbacks[i] );
            }
        }
        
        public void _ContextMenuPopup( object source, EventArgs e )
        {
            if( ContextMenuPopup == null )
            {
                return;
            }
            
            IMouseFilterMouseCache mousefiltermousecache = MouseFilterMouseCacheFactory.GetInstance();
            iMouseX = mousefiltermousecache.MouseX;
            iMouseY = mousefiltermousecache.MouseY;
            
            entity = Picker3dController.GetInstance().GetClickedEntity( iMouseX, iMouseY );
            
            contextmenu.MenuItems.Clear();
            contextmenucommanditems = new ArrayList();
            contextmenucallbacks = new ArrayList();
            
            ContextMenuPopup( source, new ContextMenuArgs( iMouseX, iMouseY, entity ) );

            AddPersistentItems();
        }
        
        // This context menu function is for ContextMenuPopup events
        // It will not persist.  Use this for menu items which are context dependent, like entity properties (doesnt display if no entity selected)
        public void RegisterContextMenu( string[] contextmenupath, ContextMenuHandler callback )
        {
            _RegisterContextMenu( contextmenupath, callback );
        }

        // This context menu function creates a persistent contextmenu.  This is good for things like Quit
        public void RegisterPersistentContextMenu( string[] contextmenupath, ContextMenuHandler callback )
        {
            persistentcontextmenupaths.Add( contextmenupath );
            persistentcontextmenucallbacks.Add( callback );
            
            return;
        }

        public void _ContextMenuClick( object o, EventArgs e )
        {
            for( int i = 0; i < contextmenucallbacks.Count; i++ )
            {
                if( contextmenucommanditems[i] == o )
                {
                    ((ContextMenuHandler)contextmenucallbacks[i])( o, new ContextMenuArgs( iMouseX, iMouseY, entity ) );
                }
            }
        }
    }
}
