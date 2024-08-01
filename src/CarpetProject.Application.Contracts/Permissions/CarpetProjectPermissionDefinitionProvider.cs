using CarpetProject.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace CarpetProject.Permissions;

public class CarpetProjectPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CarpetProjectPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(CarpetProjectPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CarpetProjectResource>(name);
    }
}
