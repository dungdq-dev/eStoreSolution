using ApiIntegration;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System.Users;

namespace AdminApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add services to the container.
            builder.Services.AddHttpClient();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Users/Logout";
                    options.AccessDeniedPath = "/Login";
                });

            builder.Services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            // Deprecated
            //builder.Services.AddControllersWithViews()
            //    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>());

            builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            builder.Services.AddFluentValidationAutoValidation(conf =>
            {
                conf.DisableDataAnnotationsValidation = true;
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddTransient<IUserApiClient, UserApiClient>();
            builder.Services.AddTransient<IRoleApiClient, RoleApiClient>();
            builder.Services.AddTransient<ILanguageApiClient, LanguageApiClient>();
            builder.Services.AddTransient<IProductApiClient, ProductApiClient>();
            builder.Services.AddTransient<ICategoryApiClient, CategoryApiClient>();
            builder.Services.AddTransient<IOrderApiClient, OrderApiClient>();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
