using ATMApplication.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using AutoMapper;
using ATMApplication.Mapping;
using System.Text.Json.Serialization;
using ATMApplication.Initial;

namespace ATMApplication
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ATMApplication", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<DbContext, ATMDbContext>(options =>
                options.UseSqlServer(connectionString),
                ServiceLifetime.Scoped
            );

            // Enum будут отображаться их именами (не цифрами)
            services.AddMvc().AddJsonOptions(opts =>
            {
                var enumConverter = new JsonStringEnumConverter();
                opts.JsonSerializerOptions.Converters.Add(enumConverter);
            });

            services.AddTransient<ISecurityService, SecurityService>();
            services.AddTransient<IDbService, SQLServerService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICookieService, CookieService>();
            services.AddTransient<ICardService, CardService>();
            services.AddTransient<IBankService, MyBankService>();
            services.AddTransient<IJwtUtils, JwtUtils>();
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddTransient<SignInManager>((serviceProvider) => new SignInManager(serviceProvider));

            services.AddClaimsAuthentication();
            services.AddHttpClient();

            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(provider.GetService<ISecurityService>()));
            })
            .CreateMapper());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ATMApplication v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
