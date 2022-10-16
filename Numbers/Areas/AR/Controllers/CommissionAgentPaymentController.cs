using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class CommissionAgentPaymentController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public CommissionAgentPaymentController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Commission Agent Payment";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
             .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
             .Select(c => c.ConfigValue)
             .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            return View();
        }
        public IActionResult Create(int id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create Commission Agent Payment";
            VMARCommissionPayment CommissionPayment = new VMARCommissionPayment();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.ItemCategory2 = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                   select new
                                                   {
                                                       Id = ac.Id,
                                                       Name = ac.Code + " - " + ac.Name
                                                   }, "Id", "Name");
            ViewBag.City = new SelectList(from ac in _dbContext.AppCities.Where(x=>x.CountryId == 1).ToList()
                                                   select new
                                                   {
                                                       Id = ac.Id,
                                                       Name = ac.Name 
                                                   }, "Id", "Name");
            ViewBag.Agent = new SelectList(from ac in _dbContext.ARCommissionAgents.Where(x => x.IsDeleted == false  && x.CompanyId == companyId).OrderBy(x => x.Id).ToList()
                                          select new
                                          {
                                              Id = ac.Id,
                                              Name =  ac.Name
                                          }, "Id", "Name");


            
            if (id != 0)
            {



                var commissionAgentPayments = (from d in _dbContext.ARCommissionPaymetDetail
                                               join m in _dbContext.ARCommissionPayment on d.CommissionPaymentId equals m.Id
                                               join cus in _dbContext.ARCustomers on d.CustomerId equals cus.Id
                                               join Agent in _dbContext.ARCommissionAgents on m.AgentId equals Agent.Id
                                               join city in _dbContext.AppCities on m.CityId equals city.Id
                                               join cat in _dbContext.InvItemCategories on m.CategoryId equals cat.Id
                                               where m.Id == id
                                               select new
                                               {
                                                   m.Id,
                                                   m.TransactionNo,
                                                   m.TransactionDate,
                                                   m.StartDate,
                                                   m.EndDate,
                                                   m.CategoryId,
                                                   m.CityId,
                                                   m.AgentId,
                                                   DetailId = d.Id,
                                                   d.ReceiptId,
                                                   d.ReceiptDate,
                                                   d.CustomerId,
                                                   Customer=cus.Name,
                                                    Category=cat.Name,
                                                    Agent=Agent.Name,
                                                    City=city.Name,
                                                   d.ReceiptAmount,
                                                   d.CommissionAmount,
                                                   d.PayAmount,
                                                   d.CommissionPaymentId
                                               }).ToList();
                foreach (var grp in commissionAgentPayments)
                {
                    CommissionPayment.Id = grp.Id;

                    CommissionPayment.CommissionPaymentId = grp.CommissionPaymentId;
                    CommissionPayment.TransactionNo = grp.TransactionNo;
                    CommissionPayment.TransactionDate = grp.TransactionDate;
                    CommissionPayment.StartDate = grp.StartDate;
                    CommissionPayment.EndDate = grp.EndDate;
                    CommissionPayment.CategoryId = grp.CategoryId;
                    CommissionPayment.CityId = grp.CityId;
                    CommissionPayment.AgentId = grp.AgentId;
                    CommissionPayment.DetailId = grp.DetailId;
                    CommissionPayment.ReceiptId = grp.ReceiptId;
                    CommissionPayment.ReceiptDate = grp.ReceiptDate;
                    CommissionPayment.CustomerId = grp.CustomerId;
                    CommissionPayment.Agent = grp.Agent;
                    CommissionPayment.Category = grp.Category;
                    CommissionPayment.City = grp.City;
                    CommissionPayment.Customer = grp.Customer;
                    CommissionPayment.ReceiptAmount = grp.ReceiptAmount;
                    CommissionPayment.CommissionAmount = grp.CommissionAmount;
                    CommissionPayment.PayAmount = grp.PayAmount;

                }

                CommissionPayment.DetailList = (commissionAgentPayments.Select(p => new VMCommissionPaymentDetail
                {
                    Id = p.DetailId,
                    CommissionPaymentId=p.CommissionPaymentId,
                    ReceiptId = p.ReceiptId,
                    ReceiptDate = p.ReceiptDate,
                    CustomerId = p.CustomerId,
                    CustomerName=p.Customer,
                    CategoryId = p.CategoryId,
                    CategoryName=p.Category,
                    CityName=p.City,
                    Cityid = p.CityId,
                    ReceiptAmount = p.ReceiptAmount,
                    CommissionAmount = p.CommissionAmount,
                    PaymentAmount = p.PayAmount
                })).ToList();


                //  VMARCommissionPayment CommissionPayment = new VMARCommissionPayment();
                //CommissionPayment.ARCommissionPayment = _dbContext.ARCommissionPayment.Where(x => x.Id == id).FirstOrDefault();
                //CommissionPayment.ARCommissionPaymetDetail = _dbContext.ARCommissionPaymetDetail.Include(x=>x.Customer).Include(x=>x.Category).Where(x => x.CommissionPaymentId == id).ToList();


            }
            else
            {
                
            }

            return View(CommissionPayment);
        }
        //============================================================Shahid
        public IActionResult GetReceiptData(string StartDate, string EndDate, string CategoryId, string CityId, string AgentId )
        {
            DateTime StartDat = Convert.ToDateTime(StartDate);
            DateTime EndDat = Convert.ToDateTime(EndDate);
            int CategoryI = Convert.ToInt32(CategoryId);
            int CityI = Convert.ToInt32(CityId);
            int AgentI = Convert.ToInt32(AgentId);

            var GetReceiptData = (from m in _dbContext.ARReceipts
                                  join d in _dbContext.ARReceiptInvoices on m.Id equals d.ReceiptId
                                  join customer in _dbContext.ARCustomers on m.CustomerId equals customer.Id
                                  join city in _dbContext.AppCities on customer.CityId equals city.Id
                                  join Agentcategory in _dbContext.ARCommissionAgentCustomer on customer.Id equals Agentcategory.Customer_Id
                                  join Agent in _dbContext.ARCommissionAgents on Agentcategory.CommissionAgent_Id equals Agent.Id
                                  join category in _dbContext.InvItemCategories on m.ItemCategoryId equals category.Id
                                  
                                  where m.ReceiptDate>=StartDat && m.ReceiptDate<=EndDat && m.ItemCategoryId==CategoryI && Agent.CityId==CityI && Agent.Id==AgentI

                                  select new
                                  {
                                      m.Id,
                                      ReceiptDate=m.ReceiptDate.ToString("dd-MMM-yyyy"),
                                      m.CustomerId,
                                      CustomerName= customer.Name,
                                      CategoryId= category.Id,
                                      CategoryName= category.Name,
                                      CityId = city.Id, 
                                      CityName = city.Name,
                                      d.ReceiptAmount,
                                      CommissionAmount= (d.ReceiptAmount*Convert.ToDecimal(0.5))/100

                                  });
           
            
            var data = GetReceiptData.ToList();
          
            return Ok(data);
        }

            [HttpPost]
        public IActionResult Create(VMARCommissionPayment model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int respId = HttpContext.Session.GetInt32("Resp_ID").Value;
            var configValues = new ConfigValues(_dbContext);
           
            var ComAgPayModel = new ARCommissionPayment();

            if (model.Id == 0)
            {
                try
                {
                    ComAgPayModel.CreatedBy = userId;
                    ComAgPayModel.CreatedDate = DateTime.UtcNow;
                    ComAgPayModel.CompanyId = companyId;                   
                    ComAgPayModel.IsDeleted = false;
                    ComAgPayModel.Resp_Id = respId;                    
                    ComAgPayModel.IsApproved = false;
                    ComAgPayModel.Status = "Created";
                    
                    ComAgPayModel.TransactionNo = this.Max();
                    ComAgPayModel.TransactionDate = model.TransactionDate;
                    ComAgPayModel.StartDate = model.StartDate;
                    ComAgPayModel.EndDate = model.EndDate;
                    ComAgPayModel.CategoryId = model.CategoryId;
                    ComAgPayModel.CityId = model.CityId;
                    ComAgPayModel.AgentId = model.AgentId;
                    _dbContext.ARCommissionPayment.Add(ComAgPayModel);
                    _dbContext.SaveChanges();
                    var ComAgentPaydetailsList = new List<ARCommissionPaymetDetail>();

                    var commissionAgentPayments = (from d in _dbContext.ARCommissionPayment.Where(x => x.TransactionNo== ComAgPayModel.TransactionNo) 

                                                   select new
                                                   {
                                                       d.Id,
                                                   }).ToList();
                    for (int i = 0; i < collection["ReceiptId"].Count; i++)
                    {

                        var ComAgentPaydetails = new ARCommissionPaymetDetail();
                        ComAgentPaydetails.IsDeleted = false;
                        ComAgentPaydetails.CommissionPaymentId = this.MaxId();
                        ComAgentPaydetails.ReceiptId = Convert.ToInt32(collection["ReceiptId"][i]);
                        ComAgentPaydetails.ReceiptDate = Convert.ToDateTime(collection["receiptDate"][i]);
                        ComAgentPaydetails.CustomerId = Convert.ToInt32(collection["customerId"][i]);
                        ComAgentPaydetails.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                        ComAgentPaydetails.Cityid = Convert.ToInt32(collection["cityid"][i]);
                        ComAgentPaydetails.ReceiptAmount = Convert.ToInt32(collection["receiptAmount"][i]);
                        ComAgentPaydetails.CommissionAmount = Convert.ToInt32(collection["commissionAmount"][i]);
                        ComAgentPaydetails.PayAmount = Convert.ToInt32(collection["paymentAmount"][i]);

                        _dbContext.ARCommissionPaymetDetail.Add(ComAgentPaydetails);
                        _dbContext.SaveChanges();
                    };
                    
                    TempData["error"] = "false";
                    TempData["message"] = "Commission Agent "+ ComAgPayModel.TransactionNo + " Payment has been created successfully.";
                }
                catch (Exception ex)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "There is some problem while saving data";
                }
            }
            else
            {
                try
                {
                    ComAgPayModel.Updatedby = userId;
                    ComAgPayModel.UpdatedDate = DateTime.UtcNow;
                    ComAgPayModel.CompanyId = companyId;
                    ComAgPayModel.IsDeleted = false;
                    ComAgPayModel.Resp_Id = respId;
                    ComAgPayModel.IsApproved = false;

                    ComAgPayModel.Id = model.Id;
                    ComAgPayModel.TransactionNo = model.TransactionNo;
                    ComAgPayModel.TransactionDate = model.TransactionDate;
                    ComAgPayModel.StartDate = model.StartDate;
                    ComAgPayModel.EndDate = model.EndDate;
                    ComAgPayModel.CategoryId = model.CategoryId;
                    ComAgPayModel.CityId = model.CityId;
                    ComAgPayModel.AgentId = model.AgentId;
                    _dbContext.ARCommissionPayment.Update(ComAgPayModel);
                    _dbContext.SaveChanges();
                    var ComAgentPaydetailsList = new List<ARCommissionPaymetDetail>();

                    
                    for (int i = 0; i < collection["ReceiptId1"].Count; i++)
                    {

                        var ComAgentPaydetails = new ARCommissionPaymetDetail();

                       
                       ComAgentPaydetails.Id = Convert.ToInt32(collection["Id1"][i]);
                        ComAgentPaydetails.IsDeleted = false;
                        ComAgentPaydetails.CommissionPaymentId = Convert.ToInt32(collection["CommisionPaymentId1"][i]);
                        ComAgentPaydetails.ReceiptId = Convert.ToInt32(collection["ReceiptId1"][i]);
                        ComAgentPaydetails.ReceiptDate = Convert.ToDateTime(collection["receiptDate"][i]);
                        ComAgentPaydetails.CustomerId = Convert.ToInt32(collection["customerId1"][i]);
                        ComAgentPaydetails.CategoryId = Convert.ToInt32(collection["categoryId1"][i]);
                        ComAgentPaydetails.Cityid = Convert.ToInt32(collection["cityid1"][i]);
                        ComAgentPaydetails.ReceiptAmount = Convert.ToDecimal(collection["receiptAmount1"][i]);
                        ComAgentPaydetails.CommissionAmount = Convert.ToDecimal(collection["commissionAmount1"][i]);
                        ComAgentPaydetails.PayAmount = Convert.ToDecimal(collection["paymentAmount1"][i]);

                        _dbContext.ARCommissionPaymetDetail.Update(ComAgentPaydetails);
                        _dbContext.SaveChanges();
                    };

                    TempData["error"] = "false";
                    TempData["message"] = "Commission Agent " + ComAgPayModel.TransactionNo + " Payment has been Updated successfully.";
                }
                catch (Exception ex)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "There is some problem while saving data";
                }
            }
            return RedirectToAction("Create", "CommissionAgentPayment");
        }
      
        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                 int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;
                
                var configValues = new ConfigValues(_dbContext);
                //var TransactionTypeList = configValues.GetConfigValues("AR", "Transaction Type", companyId);
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchDate = Request.Form["columns[1][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var commissionAgentPayments = (from d in _dbContext.ARCommissionPaymetDetail
                                               join Agent in _dbContext.ARCommissionAgentCustomer on d.CustomerId equals Agent.Customer_Id
                                               join customer in _dbContext.ARCustomers on Agent.Customer_Id equals customer.Id
                                               join city in _dbContext.AppCities on customer.CityId equals city.Id
                                               join category in _dbContext.InvItemCategories on d.CategoryId equals category.Id
                                               join m in _dbContext.ARCommissionPayment.Where(x=>x.CompanyId == companyId) on d.CommissionPaymentId equals m.Id
                                               where m.IsDeleted==false
                                               select new
                                               {
                                                   m.Id,
                                                   m.TransactionNo,
                                                   m.IsApproved,
                                                   TransactionDate = m.TransactionDate.ToString("dd-MMM-yyyy"),
                                                   m.VoucherId,
                                                   CustomerName = customer.Name,
                                                   
                                                   CategoryName = category.Name,
                                                   CityName = city.Name,
                                                   d.PayAmount,
                                                   d.CommissionAmount

                                               }); 
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    commissionAgentPayments = commissionAgentPayments.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                commissionAgentPayments = !string.IsNullOrEmpty(searchTransNo) ? commissionAgentPayments.Where(m => m.TransactionNo.ToString().Contains(searchTransNo)) : commissionAgentPayments;
                commissionAgentPayments = !string.IsNullOrEmpty(searchDate) ? commissionAgentPayments.Where(m => m.TransactionDate.ToUpper().Contains(searchDate.ToUpper())) : commissionAgentPayments;

                recordsTotal = commissionAgentPayments.Count();
                var data = commissionAgentPayments.Select(p => new
                {
                    id = p.Id,
                    transactionNo = p.TransactionNo,
                    transactionDate = p.TransactionDate,
                    customerName = p.CustomerName,
                    categoryName = p.CategoryName,
                    cityName = p.CityName,
                    payAmount = p.PayAmount,
                    commissionAmount = p.CommissionAmount,
                    Approve = approve,
                    Unapprove = unApprove,
                    IsApproved = p.IsApproved,
                    VoucherId = p.VoucherId
                }).ToList();
                if (pageSize == -1)
                {
                    data = data.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = data.Skip(skip).Take(pageSize).ToList();
                }

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Delete(int id)
        {


            var comAgentPay = _dbContext.ARCommissionPaymetDetail.Where(x => x.CommissionPaymentId == id).FirstOrDefault();
            
                comAgentPay.IsDeleted = true;
                _dbContext.ARCommissionPaymetDetail.Update(comAgentPay);
                await _dbContext.SaveChangesAsync();

            var comAgentPayment = _dbContext.ARCommissionPayment.Where(x => x.Id == id).FirstOrDefault();
            comAgentPayment.IsDeleted = true;
            _dbContext.ARCommissionPayment.Update(comAgentPayment);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            List<ARCommissionPaymetDetail> commission = _dbContext.ARCommissionPaymetDetail.Include(x=>x.CommissionPayment).ThenInclude(x=>x.Agent)
             .Where(a => a.CommissionPayment.Status == "Created" && a.CommissionPayment.CompanyId == _companyId && a.CommissionPayment.Id == id && a.CommissionPayment.IsDeleted == false)
             .ToList();
            try
            {
                //Create Voucher
                var accounts = _dbContext.GLAccounts.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                Helpers.VoucherHelper voucher = new Helpers.VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Commission Agent Payment # : {0} of {1} ",
                commission.FirstOrDefault().CommissionPayment.TransactionNo,
                commission.FirstOrDefault().CommissionPayment.Agent.Name
                );

                int voucherId;
                voucherMaster.VoucherType = "CAP";
                voucherMaster.VoucherDate = DateTime.Now;
                //  voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency = "PKR";
                // voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                voucherMaster.Description = voucherDescription;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/CommissionAgentPaymentGeneration";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = commission.Sum(x=>x.CommissionAmount + x.PayAmount);

                //Voucher Details
                //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                //var amount = invoiceItems.Sum(s => s.LineTotal);
                //var discount = invoiceItems.Sum(s => s.DiscountAmount);

                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = accounts.Where(x => x.Name == "COMMISSION EXPENSE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetail.Sequence = 1;
                voucherDetail.Description = voucherDescription;
                voucherDetail.Debit = commission.Sum(x => x.CommissionAmount);
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                //Credit Entry
                GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                voucherDetailItem.AccountId = accounts.Where(x => x.Name == "LOCAL COMMISSION PAYABLE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetailItem.Sequence = 2;
                voucherDetailItem.Description = voucherDescription;
                voucherDetailItem.Debit = 0;
                voucherDetailItem.Credit = commission.Sum(x => x.CommissionAmount);
                voucherDetailItem.IsDeleted = false;
                voucherDetailItem.CreatedBy = _userId;
                voucherDetailItem.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetailItem);

                //Debit Entry
                GLVoucherDetail voucherDetail2 = new GLVoucherDetail();
                voucherDetail2.AccountId = accounts.Where(x => x.Name == "LOCAL COMMISSION PAYABLE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetail2.Sequence = 3;
                voucherDetail2.Description = voucherDescription;
                voucherDetail2.Debit = commission.Sum(x => x.PayAmount);
                voucherDetail2.Credit = 0;
                voucherDetail2.IsDeleted = false;
                voucherDetail2.CreatedBy = _userId;
                voucherDetail2.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail2);

                //Credit Entry
                GLVoucherDetail voucherDetailItem2 = new GLVoucherDetail();
                voucherDetailItem2.AccountId = accounts.Where(x => x.Name == "Cash in Hand in Factory" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetailItem2.Sequence = 4;
                voucherDetailItem2.Description = voucherDescription;
                voucherDetailItem2.Debit = 0;
                voucherDetailItem2.Credit = commission.Sum(x => x.PayAmount);
                voucherDetailItem2.IsDeleted = false;
                voucherDetailItem2.CreatedBy = _userId;
                voucherDetailItem2.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetailItem2);

                //#endregion Sale Account
                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    commission.FirstOrDefault().CommissionPayment.VoucherId = voucherId;
                    commission.FirstOrDefault().CommissionPayment.Status = "Approved";
                    commission.FirstOrDefault().CommissionPayment.ApprovedBy = _userId;
                    commission.FirstOrDefault().CommissionPayment.ApprovedDate = DateTime.Now;
                    commission.FirstOrDefault().CommissionPayment.IsApproved = true;
                    //On approval updating Invoice

                    var entry = _dbContext.Update(commission.FirstOrDefault().CommissionPayment);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    //var Invoice = _dbContext.ARInvoiceDiscount.Where(x => x.ARDiscountAdjustmentId == id).AsQueryable();
                    //foreach (var x in Invoice)
                    //{
                    //    var j = _dbContext.ARInvoices.FirstOrDefault(y => y.Id == x.InvoiceId);
                    //    j.IsDiscount = true;
                    //    _dbContext.ARInvoices.Update(j);
                    //}
                    await _dbContext.SaveChangesAsync();
                    TempData["error"] = "false";
                    TempData["message"] = "Commission Agent Payment has been approved successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.InnerException.Message == null ? ex.Message.ToString() : ex.InnerException.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var discount = _dbContext.ARCommissionPayment
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.IsApproved && v.CompanyId == companyId).FirstOrDefault();
             
            var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == discount.VoucherId).ToList();
            foreach (var item in voucherDetail)
            {
                var tracker = _dbContext.GLVoucherDetails.Remove(item);
                tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
            }
            discount.Status = "Created";
            discount.UnApprovedBy = userId;
            discount.IsApproved = false;
            var entry = _dbContext.ARCommissionPayment.Update(discount);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            
            await _dbContext.SaveChangesAsync();
            TempData["error"] = "false";
            TempData["message"] = string.Format("Invoice Id. {0} has been Un-Approved successfully", discount.TransactionNo);
            //}
            return RedirectToAction("Index", "CommissionAgentPayment");
        }
        public IActionResult Details(int id)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var commissionAgentPayment = _dbContext.ARCommissionAgentPayment.Include(p => p.CommissionAgentPaymentDetails).ThenInclude(p => p.CommissionAgentPaymentGeneration).ThenInclude(p => p.ARCommissionAgent).ToList().FirstOrDefault(x => x.Id == id);

            ViewBag.TransType = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == commissionAgentPayment.TransactionType).ConfigValue;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=PurchaseOrder&cId=", companyId, "&id={0}");
            //var discount = _dbContext.ARCustomerAdjustmentItem.Include(p => p.Customer_).Where(x => x.DiscountAdjustment_Id == id).ToList();
            //var discountItems = _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x =>x.Id == id);
            ViewBag.NavbarHeading = "Commission Agent Payment";
            ViewBag.TitleStatus = "Approved";

            return View(commissionAgentPayment);
        }
        public int Max()
        {
            var max = 0;
            var commissionAgentPayment = _dbContext.ARCommissionPayment.ToList();
            if (commissionAgentPayment.Count() > 0)
            {
                max = commissionAgentPayment.Max(p => p.TransactionNo) + 1;
            }
            else
            {
                max = 1;
            }
            return max;
        }

        public int MaxId()
        {
            var max = 0;
            var commissionAgentPayment = _dbContext.ARCommissionPayment.ToList();
            if (commissionAgentPayment.Count() > 0)
            {
                max = commissionAgentPayment.Max(p => p.Id);
            }
            else
            {
                max = 1;
            }
            return max;
        }
        public IActionResult GetCommissionAgent(int catogoryId, int cityId) {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
           var data = _dbContext.ARCommissionAgentCustomer.Include(p => p.Customer).ThenInclude(p => p.SalesPerson).Select(p => new AgentCustomerDTO()
            {
                CommissionAgent_Id = p.CommissionAgent_Id,
                CustomerId = p.Customer_Id,
                CustomerName = p.Customer.Name,
                SalesPerson = p.Customer.SalesPerson.Name,
                City = p.Customer.City.Name,
                CityId = p.Customer.City.Id,
                ItemCategoryNames = _dbContext.InvItemCategories.Where(x => x.Id == (from c in _dbContext.ARSuplierItemsGroup where c.ARCustomerId == p.Customer_Id select c).FirstOrDefault().CategoryId).Select(p => p.Name).ToList(),
                ItemCategoryId = _dbContext.InvItemCategories.Where(x => x.Id == (from c in _dbContext.ARSuplierItemsGroup where c.ARCustomerId == p.Customer_Id select c).FirstOrDefault().CategoryId).Select(p => p.Id).FirstOrDefault()
            });
            var commissionAgent = from a in _dbContext.ARCommissionAgents.Where(x=>x.IsDeleted != true && x.CompanyId == companyId)
                                  join b in data.Where(x => x.ItemCategoryId == catogoryId && x.CityId == cityId)
                                  on a.Id equals b.CommissionAgent_Id select new
                                  {
                                      a.Id,
                                      a.Name
                                  };
            var d = commissionAgent.GroupBy(x => x.Id).Select(x=> new ListOfValue{ 
                Id = x.Select(a=>a.Id).FirstOrDefault(),
                Name = x.Select(a=>a.Name).FirstOrDefault()
            });
            return Ok(d);
        }
    }
}