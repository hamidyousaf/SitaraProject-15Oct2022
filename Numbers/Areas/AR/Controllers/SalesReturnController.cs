using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Vouchers;
using Numbers.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class SalesReturnController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public SalesReturnController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            string configValue = configValues.GetReportPath("Global", "Report Path");
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            IEnumerable<ARInvoice> list = saleReturnRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Sale Returns";
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Counter = 0;
            ViewBag.Customer = configValues.CustomerType(companyId);
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            var configValue = new ConfigValues(_dbContext);
            string ReportConfigValue = configValue.GetReportPath("Global", "Report Path");
            ViewBag.Customer = new SelectList(_dbContext.ARCustomers.Where(a =>/* a.CompanyId == companyId &&*/ a.IsDeleted != true && a.IsActive != false).ToList(), "Id", "Name");
            ViewBag.ReportPath = string.Concat(ReportConfigValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            if (id == 0)
            { 
                //TempData["ReturnNo"] = saleReturnRepo.GetReturnNo(companyId);
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Sale Return";
                var model = new ARSaleReturnViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies= AppCurrencyRepo.GetCurrencies(); ;
                return View(model);
            }
            else
            {
                ARSaleReturnViewModel modelEdit = saleReturnRepo.GetById(id);
                modelEdit.TaxList = appTaxRepo.GetTaxes(companyId);
                modelEdit.Currencies = AppCurrencyRepo.GetCurrencies();
                ViewBag.Id = modelEdit.CustomerId;
                ARInvoiceItem[] returnItems = saleReturnRepo.GetSaleReturnItems(id);
                ViewBag.Items = returnItems;
                //TempData["ReturnNo"] = modelEdit.ReturnNo;
                if (modelEdit.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Sale Return";
                    ViewBag.TitleStatus = "Created";
                }
                return View(modelEdit);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ARSaleReturnViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId"); 
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.ResponsibilityId = resp_Id;
                bool isSuccess = await saleReturnRepo.Create(model, collection, companyId);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Return No. {0} has been created successfully.", saleReturnRepo.MaxSaleReturn(companyId));
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
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.ResponsibilityId = resp_Id;
                bool isSuccess = await saleReturnRepo.Update(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Return No. {0} has been updated successfully.", model.ReturnNo);
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
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            bool isSuccess = await saleReturnRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Return has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult PartialSaleReturnItems(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            var model = new ARSaleReturnViewModel();
            var appTaxRepo = new AppTaxRepo(_dbContext);
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            return PartialView("_partialSaleReturnItems", model);
        }

        public IActionResult GetItemDetails(int id)
        {
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            var itemDetails = saleReturnRepo.GetItemDetails(id);
            return Ok(itemDetails);
        }

        [HttpGet]
        public IActionResult GetReturnItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            var viewModel = saleReturnRepo.GetReturnItems(id, itemId);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Counter = id;
            ViewBag.ItemId = viewModel.ItemId;
            if (viewModel.InvoiceNo==0 || viewModel?.InvoiceNo==null )
            {
                return PartialView("_partialSaleReturnItems", viewModel);
            }
            else 
            { 
                return PartialView("_SaleReturnItems", viewModel); 
            } 
        }
        [HttpGet]
        public IActionResult GetSaleInvoiceItems(int saleInvoiceItemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            var item = saleReturnRepo.GetSaleInvoiceItems(saleInvoiceItemId);
            ViewBag.Counter = saleInvoiceItemId;
            ViewBag.ItemId = item.ItemId;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            return PartialView("_SaleReturnItems", item);
        }
        [HttpPost]
        public IActionResult GetInvoicesToReturnByCustomerId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new SaleReturnRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var invoice = saleReturnRepo.GetInvoicesToReturnByCustomerId(id, skipIds, companyId);
            List<ARSaleReturnViewModel> list = new List<ARSaleReturnViewModel>();
            foreach (var item in invoice)
            {
                var model = new ARSaleReturnViewModel();
                model.SaleReturnId = item.Invoice.Id;
                model.InvoiceItemId = item.Id;
                model.ReturnNo = item.Invoice.InvoiceNo;
                model.ReturnDate = item.Invoice.InvoiceDate;
                model.ItemId = item.ItemId;
                model.ItemName = item.Item.Name;
                model.ItemCode = item.Item.Code;
                model.UOM = configValues.GetUom(item.Item.Unit);
                model.Qty = item.Qty;
                model.Rate = item.Rate;
                model.Amount = item.LineTotal;
                list.Add(model);
            }
            return PartialView("_SaleReturnPopUp", list.ToList());
        }

        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var saleReturnVoucherRepo = new SaleReturnVoucherRepo(_dbContext,HttpContext);
            bool isSuccess = await saleReturnVoucherRepo.Approve(id, companyId, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Return has been approved successfully";
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
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ViewBag.UnApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var voucherRepo = new SaleReturnVoucherRepo(_dbContext,HttpContext);
            var list = voucherRepo.GetApprovedVouchers();
            ViewBag.NavbarHeading = "Un-Approve Sale Return";
            return View(list);
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            var purchaseVoucherRepo = new SaleReturnVoucherRepo(_dbContext, HttpContext);
            var voucher = await purchaseVoucherRepo.UnApproveVoucher(id);
            if (voucher == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Sales Return has been Un-Approved successfully";
            }
            return RedirectToAction(nameof(UnApprove));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=SaleReturn&cId=", companyId, "&id={0}");
            // var viewModel = new ARInvoiceViewModel();
            var invoice = _dbContext.ARInvoices.Include(i => i.Customer).Include(i => i.WareHouse)
            .Where(i => i.Id == id).FirstOrDefault();
            var invoiceItem = _dbContext.ARInvoiceItems
                                .Include(i => i.Item)
                                .Include(i => i.Invoice)
                                .Where(i => i.InvoiceId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Sale Return";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = invoiceItem;
            return View(invoice);
        }
        public IActionResult GetSR()
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
                var searchInvNo = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchInvDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchTotal = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchSalesTaxAmount = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[6][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[7][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var SRData = (from SR in _dbContext.ARInvoices.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select SR);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    SRData = SRData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                SRData = !string.IsNullOrEmpty(searchInvNo) ? SRData.Where(m => m.InvoiceNo.ToString().Contains(searchInvNo)) : SRData;
                SRData = !string.IsNullOrEmpty(searchInvDate) ? SRData.Where(m => m.InvoiceDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchInvDate.ToUpper())) : SRData;
                SRData = !string.IsNullOrEmpty(searchCustomer) ? SRData.Where(m => _dbContext.ARCustomers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.CustomerId)).Name.ToUpper().Contains(searchCustomer.ToUpper())) : SRData;
                SRData = !string.IsNullOrEmpty(searchTotal) ? SRData.Where(m => m.Total.ToString().Contains(searchTotal)) : SRData;
                SRData = !string.IsNullOrEmpty(searchSalesTaxAmount) ? SRData.Where(m => m.SalesTaxAmount.ToString().Contains(searchSalesTaxAmount)) : SRData;
                SRData = !string.IsNullOrEmpty(searchGrandTotal) ? SRData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : SRData;
                SRData = !string.IsNullOrEmpty(searchStatus) ? SRData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : SRData;
                recordsTotal = SRData.Count();
                var data = SRData.ToList();
                if (pageSize == -1)
                {
                    data = SRData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = SRData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARInvoiceViewModel> Details = new List<ARInvoiceViewModel>();
                foreach (var grp in data)
                {
                    ARInvoiceViewModel aRInvoiceViewModel = new ARInvoiceViewModel();
                    aRInvoiceViewModel.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == grp.CustomerId).Name;
                    aRInvoiceViewModel.InvDate = grp.InvoiceDate.ToString(Helpers.CommonHelper.DateFormat);
                    aRInvoiceViewModel.Invoice = grp;
                    aRInvoiceViewModel.Invoice.Approve = approve;
                    aRInvoiceViewModel.Invoice.Unapprove = unApprove;
                    Details.Add(aRInvoiceViewModel);
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