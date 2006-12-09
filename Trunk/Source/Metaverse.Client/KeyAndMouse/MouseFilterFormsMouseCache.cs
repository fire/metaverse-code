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
    // caches mouse attributes, and makes them available via System.Forms.MouseEventHandler events
    // This class just forwards the raw events, but other IMouseFilterMouseCache derivatives could abstract mouse events from other sources (sdl...)
    // into System.Forms.MouseEventHandler format
    public class MouseFilterFormsMouseCache : IMouseFilterMouseCache
    {
        int _mousex;
        int _mousey;
        bool _leftbuttondown;
        bool _rightbuttondown;
        
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;        
            
        static MouseFilterFormsMouseCache instance = new MouseFilterFormsMouseCache();
        public static MouseFilterFormsMouseCache GetInstance()
        {
            return instance;
        }
        
        public MouseFilterFormsMouseCache()
        {
            IRenderer renderer = RendererFactory.GetInstance();
            renderer.MouseMove +=  new MouseEventHandler( this._MouseMove );
            renderer.MouseDown +=  new MouseEventHandler( this._MouseDown );
            renderer.MouseUp +=  new MouseEventHandler( this._MouseUp );
        }
        
        public int MouseX
        {
            get
            {
                return _mousex;
            }
        }        
        public int MouseY
        {
            get
            {
                return _mousey;
            }
        }
        public bool LeftMouseDown
        {
            get
            {
                return _leftbuttondown;
            }
        }
        public bool RightMouseDown
        {
            get
            {
                return _rightbuttondown;
            }
        }        
        
        void _MouseMove( object source, MouseEventArgs e )
        {
            _mousex = e.X;
            _mousey = e.Y;
            if( MouseMove != null )
            {
                MouseMove( source, e );
            }
        }
        
        void _MouseDown( object source, MouseEventArgs e )
        {
            _mousex = e.X;
            _mousey = e.Y;
            switch( e.Button )
            {
                case MouseButtons.Left:
                    _leftbuttondown = true;
                    break;
                case MouseButtons.Right:
                    _rightbuttondown = true;
                    break;
            }
            if( MouseDown != null )
            {
                MouseDown( source, e );
            }
        }
        
        void _MouseUp( object source, MouseEventArgs e )
        {
            _mousex = e.X;
            _mousey = e.Y;
            switch( e.Button )
            {
                case MouseButtons.Left:
                    _leftbuttondown = false;
                    break;
                case MouseButtons.Right:
                    _rightbuttondown = false;
                    break;
            }
            if( MouseUp != null )
            {
                MouseUp( source, e );
            }
        }
}
}
