using IdentityServer4.Models;
using System.Collections.Generic;

namespace Host
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                // new IdentityResource
                // {
                //     Name = "office",
                //     UserClaims = { "office_number" }
                // }
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                // new Client
                // {
                //     ClientId = "client",
                //     ClientName = "Client Credentials Client",

                //     AllowedGrantTypes = GrantTypes.ClientCredentials,
                //     ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                //     AllowedScopes = { "api1" }
                // },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    //AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    //AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    // ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    //RedirectUris = { "http://localhost:5001/signin-oidc", "https://localhost:5002/signin-oidc" },
                     RedirectUris = { "http://localhost:8001/signin-oidc", "https://localhost:8002/signin-oidc" },
                    // FrontChannelLogoutUri = "http://localhost:5001/signout-oidc",
                    //PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc", "https://localhost:5002/signout-callback-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:8001/signout-callback-oidc", "https://localhost:8002/signout-callback-oidc" },
 
                    // AllowOfflineAccess = true,
                    // AllowedScopes = { "openid", "profile", "api1" }
                    AllowedScopes = { "openid", "profile", "email" }
                }
            };
        }
    }
}
