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
        [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
        class AttributePack : Attribute
        {
        }

        [StructLayout( LayoutKind.Sequential )]
        class TestClass
        {
            public int[] indexes;
            public string name;
            string country;
            [AttributePack]
            public string Country { get { return country; } set { country = value; } }
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
            testclass.Country = "Spain";
            testclass.name = "Test class name";
            testclass.indexes = new int[] { 5, 1, 4, 2, 3 };
            testclass.childclass = new ChildClass();
            testclass.childclass.name = "name inside child class";
            new BinaryPacker().WriteValueToBuffer(bytearray, ref position, testclass);
            new BinaryPacker().WriteValueToBuffer(bytearray, ref position, "The quick brown fox.");
            new BinaryPacker().WriteValueToBuffer(bytearray, ref position, "Rain in Spain.");

            //BinaryPacker binarypacker = new BinaryPacker(new Type[] { typeof( AttributePack ) });
            //Test.WriteOut(bytearray);
            new BinaryPacker().PackObjectUsingSpecifiedAttributes(bytearray, ref position, testclass,
                new Type[] { typeof(AttributePack) });

            //binarypacker.WriteValueToBuffer(bytearray, ref position, testclass);

            byte[] bytearraytowrite = Encoding.UTF8.GetBytes("Hello world");
            new BinaryPacker().WriteValueToBuffer(bytearray, ref position, bytearraytowrite);

            FractalSplineBox box = new FractalSplineBox();
            box.name = "Test box";
            box.pos = new Vector3(1, 123.123, 150.150);
            //binarypacker.allowedattributes = new Type[] { typeof(Replicate) };
            //binarypacker.WriteValueToBuffer(bytearray, ref position, box);
            new BinaryPacker().PackObjectUsingSpecifiedAttributes(bytearray, ref position,
                box, new Type[] { typeof(Replicate) });

            Test.WriteOut(bytearray );

            position = 0;
            TestClass outputobject = (TestClass)new BinaryPacker().ReadValueFromBuffer(bytearray, ref position, typeof(TestClass));
            LogFile.WriteLine(outputobject.intvalue);
            LogFile.WriteLine(outputobject.booleanvaluetrue);
            LogFile.WriteLine(outputobject.booleanvaluefalse);
            LogFile.WriteLine(outputobject.charvalue);
            LogFile.WriteLine(outputobject.doublevalue);
            LogFile.WriteLine(outputobject.Country);
            LogFile.WriteLine(outputobject.name);
            foreach (int value in outputobject.indexes)
            {
                LogFile.WriteLine(value);
            }
            LogFile.WriteLine(outputobject.childclass.name);

            LogFile.WriteLine((string)new BinaryPacker().ReadValueFromBuffer(bytearray, ref position, typeof(string)));
            LogFile.WriteLine((string)new BinaryPacker().ReadValueFromBuffer(bytearray, ref position, typeof(string)));

            //binarypacker.allowedattributes = new Type[] { typeof(AttributePack) };
            //outputobject = (TestClass)binarypacker.ReadValueFromBuffer(bytearray, ref position, typeof(TestClass));
            outputobject = new TestClass();
            new BinaryPacker().UnpackIntoObjectUsingSpecifiedAttributes(bytearray, ref position,
                outputobject, new Type[] { typeof(AttributePack) });
            LogFile.WriteLine(outputobject.name); // should be blank, because no AttributePack
            LogFile.WriteLine(outputobject.Country);

            bytearraytowrite = (byte[])new BinaryPacker().ReadValueFromBuffer(bytearray, ref position, typeof(byte[]));
            LogFile.WriteLine( "[" + Encoding.UTF8.GetString(bytearraytowrite) + "]" );

            //binarypacker.allowedattributes = new Type[] { typeof(Replicate) };
            //object boxobject = binarypacker.ReadValueFromBuffer(bytearray, ref position, typeof(FractalSplineBox));
            object boxobject = new FractalSplineBox();
            new BinaryPacker().UnpackIntoObjectUsingSpecifiedAttributes(bytearray, ref position,
                boxobject, new Type[] { typeof(Replicate) });
            LogFile.WriteLine(boxobject);
        }
    }
}
