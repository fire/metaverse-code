// Copyright Hugh Perkins 2004,2005,2006
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
// You can contact me at hughperkins@gmail.com for more information.

using System;
using System.Collections;
using Metaverse.Utility;

namespace OSMP
{
    public class SimpleCube : Prim
    {
        const int iMaxFaces = 12;
        Color[] facecolors = new Color[ iMaxFaces ];
        
        // register callbacks
        public static void Register()
        {
            ContextMenuController.GetInstance().RegisterPersistentContextMenu(new string[]{ "&Create...","Simple &Cube" }, new ContextMenuHandler( CreateCube ) );
        }
        
        public static void CreateCube( object soure, ContextMenuArgs e )
        {
            EntityCreationProperties buildproperties = new EntityCreationProperties( e.MouseX, e.MouseY );
            
            SimpleCube newcube = new SimpleCube();
            newcube.pos = buildproperties.pos;
            newcube.rot = buildproperties.rot;
            newcube.scale = new Vector3( 0.2, 0.2, 0.2 );
            newcube.name = "new cube";

            MetaverseClient.GetInstance().worldstorage.AddEntity(newcube);
        }
        
        public SimpleCube()
        {
            Test.Debug( "Cube::Cube" );
            for( int i = 0; i < iMaxFaces; i++ )
            {
                facecolors[i] = new Color( 0.8,0.8,1.0 );
            }
        }
        public override void Draw()
        {
           // Test.Debug("cube draw");
            IGraphicsHelper graphics = GraphicsHelperFactory.GetInstance();
            
            graphics.PushMatrix();            
            
           // Test.Debug( this.ToString() );
            graphics.SetMaterialColor( facecolors[0] );
            graphics.Translate( pos );            
            graphics.Rotate( rot );            
            graphics.Scale( scale );
            
            graphics.DrawCube();
            
            graphics.PopMatrix();
       }
       public override void DrawSelected()
       {
            IGraphicsHelper graphics = GraphicsHelperFactory.GetInstance();
           
            graphics.SetMaterialColor( new Color( 0.2, 0.7, 1.0 ) );
            graphics.PushMatrix();            
            graphics.Translate( pos );
            graphics.Rotate( rot );
            graphics.Scale( scale );
            
            graphics.DrawWireframeBox( 10 );
            
            graphics.PopMatrix();
       }
       public override void SetColor( int face, Color newcolor )
       {
           facecolors[ face ] = new Color( newcolor );
       }
       public override Color GetColor( int face )
       {
           return facecolors[ face ];
       }
       public override string ToString()
       {
           return "Cube, " + pos.ToString() + " " + rot.ToString() + " " + scale.ToString();
       }
    }
}
