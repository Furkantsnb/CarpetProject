using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace CarpetProject;

public static class CarpetProjectModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         *
         * Example: Change user and role name max lengths

           AbpUserConsts.MaxNameLength = 99;
           IdentityRoleConsts.MaxNameLength = 99;

         * Notice: It is not suggested to change property lengths
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        //OneTimeRunner.Run(() =>
        //{
        //    ObjectExtensionManager.Instance.Modules()
        //        .ConfigureCmsKit(cmsKit =>
        //        {
        //            // Tag entity'sine ProductId özelliğini ekleyin
        //            cmsKit.ConfigureTag(tag =>
        //            {
        //                tag.AddOrUpdateProperty<List<int>>(
        //                    "ProductIds",
        //                    property =>
        //                    {
        //                        property.Attributes.Add(new RequiredAttribute());
        //                    }
        //                );
        //            });

        //            // Comment entity'sine ProductId özelliğini ekleyin
        //            cmsKit.ConfigureComment(comment =>
        //            {
        //                comment.AddOrUpdateProperty<int?>(
        //                    "ProductId",
        //                    property =>
        //                    {
        //                        property.Attributes.Add(new RequiredAttribute());
        //                    }
        //                );
        //            });
        //        });
        //});

    }
}
