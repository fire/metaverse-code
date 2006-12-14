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
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace OSMP
{
            // failed attempt to get asynchronous color change
    /*
    class AsyncColorDialog
    {
        ColorDialog colordialog;
        
        public AsyncColorDialog()
        {
        }
        
        public Color Color{
            get{ return new Color( (double)colordialog.Color.R/255, (double)colordialog.Color.G/255, (double)colordialog.Color.B/255 );
            }
        }
        
        public bool IsFinished;
        
        public void Run()
        {
            colordialog = new ColorDialog();
            colordialog.AllowFullOpen = true;
            colordialog.FullOpen = true;
            colordialog.AnyColor = true;
            colordialog.ShowDialog();
            IsFinished = true;
        }
    }
    */
    
    // This class hooks into the gui, providing an Assign Color... option to the context menu.  Yay!
    public class AssignColorHandler
    {
        static AssignColorHandler instance = new AssignColorHandler(); // instantiate handler
        public static AssignColorHandler GetInstance()
        {
            return instance;
        }
        
        public AssignColorHandler()
        {
            Console.WriteLine("instantiating AssignColorHandler" );
            ContextMenuController.GetInstance().ContextMenuPopup += new ContextMenuHandler( ContextMenuPopup );
        }
        
        Entity entity;
        int iMouseX;
        int iMouseY;
        
        public void ContextMenuPopup( object source, ContextMenuArgs e )
        {
            iMouseX = e.MouseX;
            iMouseY = e.MouseY;
            entity = e.Entity;
            if( entity != null )
            {
                Console.WriteLine("AssignColorHandler registering in contextmenu");
                ContextMenuController.GetInstance().RegisterContextMenu(new string[]{ "Assign &Color..." }, new ContextMenuHandler( AssignColorClick ) );
            }
        }
        
        public void AssignColor( int FaceNumber, Color color )
        {
            if( entity is FractalSplinePrim )
            {
                ((FractalSplinePrim)entity).SetColor( FaceNumber, color );
                MetaverseClient.GetInstance().worldstorage.OnModifyEntity(entity);
            }
        }
        
        public void AssignColorClick( object source, ContextMenuArgs e )
        {
            if( ! ( entity is Prim ) )
            {
                return;
            }
            
            int FaceNumber = Picker3dController.GetInstance().GetClickedFace( entity as Prim, iMouseX, iMouseY );
            
            /*
            // failed attempt to get asynchronous color change
            AsyncColorDialog asynccolordialog = new AsyncColorDialog();
            Thread colordialogthread = new Thread( new ThreadStart( asynccolordialog.Run ) );
            colordialogthread.Start();
            
            while( true )
            {
                Test.WriteOut( asynccolordialog.Color );
                Thread.Sleep( 100 );
            }
            */
            
            ColorDialog colordialog = new ColorDialog();
            colordialog.AllowFullOpen = true;
            colordialog.FullOpen = true;
            colordialog.AnyColor = true;

            if (colordialog.ShowDialog() == DialogResult.OK)
            {
                AssignColor( FaceNumber, new Color( (double)colordialog.Color.R/255, (double)colordialog.Color.G/255, (double)colordialog.Color.B/255 ) );
            }
        }
    }
}
