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
using System.Collections.Generic;

namespace OSMP
{
    // Note to self : might be cool to (a) use Reflection and/or (b) use the config file
    public class PluginsLoader
    {
        static PluginsLoader instance = new PluginsLoader();
        public static PluginsLoader GetInstance()
        {
            return instance;
        }

        List<object> plugins = new List<object>();

        public void LoadClientPlugins( Arguments arguments )
        {
            Tao.DevIl.Il.ilInit();
            Tao.DevIl.Ilu.iluInit();

            LogFile.WriteLine("unnamed args: ");
            foreach (string unnamed in arguments.Unnamed)
            {
                LogFile.WriteLine("unnamed: " + unnamed);
            }
            LoadGlobalPlugins();

            UIEntityPropertiesDialog.GetInstance();
            Editing3d.GetInstance();
            SelectionController.GetInstance();
            AssignTextureHandler.GetInstance();
            AssignColorHandler.GetInstance();
            WorldPersistToXml.GetInstance();

            ImportExportPrimBlender.GetInstance();

            EntityDelete.GetInstance();


            //SimpleCube.Register();  // SimpleCube and SimpleCone are for testing primarily
            //SimpleCone.Register();

            FractalSplineBox.Register();
            FractalSplinePrism.Register();
            FractalSplineCylinder.Register();
            FractalSplineTube.Register();
            FractalSplineRing.Register();
            FractalSplineTorus.Register();

            //WorldView.GetInstance();

            //plugins.Add(new DrawAxes());
            FrustrumCulling.GetInstance();

            ServerInfo.GetInstance();
            ConnectToServerDialog.GetInstance();

            MainTerrainWindow.GetInstance();
            BrushEffectController.GetInstance().Register( new RaiseLower() );
            BrushEffectController.GetInstance().Register( new FixedHeight() );
            BrushEffectController.GetInstance().Register( new Flatten() );
            BrushEffectController.GetInstance().Register( new PaintTexture() );
            BrushShapeController.GetInstance().Register( new RoundBrush() );
            BrushShapeController.GetInstance().Register( new SquareBrush() );
            EditController.GetInstance();
            plugins.Add( new CurrentEditSpot() );

            // add allowed serialization/deserialization types (security measure)
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( Prim ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( Vector3) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( Rot ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( Vector2 ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( TerrainModel ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( Color ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( MapTextureStageModel ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( FractalSplineBox ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( FractalSplineCylinder ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( FractalSplinePrim ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( FractalSplinePrism ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( FractalSplineRing ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( FractalSplineTorus ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( FractalSplineTube ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( Avatar ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( EntityGroup ) );
            OsmpXmlSerializer.GetInstance().RegisterType( typeof( Entity ) );

            MetaverseClient.GetInstance().worldstorage.terrainmodel.NewMap();

            DumpLogfile.GetInstance();
            HelpAbout.GetInstance();
            KeyHandlerQuit.GetInstance();
        }

        public void LoadServerPlugins()
        {
            LoadGlobalPlugins();
            ServerRegistration.GetInstance();
        }

        void LoadGlobalPlugins()
        {
            UIController.GetInstance();
        }

        //public void LoadPlugins()
        //{   
             
        //}
    }
}
