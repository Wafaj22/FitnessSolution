using System;
using FitnessSolution.Areas.Identity.Data;
using FitnessSolution.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(FitnessSolution.Areas.Identity.IdentityHostingStartup))]
namespace FitnessSolution.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<FitnessSolutionContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("FitnessSolutionContextConnection")));

                services.AddDefaultIdentity<FitnessSolutionUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<FitnessSolutionContext>();
            });
        }
    }
}