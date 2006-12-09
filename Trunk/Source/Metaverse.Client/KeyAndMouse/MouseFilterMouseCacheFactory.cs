
namespace OSMP
{
    public class MouseFilterMouseCacheFactory
    {
        public static IMouseFilterMouseCache GetInstance()
        {
            return MouseFilterFormsMouseCache.GetInstance();
        }
    }
}
