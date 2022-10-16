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
using Numbers.Repository.Vouchers;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class PurchaseReturnController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public PurchaseReturnController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            IEnumerable<APPurchase> list = purchaseReturnRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Purchase Returns";
            string configValues = _dbContext.AppCompanyConfigs
                                           .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                           .Select(c => c.ConfigValue)
                                           .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=PurchaseReturn&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id=");
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext); 
            var configValues = new ConfigValues(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Counter = 0;
            
            ViewBag.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Purchase Return";
                ViewBag.Supplier = configValues.Supplier(companyId);
                TempData["ReturnNo"] = purchaseReturnRepo.GetReturnNo(companyId);
                ViewBag.ReturnMaxNo= purchaseReturnRepo.GetReturnNo(companyId);
                var model = new APPurchaseReturnViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                return View(model);
            }
            else
            {          
                APPurchaseReturnViewModel modelEdit = purchaseReturnRepo.GetById(id);
                modelEdit.TaxList = appTaxRepo.GetTaxes(companyId);
                modelEdit.Currencies = AppCurrencyRepo.GetCurrencies();
                ViewBag.Id = modelEdit.SupplierId;
                ViewBag.Supplier = configValues.SupplierById(companyId, modelEdit.SupplierId);
                List<APPurchaseItem> items = purchaseReturnRepo.GetPurchaseReturnItems(id);
                ViewBag.Items = items;
               // ViewBag.ItemCategory= _dbContext.InvItems.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList();
                ViewBag.ItemCategory= new SelectList(_dbContext.InvItems.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
                ViewBag.Tax= new SelectList(_dbContext.AppTaxes.Where(x => x.CompanyId == companyId && x.IsDeleted == false).OrderBy(x=>x.Name.Contains("NO TAX")).ToList(), "Id", "Name");

                modelEdit.APPurchaseItemList = items;
                TempData["ReturnNo"] = modelEdit.ReturnNo;
                if (modelEdit.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Purchase Return";
                    ViewBag.TitleStatus = "Created";
                }
                ViewBag.Mode = "Update";
                return View(modelEdit);
            }
        }


        public int GetMax()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            int maxPurchaseReturn = 1;
            maxPurchaseReturn = purchaseReturnRepo.GetReturnNo(companyId);
            return maxPurchaseReturn;
        }







        [HttpPost]
        public async Task<IActionResult> Create(APPurchaseReturnViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CompanyId = companyId;
                model.CreatedBy = userId;
                model.Resp_Id = resp_Id;
                model.ReturnNo = GetMax();
                bool isSuccess = await purchaseReturnRepo.Create(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Purchase Return "+ model.ReturnNo + " has been created successfully.";
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
                bool isSuccess = await purchaseReturnRepo.Update(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Purchase Return "+ model.ReturnNo + " has been updated successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpPost]
        public IActionResult PartialPurchaseReturnItems(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            var model = new APPurchaseReturnViewModel();
            var appTaxRepo = new AppTaxRepo(_dbContext);
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            return PartialView("_partialPurchaseReturnItems", model);
        }
        public IActionResult GetItemDetails(int id)
        {
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            var itemDetails = purchaseReturnRepo.GetItemDetails(id);
            return Ok(itemDetails);
        }
        [HttpGet]
        public IActionResult GetReturnItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            var viewModel = purchaseReturnRepo.GetReturnItems(id, itemId);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Counter = id;
            ViewBag.ItemId = viewModel.ItemId;
            if (viewModel.InvoiceNo == 0 || viewModel?.InvoiceNo == null)
            {
                return PartialView("_partialPurchaseReturnItems", viewModel);
            }
            else
            {
                return PartialView("_PurchaseReturnItems", viewModel);
            }
                
        }

        public async Task<IActionResult> Delete(int id)
        {
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            bool isSuccess =await purchaseReturnRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Return has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetPurchaseInvoiceItems(int purchaseInvoiceItemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = purchaseReturnRepo.GetPurchaseInvoiceItems(purchaseInvoiceItemId);
            ViewBag.Counter = purchaseInvoiceItemId;
            ViewBag.ItemId = item.ItemId;
            item.TaxList = appTaxRepo.GetTaxesById(companyId, item.TaxId);
            foreach (var tax in item.TaxList)
            {
                item.SalesTaxPercentage = tax.SalesTaxPercentage;
            }
            item.ItemList = _dbContext.InvItems.Where(x => x.Id == item.ItemId && x.CompanyId == companyId&&x.IsDeleted==false).ToList();
            return Ok(item);
        }
        [HttpPost]
        public IActionResult GetInvoicesToReturnBySupplierId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var purchaseReturnRepo = new PurchaseReturnRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var invoice = purchaseReturnRepo.GetInvoicesToReturnBySupplierId(id, skipIds, companyId);
            List<APPurchaseReturnViewModel> list = new List<APPurchaseReturnViewModel>();
            foreach (var item in invoice)
            {
                var model = new APPurchaseReturnViewModel();
                //model.PurchaseInvoiceId = item.Purchase.Id;
                model.PurchaseInvoiceItemId = item.Id;
                model.ReturnNo = item.Purchase.PurchaseNo;
                model.ReturnDate = item.Purchase.PurchaseDate;
                model.ItemId = item.ItemId;
                model.ItemName = item.Item.Name;
                model.ItemCode = item.Item.Code;
                model.UOM = configValues.GetUom(item.Item.Unit);
                model.Qty = item.Qty;
                model.Rate = item.Rate;
                model.TaxId = item.TaxId;
                model.Amount = item.LineTotal;
                list.Add(model);
            }
            return PartialView("_PurchaseReturnPopUp", list.ToList());
        }

        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var purchaseReturnVoucherRepo = new PurchaseReturnVoucherRepo(_dbContext, HttpContext);
            bool isSuccess = await purchaseReturnVoucherRepo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Return has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong while Approving.";
            }
            return RedirectToAction(nameof(Index));
        }
        public ActionResult UnApprove()
        {
            var purchaseVoucherRepo = new PurchaseReturnRepo(_dbContext, HttpContext);
            var list = purchaseVoucherRepo.GetApprovedVouchers();
            ViewBag.NavbarHeading = "Un-Approve Purchase Return";
            return View(list);
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            var purchaseVoucherRepo = new PurchaseReturnRepo(_dbContext, HttpContext);
            var voucher = await purchaseVoucherRepo.UnApproveVoucher(id);
            if (voucher == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Return has been Un-Approved successfully";
            }
            return RedirectToAction(nameof(UnApprove));
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

            var purchase = _dbContext.APPurchases.Include(i => i.Supplier).Include(i => i.WareHouse)
            .Where(i => i.Id == id).FirstOrDefault();
            var purchaseItem = _dbContext.APPurchaseItems
                                .Include(i => i.Item)
                                .Include(i => i.Purchase)
                                .Where(i => i.PurchaseId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Purchase Return";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = purchaseItem;
            return View(purchase);
        }

        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            try
            {
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var searchPurchaseNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchPurchaseDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchSupplier = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchTotal = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchTotalSaleTax = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var PurchasesData = (from Purchases in _dbContext.APPurchases.Include(p => p.Supplier).Where(p => p.IsDeleted == false && p.TransactionType == "Purchase Return" && p.CompanyId == companyId && p.Resp_ID == resp_Id) select Purchases);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PurchasesData = PurchasesData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PurchasesData = PurchasesData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                PurchasesData = !string.IsNullOrEmpty(searchPurchaseNo) ? PurchasesData.Where(m => m.PurchaseNo.ToString().Contains(searchPurchaseNo)) : PurchasesData;
                PurchasesData = !string.IsNullOrEmpty(searchPurchaseDate) ? PurchasesData.Where(m => m.PurchaseDate != null ? m.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchPurchaseDate.ToUpper()) : false) : PurchasesData;
                PurchasesData = !string.IsNullOrEmpty(searchSupplier) ? PurchasesData.Where(m => m.SupplierId != 0 ? _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchSupplier.ToUpper()) : false) : PurchasesData;
                PurchasesData = !string.IsNullOrEmpty(searchTotal) ? PurchasesData.Where(m => m.Total.ToString().Contains(searchTotal)) : PurchasesData;
                PurchasesData = !string.IsNullOrEmpty(searchTotalSaleTax) ? PurchasesData.Where(m => m.TotalSalesTaxAmount.ToString().Contains(searchTotalSaleTax)) : PurchasesData;
                PurchasesData = !string.IsNullOrEmpty(searchGrandTotal) ? PurchasesData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : PurchasesData;
                PurchasesData = !string.IsNullOrEmpty(searchStatus) ? PurchasesData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : PurchasesData;

                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    PurchasesData = PurchasesData.Where(m => m.PurchaseNo.ToString().Contains(searchValue)
                //                                    || m.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Supplier.Name.ToString().Contains(searchValue)
                //                                    || m.Total.ToString().Contains(searchValue)
                //                                    || m.TotalSalesTaxAmount.ToString().Contains(searchValue)
                //                                    || m.GrandTotal.ToString().Contains(searchValue)
                //                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                  );

                //}
                recordsTotal = PurchasesData.Count();
                var data = PurchasesData.ToList();
                if (pageSize == -1)
                {
                    data = PurchasesData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = PurchasesData.Skip(skip).Take(pageSize).ToList();
                }
                List<APPurchaseViewModel> Details = new List<APPurchaseViewModel>();
                foreach (var grp in data)
                {
                    APPurchaseViewModel aPPurchaseViewModel = new APPurchaseViewModel();
                    aPPurchaseViewModel.PurchaseDate = grp.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat);
                    aPPurchaseViewModel.APPurchases = grp;
                    aPPurchaseViewModel.APPurchases.Approve = approve;

                    Details.Add(aPPurchaseViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}