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
    public class CustomerDiscountAdjustmentController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly ARDiscountAdjustmentRepository _ARDiscountAdjustmentRepository;
        private readonly ARDiscountAdjustmentItemRepository _ARDiscountAdjustmentItemRepository;
        public CustomerDiscountAdjustmentController(NumbersDbContext context, ARDiscountAdjustmentRepository ARDiscountAdjustmentRepository, ARDiscountAdjustmentItemRepository ARDiscountAdjustmentItemRepository)
        {
            _dbContext = context;
            _ARDiscountAdjustmentRepository = ARDiscountAdjustmentRepository;
            _ARDiscountAdjustmentItemRepository = ARDiscountAdjustmentItemRepository;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Customer Discounts Adjustment";
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
                var DiscountData = (from tempcustomer in _dbContext.ARDiscountAdjustment.Where(x => x.IsDeleted != true && x.CompanyId == companyId) select tempcustomer);
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
                List<ARCustomerDiscountAdjViewModel> Details = new List<ARCustomerDiscountAdjViewModel>();
                foreach (var grp in data)
                {
                    ARCustomerDiscountAdjViewModel ARDiscountAdjustment = new ARCustomerDiscountAdjViewModel();
                    ARDiscountAdjustment.EndDate = grp.EndDate.ToString("dd-MMM-yyyy");
                    ARDiscountAdjustment.StartDate = grp.StartDate.ToString("dd-MMM-yyyy");
                    ARDiscountAdjustment.CategoryName = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategory_Id).Name;
                    ARDiscountAdjustment.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == grp.Customer_Id).Name;
                    ARDiscountAdjustment.ProductName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == grp.ProductType_Id).ConfigValue;
                    ARDiscountAdjustment.Trans = grp.TransactionNo;
                    ARDiscountAdjustment.ARDiscountAdjustment = grp;
                    ARDiscountAdjustment.ARDiscountAdjustment.Approve = approve;
                    ARDiscountAdjustment.ARDiscountAdjustment.Unapprove = unApprove;
                    Details.Add(ARDiscountAdjustment);
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
            ViewBag.NavbarHeading = "Create Customer Discount Adjustment";
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string UserId = HttpContext.Session.GetString("UserId");
            ViewBag.CustomerDiscounts = new SelectList((from p in _dbContext.ARDiscount.Where(x => x.IsDeleted == false && x.IsApproved == true).ToList()
                                                where !_dbContext.ARDiscountAdjustment.Where(x => x.IsDeleted == false).Any(s => p.Id.ToString().Contains(s.CustomerDiscountId.ToString()))
                                                select p
                                                             ).ToList(), "Id", "TransactionNo");
            ViewBag.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId));
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted != true && x.IsActive != false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                          select new
                                                          {
                                                              Id = ac.Id,
                                                              Name = ac.Code + " - " + ac.Name
                                                          }, "Id", "Name");
            ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(x => x.CountryId == 1).ToList(), "Id", "Name");
            ARCustomerDiscountAdjViewModel ARDiscountAdjustment = new ARCustomerDiscountAdjViewModel();
            if (id == null)
            {
                //var result = _ARDiscountAdjustmentRepository.Get(x => x.IsActive == true).ToList();
                //if (result.Count > 0)
                //{
                //    ViewBag.Id = _ARDiscountAdjustmentRepository.Get().Select(x => x.TransactionNo).Max() + 1;
                //}
                //else
                //{
                //    ViewBag.Id = 1;
                //}
            }
            else
            {
                ARDiscountAdjustment.Status = "Updated";
                ARDiscountAdjustment.ARDiscountAdjustment = _ARDiscountAdjustmentRepository.Get(x => x.Id == id).Include(p => p.DiscountItems).FirstOrDefault();
               // ViewBag.SalePerson = _dbContext.ARSalePerson.FirstOrDefault(x => x.ID == ARDiscountAdjustment.ARDiscountAdjustment.SalesPerson_Id).Name;

                ARCustomer customer = _dbContext.ARCustomers.Where(x => x.Id == ARDiscountAdjustment.ARDiscountAdjustment.Customer_Id).FirstOrDefault();
                CustomerDiscountViewModel customerDiscountViewModel = new CustomerDiscountViewModel();

                ARSalePerson salePerson = _dbContext.ARSalePerson.FirstOrDefault(x => x.ID == customer.SalesPersonId && x.CompanyId == companyId);
                ARCommissionAgentCustomer commissionAgentCustomer = _dbContext.ARCommissionAgentCustomer.FirstOrDefault(x => x.Customer_Id == customer.Id);
                var commissionAgent = _dbContext.ARCommissionAgents.FirstOrDefault(x => x.Id == commissionAgentCustomer.CommissionAgent_Id && x.CompanyId == companyId).Name;

                ViewBag.CommisionAgent = commissionAgent;
                ViewBag.CustomerDiscounts = new SelectList(_dbContext.ARDiscount.Where(x=>x.Id== ARDiscountAdjustment.ARDiscountAdjustment.CustomerDiscountId), "Id", "TransactionNo");
                ARDiscountAdjustment.ARDiscountAdjustmentItem = new List<ARDiscountAdjustmentItem>();
                ARDiscountAdjustment.Category = new List<string>();
                foreach (var grp in ARDiscountAdjustment.ARDiscountAdjustment.DiscountItems)
                {
                    string Category = null;
                    Category = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategory).Name;
                    ARDiscountAdjustment.Category.Add(Category);
                }
                ViewBag.Customers = new SelectList(_dbContext.ARCustomers.Where(x => x.IsActive != false && x.IsDeleted != true/* && x.CompanyId == companyId*/), "Id", "Name");
                ViewBag.SalesPersons = new SelectList(_dbContext.ARSalePerson.Where(x => x.IsActive == true && x.CompanyId == companyId), "ID", "Name");
            }
            return View(ARDiscountAdjustment);
        }
        public async Task<IActionResult> Submit(ARCustomerDiscountAdjViewModel ARCustomerDiscountAdjViewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            // var DiscountData = _ARDiscountAdjustmentRepository.Get().Include(p => p.DiscountItems).ThenInclude(p => p.DeliveryChallanDiscountDetails);
            var DiscountData = _dbContext.ARDiscountAdjustment.ToList();
            try
            {
                ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.CityId = Convert.ToInt32(collection["CityId"]);
                if (ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Id == 0)
                {
                    ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.CompanyId = companyId;
                    ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.CreatedBy = userId;
                    ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.CreatedDate = DateTime.Now;
                    ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.IsActive = true;
                    ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Status = "Created";
                    ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Resp_Id = resp_Id;
                    if(DiscountData.Count==0)
                    {
                        ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.TransactionNo = 1;
                    }else
                    {
                        ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.TransactionNo = DiscountData.Max(p => p.TransactionNo) + 1;
                    }
                  
                    await _ARDiscountAdjustmentRepository.CreateAsync(ARCustomerDiscountAdjViewModel.ARDiscountAdjustment);

                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        ARDiscountAdjustmentItem ARDiscountAdjustmentItem = new ARDiscountAdjustmentItem();
                        ARDiscountAdjustmentItem.ARDiscountAdjustmentId = ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Id;
                        ARDiscountAdjustmentItem.DiscountPercentage = Convert.ToDecimal(collection["Percentage"][i]);
                        ARDiscountAdjustmentItem.DiscountAmount = Convert.ToDecimal(collection["DisAmt"][i]);
                        ARDiscountAdjustmentItem.Amount = Convert.ToDecimal(collection["Amount"][i]);
                        ARDiscountAdjustmentItem.ItemCategory = Convert.ToInt32(collection["CategoryId"][i]);
                        ARDiscountAdjustmentItem.Quantity = Convert.ToDecimal(collection["Qty"][i]);
                        ARDiscountAdjustmentItem.ActualDiscountRate = Convert.ToDecimal(collection["actualDiscountRate"][i]);
                        ARDiscountAdjustmentItem.PaymentAmount = Convert.ToDecimal(collection["PayAmt"][i]);
                        ARDiscountAdjustmentItem.ReserveRate = Convert.ToDecimal(collection["ResRate"][i]);
                        ARDiscountAdjustmentItem.ReserveAmount = Convert.ToDecimal(collection["ResAmt"][i]);
                        var result = _dbContext.ARDiscountAdjustmentItem.Add(ARDiscountAdjustmentItem);
                        await _dbContext.SaveChangesAsync();
                       // var Ids = collection["DiscountItemId"][i].Split(',');
                        //for (int z = 0; z < Ids.Count(); z++)
                        //{
                        //    var data = new ARDeliveryChallanDiscountDetails
                        //    {
                        //        ARDiscountAdjustmentItemId = result.Entity.Id,
                        //        DeliveryChallanItemId = Convert.ToInt32(Ids[z])
                        //    };
                        //    _dbContext.ARDeliveryChallanDiscountDetails.Add(data);
                        //    _dbContext.SaveChanges();
                        //}
                    }

                    TempData["error"] = "false";
                    TempData["message"] = "Discount Adjustment has been saved successfully.";
                }
                else
                {
                    ARDiscountAdjustment ARDiscountAdjustment = _dbContext.ARDiscountAdjustment.FirstOrDefault(x => x.Id == ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Id);
                    ARDiscountAdjustment.UpdatedBy = userId;
                    ARDiscountAdjustment.UpdatedDate = DateTime.Now;
                    ARDiscountAdjustment.IsActive = true;
                    ARDiscountAdjustment.Status = "Created";
                    ARDiscountAdjustment.DiscountPercent = ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.DiscountPercent;
                    ARDiscountAdjustment.GrandTotal = ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.GrandTotal;
                    await _ARDiscountAdjustmentRepository.UpdateAsync(ARDiscountAdjustment);
                    var UpdateList = new List<ARDiscountAdjustmentItem>();
                    var rownber = collection["ChildId"].Count;
                    List<int> ChildList = new List<int>();
                    for (int i = 0; i < rownber; i++)
                    {
                        int id = Convert.ToInt32(collection["ChildId"][i]);
                        ChildList.Add(id);
                    }
                    var foundDetail = _ARDiscountAdjustmentItemRepository.Get(a => a.ARDiscountAdjustmentId == ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Id).ToList();
                    //if (!ReferenceEquals(ChildList, null))
                    //{
                    //    for (int i = 0; i < foundDetail.Count; i++)
                    //    {
                    //        bool result = ChildList.Exists(s => s == foundDetail[i].Id);
                    //        if (!result)
                    //        {
                    //            var delete = _dbContext.ARDeliveryChallanDiscountDetails.Where(x => x.ARDiscountAdjustmentItemId == foundDetail[i].Id).AsQueryable();
                    //            _dbContext.ARDeliveryChallanDiscountDetails.RemoveRange(delete);
                    //            await _ARDiscountAdjustmentItemRepository.DeleteAsync(foundDetail[i]);
                    //        }
                    //    }
                    //}
                    for (int i = 0; i < ChildList.Count; i++)
                    {
                        ARDiscountAdjustmentItem detail = foundDetail.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["ChildId"][i]));
                        if (!ReferenceEquals(detail, null))
                        {
                            detail.ARDiscountAdjustmentId = ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Id;
                            detail.DiscountPercentage = Convert.ToDecimal(collection["percentage"][i]);
                            detail.DiscountAmount = Convert.ToDecimal(collection["disAmt"][i]);
                            detail.Amount = Convert.ToDecimal(collection["amount"][i]);
                            detail.ItemCategory = Convert.ToInt32(collection["categoryId"][i]);
                            detail.Quantity = Convert.ToDecimal(collection["qty"][i]);
                            detail.ActualDiscountRate = Convert.ToDecimal(collection["actualDiscountRate"][i]);
                            detail.PaymentAmount = Convert.ToDecimal(collection["PayAmt"][i]);
                            detail.ReserveRate = Convert.ToDecimal(collection["ResRate"][i]);
                            detail.ReserveAmount = Convert.ToDecimal(collection["ResAmt"][i]);
                            UpdateList.Add(detail);
                        }
                    }
                    await _ARDiscountAdjustmentItemRepository.UpdateRangeAsync(UpdateList);

                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        var Ids = collection["DcIds"][i] == "" ? Array.Empty<string>() : collection["DcIds"][i].Split(',');
                        if (Ids.Count() > 0) {
                            ARDiscountAdjustmentItem ARDiscountAdjustmentItem = new ARDiscountAdjustmentItem();
                            ARDiscountAdjustmentItem.ARDiscountAdjustmentId = ARCustomerDiscountAdjViewModel.ARDiscountAdjustment.Id;
                            ARDiscountAdjustmentItem.DiscountPercentage = Convert.ToDecimal(collection["Percentage"][i]);
                            ARDiscountAdjustmentItem.DiscountAmount = Convert.ToDecimal(collection["DisAmt"][i]);
                            ARDiscountAdjustmentItem.Amount = Convert.ToDecimal(collection["Amount"][i]);
                            ARDiscountAdjustmentItem.ItemCategory = Convert.ToInt32(collection["categoryId"][i]);
                            ARDiscountAdjustmentItem.Quantity = Convert.ToDecimal(collection["Qty"][i]);
                            ARDiscountAdjustmentItem.ActualDiscountRate = Convert.ToDecimal(collection["actualDiscountRate"][i]);
                            ARDiscountAdjustmentItem.PaymentAmount = Convert.ToDecimal(collection["PayAmt"][i]);
                            ARDiscountAdjustmentItem.ReserveRate = Convert.ToDecimal(collection["ResRate"][i]);
                            ARDiscountAdjustmentItem.ReserveAmount = Convert.ToDecimal(collection["ResAmt"][i]);

                            var result = _dbContext.ARDiscountAdjustmentItem.Add(ARDiscountAdjustmentItem);
                            _dbContext.SaveChanges();

                            //for (int z = 0; z < Ids.Count(); z++)
                            //{
                            //    var data = new ARDeliveryChallanDiscountDetails
                            //    {
                            //        ARDiscountAdjustmentItemId = result.Entity.Id,
                            //        DeliveryChallanItemId = Convert.ToInt32(Ids[z])
                            //    };
                            //    _dbContext.ARDeliveryChallanDiscountDetails.Add(data);
                            //    _dbContext.SaveChanges();
                            //}
                        }
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "Discount Adjustment has been updated successfully.";
                }
            }
            catch (Exception ex)
            {

                TempData["error"] = "true";
                TempData["message"] = "Something went wrong";
            }

            return RedirectToAction("Index", "CustomerDiscountAdjustment");
        }
        public IActionResult GetCustomers(int ProductId, int CategoryId,int CityId)
        {
            List<int> Category = _dbContext.ARSuplierItemsGroup.Where(x => x.CategoryId == CategoryId).Select(x => x.ARCustomerId).ToList();
            var Customers = _dbContext.ARCustomers.Where(x => x.ProductTypeId == ProductId && Category.Contains(x.Id) && x.CityId==CityId && x.IsActive == true && x.IsDeleted == false).ToList();
            return Ok(Customers);
        }

        [HttpGet]
        public IActionResult GetDiscount(int Id)
        {
            if (Id != 0)
            {
                // var packing = _dbContext.ARPacking.Where(x => x.CustomerId == Id && x.IsApproved == true).ToList();
                var pendingDiscount = (from p in _dbContext.ARDiscount.Where(x => x.IsDeleted == false && x.Customer_Id == Id && x.IsApproved == true).ToList()
                                      where !_dbContext.ARDiscountAdjustment.Where(x => x.IsDeleted == false).Any(s => p.Id.ToString().Contains(s.CustomerDiscountId.ToString()))
                                      select p
                                                              ).ToList();
                if (pendingDiscount != null)
                {
                    return Ok(pendingDiscount);
                }
                return Ok(null);
            }
            return Ok(null);
        }

     
        public IActionResult GetDiscountList(int id)
        {
            if (id != 0)
            {
                var IGPList = (from a in _dbContext.ARDiscountItem.Include(x => x.Discount).Include(x => x.InvItemCategories).Where(x => x.Discount.Id == id && x.Discount.IsDeleted == false && x.Discount.IsApproved == true).ToList()
                               select new
                               {
                                   DiscountItemId = a.Id,
                                   ItemCategoryId = a.ItemCategory,
                                   CategoryName = a.InvItemCategories.Name,
                                   Qty = a.Quantity,
                                   Amount = a.Amount,
                                   DiscountRate = a.DiscountPercentage,
                                   DiscountAmount = a.DiscountAmount
                               }).ToList();
                     
                
                return Ok(IGPList);
            }
            else
            {
                return Ok();
            }
        }



        public IActionResult GetCategory(int CustomerId, string startDate, string endDate, int category)
        {
            DateTime StartDate = Convert.ToDateTime(startDate);
            DateTime EndDate = Convert.ToDateTime(endDate);
            var DeliveryChallanDiscountDetails = _dbContext.ARDeliveryChallanDiscountDetails.Include(p => p.DiscountItem).ThenInclude(p => p.Discount).Where(p => p.DiscountItem.Discount.Customer_Id == CustomerId);

            var InvCategories = _dbContext.InvItemCategories;
            var CategoryLevels = from L1 in InvCategories
                                 join L2 in InvCategories on L1.Id equals L2.ParentId
                                 join L3 in InvCategories on L2.Id equals L3.ParentId
                                 join L4 in InvCategories on L3.Id equals L4.ParentId
                                 join item in _dbContext.InvItems on L4.Id equals item.CategoryId
                                 where L2.Id == category
                                 select new
                                 {
                                     Level2Id = L2.Id,
                                     Level3Id = L3.Id,
                                     Level4Id = L4.Id,
                                     ItemId = item.Id
                                 };

            var invoiceItems = _dbContext.ARInvoiceItems
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
                    CategoryIdLevel4 = p.FirstOrDefault().Item.CategoryId,
                    CategoryNameLevel4 = InvCategories.Where(x => x.Id == p.FirstOrDefault().Item.CategoryId).FirstOrDefault().Name,
                    Rate =_dbContext.ItemPricingDetails.Where(x=>x.ItemID_FourthLevel==p.FirstOrDefault().Item.CategoryId).FirstOrDefault().DiscountAmount
                }).ToList();
            return Ok(invoiceItems);
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            ARDiscountAdjustment discount = _dbContext.ARDiscountAdjustment.Include(x=>x.Customer_)
             .Where(a => a.Status == "Created" && a.CompanyId == _companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            List<ARDiscountAdjustmentItem> discountItems = _dbContext.ARDiscountAdjustmentItem.Where(x => x.ARDiscountAdjustmentId == id).ToList();
            try
            {
                //Create Voucher
                var accounts = _dbContext.GLAccounts.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                Helpers.VoucherHelper voucher = new Helpers.VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Customer Discount Adjustment # : {0} of {1} ",
                discount.TransactionNo,
                discount.Customer_.Name
                );

                int voucherId;
                voucherMaster.VoucherType = "CDISADJ";
                voucherMaster.VoucherDate = DateTime.Now;
                //  voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency = "PKR";
                // voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                voucherMaster.Description = voucherDescription;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/DiscountAdjustment";
                voucherMaster.ModuleId = id;
                voucherMaster.ReferenceId = discount.Customer_Id;
                voucherMaster.Amount = discount.DiscountPercent;

                //Voucher Details
                //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                //var amount = invoiceItems.Sum(s => s.LineTotal);
                //var discount = invoiceItems.Sum(s => s.DiscountAmount);

                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = accounts.Where(x => x.Name == "DISCOUNT RESERVE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetail.Sequence = 1;
                voucherDetail.Description = voucherDescription;
                voucherDetail.Debit = discountItems.Sum(x=>x.ReserveAmount);
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                //Credit Entry
                GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                voucherDetailItem.AccountId = accounts.Where(x => x.Name == "DISCOUNT PAYABLE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetailItem.Sequence = 2;
                voucherDetailItem.Description = voucherDescription;
                voucherDetailItem.Debit = 0;
                voucherDetailItem.Credit = discountItems.Sum(x => x.ReserveAmount); ;
                voucherDetailItem.IsDeleted = false;
                voucherDetailItem.CreatedBy = _userId;
                voucherDetailItem.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetailItem);

                //Debit Entry
                GLVoucherDetail voucherDetail2 = new GLVoucherDetail();
                voucherDetail2.AccountId = accounts.Where(x => x.Name == "DISCOUNT PAYABLE" && x.AccountLevel == 4 && x.IsDeleted == false && x.IsActive == true).FirstOrDefault().Id;   //Discount Expense
                voucherDetail2.Sequence = 3;
                voucherDetail2.Description = voucherDescription;
                voucherDetail2.Debit = discountItems.Sum(x => x.PaymentAmount); 
                voucherDetail2.Credit = 0;
                voucherDetail2.IsDeleted = false;
                voucherDetail2.CreatedBy = _userId;
                voucherDetail2.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail2);

                //Credit Entry
                GLVoucherDetail voucherDetailItem2 = new GLVoucherDetail();
                voucherDetailItem2.AccountId = accounts.Where(x => x.Name == "TRADE RECEIVABLES" && x.AccountLevel==4 && x.IsDeleted==false && x.IsActive==true).FirstOrDefault().Id;   //Discount Expense
                voucherDetailItem2.Sequence = 4;
                voucherDetailItem2.Description = voucherDescription;
                voucherDetailItem2.Debit = 0;
                voucherDetailItem2.Credit = discountItems.Sum(x => x.PaymentAmount);
                voucherDetailItem2.IsDeleted = false;
                voucherDetailItem2.CreatedBy = _userId;
                voucherDetailItem2.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetailItem2);

                //#endregion Sale Account
                //Create Voucher 
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
                    //var Invoice = _dbContext.ARInvoiceDiscount.Where(x => x.ARDiscountAdjustmentId == id).AsQueryable();
                    //foreach (var x in Invoice)
                    //{
                    //    var j = _dbContext.ARInvoices.FirstOrDefault(y => y.Id == x.InvoiceId);
                    //    j.IsDiscount = true;
                    //    _dbContext.ARInvoices.Update(j);
                    //}
                    await _dbContext.SaveChangesAsync();
                    TempData["error"] = "false";
                    TempData["message"] = "Customer Discount Adjustment has been approved successfully";
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
            var discount = _dbContext.ARDiscountAdjustment
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.IsApproved && v.CompanyId == companyId).FirstOrDefault();
            //bool ids = _dbContext.ARCustomerAdjustmentItem.Any(x => x.ARDiscountAdjustmentId.Equals(id) && x.IsDeleted == false);
            //if (ids)
            //{
            //    TempData["error"] = "true";
            //    TempData["message"] = string.Format("This transaction already used in Customer Discount Adjustment.");
            //    return RedirectToAction("Index", "CustomerDiscountAdjustment");
            //}
            //if (discount == null)
            //{
            //    TempData["error"] = "true";
            //    TempData["message"] = "Voucher not found";
            //}
            //else
            //{
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
                var entry = _dbContext.ARDiscountAdjustment.Update(discount);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                //var Invoice = _dbContext.ARInvoiceDiscount.Where(x => x.ARDiscountAdjustmentId == id).AsQueryable();
                //foreach (var x in Invoice)
                //{
                //    var j = _dbContext.ARInvoices.FirstOrDefault(y => y.Id == x.InvoiceId);
                //    j.IsDiscount = false;
                //    _dbContext.ARInvoices.Update(j);
                //}
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = string.Format("Invoice Id. {0} has been Un-Approved successfully", discount.TransactionNo);
            //}
            return RedirectToAction("Index", "CustomerDiscountAdjustment");
        }
        public async Task<IActionResult> Delete(int id)
        {

            bool ids = _dbContext.ARCustomerAdjustmentItem.Any(x => x.DiscountAdjustment_Id.Equals(id) && x.IsDeleted == false);
            if (ids != true)
            {
                //var ARDiscountAdjustment = _dbContext.ARDiscountAdjustment.Include(p => p.DiscountItems).ThenInclude(p => p.DeliveryChallanDiscountDetails).FirstOrDefault(p => p.Id == id);
                var ARDiscountAdjustment = _dbContext.ARDiscountAdjustment.Include(p => p.DiscountItems).FirstOrDefault(p => p.Id == id);
                ARDiscountAdjustment.IsDeleted = true;
                ARDiscountAdjustment.IsActive = false;
                _dbContext.ARDiscountAdjustment.Update(ARDiscountAdjustment);
                var result = await _dbContext.SaveChangesAsync();
                _dbContext.ARDiscountAdjustmentItem.RemoveRange(ARDiscountAdjustment.DiscountItems);
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
            //    foreach (var item in ARDiscountAdjustment.DiscountItems)
            //    {
            //        _dbContext.ARDeliveryChallanDiscountDetails.RemoveRange(item.DeliveryChallanDiscountDetails);
            //        _dbContext.SaveChanges();
            //        _dbContext.ARDiscountAdjustmentItem.Remove(item);
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
            ARCommissionAgent commissionAgent = _dbContext.ARCommissionAgents.FirstOrDefault(x => x.Id == commissionAgentCustomer.CommissionAgent_Id && x.CompanyId == companyId);

            customerDiscountViewModel.SalePersonId = salePerson.ID;
            customerDiscountViewModel.SalePersonText = Convert.ToString(salePerson.Name);
            customerDiscountViewModel.CommissionAgentId = Convert.ToInt32(commissionAgent.Id);
            customerDiscountViewModel.CommissionAgentText = Convert.ToString(commissionAgent.Name);

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
            var discount = _dbContext.ARDiscountAdjustment.Include(p => p.Customer_).Where(x => x.Id == id).FirstOrDefault();
            var discountItems = _dbContext.ARDiscountAdjustmentItem.Include(x => x.InvItemCategories).Where(x => x.ARDiscountAdjustmentId == id).ToList();
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