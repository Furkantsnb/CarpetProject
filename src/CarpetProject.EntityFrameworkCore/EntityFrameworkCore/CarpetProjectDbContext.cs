using CarpetProject.Categories;
using CarpetProject.Products;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.CmsKit.Comments;
using Volo.CmsKit.EntityFrameworkCore;

namespace CarpetProject.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class CarpetProjectDbContext :
    AbpDbContext<CarpetProjectDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<CategoryProduct> CategoryProducts { get; set; }


    #endregion

    public CarpetProjectDbContext(DbContextOptions<CarpetProjectDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();



        // CategoryProduct ve Product ili�kileri i�in yap�land�rma
        builder.Entity<CategoryProduct>()
            .HasKey(cp => new { cp.CategoryId, cp.ProductId });

        builder.Entity<CategoryProduct>()
            .HasOne(cp => cp.Category)
            .WithMany(c => c.CategoryProducts)
            .HasForeignKey(cp => cp.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // �li�kiyi opsiyonel yapmak yerine silme davran���n� s�n�rland�r

        builder.Entity<CategoryProduct>()
            .HasOne(cp => cp.Product)
            .WithMany(p => p.CategoryProducts)
            .HasForeignKey(cp => cp.ProductId);

        // Product ve Image aras�nda bire �ok ili�ki
        builder.Entity<Product>()
            .HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.SetNull); // Product silindi�inde ili�kili Imagenin ProductId'si null olur

        // Kategori ve Image aras�nda bire bir ili�ki
        builder.Entity<Category>()
            .HasOne(c => c.Image) // Category'nin bir Image'� olur
            .WithOne(i => i.Category) // Image'nin bir Category'si olur
            .HasForeignKey<Image>(i => i.CategoryId) // Image'da CategoryId foreign key olarak kullan�l�r
            .OnDelete(DeleteBehavior.SetNull); // Kategori silindi�inde ili�kili Image'�n CategoryId null olur

        // Kategori i�in ParentCategory ili�kisi
        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany() // ParentCategory'nin alt kategorileri bu koleksiyonla y�netilmeyecek
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Silme davran���n� s�n�rla

        // E�er Category �zerinde bir global query filter kullan�yorsan�z, ayn� query filter'� CategoryProduct i�in de ekleyin:
        builder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted); // �rne�in, soft delete kullan�yorsan�z

        builder.Entity<CategoryProduct>()
            .HasQueryFilter(cp => !cp.Category.IsDeleted); // Ayn� filter CategoryProduct i�in de uygulanmal�



        builder.ConfigureCmsKit();
        }
}
