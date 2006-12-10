using System;

// this weakreference can be used in hashtables, and compared
// I'm guessing there's a reason why this is a bad idea, but didnt find it yet
class HashableWeakReference : WeakReference
{
    int hashcode;
    public HashableWeakReference( object targetobject ) : base( targetobject )
    {
        hashcode = targetobject.GetHashCode();
    }
    public override int GetHashCode()
    {
        return hashcode;
    }
    public static bool operator==( HashableWeakReference A, HashableWeakReference B )
    {
        return( A.Target == B.Target );
    }
    public static bool operator!=( HashableWeakReference A, HashableWeakReference B )
    {
        return( A.Target != B.Target );
    }
    public override bool Equals( object A )
    {
        if( !(A is HashableWeakReference ) )
        {
            return false;
        }
        return( Target == ((HashableWeakReference)A).Target );
    }
}

