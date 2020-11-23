
#pragma warning disable 1591

namespace IdentityServer4.AspNetIdentity.EntityFramework
{
    public class AspNetClaimTypes
    {
        public string Id { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public bool Required { get; set; }
        public bool Reserved { get; set; }
        public string Rule { get; set; }
        public string RuleValidationFailureDescription { get; set; }
        public bool UserEditable { get; set; }
        public int ValueType { get; set; }
    }
}
