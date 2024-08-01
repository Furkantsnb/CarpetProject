using Volo.Abp.Settings;

namespace CarpetProject.Settings;

public class CarpetProjectSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(CarpetProjectSettings.MySetting1));
    }
}
