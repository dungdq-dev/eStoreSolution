using AutoMapper;
using BusinessLogic.Catalog.Categories;
using BusinessLogic.Catalog.Products;
using BusinessLogic.Common.Email;
using BusinessLogic.Common.FileSystem;
using BusinessLogic.Sales.OrderDetails;
using BusinessLogic.Sales.Orders;
using BusinessLogic.System.Languages;
using BusinessLogic.System.Roles;
using BusinessLogic.System.Users;
using BusinessLogic.Utilities.Slides;
using Common.Constants;
using DataAccess.Data;
using DataAccess.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ViewModels.System.Users;
using WebApi.Mapper;

namespace WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(
                options => options.SuppressAsyncSuffixInActionNames = false);

            builder.Services.AddDbContext<EStoreDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString(SystemConstants.DefaultConnection) ??
                throw new InvalidOperationException($"Connection string '{SystemConstants.DefaultConnection}' not found.")));

            builder.Services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<EStoreDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.Cache());
            });

            // config automapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            // Declare DI
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddTransient<IOrderDetailService, OrderDetailService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IRoleService, RoleService>();

            builder.Services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            builder.Services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
            builder.Services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();

            builder.Services.AddScoped<IStorageService, FileStorageService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<ISlideService, SlideService>();
            builder.Services.AddScoped<ILanguageService, LanguageService>();

            builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();

            builder.Services.AddHttpContextAccessor();

            // add cors to access api for single-page app
            var eStoreAllowSpecificOrigins = "_eStoreAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: eStoreAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy
                                        .WithOrigins("http://localhost:5173")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                  });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Electronix Store", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                        Enter 'Bearer' [space] and then your token in the text input below.
                        \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            string issuer = builder.Configuration.GetValue<string>("Tokens:Issuer");
            string signingKey = builder.Configuration.GetValue<string>("Tokens:Key");
            byte[] signingKeyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseOutputCache();

            app.MapControllers();

            app.UseCors(eStoreAllowSpecificOrigins);

            app.UseAuthentication();

            app.UseAuthorization();

            app.Run();
        }
    }
}