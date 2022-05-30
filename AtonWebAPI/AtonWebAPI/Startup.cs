using AtonWebAPI.Authentication;
using AtonWebAPI.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;

namespace AtonWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRepository, UserRepository>();
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            services.AddControllers();
            services.AddControllers(config =>
            {
                config.Filters.Add(new AuthorizeFilter());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "UserApi", Version = "v1"});
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.Http,  
                    Scheme = "basic",  
                    In = ParameterLocation.Header,  
                    Description = "Basic Authorization header using the Bearer scheme."  
                });  
                c.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  
                        new OpenApiSecurityScheme  
                        {  
                            Reference = new OpenApiReference  
                            {  
                                Type = ReferenceType.SecurityScheme,  
                                Id = "basic"  
                            }  
                        },  
                        new string[] {}  
                    }  
                });  
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UsersApi v1"));
            }

            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}