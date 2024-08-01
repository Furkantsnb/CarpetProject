using Volo.Abp.Modularity;

namespace CarpetProject;

[DependsOn(
    typeof(CarpetProjectApplicationModule),
    typeof(CarpetProjectDomainTestModule)
)]
public class CarpetProjectApplicationTestModule : AbpModule
{

}
