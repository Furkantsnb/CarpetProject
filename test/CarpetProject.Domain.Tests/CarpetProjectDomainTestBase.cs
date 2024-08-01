using Volo.Abp.Modularity;

namespace CarpetProject;

/* Inherit from this class for your domain layer tests. */
public abstract class CarpetProjectDomainTestBase<TStartupModule> : CarpetProjectTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
