using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Numbers.Repository.GL;

namespace Numbers.Areas.GL.Controllers
{
    [Area("GL")]
    [Authorize]
    public class DebitCreditMemoController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public DebitCreditMemoController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int? Id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            GLDebitCreditMemoViewModel viewModel = new GLDebitCreditMemoViewModel();
            var configValues = new ConfigValues(_dbContext);
            AppConfigHelper helper = new AppConfigHelper(_dbContext, HttpContext);


            viewModel.Suppliers = configValues.Supplier(companyId);
            viewModel.WareHouse = helper.GetWareHouses();
            viewModel.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(), "Id", "Description");
            viewModel.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            viewModel.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
            viewModel.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            viewModel.TransactionType = configValues.GetConfigValues("GL", "Transaction Type", companyId);
            viewModel.Tax = new SelectList(new AppTaxRepo(_dbContext).GetTaxes(companyId), "Id", "Name");
            ViewBag.Counter = 0;

            ViewBag.NavbarHeading = "Invoice & Debit/Credit Memo";
            if (Id != 0 && Id != null)
            {
                var memo = _dbContext.GLDebitCreditMemos.Find(Id);

                if (memo.TransactionTypeId == 868 || memo.TransactionTypeId== 870) // 868 mean Credit Note: 870 mean Invoice 
                {
                    viewModel.Party = new SelectList(_dbContext.ARCustomers.Include(x => x.City).Where(x => !x.IsDeleted).ToList().Select(x => new {
                        Id = x.Id,
                        Name = $"{x.Id} - {x.Name} - {x.City.Name}"
                    }), "Id", "Name");
                    viewModel.PartyType = "Customer";
                }
                else if (memo.TransactionTypeId == 869) // 869 mean Debit Note
                {
                    viewModel.Party = new SelectList(_dbContext.APSuppliers.Include(x => x.Cities).Where(x => x.IsActive).ToList().Select(x => new {
                        Id = x.Id,
                        Name = $"{x.Id} - {x.Name} - {x.Cities.Name}"
                    }), "Id", "Name");
                    viewModel.PartyType = "Supplier";
                }

                var list = _dbContext.GLDebitCreditMemoDetails.Where(i => i.GLDebitCreditMemoId == Id).ToArray();
                ViewBag.ServiceInvoices = list;
                viewModel.GLDebitCreditMemo = memo;
                viewModel.Tax = new SelectList(new AppTaxRepo(_dbContext).GetTaxes(companyId), "Id", "Name");
                return View(viewModel);
            }
            return View(viewModel);
        }
        public IActionResult PartialPurchaseService(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GLDebitCreditMemoViewModel viewModel = new GLDebitCreditMemoViewModel();

            ViewBag.Counter = counter;
            viewModel.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(), "Id", "Description");
            viewModel.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            viewModel.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
            viewModel.Tax = new SelectList(new AppTaxRepo(_dbContext).GetTaxes(companyId), "Id", "Name");

            return PartialView("_partialServiceInvoice", viewModel);
        }
        public IActionResult getCustomerOrVender(int transactionTypeId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GLDebitCreditMemoViewModel viewModel = new GLDebitCreditMemoViewModel();

            if (transactionTypeId == 868 || transactionTypeId == 870) // 868 mean Credit Note: 870 mean Invoice 
            {
                viewModel.Party = new SelectList(_dbContext.ARCustomers.Include(x=>x.City).Where(x => !x.IsDeleted).ToList().Select(x => new { 
                    Id = x.Id,
                    Name = $"{x.Id} - {x.Name} - {x.City.Name}"
                }), "Id", "Name");
                viewModel.PartyType = "Customer";
            }
            else if (transactionTypeId == 869) // 869 mean Debit Note
            {
                viewModel.Party = new SelectList(_dbContext.APSuppliers.Include(x=>x.Cities).Where(x => x.IsActive).ToList().Select(x => new {
                    Id = x.Id,
                    Name = $"{x.Id} - {x.Name} - {x.Cities.Name}"
                }), "Id", "Name");
                viewModel.PartyType = "Supplier";
            }

            return Ok(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(GLDebitCreditMemoViewModel viewModel, IFormCollection collection)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                if (viewModel.GLDebitCreditMemo.Id == 0)
                {
                    if (collection["GLDebitCreditMemoDetail.GLDebitCreditMemoId"].Count > 0)
                    {
                        int transactionNo = 1;
                        var result = _dbContext.GLDebitCreditMemos.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
                        if (result.Count() > 0)
                        {
                            transactionNo = result.Max(x => x.TransactionNo) + 1;
                        }
                        viewModel.GLDebitCreditMemo.CreatedBy = userId;
                        viewModel.GLDebitCreditMemo.CompanyId = companyId;
                        viewModel.GLDebitCreditMemo.CreatedDate = DateTime.Now;
                        viewModel.GLDebitCreditMemo.PartyType = (collection["PartyType"]);
                        viewModel.GLDebitCreditMemo.Total = Convert.ToDecimal(collection["Total"]);
                        viewModel.GLDebitCreditMemo.TotalDiscountAmount = Convert.ToDecimal(collection["TotalDiscountAmount"]);
                        viewModel.GLDebitCreditMemo.TotalSTAmount = Convert.ToDecimal(collection["TotalSalesTaxAmount"]);
                        viewModel.GLDebitCreditMemo.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                        viewModel.GLDebitCreditMemo.Status = "Created";
                        viewModel.GLDebitCreditMemo.TransactionNo = transactionNo;
                        _dbContext.GLDebitCreditMemos.Add(viewModel.GLDebitCreditMemo);
                        await _dbContext.SaveChangesAsync();

                        for (int i = 0; i < collection["GLDebitCreditMemoDetail.GLDebitCreditMemoId"].Count; i++)
                        {
                            var glAccountId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.GLAccountId"][i]);
                            var subAccountId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.SubAccountId"][i]);
                            var departmentId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.DepartmentId"][i]);
                            var subDepartmentId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.SubDepartmentId"][i]);
                            var costCenterId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.CostCenterId"][i]);
                            var remarks = Convert.ToString(collection["GLDebitCreditMemoDetail.Remarks"][i]);
                            var total = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.Total"][i]);
                            var discountAmount = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.DiscountAmount"][i]);
                            var discountPercentage = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.DiscountPercentage"][i]);
                            var taxId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.TaxId"][i]);
                            var taxAmount = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.TaxAmount"][i]);
                            var lineTotal = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.LineTotal"][i]);

                            var detail = new GLDebitCreditMemoDetail();
                            detail.GLDebitCreditMemoId = viewModel.GLDebitCreditMemo.Id;
                            detail.GLAccountId = glAccountId;
                            detail.SubAccountId = subAccountId;
                            detail.DepartmentId = departmentId;
                            detail.SubDepartmentId = subDepartmentId;
                            detail.CostCenterId = costCenterId;
                            detail.Remarks = remarks;
                            detail.Total = total;
                            detail.DiscountAmount = discountAmount;
                            detail.DiscountPercentage = discountPercentage;
                            detail.TaxId = taxId;
                            detail.TaxAmount = taxAmount;
                            detail.LineTotal = lineTotal;
                            _dbContext.GLDebitCreditMemoDetails.Add(detail);
                            await _dbContext.SaveChangesAsync();
                        }
                        TempData["error"] = "false";
                        TempData["message"] = "Invoice # " + viewModel.GLDebitCreditMemo.TransactionNo + " has been created successfully";
                        return RedirectToAction("Index", "DebitCreditMemo", new { area = "GL" });
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "No any Invoice has been created. It must contain atlest one item";
                        return RedirectToAction("Index", "DebitCreditMemo", new { area = "GL" });
                    }
                }
                else
                {
                    if (collection["GLDebitCreditMemoDetail.GLDebitCreditMemoId"].Count > 0)
                    {
                        GLDebitCreditMemo memo = _dbContext.GLDebitCreditMemos.FirstOrDefault(x => x.Id == viewModel.GLDebitCreditMemo.Id);
                        memo.TransactionTypeId = viewModel.GLDebitCreditMemo.TransactionTypeId;
                        memo.DocumentNo = viewModel.GLDebitCreditMemo.DocumentNo;
                        memo.OperatingUnitId = viewModel.GLDebitCreditMemo.OperatingUnitId;
                        memo.PartyId = viewModel.GLDebitCreditMemo.PartyId;
                        memo.ReferenceNo = viewModel.GLDebitCreditMemo.ReferenceNo;
                        memo.DocumentDate = viewModel.GLDebitCreditMemo.DocumentDate;
                        memo.UpdatedBy = userId;
                        memo.UpdatedDate = DateTime.Now;
                        memo.Total = Convert.ToDecimal(collection["Total"]);
                        memo.TotalDiscountAmount = Convert.ToDecimal(collection["TotalDiscountAmount"]);
                        memo.TotalSTAmount = Convert.ToDecimal(collection["TotalSalesTaxAmount"]);
                        memo.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);

                        _dbContext.GLDebitCreditMemos.Update(memo);
                        await _dbContext.SaveChangesAsync();
                        var existingDetail = _dbContext.GLDebitCreditMemoDetails.Where(x => x.GLDebitCreditMemoId == memo.Id).ToList();
                        //Deleting monthly target limit
                        List<int> ids = new List<int>();
                        for (int i = 0; i < (collection["GLDebitCreditMemoDetail.Id"]).Count; i++)
                        {
                            ids.Add(Convert.ToInt32(collection["GLDebitCreditMemoDetail.Id"][i]));
                        }
                        int x = 0;
                        foreach (var detail in existingDetail)
                        {
                            bool isExist = ids.Any(x => x == detail.Id);
                            if (!isExist)
                            {
                                //Handling Balance
                                //var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == saleReturn.IGPId);
                                //igp.BaleBalance = igp.BaleBalance + detail.BalesBalance;
                                //_dbContext.ARInwardGatePass.Update(igp);
                                //----------
                                _dbContext.GLDebitCreditMemoDetails.Remove(detail);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        for (int i = 0; i < collection["GLDebitCreditMemoDetail.GLDebitCreditMemoId"].Count; i++)
                        {
                            var detailId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.Id"][i]);
                            var glAccountId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.GLAccountId"][i]);
                            var subAccountId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.SubAccountId"][i]);
                            var departmentId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.DepartmentId"][i]);
                            var subDepartmentId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.SubDepartmentId"][i]);
                            var costCenterId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.CostCenterId"][i]);
                            var remarks = Convert.ToString(collection["GLDebitCreditMemoDetail.Remarks"][i]);
                            var total = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.Total"][i]);
                            var discountAmount = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.DiscountAmount"][i]);
                            var discountPercentage = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.DiscountPercentage"][i]);
                            var taxId = Convert.ToInt32(collection["GLDebitCreditMemoDetail.TaxId"][i]);
                            var taxAmount = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.TaxAmount"][i]);
                            var lineTotal = Convert.ToDecimal(collection["GLDebitCreditMemoDetail.LineTotal"][i]);

                            if (detailId == 0) //Inserting New Records
                            {
                                var detail = new GLDebitCreditMemoDetail();
                                detail.GLDebitCreditMemoId = memo.Id;
                                detail.GLAccountId = glAccountId;
                                detail.SubAccountId = subAccountId;
                                detail.DepartmentId = departmentId;
                                detail.SubDepartmentId = subDepartmentId;
                                detail.CostCenterId = costCenterId;
                                detail.Remarks = remarks;
                                detail.Total = total;
                                detail.DiscountAmount = discountAmount;
                                detail.DiscountPercentage = discountPercentage;
                                detail.TaxId = taxId;
                                detail.TaxAmount = taxAmount;
                                detail.LineTotal = lineTotal;

                                _dbContext.GLDebitCreditMemoDetails.Add(detail);
                            }
                            else
                            {
                                GLDebitCreditMemoDetail Items = _dbContext.GLDebitCreditMemoDetails.FirstOrDefault(x => x.Id == detailId);
                                Items.GLAccountId = glAccountId;
                                Items.SubAccountId = subAccountId;
                                Items.DepartmentId = departmentId;
                                Items.SubDepartmentId = subDepartmentId;
                                Items.CostCenterId = costCenterId;
                                Items.Remarks = remarks;
                                Items.Total = total;
                                Items.DiscountAmount = discountAmount;
                                Items.DiscountPercentage = discountPercentage;
                                Items.TaxId = taxId;
                                Items.TaxAmount = taxAmount;
                                Items.LineTotal = lineTotal;

                                _dbContext.GLDebitCreditMemoDetails.Update(Items);
                            }
                            await _dbContext.SaveChangesAsync();
                        }
                        TempData["error"] = "false";
                        TempData["message"] = "Invoice # " + viewModel.GLDebitCreditMemo.TransactionNo + " has been created successfully";
                        return RedirectToAction("Index", "DebitCreditMemo", new { area = "GL" });
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "No any Invoice has been created. It must contain atlest one item";
                        return RedirectToAction("Index", "DebitCreditMemo", new { area = "GL" });
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["error"] = "true";
                TempData["message"] = ex.Message == null ? ex.InnerException.Message.ToString() : ex.Message.ToString();
                return RedirectToAction("Index", "DebitCreditMemo", new { area = "GL" });
            }
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Invoice & Debit/Credit Memos";
            return View();
        }
        public IActionResult GetList()
        {
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchTransDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchTransactionType = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchParty = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchDocumentNo = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchDocumentDate = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.GLDebitCreditMemos.Include(x => x.TransactionType).Where(x => x.IsDeleted != true && x.CompanyId == companyId) select records);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchTransDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchTransactionType) ? Data.Where(m => m.TransactionType.ConfigValue.ToString().ToUpper().Contains(searchTransactionType.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchParty) ? Data.Where(m => m.PartyType == "Customer" ?
                                _dbContext.ARCustomers.Include(x => x.City).Where(x => x.Id == m.PartyId).Select(x => new { Name = $"{x.Id} - {x.Name} - {x.City.Name}" }).FirstOrDefault().ToString().ToUpper().Contains(searchParty.ToUpper()) :
                                _dbContext.APSuppliers.Include(x => x.Cities).Where(x => x.Id == m.PartyId).Select(x => new { Name = $"{x.Id} - {x.Name} - {x.Cities.Name}" }).FirstOrDefault().ToString().ToUpper().Contains(searchParty.ToUpper()) ) : Data;

                Data = !string.IsNullOrEmpty(searchDocumentNo) ? Data.Where(m => m.DocumentNo != null ? m.DocumentNo.ToString().ToUpper().Contains(searchDocumentNo.ToUpper()): false) : Data;
                Data = !string.IsNullOrEmpty(searchDocumentDate) ? Data.Where(m => m.DocumentDate != null ? m.DocumentDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDocumentDate.ToUpper()) : false) : Data;

                //recordsTotal = Data.Count();

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

                //var data = Data.Skip(skip).Take(pageSize).ToList();

                List<GLDebitCreditMemoViewModel> viewModel = new List<GLDebitCreditMemoViewModel>();
                foreach (var item in data)
                {
                    GLDebitCreditMemoViewModel model = new GLDebitCreditMemoViewModel();
                    model.GLDebitCreditMemo = item;
                    model.PartyType = item.PartyType == "Customer" ?
                                _dbContext.ARCustomers.Include(x => x.City).Where(x => x.Id == item.PartyId).Select(x => new { Name = $"{x.Id} - {x.Name} - {x.City.Name}" }).FirstOrDefault().Name:
                                _dbContext.APSuppliers.Include(x => x.Cities).Where(x => x.Id == item.PartyId).Select(x => new { Name = $"{x.Id} - {x.Name} - {x.Cities.Name}" }).FirstOrDefault().Name;
                    model.TransactionDate = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.DocumentDate = item.DocumentDate.ToString(Helpers.CommonHelper.DateFormat);
                    viewModel.Add(model);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = viewModel };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var Voucher = new DebitCreditMemoRepository(_dbContext, HttpContext);
            bool isSuccess = await Voucher.Approve(id, userId, companyId);
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
        [HttpPost]
        public IActionResult GetInvoiceItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var item = _dbContext.GLDebitCreditMemoDetails.Include(i => i.GLDebitCreditMemo).Where(i => i.Id == id).FirstOrDefault();
            GLDebitCreditMemoViewModel viewModel = new GLDebitCreditMemoViewModel();
            ViewBag.Counter = id;
            viewModel.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(), "Id", "Description");
            viewModel.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            viewModel.SubDepartment = new SelectList(_dbContext.GLSubDivision.Where(x => x.GLDivisionId == item.DepartmentId).ToList(), "Id", "Name");
            viewModel.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.SubDivisionId == item.SubDepartmentId &&  x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
            if (item != null)
            {
                viewModel.GLDebitCreditMemoDetail = item;
                ViewBag.ItemId = itemId;
                viewModel.Tax = new SelectList(new AppTaxRepo(_dbContext).GetTaxes(companyId), "Id", "Name");
            }
            return PartialView("_partialServiceInvoice", viewModel);
        }
        public async Task<IActionResult> Delete(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Repo = new DebitCreditMemoRepository(_dbContext);
            bool isSuccess = await Repo.Delete(id, _companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Debit/Credit Memo & Invoice has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Debit/Credit Memo & Invoice not found";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Details(int Id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            GLDebitCreditMemoViewModel viewModel = new GLDebitCreditMemoViewModel();
            var configValues = new ConfigValues(_dbContext);
            AppConfigHelper helper = new AppConfigHelper(_dbContext, HttpContext);


            viewModel.Suppliers = configValues.Supplier(companyId);
            viewModel.WareHouse = helper.GetWareHouses();
            viewModel.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(), "Id", "Description");
            viewModel.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            viewModel.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
            viewModel.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            viewModel.TransactionType = configValues.GetConfigValues("GL", "Transaction Type", companyId);
            viewModel.Tax = new SelectList(new AppTaxRepo(_dbContext).GetTaxes(companyId), "Id", "Name");
            ViewBag.Counter = 0;

            ViewBag.NavbarHeading = "Invoice & Debit/Credit Memo";
            if (Id != 0)
            {
                var memo = _dbContext.GLDebitCreditMemos.Find(Id);

                if (memo.TransactionTypeId == 868 || memo.TransactionTypeId == 870) // 868 mean Credit Note: 870 mean Invoice 
                {
                    viewModel.Party = new SelectList(_dbContext.ARCustomers.Include(x => x.City).Where(x => !x.IsDeleted).ToList().Select(x => new {
                        Id = x.Id,
                        Name = $"{x.Id} - {x.Name} - {x.City.Name}"
                    }), "Id", "Name");
                    viewModel.PartyType = "Customer";
                }
                else if (memo.TransactionTypeId == 869) // 869 mean Debit Note
                {
                    viewModel.Party = new SelectList(_dbContext.APSuppliers.Include(x => x.Cities).Where(x => x.IsActive).ToList().Select(x => new {
                        Id = x.Id,
                        Name = $"{x.Id} - {x.Name} - {x.Cities.Name}"
                    }), "Id", "Name");
                    viewModel.PartyType = "Supplier";
                }

                var list = _dbContext.GLDebitCreditMemoDetails.Where(i => i.GLDebitCreditMemoId == Id).ToArray();
                ViewBag.ServiceInvoices = list;
                viewModel.GLDebitCreditMemo = memo;
                viewModel.Tax = new SelectList(new AppTaxRepo(_dbContext).GetTaxes(companyId), "Id", "Name");
                return View(viewModel);
            }
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult GetInvoiceItemsDetails(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var item = _dbContext.GLDebitCreditMemoDetails.Include(i => i.GLDebitCreditMemo).Where(i => i.Id == id).FirstOrDefault();
            GLDebitCreditMemoViewModel viewModel = new GLDebitCreditMemoViewModel();
            ViewBag.Counter = id;
            viewModel.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(), "Id", "Description");
            viewModel.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            viewModel.SubDepartment = new SelectList(_dbContext.GLSubDivision.Where(x => x.GLDivisionId == item.DepartmentId).ToList(), "Id", "Name");
            viewModel.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.SubDivisionId == item.SubDepartmentId && x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
            if (item != null)
            {
                viewModel.GLDebitCreditMemoDetail = item;
                ViewBag.ItemId = itemId;
                viewModel.Tax = new SelectList(new AppTaxRepo(_dbContext).GetTaxes(companyId), "Id", "Name");
            }
            return PartialView("_partialServiceInvoiceDetails", viewModel);
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var paymentRepo = new DebitCreditMemoRepository(_dbContext);
            var receipt = await paymentRepo.UnApproveVoucher(id, companyId);
            if (receipt == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Debit/Credit Memo & Invoice not Approved";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Debit/Credit Memo & Invoice has been Un-Approved successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
