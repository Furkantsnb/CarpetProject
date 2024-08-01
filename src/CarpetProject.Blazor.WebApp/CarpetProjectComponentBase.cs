using CarpetProject.Localization;
using Volo.Abp.AspNetCore.Components;

namespace CarpetProject.Blazor.WebApp;

public abstract class CarpetProjectComponentBase : AbpComponentBase
{
    protected CarpetProjectComponentBase()
    {
        LocalizationResource = typeof(CarpetProjectResource);
    }
}
