using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class CustomerDiscountAdjustmentOldController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly ARCustomerDiscountAdjustmentRepository _ARCustomerDiscountAdjustmentRepository;
        private readonly ARCustomerAdjustmentItemRepository _ARCustomerAdjustmentItemRepository;
        public CustomerDiscountAdjustmentOldController(NumbersDbContext context, ARCustomerDiscountAdjustmentRepository ARCustomerDiscountAdjustmentRepository, ARCustomerAdjustmentItemRepository ARCustomerAdjustmentItemRepository)
        {
            _dbContext = context;
            _ARCustomerAdjustmentItemRepository = ARCustomerAdjustmentItemRepository;
            _ARCustomerDiscountAdjustmentRepository = ARCustomerDiscountAdjustmentRepository;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Customer Discount Adjustment";
            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.TransactionType = configValues.GetConfigValues("AR", "Transaction Type", companyId);
            ViewBag.Items = _dbContext.ARCustomerDiscountAdjustment.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.IsActive == true);
            ViewBag.ARCustomer = new SelectList(_dbContext.ARCustomers.Where(p => p.IsDeleted != true && p.IsActive == true).OrderBy(c => c.Name).ToList(), "Id", "Name");
            var DiscountList = _dbContext.ARDiscount.Include(x => x.Customer_).Where(x => x.IsActive && x.IsClosed != true && x.IsDeleted == false).ToList();

            List<CustomerDiscountAdjustmentVM> list = new List<CustomerDiscountAdjustmentVM>();
            foreach (var item in DiscountList)
            {
                var record = new CustomerDiscountAdjustmentVM();
                //model.PurchaseInvoiceId = item.Purchase.Id;
                record.ARDiscountId = item.Id;
                record.CategoryId = item.ItemCategory_Id;
                record.CategoryDesc = _dbContext.InvItemCategories.Where(x => x.Id == item.ItemCategory_Id).Select(x => x.Name).FirstOrDefault();
                record.Customer_Id = item.Customer_Id;
                record.CustomerDesc = _dbContext.ARCustomers.Where(x => x.Id == item.Customer_Id).Select(x => x.Name).FirstOrDefault();
                record.SalesPersonId = item.Customer_.SalesPersonId;
                record.SalesPersonDesc = _dbContext.ARSalePerson.Where(x => x.ID == item.Customer_.SalesPersonId).Select(x => x.Name).FirstOrDefault();
                record.CityId = item.Customer_.CityId;
                record.CityDesc = _dbContext.AppCities.Where(x => x.Id == item.Customer_.CityId).Select(x => x.Name).FirstOrDefault();
                record.GrandTotal = item.GrandTotal;
                record.IsClosed = item.IsClosed ?? false;
                var dis = _dbContext.ARCustomerAdjustmentItem.LastOrDefault(x => x.ARDiscount_Id == record.ARDiscountId && x.DiscountBalance != 0);
                if (dis == null)
                {
                    record.DiscountBalance = item.GrandTotal;
                    record.Utilized = 0;
                }
                else
                {
                    record.DiscountBalance = dis.DiscountBalance;
                    record.Utilized = dis.Utilized;
                }

                list.Add(record);
            }
            ViewBag.DiscountList = list;
            CustomerDiscountAdjustmentVM model = new CustomerDiscountAdjustmentVM();
            if (id == 0)
            {

                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Customer Discount Adjustment";
                return View(model);
            }
            else
            {
                CustomerDiscountAdjustmentVM record = new CustomerDiscountAdjustmentVM();
                var aRCustomerAdjustmentItem = _dbContext.ARCustomerAdjustmentItem.Where(x => x.DiscountAdjustment_Id == id).ToList();
                var aRCustomerDiscountAdjustment = _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x => x.Id == id);
                record.CustomerDiscountAdjustment = aRCustomerDiscountAdjustment;
                record.CustomerAdjustmentItem = aRCustomerAdjustmentItem;
                foreach (var x in record.CustomerAdjustmentItem)
                {
                    x.CustomerName = _dbContext.ARCustomers?.FirstOrDefault(y => y.Id == x.Customer_Id)?.Name;
                }
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Customer Discount Adjustment";
                ViewBag.TitleStatus = "Created";
                model = record;
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerDiscountAdjustmentVM customerDiscountAdjustmentVM, IFormFile File, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int respId = HttpContext.Session.GetInt32("Resp_ID").Value;
            var configValues = new ConfigValues(_dbContext);
            var TransactionTypeList = configValues.GetConfigValues("AR", "Transaction Type", companyId);

            if (customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id == 0)
            {
                try
                {
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.CreatedBy = userId;
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.CreatedDate = DateTime.UtcNow;
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.CompanyId = companyId;
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.IsActive = true;
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.IsDeleted = false;
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Status = "Created";
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Resp_Id = respId;
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionNo = this.MaxTransNo(companyId);

                    await _ARCustomerDiscountAdjustmentRepository.CreateAsync(customerDiscountAdjustmentVM.CustomerDiscountAdjustment);
                    List<ARCustomerAdjustmentItem> Details = new List<ARCustomerAdjustmentItem>();
                    for (int i = 0; i < collection["DiscountId"].Count; i++)
                    {
                        ARCustomerAdjustmentItem aRCustomerAdjustmentItem = new ARCustomerAdjustmentItem();

                        aRCustomerAdjustmentItem.DiscountAmount = Convert.ToDecimal(collection["GrandTotal"][i]);
                        aRCustomerAdjustmentItem.PaidAmount = Convert.ToDecimal(collection["PaidAmount"][i]);
                        aRCustomerAdjustmentItem.TransferAmount = Convert.ToDecimal(collection["TransferAmount"][i]);
                        aRCustomerAdjustmentItem.ARDiscount_Id = Convert.ToInt32(collection["DiscountId"][i]);
                        if (customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionType.ToString() == TransactionTypeList.FirstOrDefault(x => x.Text == "Payment").Value)
                        {
                            aRCustomerAdjustmentItem.DiscountBalance = Convert.ToDecimal(collection["DiscountBalance"][i]) - aRCustomerAdjustmentItem.PaidAmount;
                            aRCustomerAdjustmentItem.Utilized = aRCustomerAdjustmentItem.PaidAmount + Convert.ToDecimal(collection["UtilizedBalance"][i]);
                        }
                        else if (customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionType.ToString() == TransactionTypeList.FirstOrDefault(x => x.Text == "Transfer").Value)
                        {
                            aRCustomerAdjustmentItem.DiscountBalance = Convert.ToDecimal(collection["DiscountBalance"][i]) - aRCustomerAdjustmentItem.TransferAmount;
                            aRCustomerAdjustmentItem.Utilized = aRCustomerAdjustmentItem.TransferAmount + Convert.ToDecimal(collection["UtilizedBalance"][i]);
                            aRCustomerAdjustmentItem.TransferToCustomer = Convert.ToInt32(collection["TransferToCustomer"][i]);
                        }
                        else
                        {
                            ARDiscount discount = new ARDiscount();
                            discount = _dbContext.ARDiscount.Where(x => x.Id == aRCustomerAdjustmentItem.ARDiscount_Id).FirstOrDefault();
                            discount.IsClosed = true;
                            _dbContext.Update(discount);
                            _dbContext.SaveChanges();
                            aRCustomerAdjustmentItem.DiscountAmount = Convert.ToDecimal(collection["GrandTotal"][i]);
                            aRCustomerAdjustmentItem.DiscountBalance = Convert.ToDecimal(collection["DiscountBalance"][i]);
                            aRCustomerAdjustmentItem.IsClosed = true;

                        }
                        aRCustomerAdjustmentItem.Customer_Id = Convert.ToInt32(collection["Customer_Id"][i]);
                        aRCustomerAdjustmentItem.DiscountAdjustment_Id = customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id;

                        Details.Add(aRCustomerAdjustmentItem);

                    };
                    await _ARCustomerAdjustmentItemRepository.CreateRangeAsync(Details);
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Customer Discount Adjustment No. {0} has been created successfully.", customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionNo);
                }
                catch (Exception ex)
                {
                    await _ARCustomerDiscountAdjustmentRepository.DeleteAsync(customerDiscountAdjustmentVM.CustomerDiscountAdjustment);
                    var DeleteList = _ARCustomerAdjustmentItemRepository.Get(x => x.DiscountAdjustment_Id == customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id).ToList();
                    await _ARCustomerAdjustmentItemRepository.DeleteRangeAsync(DeleteList);

                }
            }
            else
            {
                ARCustomerDiscountAdjustment aRCustomerDiscountAdjustment = _ARCustomerDiscountAdjustmentRepository.Get(x => x.Id == customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id).FirstOrDefault();
                aRCustomerDiscountAdjustment.TransactionNo = customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionNo;
                aRCustomerDiscountAdjustment.TransactionType = customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionType;
                aRCustomerDiscountAdjustment.UpdatedBy = userId;
                aRCustomerDiscountAdjustment.UpdatedDate = DateTime.UtcNow;
                aRCustomerDiscountAdjustment.CompanyId = companyId;
                aRCustomerDiscountAdjustment.IsActive = true;
                aRCustomerDiscountAdjustment.IsDeleted = false;
                //aRCustomerDiscountAdjustment.TransactionNo = customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id;

                await _ARCustomerDiscountAdjustmentRepository.UpdateAsync(aRCustomerDiscountAdjustment);
                var UpdateList = new List<ARCustomerAdjustmentItem>();
                var foundDetail = _dbContext.ARCustomerAdjustmentItem.Where(a => a.DiscountAdjustment_Id == customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id).ToList();
                if (!ReferenceEquals(customerDiscountAdjustmentVM.CustomerAdjustmentItem, null))
                {
                    foreach (var det in foundDetail)
                    {
                        bool result = customerDiscountAdjustmentVM.CustomerAdjustmentItem.Exists(s => s.Id == det.Id);
                        if (!result)
                        {
                            await _ARCustomerAdjustmentItemRepository.DeleteAsync(det);
                        }
                    }

                    for (int i = 0; i < customerDiscountAdjustmentVM.CustomerAdjustmentItem.Count; i++)
                    {
                        ARCustomerAdjustmentItem detail = foundDetail.FirstOrDefault(x => x.Id == customerDiscountAdjustmentVM.CustomerAdjustmentItem[i].Id);
                        if (!ReferenceEquals(detail, null) && detail.IsClosed == false)
                        {
                            detail.DiscountAdjustment_Id = customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id;
                            detail.DiscountAmount = customerDiscountAdjustmentVM.CustomerAdjustmentItem[i].DiscountAmount;
                            detail.PaidAmount = customerDiscountAdjustmentVM.CustomerAdjustmentItem[i].PaidAmount;
                            detail.TransferAmount = customerDiscountAdjustmentVM.CustomerAdjustmentItem[i].TransferAmount;
                            detail.TransferToCustomer = customerDiscountAdjustmentVM.CustomerAdjustmentItem[i].TransferToCustomer;
                            detail.Customer_Id = customerDiscountAdjustmentVM.CustomerAdjustmentItem[i].Customer_Id;
                            UpdateList.Add(detail);
                        }
                    }
                }
                List<ARCustomerAdjustmentItem> Details = new List<ARCustomerAdjustmentItem>();
                for (int i = 0; i < collection["DiscountId"].Count; i++)
                {
                    ARCustomerAdjustmentItem aRCustomerAdjustmentItem = new ARCustomerAdjustmentItem();
                    aRCustomerAdjustmentItem.DiscountAmount = Convert.ToDecimal(collection["GrandTotal"][i]);
                    aRCustomerAdjustmentItem.PaidAmount = Convert.ToDecimal(collection["PaidAmount"][i]);
                    aRCustomerAdjustmentItem.TransferAmount = Convert.ToDecimal(collection["TransferAmount"][i]);
                    aRCustomerAdjustmentItem.ARDiscount_Id = Convert.ToInt32(collection["DiscountId"][i]);
                    if (customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionType.ToString() == TransactionTypeList.FirstOrDefault(x => x.Text == "Payment").Value)
                    {
                        aRCustomerAdjustmentItem.DiscountBalance = Convert.ToDecimal(collection["DiscountBalance"][i]) - aRCustomerAdjustmentItem.PaidAmount;
                        aRCustomerAdjustmentItem.Utilized = aRCustomerAdjustmentItem.PaidAmount + Convert.ToDecimal(collection["UtilizedBalance"][i]);
                    }
                    else if (customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionType.ToString() == TransactionTypeList.FirstOrDefault(x => x.Text == "Transfer").Value)
                    {
                        aRCustomerAdjustmentItem.DiscountBalance = Convert.ToDecimal(collection["DiscountBalance"][i]) - aRCustomerAdjustmentItem.TransferAmount;
                        aRCustomerAdjustmentItem.Utilized = aRCustomerAdjustmentItem.TransferAmount + Convert.ToDecimal(collection["UtilizedBalance"][i]);
                        aRCustomerAdjustmentItem.TransferToCustomer = Convert.ToInt32(collection["TransToCustomer"][i]);
                    }
                    else
                    {
                        ARDiscount discount = new ARDiscount();
                        discount = _dbContext.ARDiscount.Where(x => x.Id == aRCustomerAdjustmentItem.ARDiscount_Id).FirstOrDefault();
                        discount.IsClosed = true;
                        _dbContext.Update(discount);
                        _dbContext.SaveChanges();
                        aRCustomerAdjustmentItem.DiscountAmount = Convert.ToDecimal(collection["GrandTotal"][i]);
                        aRCustomerAdjustmentItem.DiscountBalance = Convert.ToDecimal(collection["DiscountBalance"][i]);
                        aRCustomerAdjustmentItem.IsClosed = true;
                    }
                    aRCustomerAdjustmentItem.Customer_Id = Convert.ToInt32(collection["Customer_Id"][i]);
                    aRCustomerAdjustmentItem.DiscountAdjustment_Id = customerDiscountAdjustmentVM.CustomerDiscountAdjustment.Id;
                    Details.Add(aRCustomerAdjustmentItem);
                };
                await _ARCustomerAdjustmentItemRepository.UpdateRangeAsync(UpdateList);
                await _ARCustomerAdjustmentItemRepository.CreateRangeAsync(Details);
                TempData["error"] = "false";
                TempData["message"] = string.Format("Customer Discount Adjustment No. {0} has been updated successfully.", aRCustomerDiscountAdjustment.TransactionNo);
            }
            return RedirectToAction("Create", "CustomerDiscountAdjustment");
        }

        public IActionResult GetList()
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
                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchTransType = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchDate = Request.Form["columns[2][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var DiscountAdj = (from tempcustomer in _dbContext.ARCustomerDiscountAdjustment.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    DiscountAdj = DiscountAdj.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                DiscountAdj = !string.IsNullOrEmpty(searchTransNo) ? DiscountAdj.Where(m => m.TransactionNo.ToString().Contains(searchTransNo)) : DiscountAdj;
                DiscountAdj = !string.IsNullOrEmpty(searchDate) ? DiscountAdj.Where(m => m.CreatedDate.ToString("dd-MMM-yyyy").ToUpper().Contains(searchDate.ToUpper())) : DiscountAdj;
                DiscountAdj = !string.IsNullOrEmpty(searchTransType) ? DiscountAdj.Where(m => _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(m.TransactionType)).ConfigValue.ToUpper().Contains(searchTransType.ToUpper())) : DiscountAdj;

                recordsTotal = DiscountAdj.Count();
                var data = DiscountAdj.Skip(skip).Take(pageSize).ToList();
                List<CustomerDiscountAdjustmentVM> Details = new List<CustomerDiscountAdjustmentVM>();
                foreach (var grp in data)
                {
                    CustomerDiscountAdjustmentVM customerDiscountAdjustmentVM = new CustomerDiscountAdjustmentVM();
                    customerDiscountAdjustmentVM.TransactionType = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.TransactionType)).ConfigValue;
                    customerDiscountAdjustmentVM.TransactionNo = grp.TransactionNo;
                    customerDiscountAdjustmentVM.Date = grp.CreatedDate.ToString("dd-MMM-yyyy");
                    customerDiscountAdjustmentVM.CustomerDiscountAdjustment = grp;
                    Details.Add(customerDiscountAdjustmentVM);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<IActionResult> Delete(int id)
        {
            ARCustomerDiscountAdjustment aRCustomerDiscountAdjustment = _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x => x.Id == id);
            aRCustomerDiscountAdjustment.IsDeleted = true;
            aRCustomerDiscountAdjustment.IsActive = false;
            _dbContext.ARCustomerDiscountAdjustment.Update(aRCustomerDiscountAdjustment);
            await _dbContext.SaveChangesAsync();

            List<ARCustomerAdjustmentItem> aRCustomerAdjustmentItem = _dbContext.ARCustomerAdjustmentItem.Where(x => x.DiscountAdjustment_Id == aRCustomerDiscountAdjustment.Id).ToList();
            foreach (var item in aRCustomerAdjustmentItem)
            {
                item.IsDeleted = true;
                _dbContext.ARCustomerAdjustmentItem.Update(item);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CloseDiscount(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            var discount = _dbContext.ARDiscount.FirstOrDefault(x => x.Id == id);
            ARCustomerAdjustmentItem discountclosed = _dbContext.ARCustomerAdjustmentItem.LastOrDefault(x => x.ARDiscount_Id == id && x.IsApproved == false);
            try
            {

                if (discountclosed != null)
                {

                    //Create Voucher
                    VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                    GLVoucher voucherMaster = new GLVoucher();
                    List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                    string voucherDescription = string.Format(
                    "Invoice # : {0} ",
                    _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x => x.Id == discountclosed.DiscountAdjustment_Id).TransactionNo
                    //invoice.Customer.Name
                    );

                    int voucherId;
                    voucherMaster.VoucherType = "JV";
                    voucherMaster.VoucherDate = DateTime.Now;
                    //  voucherMaster.Reference = invoice.ReferenceNo;
                    voucherMaster.Currency = "PKR";
                    // voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                    voucherMaster.Description = voucherDescription;
                    voucherMaster.Status = "Approved";
                    voucherMaster.ApprovedBy = _userId;
                    voucherMaster.ApprovedDate = DateTime.Now;
                    voucherMaster.IsSystem = true;
                    voucherMaster.ModuleName = "AR/CustomerDiscountAdjustment";
                    voucherMaster.ModuleId = id;
                    voucherMaster.Amount = discountclosed.DiscountBalance;

                    //Voucher Details
                    //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                    //var amount = invoiceItems.Sum(s => s.LineTotal);
                    //var discount = invoiceItems.Sum(s => s.DiscountAmount);
                    //Debit Entry
                    GLVoucherDetail voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = 633;
                    voucherDetail.Sequence = 1;
                    voucherDetail.Description = voucherDescription;
                    voucherDetail.Debit = discountclosed.DiscountBalance;
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = _userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                    GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                    voucherDetailItem.AccountId = 634;
                    voucherDetailItem.Sequence = 1;
                    voucherDetailItem.Description = voucherDescription;
                    voucherDetailItem.Debit = 0;
                    voucherDetailItem.Credit = discountclosed.DiscountBalance;
                    voucherDetailItem.IsDeleted = false;
                    voucherDetailItem.CreatedBy = _userId;
                    voucherDetailItem.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetailItem);
                    voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                    if (voucherId != 0)
                    {
                        discountclosed.VoucherId = voucherId;
                        discountclosed.IsClosed = true;
                        discountclosed.ApprovedBy = _userId;
                        discountclosed.ApprovedDate = DateTime.Now;
                        discountclosed.IsApproved = true;
                        //On approval updating Invoice
                        TempData["error"] = "false";
                        TempData["message"] = "Invoice has been approved successfully";
                        var entry = _dbContext.ARCustomerAdjustmentItem.Update(discountclosed);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        discount.IsClosed = true;
                        _dbContext.ARDiscount.Update(discount);
                        await _dbContext.SaveChangesAsync();

                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";

                    }

                }
                return RedirectToAction("Create", discountclosed.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.InnerException.Message == null ? ex.Message.ToString() : ex.InnerException.Message.ToString();
                return RedirectToAction("Create", discount.Id);
            }
        }
        public async Task<IActionResult> ClosedDiscount(int id)
        {
            var discount = _dbContext.ARDiscount.Include(x => x.Customer_).FirstOrDefault(x => x.Id == id);
            try
            {
                if (discount != null)
                {
                    discount.IsClosed = true;
                    _dbContext.ARDiscount.Update(discount);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Cannot close discount due to some internal error";

                }
                return Json(true);
                //if (discountclosed != null)
                //{

                //    //Create Voucher
                //    VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                //    GLVoucher voucherMaster = new GLVoucher();
                //    List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                //    string voucherDescription = string.Format(
                //    "Invoice # : {0} ",
                //    _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x => x.Id == discountclosed.DiscountAdjustment_Id).TransactionNo
                //    //invoice.Customer.Name
                //    );

                //    int voucherId;
                //    voucherMaster.VoucherType = "JV";
                //    voucherMaster.VoucherDate = DateTime.Now;
                //    //  voucherMaster.Reference = invoice.ReferenceNo;
                //    voucherMaster.Currency = "PKR";
                //    // voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                //    voucherMaster.Description = voucherDescription;
                //    voucherMaster.Status = "Approved";
                //    voucherMaster.ApprovedBy = _userId;
                //    voucherMaster.ApprovedDate = DateTime.Now;
                //    voucherMaster.IsSystem = true;
                //    voucherMaster.ModuleName = "AR/CustomerDiscountAdjustment";
                //    voucherMaster.ModuleId = id;
                //    voucherMaster.Amount = discountclosed.DiscountBalance;

                //    //Voucher Details
                //    //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                //    //var amount = invoiceItems.Sum(s => s.LineTotal);
                //    //var discount = invoiceItems.Sum(s => s.DiscountAmount);
                //    //Debit Entry
                //    GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = 633;
                //    voucherDetail.Sequence = 1;
                //    voucherDetail.Description = voucherDescription;
                //    voucherDetail.Debit = discountclosed.DiscountBalance;
                //    voucherDetail.Credit = 0;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = _userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //    GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                //    voucherDetailItem.AccountId = 634;
                //    voucherDetailItem.Sequence = 1;
                //    voucherDetailItem.Description = voucherDescription;
                //    voucherDetailItem.Debit = 0;
                //    voucherDetailItem.Credit = discountclosed.DiscountBalance;
                //    voucherDetailItem.IsDeleted = false;
                //    voucherDetailItem.CreatedBy = _userId;
                //    voucherDetailItem.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetailItem);
                //    voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);

                //    if (voucherId != 0)
                //    {
                //        discountclosed.VoucherId = voucherId;
                //        discountclosed.IsClosed = true;
                //        discountclosed.ApprovedBy = _userId;
                //        discountclosed.ApprovedDate = DateTime.Now;
                //        discountclosed.IsApproved = true;
                //        //On approval updating Invoice
                //        TempData["error"] = "false";
                //        TempData["message"] = "Invoice has been approved successfully";
                //        var entry = _dbContext.ARCustomerAdjustmentItem.Update(discountclosed);
                //        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                //        discount.IsClosed = true;
                //        _dbContext.ARDiscount.Update(discount);
                //        await _dbContext.SaveChangesAsync();
                //    }
                //    else
                //    {
                //        TempData["error"] = "true";
                //        TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";

                //    }

                //}
                //else
                //{
                //    VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                //    GLVoucher voucherMaster = new GLVoucher();
                //    List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                //    string voucherDescription = string.Format(
                //    "Invoice # : {0} ",
                //    discount.TransactionNo
                //    //invoice.Customer.Name
                //    );

                //    int voucherId;
                //    voucherMaster.VoucherType = "JV";
                //    voucherMaster.VoucherDate = DateTime.Now;
                //    //  voucherMaster.Reference = invoice.ReferenceNo;
                //    voucherMaster.Currency = "PKR";
                //    // voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                //    voucherMaster.Description = voucherDescription;
                //    voucherMaster.Status = "Approved";
                //    voucherMaster.ApprovedBy = _userId;
                //    voucherMaster.ApprovedDate = DateTime.Now;
                //    voucherMaster.IsSystem = true;
                //    voucherMaster.ModuleName = "AR/CustomerDiscountAdjustment";
                //    voucherMaster.ModuleId = id;
                //    voucherMaster.Amount = discount.GrandTotal;

                //    //Voucher Details
                //    //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                //    //var amount = invoiceItems.Sum(s => s.LineTotal);
                //    //var discount = invoiceItems.Sum(s => s.DiscountAmount);
                //    //Debit Entry
                //    GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = 633;
                //    voucherDetail.Sequence = 1;
                //    voucherDetail.Description = voucherDescription;
                //    voucherDetail.Debit = discount.GrandTotal;
                //    voucherDetail.Credit = 0;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = _userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //    GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                //    voucherDetailItem.AccountId = 634;
                //    voucherDetailItem.Sequence = 1;
                //    voucherDetailItem.Description = voucherDescription;
                //    voucherDetailItem.Debit = 0;
                //    voucherDetailItem.Credit = discount.GrandTotal;
                //    voucherDetailItem.IsDeleted = false;
                //    voucherDetailItem.CreatedBy = _userId;
                //    voucherDetailItem.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetailItem);
                //    voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);

                //    if (voucherId != 0)
                //    {
                //        TempData["error"] = "false";
                //        TempData["message"] = "Invoice has been approved successfully";
                //        discount.IsClosed = true;
                //        _dbContext.ARDiscount.Update(discount);
                //        await _dbContext.SaveChangesAsync();
                //    }
                //    else
                //    {
                //        TempData["error"] = "true";
                //        TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";

                //    }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.InnerException.Message == null ? ex.Message.ToString() : ex.InnerException.Message.ToString();
                return RedirectToAction("Create");
            }
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            var discount = _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x => x.Id == id);
            var discountItem = _dbContext.ARCustomerAdjustmentItem.Include(p => p.Customer_).Where(x => x.DiscountAdjustment_Id == id).ToList();
            var configValues = new ConfigValues(_dbContext);
            var TransactionType = configValues.GetConfigValues("AR", "Transaction Type", _companyId);
            try
            {
                if (discount.TransactionType.ToString() == TransactionType.FirstOrDefault(x => x.Text == "Payment").Value)
                {
                    for (int i = 0; i < discountItem.Count; i++)
                    {
                        VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                        GLVoucher voucherMaster = new GLVoucher();
                        List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                        string voucherDescription = string.Format(
                        "Customer Discount Payment # : ",
                        discount.TransactionNo, " of ",
                        discountItem[i]?.Customer_?.Name
                        );

                        int voucherId;
                        voucherMaster.VoucherType = "CDISADJ";
                        voucherMaster.VoucherDate = DateTime.Now;
                        voucherMaster.ReferenceId = discountItem[i].Customer_Id;
                        //  voucherMaster.Reference = invoice.ReferenceNo;
                        voucherMaster.Currency = "PKR";
                        // voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                        voucherMaster.Description = voucherDescription;
                        voucherMaster.Status = "Approved";
                        voucherMaster.ApprovedBy = _userId;
                        voucherMaster.ApprovedDate = DateTime.Now;
                        voucherMaster.IsSystem = true;
                        voucherMaster.ModuleName = "AR/CustomerDiscountAdjustment";
                        voucherMaster.ModuleId = id;
                        voucherMaster.Amount = discountItem[i].PaidAmount;

                        //Voucher Details
                        //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                        //var amount = invoiceItems.Sum(s => s.LineTotal);
                        //var discount = invoiceItems.Sum(s => s.DiscountAmount);

                        //Debit Entry
                        GLVoucherDetail voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = 633;
                        voucherDetail.Sequence = 1;
                        voucherDetail.Description = voucherDescription;
                        voucherDetail.Debit = discountItem[i].PaidAmount;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                        //Credit
                        GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                        voucherDetailItem.AccountId = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == discountItem[i].Customer_Id).AccountId;
                        voucherDetailItem.Sequence = 1;
                        voucherDetailItem.Description = voucherDescription;
                        voucherDetailItem.Debit = 0;
                        voucherDetailItem.Credit = discountItem[i].PaidAmount;
                        voucherDetailItem.IsDeleted = false;
                        voucherDetailItem.CreatedBy = _userId;
                        voucherDetailItem.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetailItem);
                        voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                        if (voucherId != 0)
                        {
                            discountItem[i].VoucherId = voucherId;
                            discountItem[i].ApprovedBy = _userId;
                            discountItem[i].ApprovedDate = DateTime.Now;
                            discountItem[i].IsApproved = true;
                            discount.ApprovedBy = _userId;
                            discount.ApprovedDate = DateTime.Now;
                            discount.Status = "Approved";
                            discount.IsApproved = true;
                            _dbContext.ARCustomerDiscountAdjustment.Update(discount);
                            //On approval updating Invoice
                            TempData["error"] = "false";
                            TempData["message"] = "Customer Doiscount Adjustment has been approved successfully";
                            var entry = _dbContext.ARCustomerAdjustmentItem.Update(discountItem[i]);
                            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();

                        }
                        else
                        {
                            TempData["error"] = "true";
                            TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";

                        }
                    }
                }
                else if (discount.TransactionType.ToString() == TransactionType.FirstOrDefault(x => x.Text == "Transfer").Value)
                {
                    for (int i = 0; i < discountItem.Count; i++)
                    {
                        VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                        GLVoucher voucherMaster = new GLVoucher();
                        List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                        string voucherDescription = string.Format(
                        "Customer Discount Transfer # : ",
                        discount.TransactionNo, " of ",
                        discountItem[i]?.Customer_?.Name
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
                        voucherMaster.ModuleName = "AR/CustomerDiscountAdjustment";
                        voucherMaster.ModuleId = id;
                        voucherMaster.ReferenceId = discountItem[i].Customer_Id;
                        voucherMaster.Amount = discountItem[i].TransferAmount;

                        //Voucher Details
                        //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                        //var amount = invoiceItems.Sum(s => s.LineTotal);
                        //var discount = invoiceItems.Sum(s => s.DiscountAmount);
                        //Debit Entry
                        GLVoucherDetail voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = 633;
                        voucherDetail.Sequence = 1;
                        voucherDetail.Description = voucherDescription;
                        voucherDetail.Debit = discountItem[i].TransferAmount;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                        GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                        voucherDetailItem.AccountId = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == discountItem[i].TransferToCustomer).AccountId;
                        voucherDetailItem.Sequence = 1;
                        voucherDetailItem.Description = voucherDescription;
                        voucherDetailItem.Debit = 0;
                        voucherDetailItem.Credit = discountItem[i].TransferAmount;
                        voucherDetailItem.IsDeleted = false;
                        voucherDetailItem.CreatedBy = _userId;
                        voucherDetailItem.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetailItem);
                        voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                        if (voucherId != 0)
                        {
                            discountItem[i].VoucherId = voucherId;
                            discountItem[i].ApprovedBy = _userId;
                            discountItem[i].ApprovedDate = DateTime.Now;
                            discountItem[i].IsApproved = true;
                            discount.ApprovedBy = _userId;
                            discount.ApprovedDate = DateTime.Now;
                            discount.Status = "Approved";
                            discount.IsApproved = true;
                            _dbContext.ARCustomerDiscountAdjustment.Update(discount);
                            //On approval updating Invoice
                            TempData["error"] = "false";
                            TempData["message"] = "Invoice has been approved successfully";
                            var entry = _dbContext.ARCustomerAdjustmentItem.Update(discountItem[i]);
                            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();

                        }
                        else
                        {
                            TempData["error"] = "true";
                            TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";

                        }
                    }
                    //Create Voucher
                }
                else
                {
                    for (int i = 0; i < discountItem.Count; i++)
                    {
                        VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                        GLVoucher voucherMaster = new GLVoucher();
                        List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                        string voucherDescription = string.Format(
                        "Customer Discount Expire # : ",
                        discount.TransactionNo, " of ",
                        discountItem[i]?.Customer_?.Name
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
                        voucherMaster.ModuleName = "AR/CustomerDiscountAdjustment";
                        voucherMaster.ModuleId = id;
                        voucherMaster.Amount = discountItem[i].DiscountBalance;

                        //Voucher Details
                        //var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                        //var amount = invoiceItems.Sum(s => s.LineTotal);
                        //var discount = invoiceItems.Sum(s => s.DiscountAmount);
                        //Debit Entry
                        GLVoucherDetail voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = 633; // DISCOUNT PAYABLE
                        voucherDetail.Sequence = 1;
                        voucherDetail.Description = voucherDescription;
                        voucherDetail.Debit = discountItem[i].DiscountBalance;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                        GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                        voucherDetailItem.AccountId = 634; // EXPIRED DISCOUNT
                        voucherDetailItem.Sequence = 1;
                        voucherDetailItem.Description = voucherDescription;
                        voucherDetailItem.Debit = 0;
                        voucherDetailItem.Credit = discountItem[i].DiscountBalance;
                        voucherDetailItem.IsDeleted = false;
                        voucherDetailItem.CreatedBy = _userId;
                        voucherDetailItem.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetailItem);
                        voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                        if (voucherId != 0)
                        {
                            discountItem[i].VoucherId = voucherId;
                            discountItem[i].ApprovedBy = _userId;
                            discountItem[i].ApprovedDate = DateTime.Now;
                            discountItem[i].IsApproved = true;
                            discount.ApprovedBy = _userId;
                            discount.ApprovedDate = DateTime.Now;
                            discount.Status = "Approved";
                            discount.IsApproved = true;
                            _dbContext.ARCustomerDiscountAdjustment.Update(discount);
                            //On approval updating Invoice
                            TempData["error"] = "false";
                            TempData["message"] = "Invoice has been approved successfully";
                            var entry = _dbContext.ARCustomerAdjustmentItem.Update(discountItem[i]);
                            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();

                        }
                        else
                        {
                            TempData["error"] = "true";
                            TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";

                        }
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.InnerException.Message == null ? ex.Message.ToString() : ex.InnerException.Message.ToString();
                return RedirectToAction("Create", discount.Id);
            }
        }

        public IActionResult Details(int id)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            CustomerDiscountAdjustmentVM customerDiscountAdjustmentVM = new CustomerDiscountAdjustmentVM();
            customerDiscountAdjustmentVM.CustomerDiscountAdjustment = _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x => x.Id == id);
            customerDiscountAdjustmentVM.CustomerAdjustmentItem = _dbContext.ARCustomerAdjustmentItem.Include(p => p.Customer_).Where(x => x.DiscountAdjustment_Id == id).ToList();
            foreach (var b in customerDiscountAdjustmentVM.CustomerAdjustmentItem)
            {
                if (b.TransferToCustomer != 0)
                {
                    b.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == b.TransferToCustomer).Name;
                }
            }
            customerDiscountAdjustmentVM.TransType = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == customerDiscountAdjustmentVM.CustomerDiscountAdjustment.TransactionType).ConfigValue;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=PurchaseOrder&cId=", companyId, "&id={0}");
            //var discount = _dbContext.ARCustomerAdjustmentItem.Include(p => p.Customer_).Where(x => x.DiscountAdjustment_Id == id).ToList();
            //var discountItems = _dbContext.ARCustomerDiscountAdjustment.FirstOrDefault(x =>x.Id == id);
            ViewBag.NavbarHeading = "Customer Discount";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = customerDiscountAdjustmentVM.CustomerAdjustmentItem;
            return View(customerDiscountAdjustmentVM);
        }
        public int MaxTransNo(int companyId)
        {
            int maxTranscationNo = 1;
            var transcation = _ARCustomerDiscountAdjustmentRepository.Get(c => c.CompanyId == companyId).ToList();
            if (transcation.Count > 0)
            {
                maxTranscationNo = transcation.Max(v => v.TransactionNo);
                return maxTranscationNo + 1;
            }
            else
            {
                return maxTranscationNo;
            }
        }
    }
}