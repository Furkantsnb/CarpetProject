using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace CarpetProject.Web;

[Dependency(ReplaceServices = true)]
public class CarpetProjectBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "CarpetProject";
}
