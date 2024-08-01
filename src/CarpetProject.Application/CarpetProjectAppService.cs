using System;
using System.Collections.Generic;
using System.Text;
using CarpetProject.Localization;
using Volo.Abp.Application.Services;

namespace CarpetProject;

/* Inherit your application services from this class.
 */
public abstract class CarpetProjectAppService : ApplicationService
{
    protected CarpetProjectAppService()
    {
        LocalizationResource = typeof(CarpetProjectResource);
    }
}
