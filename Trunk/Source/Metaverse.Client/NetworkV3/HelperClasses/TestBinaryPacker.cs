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
using System.Text;
using System.Runtime.InteropServices;

namespace OSMP
{
    // call this to test binary packer
    class TestBinaryPacker
    {
        [StructLayout( LayoutKind.Sequential )]
        class TestClass
        {
            public int[] indexes;
            public string name;
            public bool booleanvaluetrue;
            public bool booleanvaluefalse;
            public char charvalue;
            public int intvalue;
            public double doublevalue;
            public ChildClass childclass;
        }

        [StructLayout(LayoutKind.Sequential)]
        class ChildClass
        {
            public string name;
        }

        public void Go()
        {
            byte[] bytearray = new byte[256];
            int position = 0;
            TestClass testclass = new TestClass();
            testclass.booleanvaluetrue = true;
            testclass.charvalue = 'C';
            testclass.doublevalue = 123.4567890;
            testclass.intvalue = 1234567890;
            testclass.name = "Test class name";
            testclass.indexes = new int[] { 5, 1, 4, 2, 3 };
            testclass.childclass = new ChildClass();
            testclass.childclass.name = "name inside child class";
            BinaryPacker.WriteValueToBuffer(bytearray, ref position, testclass);

            Test.WriteOut(bytearray );

            position = 0;
            TestClass outputobject = (TestClass)BinaryPacker.ReadValueFromBuffer(bytearray, ref position, typeof(TestClass));
            Console.WriteLine(outputobject.intvalue);
            Console.WriteLine(outputobject.booleanvaluetrue);
            Console.WriteLine(outputobject.booleanvaluefalse);
            Console.WriteLine(outputobject.charvalue);
            Console.WriteLine(outputobject.doublevalue);
            Console.WriteLine(outputobject.name);
            foreach (int value in outputobject.indexes)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine(outputobject.childclass.name);
        }
    }
}
