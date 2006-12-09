using System.Windows.Forms;

namespace OSMP
{
    public interface IMouseFilterMouseCache
    {
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseUp;        
          
        int MouseX
        {
            get;
        }
        int MouseY
        {
            get;
        }
        bool LeftMouseDown
        {
            get;
        }
        bool RightMouseDown
        {
            get;
        }
    }
}
