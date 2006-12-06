// Copyright Hugh Perkins 2005,2006
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
    // This form is the main property dialog box for Entities.
    //
    // we register with selectionmodel to receive selection changed events
    // if a single entity is selected, we register ourselves in the renderer's contextmenu
    //
    // We are using the layout manager from http://dotnet.jku.at/projects/slm/ ( which you should have in the project files at slm\LayoutMgr.dll , or similar  )
    public class EntityPropertiesDialog : Form, IPropertyController
    {
        SelectionModel selectionmodel;
        Entity thisentity = null;
        
        object ourmenuregistration = null;
        
        LayoutManager.LayoutControl layoutcontrol;
        
        ArrayList PropertyInfos = new ArrayList();
        Hashtable ControlsIndex = new Hashtable();
                
        static EntityPropertiesDialog instance = new EntityPropertiesDialog();
        public static EntityPropertiesDialog GetInstance()
        {
            return instance;
        }
        
        void LayoutNewProperty( string name, Control control )
        {
            LayoutManager.LayoutControl rowlayout = new LayoutManager.LayoutControl(new LayoutManager.RubberLayout());
            rowlayout.Size = new Size(100,100);
            
            Label label = new Label();
            label.Text = name;
            label.Bounds = new Rectangle( 0, 0, 30, 100 ); // position (0,0), 30% width, 100% height
            rowlayout.Controls.Add( label );
            
            control.Bounds = new Rectangle( 30, 0, 70, 100 );
            rowlayout.Controls.Add( control );
            
            layoutcontrol.Controls.Add( rowlayout );
        }
        
        void AddTextProperty(string name, string initialvalue, int length )
        {
            TextBox textbox = new TextBox();
            textbox.Text = initialvalue;
            textbox.TextChanged += new EventHandler( FormValueChanged );
            LayoutNewProperty( name, textbox );
            
            ControlsIndex.Add( name, textbox );
        }
        
        void AddSliderIntProperty( string name, int initialvalue, int min, int max )
        {
            TrackBar trackbar = new TrackBar();
            trackbar.Minimum = min;
            trackbar.Maximum = max;
            trackbar.Value = initialvalue;
            trackbar.SmallChange = 1;
            trackbar.TickFrequency = ( max - min ) / 10;
            trackbar.ValueChanged += new EventHandler( FormValueChanged );
            
            LayoutNewProperty( name, trackbar );
            ControlsIndex.Add( name, trackbar );
        }
        
        void AddButton( Control.ControlCollection Controls, string text, EventHandler handler )
        {
            Button button = new Button();
            button.Text = text;
            Controls.Add( button );
            button.Click += handler;
        }
        
        void SetFormSize()
        {
            Size = new Size( 500, 500 );
        }        
        
        public void FormValueChanged( object source, EventArgs e )
        {
            Console.WriteLine("FormValueChanged " + source.ToString() );
            string name = "";
            foreach( DictionaryEntry controlobject in ControlsIndex )
            {
                if( source == controlobject.Value )
                {
                    name = (string)controlobject.Key;
                }
            }
            foreach( object propertyinfoobject in PropertyInfos )
            {
                PropertyInfo propertyinfo = propertyinfoobject as PropertyInfo;
                if( propertyinfo.Name == name )
                {
                    WritePropertyInfo( propertyinfo );
                }
            }
        }
        
        public void LoadProperty( string name, object value )
        {
            Control targetcontrol = (Control)ControlsIndex[name];
            if( targetcontrol is TextBox )
            {
                ( (TextBox)targetcontrol ).Text = value.ToString();
            }
        }
        
        public string ReadStringPropertyFromForm( string name )
        {
            TextBox control = (TextBox)ControlsIndex[name];
            return control.Text;
        }
        
        public int ReadIntPropertyFromForm( string name )
        {
            TrackBar control = (TrackBar)ControlsIndex[name];
            return control.Value;
        }
        
        public EntityPropertiesDialog()
        {
            selectionmodel = SelectionModel.GetInstance();
            //selectionmodel.ChangedEvent += new SelectionModel.ChangedHandler( SelectionChangedHandler );
            // ourmenuregistration = RendererFactory.GetInstance().RegisterContextMenu(new string[]{ "&Entity","&Properties" }, new ContextMenuCallback( ContextMenuProperties ) );
            ContextMenuController.GetInstance().ContextMenuPopup += new ContextMenuHandler( _ContextMenuPopup );
            
            Text = "Entity Properties";            
            
            // we use a Rubber Layout (proportional layout) to position a vertical QLayout for the properties, and a horizontal QLayout for the buttons
    
            layoutcontrol = new LayoutManager.LayoutControl( new LayoutManager.QLayout(LayoutManager.QLayout.Direction.Vertical ) );
            layoutcontrol.Bounds = new Rectangle( 5, 5, 90, 80 ); // allow a small border area, plus space for buttons
            
            LayoutManager.LayoutControl buttonarea = new LayoutManager.LayoutControl( new LayoutManager.QLayout( LayoutManager.QLayout.Direction.Horizontal) );
            buttonarea.Bounds = new Rectangle( 5, 85, 90, 10 );
            
            AddButton( buttonarea.Controls, "&Cancel",new EventHandler( OnClick_CancelButton ) );
            AddButton( buttonarea.Controls, "&Apply",new EventHandler( OnClick_ApplyButton ) );
            AddButton( buttonarea.Controls, "&Ok",new EventHandler( OnClick_OkButton ) );

            LayoutManager.LayoutControl framecontrol = new LayoutManager.LayoutControl( new LayoutManager.RubberLayout() );
            framecontrol.Size = new Size( 100, 100 );
            framecontrol.Controls.Add( layoutcontrol );
            framecontrol.Controls.Add( buttonarea );

            framecontrol.Dock = DockStyle.Fill;
            Controls.Add( framecontrol );
            
            SetFormSize();
            
            Closing += new System.ComponentModel.CancelEventHandler( _Closing );
        }
        
        public void _ContextMenuPopup( object source, ContextMenuArgs e )
        {
        //    if( selectionmodel.GetNumSelected() == 1 )
          //  {
            thisentity = e.Entity;
            if( thisentity != null )
            {
                Console.WriteLine("EntityPropertiesDialog registering in contextmenu");
                ContextMenuController.GetInstance().RegisterContextMenu(new string[]{ "&Properties" }, new ContextMenuHandler( ContextMenuProperties ) );
            }
           // }
        }
        
        protected void OnClick_ApplyButton( object source, EventArgs e )
        {
            WriteProperties();
        }
        
        protected void OnClick_OkButton( object source, EventArgs e )
        {
            WriteProperties();
            Hide();
        }
        
        protected void OnClick_CancelButton( object source, EventArgs e )
        {
            Hide();
        }
        
        // we trap the closing event and cancel it
        // if we dont cancel it, the form gets partially garbage collected.  Maybe there is a solution for this?
        // if we hide it here, showing it later on doesnt work correcty.  Maybe there is a solution for this?
        // For now, we simply trap and cancel the event.
        protected void _Closing( object source, System.ComponentModel.CancelEventArgs e )
        {
            //WriteProperties();
            e.Cancel = true;  // simply cancel the event, otherwise the form gets garbage collected
            // Hide();  // if we hide the form during this event, unfortunately showing the form later doesnt seem to work correctly
        }
        
        void LoadProperties()
        {
            foreach( object propertyinfoobject in PropertyInfos )
            {
                Console.WriteLine( propertyinfoobject.ToString() );
                if( propertyinfoobject is IntPropertyInfo )
                {
                    IntPropertyInfo intpropertyinfo = propertyinfoobject as IntPropertyInfo;
                    AddSliderIntProperty( intpropertyinfo.Name, intpropertyinfo.InitialValue, intpropertyinfo.Min, intpropertyinfo.Max );
                }
                else if( propertyinfoobject is StringPropertyInfo )
                {
                    StringPropertyInfo stringpropertyinfo = propertyinfoobject as StringPropertyInfo;
                    AddTextProperty( stringpropertyinfo.Name, stringpropertyinfo.InitialValue, stringpropertyinfo.Length );
                }
            }
        }
        
        void WritePropertyInfo( PropertyInfo propertyinfo )
        {
            Console.WriteLine("WritePropertyInfo " + propertyinfo.ToString() );
            if( propertyinfo is IntPropertyInfo )
            {
                ((IntPropertyInfo)propertyinfo).Handler( ReadIntPropertyFromForm( propertyinfo.Name ) );
            }
            else if( propertyinfo is StringPropertyInfo )
            {
                ((StringPropertyInfo)propertyinfo).Handler( ReadStringPropertyFromForm( propertyinfo.Name ) );
            }
        }
        
        void WriteProperties()
        {
            foreach( object propertyinfoobject in PropertyInfos )
            {
                WritePropertyInfo( propertyinfoobject as PropertyInfo );
            }
        }
        
        public void RegisterIntProperty( string name, int currentvalue, int Min, int Max, SetIntPropertyHandler handler )
        {
            Console.WriteLine("RegisterIntProperty " + name );
            PropertyInfos.Add( new IntPropertyInfo( name, currentvalue, Min, Max, handler ) );
        }
        
        public void RegisterStringProperty( string name, string currentvalue, int Length, SetStringPropertyHandler handler )
        {
            Console.WriteLine("RegisterStringProperty " + name );
            PropertyInfos.Add( new StringPropertyInfo( name, currentvalue, Length, handler ) );
        }
        
        public void ContextMenuProperties( object source, ContextMenuArgs e )
        {
            Test.WriteOut( "opening context menu...");
            
            PropertyInfos = new ArrayList();
            ControlsIndex = new Hashtable();
            
            layoutcontrol.Controls.Clear();
            thisentity.RegisterProperties( this );
            LoadProperties();
            //TopMost = true; // topmost causes horrible lag, better not use this...
            Show();
            Activate();
            Test.WriteOut( "...opened");
        }  
    }
}
