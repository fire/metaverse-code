using System;
using System.Collections.Generic;
using System.Text;

namespace OSMP
{
    // attributes that affect which fields/properties are replicated

    public interface IReplicationAttribute
    {
    }

    public class Replicate : Attribute, IReplicationAttribute
    {
    }

    public class Movement : Attribute, IReplicationAttribute
    {
    }
    // Add more attributes here...


    public class ReplicateAttributeHelper
    {
        public ReplicateAttributeHelper()
        {
            RegisterAttributes();
        }
        
        // careful of changing this; OSMP clients with different lists here will be incompatible
        void RegisterAttributes()
        {
            Add( typeof( Replicate ) );
            Add( typeof( Movement ) );
        }

        Dictionary<Type, int> BitnumByTypeAttribute = new Dictionary<Type, int>();
        Dictionary<int, Type> TypeAttributeByBitnum = new Dictionary<int, Type>();
        
        int nextbitnum = 0;
        void Add(Type AttributeType)
        {
            BitnumByTypeAttribute.Add( AttributeType, nextbitnum );
            TypeAttributeByBitnum.Add( nextbitnum,AttributeType );
            nextbitnum++;
        }

        public List<Type> BitmapToAttributeTypeArray(int bitmap)
        {
            List<Type> typeattributes = new List<Type>();
            foreach( int bitnumber in TypeAttributeByBitnum.Keys )
            {
                if( ( ( 1 << bitnumber ) & bitmap ) > 0 )
                {
                    typeattributes.Add( TypeAttributeByBitnum[ bitnumber ] );
                }
            }
            return typeattributes;
        }

        public int TypeArrayToBitmap(Type[] typearray)
        {
            int bitmap = 0;
            foreach (Type attributetype in typearray)
            {
                bitmap |= ( 1 << BitnumByTypeAttribute[ attributetype ] );
            }
            return bitmap;
        }
    }
    
    public class TestReplicationAttributes
    {
        public void Go()
        {
            int bitmap = new ReplicateAttributeHelper().TypeArrayToBitmap(new Type[] { typeof(Replicate) });
            foreach (Type attributetype in new ReplicateAttributeHelper().BitmapToAttributeTypeArray(bitmap))
            {
                Console.WriteLine( attributetype );
            }
            Console.WriteLine();

            bitmap = new ReplicateAttributeHelper().TypeArrayToBitmap(new Type[] { typeof(Movement) });
            foreach (Type attributetype in new ReplicateAttributeHelper().BitmapToAttributeTypeArray(bitmap))
            {
                Console.WriteLine( attributetype );
            }
            Console.WriteLine();

            bitmap = new ReplicateAttributeHelper().TypeArrayToBitmap(new Type[] { typeof(Replicate), typeof(Movement) });
            foreach (Type attributetype in new ReplicateAttributeHelper().BitmapToAttributeTypeArray(bitmap))
            {
                Console.WriteLine( attributetype );
            }
            Console.WriteLine();
        }
    }
}
