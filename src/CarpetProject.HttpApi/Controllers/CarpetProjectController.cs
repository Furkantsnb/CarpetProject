using CarpetProject.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CarpetProject.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class CarpetProjectController : AbpControllerBase
{
    protected CarpetProjectController()
    {
        LocalizationResource = typeof(CarpetProjectResource);
    }
}
