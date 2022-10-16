using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Numbers.Entity.ViewModels;

namespace Numbers.Entity.Models
{
    public class NumbersDbContext : IdentityDbContext<ApplicationUser>     //DbContext
    {
        private readonly IHttpContextAccessor IHttpContext;
        public NumbersDbContext()
        {
        }
        public NumbersDbContext(IHttpContextAccessor httpContext)
        {
            IHttpContext = httpContext;
        }
        public NumbersDbContext(DbContextOptions<NumbersDbContext> options, IHttpContextAccessor httpContext) : base(options)
        {
            IHttpContext = httpContext;
            //_companyId = IHttpContext.HttpContext.Session.GetInt32("CompanyId").Value;
            //_userId = IHttpContext.HttpContext.Session.GetString("UserId");

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)//Used whene we create new object of dbContext
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("NumbersConnection");
                optionsBuilder.UseSqlServer(connectionString).EnableSensitiveDataLogging();
            }
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
        public DbSet<FGSInwardGatePassDetail> FGSInwardGatePassDetails { get; set; }
        public DbSet<FGSInwardGatePass> FGSInwardGatePasses { get; set; }
        public DbSet<GLDebitCreditMemoDetail> GLDebitCreditMemoDetails { get; set; }
        public DbSet<GLDebitCreditMemo> GLDebitCreditMemos { get; set; }
        public DbSet<FGSOutwardGatePass> FGSOutwardGatePasses { get; set; }
        public DbSet<FGSOutwardGatePassDetails> FGSOutwardGatePassDetails { get; set; }
        public DbSet<PlanInvoice> PlanInvoices { get; set; }
        public DbSet<PlanInvoiceDetail> PlanInvoiceDetails { get; set; }
        public DbSet<InvBOM> InvBOM { get; set; }
        public DbSet<InvBOMDetail> InvBOMDetail { get; set; }
        public DbSet<BomAccessories> BomAccessories { get; set; }
        public DbSet<BomAccessoriesDetail> BomAccessoriesDetail { get; set; }
        public DbSet<InterSegmentalSaleTransfer> InterSegmentalSaleTransfer { get; set; }
        public DbSet<InterSegmentalSaleTransferDetail> InterSegmentalSaleTransferDetail { get; set; }
        public DbSet<WarpIssuance> WarpIssuances { get; set; }
        public DbSet<WeftIssuance> WeftIssuances { get; set; }
        public DbSet<BillOfMaterial> BillOfMaterials { get; set; }
        public DbSet<ProductionOrderItem> ProductionOrderItems { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<GRGriegeRequisition> GRGriegeRequisition { get; set; }
        public DbSet<GRGriegeRequisitionDetails> GRGriegeRequisitionDetails { get; set; }     
        public DbSet<GRGriegeRequisitionST> GRGriegeRequisitionST { get; set; }
        public DbSet<GRGriegeRequisitionDetailsST> GRGriegeRequisitionDetailsST { get; set; }
        public DbSet<GRStackingItem> GRStackingItems { get; set; }
        public DbSet<GRStacking> GRStackings { get; set; }
        public DbSet<GRGRNItem> GRGRNItems { get; set; }
        public DbSet<GRGRN> GRGRNS { get; set; }
        public DbSet<GRGRNInvoice> GRGRNInvoices { get; set; }
        public DbSet<GRGRNInvoiceDetail> GRGRNInvoiceDetails { get; set; }
        public DbSet<GRInwardGatePassDetail> GRInwardGatePassDetails { get; set; }
        public DbSet<GRInwardGatePass> GRInwardGatePass { get; set; }
        public DbSet<GRPurchaseContract> GRPurchaseContract { get; set; }
        public DbSet<GRQuality> GRQuality { get; set; }
        public DbSet<ARPackingItems> ARPackingItems { get; set; }
        public DbSet<SaleReturnItems> SaleReturnItems { get; set; }
        public DbSet<SaleReturn> SaleReturn { get; set; }
        public DbSet<ARSaleReturnInwardGatePass> ARSaleReturnInwardGatePass { get; set; }
        public DbSet<ARInwardGatePass> ARInwardGatePass { get; set; }
        public DbSet<ARPacking> ARPacking { get; set; }
        public DbSet<ARSaleReturnInvoice> ARSaleReturnInvoice { get; set; }
        public DbSet<ARSaleReturnInvoiceItems> ARSaleReturnInvoiceItems { get; set; }
        public DbSet<AROutwardGatePass> AROutwardGatePass { get; set; }
        public DbSet<BaleInformation> BaleInformation { get; set; }
        public DbSet<GRCategory> GRCategory { get; set; }
        public DbSet<GRConstruction> GRConstruction { get; set; }
        public DbSet<ARRecoveryPercentageItem> ARRecoveryPercentageItem { get; set; }
        public DbSet<ARRecoveryPercentage> ARRecoveryPercentage { get; set; }
        public DbSet<ARDiscount> ARDiscount { get; set; }
        public DbSet<ARCreditLimit> ARCreditLimit { get; set; }
        public DbSet<AppAuditTrial> AppAuditTrials { get; set; }
        public DbSet<APShipment> APShipment { get; set; }
        public DbSet<APShipmentDetail> APShipmentDetails { get; set; }
        public DbSet<AppCompany> AppCompanies { get; set; }
        public DbSet<AppModule> AppModules { get; set; }
        public DbSet<AppCompanyModule> AppCompanyModules { get; set; }
        public DbSet<AppTax> AppTaxes { get; set; }
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
        public DbSet<AppCompanyConfigBase> AppCompanyConfigBases { get; set; }
        public DbSet<AppCompanySetup> AppCompanySetups { get; set; }
        public DbSet<AppRoleMenu> AppRoleMenus { get; set; }
        public DbSet<AppUserCompany> AppUserCompanies { get; set; }
        public DbSet<BkgVehicle> BkgVehicles { get; set; }
        public DbSet<BkgReceipt> BkgReceipts { get; set; }
        public DbSet<BkgCustomer> BkgCustomers { get; set; }
        public DbSet<BkgPayment> BkgPayments { get; set; }
        public DbSet<BkgVehicleIGP> BkgVehicleIGPs { get; set; }
        public DbSet<BkgVehicleOGP> BkgVehicleOGPs { get; set; }
        public DbSet<BkgCommissionReceived> BkgCommissionReceiveds { get; set; }
        public DbSet<BkgCommissionPayment> BkgCommissionPayments { get; set; }
        public DbSet<BkgCashPurchaseSale> BkgCashPurchaseSales { get; set; }
        public DbSet<BkgAdvanceBooking> BkgAdvanceBookings { get; set; }
        public DbSet<InvItem> InvItems { get; set; }
        public DbSet<InvAdjustment> InvAdjustments { get; set; }
        public DbSet<InvAdjustmentItem> InvAdjustmentItems { get; set; }
        public DbSet<AppReportQueue> AppReportQueues { get; set; }
        public DbSet<AppReportQueueParameters> AppReportQueueParameters { get; set; }
        public DbSet<AppReport> AppReports { get; set; }
        public DbSet<ARCustomer> ARCustomers { get; set; }
        public DbSet<ARCommissionAgent> ARCommissionAgents { get; set; }
        public DbSet<ARDeliveryChallan> ARDeliveryChallans { get; set; }
        public DbSet<ARDeliveryChallanItem> ARDeliveryChallanItems { get; set; }
        public DbSet<ARInvoice> ARInvoices { get; set; }
        public DbSet<ARInvoiceItem> ARInvoiceItems { get; set; }
        public DbSet<ARContactPerson> ARContactPerson { get; set; }
        public DbSet<ARShippingDetail> ARShippingDetail { get; set; }

        public DbSet<ARReceipt> ARReceipts { get; set; }
        public DbSet<ARReceiptInvoice> ARReceiptInvoices { get; set; }
        public DbSet<ARSaleOrder> ARSaleOrders { get; set; }
        public DbSet<ARSaleOrderItem> ARSaleOrderItems { get; set; }
        public DbSet<ARSaleReturn> ARSaleReturns { get; set; }
        public DbSet<ARSaleReturnItem> ARSaleReturnItems { get; set; }
        public DbSet<ARDiscountAdjustment> ARDiscountAdjustment { get; set; }
        public DbSet<ARDiscountAdjustmentItem> ARDiscountAdjustmentItem { get; set; }
        public DbSet<ARCustomerAdjustmentItem> ARCustomerAdjustmentItem { get; set; }
        public DbSet<ARCustomerDiscountAdjustment> ARCustomerDiscountAdjustment { get; set; }
        public DbSet<APAdvance> APAdvances { get; set; }
        public DbSet<ARAdvance> ARAdvances { get; set; }
        public DbSet<APSupplier> APSuppliers { get; set; }
        public DbSet<AppCountry> AppCountries { get; set; }
        public DbSet<AppCitiy> AppCities { get; set; }
        public DbSet<APPurchase> APPurchases { get; set; }
        public DbSet<APPurchaseOrder> APPurchaseOrders { get; set; }
        public DbSet<APPurchaseOrderItem> APPurchaseOrderItems { get; set; }
        public DbSet<APPurchaseReturn> APPurchaseReturns { get; set; }
        public DbSet<APPurchaseReturnItem> APPurchaseReturnItems { get; set; }
        public DbSet<APPurchaseItem> APPurchaseItems { get; set; }
        public DbSet<APPayment> APPayments { get; set; }
        public DbSet<APPaymentInvoice> APPaymentInvoices { get; set; }
        public DbSet<GRPayment> GRPayments { get; set; }
        public DbSet<GRPaymentInvoice> GRPaymentInvoices { get; set; }
        public DbSet<GRPricing> GRPricing { get; set; }
        public DbSet<InvItemCategories> InvItemCategories { get; set; }
        public DbSet<InvStoreIssue> InvStoreIssues { get; set; }
        public DbSet<InvStoreIssueItem> InvStoreIssueItems { get; set; } 

        public DbSet<YarnIssuance> YarnIssuances { get; set; }
        public DbSet<YarnIssuanceItem> YarnIssuanceItems { get; set; }
        public DbSet<InvStockTransfer> InvStockTransfers { get; set; }
        public DbSet<InvStockTransferItem> InvStockTransferItems { get; set; }
        public DbSet<InvItemAccount> InvItemAccounts { get; set; }
        public DbSet<BkgCashPurchaseSalePayment> BkgCashPurchaseSalePayments { get; set; }
        public DbSet<BkgCashPurchaseSaleReceipt> BkgCashPurchaseSaleReceipts { get; set; }
        public DbSet<GLBankCashAccount> GLBankCashAccounts { get; set; }
        public DbSet<ARQuotation> ARQuotations { get; set; }
        public DbSet<ARQuotationDetail> ARQuotationDetail { get; set; }
        public DbSet<APGRN> APGRN { get; set; }
        public DbSet<APGRNItem> APGRNItem { get; set; }
        public DbSet<APGRNExpense> APGRNExpense { get; set; }
        public DbSet<CostCenter> CostCenter { get; set; }
        public DbSet<PlanSpecification> PlanSpecifications { get; set; }
        
        public DbSet<PlanMonthlyPlanning> PlanMonthlyPlanning { get; set; }
        public DbSet<PlanMonthlyPlanningItems> PlanMonthlyPlanningItems { get; set; }
        public DbSet<CostCenterDetail> CostCenterDetails { get; set; }
        public DbSet<AppAttachment> AppAttachments { get; set; }
        public DbSet<APInsuranceInfo> APInsuranceInfo { get; set; }
        public DbSet<GLDivision> GLDivision { get; set; }
        //
        public DbSet<OnScreenTrialBalance> OnScreenTrialBalance { get; set; }
        public DbQuery<VwGLVoucher> VwGLVouchers { get; set; }
        public DbQuery<VwOnScreenTrial> VwOnScreenTrial { get; set; }
        public DbQuery<vwGlLedger> vwGlLedgers { get; set; }
        //Application Modules
        public DbSet<SYS_FORMS> SYS_FORMS { get; set; }
        public DbSet<SYS_MENU_M> SYS_MENU_M { get; set; }
        public DbSet<SYS_MENU_D> SYS_MENU_D { get; set; }
        public DbSet<Responsibilities> Sys_Responsibilities { get; set; }
        public DbSet<SysResponsibilityDetails> SysResponsibilityDetails { get; set; }
        public DbSet<Sys_ResponsibilitiesDetail> Sys_ResponsibilitiesDetail { get; set; }
        public DbSet<SysUserDepartment> SysUserDepartments { get; set; }
        public DbSet<Sys_ResponsibilityItemCategory> Sys_ResponsibilityItemCategory { get; set; }
        public DbSet<SysOrganization> SysOrganization { get; set; }
        public DbSet<SysOrgClassification> SysOrgClassification { get; set; }
        public DbSet<HRRestdayRoster> HRRestdayRoster { get; set; }
        public DbSet<HRRestdayRosterItems> HRRestdayRosterItems { get; set; }
        public DbSet<SysProfileValues> SysProfileValues { get; set; }
        public DbSet<GLSubDivision> GLSubDivision { get; set; }

        public DbSet<Sys_ORG_Profile> Sys_ORG_Profile { get; set; }
        public DbSet<Sys_ORG_Profile_Details> Sys_ORG_Profile_Details { get; set; }

        public DbSet<SysApprovalGroup> SysApprovalGroup { get; set; }
        public DbSet<SysApprovalGroupDetails> SysApprovalGroupDetails { get; set; }
        public DbSet<Sys_Rules_Approval> Sys_Rules_Approval { get; set; }
        public DbSet<Sys_Rules_Approval_Details> Sys_Rules_Approval_Details { get; set; }
        public DbSet<Sys_Vacation_Rule> Sys_Vacation_Rule { get; set; }
        public DbSet<APComparativeStatement> APComparativeStatements { get; set; }
        public DbSet<APCSRequestDetail> APCSRequestDetail { get; set; }
        public DbSet<APCSRequest> APCSRequest { get; set; }

        public DbSet<APPurchaseRequisitionDetails> APPurchaseRequisitionDetails { get; set; }
        public DbSet<APPurchaseRequisition> APPurchaseRequisition { get; set; }
        public DbSet<APIRN> APIRN { get; set; }
        public DbSet<APIRNDetails> APIRNDetails { get; set; } 
        public DbSet<APOGP> APOGP { get; set; }
        public DbSet<APOGPDetails> APOGPDetails { get; set; }
        public DbSet<APIGP> APIGP { get; set; }
        public DbSet<APIGPDetails> APIGPDetails { get; set; }
        public DbSet<ARSuplierItemsGroup> ARSuplierItemsGroup { get; set; }
        public DbSet<APLC> APLC { get; set; }
        public DbSet<APCustomInfo> APCustomInfo { get; set; }
        public DbSet<APCustomInfoDetails> APCustomInfoDetails { get; set; }
        public DbSet<ARSalePerson> ARSalePerson { get; set; }
        public DbSet<ARAnnualSaleTargets> ARAnnualSaleTargets { get; set; }
        public DbSet<ARMonthlySaleTargets> ARMonthlySaleTargets { get; set; }
        public DbSet<ARSalePersonCity> ARSalePersonCity { get; set; }
        public DbSet<ARSalePersonItemCategory> ARSalePersonItemCategory { get; set; }
        public DbSet<ItemPricings> ItemPricings { get; set; }
        public DbSet<ItemPricingDetails> ItemPricingDetails { get; set; }
        public DbSet<ARDiscountItem> ARDiscountItem { get; set; }
        public DbSet<ARCommissionAgentCustomer> ARCommissionAgentCustomer { get; set; }
        public DbSet<ARCommissionAgentItemCategory> ARCommissionAgentItemCategory { get; set; }
        public DbSet<ARCommissionAgentSalePerson> ARCommissionAgentSalePerson { get; set; }
        public DbSet<ARDeliveryChallanDiscountDetails> ARDeliveryChallanDiscountDetails { get; set; }
        public DbSet<ARDeliveryChallanComAgentPayGenDetails> ARDeliveryChallanComAgentPayGenDetails { get; set; }
        public DbSet<ARCommissionAgentPaymentGeneration> ARCommissionAgentPaymentGeneration { get; set; }
        public DbSet<ARCommissionAgentPaymentGenerationDetails> ARCommissionAgentPaymentGenerationDetails { get; set; }
        public DbSet<ARCommissionAgentPayment> ARCommissionAgentPayment { get; set; }
        public DbSet<ARCommissionAgentPaymentDetails> ARCommissionAgentPaymentDetails { get; set; }

        public DbSet<ARCommissionPayment> ARCommissionPayment { get; set; }
        public DbSet<ARCommissionPaymetDetail> ARCommissionPaymetDetail { get; set; }
        public DbSet<GRFolding> GRFolding { get; set; }
        public DbSet<GRFoldingItems> GRFoldingItems { get; set; }
        public DbSet<GRMending> GRMending { get; set; }
        public DbSet<GRStacking> GRStacking { get; set; }
        public DbSet<GRMendingDetail> GRMendingDetail { get; set; }
        public DbSet<PlnMonthly> PlnMonthlies { get; set; }
        public DbSet<PlnMonthlyItem> PlnMonthlyItems { get; set; }
        public DbSet<SeasonalPlaning> SeasonalPlaning { get; set; }
        public DbSet<SeasonalPlaningDetail> SeasonalPlaningDetail { get; set; }
        public DbSet<GreigeIssuance> GreigeIssuance { get; set; }
        public DbSet<GreigeIssuanceDetail> GreigeIssuanceDetail { get; set; }
        public DbSet<BillOfMaterialItem> BillOfMaterialItems { get; set; }




        #region HR Pay Roll
        public DbSet<HRAttendance> HRAttendances { get; set; }
        public DbSet<HRAttendanceDate> HRAttendanceDates { get; set; }
        public DbSet<HRAttendanceLog> HRAttendanceLogs { get; set; }
        public DbSet<HRAttendanceMannual> HRAttendanceMannuals { get; set; }
        public DbSet<HRDeduction> HRDeductions { get; set; }
        public DbSet<HRDeductionEmployee> HRDeductionEmployees { get; set; }
        public DbSet<HREmployee> HREmployees { get; set; }
        public DbSet<HREmployeeAllowance> HREmployeeAllowances { get; set; }
        public DbSet<HREmployeeBreakUpHistory> HREmployeeBreakUpHistorys { get; set; }
        public DbSet<HREmployeeExperience> HREmployeeExperiences { get; set; }
        public DbSet<HREmployeeFamily> HREmployeeFamilies { get; set; }
        public DbSet<HREmployeeIncrementBreakUp> HREmployeeIncrementBreakUps { get; set; }
        public DbSet<HREmployeeJobDescription> HREmployeeJobDescriptions { get; set; }
        public DbSet<HREmployeeJoinBreakUp> HREmployeeJoinBreakUps { get; set; }
        public DbSet<HREmployeeJoinResign> HREmployeeJoinResigns { get; set; }
        public DbSet<HREmployeeLeave> HREmployeeLeaves { get; set; }
        public DbSet<HREmployeeLeaveBalance> HREmployeeLeaveBalances { get; set; }
        public DbSet<HREmployeeLeaveOpening> HREmployeeLeaveOpenings { get; set; }
        public DbSet<HREmployeeQualification> HREmployeeQualifications { get; set; }
        public DbSet<HREmployeeReinstate> HREmployeeReinstates { get; set; }
        public DbSet<HREmployeeSalaryBreakUp> HREmployeeSalaryBreakUps { get; set; }
        public DbSet<HREmployeeShortLeave> HREmployeeShortLeaves { get; set; }
        public DbSet<HREmployeeStrength> HREmployeeStrengths { get; set; }
        public DbSet<HREmployeeType> HREmployeeTypes { get; set; }
        public DbSet<HREmployeeTypePeriod> HREmployeeTypePeriods { get; set; }
        public DbSet<HREncashment> HREncashments { get; set; }
        public DbSet<HREncashmentLeave> HREncashmentLeaves { get; set; }
        public DbSet<HREobiLedger> HREobiLedgers { get; set; }
        public DbSet<HRFinalSettlement> HRFinalSettlements { get; set; }
        public DbSet<HRFinalSettlementDeduction> HRFinalSettlementDeductions { get; set; }
        public DbSet<HRFinalSettlementIncentive> HRFinalSettlementIncentives { get; set; }
        public DbSet<HRFinalSettlementLeave> HRFinalSettlementLeaves { get; set; }
        public DbSet<HRFixedDeduction> HRFixedDeductions { get; set; }
        public DbSet<HRGazetedHoliday> HRGazetedHolidays { get; set; }
        public DbSet<HRIncentive> HRIncentives { get; set; }
        public DbSet<HRIncentiveEmployee> HRIncentiveEmployees { get; set; }
        public DbSet<HRIncrement> HRIncrements { get; set; }
        public DbSet<HRIncrementEmployee> HRIncrementEmployees { get; set; }
        public DbSet<HRJobDescription> HRJobDescriptions { get; set; }
        public DbSet<HRLeave> HRLeaves { get; set; }
        public DbSet<HRLeaveType> HRLeaveTypes { get; set; }
        public DbSet<HRLeaveTypeGroup> HRLeaveTypeGroups { get; set; }
        public DbSet<HRLeaveYear> HRLeaveYears { get; set; }
        public DbSet<HRLoan> HRLoans { get; set; }
        public DbSet<HRLoanSchedule> HRLoanSchedules { get; set; }
        public DbSet<HRManualAtt> HRManualAtts { get; set; }
        public DbSet<HRMedical> HRMedicals { get; set; }
        public DbSet<HRMedicalDetail> HRMedicalDetails { get; set; }
        public DbSet<HROt> HROts { get; set; }
        public DbSet<HROtReq> HROtReqs { get; set; }
        public DbSet<HROtReqEmployee> HROtReqEmployees { get; set; }
        public DbSet<HRPayroll> HRPayrolls { get; set; }
        public DbSet<HRPayrollEmployeeBreakUp> HRPayrollEmployeeBreakUps { get; set; }
        public DbSet<HRPayrollPieceRateBreakUp> HRPayrollPieceRateBreakUps { get; set; }
        public DbSet<HRPayrollVoucher> HRPayrollVouchers { get; set; }
        public DbSet<HRPfLedger> HRPfLedgers { get; set; }
        public DbSet<HRRestChange> HRRestChanges { get; set; }
        public DbSet<HRRouster> HRRousters { get; set; }
        public DbSet<HRRousterGroupSchedule> HRRousterGroupSchedules { get; set; }
        public DbSet<HRSalaryAdvance> HRSalaryAdvances { get; set; }
        public DbSet<HRSalaryAdvanceEmployee> HRSalaryAdvanceEmployees { get; set; }
        public DbSet<HRShift> HRShifts { get; set; }
        public DbSet<HRShiftChange> HRShiftChanges { get; set; }
        public DbSet<HRSocialSecurityLedger> HRSocialSecurityLedgers { get; set; }

        #region Greige

        public DbSet<GRWeavingContract> GRWeavingContracts { get; set; }

        #endregion 


        #endregion HR
        #region Aus Pak

        public object HREmployeeType { get; set; }
        #endregion Aus Pak
        #region Views
        public DbQuery<Vw_InvLedger> VwInvLedgers { get; set; }
        // public DbQuery<VwGLVoucher> VwGLVouchers { get; set; }
        public IEnumerable<object> APPurchaseOrder { get; set; }
       
        #endregion
        //public override int SaveChanges()
        //{
        //    throw new InvalidOperationException("SaveChanges method is not allowed in the context of this application.Replace it with SaveChangesAsync.");
        //}

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AppAuditTrial || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                {
                    string transactionId = "";
                    foreach (var property in entry.Properties)
                    {
                        var auditEntry = new AuditEntry(entry);
                        auditEntry.TableName = entry.Metadata.Relational().TableName;
                        if (auditEntry.TableName == "AspNetUsers")
                        {
                            auditEntry.UserId = "";
                            auditEntry.CompanyId = 0;
                            auditEntry.UserFullName = "Anonymous";
                        }
                        else
                        {
                            auditEntry.UserId = IHttpContext.HttpContext.Session.GetString("UserId");
                            auditEntry.CompanyId = IHttpContext.HttpContext.Session.GetInt32("CompanyId").Value;
                            auditEntry.UserFullName = IHttpContext.HttpContext.Session.GetString("UserName");
                        }


                        var tableName = auditEntry.TableName;
                        //Following line returns Display Attributes in a list, but we need name against property
                        var displayAttr = typeof(InvItem).GetProperties().Select(x => x.GetCustomAttribute<DisplayAttribute>()).Where(x => x != null).Select(x => x.Name).ToList();

                        Type type = Type.GetType(tableName);
                        //object myspork = Activator.CreateInstance(type);
                        ////////////////////////////////////////////////////////////////////////////////////
                        #region testing
                        //var displayName = (property.Metadata.FieldInfo.GetCustomAttribute<DisplayAttribute>()).Name;
                        #endregion testing
                        //////////////////////////////////////////////////////////////////////////////////

                        if (property.IsTemporary)
                        {
                            // value will be generated by the database, get the value after saving
                            auditEntry.TemporaryProperties.Add(property);
                            continue;
                        }
                        string propertyName = property.Metadata.Name;
                        if (property.Metadata.IsPrimaryKey())
                        {
                            auditEntry.KeyValues[propertyName] = property.CurrentValue;
                            transactionId = property.CurrentValue.ToString();
                            continue;
                        }
                        switch (entry.State)
                        {

                            case EntityState.Added:
                                if (propertyName != "CreatedBy")
                                {
                                    if (propertyName != "CreatedDate")
                                    {
                                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                                        auditEntry.ActionName = "Added";
                                        //var max = GetTable(tbl);
                                        auditEntries.Add(auditEntry);
                                        auditEntry.TransactionId = transactionId;
                                    }
                                }
                                break;
                            case EntityState.Deleted:
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.ActionName = "Deleted";
                                auditEntries.Add(auditEntry);
                                break;

                            case EntityState.Modified:
                                if (!Equals(property.OriginalValue, property.CurrentValue))
                                {
                                    if (propertyName != "UpdatedBy")
                                    {
                                        if (propertyName != "UpdatedDate")
                                        {
                                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                                            auditEntry.TransactionId = transactionId;
                                            auditEntry.ActionName = "Updated";
                                            auditEntries.Add(auditEntry);
                                        }
                                    }
                                }
                                break;
                            case EntityState.Unchanged:
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                break;
                        }
                    }
                }
            }
            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(temp => !temp.HasTemporaryProperties))
            {
                AppAuditTrials.Add(auditEntry.ToAudit());
            }
            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(temp => temp.HasTemporaryProperties).ToList();
        }
        //public int GetTable(string tableName)
        //{
        //    var sql = "SELECT MAX(Id) FROM " + tableName;
        //    var max = (from x in tableName select x.Id).max();
        //    return max;
        //}
        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;
            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                        //auditEntry.keyVal = Convert.ToString(prop.CurrentValue);
                    }

                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;

                        //auditEntry.newVals = prop.CurrentValue.ToString();
                    }
                }
                // Save the Audit entry
                AppAuditTrials.Add(auditEntry.ToAudit());
            }
            return SaveChangesAsync();
        }
    }
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public string TableName { get; set; }
        public string UserId { get; set; }
        public string Id { get; set; }
        public string UserFullName { get; set; }
        public int CompanyId { get; set; }
        public string ActionName { get; set; }
        public string SourceField { get; set; }
        public string TransactionId { get; set; }

        public Dictionary<string, object> KeyValues { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public AppAuditTrial ToAudit()
        {
            var audit = new AppAuditTrial();
            #region Where Values are being tracked and saved
            foreach (var item in NewValues)
            {
                var newVal = Convert.ToString(item.Value);
                //Below is the code to to track IP and Host Name, We may need Client IP and Host
                IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
                audit.IP = heserver.AddressList[1].ToString();
                //audit.UserId = heserver.HostName.ToString();


                string prefix = new string(TableName.Take(2).ToArray());
                if (prefix == "Bk")
                {
                    audit.Module = "Booking";
                }
                else if (prefix == "GL")
                {
                    audit.Module = "General Ledger (GL)";
                }
                else if (prefix == "In")
                {
                    audit.Module = "Inventory";
                }
                else if (prefix == "AP")
                {
                    audit.Module = "Account Payable (AP)";
                }
                else if (prefix == "AR")
                {
                    audit.Module = "Account Receivable (AR)";
                }
                else if (prefix == "As")
                {
                    audit.Module = "Identity (ASP.Net Identity)";
                }
                audit.SourceTable = TableName;
                audit.UserFullName = UserFullName;
                audit.CompanyId = CompanyId;
                //audit.UserId = UserId;
                audit.Action = ActionName;
                audit.CreatedDate = DateTime.Now;
                audit.SourceField = item.Key;

                if (OldValues.Count != 0)
                {
                    audit.TransactionId = TransactionId;
                    audit.BeforeValue = Convert.ToString(OldValues[item.Key]);
                    //audit.AfterValue = item.Value.ToString();
                    audit.AfterValue = newVal;
                }
                else
                {
                    audit.BeforeValue = null;
                }
                audit.TransactionId = TransactionId;
                audit.AfterValue = newVal;
            }
            return audit;
            #endregion


        }
    }
}





