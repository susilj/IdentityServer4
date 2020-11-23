
namespace IdentityServer4.AspNetIdentity.EntityFramework
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    
    public class AspNetIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<AspNetClaimTypes> AspNetClaimTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Tenant>()
                .HasIndex(u => u.Code)
                .IsUnique();

            builder.Entity<Tenant>()
                .HasIndex(u => u.DomainName)
                .IsUnique();

            builder.Entity<AspNetClaimTypes>(claimType =>
            {
                claimType.ToTable("AspNetClaimTypes");
                claimType.HasKey(x => x.Id);

                claimType.Property(x => x.Name).HasMaxLength(50).IsRequired();
                claimType.Property(x => x.Description).HasMaxLength(200);
                claimType.Property(x => x.NormalizedName).HasMaxLength(50);
                claimType.Property(x => x.Required).HasColumnType("tinyint(1)").IsRequired();
                claimType.Property(x => x.Reserved).HasColumnType("tinyint(1)").IsRequired();
                claimType.Property(x => x.Rule).HasMaxLength(100);
                claimType.Property(x => x.RuleValidationFailureDescription).HasMaxLength(200);
                claimType.Property(x => x.UserEditable).HasColumnType("tinyint(1)").HasDefaultValue(0).IsRequired();
                claimType.Property(x => x.ValueType).HasMaxLength(20).IsRequired();
                claimType.Property(x => x.ConcurrencyStamp).HasMaxLength(50);

                claimType.HasIndex(x => x.Name).IsUnique();
                claimType.HasIndex(x => x.NormalizedName).IsUnique().HasFilter("[NormalizedName] IS NOT NULL");
            });

            base.OnModelCreating(builder);
        }
    }
}
