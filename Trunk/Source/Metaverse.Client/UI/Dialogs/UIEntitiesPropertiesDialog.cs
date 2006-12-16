﻿// Copyright Hugh Perkins 2005,2006
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
using System.Collections.Generic;
using Glade;
using Gtk;

namespace OSMP
{
    // This form is the main property dialog box for Entities.
    //
    // we register with selectionmodel to receive selection changed events
    // if a single entity is selected, we register ourselves in the renderer's contextmenu
    public class UIEntityPropertiesDialog : IPropertyController
    {
        [Widget]
        Table propertytable = null;

        [Widget]
        Window entitypropertiesdialog = null;

        SelectionModel selectionmodel;
        Entity thisentity = null;

        object ourmenuregistration = null;

        List<PropertyInfo> PropertyInfos = new List<PropertyInfo>();
        Dictionary<string,Widget> ControlsIndex = new Dictionary<string,Widget>();

        static UIEntityPropertiesDialog instance = new UIEntityPropertiesDialog();
        public static UIEntityPropertiesDialog GetInstance()
        {
            return instance;
        }

        void AddTextProperty(string name, string initialvalue, int length)
        {
            Entry textbox = new Entry();
            textbox.Text = initialvalue;
            textbox.Changed += new EventHandler(FormValueChanged);
            //LayoutNewProperty(name, textbox);

            propertytable.Attach(new Label(name), 0, 1, (uint)nextpropertyindex, (uint)nextpropertyindex + 1);
            propertytable.Attach(textbox, 1, 2, (uint)nextpropertyindex, (uint)nextpropertyindex + 1);
            nextpropertyindex++;
            ControlsIndex.Add(name, textbox);
        }
        
        void AddSliderIntProperty(string name, int initialvalue, int min, int max)
        {
            Gtk.HScale trackbar = new HScale(min, max, (max - min )/ 10);
            trackbar.Value = initialvalue;
            trackbar.ValueChanged += new EventHandler(FormValueChanged);

            propertytable.Attach(new Label(name), 0, 1, (uint)nextpropertyindex, (uint)nextpropertyindex + 1);
            propertytable.Attach(trackbar, 1, 2, (uint)nextpropertyindex, (uint)nextpropertyindex + 1);
            nextpropertyindex++;
        //    LayoutNewProperty(name, trackbar);
            ControlsIndex.Add(name, trackbar);
        }

        void SetFormSize()
        {
        }

        public void SliderValueChanged(object source, MoveSliderArgs e)
        {
            Console.WriteLine("FormValueChanged " + source.ToString());
            string name = "";
            foreach (string thisname in ControlsIndex.Keys)
            {
                if (source == ControlsIndex[thisname])
                {
                    name = thisname;
                }
            }
            foreach (object propertyinfoobject in PropertyInfos)
            {
                PropertyInfo propertyinfo = propertyinfoobject as PropertyInfo;
                if (propertyinfo.Name == name)
                {
                    WritePropertyInfo(propertyinfo);
                }
            }
            MetaverseClient.GetInstance().worldstorage.OnModifyEntity(thisentity);
        }

        public void FormValueChanged(object source, EventArgs e)
        {
            Console.WriteLine("FormValueChanged " + source.ToString());
            string name = "";
            foreach (string thisname in ControlsIndex.Keys)
            {
                if (source == ControlsIndex[thisname])
                {
                    name = thisname;
                }
            }
            foreach (object propertyinfoobject in PropertyInfos)
            {
                PropertyInfo propertyinfo = propertyinfoobject as PropertyInfo;
                if (propertyinfo.Name == name)
                {
                    WritePropertyInfo(propertyinfo);
                }
            }
            MetaverseClient.GetInstance().worldstorage.OnModifyEntity(thisentity);
        }

        public void LoadProperty(string name, object value)
        {
            Widget targetcontrol = ControlsIndex[name];
            if (targetcontrol is Entry)
            {
                ((Entry)targetcontrol).Text = value.ToString();
            }
        }

        public string ReadStringPropertyFromForm(string name)
        {
            Entry control = (Entry)ControlsIndex[name];
            return control.Text;
        }

        public int ReadIntPropertyFromForm(string name)
        {
            HScale control = (HScale)ControlsIndex[name];
            return (int)control.Value;
        }

        public UIEntityPropertiesDialog()
        {
            selectionmodel = SelectionModel.GetInstance();
            ContextMenuController.GetInstance().ContextMenuPopup += new ContextMenuHandler(_ContextMenuPopup);

        //    SetFormSize();
        }

        public void _ContextMenuPopup(object source, ContextMenuArgs e)
        {
            //    if( selectionmodel.GetNumSelected() == 1 )
            //  {
            thisentity = e.Entity;
            if (thisentity != null)
            {
                Console.WriteLine("EntityPropertiesDialog registering in contextmenu");
                ContextMenuController.GetInstance().RegisterContextMenu(new string[] { "&Edit" }, new ContextMenuHandler(ContextMenuProperties));
            }
            // }
        }

        void on_btnclose_clicked(object source, EventArgs e)
        {
            Console.WriteLine("close button clicked");
            WriteProperties();
            entitypropertiesdialog.Destroy();
        }

        void LoadProperties()
        {
            foreach (object propertyinfoobject in PropertyInfos)
            {
                Console.WriteLine(propertyinfoobject.ToString());
                if (propertyinfoobject is IntPropertyInfo)
                {
                    IntPropertyInfo intpropertyinfo = propertyinfoobject as IntPropertyInfo;
                    AddSliderIntProperty(intpropertyinfo.Name, intpropertyinfo.InitialValue, intpropertyinfo.Min, intpropertyinfo.Max);
                }
                else if (propertyinfoobject is StringPropertyInfo)
                {
                    StringPropertyInfo stringpropertyinfo = propertyinfoobject as StringPropertyInfo;
                    AddTextProperty(stringpropertyinfo.Name, stringpropertyinfo.InitialValue, stringpropertyinfo.Length);
                }
            }
        }

        void WritePropertyInfo(PropertyInfo propertyinfo)
        {
            Console.WriteLine("WritePropertyInfo " + propertyinfo.ToString());
            if (propertyinfo is IntPropertyInfo)
            {
                ((IntPropertyInfo)propertyinfo).Handler(ReadIntPropertyFromForm(propertyinfo.Name));
            }
            else if (propertyinfo is StringPropertyInfo)
            {
                ((StringPropertyInfo)propertyinfo).Handler(ReadStringPropertyFromForm(propertyinfo.Name));
            }
        }

        void WriteProperties()
        {
            foreach (object propertyinfoobject in PropertyInfos)
            {
                WritePropertyInfo(propertyinfoobject as PropertyInfo);
            }
        }

        public void RegisterIntProperty(string name, int currentvalue, int Min, int Max, SetIntPropertyHandler handler)
        {
            Console.WriteLine("RegisterIntProperty " + name);
            PropertyInfos.Add(new IntPropertyInfo(name, currentvalue, Min, Max, handler));
        }

        public void RegisterStringProperty(string name, string currentvalue, int Length, SetStringPropertyHandler handler)
        {
            Console.WriteLine("RegisterStringProperty " + name);
            PropertyInfos.Add(new StringPropertyInfo(name, currentvalue, Length, handler));
        }

        int nextpropertyindex = 0;

        public void ContextMenuProperties(object source, ContextMenuArgs e)
        {
            if (entitypropertiesdialog != null)
            {
                entitypropertiesdialog.Destroy();
            }

            Test.WriteOut("opening properties box ...");

            Glade.XML app = new Glade.XML("./metaverse.client.glade", "entitypropertiesdialog", "");
            app.Autoconnect(this);            
            
            PropertyInfos = new List<PropertyInfo>();
            ControlsIndex = new Dictionary<string,Widget>();
            nextpropertyindex = 0;

            thisentity.RegisterProperties(this);
            LoadProperties();
            // Show();

            // hack to add a semblance of user-friendliness
            DialogHelpers.ShowInfoMessage(null, "Hold down z to move the object, x to change scale and v to rotate.");

            entitypropertiesdialog.ShowAll();
            //entitypropertiesdialog.Show();

            selectionmodel.Clear();
            selectionmodel.ToggleObjectInSelection(thisentity, true);
            Test.WriteOut("...opened");
        }
    }
}
