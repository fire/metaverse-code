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
using Tao.DevIl;
using System.IO;
using Tao.OpenGl;

namespace OSMP
{
    public class TextureController
    {
        // This is responsible for loading a texture progressively
        // we can add an extra thread into this class to progressively change the OpenGL texture as the texture is loading
        // note to self: currently only support File Uris.
        class TextureProxy
        {
            public string UriString // this was created specifically for XmlSerializer usage
            {
                get
                {
                    return ProjectFileController.GetInstance().GetRelativePathString(uri);
                }
                set
                {
                    this.uri = ProjectFileController.GetInstance().CreateUriFromRelativePathString(value);
                    idingraphicsengine = _LoadUri(uri);
                }
            }

            // from http://svn.sourceforge.net/viewvc/boogame/trunk/BooGame/src/Texture.cs?view=markup
            // note to self: this is pretty inefficient, should use bit shifting
            int NextPowerOfTwo(int n)
            {
                double power = 0;
                while (n > Math.Pow(2.0, power))
                    power++;
                return (int)Math.Pow(2.0, power);
            }

            int LoadTexture(string filepath)
            {
                FileStream fs = new FileStream(filepath, FileMode.Open);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                fs.Close();

                // from http://svn.sourceforge.net/viewvc/boogame/trunk/BooGame/src/Texture.cs?view=markup
                // Generate an DevIL image
                int ilImage;
                Il.ilGenImages(1, out ilImage);

                // Bind the image so that we work with it
                Il.ilBindImage(ilImage);

                // Load the buffer
                if (!Il.ilLoadL(Il.IL_TYPE_UNKNOWN, bytes, bytes.Length))
                    throw new Exception("Failed to load image.");

                // Convert every colour component into unsigned byte. If your image contains alpha channel you can replace IL_RGB with IL_RGBA
                if (!Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE))
                    throw new Exception("Failed to convert image.");

                // Get details
                int m_BytesPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BPP);
                int m_Width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int m_Height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int m_Format = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);
                int m_Depth = Il.ilGetInteger(Il.IL_IMAGE_DEPTH);

                int m_TextureWidth = NextPowerOfTwo(m_Width);
                int m_TextureHeight = NextPowerOfTwo(m_Height);
                if ((m_TextureWidth != m_Width) || (m_TextureHeight != m_Height))
                    Ilu.iluEnlargeCanvas(m_TextureWidth, m_TextureHeight, m_Depth);

                //Ilu.iluFlipImage();

                int m_TextureID;
                // Generate GL name
                Gl.glGenTextures(1, out m_TextureID);

                // Bind GL texture
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, m_TextureID);

                // Use linear interpolation for magnification filter
                Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);

                // Use linear interpolation for minifying filter
                Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);

                // Texture specification
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, m_BytesPerPixel, m_TextureWidth,
                    m_TextureHeight, 0, m_Format, Gl.GL_UNSIGNED_BYTE, Il.ilGetData());

                // Release from IL memory
                Il.ilDeleteImages(1, ref ilImage);

                return m_TextureID;
            }

            int _LoadUri(Uri uri)
            {
                string localpath = uri.LocalPath;
                Console.WriteLine("Loading image " + localpath);
                return LoadTexture(localpath);
                // return TextureHelper.LoadBitmapToOpenGl( DevIL.DevIL.LoadBitmap( uri.LocalPath ) ); // need to check if DevIL handles generic URLs, but anyway we should probably cache them locally, as files
            }

            Uri uri;
            public object IdInGraphicsEngine { get { return idingraphicsengine; } } // since this doesnt have a set, this doesnt get saved to xml by serializer, which is correct behavior
            int idingraphicsengine = 0;
            bool IsLoaded
            {
                get { return true; }
            }
            public TextureProxy() // for XmlSerializer Usage
            {
            }
            public TextureProxy(Uri uri)
            {
                this.uri = uri;
                idingraphicsengine = _LoadUri(uri);
            }
        }

        public Hashtable TextureProxies;
        
        static TextureController instance = new TextureController();
        public static TextureController GetInstance(){ return instance; }
        
        public TextureController()
        {
            Il.ilInit();
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
