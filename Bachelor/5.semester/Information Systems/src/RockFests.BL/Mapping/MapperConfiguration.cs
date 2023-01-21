using System.Reflection;
using AutoMapper;

namespace RockFests.BL.Mapping
{
    public static class MapperConfig
    {
        public static void SetMapper()
            => Mapper.Initialize(cfg => cfg.AddProfiles(Assembly.GetAssembly(typeof(BandProfile))));
    }
}
