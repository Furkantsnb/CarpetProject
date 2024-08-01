using Volo.Abp.Modularity;

namespace CarpetProject;

[DependsOn(
    typeof(CarpetProjectDomainModule),
    typeof(CarpetProjectTestBaseModule)
)]
public class CarpetProjectDomainTestModule : AbpModule
{

}
