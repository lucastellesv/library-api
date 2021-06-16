using Library_API.Data;
using Library_API.Models;
using Library_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Library_API
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection").ToString(), ServerVersion.AutoDetect(Configuration.GetConnectionString("MySqlConnection").ToString())));
            services.AddControllers();

            string security_key = Configuration.GetSection("TokenAuthentication")["SecretKey"];
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(security_key));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;


            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.TokenValidationParameters.IssuerSigningKey = symetricSecurityKey;
                options.TokenValidationParameters.ValidAudience = Configuration.GetSection("TokenAuthentication")["Audience"];
                options.TokenValidationParameters.ValidIssuer = Configuration.GetSection("TokenAuthentication")["Issuer"];
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            });
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters = String.Empty;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            }).AddRoleManager<RoleManager<IdentityRole>>()
             .AddRoles<IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();


            services.AddCors();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<UserService>();
            services.AddHttpClient();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library_API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library_API v1"));
            }

            app.UseHttpsRedirection();
            app.UseCors(option => option
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            CreateRoles(serviceProvider).Wait();
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //adding custom roles
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
    

            User poweruser = new User
            {  
                UserName = "telles",
                Email = "meu@gmail.com"
            };

            string UserPassword = "123456";
            User _user = null;
            try
            {
                _user = await UserManager.FindByEmailAsync(poweruser.Email);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, UserPassword);
              }

        }

    }
}
