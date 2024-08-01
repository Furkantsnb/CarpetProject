using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CarpetProject.Data;
using Volo.Abp.DependencyInjection;

namespace CarpetProject.EntityFrameworkCore;

public class EntityFrameworkCoreCarpetProjectDbSchemaMigrator
    : ICarpetProjectDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreCarpetProjectDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the CarpetProjectDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<CarpetProjectDbContext>()
            .Database
            .MigrateAsync();
    }
}
