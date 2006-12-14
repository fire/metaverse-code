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
            //RendererFactory.GetInstance().ContextMenuPopup += new EventHandler(ContextMenu_ContextMenuPopup);
            //UIController.GetInstance().contextmenu.RegisterContextMenu(contextmenupath, callback);
            MouseFilterFormsMouseCache.GetInstance().MouseDown += new System.Windows.Forms.MouseEventHandler(GtkContextMenu_MouseDown);
            MouseFilterFormsMouseCache.GetInstance().MouseUp += new System.Windows.Forms.MouseEventHandler(UIContextMenu_MouseUp);
        }

        void UIContextMenu_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        void GtkContextMenu_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Console.WriteLine(e.Button + " " + MouseFilterFormsMouseCache.GetInstance().RightMouseDown);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (contextmenu != null)
                {
                  //  contextmenu.Destroy();
                }
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu_ContextMenuPopup(sender, e);
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

            //MenuItem submenuitem = new MenuItem("About TestPlugin...");
            //submenuitem.Activated += new EventHandler(Testpluginmenuitem_Activated);
            //contextmenu.Add(submenuitem);
            //contextmenu.Add(new MenuItem("item3"));

            IMouseFilterMouseCache mousefiltermousecache = MouseFilterMouseCacheFactory.GetInstance();
            iMouseX = mousefiltermousecache.MouseX;
            iMouseY = mousefiltermousecache.MouseY;

            entity = Picker3dController.GetInstance().GetClickedEntity(iMouseX, iMouseY);

            contextmenucommanditems = new List<MenuItem>();
            contextmenucallbacks = new List<ContextMenuHandler>();

            AddPersistentItems();

            ContextMenuController.GetInstance().OnContextMenuPopup(this, new ContextMenuArgs(iMouseX, iMouseY, entity));

            contextmenu.ShowAll();
            contextmenu.Popup(null,null,null,IntPtr.Zero,3, Gtk.Global.CurrentEventTime);
        }

        //void _menupositionfunc( Menu menu, out int x, out int y, out bool pushin )
        //{
            //Console.WriteLine("_menupositionfunc mouse: " + MouseFilterFormsMouseCache.GetInstance().MouseX + " " + MouseFilterFormsMouseCache.GetInstance().MouseY );
            //Point renderertopleftscreencoords = RendererSdlCtrl.GetInstance().WindowTopLeftScreenCoords;
            //Console.WriteLine(renderertopleftscreencoords.X + " " + renderertopleftscreencoords.Y);
            //x = MouseFilterFormsMouseCache.GetInstance().MouseX - 1 + renderertopleftscreencoords.X;
            //y = MouseFilterFormsMouseCache.GetInstance().MouseY - 1 + renderertopleftscreencoords.Y;
            //pushin = true;
        //}

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

            //Console.WriteLine("scan existing:");
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
            Console.WriteLine("contextmenuclick " + o);
            for (int i = 0; i < contextmenucallbacks.Count; i++)
            {
                if (contextmenucommanditems[i] == o)
                {
                    contextmenucallbacks[i](o, new ContextMenuArgs(iMouseX, iMouseY, entity));
                }
            }
        }
    }
}
