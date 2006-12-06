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
    public class KeyFilterFormsKeyCache
    {
        bool[] _Keys = new bool[ 513 ];
        
        public event KeyEventHandler KeyUp;
        public event KeyEventHandler KeyDown;
                
        static KeyFilterFormsKeyCache instance = new KeyFilterFormsKeyCache();
        public static KeyFilterFormsKeyCache GetInstance()
        {
            return instance;
        }
        public KeyFilterFormsKeyCache()
        {
            Test.Debug("Instantiating KeyFilterFormsKeyCache()");
            IRenderer renderer = RendererFactory.GetInstance();
            renderer.KeyDown +=  new KeyEventHandler( this._KeyDown );
            renderer.KeyUp +=  new KeyEventHandler( this._KeyUp );
        }
        void _KeyDown( object source, KeyEventArgs e )
        {
            Keys[ e.KeyValue ] = true;
            //Test.WriteOut("KeyFilterFormsKeyCache._KeyDown(" + e.KeyCode.ToString() + ")" );
            if( KeyDown != null )
            {
                //Test.WriteOut("KeyFilterFormsKeyCache Sending keydown(" + e.KeyCode.ToString() + ")" );
                KeyDown( source, e );
            }
        }
        
        void _KeyUp( object source, KeyEventArgs e )
        {
            //Test.WriteOut("KeyFilterFormsKeyCache._KeyUp(" + e.KeyCode.ToString() + ")" );
            Keys[ e.KeyValue ] = false;
            if( KeyUp != null )
            {
                KeyUp( source, e );
            }
        }
    
        public bool[]Keys
        {
            get
            {
                return _Keys;
            }
        }
    }
}
