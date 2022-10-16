using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class SaleReturnInvoiceController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public SaleReturnInvoiceController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Sale Return Invoice";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                     .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                     .Select(c => c.ConfigValue)
                     .FirstOrDefault();
            ViewBag.ReportUrl = configs;
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=SaleReturnInvoiceBP&cId=", companyId, "&id=");
            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnInvoiceRepo = new SaleReturnInvoiceRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            SaleReturnInvoiceViewModel SaleReturnInvoiceViewModel = new SaleReturnInvoiceViewModel();
            SaleReturnInvoiceViewModel.ReasonType = configValues.GetConfigValues("AR", "Reason Type", companyId);
            SaleReturnInvoiceViewModel.ReturnType = configValues.GetConfigValues("AR", "Return Type", companyId);
            SaleReturnInvoiceViewModel.Season = configValues.GetConfigValues("Inventory", "Season", companyId);
            ViewBag.PackingNo = new SelectList((from p in _dbContext.ARPacking.Where(x => x.CompanyId == companyId && x.IsDeleted == false &&  x.IsApproved == true).ToList()
                                                where !_dbContext.ARSaleReturnInvoice.Where(x => x.IsDeleted == false).Any(s => p.Id.ToString().Contains(s.PackingId.ToString()))
                                                select p
                                                              ).ToList(), "Id", "PackingNo");
            var Categories = _dbContext.InvItemCategories.AsQueryable();
            ViewBag.FourthLevelCategoryLOV = new SelectList(from c in Categories

                                                            where c.IsDeleted != true && c.CategoryLevel == 4 && c.Code.Contains("07.01")
                                                            select new
                                                            {
                                                                Id = c.Id,
                                                                Name = c.Code + " - " + c.Name
                                                            }, "Id", "Name");
            if (id == 0)
            {
                //SaleReturnInvoiceViewModel.SIGPNo = SIGPRepository.SIGPMaxNo(companyId);
                SaleReturnInvoiceViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => /*x.CompanyId == companyId &&*/ x.IsDeleted != true).ToList(), "Id", "Name");
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Sale Return Invoice";
                return View(SaleReturnInvoiceViewModel);
            }
            else
            {
                SaleReturnInvoiceViewModel = saleReturnInvoiceRepo.GetById(id);
                SaleReturnInvoiceViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x =>/* x.CompanyId == companyId &&*/ x.IsDeleted != true).ToList(), "Id", "Name");
                SaleReturnInvoiceViewModel.Address = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == SaleReturnInvoiceViewModel.CustomerId).Address;
                SaleReturnInvoiceViewModel.SaleReturnInvoiceItemsList = _dbContext.ARSaleReturnInvoiceItems.Include(x=>x.SaleInvoice).Include(x=>x.FourthLevel).Include(x=>x.Item).Include(x => x.ReasonType).Include(x => x.ReasonType).Include(x => x.Season).Where(x => x.SRInvoiceId == SaleReturnInvoiceViewModel.Id).ToArray();
                SaleReturnInvoiceViewModel.ReasonType = configValues.GetConfigValues("AR", "Reason Type", companyId);
                SaleReturnInvoiceViewModel.ReturnType = configValues.GetConfigValues("AR", "Return Type", companyId);
                SaleReturnInvoiceViewModel.Season = configValues.GetConfigValues("Inventory", "Season", companyId);

                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Sale Return Invoice";
                ViewBag.TitleStatus = "Created";
                ViewBag.PackingNo =new SelectList(_dbContext.ARPacking.Where(x=>x.Id==SaleReturnInvoiceViewModel.PackingId).ToList(), "Id", "PackingNo");
                return View(SaleReturnInvoiceViewModel);
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var model = _dbContext.ARSaleReturnInvoice
                .Include(i => i.Customer)
                .Include(i => i.Packing)
            .Where(i => i.Id == id).FirstOrDefault();
            var modelItems = _dbContext.ARSaleReturnInvoiceItems
                                .Include(i => i.FourthLevel)
                                .Include(i => i.Item)
                                .Include(i => i.ReturnType)
                                .Include(i => i.ReasonType)
                                .Include(i => i.Season)
                                .Include(i => i.SaleInvoice)
                                .Where(i => i.SRInvoiceId == id)
                                .ToList();
            ViewBag.TotalQty = modelItems.Sum(x => x.Qty);
            ViewBag.TotalAmount = modelItems.Sum(x => x.Amount);

            ViewBag.NavbarHeading = "Sale Return Invoice";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = modelItems;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleReturnInvoiceViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var saleReturnInvoiceRepo = new SaleReturnInvoiceRepo(_dbContext);
          
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.InvoiceNo = saleReturnInvoiceRepo.MaxNo(companyId);
                
                bool isSuccess = await saleReturnInvoiceRepo.Create(model,collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Return Invoice. {0} has been created successfully.", model.InvoiceNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                // return RedirectToAction("Index", "SaleReturnInvoice");
               return RedirectToAction(nameof(Index));
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                
                bool isSuccess = await saleReturnInvoiceRepo.Update(model,collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Return Invoice. {0} has been updated successfully.", model.InvoiceNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetList()
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
                var searchInvNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchInvDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchPackingNo = Request.Form["columns[3][search][value]"].FirstOrDefault();
               // var searchBuiltyNo = Request.Form["columns[4][search][value]"].FirstOrDefault();
                //var searchBails = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from srInvoice in _dbContext.ARSaleReturnInvoice.Include(x => x.Packing).Include(x=>x.Customer).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select srInvoice);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                Data = !string.IsNullOrEmpty(searchInvNo) ? Data.Where(m => m.InvoiceNo.ToString().ToLower().Contains(searchInvNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchInvDate) ? Data.Where(m => m.InvoiceDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchInvDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchCustomer) ? Data.Where(m => m.Customer.Name.ToString().ToLower().Contains(searchCustomer.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchPackingNo) ? Data.Where(m => m.Packing.PackingNo.ToString().Contains(searchPackingNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchStatus) ? Data.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : Data;

                recordsTotal = Data.Count();
                var data = Data.ToList();
                if (pageSize == -1)
                {
                    data = Data.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = Data.Skip(skip).Take(pageSize).ToList();
                }
                List<SaleReturnInvoiceViewModel> Details = new List<SaleReturnInvoiceViewModel>();
                foreach (var grp in data)
                {
                    SaleReturnInvoiceViewModel SaleReturnInvoiceViewModel = new SaleReturnInvoiceViewModel();
                    SaleReturnInvoiceViewModel.ARSaleReturnInvoice = grp;
                    SaleReturnInvoiceViewModel.ARSaleReturnInvoice.Approve = approve;
                    SaleReturnInvoiceViewModel.ARSaleReturnInvoice.Unapprove = unApprove;
                    SaleReturnInvoiceViewModel.Date = grp.InvoiceDate.ToString(Helpers.CommonHelper.DateFormat);
                    Details.Add(SaleReturnInvoiceViewModel);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult GetPackingList(int id)
        {
            if (id != 0)
            {
                List<ItemListViewModel> itemListViewModel = new List<ItemListViewModel>();
                var igpId = (from m in _dbContext.APIGP where m.IGP == id && m.IsApproved == true && m.IsDeleted == false select m).FirstOrDefault();
                var IGPList = _dbContext.ARPackingItems.Include(x => x.Packing).Include(x=>x.FourthLevel).Include(x => x.Item).Include(x => x.ReasonType).Include(x => x.ReturnType).Include(x => x.Season).Where(x=>x.Packing.Id==id && x.Packing.IsDeleted==false && x.Packing.IsApproved==true).ToList();
                //wareHouseId = IGPList.FirstOrDefault().Packing.WareHouseId;
                //return Ok(new { IGPList, wareHouseId });
                return Ok(IGPList);
            }
            else
            {
                return Ok();
            }
        }

        public IActionResult GetViewDetail(int id,int customerId)
        {
            List<ItemDetailViewModel> itemDetailList = new List<ItemDetailViewModel>();
            var itemCategory = _dbContext.InvItems.Where(x => x.Id == id).FirstOrDefault();
            ItemDetailViewModel _itemDetailViewModel = new ItemDetailViewModel();

            var data = (from f in  _dbContext.ARInvoiceItems
                        join
                        g in _dbContext.ARInvoices.Include(x=>x.Customer) on f.InvoiceId equals g.Id
                        where f.ItemId == id && g.CustomerId==customerId && g.IsDeleted == false && g.Status == "Approved"
                        select new
                        {
                            f,
                            g
                        }).OrderByDescending(a => a.f.Id).Take(10);

            var salereturn = (from s in _dbContext.ARSaleReturnInvoiceItems.Where(x => x.ItemId == id)
                              join sr in _dbContext.ARSaleReturnInvoice.Where(x=>x.CustomerId==customerId) on s.SRInvoiceId equals sr.Id
                              where sr.Status == "Approved" && sr.ApprovedDate <= DateTime.Now
                              select new
                              {
                                  sr,
                                  s,
                              }).OrderByDescending(a => a.sr.Id).Take(90);
            var InvoiceQty = salereturn.Sum(x => x.s.Qty);
            _itemDetailViewModel.Category = _dbContext.InvItemCategories.Where(x => x.Id == itemCategory.CategoryId).Select(x => x.Name).FirstOrDefault();
            _itemDetailViewModel.Stock = itemCategory.StockQty;
            _itemDetailViewModel.Average = itemCategory.AvgRate;
            _itemDetailViewModel.ItemDescription = itemCategory.Name;
            _itemDetailViewModel.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == itemCategory.Unit).ConfigValue;
            _itemDetailViewModel.Consumption = InvoiceQty / 90;//Per day consumptions of 3 months. 

            itemDetailList.Add(_itemDetailViewModel);
            foreach (var item in data)
            {
                ItemDetailViewModel itemDetailViewModel = new ItemDetailViewModel();
                itemDetailViewModel.PONo = Convert.ToInt32(item.g.InvoiceNo);
                itemDetailViewModel.PODate = item.g.InvoiceDate.ToString(Helpers.CommonHelper.DateFormat);
                itemDetailViewModel.POQty = item.f.Qty;
                itemDetailViewModel.GRNRate = item.f.Rate;
                itemDetailViewModel.ItemId = item.f.ItemId;
                itemDetailViewModel.InvoiceId = item.g.Id;
                itemDetailViewModel.InvoiceItemId = item.f.Id;

                // itemDetailViewModel.Vendor = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(item.g.VendorName)).Name;
                itemDetailViewModel.Vendor = item.g.Customer.Name;
                itemDetailViewModel.Phone = item.g.Customer.Phone1;
                itemDetailViewModel.Category = _dbContext.InvItemCategories.Where(x => x.Id == itemCategory.CategoryId).Select(x => x.Name).FirstOrDefault();
                itemDetailViewModel.Stock = itemCategory.StockQty;
                itemDetailList.Add(itemDetailViewModel);
            }

            return Ok(itemDetailList);
        }

       
        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ARSaleReturnInvoice invoice = _dbContext.ARSaleReturnInvoice
             .Include(c => c.Customer)
             .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                int voucherId;
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Invoice # : {0} of  " +
                "{1}",
                invoice.InvoiceNo,
                invoice.Customer.Name);

                voucherMaster.VoucherType = "SR";
                voucherMaster.ReferenceId = invoice.CustomerId;
                voucherMaster.VoucherDate = invoice.InvoiceDate;
                voucherMaster.Reference = "";
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate =1;
                voucherMaster.Description = "";
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Sales Return";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = invoice.TotalAmount;
                //Voucher Details
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //Credit Entry
                #region Customer Entry
                var invoiceItems = _dbContext.ARSaleReturnInvoiceItems.Where(i => i.SRInvoiceId == invoice.Id).ToList();
                var amount = invoiceItems.Sum(s => s.Amount);
                //Credit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = invoice.Customer.AccountId;
                voucherDetail.Sequence = 30;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
                voucherDetail.Credit = Math.Abs(amount);
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion Customer Entry
                //Debit Entry
                #region Sales Account
                var itemAccounts = (from li in _dbContext.ARSaleReturnInvoiceItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.SRInvoiceId == id
                                    select new
                                    {
                                        li.Amount,
                                        i.InvItemAccount.GLSaleAccountId
                                    }).GroupBy(l => l.GLSaleAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        Total = li.Sum(c => c.Amount),
                                        InvoiceId = li.FirstOrDefault().GLSaleAccountId //invoice id is temporarily containing GLSaleAccountId
                                    }).ToList();

                foreach (var item in itemAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.InvoiceId;
                    voucherDetail.Sequence = 10;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = Math.Abs(item.Total);
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Sales Account
                 
                //Create Voucher 
                VoucherHelper helper = new VoucherHelper(_dbContext, HttpContext);
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    invoice.VoucherId = voucherId;
                    invoice.Status = "Approved";
                    invoice.ApprovedBy = userId;
                    invoice.ApprovedDate = DateTime.Now;
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var invoiceItem in invoiceItems)
                            {
                                var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                                item.StockQty = item.StockQty + Math.Abs(invoiceItem.Qty);
                                item.StockValue = item.StockValue + (item.AvgRate * Math.Abs(invoiceItem.Qty));
                                //if (item.StockQty != 0)
                                //{
                                //    item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                                //}
                                var dbEntry = _dbContext.InvItems.Update(item);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
                            var entry = _dbContext.Update(invoice);
                            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();
                            TempData["error"] = "false";
                            TempData["message"] = "Sale Return Invoice has been approved successfully";
                            return RedirectToAction(nameof(Index));

                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.Message);
                            transaction.Rollback();
                            TempData["error"] = "true";
                            TempData["message"] = "Something went wrong while Approving.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message.ToString();
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong while Approving.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Getpacking(int Id)
        {
            if (Id != 0)
            {
               // var packing = _dbContext.ARPacking.Where(x => x.CustomerId == Id && x.IsApproved == true).ToList();
                var pendingPacking = (from p in _dbContext.ARPacking.Where(x => x.IsDeleted == false && x.CustomerId == Id && x.IsApproved == true).ToList()
                                                              where !_dbContext.ARSaleReturnInvoice.Where(x => x.IsDeleted == false).Any(s => p.Id.ToString().Contains(s.PackingId.ToString()))
                                                              select p
                                                              ).ToList();
                if (pendingPacking != null)
                {
                    return Ok(pendingPacking);
                }
                return Ok(null);
            }
            return Ok(null);
        }



    }
}
