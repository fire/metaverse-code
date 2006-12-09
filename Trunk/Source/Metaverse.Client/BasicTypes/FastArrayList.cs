
using System;
using System.Collections;

namespace OSMP
{
    // This is not used at this time; it is however basically a copy and paste of a lot of stuff that was in the original WorldStorage class (eg mvWorldStorage.cpp),
    // and has been replaced by a simple EntityArrayList in the new WorldModel class.
    public class FastArrayList
    {
        int iAllocated = 1000;  //<! maximum number of objects we can hold in world
        public object[] array;  //!< All the objects in the world
        public int inumberitems;  //!< number of objects in world
            
        public FastArrayList()
        {
            array = new object[ iAllocated ];
        }
        
        int GetIndex( object targetobject )
        {
            for( int index = 0; index < inumberitems; index++ )
            {
                if( array[ index ] == targetobject )
                {
                    return index;
                }
            }
            //Test.Debug(  "Couldnt find index for  " + targetobject.ToString() );
            //SignalCriticalError( "Bounds check problem in GetArrayNumForEntityReference\n" );
            return -1;
        }
        
       // does the actual add
        public int Add( object targetobject )
        {
            if( inumberitems >= iAllocated )
            {
                int iNewAllocated = iAllocated * 2;
                object[] newentities = new object[ iNewAllocated ];
                for( int i = 0; i < inumberitems; i++ )
                {
                    newentities[ i ] = array[ i ];
                }
                array = newentities;
                iAllocated = iNewAllocated;
            }
            
            array[ inumberitems ] = targetobject;
            inumberitems++;
            return( inumberitems - 1 );
        }
    
    // does the actual delete
        public void Delete( object targetobject )
        {
            int iArrayNum = GetIndex( targetobject );
            
            if( iArrayNum == -1 )
            {
               // Test.Debug(  " Delete warning: object " + targetobject.ToString() + " not found" ); // Test.Debug
                return;
            }
        
            //Test.Debug(  "DeleteEntity " + iArrayNum.ToString() ); // Test.Debug
            if(iArrayNum < ( iNumEntities - 1 ) )
            {
                array[ iArrayNum ] = array[ inumberitems - 1 ];
                array[ inumberitems - 1 ] = null;
                inumberitems--;
            }
            else
            {
                array[ inumberitems - 1 ] = null;
                inumberitems--;
            }
        }
        
        public void Clear()
        {
            iAllocated = 1000;
            inumberitems = 0;
            array = new object[ iAllocated ];
        }
    }
}    
