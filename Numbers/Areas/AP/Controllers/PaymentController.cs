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
    public class PaymentController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public PaymentController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var paymentRepo = new PaymentRepo(_dbContext);
            string configValues = _dbContext.AppCompanyConfigs
              .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
              .Select(c => c.ConfigValue)
              .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            IEnumerable<APPayment> paymentList = paymentRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Payment Invoices";
            return View(paymentList);
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var paymentRepo = new PaymentRepo(_dbContext);
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
                TempData["PaymentNo"] = paymentRepo.GetPaymentNo(companyId);
                ViewBag.PurchaseNumber = TempData["PaymentNo"];
                var model = new APPaymentViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                return View(model);
            }
            else
            {
                APPaymentViewModel model = paymentRepo.GetById(id);
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                TempData["PaymentNo"] = model.PaymentNo;
                APPaymentInvoice[] invoices = paymentRepo.GetPaymentInvoices(id);
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
        public async Task<IActionResult> Create(APPaymentViewModel model, IFormCollection collection, IFormFile Attachment)
        {
           model.Resp_ID = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var paymentRepo = new PaymentRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CompanyId = companyId;
                model.CreatedBy = userId;
                model.PaymentNo = paymentRepo.GetPaymentNo(companyId);
                bool isSuccess = await paymentRepo.Create(model, collection, Attachment);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Supplier Payment "+ Convert.ToString(model.PaymentNo) + " has been Created successfully.";
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
                bool isSuccess = await paymentRepo.Update(model, collection, Attachment);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Supplier Payment has been Updated successfully.";
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
            var paymentRepo = new PaymentRepo(_dbContext);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var viewModel = paymentRepo.GetPaymentInvoices(id, invoiceId);
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Counter = id;
            //ViewBag.InvoiceId = viewModel.InvoiceId;
            return PartialView("_PaymentInvoices", viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var paymentRepo = new PaymentRepo(_dbContext);
            bool isSuccess =await paymentRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Supplier Payment has been Deleted successfully.";
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
            var paymentRepo = new PaymentRepo(_dbContext);
            var item = paymentRepo.GetPurchaseInvoice(purchaseItemId);
            var appTaxRepo = new AppTaxRepo(_dbContext);
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Counter = purchaseItemId;
            return PartialView("_PaymentInvoices", item);
        }

        [HttpPost]
        public IActionResult GetUnpaidInvoicesBySupplierId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var paymentRepo = new PaymentRepo(_dbContext);
            var invoice = paymentRepo.GetUnpaidInvoicesBySupplierId(id, skipIds,companyId);
            List<APPaymentViewModel> list = new List<APPaymentViewModel>();
            foreach (var item in invoice)
            {
                var model = new APPaymentViewModel();
                model.InvoiceId = item.Id;
                model.InvoiceNo = item.PurchaseNo;
                model.InvoiceDate = item.PurchaseDate;
                model.LineTotal = item.Total;
                model.TaxAmount = item.TotalSalesTaxAmount;
                model.ExciseTaxAmount = item.TotalExciseTaxAmount;
                model.InvoiceAmount = item.GrandTotal;
                model.PaymentTotal = item.PaymentTotal;
                model.Balance = item.GrandTotal;
                list.Add(model);
            }
            return PartialView("_partialPaymentInvoices", list.ToList());
        }

        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var paymentVoucherRepo = new PaymentVoucherRepo(_dbContext, HttpContext);
            bool isSuccess = await paymentVoucherRepo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Supplier Payment has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong and Payment have not Approved.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UnApprove()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var paymentRepo = new PaymentRepo(_dbContext);
            var model = paymentRepo.GetApprovedPayments(companyId);
            ViewBag.NavbarHeading = "Un-Approve Supplier Payment";
            return View(model);
        }

        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var paymentRepo = new PaymentRepo(_dbContext);
            var receipt = await  paymentRepo.UnApproveVoucher(id, companyId);
            if (receipt == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Supplier Payment not Approved";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Supplier Payment has been Un-Approved successfully";
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

            var payment = _dbContext.APPayments.Include(i => i.Supplier).Include(i => i.BankCashAccount)
                .Include(i => i.PaymentMode).Where(i => i.Id == id).FirstOrDefault();
            var paymentInvoices = _dbContext.APPaymentInvoices
                                .Include(i => i.Payment)
                                .Where(i => i.PaymentId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Payment";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = paymentInvoices;
            return View(payment);
        }

        public IActionResult GetList()
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

                var searchPaymentNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchPaymentDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchSupplier = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchPaymentMode = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchTotalPaid = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();



                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var PaymentsData = (from Payments in _dbContext.APPayments.Where(x => x.IsDeleted != true && x.CompanyId == companyId ).Include(a => a.PaymentMode) select Payments);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PaymentsData = PaymentsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PaymentsData = PaymentsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                PaymentsData = !string.IsNullOrEmpty(searchPaymentNo) ? PaymentsData.Where(m => m.PaymentNo.ToString().Contains(searchPaymentNo)) : PaymentsData;
                PaymentsData = !string.IsNullOrEmpty(searchPaymentDate) ? PaymentsData.Where(m => m.PaymentDate != null ? m.PaymentDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchPaymentDate.ToUpper()) : false) : PaymentsData;
                PaymentsData = !string.IsNullOrEmpty(searchSupplier) ? PaymentsData.Where(m => m.SupplierId != 0 ? _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchSupplier.ToUpper()) : false) : PaymentsData;
                PaymentsData = !string.IsNullOrEmpty(searchPaymentMode) ? PaymentsData.Where(m => m.PaymentMode.ToString().ToUpper().Contains(searchPaymentMode.ToUpper())) : PaymentsData;
                PaymentsData = !string.IsNullOrEmpty(searchTotalPaid) ? PaymentsData.Where(m => m.TotalPaidAmount.ToString().Contains(searchTotalPaid)) : PaymentsData;
                PaymentsData = !string.IsNullOrEmpty(searchGrandTotal) ? PaymentsData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : PaymentsData;
                PaymentsData = !string.IsNullOrEmpty(searchStatus) ? PaymentsData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : PaymentsData;


                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    PaymentsData = PaymentsData.Where(m => m.PaymentNo.ToString().Contains(searchValue)
                //                                    || m.PaymentDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                    || _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.PaymentMode.ConfigValue.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.TotalPaidAmount.ToString().Contains(searchValue)
                //                                    || m.GrandTotal.ToString().Contains(searchValue)
                //                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                  );
                //}
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
                List<APPaymentViewModel> Details = new List<APPaymentViewModel>();
                foreach (var grp in data)
                {
                    APPaymentViewModel aPPaymentViewModel = new APPaymentViewModel();
                    aPPaymentViewModel.SupplierName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.SupplierId)).Name;
                    aPPaymentViewModel.PayDate = grp.PaymentDate.ToString(Helpers.CommonHelper.DateFormat);
                    aPPaymentViewModel.Payment = grp;
                    aPPaymentViewModel.Payment.Approve = approve;
                    aPPaymentViewModel.Payment.Unapprove = unApprove;

                    Details.Add(aPPaymentViewModel);

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