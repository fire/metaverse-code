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
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;

namespace OSMP
{
    public class ContextMenuArgs : EventArgs
    {
        public int MouseX;
        public int MouseY;
        public Entity Entity;
        public ContextMenuArgs(int mousex, int mousey, Entity entity)
        {
            MouseX = mousex; MouseY = mousey; this.Entity = entity;
        }
    }

    public delegate void ContextMenuHandler(object source, ContextMenuArgs e);

    public class UIContextMenu
    {
        List<MenuItem> contextmenucommanditems = new List<MenuItem>();
        List<ContextMenuHandler> contextmenucallbacks = new List<ContextMenuHandler>();

        List<string[]> persistentcontextmenupaths = new List<string[]>();
        List<ContextMenuHandler> persistentcontextmenucallbacks = new List<ContextMenuHandler>();

        int iMouseX;
        int iMouseY;
        Entity entity;

        public UIContextMenu()
        {
            RendererFactory.GetInstance().ContextMenuPopup += new EventHandler(ContextMenu_ContextMenuPopup);
            //UIController.GetInstance().contextmenu.RegisterContextMenu(contextmenupath, callback);
            MouseFilterFormsMouseCache.GetInstance().MouseDown += new System.Windows.Forms.MouseEventHandler(GtkContextMenu_MouseDown);

        }

        void GtkContextMenu_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (contextmenu != null)
                {
                    contextmenu.Destroy();
                }
            }
        }

        Menu contextmenu;

        void ContextMenu_ContextMenuPopup(object sender, EventArgs e)
        {
            if (contextmenu != null)
            {
                contextmenu.Destroy();
            }

            contextmenu = new Menu();

            MenuItem submenuitem = new MenuItem("About TestPlugin...");
            submenuitem.Activated += new EventHandler(Testpluginmenuitem_Activated);
            contextmenu.Add(submenuitem);
            contextmenu.Add(new MenuItem("item3"));

            IMouseFilterMouseCache mousefiltermousecache = MouseFilterMouseCacheFactory.GetInstance();
            iMouseX = mousefiltermousecache.MouseX;
            iMouseY = mousefiltermousecache.MouseY;

            entity = Picker3dController.GetInstance().GetClickedEntity(iMouseX, iMouseY);

            contextmenucommanditems = new List<MenuItem>();
            contextmenucallbacks = new List<ContextMenuHandler>();

            AddPersistentItems();

            ContextMenuController.GetInstance().OnContextMenuPopup(this, new ContextMenuArgs(iMouseX, iMouseY, entity));

            contextmenu.ShowAll();
            contextmenu.Popup();
        }

        void Testpluginmenuitem_Activated( object sender, EventArgs e )
        {
            Console.WriteLine("testpluginmenuitem activated");
        }

        public void RegisterContextMenu( string[] contextmenupath, ContextMenuHandler callback )
        {
            _RegisterContextMenu( contextmenupath, callback );
        }

        void _RegisterContextMenu(string[] contextmenupath, ContextMenuHandler handler)
        {
            MenuItem commanditem = null;

            Console.WriteLine("scan existing:");
            Menu currentmenu = contextmenu;
            for (int i = 0; i < contextmenupath.Length - 1; i++)
            {
                bool foundpathsegment = false;
                string thissegment = contextmenupath[i].Replace("&", "_");
                foreach (Widget widget in currentmenu.Children)
                {
                    //Console.WriteLine(widget.GetType());
                    foreach (Widget subwidget in (widget as MenuItem).Children)
                    {
                        //Console.WriteLine(subwidget.GetType());
                        string thismenulabel = (subwidget as AccelLabel).Text;
                        //Console.WriteLine(thismenulabel + " " + thissegment);
                        if (thismenulabel == thissegment.Replace("_",""))
                        {
                            foundpathsegment = true;
                            currentmenu = (widget as MenuItem).Submenu as Menu;
                        }
                    }
                }
                if (!foundpathsegment)
                {
                    MenuItem nextmenuitem = new MenuItem(thissegment);
                    currentmenu.Add(nextmenuitem);

                    Menu nextmenu = new Menu();
                    nextmenuitem.Submenu = nextmenu;

                    currentmenu = nextmenu;
                }
            }

            commanditem = new MenuItem(contextmenupath[contextmenupath.Length - 1].Replace("&","_") );
            currentmenu.Add(commanditem);
            commanditem.Activated += new EventHandler(_ContextMenuClick);

            contextmenucommanditems.Add(commanditem);
            contextmenucallbacks.Add(handler);

            return;
        }

        public void RegisterPersistentContextMenu(string[] contextmenupath, ContextMenuHandler callback)
        {
            persistentcontextmenupaths.Add(contextmenupath);
            persistentcontextmenucallbacks.Add(callback);
        }

        void AddPersistentItems()
        {
            for (int i = 0; i < persistentcontextmenupaths.Count; i++)
            {
                _RegisterContextMenu( persistentcontextmenupaths[i], persistentcontextmenucallbacks[i]);
            }
        }

        void _ContextMenuClick(object o, EventArgs e)
        {
            //Console.WriteLine("contextmenuclick " + o);
            for (int i = 0; i < contextmenucallbacks.Count; i++)
            {
                if (contextmenucommanditems[i] == o)
                {
                    contextmenucallbacks[i](o, new ContextMenuArgs(iMouseX, iMouseY, entity));
                }
            }
        }

        /*
        MenuItem CreateMenuItemInTree( string[] contextmenupath, Widget[] thesemenuitems )
        {
            for( int i = 0; i < contextmenupath.Length - 1; i++ )
            {
                bool bAlreadyCreated = false;
                for( int j = 0; j < thesemenuitems.Length && !bAlreadyCreated; j++ )
                {
                    //if( thesemenuitems[j] == contextmenupath[i] )
                    //{
                      //  bAlreadyCreated = true;
                        //thesemenuitems = thesemenuitems[j].MenuItems;
                    //}
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
        */
    }
}
