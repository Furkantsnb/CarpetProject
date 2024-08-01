﻿using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace CarpetProject.Blazor.WebApp.Tiered;

[Dependency(ReplaceServices = true)]
public class CarpetProjectBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "CarpetProject";
}
