
namespace IdentityServer4.AspNetIdentity.EntityFramework
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}
