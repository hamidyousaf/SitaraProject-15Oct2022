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
using Numbers.Repository.AP;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class PurchaseOrderController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly APPurchaseRequisitionDetailsRepository _APPurchaseRequisitionDetailsRepository;
        private readonly APPurchaseRequisitionRepository _APPurchaseRequisitionRepository;
        public PurchaseOrderController(NumbersDbContext dbContext, APPurchaseRequisitionDetailsRepository aPPurchaseRequisitionDetailsRepository, APPurchaseRequisitionRepository aPPurchaseRequisitionRepository)
        {
            _dbContext = dbContext;
            _APPurchaseRequisitionDetailsRepository = aPPurchaseRequisitionDetailsRepository;
            _APPurchaseRequisitionRepository = aPPurchaseRequisitionRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                  .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                  .Select(c => c.ConfigValue)
                  .FirstOrDefault();
            ViewBag.ReportUrl =configs;
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            IEnumerable<APPurchaseOrder> orders = purchaseOrderRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Purchase Orders";
            return View(orders);
        }

        public IActionResult GetPO()
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                int respId = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == respId).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == respId).FirstOrDefault().UnApprove;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var searchPoNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchCsNo = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchPoDate  = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchSuplier = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchFreight = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchTotal = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchTotalTax = Request.Form["columns[6][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[7][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[8][search][value]"].FirstOrDefault();


                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var POData = (from tempcustomer in _dbContext.APPurchaseOrders.Include(p=>p.APPurchaseOrderItems).Where(x => x.IsDeleted == false && x.Resp_ID == respId && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    POData = POData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    POData = POData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
             
                POData = !string.IsNullOrEmpty(searchPoNo) ? POData.Where(m => m.PONo.ToString().Contains(searchPoNo)) : POData;
                POData = !string.IsNullOrEmpty(searchCsNo) ? POData.Where(m => m.CSNo.ToString().Contains(searchCsNo)) : POData;
                POData = !string.IsNullOrEmpty(searchPoDate) ? POData.Where(m => m.PODate != null ? m.PODate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchPoDate.ToUpper()) : false) : POData;
                POData = !string.IsNullOrEmpty(searchSuplier) ? POData.Where(m => m.SupplierId != 0 ? _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchSuplier.ToUpper()): false)   : POData;
                POData = !string.IsNullOrEmpty(searchFreight) ? POData.Where(m => m.Freight.ToString().Contains(searchFreight)) : POData;
                POData = !string.IsNullOrEmpty(searchTotal) ? POData.Where(m => m.Total.ToString().Contains(searchTotal)) : POData;
                POData = !string.IsNullOrEmpty(searchTotalTax) ? POData.Where(m => m.TotalTaxAmount.ToString().Contains(searchTotalTax)) : POData;
                POData = !string.IsNullOrEmpty(searchGrandTotal) ? POData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : POData;
                POData = !string.IsNullOrEmpty(searchStatus) ? POData.Where(m => m.Status != null ? m.Status.ToUpper().Contains(searchStatus.ToUpper()) : false) : POData;


                //recordsTotal = POData.Count();
                //var data = POData.ToList();



                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    POData = POData.Where(m => (m.PODate != null ? m.PODate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper()) : false)
                //                                    || m.PONo.ToString().Contains(searchValue)
                //                                    || m.CSNo.ToString().Contains(searchValue)
                //                                    || m.Total.ToString().Contains(searchValue)
                //                                    || (m.Status != null ? m.Status.ToUpper().Contains(searchValue.ToUpper()) : false)
                //                                    || m.Freight.ToString().Contains(searchValue)
                //                                    || m.TotalTaxAmount.ToString().Contains(searchValue)
                //                                    || m.GrandTotal.ToString().Contains(searchValue)
                //                                    || m.Freight.ToString().Contains(searchValue)
                //                                    || (m.SupplierId != 0 ? _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchValue.ToUpper()) : false)
                //                                  );

                //}
                recordsTotal = POData.Count();
                var data = POData.ToList();
                if (pageSize == -1)
                {
                    data = POData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = POData.Skip(skip).Take(pageSize).ToList();
                }
                List<APPurchaseOrder> Details = new List<APPurchaseOrder>();
                foreach (var grp in data)
                {
                    APPurchaseOrder aP = new APPurchaseOrder();
                    aP = grp;
                    aP.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.SupplierId).Name;
                    aP.Date = grp.PODate.ToString(Helpers.CommonHelper.DateFormat);
                    aP.Qty = grp.APPurchaseOrderItems.Sum(p=>p.Qty);
                    aP.Approve = approve;
                    aP.Unapprove = unApprove;
                    Details.Add(aP);
                }
                data.ForEach(p=>p.APPurchaseOrderItems = null);
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Counter = 0;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x=>x.Resp_Id == resp_Id).Resp_Name;
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            //ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.User = (from u in _dbContext.Users select u).ToList();
            ViewBag.DepartmentName = (from g in _dbContext.GLDivision select g).ToList();
            ViewBag.UOM = _dbContext.AppCompanyConfigs.Where(p=>p.BaseId == 13).ToList();
            //ViewBag.PoType = configValues.GetConfigValues("AP", "Purchase Order Type", companyId);
            //ViewBag.DeliveryTerm = configValues.GetConfigValues("AP", "Delivery Term", companyId);
            //ViewBag.PaymentTerm = configValues.GetConfigValues("AP", "Payment Term", companyId);
            //ViewBag.Supplier = configValues.Supplier(companyId);
            ViewBag.PaymentMode = configValues.GetConfigValues("AP", "Payment Mode", companyId);
            ViewBag.ShippingMode = configValues.GetConfigValues("AP", "Shipping Mode", companyId);
            ViewBag.ImportType = configValues.GetConfigValues("AP", "Import Type", companyId);
            ViewBag.CostTerms = configValues.GetConfigValues("AP", "Cost Terms", companyId);
            ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(a => a.IsDeleted == false).ToList(), "Id", "Description");
            ViewBag.APPurchaseRequisition = _APPurchaseRequisitionRepository.Get(x=>x.IsApproved);
            //ViewBag.POType = configValues.GetConfigValues("AP", "Purchase Order Type", companyId);
            ViewBag.InvItem = _dbContext.InvItems.ToList();
            ViewBag.InvItemCategories = (from c in _dbContext.InvItemCategories select c).ToList();
            ViewBag.AppCountries = new SelectList(_dbContext.AppCountries.OrderBy(c => c.Name).ToList(), "Id", "Name");
            //ViewBag.APPurchaseRequisitionDetails = await  _APPurchaseRequisitionDetailsRepository.GetAllAsync();

            /*ViewBag.APPurchaseRequisitionDetails = (from pr in _dbContext.APPurchaseRequisitionDetails
                                                    where !(from o in _dbContext.APCSRequest select o.PR).Contains(pr.Id)
                                                    orderby pr.Id
                                                    select pr);*/
            ViewBag.APPurchaseRequisitionDetails = _APPurchaseRequisitionDetailsRepository.Get(x => !x.IsCSCreated && !x.IsPOCreated);
            // ViewBag.TaxList = new SelectList( appTaxRepo.GetTaxes(companyId).ToList(),"Id","Name");
            ViewBag.TaxList = appTaxRepo.GetTaxes(companyId);
            if (id == 0)
            {
                ViewBag.EntityState = "Create"; 
                ViewBag.NavbarHeading = "Create Purchase Order";
                ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
                ViewBag.DeliveryTerm = configValues.GetConfigValues("AP", "Delivery Term", companyId);
                ViewBag.Supplier = configValues.Supplier(companyId);
                ViewBag.PaymentTerm = configValues.GetConfigValues("AP", "Payment Term", companyId);
                ViewBag.PoType = configValues.GetConfigValues("AP", "Purchase Order Type", companyId);

                //TempData["PONo"] =  purchaseOrderRepo.PurchaseOrderNo(companyId);
                var model = new APPurchaseOrderViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
                return View(model);
            }
            else
            {
                APPurchaseOrderViewModel model = purchaseOrderRepo.GetById(id);
                ViewBag.OperatingUnit = new SelectList(_dbContext.SysOrganization.Where(x=>x.Organization_Id == model.OperationId), "Organization_Id", "OrgName");
                ViewBag.DeliveryTerm = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == model.DeliveryTermId), "Id", "ConfigValue");
                ViewBag.Supplier = new SelectList(_dbContext.APSuppliers.Where(s => s.Id == model.SupplierId), "Id", "Name");
                ViewBag.PaymentTerm = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == model.PaymentTermId), "Id", "ConfigValue");
                ViewBag.PoType = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == model.POTypeId), "Id", "ConfigValue");

                model.Currencies = AppCurrencyRepo.GetCurrencies();
                APPurchaseOrderItem[] poItems = purchaseOrderRepo.GetPurchaseOrderItems(id);
                ViewBag.Items = poItems;
                model.PurchaseItems = poItems.ToList();
                var po = (from p in _dbContext.APPurchaseOrders where p.Id == id select p).FirstOrDefault();
                model.BrandList = _dbContext.AppCompanyConfigs.Where(x=>x.IsDeleted != true).Select(x => new ListOfValue { Id= x.Id, Name= x.ConfigValue }).ToList();
                model.Currency = po.Currency;
                model.Attachment = po.Attachment;
                model.CurrencyExchangeRate = po.CurrencyExchangeRate;
                TempData["PONo"] = model.PONo;
                if (model.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Purchase Order";
                    ViewBag.TitleStatus = "Created";
                }
                 return  View(model);
            }         
        }

        [HttpPost]
        public async Task<IActionResult> Create(APPurchaseOrderViewModel model,IFormCollection collection, IFormFile Attachment)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            model.Resp_ID = HttpContext.Session.GetInt32("Resp_ID").Value;
            if (model.Id == 0)
            {
                model.CompanyId = companyId;
                model.CreatedBy = userId;
                bool isSuccess = await purchaseOrderRepo.Create(model, collection, Attachment);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Purchase Order has been created successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.CompanyId = companyId;
                model.UpdatedBy = userId;
                bool isSuccess =await purchaseOrderRepo.Update(model, collection, Attachment);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Purchase Order has been updated successfully.";
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
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            bool isSuccess = await purchaseOrderRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Order has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClosePO(int id)
        {
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            bool isSuccess = await purchaseOrderRepo.ClosePO(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Order has been Closed successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ClosePOItems(int id)
        {
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            bool isSuccess = await purchaseOrderRepo.ClosePOItems(id);
            
            if (isSuccess == true)
            {
                
               var message = new {message= "successed" };
                return Ok(message);
            }
            else
            {
                var message = new { message = "Something Went wrong" };
                return Ok(message);
            }
           
        }

        public async Task<IActionResult> Approve(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            var isSuccess = await purchaseOrderRepo.Approve(id, userId);
            if (isSuccess == "Saved")
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Order has been approved successfully";
            }
            else if(isSuccess== "Origin")
            {
                TempData["error"] = "true";
                TempData["message"] = "Please Select Origin then Approve PO";
            }
            else if(isSuccess == "HSCode")
            {
                TempData["error"] = "true";
                TempData["message"] = "Please Select HSCode then Approve PO";
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
            var model = _dbContext.APPurchaseOrders.Where(i => i.Status == "Approved" && !i.IsDeleted && i.CompanyId == companyId).ToList();
            ViewBag.NavbarHeading = "Un-Approve Purchase Order";
            return View(model);
        }

        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            bool isSuccess = await purchaseOrderRepo.UnApproveVoucher(id);
            if (isSuccess == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Purchase Order not found";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Order has been Un-Approved successfully";
            }
            return RedirectToAction(nameof(UnApprove));
        }

        [HttpPost]
        public IActionResult PartialPurchaseOrderItems(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            var model = new APPurchaseOrderViewModel();
            var appTaxRepo = new AppTaxRepo(_dbContext);
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();

            return PartialView("_partialPurchaseOrderItems", model);
        }

        public IActionResult GetItemDetails(int id)
        {
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            dynamic itemDetails = purchaseOrderRepo.GetItemDetails(id);
            return Ok(itemDetails);
        }

        [HttpGet]
        public IActionResult GetOrderItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var item = _dbContext.APPurchaseOrderItems.Include(i => i.PO).Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ViewBag.Counter = id;
            ViewBag.ItemId = item.ItemId;
            var purchaseOrderRepo = new PurchaseOrderRepo(_dbContext);
            APPurchaseOrderViewModel viewModel = purchaseOrderRepo.GetOrderItems(id,itemId);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            viewModel.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            return PartialView("_partialPurchaseOrderItems", viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id, string type)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;


            //List<APPurchaseOrderViewModel> aPPurchaseOrderViewModel = new List<APPurchaseOrderViewModel>();
            //if (type != "Import")
            //{
            //    var PoList = _dbContext.APPurchaseOrderItems.Where(x => x.Id == id).ToList();
            //    foreach (var frp in PoList)
            //    {
            //        APPurchaseOrderViewModel model = new APPurchaseOrderViewModel();
            //        if (frp.BrandId != 0)
            //        {
            //            model.Brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(frp.BrandId)).ConfigValue;
            //        }

            //        aPPurchaseOrderViewModel.Add(model);
            //    }
            //}

            List<ItemListViewModel> itemListViewModel = new List<ItemListViewModel>();
            if (type != "Import")
            {
                var PoList = _dbContext.APPurchaseOrderItems.Where(x => x.Id == id).ToList();
                foreach (var frp in PoList)
                {
                    ItemListViewModel model = new ItemListViewModel();
                   
                    if (frp.BrandId != 0)
                    {
                        model.Brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(frp.BrandId)).ConfigValue;
                    }
                   
                    itemListViewModel.Add(model);
                }
            }



            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=PurchaseOrder&cId=", companyId, "&id={0}");
            var purchaseOrder = _dbContext.APPurchaseOrders.Include(i => i.Supplier).Include(i => i.DeliveryTerm)
                .Include(i => i.PaymentTerm).Include(i => i.POType).Where(i => i.Id == id).FirstOrDefault();
            var purchaseOrderItems = _dbContext.APPurchaseOrderItems
                                .Include(i => i.Item)
                                .Include(i => i.PO)
                                .Where(i => i.POId == id && i.IsDeleted == false)
                                .ToList();
            var tax = purchaseOrderItems.Sum(x => x.TaxAmount);
            var SubTotal = purchaseOrderItems.Sum(x => x.Total);



                ViewBag.TaxAmount = tax;
            ViewBag.SubTotal = SubTotal;
            ViewBag.NavbarHeading = "Purchase Order";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = purchaseOrderItems;
            return View(purchaseOrder);
        }

        public IActionResult SaveLC(APPurchaseOrder model)
        {
            APPurchaseOrder order = _dbContext.APPurchaseOrders.Find(model.Id);

            order.PerformaNo = model.PerformaNo;
            order.PerformaDate = model.PerformaDate;
            order.LCNo = model.LCNo;
            order.LCOpeningDate = model.LCOpeningDate;
            order.LCExpiryDate = model.LCExpiryDate;
            order.LCBank = model.LCBank;
            order.SwiftCode = model.SwiftCode;
            order.CustomerBank = model.CustomerBank;

            _dbContext.APPurchaseOrders.Update(order);
            _dbContext.SaveChanges();
            return Ok(true);
            



        }

        public string GetPOType(int type)
        {

            string name = _dbContext.AppCompanyConfigs.Where(a => a.Id == type && a.IsDeleted == false).Select(b => b.ConfigValue).FirstOrDefault();

            return name;

        }

        public int GetMaxPerFormeNo()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int maxPerformaNo = 1;
            var orders = _dbContext.APPurchaseOrders.Where(c => c.CompanyId == companyId && c.PerformaNo!=0).ToList();
            if (orders.Count > 0)
            {
                maxPerformaNo = orders.Max(o => o.PerformaNo);
                return maxPerformaNo + 1;
            }
            else
            {
                return maxPerformaNo;
            }
        }

        public IActionResult GetPOClose(int poId)
        {
            List<ItemDetailViewModel> itemDetailList = new List<ItemDetailViewModel>();

            ViewBag.POTOClose = new SelectList( _dbContext.APPurchaseOrders.Where(x => x.Status == "Approved").ToList(),"Id","PONo");
            var data = (from f in _dbContext.APPurchaseOrderItems.Include(x=>x.Item)
                        join
                        g in _dbContext.APPurchaseOrders.Include(x => x.Supplier) on f.POId equals g.Id
                        where f.POId == poId && f.IsClosed==false  && g.IsDeleted == false && g.Status == "Approved"
                        select new
                        {
                            f,
                            g
                        }).OrderByDescending(a => a.f.Id).ToList();


          
            
            foreach (var item in data)
            {
                ItemDetailViewModel itemDetailViewModel = new ItemDetailViewModel();
                itemDetailViewModel.PONo = Convert.ToInt32(item.g.PONo);
                itemDetailViewModel.PODate = item.g.PODate.ToString(Helpers.CommonHelper.DateFormat);
                itemDetailViewModel.POQty = item.f.Qty;
                itemDetailViewModel.GRNRate = item.f.Rate;
                itemDetailViewModel.Brand = _dbContext.AppCompanyConfigs.Where(x=>x.Id==item.f.BrandId).Select(x=>x.ConfigValue).FirstOrDefault();
                itemDetailViewModel.ItemId = item.f.ItemId;
                itemDetailViewModel.InvoiceId = item.g.Id;
                itemDetailViewModel.InvoiceItemId = item.f.Id;
                itemDetailViewModel.Category = _dbContext.InvItemCategories.Where(x => x.Id == item.f.Item.CategoryId).Select(x => x.Name).FirstOrDefault();
                itemDetailViewModel.Stock = item.f.Item.StockQty;
                itemDetailViewModel.Average = item.f.Item.AvgRate;
                itemDetailViewModel.ItemDescription = item.f.Item.Name;
                itemDetailViewModel.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == item.f.Item.Unit).ConfigValue;
                
                // itemDetailViewModel.Vendor = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(item.g.VendorName)).Name;
                itemDetailViewModel.Vendor = item.g.Supplier.Name;
                itemDetailViewModel.Phone = item.g.Supplier.Phone1;
                itemDetailList.Add(itemDetailViewModel);
            }

            //APPurchaseOrderViewModel model = new APPurchaseOrderViewModel();
            //model.PO = _dbContext.APPurchaseOrders.Where(x => x.Id == poId).Include(x=>x.Supplier).FirstOrDefault();
            //model.PurchaseItems = _dbContext.APPurchaseOrderItems.Where(x => x.POId == poId).Include(x=>x.Item).ToList();
            

            return Ok(itemDetailList);
        }
        public int GetMaxLCNo()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int maxLCNo = 1;
            var orders = _dbContext.APPurchaseOrders.Where(c => c.CompanyId == companyId && c.LCNo != null).ToList();
            if (orders.Count > 0)
            {
                maxLCNo = orders.Max(o => o.LCNo);
                return maxLCNo + 1;
            }
            else
            {
                return maxLCNo;
            }
        }
       
    }
} 