using CarpetProject.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace CarpetProject.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CarpetProjectEntityFrameworkCoreModule),
    typeof(CarpetProjectApplicationContractsModule)
    )]
public class CarpetProjectDbMigratorModule : AbpModule
{
}
