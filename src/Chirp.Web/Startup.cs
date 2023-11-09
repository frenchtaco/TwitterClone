using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Chirp.Infrastructure;
using Chirp.Interfaces;
using Chirp.Models;
using DBContext;

/*
    @DESCRIPTION:
        - The 'Startup.cs' file is an industry standard throughout many C# projects.
        - The aim of the file is to encapsulate the applicatins services and middleware being injected on start up.
        - The file is divide into 2 sections:
                01. Configure Services:
                    -> Used to configure Depedency Injection
                02. Configure Middle:
                    -> Used to inject Middleware into the Request Pipeline - [https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-7.0]
    
        - Both of these sections perform Dependecy Injection (DI). 
            -> The core aim of DI is to "Inversion of Control" a design principle wherein classes are designed to recieve objects rather
               than instantiate it within the class, allowing for loose coupling between objects.
            -> The resulting applications are more testable, modular, and maintainable as a result.
            -> Example: [https://stackoverflow.com/questions/3058/what-is-inversion-of-control].

        - For future reference - OAuth, Cookies and JWT Security [https://fusionauth.io/blog/securing-asp-netcore-razor-pages-app-with-oauth]
*/


/*
    if(SignInManager.IsSignedIn)

*/

namespace Chirp.StartUp
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
     options.UseSqlServer(
         _configuration.GetConnectionString("DefaultConnection"),
         b => b.MigrationsAssembly("Chirp.Web")
     )
 );


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
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DatabaseContext>();

            services.AddRazorPages();

            // services.ConfigureApplicationCookie(option => { ... https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.cookies.cookieauthenticationoptions?view=aspnetcore-7.0 ... });

            // When you request an ICheepRepository in your application, ASP.NET Core's dependency injection system will 
            // create an instance of CheepRepository and provide it to you.
            services.AddScoped<ICheepRepository, CheepRepository>();
            //services.AddScoped<IAuthorRepository, AuthorRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}