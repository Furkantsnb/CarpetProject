﻿using CarpetProject.Localization;
using Volo.Abp.AspNetCore.Components;

namespace CarpetProject.Blazor.WebApp.Tiered.Client;

public abstract class CarpetProjectComponentBase : AbpComponentBase
{
    protected CarpetProjectComponentBase()
    {
        LocalizationResource = typeof(CarpetProjectResource);
    }
}
