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

using System;
using System.Collections;

namespace OSMP
{
    public class EntityArrayList : ArrayList{
        public new virtual Entity this[ int index ]{get{return (Entity)base[ index ];}set{base[ index ] = value;}}
        public class EntityEnumerator{
            IEnumerator enumerator;
            public EntityEnumerator( IEnumerator enumerator ){this.enumerator = enumerator;}
            public Entity Current{get{return (Entity)enumerator.Current;}}
            public void MoveNext(){enumerator.MoveNext();}
            public void Reset(){enumerator.Reset();}
        }
        public new EntityEnumerator GetEnumerator()
        {
            return new EntityEnumerator( base.GetEnumerator() );
        }
    }
}