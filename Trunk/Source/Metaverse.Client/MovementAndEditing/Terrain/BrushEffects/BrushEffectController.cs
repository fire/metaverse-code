// Created by Hugh Perkins 2006
// hughperkins@gmail.com http://manageddreams.com
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
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
using System.Text;

namespace OSMP
{
    /// <summary>
    /// handles brush registration etc
    /// </summary>
    public class BrushEffectController
    {
        static BrushEffectController instance = new BrushEffectController();
        public static BrushEffectController GetInstance() { return instance; }

        /// <summary>
        /// Use Register to add to this dictionary
        /// </summary>
        public Dictionary<Type,IBrushEffect> brusheffects = new Dictionary<Type,IBrushEffect>();

        public BrushEffectController()
        {
        }

        public void Register( IBrushEffect brusheffect )
        {
            Console.WriteLine(this.GetType() + " registering " + brusheffect );
            this.brusheffects.Add( brusheffect.GetType(), brusheffect );
            if (CurrentEditBrush.GetInstance().BrushEffect == null)
            {
                CurrentEditBrush.GetInstance().BrushEffect = brusheffect;
            }
            MainTerrainWindow.GetInstance().AddBrushEffect( brusheffect.Name, brusheffect.Description, brusheffect );
        }
    }
}
