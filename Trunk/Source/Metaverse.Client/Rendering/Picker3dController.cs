using System;

namespace OSMP
{
    // converts picker3dmodel results to an entity
    // dependencies:
    // - picker3dmodel (from RendererFactory.GetInstance().GetPicker3dModel();
    // - worldmodel
    public class Picker3dController
    {
        IPicker3dModel picker3dmodel;
        
        public class SinglePrimFaceDrawer : IRenderable
        {
            public Prim prim;
            public SinglePrimFaceDrawer( Prim prim )
            {
                this.prim = prim;
            }
            public void Render()
            {
                IRenderer renderer = RendererFactory.GetInstance();
                Camera.GetInstance().ApplyCamera();
                for( int i = 0; i < prim.NumFaces; i++ )
                {
                    HitTarget thishittarget = new HitTargetEntityFace( prim, i );
                    Console.WriteLine( "Renderering " + thishittarget.ToString() );
                    renderer.GetPicker3dModel().AddHitTarget( thishittarget );
                    if( prim.Parent != null )
                    {
                        ( prim.Parent as EntityGroup ).ApplyTransforms();
                    }
                    prim.RenderSingleFace( i );
                }
            }
        }
        
        public class HitTargetEntity : HitTarget
        {
            public Entity entity;
            public HitTargetEntity( Entity entity )
            {
                this.entity = entity;
            }        
            public override string ToString()
            {
                return "HitTargetEntity entity=" + entity.ToString();
            }
        }
        
        public class HitTargetEntityFace : HitTargetEntity
        {
            public int FaceNumber;
            public HitTargetEntityFace( Entity entity, int FaceNumber ) : base( entity )
            {
                this.FaceNumber = FaceNumber;
            }
            public override string ToString()
            {
                return "HitTargetEntityFace entity=" + entity.ToString() + " face=" + FaceNumber.ToString();
            }
        }
    
        static Picker3dController instance = new Picker3dController();
        public static Picker3dController GetInstance()
        {
            return instance;
        }
        
        public Picker3dController()
        {
            picker3dmodel = RendererFactory.GetInstance().GetPicker3dModel();
        }
        
        public void AddHitTarget( Entity entity )
        {
            picker3dmodel.AddHitTarget( new HitTargetEntity( entity ) );
        }
        
        public Entity GetClickedEntity( int iMouseX, int iMouseY )
        {
            HitTarget hittarget = picker3dmodel.GetClickedHitTarget( iMouseX, iMouseY );
        
            if( hittarget != null )
            {
                if( hittarget is HitTargetEntity )
                {
                    // Test.Debug(  "selected has reference " + hittarget.iForeignReference.ToString() );
                    return ((HitTargetEntity)hittarget).entity;
                }
            }
        
            return null;
        }      
        
        // we run another selection, with only a single prim, making each face a single pick target
        public int GetClickedFace( Prim prim, int iMouseX, int iMouseY )
        {
            HitTarget hittarget = picker3dmodel.GetClickedHitTarget( new SinglePrimFaceDrawer( prim as Prim ), iMouseX, iMouseY );
            if( hittarget == null || !( hittarget is HitTargetEntityFace ) )
            {
                return 0;
            }
            //Console.WriteLine( "result " + hittarget.ToString() );
            return ( hittarget as HitTargetEntityFace ).FaceNumber;
        }            
    }
}
