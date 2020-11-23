
namespace Host
{
    using IdentityServer4.AspNetIdentity.EntityFramework;
    using IdentityServer4.EntityFramework.DbContexts;
    using IdentityServer4.EntityFramework.Mappers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                .AddEnvironmentVariables()
                                                .AddCommandLine(args)
                                                .Build();

            string connectionString = "Server=localhost;Port=3306;Database=IdentityServer;Uid=root;Pwd=root@123"; // configuration.GetConnectionString("DefaultConnection");

            //Console.WriteLine(connectionString ?? "N/A");
            //Console.ReadLine();
            IServiceCollection services = new ServiceCollection()
                                                .AddDbContext<AspNetIdentityDbContext>(options => options.UseMySql(connectionString));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

            string migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddConfigurationStore(configDb =>
                {
                    configDb.ConfigureDbContext = db => db.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(operationalDb =>
                {
                    operationalDb.ConfigureDbContext = db => db.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                });
            //.AddAspNetIdentity<ApplicationUser>();

            var serviceProvider = services.BuildServiceProvider(true);

            InitializeDatabase(serviceProvider);

            IApplicationBuilder app = new ApplicationBuilder(serviceProvider);
            app.UseIdentityServer();

            Console.WriteLine("Seeded data");
            Console.ReadLine();
        }

        private static void InitializeDatabase(ServiceProvider serviceProvider)
        {
            using (IServiceScope serviceScope = serviceProvider.CreateScope())
            {
                PersistedGrantDbContext persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                persistedGrantDbContext.Database.Migrate();

                ConfigurationDbContext configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationDbContext.Database.Migrate();

                if (!configurationDbContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }

                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        configurationDbContext.IdentityResources.Add(resource.ToEntity());
                    }

                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.GetApis())
                    {
                        configurationDbContext.ApiResources.Add(api.ToEntity());
                    }

                    configurationDbContext.SaveChanges();
                }
            }
        }
    }
}
