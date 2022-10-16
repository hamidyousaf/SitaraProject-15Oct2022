using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;
using Numbers.Controllers;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class SaleOrderController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public SaleOrderController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //var saleOrderRepo = new SaleOrderRepo(_dbContext);
            //IEnumerable<ARSaleOrder> list = saleOrderRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Sale Orders";
            return View();
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var saleOrderRepo = new SaleOrderRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var model = new ARSaleOrderViewModel();
            try
            {
                var saleOrderExp = new BackGroundRefresh(_dbContext);

                saleOrderExp.SaleOrderExpiration();
            }catch(Exception e)
            {

            }
            ViewBag.Counter = 0;
            //ViewBag.DeliveryTerm = configValues.GetConfigValues("AR", "Delivery Term", companyId);
            //ViewBag.PaymentTerm = configValues.GetConfigValues("AR", "Payment Term", companyId).OrderByDescending (x=>x.Text.Contains("After 90 Days"));
            ViewBag.Customer = new SelectList((_dbContext.ARCustomers.Where(x => x.CompanyId == companyId)).ToList(), "Id", "Name");
            //ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x=>x.Text.Contains("GD Sale Local"));
            ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(a => a.IsDeleted == false).ToList(), "Id", "Description");
            ViewBag.TaxList = new SelectList(appTaxRepo.GetTaxes(companyId).ToList(), "Id", "Name");
            //ViewBag.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId)).OrderByDescending(x=>x.Text.Contains("Fresh Category"));
            //ViewBag.CategoryList = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith("01")).OrderBy(x => x.Code).ToList()
                                                  //select new
                                                  //{
                                                  //    Id = ac.Id,
                                                  //    Name = ac.Code + " - " + ac.Name
                                                  //}, "Id", "Name");
            
           
            ViewBag.NoTax = _dbContext.AppTaxes.Where(x => x.Name == "NO TAX").Select(x => x.Id).FirstOrDefault();
            model.ItemCategoryThird = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.Code.StartsWith("07") && x.CategoryLevel == 3 && x.CompanyId == companyId).OrderBy(x => x.Code).ToList()
                                                     select new
                                                     {
                                                         Id = ac.Id,
                                                         Name = ac.Code + " - " + ac.Name
                                                     }, "Id", "Name");
            if (id == 0)
            {
                ViewBag.NoTax = _dbContext.AppTaxes.Where(x => x.Name == "NO TAX").Select(x => x.Id).FirstOrDefault();
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Sale Order";
                //TempData["SaleOrderNo"] = saleOrderRepo.SaleOrderCountNo(companyId);
                
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();


                ViewBag.ProductTypeLOV = configValues.GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId)).OrderByDescending(x=>x.Text.Contains("Fresh Category"));
                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.SecondLevelCategoryLOV = configValues.GetSecondCategoryByResp(resp_Id);
                //ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                //                                              select new
                //                                              {
                //                                                  Id = ac.Id,
                //                                                  Name = ac.Code + " - " + ac.Name
                //                                              }, "Id", "Name");

                ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
                ViewBag.DeliveryTermLOV = configValues.GetConfigValues("AR", "Delivery Term", companyId);
                ViewBag.PaymentTermLOV = configValues.GetConfigValues("AR", "Payment Term", companyId).OrderByDescending(x => x.Text.Contains("After 90 Days"));


                return View(model);
            }
            else
            {
                ViewBag.Id = id;
                ARSaleOrderViewModel modelEdit = saleOrderRepo.GetById(id);
                ViewBag.CustomerId = modelEdit.CustomerId;

                ViewBag.ProductTypeLOV = new SelectList(_dbContext.AppCompanyConfigs.Where(x=>x.Id== modelEdit.ProductTypeId && x.IsDeleted != true /*&& x.CompanyId == companyId*/), "Id", "ConfigValue");
                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.Id == modelEdit.ItemCategoryId && x.IsDeleted != true && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                      select new
                                                      {
                                                          Id = ac.Id,
                                                          Name = ac.Code + " - " + ac.Name
                                                      }, "Id", "Name");
                
                var Categories = _dbContext.InvItemCategories.AsQueryable();
                ViewBag.FourthLevelCategoryLOV = new SelectList(from L1 in Categories
                                                        join L2 in Categories on L1.Id equals L2.ParentId
                                                        join L3 in Categories on L2.Id equals L3.ParentId
                                                        join L4 in Categories on L3.Id equals L4.ParentId
                                                        where L2.Id == modelEdit.ItemCategoryId && L1.IsDeleted != true && L2.IsDeleted != true && L3.IsDeleted != true && L4.IsDeleted != true
                                                        select new
                                                        {
                                                            Id = L4.Id,
                                                            Name = L4.Code + " - " + L4.Name
                                                        }, "Id", "Name");
                ViewBag.CustomersLOV = new SelectList(_dbContext.ARCustomers.Include(x=>x.City).Where(x => x.Id == modelEdit.CustomerId).Select(x => new AppCitiy { Id = x.Id, Name = x.Id +" - " + x.Name + " - " + x.City.Name }).ToList(), "Id", "Name");
                ViewBag.WareHouseLOV = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == modelEdit.WareHouseId && x.IsDeleted != true /*&& x.CompanyId == companyId*/), "Id", "ConfigValue");
                ViewBag.DeliveryTermLOV = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == modelEdit.DeliveryTermId && x.IsDeleted != true && x.CompanyId == companyId), "Id", "ConfigValue");
                ViewBag.PaymentTermLOV = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == modelEdit.PaymentTermId && x.IsDeleted != true && x.CompanyId == companyId), "Id", "ConfigValue");

                modelEdit.TaxList = appTaxRepo.GetTaxes(companyId);
                modelEdit.Currencies = AppCurrencyRepo.GetCurrencies();
                modelEdit.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
                ARSaleOrderItem[] soItems = saleOrderRepo.GetSaleOrderItems(id);
                ViewBag.Items = soItems;
                modelEdit.ARSaleOrderDetails = _dbContext.ARSaleOrderItems.Include(i => i.Item).Where(x => x.SaleOrderId == id).ToList();
                List<int> Category = _dbContext.ARSuplierItemsGroup.Where(x => x.CategoryId == modelEdit.ItemCategoryId).Select(x => x.ARCustomerId).ToList();
                //ViewBag.Customers = new SelectList(_dbContext.ARCustomers.Where(x => x.ProductTypeId == modelEdit.ProductTypeId && Category.Contains(x.Id)).ToList(), "Id", "Name");
                TempData["SaleOrderNo"] = modelEdit.SaleOrderNo;
                for (int i = 0; i < modelEdit.ARSaleOrderDetails.Count; i++)
                {
                    modelEdit.ARSaleOrderDetails[i].Unit = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == modelEdit.ARSaleOrderDetails[i].Item.Unit).ConfigValue;
                    if (modelEdit.ARSaleOrderDetails[i].Item.PackUnit != 0)
                        modelEdit.ARSaleOrderDetails[i].PackUnit = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == modelEdit.ARSaleOrderDetails[i].Item.PackUnit).ConfigValue;
                    modelEdit.ARSaleOrderDetails[i].TaxName = _dbContext.AppTaxes.FirstOrDefault(x => x.Id == modelEdit.ARSaleOrderDetails[i].TaxId).Name;
                }
                //foreach(var a in modelEdit.ARSaleOrderDetails)
                //{
                //    a.Unit = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == modelEdit.ARSaleOrderDetails[1].Item.Unit).ConfigValue;
                //    a.PackUnit = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == modelEdit.ARSaleOrderDetails[1].Item.PackUnit).ConfigValue;
                //    a.TaxName = _dbContext.AppTaxes.FirstOrDefault(x => x.Id == modelEdit.ARSaleOrderDetails[1].TaxId).Name;
                //}

                if (modelEdit.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Update Sale Order";
                    ViewBag.TitleStatus = "Created";
                }
                return View(modelEdit);
            }
        }
        [HttpGet]
        public IActionResult GetItemPrice(int Id)
        {
            if (Id != 0)
            {
                var FourthLevelCatId = _dbContext.InvItems.Where(p => p.Id == Id).FirstOrDefault().CategoryId;
                var itemDetail = _dbContext.ItemPricingDetails.Include(p=>p.ItemPricing)?.Where(p => p.ItemID_FourthLevel == FourthLevelCatId && p.ItemPricing.IsClosed != true && p.ItemPricing.IsDelete != true)?.FirstOrDefault();
                if (itemDetail?.Price_EndDate.Date < DateTime.Now.Date)
                {
                    return Json(new { message = "This Item price has been expired, please price again", status = false });
                };

                return Ok(itemDetail?.ItemPrice);
            }
            return Json(false);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ARSaleOrderViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var saleOrderRepo = new SaleOrderRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.ResponsibilityId = resp_Id;
                bool isSuccess = await saleOrderRepo.Create(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Order No. {0} has been created successfully.", saleOrderRepo.MaxSaleOrder(companyId));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "SaleOrder");
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.ResponsibilityId = resp_Id;
                bool isSuccess = await saleOrderRepo.Update(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Order No. {0} has been updated successfully.", model.SaleOrderNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var saleOrderRepo = new SaleOrderRepo(_dbContext);
            bool isSuccess = await saleOrderRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Order has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Approve(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var saleOrderRepo = new SaleOrderRepo(_dbContext);
            bool isSuccess = await saleOrderRepo.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Order has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var model = _dbContext.ARSaleOrders.Where(i => i.Status == "Approved" && !i.IsDeleted && i.CompanyId == companyId).ToList();
            ViewBag.NavbarHeading = "Un-Approve Sale Order";
            return View(model);
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            //int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            //int []saleOrderItemIds = (from item in _dbContext.ARDeliveryChallanItems
            //                    join master in _dbContext.ARDeliveryChallans.Where(x => x.IsDeleted != true && x.ResponsibilityId == resp_Id)
            //                    on item.DeliveryChallanId equals master.Id
            //                    select item.SaleOrderId).ToArray();
            //var saleOrderIds = _dbContext.ARSaleOrderItems.Where(x=> saleOrderItemIds.Contains(x.Id)).Select(x=>x.SaleOrderId);
            //var result = _dbContext.ARSaleOrders.Any(x=> saleOrderIds.Contains(x.Id));

            var data = (from so in _dbContext.ARSaleOrders.Where(x => x.Id == id)
            join soD in _dbContext.ARSaleOrderItems on so.Id equals soD.SaleOrderId
            where soD.DCQty == 0
            select soD).ToList();

            if (data.Count() == 0)
            {
                TempData["error"] = "true";
                TempData["message"] = "This Sale Order is used in transactions.";
                return RedirectToAction(nameof(Index));
                
            }
            string userId = HttpContext.Session.GetString("UserId");
            var saleOrderRepo = new SaleOrderRepo(_dbContext);
            bool isSuccess = await saleOrderRepo.UnApproveVoucher(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Order has been Un-Approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(UnApproveVoucher));
        }

        [HttpPost]
        public IActionResult PartialSaleOrderItems(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            var model = new ARSaleOrderViewModel();
            var appTaxRepo = new AppTaxRepo(_dbContext);
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            return PartialView("_partialSaleOrderItems", model);
        }
        public IActionResult GetCustomers(int ProductId, int CategoryId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<int> Category = _dbContext.ARSuplierItemsGroup.Where(x => x.CategoryId == CategoryId).Select(x => x.ARCustomerId).ToList();
            var Customers = _dbContext.ARCustomers.Include(x=>x.City).Where(x => x.ProductTypeId == ProductId && Category.Contains(x.Id) /*&& x.CompanyId == companyId*/ && x.IsDeleted != true).ToList();
            var Categories = _dbContext.InvItemCategories.AsQueryable();
            var CategoriesData = from L1 in Categories
                               join L2 in Categories on L1.Id equals L2.ParentId
                               join L3 in Categories on L2.Id equals L3.ParentId
                               join L4 in Categories on L3.Id equals L4.ParentId
                               join item in _dbContext.InvItems on L4.Id equals item.CategoryId
                               where L2.Id == CategoryId && item.IsDeleted != true && L4.IsDeleted != true && L3.IsDeleted != true
                               select new
                               {
                                   id = item.Id,
                                   text = string.Concat(item.Name, " - ", item.Code)
                               };

            return Ok(new { Customers = Customers , categories = CategoriesData });
        }

        [HttpGet]
        public IActionResult GetCustomersByCity( int CityId, int CategoryId)
        {
            List<int> Category = _dbContext.ARSuplierItemsGroup.Where(x => x.CategoryId == CategoryId).Select(x => x.ARCustomerId).ToList();
            var Customers = _dbContext.ARCustomers.Include(x => x.City).Where(x =>x.CityId==CityId && Category.Contains(x.Id) && x.IsDeleted != true).ToList();
            var Categories = _dbContext.InvItemCategories.AsQueryable();
            var CategoriesData = from L1 in Categories
                                 join L2 in Categories on L1.Id equals L2.ParentId
                                 join L3 in Categories on L2.Id equals L3.ParentId
                                 join L4 in Categories on L3.Id equals L4.ParentId
                                 join item in _dbContext.InvItems on L4.Id equals item.CategoryId
                                 where L2.Id == CategoryId && item.IsDeleted != true && L4.IsDeleted != true && L3.IsDeleted != true
                                 select new
                                 {
                                     id = item.Id,
                                     text = string.Concat(item.Name, " - ", item.Code)
                                 };

            return Ok(new { Customers = Customers, categories = CategoriesData });
        }
        public IActionResult GetItemDetails(int id)
        {
            var saleOrderRepo = new SaleOrderRepo(_dbContext);
            var itemDetails = saleOrderRepo.GetItemDetails(id);
            return Ok(itemDetails);
        }
        [HttpGet]
        public IActionResult GetOrderItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleOrderRepo = new SaleOrderRepo(_dbContext);
            var viewModel = saleOrderRepo.GetOrderItems(id, itemId);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            viewModel.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            ViewBag.Counter = id;
            ViewBag.ItemId = viewModel.ItemId;
            return PartialView("_partialSaleOrderItems", viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=SaleOrder&cId=", companyId, "&id={0}");
            var saleOrders = _dbContext.ARSaleOrders
                .Include(i => i.Customer)
                .Include(i => i.WareHouse)
                .Include(x=>x.ProductType)
                .Include(x=>x.ItemCategory)
            //.Include(i => i.DeliveryTerm).Include(i => i.PaymentTerm)
            .Where(i => i.Id == id).FirstOrDefault();
            var saleOrderItems = _dbContext.ARSaleOrderItems
                                .Include(i => i.Item)
                                .Include(i => i.SaleOrder)
                                .Where(i => i.SaleOrderId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Sale Order";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = saleOrderItems;
            return View(saleOrders);
        }
        

        public IActionResult GetSO()
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
                var searchSONo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchSODate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchTotal = Request.Form["columns[3][search][value]"].FirstOrDefault();
                //var searchFrieght = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGrand = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var SOData = (from SO in _dbContext.ARSaleOrders.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select SO);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    SOData = SOData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                SOData = !string.IsNullOrEmpty(searchSONo) ? SOData.Where(m => m.SaleOrderNo.ToString().ToLower().Contains(searchSONo.ToUpper())) : SOData;
                SOData = !string.IsNullOrEmpty(searchSODate) ? SOData.Where(m => m.SaleOrderDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchSODate.ToUpper())) : SOData;
                SOData = !string.IsNullOrEmpty(searchCustomer) ? SOData.Where(m => _dbContext.ARCustomers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.CustomerId)).Name.ToUpper().Contains(searchCustomer.ToUpper())) : SOData;
                SOData = !string.IsNullOrEmpty(searchTotal) ? SOData.Where(m => m.Total.ToString().Contains(searchTotal)) : SOData;
                //SOData = !string.IsNullOrEmpty(searchFrieght) ? SOData.Where(m => m.Freight.ToString().Contains(searchFrieght)) : SOData;
                SOData = !string.IsNullOrEmpty(searchGrand) ? SOData.Where(m => m.GrandTotal.ToString().Contains(searchGrand)) : SOData;
                SOData = !string.IsNullOrEmpty(searchStatus) ? SOData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : SOData;

                recordsTotal = SOData.Count();
                var data = SOData.ToList();
                if (pageSize == -1)
                {
                    data = SOData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = SOData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARSaleOrderViewModel> Details = new List<ARSaleOrderViewModel>();
                foreach (var grp in data)
                {
                    ARSaleOrderViewModel aRSaleOrderViewModel = new ARSaleOrderViewModel();
                    aRSaleOrderViewModel.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == grp.CustomerId).Name;
                    aRSaleOrderViewModel.SODate = grp.SaleOrderDate.ToString(Helpers.CommonHelper.DateFormat);
                    aRSaleOrderViewModel.APSaleOrder = grp;
                    aRSaleOrderViewModel.APSaleOrder.Approve = approve;
                    aRSaleOrderViewModel.APSaleOrder.Unapprove = unApprove;

                    Details.Add(aRSaleOrderViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult GetUnApproveSO()
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
                var searchSONo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchSODate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchTotal = Request.Form["columns[3][search][value]"].FirstOrDefault();
                //var searchFrieght = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGrand = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var SOData = (from SO in _dbContext.ARSaleOrders.Where(x => x.IsDeleted == false && x.CompanyId == companyId && x.Status=="Approved") select SO);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    SOData = SOData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                SOData = !string.IsNullOrEmpty(searchSONo) ? SOData.Where(m => m.SaleOrderNo.ToString().ToLower().Contains(searchSONo.ToUpper())) : SOData;
                SOData = !string.IsNullOrEmpty(searchSODate) ? SOData.Where(m => m.SaleOrderDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchSODate.ToUpper())) : SOData;
                SOData = !string.IsNullOrEmpty(searchCustomer) ? SOData.Where(m => _dbContext.ARCustomers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.CustomerId)).Name.ToUpper().Contains(searchCustomer.ToUpper())) : SOData;
                SOData = !string.IsNullOrEmpty(searchTotal) ? SOData.Where(m => m.Total.ToString().Contains(searchTotal)) : SOData;
                //SOData = !string.IsNullOrEmpty(searchFrieght) ? SOData.Where(m => m.Freight.ToString().Contains(searchFrieght)) : SOData;
                SOData = !string.IsNullOrEmpty(searchGrand) ? SOData.Where(m => m.GrandTotal.ToString().Contains(searchGrand)) : SOData;
                SOData = !string.IsNullOrEmpty(searchStatus) ? SOData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : SOData;

                recordsTotal = SOData.Count();
                var data = SOData.ToList();
                if (pageSize == -1)
                {
                    data = SOData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = SOData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARSaleOrderViewModel> Details = new List<ARSaleOrderViewModel>();
                foreach (var grp in data)
                {
                    ARSaleOrderViewModel aRSaleOrderViewModel = new ARSaleOrderViewModel();
                    aRSaleOrderViewModel.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == grp.CustomerId).Name;
                    aRSaleOrderViewModel.SODate = grp.SaleOrderDate.ToString(Helpers.CommonHelper.DateFormat);
                    aRSaleOrderViewModel.APSaleOrder = grp;
                    aRSaleOrderViewModel.APSaleOrder.Approve = approve;
                    aRSaleOrderViewModel.APSaleOrder.Unapprove = unApprove;

                    Details.Add(aRSaleOrderViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public IActionResult GetFourthLevelCategories(int id) 
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var fourthLevel = _dbContext.InvItemCategories.Where(x=>x.ParentId == id && x.Code.StartsWith("07") && x.CategoryLevel == 4 && x.IsDeleted != true && x.CompanyId == companyId)
        //        .Select(x => new { 
        //            Id = x.Id,
        //            Name = string.Concat(x.Code + " - " + x.Name)
        //        });
        //    return Ok(fourthLevel);
        //}
        [HttpPost]
        public IActionResult GetItems(int CategoryId, int[] SkipIds, int FourthCat, int WarehouseId)
        {
            var Categories = _dbContext.InvItemCategories.AsQueryable();
            var CategoriesData = from L1 in Categories
                                 join L2 in Categories on L1.Id equals L2.ParentId
                                 join L3 in Categories on L2.Id equals L3.ParentId
                                 join L4 in Categories on L3.Id equals L4.ParentId
                                 join item in _dbContext.InvItems.Where(x => !SkipIds.Contains(x.Id)) on L4.Id equals item.CategoryId

                                 join bale in _dbContext.BaleInformation.Where(x=>x.TempBales != 0 && !x.UsedFNumber && x.WarehouseId == WarehouseId) on item.Id equals bale.ItemId

                        
                                 join sq in _dbContext.ItemPricingDetails.Where(x=>x.IsDelete!=true && x.Price_EndDate.Date >= DateTime.Now.Date ? x.Price_EndDate.Date >= DateTime.Now.Date : false) on L4.Id equals sq.ItemID_FourthLevel
                                   into pma
                                 from sq in pma.DefaultIfEmpty()
                                 join pricingmaster in _dbContext.ItemPricings.Where(x => x.IsClosed == false) on (sq == null ? 0 : sq.ItemPrice_Id) equals pricingmaster.ID
                                 //where L2.Id == CategoryId && item.IsDeleted != true && L4.IsDeleted != true && L3.IsDeleted != true
                                 where L4.Id == FourthCat && item.IsDeleted != true && L4.IsDeleted != true && L3.IsDeleted != true
                                 select new InventoryViewModel
                                 {
                                     Id = item.Id,
                                     BaleId = bale.Id,
                                     Code = item.Code,
                                     Name = item.Name,
                                     BaleType = bale.BaleType,
                                     Meters =bale.Meters,
                                     TempBales = bale.TempBales,
                                     AvailableStock = bale.Bales,
                                     Rate = sq != null ? sq.ItemPrice : 0 ,
                                     CategoryId =item.CategoryId
                                     

                                 };
            var data = CategoriesData.GroupBy(x => new { x.Id, x.Meters ,x.BaleType })
                 .Select(item => new InventoryViewModel
                 {
                     Id = item.Select(x=>x.Id).FirstOrDefault(),
                     BaleId = item.Select(x=>x.BaleId).FirstOrDefault(),
                     Code = item.Select(x=>x.Code).FirstOrDefault(),
                     Name = item.Select(x=>x.Name).FirstOrDefault(),
                     BaleType = item.Select(x=>x.BaleType).FirstOrDefault(),
                     Meters = item.Select(x=>x.Meters).FirstOrDefault(),
                     TempBales = item.Select(x => x.TempBales).Sum(),
                     AvailableStock = item.Select(x=>x.AvailableStock).FirstOrDefault(),
                     Rate = item.Select(x=>x.Rate).FirstOrDefault(),
                     CategoryId = item.Select(x=>x.CategoryId).FirstOrDefault()
                 }).ToList();

            //var dtnew = (from m in _dbContext.ItemPricings.Where(x=>x.IsClosed==false).ToList()
            //             join pricing in _dbContext.ItemPricingDetails.ToList() on m.ID equals pricing.ItemPrice_Id into gj
            //             from pricing in gj.DefaultIfEmpty()
            //             select pricing
            //             ).ToList();

            //var newmodel = from cd in  CategoriesData 
            //               join pricing in dtnew.Where(x => x.Price_EndDate.Date <= DateTime.Now.Date ) on cd.CategoryId equals pricing.ItemID_FourthLevel into gj
            //               from pricing in gj.DefaultIfEmpty()

            //               select new InventoryViewModel
            //               {
            //                   Id = cd.Id,
            //                   BaleId = cd.BaleId,
            //                   Code = cd.Code,
            //                   Name = cd.Name,
            //                   BaleType = cd.BaleType,
            //                   Meters = cd.Meters,
            //                   AvailableStock = cd.AvailableStock,
            //                   CategoryId = cd.CategoryId,
            //                   Rate = pricing.ItemPrice


            //               };




            return PartialView("_SaleOrderItemPopUp", data);
        }

        public IActionResult GetDetails(int ItemId , decimal Meters)
        {
            var Data = from item in _dbContext.InvItems 
                                 join bale in _dbContext.BaleInformation on item.Id equals bale.ItemId
                                 join pricing in _dbContext.ItemPricingDetails.Where(x => x.IsDelete != true && x.Price_EndDate.Date >= DateTime.Now.Date ? x.Price_EndDate.Date >= DateTime.Now.Date : false) on item.CategoryId equals pricing.ItemID_FourthLevel
                                 join pricingmaster in _dbContext.ItemPricings.Where(x => x.IsClosed == false) on pricing.ItemPrice_Id equals pricingmaster.ID
                       where bale.ItemId == ItemId && bale.Meters == Meters && item.IsDeleted != true 
                                 select new InventoryViewModel
                                 {
                                     Id = item.Id,
                                     Code = item.Code,
                                     Name = item.Name,
                                     BaleType = bale.BaleType,
                                     Meters = bale.Meters,
                                     AvailableStock = bale.Bales,
                                     BaleId = bale.Id,
                                     TempBales = bale.TempBales,
                                    // Rate = item.AvgRate,
                                     Rate = pricing.ItemPrice
                                 };
            var data = Data.GroupBy(x => new { x.Id, x.Meters })
                 .Select(item => new InventoryViewModel
                 {
                     Id = item.Select(x => x.Id).FirstOrDefault(),
                     BaleId = item.Select(x => x.BaleId).FirstOrDefault(),
                     Code = item.Select(x => x.Code).FirstOrDefault(),
                     Name = item.Select(x => x.Name).FirstOrDefault(),
                     BaleType = item.Select(x => x.BaleType).FirstOrDefault(),
                     Meters = item.Select(x => x.Meters).FirstOrDefault(),
                     TempBales = item.Select(x => x.TempBales).Sum(),
                     AvailableStock = item.Select(x => x.AvailableStock).FirstOrDefault(),
                     Rate = item.Select(x => x.Rate).FirstOrDefault(),
                     CategoryId = item.Select(x => x.CategoryId).FirstOrDefault()
                 }).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetFourthLevel(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Categories = _dbContext.InvItemCategories.AsQueryable();
            var thirdLevelCategory = new SelectList (from L1 in Categories
                                 join L2 in Categories on L1.Id equals L2.ParentId
                                 join L3 in Categories on L2.Id equals L3.ParentId
                                 join L4 in Categories on L3.Id equals L4.ParentId
                                 where L2.Id == id && L1.IsDeleted != true && L2.IsDeleted!= true && L3.IsDeleted != true && L4.IsDeleted != true
                                 select new
                                 {
                                    Id = L4.Id,
                                    Name = L4.Code + " - " + L4.Name
                                 }, "Id", "Name");
            return Ok(thirdLevelCategory);
        }
    }
}
