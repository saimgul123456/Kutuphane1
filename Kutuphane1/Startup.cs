using System;
using System.Globalization;
using Kutuphane1.Context;
using Kutuphane1.ModelsIdentity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Kutuphane1.Helpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Kutuphane1
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services
          .AddControllersWithViews(opt => opt.EnableEndpointRouting = false)
          .AddRazorRuntimeCompilation();

      //RazorPages yapýlarýný rota olarak kullanmak için ekliyoruz.
      services
          .AddRazorPages();

      //UseSqlServer:  EntityFrameworkCore.SqlServer kütüphanesi yardýmýyla yazýlýr.
      services
          .AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("DBConStr")));
      services
          .AddDbContext<IdentityContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Identity")));
      services
          .AddIdentity<CustomUser, CustomRole>()
          .AddEntityFrameworkStores<IdentityContext>()
          .AddDefaultTokenProviders();

      //E-Posta Gönderebilmemiz için ortak bir service oluþturduk.
      services.ConfigureApplicationCookie(options =>
      {
        options.LoginPath = "/identity/account/login";
        options.LogoutPath = "/identity/account/logout";
        options.AccessDeniedPath = "/identity/account/accessdenied";
      });

			//E-Posta Gönderebilmemiz için ortak bir service oluþturduk.
			services
					.AddTransient<IEmailSender, EmailSender>();
			//**
			services.AddSession(session => { session.IdleTimeout = TimeSpan.FromMinutes(20); });

      services.AddControllersWithViews().AddRazorRuntimeCompilation();

      //**
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();

      //Statik olan dosyalarýn bulunacaðý wwwroot klasörünü kullanýma açar.
      app.UseStaticFiles();


      //Kimlik Doðrulama: Kullanýcý yapýsný denetler.
      app.UseAuthentication();

      //**
      app.UseRouting();

      //**

      //Yetkilendirme: Eriþimleri denetler.
      app.UseAuthorization();

      //**
      app.UseSession();

      var ci = new CultureInfo("tr-TR");
      CultureInfo.DefaultThreadCurrentCulture = ci;
      CultureInfo.DefaultThreadCurrentCulture = ci;
      CultureInfo.CurrentCulture = ci;

      ci.NumberFormat.NumberDecimalSeparator = ".";
      ci.NumberFormat.CurrencyDecimalSeparator = ".";


      //**

      //Sayfa adres rota sistemini kullanmamýzý saðlar.
      app.UseMvc(route =>
      {
        //SEO açýsýndan uzun url yapýlarýný kýsaltmak amacýyla yeniden yazabiliriz, lokal projelerde (bence) hiç gerek yok.
        route.MapRoute(
            name: "privacypage",
            template: "privacy",
            defaults: new { controller = "Home", action = "Privacy" });

        //area: Bu yapý controller veya Razorpage projelerini tek klasör (yol) adý altýnda toplamak için kullanýlýr.
        route.MapRoute(
            name: "areas",
            template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        route.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
