using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Helpers;
using Newtonsoft.Json;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class CustomerDiscountController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly ARDiscountRepository _ARDiscountRepository;
        private readonly ARDiscountItemRepository _ARDiscountItemRepository;
        public CustomerDiscountController(NumbersDbContext context, ARDiscountRepository ARDiscountRepository, ARDiscountItemRepository ARDiscountItemRepository)
        {
            _dbContext = context;
            _ARDiscountRepository = ARDiscountRepository;
            _ARDiscountItemRepository = ARDiscountItemRepository;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Customer Discounts";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
              .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
              .Select(c => c.ConfigValue)
              .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            return View();
        }
        public IActionResult GetDiscount()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchItemCat = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchProduct = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchStartDate = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchEndDate = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var DiscountData = (from tempcustomer in _dbContext.ARDiscount.Where(x => x.IsDeleted != true && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    DiscountData = DiscountData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                DiscountData = !string.IsNullOrEmpty(searchTransNo) ? DiscountData.Where(m => m.TransactionNo.ToString().Contains(searchTransNo)) : DiscountData;
                DiscountData = !string.IsNullOrEmpty(searchCustomer) ? DiscountData.Where(m => _dbContext.ARCustomers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.Customer_Id)).Name.ToUpper().Contains(searchCustomer.ToUpper())) : DiscountData;
                DiscountData = !string.IsNullOrEmpty(searchItemCat) ? DiscountData.Where(m => _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == Convert.ToInt32(m.ItemCategory_Id)).Name.ToUpper().Contains(searchItemCat.ToUpper())) : DiscountData;
                DiscountData = !string.IsNullOrEmpty(searchProduct) ? DiscountData.Where(m => _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(m.ProductType_Id)).ConfigValue.ToUpper().Contains(searchProduct.ToUpper())) : DiscountData;
                DiscountData = !string.IsNullOrEmpty(searchStartDate) ? DiscountData.Where(m => m.StartDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchStartDate.ToUpper())) : DiscountData;
                DiscountData = !string.IsNullOrEmpty(searchEndDate) ? DiscountData.Where(m => m.EndDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchEndDate.ToUpper())) : DiscountData;

                recordsTotal = DiscountData.Count();
                var data = DiscountData.ToList();
                if (pageSize == -1)
                {
                    data = DiscountData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = DiscountData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARDiscountViewModel> Details = new List<ARDiscountViewModel>();
                foreach (var grp in data)
                {
                    ARDiscountViewModel aRDiscount = new ARDiscountViewModel();
                    aRDiscount.EndDate = grp.EndDate.ToString("dd-MMM-yyyy");
                    aRDiscount.StartDate = grp.StartDate.ToString("dd-MMM-yyyy");
                    aRDiscount.CategoryName = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategory_Id).Name;
                    aRDiscount.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == grp.Customer_Id).Name;
                    aRDiscount.ProductName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == grp.ProductType_Id).ConfigValue;
                    aRDiscount.Trans = grp.TransactionNo;
                    aRDiscount.ARDiscount = grp;
                    aRDiscount.ARDiscount.Approve = approve;
                    aRDiscount.ARDiscount.Unapprove = unApprove;
                    Details.Add(aRDiscount);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult Create(int? id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create Customer Discount";
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string UserId = HttpContext.Session.GetString("UserId");
            ViewBag.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(1));
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted != true && x.IsActive != false && x.CategoryLevel == 2 && x.CompanyId == 1 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                          select new
                                                          {
                                                              Id = ac.Id,
                                                              Name = ac.Code + " - " + ac.Name
                                                          }, "Id", "Name");
            ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(x => x.CountryId == 1).ToList(), "Id", "Name");
            ARDiscountViewModel aRDiscount = new ARDiscountViewModel();
            if (id == null)
            {
                //var result = _ARDiscountRepository.Get(x => x.IsActive == true).ToList();
                //if (result.Count > 0)
                //{
                //    ViewBag.Id = _ARDiscountRepository.Get().Select(x => x.TransactionNo).Max() + 1;
                //}
                //else
                //{
                //    ViewBag.Id = 1;
                //}
            }
            else
            {
                aRDiscount.Status = "Updated";
                aRDiscount.ARDiscount = _ARDiscountRepository.Get(x => x.Id == id).Include(p => p.DiscountItems).FirstOrDefault();
                ViewBag.SalePerson = _dbContext.ARSalePerson.FirstOrDefault(x => x.ID == aRDiscount.ARDiscount.SalesPerson_Id).Name;

                ARCustomer customer = _dbContext.ARCustomers.Where(x => x.Id == aRDiscount.ARDiscount.Customer_Id).FirstOrDefault();
                CustomerDiscountViewModel customerDiscountViewModel = new CustomerDiscountViewModel();

                ARSalePerson salePerson = _dbContext.ARSalePerson.FirstOrDefault(x => x.ID == customer.SalesPersonId && x.CompanyId == companyId);
                ARCommissionAgentCustomer commissionAgentCustomer = _dbContext.ARCommissionAgentCustomer.FirstOrDefault(x => x.Customer_Id == customer.Id);
                var commissionAgent = _dbContext.ARCommissionAgents.FirstOrDefault(x => x.Id == commissionAgentCustomer.CommissionAgent_Id && x.CompanyId == companyId).Name;

                ViewBag.CommisionAgent = commissionAgent;
                aRDiscount.ARDiscountItem = new List<ARDiscountItem>();
                aRDiscount.Category = new List<string>();
                foreach (var grp in aRDiscount.ARDiscount.DiscountItems)
                {
                    string Category = null;
                    Category = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategory).Name;
                    aRDiscount.Category.Add(Category);
                }
                ViewBag.Customers = new SelectList(_dbContext.ARCustomers.Where(x => x.IsActive != false && x.IsDeleted != true /*&& x.CompanyId == companyId*/), "Id", "Name");
                ViewBag.SalesPersons = new SelectList(_dbContext.ARSalePerson.Where(x => x.IsActive == true && x.CompanyId == companyId), "ID", "Name");
            }
            return View(aRDiscount);
        }
        public async Task<IActionResult> Submit(ARDiscountViewModel aRDiscountViewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var DiscountData = _ARDiscountRepository.Get().Include(p => p.DiscountItems).ThenInclude(p => p.DeliveryChallanDiscountDetails);
            try
            {
                aRDiscountViewModel.ARDiscount.CityId = Convert.ToInt32(collection["CityId"]);
                if (aRDiscountViewModel.ARDiscount.Id == 0)
                {
                    aRDiscountViewModel.ARDiscount.CompanyId = companyId;
                    aRDiscountViewModel.ARDiscount.CreatedBy = userId;
                    aRDiscountViewModel.ARDiscount.CreatedDate = DateTime.Now;
                    aRDiscountViewModel.ARDiscount.IsActive = true;
                    aRDiscountViewModel.ARDiscount.Status = "Created";
                    aRDiscountViewModel.ARDiscount.Resp_Id = resp_Id;
                    aRDiscountViewModel.ARDiscount.TransactionNo = DiscountData.Max(p => p.TransactionNo) + 1;
                   
                    await _ARDiscountRepository.CreateAsync(aRDiscountViewModel.ARDiscount);

                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        ARDiscountItem aRDiscountItem = new ARDiscountItem();
                        aRDiscountItem.ARDiscount_Id = aRDiscountViewModel.ARDiscount.Id;
                        aRDiscountItem.DiscountPercentage = Convert.ToDecimal(collection["Percentage"][i]);
                        aRDiscountItem.DiscountAmount = Convert.ToDecimal(collection["DisAmt"][i]);
                        aRDiscountItem.Amount = Convert.ToDecimal(collection["Amount"][i]);
                        aRDiscountItem.ItemCategory = Convert.ToInt32(collection["CategoryId"][i]);
                        aRDiscountItem.Quantity = Convert.ToDecimal(collection["Qty"][i]);
                        var result = _dbContext.ARDiscountItem.Add(aRDiscountItem);
                        await _dbContext.SaveChangesAsync();
                        var Ids = collection["DcIds"][i].Split(',');
                        for (int z = 0; z < Ids.Count(); z++)
                        {
                            var data = new ARDeliveryChallanDiscountDetails
                            {
                                ARDiscountItemId = result.Entity.Id,
                                DeliveryChallanItemId = Convert.ToInt32(Ids[z])
                            };
                            _dbContext.ARDeliveryChallanDiscountDetails.Add(data);
                            _dbContext.SaveChanges();
                        }
                    }

                    TempData["error"] = "false";
                    TempData["message"] = "Discount has been saved successfully.";
                }
                else
                {
                    ARDiscount aRDiscount = _dbContext.ARDiscount.FirstOrDefault(x => x.Id == aRDiscountViewModel.ARDiscount.Id);
                    aRDiscount.UpdatedBy = userId;
                    aRDiscount.UpdatedDate = DateTime.Now;
                    aRDiscount.IsActive = true;
                    aRDiscount.Status = "Created";
                    aRDiscount.DiscountPercent = aRDiscountViewModel.ARDiscount.DiscountPercent;
                    aRDiscount.GrandTotal = aRDiscountViewModel.ARDiscount.GrandTotal;
                    await _ARDiscountRepository.UpdateAsync(aRDiscount);
                    var UpdateList = new List<ARDiscountItem>();
                    var rownber = collection["ChildId"].Count;
                    List<int> ChildList = new List<int>();
                    for (int i = 0; i < rownber; i++)
                    {
                        int id = Convert.ToInt32(collection["ChildId"][i]);
                        ChildList.Add(id);
                    }
                    var foundDetail = _ARDiscountItemRepository.Get(a => a.ARDiscount_Id == aRDiscountViewModel.ARDiscount.Id).ToList();
                    if (!ReferenceEquals(ChildList, null))
                    {
                        for (int i = 0; i < foundDetail.Count; i++)
                        {
                            bool result = ChildList.Exists(s => s == foundDetail[i].Id);
                            if (!result)
                            {
                                var delete = _dbContext.ARDeliveryChallanDiscountDetails.Where(x => x.ARDiscountItemId == foundDetail[i].Id).AsQueryable();
                                _dbContext.ARDeliveryChallanDiscountDetails.RemoveRange(delete);
                                await _ARDiscountItemRepository.DeleteAsync(foundDetail[i]);
                            }
                        }
                    }
                    for (int i = 0; i < ChildList.Count; i++)
                    {
                        ARDiscountItem detail = foundDetail.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["ChildId"][i]));
                        if (!ReferenceEquals(detail, null))
                        {
                            detail.ARDiscount_Id = aRDiscountViewModel.ARDiscount.Id;
                            detail.DiscountPercentage = Convert.ToDecimal(collection["percentage"][i]);
                            detail.DiscountAmount = Convert.ToDecimal(collection["disAmt"][i]);
                            detail.Amount = Convert.ToDecimal(collection["amount"][i]);
                            detail.ItemCategory = Convert.ToInt32(collection["categoryId"][i]);
                            detail.Quantity = Convert.ToDecimal(collection["qty"][i]);
                            UpdateList.Add(detail);
                        }
                    }
                    await _ARDiscountItemRepository.UpdateRangeAsync(UpdateList);

                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        var Ids = collection["DcIds"][i] == "" ? Array.Empty<string>() : collection["DcIds"][i].Split(',');
                        if (Ids.Count() > 0) {
                            ARDiscountItem aRDiscountItem = new ARDiscountItem();
                            aRDiscountItem.ARDiscount_Id = aRDiscountViewModel.ARDiscount.Id;
                            aRDiscountItem.DiscountPercentage = Convert.ToDecimal(collection["Percentage"][i]);
                            aRDiscountItem.DiscountAmount = Convert.ToDecimal(collection["DisAmt"][i]);
                            aRDiscountItem.Amount = Convert.ToDecimal(collection["Amount"][i]);
                            aRDiscountItem.ItemCategory = Convert.ToInt32(collection["categoryId"][i]);
                            aRDiscountItem.Quantity = Convert.ToDecimal(collection["Qty"][i]);
                            var result = _dbContext.ARDiscountItem.Add(aRDiscountItem);
                            _dbContext.SaveChanges();

                            for (int z = 0; z < Ids.Count(); z++)
                            {
                                var data = new ARDeliveryChallanDiscountDetails
                                {
                                    ARDiscountItemId = result.Entity.Id,
                                    DeliveryChallanItemId = Convert.ToInt32(Ids[z])
                                };
                                _dbContext.ARDeliveryChallanDiscountDetails.Add(data);
                                _dbContext.SaveChanges();
                            }
                        }
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "Discount has been updated successfully.";
                }
            }
            catch (Exception ex)
            {

                TempData["error"] = "true";
                TempData["message"] = "Something went wrong";
            }

            return RedirectToAction("Index", "CustomerDiscount");
        }
        public IActionResult GetCustomers(int ProductId, int CategoryId)
        {
            List<int> Category = _dbContext.ARSuplierItemsGroup.Where(x => x.CategoryId == CategoryId).Select(x => x.ARCustomerId).ToList();
            var Customers = _dbContext.ARCustomers.Where(x => x.ProductTypeId == ProductId && Category.Contains(x.Id) && x.IsActive == true && x.IsDeleted == false).ToList();
            return Ok(Customers);
        }
        public async Task<IActionResult> GetCategory(int CustomerId, string startDate, string endDate, int category)
        {
            DateTime StartDate = Convert.ToDateTime(startDate);
            DateTime EndDate = Convert.ToDateTime(endDate);
            var DeliveryChallanDiscountDetails = _dbContext.ARDeliveryChallanDiscountDetails.Include(p => p.DiscountItem).ThenInclude(p => p.Discount).Where(p => p.DiscountItem.Discount.Customer_Id == CustomerId);

            var InvCategories = _dbContext.InvItemCategories;
           
            var invoiceItems = await _dbContext.ARInvoiceItems
                .Include(p => p.Invoice)
                .Include(p => p.ARDeliveryChallanItem).ThenInclude(p => p.DC)
                .Include(p => p.ARDeliveryChallanItem).ThenInclude(p => p.ARSaleOrderItem).ThenInclude(p => p.Item)
                .Where(p => 
                p.Invoice.CustomerId == CustomerId
                && p.ARDeliveryChallanItem.DC.BuiltyDate.Date >= StartDate.Date
                && p.ARDeliveryChallanItem.DC.BuiltyDate.Date <= EndDate.Date
                && p.ARDeliveryChallanItem.DC.BuiltyNo != ""
                && p.Invoice.Status == "Approved" && p.ARDeliveryChallanItem.DC.Status == "Approved"
                && !DeliveryChallanDiscountDetails.Any(x => x.DeliveryChallanItemId == p.ARDeliveryChallanItem.Id)
                ).Where(p=>p.Item.CategoryId != 0)
                .GroupBy(p => p.Item.CategoryId)
                .Select(p => new
                {
                    DCIds = p.Select(p => p.ARDeliveryChallanItem.Id),
                    TotalAmount = p.Sum(p => p.LineTotal),
                    TotalQty = p.Sum(p => p.Qty),
                    TotalMeters = p.Sum(p => p.TotalMeter),
                    CategoryIdLevel4 = p.FirstOrDefault().Item.CategoryId,
                    CategoryNameLevel4 = InvCategories.Where(x => x.Id == p.FirstOrDefault().Item.CategoryId).FirstOrDefault().Name,
                    Rate =_dbContext.ItemPricingDetails.Include(x=>x.ItemPricing).Where(x=>x.ItemID_FourthLevel==p.FirstOrDefault().Item.CategoryId && x.ItemPricing.Status!= "Closed").FirstOrDefault().DiscountAmount
                }).ToListAsync();
            return Ok(invoiceItems);
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            ARDiscount discount = _dbContext.ARDiscount.Include(x=>x.Customer_)
             .Where(a => a.Status == "Created" && a.CompanyId == _companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            List<ARDiscountItem> discountItems = _dbContext.ARDiscountItem.Where(x => x.ARDiscount_Id == id).ToList();
            try
            {
                //Create Voucher
                var accounts = _dbContext.GLAccounts.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                Helpers.VoucherHelper voucher = new Helpers.VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Customer Discount # : {0} of {1} ",
                discount.TransactionNo,
                discount.Customer_.Name
                );

                int voucherId;
                voucherMaster.VoucherType = "CDIS";
                voucherMaster.VoucherDate = DateTime.Now;
                //  voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency = "PKR";
                // voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                voucherMaster.Description = voucherDescription;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Discount";
                voucherMaster.ModuleId = id;
                voucherMaster.ReferenceId = discount.Customer_Id;
                voucherMaster.Amount = discount.GrandTotal;

                //Voucher Details
                //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                //var amount = invoiceItems.Sum(s => s.LineTotal);
                //var discount = invoiceItems.Sum(s => s.DiscountAmount);

                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = accounts.Where(x => x.Name == "DISCOUNT EXPENSE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetail.Sequence = 1;
                voucherDetail.Description = voucherDescription;
                voucherDetail.Debit = discountItems.Sum(x => x.DiscountAmount); 
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                // Credit Entry
                GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                voucherDetailItem.AccountId = accounts.Where(x=>x.Name == "DISCOUNT RESERVE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;  // Discount Reserve 
                voucherDetailItem.Sequence = 1;
                voucherDetailItem.Description = voucherDescription;
                voucherDetailItem.Debit = 0;
                voucherDetailItem.Credit = discountItems.Sum(x => x.DiscountAmount);
                voucherDetailItem.IsDeleted = false;
                voucherDetailItem.CreatedBy = _userId;
                voucherDetailItem.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetailItem);
                //if (discount > 0)
                //{
                //    var accountCode = _dbContext.AppCompanyConfigs.Where(c => c.CompanyId == _companyId && c.ConfigName == "Discount" && c.IsActive).FirstOrDefault().UserValue1;
                //    var discountAccount = _dbContext.GLAccounts.Where(a => a.Code == accountCode && a.CompanyId == _companyId && a.IsActive).FirstOrDefault().Id;
                //    voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = discountAccount;
                //    voucherDetail.Sequence = 10;
                //    voucherDetail.Description = voucherDescription;
                //    voucherDetail.Debit = discount;
                //    voucherDetail.Credit = 0;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = _userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //}
                ////#region Sale Account
                //////Credit Entry
                //////var lineItems = _dbContext.ARInvoiceItems.Where(li => li.InvoiceId == id && !li.IsDeleted).ToList();
                ////var lineItems = (from li in _dbContext.ARInvoiceItems
                ////                 where li.InvoiceId == id
                ////                 select new
                ////                 {
                ////                     li.Total,
                ////                     li.DiscountAmount,
                ////                     li.ServiceAccountId
                ////                 }).GroupBy(l => l.ServiceAccountId)
                ////                           .Select(li => new ARInvoiceItem
                ////                           {
                ////                               Total = li.Sum(c => c.Total) - li.Sum(c => c.DiscountAmount),
                ////                               InvoiceId = li.FirstOrDefault().ServiceAccountId //invoice id is temporarily containing SalesTaxAccountId
                ////                           })
                ////                       .ToList();


                ////var saleTaxAccounts = (from li in _dbContext.ARInvoiceItems
                ////                       join t in _dbContext.AppTaxes on li.TaxSlab equals t.Id 
                ////                       where li.InvoiceId == id
                ////                       select new
                ////                       {
                ////                           li.SalesTaxAmount,
                ////                           t.SalesTaxAccountId
                ////                       }).GroupBy(l => l.SalesTaxAccountId)
                ////                           .Select(li => new ARInvoiceItem
                ////                           {
                ////                               SalesTaxAmount = li.Sum(c => c.SalesTaxAmount),
                ////                               InvoiceId = li.FirstOrDefault().SalesTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                ////                           })
                ////                       .ToList();


                ////#endregion Sale Account
                //////Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    discount.VoucherId = voucherId;
                    discount.Status = "Approved";
                    discount.ApprovedBy = _userId;
                    discount.ApprovedDate = DateTime.Now;
                    discount.IsApproved = true;
                    //On approval updating Invoice
                    
                    var entry = _dbContext.Update(discount);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    //var Invoice = _dbContext.ARInvoiceDiscount.Where(x => x.ARDiscount_Id == id).AsQueryable();
                    //foreach (var x in Invoice)
                    //{
                    //    var j = _dbContext.ARInvoices.FirstOrDefault(y => y.Id == x.InvoiceId);
                    //    j.IsDiscount = true;
                    //    _dbContext.ARInvoices.Update(j);
                    //}
                    await _dbContext.SaveChangesAsync();
                    TempData["error"] = "false";
                    TempData["message"] = "Customer Discount has been approved successfully";
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
            var discount = _dbContext.ARDiscount
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.IsApproved && v.CompanyId == companyId).FirstOrDefault();
            bool ids = _dbContext.ARCustomerAdjustmentItem.Any(x => x.ARDiscount_Id.Equals(id) && x.IsDeleted == false);
            if (ids)
            {
                TempData["error"] = "true";
                TempData["message"] = string.Format("This transaction already used in Customer Discount Adjustment.");
                return RedirectToAction("Index", "CustomerDiscount");
            }
            if (discount == null)
            {
                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == discount.Id).ToList();
                foreach (var item in voucherDetail)
                {
                    var tracker = _dbContext.GLVoucherDetails.Remove(item);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
                discount.Status = "Created";
                discount.UnApprovedBy = userId;
                discount.UnApprovedDate = DateTime.Now;
                discount.IsApproved = false;
                var entry = _dbContext.ARDiscount.Update(discount);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                //var Invoice = _dbContext.ARInvoiceDiscount.Where(x => x.ARDiscount_Id == id).AsQueryable();
                //foreach (var x in Invoice)
                //{
                //    var j = _dbContext.ARInvoices.FirstOrDefault(y => y.Id == x.InvoiceId);
                //    j.IsDiscount = false;
                //    _dbContext.ARInvoices.Update(j);
                //}
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = string.Format("Invoice Id. {0} has been Un-Approved successfully", discount.TransactionNo);
            }
            return RedirectToAction("Index", "CustomerDiscount");
        }
        public async Task<IActionResult> Delete(int id)
        {

            bool ids = _dbContext.ARCustomerAdjustmentItem.Any(x => x.ARDiscount_Id.Equals(id) && x.IsDeleted == false);
            if (ids != true)
            {
                var aRDiscount = _dbContext.ARDiscount.Include(p => p.DiscountItems).ThenInclude(p => p.DeliveryChallanDiscountDetails).FirstOrDefault(p => p.Id == id);
                aRDiscount.IsDeleted = true;
                aRDiscount.IsActive = false;
                _dbContext.ARDiscount.Update(aRDiscount);
                var result = await _dbContext.SaveChangesAsync();
                _dbContext.ARDiscountItem.RemoveRange(aRDiscount.DiscountItems);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = string.Format("The record deleted successfully.");
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = string.Format("This transaction already used in Customer Discount Adjustment.");
            }
            //if (result > 0)
            //{
            //    foreach (var item in aRDiscount.DiscountItems)
            //    {
            //        _dbContext.ARDeliveryChallanDiscountDetails.RemoveRange(item.DeliveryChallanDiscountDetails);
            //        _dbContext.SaveChanges();
            //        _dbContext.ARDiscountItem.Remove(item);
            //        _dbContext.SaveChanges();

            //    }
            //}
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetSalePerson(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ARCustomer customer = _dbContext.ARCustomers.Where(x => x.Id == id).FirstOrDefault();
            CustomerDiscountViewModel customerDiscountViewModel = new CustomerDiscountViewModel();
            
            ARSalePerson salePerson = _dbContext.ARSalePerson.FirstOrDefault(x => x.ID == customer.SalesPersonId && x.CompanyId == companyId);
            ARCommissionAgentCustomer commissionAgentCustomer = _dbContext.ARCommissionAgentCustomer.FirstOrDefault(x => x.Customer_Id == customer.Id);
            
            customerDiscountViewModel.SalePersonId = salePerson.ID;
            customerDiscountViewModel.SalePersonText = Convert.ToString(salePerson.Name);
            if (commissionAgentCustomer != null)
            {
                ARCommissionAgent commissionAgent = _dbContext.ARCommissionAgents.FirstOrDefault(x => x.Id == commissionAgentCustomer.CommissionAgent_Id && x.CompanyId == companyId);
                if (commissionAgent != null)
                {
                    customerDiscountViewModel.CommissionAgentId = Convert.ToInt32(commissionAgent.Id);
                    customerDiscountViewModel.CommissionAgentText = Convert.ToString(commissionAgent.Name);
                }
            } 
            return Ok(customerDiscountViewModel);
        }
        public IActionResult Details(int id)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=PurchaseOrder&cId=", companyId, "&id={0}");
            var discount = _dbContext.ARDiscount.Include(p => p.Customer_).Where(x => x.Id == id).FirstOrDefault();
            var discountItems = _dbContext.ARDiscountItem.Include(x => x.InvItemCategories).Where(x => x.ARDiscount_Id == id).ToList();
            ViewBag.NavbarHeading = "Customer Discount";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = discountItems;
            return View(discount);
        }
        public string GetFormatedDate(DateTime date)
        {
            date = date.AddMonths(3).AddSeconds(-1);
            return date.ToString("dd-MMM-yyyy");
        }
    }
}