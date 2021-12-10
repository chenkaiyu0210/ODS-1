using fontWebCore.Common.Context;
using fontWebCore.Common.Function;
using fontWebCore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace fontWebCore
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
            services.AddControllersWithViews();
            services.AddDbContext<ODSContext>(options => options.UseSqlServer("Name=ConnectionStrings:ODSConnect"));
            services.AddTransient<EncryptionProcessor<SHA256Processor>, SHA256Processor>();
            services.AddSession();


            //將appsetting中一個settingConfig設定至Model中
            settingConifgModel setting = new settingConifgModel();
            Configuration.GetSection("SettingConfig").Bind(setting);
            services.AddSingleton(setting);

            //從組態讀取登入逾時設定
            //double LoginExpireMinute = this.Configuration.GetValue<double>("loginExpireMinute");
            //註冊 CookieAuthentication，Scheme必填
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                //或許要從組態檔讀取，自己斟酌決定
                option.LoginPath = new PathString("/Home/Login");//登入頁
                option.LogoutPath = new PathString("/Home/Logout");//登出Action
                //用戶頁面停留太久，登入逾期，或Controller的Action裡用戶登入時，也可以設定↓
                option.ExpireTimeSpan = TimeSpan.FromMinutes(setting.loginExpireMinute);//沒給預設14天
                //↓資安建議false，白箱弱掃軟體會要求cookie不能延展效期，這時設false變成絕對逾期時間
                //↓如果你的客戶反應明明一直在使用系統卻容易被自動登出的話，你再設為true(然後弱掃policy請客戶略過此項檢查) 
                option.SlidingExpiration = false;
            });

            services.AddControllersWithViews(options =>
            {
                //↓和CSRF資安有關，這裡就加入全域驗證範圍Filter的話，待會Controller就不必再加上[AutoValidateAntiforgeryToken]屬性
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddMvc(config =>
            {
                // MVC 服務中註冊 Filter，這樣就可以套用到所有的 Request
                config.Filters.Add(new AuthorizeFilter());
            });
            // 圖形驗證
            services.AddMemoryCache()
                .AddSimpleCaptcha(builder =>
                {
                    builder.UseMemoryStore();
                    builder.AddConfiguration(options =>
                    {
                        //設定驗證碼長度
                        options.CodeLength = 6;
                        //設定圖片大小
                        //options.ImageWidth = 100;
                        //options.ImageHeight = 36;
                        //設定是否區分大小寫
                        options.IgnoreCase = false;
                        //驗證碼預設的有效期為5分鐘
                        //options.ExpiryTime = TimeSpan.FromMinutes(5);
                        options.CodeGenerator = new MyCaptchaCodeGenerator();
                    });
                });
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
        app.UseStaticFiles();

        app.UseRouting();

        // 留意寫Code順序，先執行驗證...
        app.UseAuthentication();
        //Controller、Action才能加上 [Authorize] 屬性
        app.UseAuthorization();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
}
