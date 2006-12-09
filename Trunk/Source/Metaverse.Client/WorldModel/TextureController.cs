// Copyright Hugh Perkins 2006
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
using DevIL;

namespace OSMP
{
    // This is responsible for loading a texture progressively
    // we can add an extra thread into this class to progressively change the OpenGL texture as the texture is loading
    // note to self: currently only support File Uris.
    public class TextureProxy
    {
        public string UriString // this was created specifically for XmlSerializer usage
        {
            get{
                return ProjectFileController.GetInstance().GetRelativePathString( uri );
            }
            set{
                this.uri = ProjectFileController.GetInstance().CreateUriFromRelativePathString( value );
                idingraphicsengine = _LoadUri( uri );
            }
        }
        
        object _LoadUri( Uri uri )
        {
            return TextureHelper.LoadBitmapToOpenGl( DevIL.DevIL.LoadBitmap( uri.LocalPath ) ); // need to check if DevIL handles generic URLs, but anyway we should probably cache them locally, as files
        }
        
        Uri uri;
        public object IdInGraphicsEngine{ get{ return idingraphicsengine; } } // since this doesnt have a set, this doesnt get saved to xml by serializer, which is correct behavior
        object idingraphicsengine = 0;
        bool IsLoaded{
            get{ return true; }
        }
        public TextureProxy() // for XmlSerializer Usage
        {
        }
        public TextureProxy( Uri uri )
        {
            this.uri = uri;
            idingraphicsengine = _LoadUri( uri );
        }
    }
    
    public class TextureController
    {
        public Hashtable TextureProxies;
        
        static TextureController instance = new TextureController();
        public static TextureController GetInstance(){ return instance; }
        
        public TextureController()
        {
            TextureProxies = new Hashtable();
        }
        public object LoadUri( Uri uri )
        {
            if( !TextureProxies.Contains( uri.ToString() ) )
            {
                TextureProxies.Add( uri.ToString(), new TextureProxy( uri ) );
            }
            return ( (TextureProxy)TextureProxies[ uri.ToString() ] ).IdInGraphicsEngine;
        }
    }
}
