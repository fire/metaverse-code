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
using System.Collections;
using System.Windows.Forms;

namespace OSMP
{
    // This class points to the renderer obtained by RendererFactory
    // It converts the key codes returned by the renderer, which are assumed to be in System.Windows.Forms format
    // into the commandstrings defined in the configuration file (config.xml, via Config.cs )
    public class KeyFilterFormsConfigMappings : IKeyFilterConfigMappings
    {
        Config config;
        static KeyFilterFormsConfigMappings instance = new KeyFilterFormsConfigMappings();
            
        public Hashtable allpressedcommandkeysbykeycode = new Hashtable();
        public StringArrayList CommandKeysForLastKeyevent = new StringArrayList();
        public StringArrayList _AllPressedCommandKeys = new StringArrayList();
        
        public event KeyConfigMappingEventHandler KeyEvent;
            
        public static KeyFilterFormsConfigMappings GetInstance()
        {
            return instance;
        }
        public StringArrayList AllPressedCommandKeys{
            get{ return _AllPressedCommandKeys; }
        }
        public Hashtable AllPressedCommandKeysByKeyCode{
            get{ return allpressedcommandkeysbykeycode; }
        }
        public bool IsPressed( string commandstring )
        {
            if( AllPressedCommandKeys != null && AllPressedCommandKeys.Contains( commandstring ) )
            {
                return true;
            }
            return false;
        }
        public KeyFilterFormsConfigMappings()
        {
            config = Config.GetInstance();
            KeyFilterFormsKeyCache keycache = KeyFilterFormsKeyCache.GetInstance();
            keycache.KeyDown +=  new System.Windows.Forms.KeyEventHandler( this._KeyDown );
            keycache.KeyUp +=  new System.Windows.Forms.KeyEventHandler( this._KeyUp );
        }
        
        public string KeyCodeToKeyName( Keys keycode )
        {
            string skeyname = keycode.ToString().ToLower();
            if( skeyname == "shiftkey" )
            {
                skeyname = "shift";
            }                
            else if( skeyname == "controlkey" )
            {
                skeyname = "ctrl";
            }                
            return skeyname;	 
        }

        public StringArrayList GetCommandStringsForCode( Keys keycode )
        {
            string keyname = KeyCodeToKeyName( keycode );
            CommandKeysForLastKeyevent = (StringArrayList)config.CommandsByKeycode[ keyname ];
            return CommandKeysForLastKeyevent;
        }
        
        void _KeyDown( object source, System.Windows.Forms.KeyEventArgs e )
        {
            StringArrayList CommandStrings = GetCommandStringsForCode( e.KeyCode );
            if( CommandStrings != null )
            {
                foreach( string commandkey in CommandStrings )
                {
                    if( !_AllPressedCommandKeys.Contains( commandkey ) )
                    {
                        _AllPressedCommandKeys.Add( commandkey );
                    }
                }
                if( !allpressedcommandkeysbykeycode.Contains( e.KeyValue ) )
                {
                    allpressedcommandkeysbykeycode.Add( e.KeyValue, CommandStrings );
                }
                if( KeyEvent != null )
                {
                    KeyEvent( this, new KeyMappingEventArgs( true, CommandStrings ) );
                }
            }
        }
        
        void _KeyUp( object source, System.Windows.Forms.KeyEventArgs e )
        {
            StringArrayList CommandStrings = GetCommandStringsForCode( e.KeyCode );
            if( CommandStrings != null )
            {
                foreach( string commandkey in CommandStrings )
                {
                    if( _AllPressedCommandKeys.Contains( commandkey ) )
                    {
                        _AllPressedCommandKeys.Remove( commandkey );
                    }
                }
                if( allpressedcommandkeysbykeycode.Contains( e.KeyValue ) )
                {
                    allpressedcommandkeysbykeycode.Remove( e.KeyValue );
                }
                if( KeyEvent != null )
                {
                    KeyEvent( this, new KeyMappingEventArgs( false, CommandStrings ) );
                }
            }
        }
    }
}
