using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using Zwyssigly.ImageServer.Core;
using Zwyssigly.ImageServer.Embedded;
using Zwyssigly.ImageServer.MongoDb;
using Zwyssigly.ImageServer.Security;
using Zwyssigly.ImageServer.Standalone.Security;
using Zwyssigly.ImageServer.Thumbnails.Disk;

namespace Zwyssigly.ImageServer.Standalone
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddImageServerEmbedded();

            var mongo = Configuration.GetSection("Mongo").Get<MongoDbOptions>();
            if (!string.IsNullOrEmpty(mongo?.ConnectionString))
            {
                services.AddImageServerMongoDb(mongo);
            }

            var disk = Configuration.GetSection("Disk").Get<DiskOptions>();
            if (!string.IsNullOrEmpty(disk?.Directory))
                services.AddImageServerDisk(disk);

            var cors = Configuration.GetSection("Cors").Get<CorsOptions>();
            services.AddCors(options =>
            {
                options.AddPolicy(name: "default", builder => builder
                    .WithOrigins(cors.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });

            var login = Configuration.GetSection("Login").Get<InitialLoginOptions>();
            services.AddSingleton(login);

            services.AddAuthentication("custom").AddScheme<CustomAuthenticationOptions, CustomAuthenticationHandler>("custom", null);
            services.AddAuthorization(o =>
            {
                foreach (var permission in Permission.All)
                    o.AddPolicy(permission.ToString(), policy => policy.RequireClaim(PermissionRequirement.ClaimType, permission.ToString()));
                //.AddRequirements(new PermissionRequirement(permission)));
            });

            /*
            var authentication = services.AddAuthentication();

            var jwt = Configuration.GetSection("Auth").GetSection("Jwt").Get<JwtOptions>();
            if (jwt?.ValidIssuers != null && jwt.ValidIssuers.Length > 0) {
                authentication.AddJwtBearer(options =>
                {
                    options.TokenValidationParameters.ValidIssuers = jwt.ValidIssuers;
                    options.TokenValidationParameters.ValidAudiences = jwt.ValidAudiences;

                    options.TokenValidationParameters.RequireAudience = true;
                    options.TokenValidationParameters.RequireExpirationTime = true;
                    options.TokenValidationParameters.RequireSignedTokens = true;

                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                    options.TokenValidationParameters.ValidateTokenReplay = true;

                    options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(1);
                });
            } */

            services.AddControllers().AddNewtonsoftJson(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Map("/api", api =>
            {
                if (env.IsDevelopment())
                {
                    api.UseDeveloperExceptionPage();
                }

                api.UseRouting();

                api.UseAuthentication();
                api.UseAuthorization();

                api.UseCors("default");

                api.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });

            app.MapWhen(ctx => ctx.Request.Path.HasValue && ctx.Request.Path.Value.StartsWith("/ui"), spa =>
            {
                if (env.IsDevelopment())
                {
                    spa.UseSpa(config =>
                    {
                        config.UseProxyToSpaDevelopmentServer("http://localhost:8080/");
                    });
                }
                else
                {
                    var staticFileOptions = new StaticFileOptions
                    {
                        RequestPath = "/",
                        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "ui")),
                    };

                    spa.UseSpaStaticFiles(staticFileOptions);

                    spa.UseSpa(config =>
                    {
                        config.Options.DefaultPage = "/ui/index.html";
                        config.Options.DefaultPageStaticFileOptions = staticFileOptions;
                    });
                }
            });
        }
    }
}
