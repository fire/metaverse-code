
namespace OSMP
{
    class KeyFilterConfigMappingsFactory
    {
        public static IKeyFilterConfigMappings GetInstance()
        {
            return KeyFilterFormsConfigMappings.GetInstance();
        }
    }
}
