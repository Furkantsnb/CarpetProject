using CarpetProject.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace CarpetProject.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class CarpetProjectPageModel : AbpPageModel
{
    protected CarpetProjectPageModel()
    {
        LocalizationResourceType = typeof(CarpetProjectResource);
    }
}
