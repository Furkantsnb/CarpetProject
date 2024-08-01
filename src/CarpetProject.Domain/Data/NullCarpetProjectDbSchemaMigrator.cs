using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CarpetProject.Data;

/* This is used if database provider does't define
 * ICarpetProjectDbSchemaMigrator implementation.
 */
public class NullCarpetProjectDbSchemaMigrator : ICarpetProjectDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
