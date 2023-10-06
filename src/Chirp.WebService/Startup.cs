using Chirpin.Data;
using DBContext;
using Microsoft.EntityFrameworkCore;

/*
    @DESCRIPTION:
        - The 'Startup.cs' file is an industry standard throughout many C# projects.
        - The aim of the file is to encapsulate the applicatins services and middleware being injected on start up.

*/

namespace Chirpin.Startup
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Adds services to our Web App [Called at Runtime]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddDbContext<DatabaseContext>(options => {
                _ = options.UseSqlite(_configuration.GetConnectionString("DefaultConnection")) ?? throw new InvalidOperationException("Connection string was invalid.");
            });

            //services.AddDatabaseDeveloperPageExceptionFilter();
        }


        /*
            @Aim :: - Used to inject Middleware. 
                    - Middleware is software which sits between the application and the web server, 
                      and is responsible for further interacting with the incoming HTTP Requests / Responses.
        
        */
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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