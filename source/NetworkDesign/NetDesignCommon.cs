using System;
using System.Collections;

class ClientToServerRpc : MarshalByRefObject
{
}

class ServerToClientRpc : MarshalByRefObject
{
    public UpdateObject( object newobject );
}

interface IReplicable
{
    event ChangeEventArgs Changed;
    
    bool HasChildren
    {
        get;
    }
}

class World : IReplicable
{
    static World world = new World();
    public static World GetInstance(){ return instance; }
    
    public ArrayList entities; // ArrayLsit of Entity objects
}

class Entity
{
    public string name;
        
    Vector3 pos;
    Vector3 scale;
    Rot rot;
        
    Entity( string name )
    {
        this.name = name;
    }
    
    public override string ToString()
    {
        return "Entity " + name;
    }
}

class Box : Entity
{
    
    
    public Box( string name ) : base( name )
    {
    }
}

class Sphere : Entity
{
    float roundness; // arbitrary parameter, just for testing
    
    public Sphere( string name ) : base( name )
    {
    }
}

class LockController
{
    // throws exception if fails
    public void Lock( Entity entity )
    {
    }
    
    public void Release( Entity entity )
    {
    }
    
    public void RenewLock( Entity entity )
    {
    }
}
