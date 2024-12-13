using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.DataAccess;
using UniqloMvc.Extensions;
using UniqloMvc.Helpers;
using UniqloMvc.Models;
using UniqloMvc.Services.Abstracts;
using UniqloMvc.Services.Implements;

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
                opt.UseSqlServer(builder.Configuration.GetConnectionString("AspMonsterRemote"));
            });

            builder.Services.AddIdentity<User, IdentityRole>(opt => {
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Password.RequiredLength = 3;
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredUniqueChars = 3;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
                opt.SignIn.RequireConfirmedEmail = false;
                opt.SignIn.RequireConfirmedAccount = false;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);
            })
                .AddEntityFrameworkStores<UniqloDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.Name));

            builder.Services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Account/Login";
                opt.AccessDeniedPath = "/Home/AccessDenied";
            });

            builder.Services.AddHttpContextAccessor();

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

            app.UseAuthorization();
            app.UseCustomUserData();

            app.MapControllerRoute("login", pattern:
                "login", new
                {
                    Controller="Account",
                    Action="Login",
                });

            app.MapControllerRoute("register", pattern:
                "register", new
                {
                    Controller="Account",
                    Action="Register"
                });

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
