using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity.EntityFramework;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostWeb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            /// Store assembly for migrations

            string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            /// Replace DbContext database from SqLite in template to Postgres
            services.AddDbContext<AspNetIdentityDbContext>(options =>
                options.UseMySql(connectionString, sql => sql.MigrationsAssembly("HostWeb")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();
            //.AddRoleManager<CustomRoleManager>();

            services.AddMvc(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
            {
                // options.Events.RaiseErrorEvents = true;
                // options.Events.RaiseInformationEvents = true;
                // options.Events.RaiseFailureEvents = true;
                // options.Events.RaiseSuccessEvents = true;
            })
                /// Use Postgres database for storing configuration data
                .AddConfigurationStore(configDb =>
                {
                    configDb.ConfigureDbContext = db => db.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                /// Use Postgres database for storing operational data
                .AddOperationalStore(operationalDb => {
                    operationalDb.ConfigureDbContext = db => db.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<ApplicationUser>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential(false);
            }
            else
            {
                builder.AddDeveloperSigningCredential(false);
                // throw new Exception("need to configure key material");
            }

            services.AddAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                AspNetIdentityDbContext aspNetIdentityDbContext = serviceScope.ServiceProvider.GetRequiredService<AspNetIdentityDbContext>();
                aspNetIdentityDbContext.Database.Migrate();

                PersistedGrantDbContext persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                persistedGrantDbContext.Database.Migrate();

                ConfigurationDbContext configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationDbContext.Database.Migrate();

                configurationDbContext.SaveChanges();
                //if (!configurationDbContext.Clients.Any())
                //{
                //    foreach (var client in Config.GetClients())
                //    {
                //        configurationDbContext.Clients.Add(client.ToEntity());
                //    }

                //    configurationDbContext.SaveChanges();
                //}

                //if (!configurationDbContext.IdentityResources.Any())
                //{
                //    foreach (var resource in Config.GetIdentityResources())
                //    {
                //        configurationDbContext.IdentityResources.Add(resource.ToEntity());
                //    }

                //    configurationDbContext.SaveChanges();
                //}

                //if (!configurationDbContext.ApiResources.Any())
                //{
                //    foreach (var api in Config.GetApis())
                //    {
                //        configurationDbContext.ApiResources.Add(api.ToEntity());
                //    }

                //    configurationDbContext.SaveChanges();
                //}
            }
        }
    }
}
