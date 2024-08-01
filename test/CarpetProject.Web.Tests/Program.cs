using Microsoft.AspNetCore.Builder;
using CarpetProject;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<CarpetProjectWebTestModule>();

public partial class Program
{
}
