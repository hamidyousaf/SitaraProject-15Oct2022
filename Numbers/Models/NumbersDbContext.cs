using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Numbers.Models;
using Numbers.ViewModels;

namespace Numbers.Models
{
    public class NumbersDbContext : IdentityDbContext<ApplicationUser>     //DbContext
    {
        public NumbersDbContext()
        {

        }
        public NumbersDbContext(DbContextOptions<NumbersDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //unique constraint for Booking Customer
            modelBuilder.Entity<BkgCustomer>()
            .HasIndex(u => u.CNIC)
            .IsUnique();

            //Default values for GLVoucherDetail table 
            modelBuilder.Entity<GLVoucherDetail>()
            .Property(v => v.Debit)
            .HasDefaultValue(0);

            modelBuilder.Entity<GLVoucherDetail>()
            .Property(v => v.Credit)
            .HasDefaultValue(0);


            //IsDelete column for all tables
            modelBuilder.Entity<GLVoucher>()
           .Property(v => v.IsDeleted)
           .HasDefaultValue(false);

            modelBuilder.Entity<GLVoucherDetail>()
            .Property(v => v.IsDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<GLAccount>()
            .Property(a => a.IsDeleted)
            .HasDefaultValue(false);



        }
        public DbSet<AppAuditTrial> AppAuditTrials { get; set; }
        public DbSet<AppCompany> AppCompanies { get; set; }
        public DbSet<AppCurrency> AppCurrencies { get; set; }
        public DbSet<AppCurrencyExchangeRate> AppCurrencyExchangeRates { get; set; }
        public DbSet<AppDocumentAttachment> AppDocumentAttachments { get; set; }
        public DbSet<AppMenu> AppMenus { get; set; }
        public DbSet<AppPackage> AppPackages { get; set; }
        public DbSet<AppPeriod> AppPeriods { get; set; }
        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<GLVoucher> GLVouchers { get; set; }
        public DbSet<GLVoucherDetail> GLVoucherDetails { get; set; }
        public DbSet<GLVoucherType> GLVoucherTypes { get; set; }
        public DbSet<GLSubAccount> GLSubAccounts { get; set; }
        public DbSet<GLSubAccountDetail> GLSubAccountDetails { get; set; }
        public DbSet<AppCompanyConfig> AppCompanyConfigs { get; set; }
        public DbSet<AppRoleMenu> AppRoleMenus { get; set; }
        public DbSet<AppUserCompany> AppUserCompanies { get; set; }
        public DbSet<BkgItem> BkgItems { get; set; }
        public DbSet<BkgVehicle> BkgVehicles { get; set; }
        public DbSet<BkgReceipt> BkgReceipts { get; set; }
        public DbSet<BkgCustomer> BkgCustomers { get; set; }
        public DbSet<BkgPayment> BkgPayments { get; set; }
        public DbSet<BkgVehicleIGP> BkgVehicleIGPs { get; set; }
        public DbSet<BkgVehicleOGP> BkgVehicleOGPs { get; set; }
        public DbSet<BkgComissionReceived> BkgComissionReceiveds { get; set; }
        public DbSet<BkgComissionPayment> BkgComissionPayments { get; set; }
        public DbSet<BkgCashPurchaseSale> BkgCashPurchaseSales { get; set; }
        public DbSet<BkgCashPurchaseSalePayment> BkgCashPurchaseSalePayments { get; set; }
        public DbSet<BkgCashPurchaseSaleReceipt> BkgCashPurchaseSaleReceipts { get; set; }
        public DbSet<Sys_ORG_Profile> Sys_ORG_Profile { get; set; }
        public DbSet<Sys_ORG_Profile_Details> Sys_ORG_Profile_Details { get; set; }



    }
}
