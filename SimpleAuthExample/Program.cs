using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleAuthExample.DB;

namespace SimpleAuthExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;
            // Add services to the container.

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<SimpleAuthContext>(x =>
            {
                x.UseNpgsql(config.GetConnectionString("DbConnectionString"));
            });


            builder.Services.AddAuthentication(
                x=>
                {
                    x.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    x.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    x.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }
                )
                 .AddCookie(x=>
                 {
                     x.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                     x.SlidingExpiration = true;
                 });
            builder.Services.AddAuthorization(x =>
            {
                x.DefaultPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
                x.AddPolicy("UserOrAdmin", new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .RequireRole("admin", "user")
                .Build());
                x.AddPolicy("Admin", new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
               .RequireAuthenticatedUser()
               .RequireRole("admin")
               .Build());
            });
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
               
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                x.RoutePrefix = string.Empty;
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();
            app.MapSwagger();
            app.UseCookiePolicy();

            app.UseCors();

            app.UseAuthorization();
            app.UseAuthentication();

            app.Run();
        }
    }
}
