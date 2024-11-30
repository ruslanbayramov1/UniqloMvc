using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;

namespace UniqloMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            //IoC - Inversion of Control
            builder.Services.AddDbContext<UniqloDbContext>(opt => {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSql"));
            }); 
            

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //using static files in wwwroot
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();
            

            app.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
                );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
