using Volo.Abp.Modularity;

namespace CarpetProject;

public abstract class CarpetProjectApplicationTestBase<TStartupModule> : CarpetProjectTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
