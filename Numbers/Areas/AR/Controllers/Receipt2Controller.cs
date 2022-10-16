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
using Numbers.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class Receipt2Controller : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public Receipt2Controller(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public  async Task<IActionResult> Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var receiptRepo = new ReceiptRepo(_dbContext);
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportUrl = configValues;
            IEnumerable<ARReceipt> list =await receiptRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Receipt Invoices";
            return View(list);
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var receiptRepo = new ReceiptRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Counter = 0;
            ViewBag.Customer = configValues.CustomerType(companyId);
            ViewBag.PaymentMode = configValues.GetConfigValues("AP", "Payment Mode", companyId);
            ViewBag.Custmer = new SelectList(_dbContext.ARCustomers.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.IsActive == true).ToList(), "Id", "Name");
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Receipt";
                TempData["ReceiptNo"] = receiptRepo.GetReceiptNo(companyId);
                var model = new ARReceiptViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                return View(model);
            }
            else
            {
                ARReceiptViewModel model = receiptRepo.GetById(id);
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                TempData["ReceiptNo"] = model.ReceiptNo;
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                ARReceiptInvoice[] invoices = receiptRepo.GetReceiptInvoices(id);
                ViewBag.Invoices = invoices;
                ViewBag.Id = model.CustomerId;
                if (model.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Receipt";
                    ViewBag.TitleStatus = "Created";
                }
                return View(model);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(ARReceiptViewModel model, IFormCollection collection, IFormFile Attachment)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId"); 
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var receiptRepo = new ReceiptRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CompanyId = companyId;
                model.CreatedBy = userId;
                model.ResponsibilityId = resp_Id;
                bool isSuccess = await receiptRepo.Create(model, collection, Attachment,companyId);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Receipt has been Created successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while Creating.";
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.CompanyId = companyId;
                model.UpdatedBy = userId;
                model.ResponsibilityId = resp_Id;
                bool isSuccess = await receiptRepo.Update(model, collection, Attachment);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Receipt has been Updated successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while Updating.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var receiptRepo = new ReceiptRepo(_dbContext);
            bool isSuccess = await receiptRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Receipt has been Deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong while Deleting.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetReceiptInvoices(int id, int invoiceId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var receiptRepo = new ReceiptRepo(_dbContext);         
            var viewModel = receiptRepo.GetReceiptInvoices(id, invoiceId);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Counter = id;
            return PartialView("_ReceiptInvoices", viewModel);
        }

        [HttpGet]
        public IActionResult GetSaleInvoice(int saleInvoiceItemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var receiptRepo = new ReceiptRepo(_dbContext);
            var item = receiptRepo.GetSaleInvoice(saleInvoiceItemId);
            ViewBag.Counter = saleInvoiceItemId;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            return PartialView("_ReceiptInvoices", item);
        }
        [HttpPost]
        public IActionResult GetUnpaidInvoicesByCustomerId(int id, int[]skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var receiptRepo = new ReceiptRepo(_dbContext);
            var invoice = receiptRepo.GetUnpaidInvoicesByCustomerId(id,skipIds, companyId);
            List<ARReceiptViewModel> list = new List<ARReceiptViewModel>();
            foreach(var item in invoice)
            {
                var model = new ARReceiptViewModel();
                model.InvoiceId = item.Id;
                model.InvoiceNo = item.InvoiceNo;
                model.InvoiceDate = item.InvoiceDate;
                model.LineTotal = item.Total;
                model.TaxAmount = item.SalesTaxAmount;
                model.ExciseTaxAmount = item.ExciseTaxAmount;
                model.InvoiceAmount = item.GrandTotal;
                model.ReceiptTotal = item.ReceiptTotal;
                model.Balance = item.FreightAmount;
                list.Add(model);
            }
            return PartialView("_partialReceiptInvoices", list.ToList());
        }
        public async Task<IActionResult> Approve(int id)
        {            
            try
            {
                var repo = await new ReceiptRepo(_dbContext, HttpContext).Approve(id);
                if (repo == true)
                {
                    //On approval updating Invoice
                    TempData["error"] = "false";
                    TempData["message"] = "Receipt has been approved successfully";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Receipt was not Approved";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult UnApprove()
        {
            var model = new ReceiptRepo(_dbContext,HttpContext).GetApprovedReceipts();
            ViewBag.NavbarHeading = "Un-Approve Receipt";
            return View(model);
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            var receipt = await new ReceiptRepo(_dbContext, HttpContext).UnApproveVoucher(id);
            if (receipt == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Voucher not Approved";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Receipt has been Un-Approved successfully";
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
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=Receipt&cId=", companyId, "&id={0}");
            var receipt = _dbContext.ARReceipts.Include(i => i.Customer).Include(i => i.BankCashAccount)
                .Include(i => i.PaymentMode).Where(i => i.Id == id).FirstOrDefault();
            var receiptInvoices = _dbContext.ARReceiptInvoices
                                .Include(i => i.Receipt)
                                .Where(i => i.ReceiptId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Receipt";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = receiptInvoices;
            return View(receipt);
        }

        public IActionResult GetReceiptList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchRecNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchRecDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchPaymentMode = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchTotalReceivedAmount = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var ReceiptsData = (from Receipts in _dbContext.ARReceipts.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select Receipts);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    ReceiptsData = ReceiptsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                ReceiptsData = !string.IsNullOrEmpty(searchRecNo) ? ReceiptsData.Where(m => m.ReceiptNo.ToString().Contains(searchRecNo)) : ReceiptsData;
                ReceiptsData = !string.IsNullOrEmpty(searchRecDate) ? ReceiptsData.Where(m => m.ReceiptDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchRecDate.ToUpper())) : ReceiptsData;
                ReceiptsData = !string.IsNullOrEmpty(searchCustomer) ? ReceiptsData.Where(m => _dbContext.ARCustomers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.CustomerId)).Name.ToUpper().Contains(searchCustomer.ToUpper())) : ReceiptsData;
                ReceiptsData = !string.IsNullOrEmpty(searchPaymentMode) ? ReceiptsData.Where(m => _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(m.PaymentModeId)).ConfigValue.ToUpper().Contains(searchPaymentMode.ToUpper())) : ReceiptsData;
                ReceiptsData = !string.IsNullOrEmpty(searchTotalReceivedAmount) ? ReceiptsData.Where(m => m.TotalReceivedAmount.ToString().Contains(searchTotalReceivedAmount.ToString())) : ReceiptsData;
                ReceiptsData = !string.IsNullOrEmpty(searchGrandTotal) ? ReceiptsData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : ReceiptsData;
                ReceiptsData = !string.IsNullOrEmpty(searchStatus) ? ReceiptsData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : ReceiptsData;

                recordsTotal = ReceiptsData.Count();
                var data = ReceiptsData.Skip(skip).Take(pageSize).ToList();
                if (pageSize == -1)
                {
                    data = ReceiptsData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = ReceiptsData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARReceiptViewModel> Details = new List<ARReceiptViewModel>();
                foreach (var grp in data)
                {
                    ARReceiptViewModel aRReceiptViewModel = new ARReceiptViewModel();
                    aRReceiptViewModel.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == grp.CustomerId).Name;
                    aRReceiptViewModel.PaymentType = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == grp.PaymentModeId).ConfigValue;
                    aRReceiptViewModel.RecDate = grp.ReceiptDate.ToString(Helpers.CommonHelper.DateFormat);
                    aRReceiptViewModel.Receipt = grp;
                    Details.Add(aRReceiptViewModel);
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