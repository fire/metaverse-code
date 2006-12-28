using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP
{
    public class ArrayHelper
    {
        public static bool IsInArray(object[] targetarray, object value)
        {
            foreach (object item in targetarray)
            {
                if (item.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
