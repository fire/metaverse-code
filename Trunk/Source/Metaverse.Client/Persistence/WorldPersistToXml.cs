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
using System.IO;
using Gtk;
using System.Net;

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
            ContextMenuController.GetInstance().RegisterPersistentContextMenu( new string[] { "World", "&Load from Url..." }, new ContextMenuHandler( ContextMenuLoadFromUrl ) );
        }
        
        public void ContextMenuSave( object source, ContextMenuArgs e )
        {
            SaveWorld();
        }
        
        public void SaveWorld()
        {
            string filename = DialogHelpers.GetFilePath("Save world file", "world.OSMP");

            if( filename != "" )
            {
                Console.WriteLine ( filename );
                Store( filename );
                DialogHelpers.ShowInfoMessage(null, "World save completed");
            }
        }
        
        public void ContextMenuLoad( object source, ContextMenuArgs e )
        {
            LoadWorld();
        }

        public void ContextMenuLoadFromUrl( object source, ContextMenuArgs e )
        {
            new InputBox( "Please enter URL:", new InputBox.Callback( LoadFromUrl ) );
        }

        public void LoadFromUrl( string url )
        {
            if (url == "")
            {
                return;
            }

            Uri projecturi = new Uri( new Uri( url ), "." );

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create( url );
            HttpWebResponse httpwebresponse = (HttpWebResponse)myReq.GetResponse();
            Stream stream = httpwebresponse.GetResponseStream();
            
            //StreamReader streamreader = new StreamReader( stream );
            //string contents = streamreader.ReadToEnd();
            //streamreader.Close();
            Restore( stream, projecturi );
            stream.Close();
            httpwebresponse.Close();

            //Console.WriteLine( contents );

            //StringReader stringreader = new StringReader( contents );
            //Restore( stringreader, projecturi );
            //stringreader.Close();

            DialogHelpers.ShowInfoMessage( null, "World load completed" );
        }

        public void LoadWorld()
        {
            string filename = DialogHelpers.GetFilePath("Open world file", "world.OSMP");

            if (filename != "")
            {
                Console.WriteLine ( filename );
                Restore( filename );
                DialogHelpers.ShowInfoMessage(null, "World load completed");
            }
        }

        public void Store( string filename )
        {
            Console.WriteLine( "store " + filename );
            WorldModel worldmodel = MetaverseClient.GetInstance().worldstorage;
            
            ArrayList types = new ArrayList();
            foreach( Entity entity in worldmodel.entities )
            {
                if( !types.Contains( entity.GetType() ) )
                {
                    types.Add( entity.GetType() );
                }
            }
            
            //XmlSerializer serializer = new XmlSerializer( worldmodel.entities.GetType(), (Type[])types.ToArray( typeof( Type ) ) );
            XmlSerializer serializer = new XmlSerializer( typeof( Entity[]), (Type[])types.ToArray( typeof( Type ) ) );
            StreamWriter streamwriter = new StreamWriter( filename );
            ProjectFileController.GetInstance().SetProjectPath( new Uri( Path.GetDirectoryName( filename ) + "/" ) );
            serializer.Serialize( streamwriter, worldmodel.entities.ToArray() );
            streamwriter.Close();
        }

        // need to add a publisher/subscriber to this ;-)
        public void Restore( string filename )
        {
            Uri projecturi = new Uri( Path.GetDirectoryName( filename ) + "/" );
            FileStream filestream = new FileStream( filename, FileMode.Open );
            Restore( filestream, projecturi );
            filestream.Close();

            //StreamReader streamreader = new StreamReader( filename );
            //string contents = streamreader.ReadToEnd();
            //streamreader.Close();
            //StringReader stringreader = new StringReader( contents );
            //Restore( stringreader, projecturi );
            //streamreader.Close();
        }

        public void Restore( Stream stream, Uri projecturi )
        //public void Restore( StringReader stringreader, Uri projecturi )
        {
            WorldModel worldmodel = MetaverseClient.GetInstance().worldstorage;

            // note to self: should make these types a publisher/subscriber thing
            XmlSerializer serializer = new XmlSerializer( typeof( Entity[] ), new Type[]{
                typeof( Avatar ),
                typeof( FractalSplineCylinder ), 
                typeof( FractalSplineRing ), 
                typeof( FractalSplineBox ),
                typeof( FractalSplineTorus ),
                typeof( FractalSplinePrism ),
                typeof( FractalSplineTube )
                } );
            //DialogHelpers.ShowInfoMessage( null, serializer.Deserialize(filestream).GetType().ToString());
            ProjectFileController.GetInstance().SetProjectPath( projecturi );
            Entity[] entities = (Entity[])serializer.Deserialize( stream );
            worldmodel.Clear();
            foreach (Entity entity in entities)
            {
                Console.WriteLine( entity );
                worldmodel.AddEntity( entity );
            }
        }
    }
}
