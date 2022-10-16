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
    [Area("Greige")]
    [Authorize]
    public class GRPaymentController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public GRPaymentController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            string configValues = _dbContext.AppCompanyConfigs
              .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
              .Select(c => c.ConfigValue)
              .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            IEnumerable<GRPayment> paymentList = GRPaymentRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Greige Payment Invoices";
            return View(paymentList);
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Counter = 0;
            ViewBag.Supplier = configValues.Supplier(companyId);
            ViewBag.SupplierPayment = configValues.Supplier(companyId);
            ViewBag.PaymentMode = configValues.GetConfigValues("AP","Payment Mode", companyId);
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Payment";
                TempData["PaymentNo"] = GRPaymentRepo.GetPaymentNo(companyId);
                ViewBag.PurchaseNumber = TempData["PaymentNo"];
                var model = new GRPaymentViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                return View(model);
            }
            else
            {
                GRPaymentViewModel model = GRPaymentRepo.GetById(id);
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                TempData["PaymentNo"] = model.PaymentNo;
                GRPaymentInvoice[] invoices = GRPaymentRepo.GetPaymentInvoices(id);
                ViewBag.Invoices = invoices;
                ViewBag.Id = model.SupplierId;
                if (model.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Payment";
                    ViewBag.TitleStatus = "Created";
                }
                return View(model);
            }  
        }

     

        [HttpPost]
        public async Task<IActionResult> Create(GRPaymentViewModel model, IFormCollection collection, IFormFile Attachment)
        {
           model.Resp_ID = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CompanyId = companyId;
                model.CreatedBy = userId;
                model.PaymentNo = GRPaymentRepo.GetPaymentNo(companyId);
                bool isSuccess = await GRPaymentRepo.Create(model, collection, Attachment);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Greige Supplier Payment "+ Convert.ToString(model.PaymentNo) + " has been Created successfully.";
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
                bool isSuccess = await GRPaymentRepo.Update(model, collection, Attachment);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Greige Supplier Payment " + Convert.ToString(model.PaymentNo) + "  has been Updated successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult GetPaymentInvoices(int id, int invoiceId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var viewModel = GRPaymentRepo.GetPaymentInvoices(id, invoiceId);
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Counter = id;
            //ViewBag.InvoiceId = viewModel.InvoiceId;
            return PartialView("_PaymentInvoices", viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            bool isSuccess =await GRPaymentRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige Supplier Payment has been Deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetPurchaseInvoice(int purchaseItemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            var item = GRPaymentRepo.GetPurchaseInvoice(purchaseItemId, companyId);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Counter = purchaseItemId;
            return PartialView("_PaymentInvoices", item);
        }

        [HttpPost]
        public IActionResult GetUnpaidInvoicesBySupplierId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            var invoice = (from a in _dbContext.GRGRNInvoices.Where(i => !skipIds.Contains(i.Id)).ToList()
                           join c in _dbContext.GRGRNInvoiceDetails on a.Id equals c.GRNId
                           join b in _dbContext.GRGRNS.Where(i => i.VendorId == id && i.CompanyId == companyId) on a.GRNId equals b.Id
                           select new
                           {
                               a.Id,
                               a.PurchaseNo,
                               PurchaseDate = a.CreatedDate,
                               Total = c.NetPayableAmount,
                               TotalSalesTaxAmount = c.NetPayableAmount*17/100,
                               TotalExciseTaxAmount = 0,
                               GrandTotal = c.NetPayableAmount,
                               PaymentTotal = _dbContext.GRPaymentInvoices.Where(x=> x.InvoiceId == a.Id).Select(x=> x.PaymentAmount).Sum()
                           }).ToList();
            List<GRPaymentViewModel> list = new List<GRPaymentViewModel>();
            foreach (var item in invoice)
            {
                var model = new GRPaymentViewModel();
                model.InvoiceId = item.Id;
                model.InvoiceNo = item.PurchaseNo;
                model.InvoiceDate = item.PurchaseDate;
                model.LineTotal = item.Total;
                model.TaxAmount = item.TotalSalesTaxAmount;
                model.ExciseTaxAmount = item.TotalExciseTaxAmount;
                model.InvoiceAmount = item.Total+item.TotalSalesTaxAmount+item.TotalExciseTaxAmount;
                model.PaymentTotal = Convert.ToDecimal(item.PaymentTotal);
                model.Balance = Convert.ToDecimal(item.Total + item.TotalSalesTaxAmount + item.TotalExciseTaxAmount) -Convert.ToDecimal(item.PaymentTotal);
                list.Add(model);
            }
            return PartialView("_partialPaymentInvoices", list.ToList());
        }

        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var paymentVoucherRepo = new GRPaymentRepo(_dbContext, HttpContext);
            bool isSuccess = await paymentVoucherRepo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige Supplier Payment has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Greige Something went wrong and Payment have not Approved.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UnApprove()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            var model = GRPaymentRepo.GetApprovedPayments(companyId);
            ViewBag.NavbarHeading = "Un-Approve Supplier Payment";
            return View(model);
        }

        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var GRPaymentRepo = new GRPaymentRepo(_dbContext);
            var receipt = await  GRPaymentRepo.UnApproveVoucher(id, companyId);
            if (receipt == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Greige Supplier Payment not Approved";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige Supplier Payment has been Un-Approved successfully";
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
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=Payment&cId=", companyId, "&id={0}");

            var payment = _dbContext.GRPayments.Include(i => i.Supplier).Include(i => i.BankCashAccount)
                .Include(i => i.PaymentMode).Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();
            var paymentInvoices = _dbContext.GRPaymentInvoices
                                .Include(i => i.Payment)
                                .Where(i => i.PaymentId == id && i.IsDeleted == false && i.CompanyId==companyId)
                                .ToList();
            ViewBag.NavbarHeading = "Payment";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = paymentInvoices;
            return View(payment);
        }

        public IActionResult   GetList()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
             .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
             .Select(c => c.ConfigValue)
             .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            try
            {
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var PaymentsData = (from Payments in _dbContext.GRPayments.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select Payments);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PaymentsData = PaymentsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    PaymentsData = PaymentsData.Where(m => m.PaymentNo.ToString().Contains(searchValue)
                                                    || m.PaymentDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                                                    || _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchValue.ToUpper())
                                                  
                                                    || m.TotalPaidAmount.ToString().Contains(searchValue)
                                                    || m.GrandTotal.ToString().Contains(searchValue)
                                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                                                  );
                }
                //recordsTotal = PaymentsData.Count();

                recordsTotal = PaymentsData.Count();
                var data = PaymentsData.ToList();
                if (pageSize == -1)
                {
                    data = PaymentsData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = PaymentsData.Skip(skip).Take(pageSize).ToList();
                }

                //var data = PaymentsData.Skip(skip).Take(pageSize).ToList();
                List<GRPaymentViewModel> Details = new List<GRPaymentViewModel>();
                foreach (var grp in data)
                {
                    GRPaymentViewModel GRPaymentViewModel = new GRPaymentViewModel();
                    GRPaymentViewModel.SupplierName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.SupplierId)).Name;
                    GRPaymentViewModel.PayDate = grp.PaymentDate.ToString(Helpers.CommonHelper.DateFormat);
                    GRPaymentViewModel.Payment = grp;
                    GRPaymentViewModel.Payment.Approve = approve;
                    GRPaymentViewModel.Payment.Unapprove = unApprove;

                    Details.Add(GRPaymentViewModel);

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