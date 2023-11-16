using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Chirp.Infrastructure;
using Chirp.Interfaces;
using Chirp.Models;
using DBContext;

namespace Chirp.StartUp
{
    public class Startup
    {
        private IConfiguration _configuration { get; set; }
        private IWebHostEnvironment _env { get; set; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env    = env;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultIdentity<Author>(options =>
            {

                // Sign-in Procedure [This has been disabled during Development-Phase]:
                options.SignIn.RequireConfirmedAccount = false;                     // Default is: true
                options.SignIn.RequireConfirmedEmail = false;                       // Default is: true

                // Lock Mechanism: [NOT ACTIVE DURING DEVELOPMENT]
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Sets the maximum amount of "Lock-Out" time.
                //options.Lockout.MaxFailedAccessAttempts = 5;                      // Sets the maximum number of failed attempts.

                // Password:
                options.Password.RequireDigit = false;                              // Default is: true
                options.Password.RequireLowercase = false;                          // Default is: true
                options.Password.RequireNonAlphanumeric = false;                    // Default is: true
                options.Password.RequireUppercase = false;                          // Default is: true
                options.Password.RequiredLength = 6;                                // Default is: 6
                options.Password.RequiredUniqueChars = 1;                           // Default is: 1

                // Users:
                options.User.RequireUniqueEmail = true;                             // Default is: true [..this was a massive pain in the ass...]

            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

            services.AddDbContext<DatabaseContext>(options =>
            {
                if(_env.IsDevelopment())
                {
                    Console.WriteLine("ENVIRONMENT == DEVELOPMENT");
                    string databasePath = Path.Combine(Path.GetTempPath(), "chirp.db");
                    options.UseSqlite($"Data Source={databasePath}");
                }
                else if(_env.IsProduction())
                {
                    Console.WriteLine("ENVIRONMENT == PRODUCTION");
                    string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("'ConnectionString' could not be located");
                    options.UseSqlServer(connectionString);
                }
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddRazorPages();


            services.AddScoped<ICheepRepository, CheepRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}