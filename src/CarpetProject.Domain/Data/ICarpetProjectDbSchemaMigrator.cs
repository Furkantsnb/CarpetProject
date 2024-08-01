using System.Threading.Tasks;

namespace CarpetProject.Data;

public interface ICarpetProjectDbSchemaMigrator
{
    Task MigrateAsync();
}
