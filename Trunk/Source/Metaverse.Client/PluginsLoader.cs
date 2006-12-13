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
        public void LoadPlugins()
        {
            KeyHandlerQuit.GetInstance();
            
            EntityPropertiesDialog.GetInstance();
            Editing3d.GetInstance();
            SelectionController.GetInstance();
            AssignTextureHandler.GetInstance();
            AssignColorHandler.GetInstance();
            WorldPersistToXml.GetInstance();
            
            ImportExportPrimBlender.GetInstance();
            
            ChatController.GetInstance();
            
            HelpAbout.GetInstance();
            
            //SimpleCube.Register();  // SimpleCube and SimpleCone are for testing primarily
            //SimpleCone.Register();
            
            FractalSplineBox.Register();
            FractalSplinePrism.Register();
            FractalSplineCylinder.Register();
            FractalSplineTube.Register();
            FractalSplineRing.Register();
            FractalSplineTorus.Register();
             
        }
    }
}
