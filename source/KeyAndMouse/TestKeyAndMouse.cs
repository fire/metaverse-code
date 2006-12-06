
using System;
using System.Collections;
using System.Windows.Forms;

namespace OSMP
{
    public class RendererFactory
    {
        static IRenderer renderer = new FakeRenderer();
        public static IRenderer GetInstance()
        {
            return renderer;
        }
    }
    
    public interface IRenderer
    {
        event KeyEventHandler KeyUp;
        event KeyEventHandler KeyDown;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        void FakeKeyDown( Keys keycode );
        void FakeKeyUp( Keys keycode );
    }
    
    public class FakeRenderer : IRenderer
    {
        public event KeyEventHandler KeyUp;
        public event KeyEventHandler KeyDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        
        public void FakeKeyDown( Keys keycode )
        {
            if( KeyDown != null )
            {
                Console.WriteLine("FakeRenderer, sending keycode : " + keycode.ToString() );
                KeyDown( this, new KeyEventArgs( keycode ) );
            }
        }
        public void FakeKeyUp( Keys keycode )
        {
            if( KeyUp != null )
            {
                KeyUp( this, new KeyEventArgs( keycode ) );
            }
        }
    }
    
    public class TestKeyAndMouse
    {
        public void Go()
        {
            Console.WriteLine( Config.GetInstance().iDebugLevel.ToString() );
            
            KeyFilterComboKeys.GetInstance().RegisterCombo( new string[]{"shift"}, null, new KeyComboHandler( keyhandlershift ) );
            KeyFilterComboKeys.GetInstance().RegisterCombo( new string[]{"movedown"}, null, new KeyComboHandler( keyhandlershift ) );
            KeyFilterComboKeys.GetInstance().RegisterCombo( new string[]{"moveforwards"}, null, new KeyComboHandler( keyhandlershift ) );
            KeyFilterComboKeys.GetInstance().RegisterCombo( new string[]{"editmode"}, null, new KeyComboHandler( keyhandlerposedit ) );
            KeyFilterComboKeys.GetInstance().RegisterCombo( new string[]{"editmode","editscale"}, null, new KeyComboHandler( keyhandlerscaleedit ) );
            /*
            RendererFactory.GetInstance().FakeKeyDown( Keys.Shift );
            RendererFactory.GetInstance().FakeKeyDown( Keys.C );
            RendererFactory.GetInstance().FakeKeyUp( Keys.C );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyUp( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyUp( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.W );
            RendererFactory.GetInstance().FakeKeyUp( Keys.W );
            RendererFactory.GetInstance().FakeKeyDown( Keys.Menu );
            RendererFactory.GetInstance().FakeKeyUp( Keys.Menu );
            */
            RendererFactory.GetInstance().FakeKeyDown( Keys.Z );
            RendererFactory.GetInstance().FakeKeyDown( Keys.X );
        }
        public void keyhandlershift( object source, ComboKeyEventArgs e )
        {
            Console.WriteLine( "got event " + e.IsComboDown.ToString() );
        }
        public void keyhandlerposedit( object source, ComboKeyEventArgs e )
        {
            Console.WriteLine( "PosEdit " + e.IsComboDown.ToString() );
        }
        public void keyhandlerscaleedit( object source, ComboKeyEventArgs e )
        {
            Console.WriteLine( "SCALEDIT " + e.IsComboDown.ToString() );
        }
    }
    
    public class EntryPoint
    {
        public static void Main()
        {
            try{
                new TestKeyAndMouse().Go();
            }catch( Exception e )
            {
                Console.WriteLine(e );
            }
        }
    }
}
