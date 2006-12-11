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

    public interface ITestInterface
    {
        void SayHello();
        void SayMessage(string message);
        void SendObject(TestClass testobject);
    }
}
