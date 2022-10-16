using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;

namespace Numbers.Areas.GL.Controllers
{
    [Authorize]
    [Area("GL")]
    public class ApiController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        Dictionary<string, object> returnResponse = new Dictionary<string, object>();
        public ApiController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetVoucherImage(int id)
        {
            var files = _dbContext.AppAttachments.Where(c => c.Source == "Voucher Entry" && c.SourceId == id).Select(c => c.FileName).ToList();

            return Ok(files);
        }
        [HttpGet]
        public IActionResult GetCashBankAccounts(string q = "")
        {
            q = q ?? "";
            var accounts = _dbContext.GLBankCashAccounts
                            .Where(b => b.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value && b.IsActive != false)
                            .Where(b => b.AccountName.Contains(q))
                            .Select(b => new
                            {
                                id = b.Id,
                                text = string.Concat((b.AccountNumber != null ? b.AccountNumber:  "") + " - " + b.AccountName)
                            })
                            .OrderByDescending(a => a.text.ToUpper().Contains("CASH IN HAND"))
                            .Take(25)
                            .ToList();
            return Ok(accounts);
        }
        [HttpGet]
        public async Task<IActionResult> GetGLBankCashAccounts(string q = "", string type = "")
        {
            var accounts = _dbContext.GLBankCashAccounts
                            .Where(b => b.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value && b.IsActive == true).Include(x=>x.Account)
                            .Where(b => b.AccountName.Contains(q))
                            .Select(b => new
                            {
                                id = b.Id,
                                text = string.Concat( b.AccountNumber , " - " , b.AccountName)
                            })
                            .OrderBy(a => a.text)
                            .Take(25)
                            .ToListAsync();
            if (type != "")
            {
                accounts = _dbContext.GLBankCashAccounts
                            .Where(b => b.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value && b.IsActive == true && b.PaymentVoucherType == type || b.VoucherType==type)
                            .Where(b => b.AccountName.Contains(q))
                            .Select(b => new
                            {
                                id = b.Id,
                                text = string.Concat(b.AccountNumber, " - ", b.AccountName)
                            })
                            .OrderBy(a => a.text)
                            .Take(25)
                            .ToListAsync();
            }
            return Ok(await accounts);
        }
      
        [HttpGet]
        public IActionResult GetCashBank (int id)
        {
            var accounts = _dbContext.GLBankCashAccounts
                            .Where(b => b.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value && b.IsActive == true)
                            .Where(b => b.Id.Equals(id))
                            .FirstOrDefault();
            return Ok(accounts);
        }
        public IActionResult GetSubDepartment(int departmentId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var SubDepartment = new SelectList(_dbContext.GLSubDivision.Where(x => x.GLDivisionId == departmentId).ToList(), "Id", "Name");
            return Ok(SubDepartment);
        }
        public IActionResult GetYear()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Period = _dbContext.AppPeriods.Where(x => x.CompanyId == companyId && x.PayrollOpen == true).ToList();
            Period.ForEach(x =>
            {
                x.Description = x.Description.Substring(4, 4);
            });
            var Year = new SelectList(Period.Where(x => x.CompanyId == companyId).ToList(), "Description", "Description");
            return Ok(Year);
        }
        public IActionResult GetCostCenter(int departmentId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.SubDivisionId == departmentId).ToList(), "Id", "Description");
            return Ok(CostCenter);
        }
        public IActionResult GetSubAccounts(string accountCode)
        {
           
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
          //  int accountId = _dbContext.GLAccounts.Where(x => x.Code + " - " + x.Name == accountCode).FirstOrDefault().Id;
            var SubAccounts = (dynamic)null;
            GLAccount checkAccounts = _dbContext.GLAccounts.FirstOrDefault(x => x.RequireSubAccount == true && x.Code + " - " + x.Name == accountCode.ToString() && x.CompanyId == companyId);
            if (!ReferenceEquals(checkAccounts, null))
            {
                SubAccounts = (new SelectList(_dbContext.GLSubAccountDetails.Where(x => x.GLSubAccountId == int.Parse(checkAccounts.SubAccountId)).ToList(), "Id", "Description"));

            }
            return Ok(SubAccounts);

        }
        [HttpGet]
        public IActionResult GetCashBankAccount(int id)
        {
            var accounts = _dbContext.GLBankCashAccounts
                            .Where(b => b.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value && b.IsActive != false)
                            .Where(b => b.Id.Equals(id))
                            .Select(b => new
                            {
                                id = b.Id,
                                text = string.Concat(/*b.AccountName, "-",*/ b.AccountName)
                            }).FirstOrDefault();
            return Ok(accounts);
        }
        [HttpGet]
        public IActionResult GetAccounts(string q = "")
        {
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4 /*&& a.IsActive == true*/
                                                && (a.Code.Contains(q) || a.Name.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               //.Take(25)
                                               .ToList();
            if(q==null)
            {
                accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false  && a.AccountLevel == 4 /*&& a.IsActive == true*/
                                                 )
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               //.Take(25)
                                               .ToList();
            }
            return Ok(accounts);
        }

        [HttpGet]
        public IActionResult GetDepartments(string q = "")
        {
            var accounts = _dbContext.GLDivision.Where(
                                                a => a.IsDeleted == false 
                                                && (a.Description.Contains(q) || a.Name.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Name,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               //.Take(25)
                                               .ToList();
            if (q == null)
            {
                accounts = _dbContext.GLDivision.Where(
                                                a => a.IsDeleted == false 
                                                 )
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Name,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               //.Take(25)
                                               .ToList();
            }
            return Ok(accounts);
        }

        [HttpGet]

        public IActionResult GetAconts(int id)
        {
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4 && a.IsActive == true
                                                && (a.Id.Equals(id))
                                               )
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   name = a.Name
                                               })
                                               .FirstOrDefault();
            return Ok(accounts);
        }

        [HttpGet]
        public IActionResult GetAccount(int id, bool isFirst, bool isLast)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (isFirst)
            {
                var account = _dbContext.GLAccounts.Where(a => a.IsDeleted == false  && a.AccountLevel==4)
                                                 .Select(a => new
                                                 {
                                                     id = a.Id,
                                                     text = string.Concat(a.Code, " - ", a.Name),
                                                     code = a.Code,
                                                     description = a.Name
                                                 })
                                                .OrderBy(a => a.text)
                                                .FirstOrDefault();
                return Ok(account);
            }
            else if (isLast)
            {
                var account = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.AccountLevel == 4)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   description = a.Name
                                               })
                                               .OrderByDescending(a => a.text)
                                               .FirstOrDefault();
                return Ok(account);
            }
            else
            {
                var account = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.Id == id)
                                                   .Select(a => new
                                                   {
                                                       id = a.Id,
                                                       text = string.Concat(a.Code, " - ", a.Name)
                                                   })
                                                   .FirstOrDefault();
                return Ok(account);
            }

            
        }

        public IActionResult GetAccountById(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var account = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.Id == id && a.IsActive==true)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   requireCostCenter = a.RequireCostCenter,
                                                   requireSubAccount = a.RequireSubAccount,
                                               }).FirstOrDefault();
                return Ok(account);
     
        }
        [HttpGet]
        public IActionResult GetBankGLAccounts(string q = "")
        {
            var bank = _dbContext.GLBankCashAccounts.Where(x => x.IsActive == true && x.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).Select(x => x.AccountId).ToList();
            var accounts = _dbContext.GLAccounts.Where(
                                                a =>bank.Contains(a.Id) && a.IsDeleted == false  && a.AccountLevel == 4
                                                && (a.Code.Contains(q) || a.Name.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               //.Take(25)
                                               .ToList();
            return Ok(accounts);
        }
        [HttpGet]
        public IActionResult GetGLVouchersbycheque(int id,string cheque)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("CompanyId");
            if (cheque==null)
            {
                cheque = "";
            }
            var voucher = (from a in _dbContext.GLVouchers.Where(x => x.IsDeleted == false && x.Reference.Contains(cheque))
                           join b in _dbContext.GLVoucherDetails  on a.Id equals b.VoucherId
                           where b.AccountId == id
                            && b.Realization == null
                           select new
                           {
                               a.VoucherNo,
                               a.VoucherType,
                               a.VoucherDate,
                               a.Reference,b.Id,
                               b.Description,
                               b.Debit,
                               b.Credit,
                               b.Realization
                               
                           }
                           ).ToList();
            return Ok(voucher);
        }
        [HttpGet]
        public IActionResult GetGLVouchersbychequeReal(int id, string cheque)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("CompanyId");
            if (cheque == null)
            {
                cheque = "";
            }
            var voucher = (from a in _dbContext.GLVouchers.Where(x => x.IsDeleted == false && x.Reference.Contains(cheque))
                           join b in _dbContext.GLVoucherDetails on a.Id equals b.VoucherId
                           where b.AccountId == id
                            && b.Realization != null
                           select new
                           {
                               a.VoucherNo,
                               a.VoucherType,
                               a.VoucherDate,
                               a.Reference,
                               b.Id,
                               b.Description,
                               b.Debit,
                               b.Credit,
                               b.Realization

                           }
                           ).ToList();
            return Ok(voucher);
        }
        [HttpGet]
        public IActionResult GetGLVouchers(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("CompanyId");
            var voucher = (from a in _dbContext.GLVouchers.Where(x => x.IsDeleted==false)
                           join b in _dbContext.GLVoucherDetails on a.Id equals b.VoucherId
                           where b.AccountId == id
                           && b.Realization == null
                           select new
                           {
                               
                               a.VoucherNo,
                               a.VoucherType,
                               a.VoucherDate,
                               a.Reference,b.Id,
                               b.Description,
                               b.Debit,
                               b.Credit,
                               b.Realization
                           }
                           ).ToList();
            return Ok(voucher);
        }
        [HttpGet]
        public IActionResult GetGLVouchersReal(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("CompanyId");
            var voucher = (from a in _dbContext.GLVouchers.Where(x => x.IsDeleted==false)
                           join b in _dbContext.GLVoucherDetails  on a.Id equals b.VoucherId
                           where b.AccountId == id
                           && b.Realization !=null
                           select new
                           {

                               a.VoucherNo,
                               a.VoucherType,
                               a.VoucherDate,
                               a.Reference,
                               b.Id,
                               b.Description,
                               b.Debit,
                               b.Credit,
                               b.Realization
                           }
                           ).ToList();
            return Ok(voucher);
        }
        [HttpPost]
        public async Task <IActionResult>  PostVouchersRealization( List<VoucherRealizationHelper> vouchers) {

           
            foreach (var item in vouchers)
            {
                var voucherDetail = _dbContext.GLVoucherDetails.Where(x => x.Id == item.Id).FirstOrDefault();
                voucherDetail.Realization = item.Real;
                _dbContext.Update(voucherDetail);
               
            }
          await  _dbContext.SaveChangesAsync();

            return Ok("Success");
        }
        [HttpGet]
        public async Task <IActionResult> Delete(int id)
        {
            //check if account is already in use
            var duplicateCheck = _dbContext.GLVoucherDetails.Where(a => a.AccountId == id && a.IsDeleted == false).Count();
            string userId = HttpContext.Session.GetString("UserId");

            var account = _dbContext.GLAccounts.Where(a => a.Id == id).FirstOrDefault();
            account.IsDeleted = true;
            account.UpdatedBy = userId;
            account.UpdatedDate = DateTime.Now;
            var entry = _dbContext.GLAccounts.Update(account);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            returnResponse.Add("success", true);
            returnResponse.Add("message", "Account has been deleted!");
            return Ok(returnResponse);
        }

        public string GetArrayToString(string[] array)
        {
            string result = "";
            result = string.Join(",", array);
            return result;
        }
        [HttpPost]
        public async Task<IActionResult> Create(Dictionary<string, string> parameters, string[] Items,string[] Categories,
            string[] Manufacturers,string[] Customers,string[] Cities, string[] Countries, string[] Stores, string[] Suppliers,string[] SalesPerson, string[] CustomerCategory,string[] CostCenters, string[] Departments, string[] LevelWiseItems)
        {
            try
            {
                string reportName = Convert.ToString(parameters["ReportTitle"]);
                var report = _dbContext.AppReports.Where(n => n.Name == reportName).FirstOrDefault();

                AppReportQueue queue = new AppReportQueue();
                queue.Id = System.Guid.NewGuid().ToString();
                queue.ReportId = report.Id;
 
                //if (Items.Length !=0)
                //{
                //    //string Count = parameters["Items"];
                //    parameters["Items"] = GetArrayToString(Items);
                //}
                //if (Categories.Length != 0)
                //{
                //    parameters["Categories"] = GetArrayToString(Categories);
                //}
                //if (Manufacturers.Length != 0)
                //{
                //    parameters["Manufacturers"] = GetArrayToString(Manufacturers);
                //}
                //if (Customers.Length != 0)
                //{
                //    parameters["Customers"] = GetArrayToString(Customers);
                //}
                //if (Cities.Length != 0)
                //{
                //    parameters["Cities"] = GetArrayToString(Cities);
                //}

                // parameters.Add("Itemsx", result);
                queue.Parameters = JsonConvert.SerializeObject(parameters);
                queue.CreatedBy = HttpContext.Session.GetString("UserId");
                queue.CreatedDate = DateTime.Now;
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    _dbContext.AppReportQueues.Add(queue);
                    await _dbContext.SaveChangesAsync();
                    //Add Report Queue Parameters

                    //Delete old parameters 
                    // var queueParameter = _dbContext.AppReportQueueParameters.ToList().RemoveAll(x => x.QueueId == queue.Id);
                    foreach (var Saleperson in SalesPerson)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "SalesPerson";
                        queueParameters.TransactionId = Convert.ToInt32(Saleperson);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var item in Items)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Items";
                        queueParameters.TransactionId = Convert.ToInt32(item);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var category in Categories)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Categories";
                        queueParameters.TransactionId = Convert.ToInt32(category);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var manufacturer in Manufacturers)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Manufacturers";
                        queueParameters.TransactionId = Convert.ToInt32(manufacturer);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var customer in Customers)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Customers";
                        queueParameters.TransactionId = Convert.ToInt32(customer);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var Supplier in Suppliers)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Suppliers";
                        queueParameters.TransactionId = Convert.ToInt32(Supplier);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var customercategory in CustomerCategory)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "CustomerCategory";
                        queueParameters.TransactionId = Convert.ToInt32(customercategory);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    } 
                    foreach (var costCenter in CostCenters)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "CostCenter";
                        queueParameters.TransactionId = Convert.ToInt32(costCenter);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var department in Departments)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Department";
                        queueParameters.TransactionId = Convert.ToInt32(department);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }

                    foreach (var city in Cities)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Cities";
                        queueParameters.TransactionId = Convert.ToInt32(city);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var city in Countries)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Countries";
                        queueParameters.TransactionId = Convert.ToInt32(city);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    foreach (var city in Stores)
                    {
                        AppReportQueueParameters queueParameters = new AppReportQueueParameters();
                        queueParameters.TransactionType = "Stores";
                        queueParameters.TransactionId = Convert.ToInt32(city);
                        queueParameters.QueueId = queue.Id;
                        _dbContext.AppReportQueueParameters.Add(queueParameters);
                    }
                    
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                //AppConfigHelper helper = new AppConfigHelper();
                var reportPath = Helpers.CommonHelper.GetReportPath(_dbContext);
                var reportUrl = string.Concat(reportPath, "?Id=", queue.Id);
                return Ok(reportUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult GetAllAccounts(DateTime startDate, DateTime endDate, bool lev1, bool lev2, bool lev3, bool lev4)
        {
            string userId = HttpContext.Session.GetString("UserId");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //var account = new StudentRepo(_dbContext, userId).GetAccounts( );
            var account = _dbContext.OnScreenTrialBalance.FromSql("dbo.SpGLTrialBalanceBYDate @StartDate = {0}, @EndDate = {1},@CompanyId = {2}", startDate, endDate, companyId);
            return Json(account);
        }

        public IActionResult GetLedgers(DateTime startDate, DateTime endDate, string StartCode, string EndCode)
        {
            string userId = HttpContext.Session.GetString("UserId");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //var account = new StudentRepo(_dbContext, userId).GetAccounts( );
            var account = _dbContext.vwGlLedgers.FromSql("dbo.SpGLLedger @StartDate = {0}, @EndDate = {1},@StartCode = {3}, @EndCode = {3},@CompanyId = {4}", startDate, endDate, StartCode, EndCode, companyId);
            return Json(account);
        }

        public IActionResult GetVouchers(DateTime startDate, DateTime endDate, int voucherId)
        {
            string userId = HttpContext.Session.GetString("UserId");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
           List<VwOnScreenTrial> account = new List<VwOnScreenTrial>();
            try
            {
                // list< VwGLVoucher> account  = new List<VwGLVoucher>();
               
                //var account = new StudentRepo(_dbContext, userId).GetAccounts( );
                //var account =  _dbContext.VwOnScreenTrial.FromSql("exec SpGLVoucher @VoucherId={0},@CompanyId={1}", voucherId  , companyId);
                  account = _dbContext.VwOnScreenTrial.FromSql("dbo.SpGLVoucher @StartDate = {0}, @EndDate = {1},@StartCode = {2}, @EndCode = {3},@VoucherId={4},@CompanyId = {5}", startDate, endDate, "", "", voucherId, companyId).ToList();
            }catch(Exception e)
            {
                //
            }
                return Json(account);
        }
        [HttpGet]
        public IActionResult GetFourthAccount(string q)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //if (q == "")
            //{
            //    return Ok(new
            //    {
            //        message = "Validation Required",
            //        error = "Missing query {q}"

            //    });
            //}
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4
                                                && (a.Code.Contains(q) || a.Name.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(accounts);
        }
        [HttpGet]
        public IActionResult CalculateTax(int taxId, decimal amount)
        {
            AppTax appTax = _dbContext.AppTaxes.FirstOrDefault(x => x.Id == taxId);
            ListOfValue values = new ListOfValue();
            values.SalesTaxAmount = Convert.ToInt32((Convert.ToInt32(appTax.SalesTaxPercentage) * amount) / 100);
            values.ExciseTaxAmount = Convert.ToInt32((Convert.ToInt32(appTax.ExciseTaxPercentage) * amount) / 100);
            values.IncomeTaxAmount = Convert.ToInt32((Convert.ToInt32(appTax.IncomeTaxPercentage) * amount) / 100);
            values.AmountWithTax = values.SalesTaxAmount + Convert.ToInt32(amount);

            values.dSalesTaxAmount = (Convert.ToInt32(appTax.SalesTaxPercentage) * amount) / 100;
            values.dExciseTaxAmount = (Convert.ToInt32(appTax.ExciseTaxPercentage) * amount) / 100;
            values.dIncomeTaxAmount = (Convert.ToInt32(appTax.IncomeTaxPercentage) * amount) / 100;
            values.dAmountWithTax = values.SalesTaxAmount + amount;

            values.AmountWithoutTax = Convert.ToInt32(amount);
            return Ok(values);
        }
    }
}