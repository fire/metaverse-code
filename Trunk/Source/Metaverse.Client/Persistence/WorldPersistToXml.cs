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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization; 
using System.Windows.Forms;
using System.IO;

namespace OSMP
{
    public class WorldPersistToXml : IWorldPersist
    {
        static WorldPersistToXml instance = new WorldPersistToXml();
        public static WorldPersistToXml GetInstance()
        {
            return instance;
        }
        
        public WorldPersistToXml()
        {
            MenuController.GetInstance().RegisterMainMenu(new string[]{ "&File","&Save World..." }, new MainMenuCallback( SaveWorld ) );
            MenuController.GetInstance().RegisterMainMenu(new string[]{ "&File","&Load World..." }, new MainMenuCallback( LoadWorld ) );
            
            ContextMenuController.GetInstance().RegisterPersistentContextMenu(new string[]{ "World","&Save to File..." }, new ContextMenuHandler( ContextMenuSave ) );
            ContextMenuController.GetInstance().RegisterPersistentContextMenu(new string[]{ "World","&Load from File..." }, new ContextMenuHandler( ContextMenuLoad ) );
        }
        
        public void ContextMenuSave( object source, ContextMenuArgs e )
        {
            SaveWorld();
        }
        
        public void SaveWorld()
        {
            SaveFileDialog savefiledialog = new SaveFileDialog();
        
            savefiledialog.InitialDirectory = "DefaultWorld" ;
            savefiledialog.Filter = "World Files(*.OSMP)|*.OSMP|All files (*.*)|*.*" ;
            savefiledialog.FilterIndex = 1;
            savefiledialog.RestoreDirectory = true ;
        
            if(savefiledialog.ShowDialog() == DialogResult.OK)
            {
                string filename = savefiledialog.FileName;
                Console.WriteLine ( filename );
                Store( filename );
            }
            DialogHelper.GetInstance().ShowInfoMessage( "World save completed", "World save completed" );
        }
        
        public void ContextMenuLoad( object source, ContextMenuArgs e )
        {
            LoadWorld();
        }
        
        public void LoadWorld()
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();
        
            openfiledialog.InitialDirectory = "DefaultWorld" ;
            openfiledialog.Filter = "World Files(*.OSMP)|*.OSMP|All files (*.*)|*.*" ;
            openfiledialog.FilterIndex = 1;
            openfiledialog.RestoreDirectory = true ;
        
            if(openfiledialog.ShowDialog() == DialogResult.OK)
            {
                string filename = openfiledialog.FileName;
                Console.WriteLine ( filename );
                Restore( filename );
            }
            DialogHelper.GetInstance().ShowInfoMessage( "World load completed", "World load completed" );
        }
        
        public void Store( string filename )
        {
            WorldModel worldmodel = MetaverseClient.GetInstance().worldstorage;
            
            ArrayList types = new ArrayList();
            foreach( Entity entity in worldmodel.entities )
            {
                if( !types.Contains( entity.GetType() ) )
                {
                    types.Add( entity.GetType() );
                }
            }
            
            XmlSerializer serializer = new XmlSerializer( worldmodel.entities.GetType(), (Type[])types.ToArray( typeof( Type ) ) );
            StreamWriter streamwriter = new StreamWriter( filename );
            serializer.Serialize( streamwriter, worldmodel.entities );
        }
        
        // need to add a publisher/subscriber to this ;-)
        public void Restore( string filename )
        {
            WorldModel worldmodel = MetaverseClient.GetInstance().worldstorage;
            
            // note to self: should make these types a publisher/subscriber thing
            XmlSerializer serializer = new XmlSerializer( worldmodel.entities.GetType(), new Type[]{
                typeof( Avatar ),
                typeof( FractalSplineCylinder ), 
                typeof( FractalSplineRing ), 
                typeof( FractalSplineBox ),
                typeof( FractalSplineTorus ),
                typeof( FractalSplinePrism ),
                typeof( FractalSplineTube )
                } );            
            FileStream filestream = new FileStream( filename, FileMode.Open );
            MessageBox.Show(serializer.Deserialize(filestream).GetType().ToString());
            List<Entity> entities = (List<Entity>)serializer.Deserialize( filestream );
            foreach (Entity entity in entities)
            {
                worldmodel.AddEntity(entity);
            }
        }
    }
}
