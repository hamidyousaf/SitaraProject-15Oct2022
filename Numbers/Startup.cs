using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
//using Hangfire;
//using Hangfire.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Numbers.Controllers;
using Numbers.Entity.Models;
using Numbers.Helpers;
using Numbers.Interface;
using Numbers.Repository.AP;
using Numbers.Repository.AppModule;
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using Numbers.Repository.Inventory;
//using Hangfire.MemoryStorage;
namespace Numbers
{
    public class Startup
    {
        public IConfigurationRoot ConfigurationRoot { get; }
       
        public Startup(IWebHostEnvironment hostingEnvironment)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            ConfigurationRoot = new ConfigurationBuilder()
                           .SetBasePath(hostingEnvironment.ContentRootPath)
                           .AddJsonFile("appsettings.json")
                           .Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-NZ");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("en-NZ") };
            });
            services.AddDbContext<NumbersDbContext>(options =>
                                options.UseSqlServer(ConfigurationRoot.GetConnectionString("NumbersConnection")).EnableSensitiveDataLogging());

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<NumbersDbContext>().AddDefaultTokenProviders();
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(10);
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);//locks out user for half an hour,if u need to unlock a user before declared time, simply simply set LockoutEnabled=false in dbo.AspNetUsers 
                options.Lockout.MaxFailedAccessAttempts = 5;//counts invalid login attempts
                options.Lockout.AllowedForNewUsers = true;
            });

            //services.AddHangfire(options =>
            //{
            //    options.UseSqlServerStorage(ConfigurationRoot.GetConnectionString("NumbersConnection"));
            //});
          
            //configure memory session

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(10);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = ConfigurationRoot.GetConnectionString("NumbersConnection");
                options.SchemaName = "dbo";
                options.TableName = "Cache";
                options.ExpiredItemsDeletionInterval = TimeSpan.FromDays(10);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddOptions();
            services.Configure<Helpers.CommonHelper>(ConfigurationRoot.GetSection("ReportPath"));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddScoped<ARDiscountRepository>();
            services.AddScoped<CustomerRepository>();
            // Bell Notification
            services.AddScoped<NotificationService>();
            services.AddScoped<PurchaseRepository>();
            services.AddScoped<ResponsibilityRepository>();
            services.AddScoped<SysApprovalGroupRepository>();
            services.AddScoped<SysApprovalGroupDetailsRepository>();
            services.AddScoped<Sys_ORG_ProfileRepository>();
            services.AddScoped<Sys_ORG_Profile_DetailsRepository>();
            services.AddScoped<SysOrganizationRepository>();
            services.AddScoped<SysOrgClassificationRepository>();
            services.AddScoped<Sys_Rules_ApprovalRepository>();
            services.AddScoped<Sys_Rules_Approval_DetailsRepository>();
            services.AddScoped<APPurchaseRequisitionDetailsRepository>();
            services.AddScoped<APPurchaseRequisitionRepository>();
            services.AddScoped<Sys_Rules_Approval_DetailsRepository>();
            services.AddScoped<APComparativeStatementRepository>();
            services.AddScoped<APCSRequestDetailRepository>();
            services.AddScoped<APCSRequestRepository>();
            services.AddScoped<APIRNRepository>();
            services.AddScoped<APIRNDetailsRepository>();
            services.AddScoped<APOGPRepository>();
            services.AddScoped<APOGPDetailsRepository>();
            services.AddScoped<APShipmentRepository>();
            services.AddScoped<APShipmentDetailsRepository>();

            services.AddScoped<APComparativeStatementRepository>();
            services.AddScoped<APIGPRepository>();
            services.AddScoped<APIGPDetailsRepository>();
            services.AddScoped<APOGPRepository>();
            services.AddScoped<APOGPDetailsRepository>();
            services.AddScoped<APLCRepository>();
            services.AddScoped<APInsuranceInfoRepository>();
            services.AddScoped<APGRNExpenseRepository>();
            services.AddScoped<APCustomInfoRepository>();
            services.AddScoped<APCustomInfoDetailRepository>();
            services.AddScoped<ARCustomerAdjustmentItemRepository>();
            services.AddScoped<ARCustomerDiscountAdjustmentRepository>();
            services.AddScoped<ARSalePersonRepository>();
            services.AddScoped<ARMonthlySaleTargetRepository>();
            services.AddScoped<ARAnualySaleTargetRepository>();
            services.AddScoped<ARSalePersonItemCategoryRepository>();
            services.AddScoped<ARSalePersonCityRepository>();
            services.AddScoped<ARDiscountItemRepository>();

            // Customer Discount Adjustment (New)

            services.AddScoped<ARDiscountAdjustmentRepository>();
            services.AddScoped<ARDiscountAdjustmentItemRepository>();

            services.AddScoped<ItemPricingsRepository>();
            services.AddScoped<ItemPricingDetailsRepository>();
            services.AddScoped<ARRecoveryPercentageRepository>();
            services.AddScoped<ARRecoveryPercentageItemRepository>();
            services.AddScoped<ARCommissionAgentPaymentGenerationRepository>();
            services.AddScoped<ARCommissionAgentPaymentGenerationDetailsRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();
            services.AddScoped<IBackGroundRefresh, BackGroundRefresh>();


            // New Cron Job working
            //  services.AddHangfireServer();
            //services.AddHangfire(config =>
            //    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseDefaultTypeSerializer()
            //    .UseMemoryStorage());
            //services.AddHangfireServer();
            //
            // End Cron Job



            //services.AddMemoryCache();
            //services.AddSession(options =>
            //{
            //    Set a short timeout for easy testing.

            //   options.IdleTimeout = TimeSpan.FromDays(10);
            //});



            // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddMvc();

        }
        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityResult roleResult;
            //Adding Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));

            }
            //Assign Admin role to the main User here we have given our newly registered 
            //login id for Admin management
            //ApplicationUser user = await UserManager.FindByEmailAsync("josh4ssrk@yahoo.com");
            //if (user != null)
            //{
            //    var User = new ApplicationUser();
            //    await UserManager.AddToRoleAsync(user, "Admin");
            //}

            string[] roleNames = { "Admin", "Manager", "Member" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 2
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //Here you could create a super user who will maintain the web app
            //var poweruser = new ApplicationUser
            //{
            //    UserName = "atifamin",
            //    Email = "atifamin@visionplus.com.pk",
            //};

            //string userPWD = "visionplus@786";
            //var _user = await UserManager.FindByEmailAsync("atifamin@visionplus.com.pk");

            //if (_user == null)
            //{
            //    var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
            //    if (createPowerUser.Succeeded)
            //    {
            //        //here we tie the new user to the role : Question 3
            //        await UserManager.AddToRoleAsync(poweruser, "Manager");

            //    }
            //}
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
       // public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services,IBackGroundRefresh bck, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            // Using hangfire library for background job                                  Install-Package Hangfire
            //app.UseHangfireDashboard(); if you want to see the dashboard uncomment it
            // app.UseHangfireServer();
            //app.UseHangfireDashboard();

            //recurringJobManager.AddOrUpdate(
            //    "Run At 12am Everyday",
            //    Job.FromExpression(() => bck.SaleOrderExpiration(_db)),
            //    // Cron.Daily(2)
            //    Cron.Minutely()
            //    );


            //New Cron Job Implementation

            //app.UseHangfireDashboard();
            //backgroundJobClient.Enqueue(() => Console.WriteLine("Hello Hanfire job!"));

            //recurringJobManager.AddOrUpdate(
            //    "Run every minute",
            //    () => serviceProvider.GetService<IBackGroundRefresh>(),
            //    "* * * * *"
            //    );

            // -------------------------------------------------//

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                        name: "AreaGL",
                        areaName: "GL",
                        pattern: "GL/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaBooking",
                        areaName: "Booking",
                        pattern: "Booking/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaInventory",
                        areaName: "Inventory",
                        pattern: "Inventory/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                       name: "AreaGreige",
                       areaName: "Greige",
                       pattern: "Greige/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                     name: "AreaPlanning",
                     areaName: "Planning",
                     pattern: "Planning/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaAR",
                        areaName: "AR",
                        pattern: "AR/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaAP",
                        areaName: "AP",
                        pattern: "AP/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaReport",
                        areaName: "Report",
                        pattern: "Report/{controller=Report}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaHR",
                        areaName: "HR",
                        pattern: "HR/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaSetup",
                        areaName: "Setup",
                        pattern: "Setup/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaApplicationModule",
                        areaName: "ApplicationModule",
                        pattern: "ApplicationModule/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaSecurity",
                        areaName: "Security",
                        pattern: "Security/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaApproval",
                        areaName: "Approval",
                        pattern: "Approval/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute(
                        name: "AreaReports",
                        areaName: "Reports",
                        pattern: "Reports/{controller=Home}/{action=Index}/{module?}");
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Account}/{action=Login}/{id?}");
                endpoints.MapRazorPages();

            });

            CreateUserRoles(services).Wait();
        }
    }
}