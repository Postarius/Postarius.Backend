using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Autofac;
using Common;
using Data;
using Domain;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.Utils;
using Web.Admin.App;
using Web.Admin.Authorization;
using Web.Common.App;
using Web.Common.App.Attributes;

namespace Web.Admin
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public AppSettings AppSettings { get; set; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<PostariusContext>();

            ConfigureAppSettings(services);

            services.AddAuthentication(JwtAuthorizationConsts.FrontendAuthenticationScheme)
                .AddJwtBearer(JwtAuthorizationConsts.FrontendAuthenticationScheme, o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(AppSettings.SigninSecretKey.GetBytes()),
                        ValidIssuers = AppSettings.IssuerNames,
                        ValidAudiences = AppSettings.FrontendUrls
                    };
                });

            services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAdminRights()
                    .AddAuthenticationSchemes(JwtAuthorizationConsts.FrontendAuthenticationScheme)
                    .Build();
            });

            services.AddHttpContextAccessor();

            services.AddMvc(c => { c.EnableEndpointRouting = false; }).ConfigureApiBehaviorOptions(
                o => { o.SuppressMapClientErrors = true; }).AddControllersAsServices();
            
            services.AddHangfire(c =>
                c.UsePostgreSqlStorage(Configuration.GetConnectionString("PostgreSQL")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.Use((async (ctx, next) =>
            {
                var identity = ctx.User.Identity;
                if ((identity != null ? (!identity.IsAuthenticated ? 1 : 0) : 1) != 0)
                {
                    var authenticateResult = await ctx.AuthenticateAsync(JwtAuthorizationConsts.FrontendAuthenticationScheme);
                    if (authenticateResult.Succeeded && authenticateResult.Principal != null)
                        ctx.User = authenticateResult.Principal;
                }
                await next();
            }));

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseMvc();
        }
        
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new Services());
        }

        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        }
    }

    public class Services : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UsersRepository>().As<IUsersRepository>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<PasswordHashingService>().As<IPasswordHashingService>();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<PostsRepository>().As<IPostsRepository>();
            builder.RegisterType<PostService>().As<IPostService>();
            builder.RegisterType<SubscriptionRepository>().As<ISubscriptionRepository>();
            builder.RegisterType<AdminContextProvider>().As<IContextProvider>();
            builder.RegisterType<JwtService>().As<IJwtService>();
            builder.RegisterType<UserContextProvider>().As<IUserContextProvider>();
            builder.RegisterType<HasAccessToPostFilter>();
        }
    }
}