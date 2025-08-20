using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Interfaces;
//using DraughtSurveyWebApp.Mappings;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.Middleware;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;

namespace DraughtSurveyWebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add user-secrets
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }

            // DB
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity
            builder.Services
                .AddDefaultIdentity<ApplicationUser>(options => 
                { 
                    options.SignIn.RequireConfirmedAccount = true; 
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options => 
            {
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            });
            
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<SurveyCalculationsService>();

            // EMAIL: Options
            builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
            builder.Services.Configure<SendGridOptions>(builder.Configuration.GetSection("SendGrid"));
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

            // EMAIL: provider
            var emailProvider = builder.Configuration["Email:Provider"] ?? "SendGrid";


            if (emailProvider.Equals("Smtp", StringComparison.OrdinalIgnoreCase))
            {                
                builder.Services.AddTransient<DraughtSurveyWebApp.Interfaces.IEmailSender, SmtpEmailSender>();                
            }
            else
            {                
                builder.Services.AddTransient<DraughtSurveyWebApp.Interfaces.IEmailSender, SendGridEmailSender>();                
            }

            builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender>(sp =>
            {
                var inner = sp.GetRequiredService<DraughtSurveyWebApp.Interfaces.IEmailSender>();
                return new IdentityEmailSenderAdapter(inner);                
            });

            //builder.Services.AddAutoMapper(typeof(MappingProfile));
            

            var app = builder.Build();

            // Apply migrations and seed admin roles/users
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbInitializer.InitializeAsync(services);
            }

            // Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");                
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            if (builder.Configuration.GetValue<bool>("Auth:Disable2FA", true))
            {
                var blocked = new[]
                    {
                        "/Identity/Account/Manage/TwoFactorAuthentication",
                        "/Identity/Account/Manage/EnableAuthenticator",
                        "/Identity/Account/Manage/Disable2fa",
                        "/Identity/Account/Manage/ResetAuthenticator",
                        "/Identity/Account/Manage/GenerateRecoveryCodes"
                    };

                app.Use(async (ctx, next) =>
                {
                    if (blocked.Any(p => ctx.Request.Path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)))
                    {
                        ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                        await ctx.Response.WriteAsync("Two-factor authentication is disabled.");
                        return;
                    }
                    await next();
                });
            }

            app.UseMiddleware<SessionTrackingMiddleware>();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            

            app.Run();
        }
    }

    // adapter
    internal sealed class IdentityEmailSenderAdapter : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        private readonly DraughtSurveyWebApp.Interfaces.IEmailSender _inner;

        public IdentityEmailSenderAdapter(DraughtSurveyWebApp.Interfaces.IEmailSender inner)
        {
            _inner = inner;
        }


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return _inner.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
