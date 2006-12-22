﻿// Copyright Hugh Perkins 2006
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
using System.Runtime.InteropServices;
using System.Text;

namespace OSMP.Testing
{
    // used for testing RPC

    [StructLayout(LayoutKind.Sequential)]
    public class TestClass
    {
        public int[] indexes = new int[] { };
        public string name = "";
        public bool booleanvaluetrue;
        public bool booleanvaluefalse;
        public char charvalue;
        public int intvalue;
        public double doublevalue;
        public ChildClass childclass = new ChildClass();
    }

    [StructLayout(LayoutKind.Sequential)]
    public class ChildClass
    {
        public string name = "";
    }

    [OSMP.NetworkInterfaces.AuthorizedRpcInterface]
    public interface ITestInterface
    {
        void SayHello();
        void SayMessage(string message);
        void SendObject(TestClass testobject);
    }
}
