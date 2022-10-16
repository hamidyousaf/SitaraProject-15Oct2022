using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Helpers;
using Numbers.Helpers;
namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class CommissionAgentPaymentGenerationController : Controller
    {
        private readonly ARCommissionAgentPaymentGenerationRepository _aRCommissionAgentPaymentGenerationRepository;
        private readonly ARCommissionAgentPaymentGenerationDetailsRepository _aRCommissionAgentPaymentGenerationDetailsRepository;
        private readonly NumbersDbContext _dbContext;

        public CommissionAgentPaymentGenerationController(ARCommissionAgentPaymentGenerationRepository aRCommissionAgentPaymentGenerationRepository, NumbersDbContext dbContext, ARCommissionAgentPaymentGenerationDetailsRepository aRCommissionAgentPaymentGenerationDetailsRepository)
        {
            _aRCommissionAgentPaymentGenerationRepository = aRCommissionAgentPaymentGenerationRepository;
            _aRCommissionAgentPaymentGenerationDetailsRepository = aRCommissionAgentPaymentGenerationDetailsRepository;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Commission Agent Payment Generation";
            return View();
        }
        public IActionResult GetCommissionAgentPaymentsList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int Resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            //var ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId));
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchAgent = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchStartDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchEndDate = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var CommissionAgentPaymentGeneration = _aRCommissionAgentPaymentGenerationRepository.Get().Include(p => p.ARCommissionAgent).Where(p => p.IsDeleted == false && p.CompanyId == companyId && p.Resp_Id == Resp_Id);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    CommissionAgentPaymentGeneration = CommissionAgentPaymentGeneration.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                CommissionAgentPaymentGeneration = !string.IsNullOrEmpty(searchTransNo) ? CommissionAgentPaymentGeneration.Where(m => m.TransactionNo.ToString().Contains(searchTransNo)) : CommissionAgentPaymentGeneration;
                CommissionAgentPaymentGeneration = !string.IsNullOrEmpty(searchAgent) ? CommissionAgentPaymentGeneration.Where(m => m.ARCommissionAgent.Name.ToLower().Contains(searchAgent.ToLower())) : CommissionAgentPaymentGeneration;
                CommissionAgentPaymentGeneration = !string.IsNullOrEmpty(searchGrandTotal) ? CommissionAgentPaymentGeneration.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : CommissionAgentPaymentGeneration;
                CommissionAgentPaymentGeneration = !string.IsNullOrEmpty(searchStartDate) ? CommissionAgentPaymentGeneration.Where(m => m.StartDate.ToString(Helpers.CommonHelper.DateFormat).ToLower().Contains(searchStartDate.ToLower())) : CommissionAgentPaymentGeneration;
                CommissionAgentPaymentGeneration = !string.IsNullOrEmpty(searchEndDate) ? CommissionAgentPaymentGeneration.Where(m => m.EndDate.ToString(Helpers.CommonHelper.DateFormat).ToLower().Contains(searchEndDate.ToLower())) : CommissionAgentPaymentGeneration;

                recordsTotal = CommissionAgentPaymentGeneration.Count();
                var data = CommissionAgentPaymentGeneration.Skip(skip).Take(pageSize).ToList();
                var Details = new List<ARCommissionAgentPaymentGenerationViewModel>();
                foreach (var grp in data)
                {
                    var CommissionAgentPayment = new ARCommissionAgentPaymentGenerationViewModel();
                    CommissionAgentPayment.Id = grp.Id;
                    CommissionAgentPayment.EndDate = grp.EndDate.ToString("dd-MMM-yyyy");
                    CommissionAgentPayment.StartDate = grp.StartDate.ToString("dd-MMM-yyyy");
                    CommissionAgentPayment.CommissionAgentName = grp.ARCommissionAgent.Name;
                    CommissionAgentPayment.GrandTotal = grp.GrandTotal;
                    CommissionAgentPayment.TransactionNo = grp.TransactionNo;
                    CommissionAgentPayment.IsApproved = grp.IsApproved;
                    Details.Add(CommissionAgentPayment);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult Create(int? id)
        {
            var CommissionAgentPayment = new ARCommissionAgentPaymentGenerationViewModel();
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string UserId = HttpContext.Session.GetString("UserId");
            CommissionAgentPayment.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId));
            CommissionAgentPayment.CommissionAgents = new SelectList(from ac in _dbContext.ARCommissionAgents.Where(x => x.IsDeleted != true && x.IsActive != false && x.CompanyId == companyId).ToList()
                                                                     select new
                                                                     {
                                                                         Id = ac.Id,
                                                                         Name = ac.Name
                                                                     }, "Id", "Name");
            if (id == null)
            {

            }
            else
            {
                var data = _aRCommissionAgentPaymentGenerationRepository
                    .Get(p => p.Id == id & p.IsDeleted != true)
                    .Include(p => p.ARCommissionAgentPaymentGenerationDetails).ThenInclude(p => p.ARCustomers).ThenInclude(p => p.SalesPerson)
                    .Include(p => p.ARCommissionAgentPaymentGenerationDetails).ThenInclude(p => p.InvItemCategories)
                    .FirstOrDefault();
                CommissionAgentPayment.Id = data.Id;
                CommissionAgentPayment.GrandTotal = data.GrandTotal;
                CommissionAgentPayment.ARCommissionAgentPaymentGenerationDetails = data.ARCommissionAgentPaymentGenerationDetails.AsQueryable();
                CommissionAgentPayment.CommissionAgentId = data.CommissionAgentId;
                CommissionAgentPayment.StartDate = Helpers.CommonHelper.FormatDate(data.StartDate);
                CommissionAgentPayment.EndDate = Helpers.CommonHelper.FormatDate(data.EndDate);
                CommissionAgentPayment.ProductTypeId = data.ProductTypeId;
                CommissionAgentPayment.TransactionNo = data.TransactionNo;
                CommissionAgentPayment.Status = "update";
            }
            return View(CommissionAgentPayment);
        }
        public IActionResult Submit(ARCommissionAgentPaymentGenerationViewModel aRCommissionAgentPaymentViewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            try
            {
                if (aRCommissionAgentPaymentViewModel.Id == 0)
                {
                    var model = new ARCommissionAgentPaymentGeneration
                    {
                        CommissionAgentId = aRCommissionAgentPaymentViewModel.CommissionAgentId,
                        CompanyId = companyId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now.Date,
                        GrandTotal = aRCommissionAgentPaymentViewModel.GrandTotal ?? 0,
                        EndDate = Convert.ToDateTime(aRCommissionAgentPaymentViewModel.EndDate),
                        StartDate = Convert.ToDateTime(aRCommissionAgentPaymentViewModel.StartDate),
                        IsActive = true,
                        IsApproved = false,
                        TransactionNo = this.Max(),
                        IsDeleted = false,
                        Resp_Id = resp_Id,
                        ProductTypeId = aRCommissionAgentPaymentViewModel.ProductTypeId
                    };
                    _dbContext.ARCommissionAgentPaymentGeneration.Add(model);
                    _dbContext.SaveChanges();
                    // Adding Child items
                    var Qty = collection["Qty"].ToString().Contains(",") ? collection["Qty"].ToString().Split(',').ToList() : collection["Qty"].ToList();
                    var Amount = collection["Amount"].ToString().Contains(",") ? collection["Amount"].ToString().Split(',').ToList() : collection["Amount"].ToList();
                    var CommissionPercentage = collection["Percentage"].ToString().Contains(",") ? collection["Percentage"].ToString().Split(',').ToList() : collection["Percentage"].ToList();
                    var CommissionAmount = collection["comAmt"].ToString().Contains(",") ? collection["comAmt"].ToString().Split(',').ToList() : collection["comAmt"].ToList();
                    var CustomerId = collection["customerId"].ToString().Contains(",") ? collection["customerId"].ToString().Split(',').ToList() : collection["customerId"].ToList();
                    var CategoryId = collection["categoryId"].ToString().Contains(",") ? collection["categoryId"].ToString().Split(',').ToList() : collection["categoryId"].ToList();

                    for (int i = 0; i < collection["Qty"].Count; i++)
                    {
                        var CommissionAgentPaymentDetail = new ARCommissionAgentPaymentGenerationDetails
                        {
                            CommissionAgentPaymentId = model.Id,
                            Qty = Convert.ToInt32(Qty[i]),
                            CustomerId = Convert.ToInt32(CustomerId[i]),
                            CategoryId = Convert.ToInt32(CategoryId[i]),
                            Amount = Convert.ToDecimal(Amount[i]),
                            CommissionPercentge = Convert.ToDecimal(CommissionPercentage[i]),
                            CommissionAmount = Convert.ToDecimal(CommissionAmount[i]),
                            Resp_Id = resp_Id,
                            CreatedDate = DateTime.Now.Date,
                            CreatedBy = userId,
                            IsDeleted = false,
                            CompanyId = companyId
                        };
                        var result = _dbContext.ARCommissionAgentPaymentGenerationDetails.Add(CommissionAgentPaymentDetail);
                        _dbContext.SaveChanges();
                        var Ids = collection["DcIds"][i].Split(',');
                        for (int z = 0; z < Ids.Count(); z++)
                        {
                            var data = new ARDeliveryChallanComAgentPayGenDetails
                            {
                                CommissionAgentPaymentGenDetailsId = result.Entity.Id,
                                DeliveryChallanItemId = Convert.ToInt32(Ids[z])
                            };
                            _dbContext.ARDeliveryChallanComAgentPayGenDetails.Add(data);
                            _dbContext.SaveChanges();
                        }
                    }

                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Commission Payment Generation No. {0} has been saved successfully.", model.TransactionNo);
                }
                else
                {
                    var commissionAgentPaymentGenUpdate = _aRCommissionAgentPaymentGenerationRepository.Get(x => x.Id == aRCommissionAgentPaymentViewModel.Id && x.IsDeleted != true).Include(p => p.ARCommissionAgentPaymentGenerationDetails).FirstOrDefault();
                    commissionAgentPaymentGenUpdate.UpdatedBy = userId;
                    commissionAgentPaymentGenUpdate.UpdatedDate = DateTime.Now;
                    commissionAgentPaymentGenUpdate.GrandTotal = aRCommissionAgentPaymentViewModel.GrandTotal ?? 0;
                    _dbContext.ARCommissionAgentPaymentGeneration.Update(commissionAgentPaymentGenUpdate);
                    _dbContext.SaveChanges();
                    var rownber = collection["ChildId"].Count;
                    List<int> ChildList = new List<int>();
                    for (int i = 0; i < rownber; i++)
                    {
                        int id = Convert.ToInt32(collection["ChildId"][i]);
                        ChildList.Add(id);
                    }
                    var foundDetails = commissionAgentPaymentGenUpdate.ARCommissionAgentPaymentGenerationDetails.ToList();
                    if (!ReferenceEquals(ChildList, null))
                    {
                        for (int i = 0; i < foundDetails.Count; i++)
                        {
                            bool result = ChildList.Exists(s => s == foundDetails[i].Id);
                            if (!result)
                            {
                                var delete = _dbContext.ARDeliveryChallanComAgentPayGenDetails.Where(x => x.CommissionAgentPaymentGenDetailsId == foundDetails[i].Id).AsQueryable();
                                _dbContext.ARDeliveryChallanComAgentPayGenDetails.RemoveRange(delete);
                                _dbContext.ARCommissionAgentPaymentGenerationDetails.Remove(foundDetails[i]);
                                _dbContext.SaveChanges();
                            }
                        }
                    }
                    var UpdateList = new List<ARCommissionAgentPaymentGenerationDetails>();
                    for (int i = 0; i < ChildList.Count; i++)
                    {
                        var detail = foundDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["ChildId"][i]));
                        if (!ReferenceEquals(detail, null))
                        {
                            detail.CommissionPercentge = Convert.ToDecimal(collection["Percentage"][i]);
                            detail.CommissionAmount = Convert.ToDecimal(collection["comAmt"][i]);
                            detail.Amount = Convert.ToDecimal(collection["Amount"][i]);
                            UpdateList.Add(detail);
                        }
                    }
                    _dbContext.ARCommissionAgentPaymentGenerationDetails.UpdateRange(UpdateList);
                    _dbContext.SaveChanges();
                    var Qty = collection["Qty"].ToString().Contains(",") ? collection["Qty"].ToString().Split(',').ToList() : collection["Qty"].ToList();
                    var Amount = collection["Amount"].ToString().Contains(",") ? collection["Amount"].ToString().Split(',').ToList() : collection["Amount"].ToList();
                    var CommissionPercentage = collection["Percentage"].ToString().Contains(",") ? collection["Percentage"].ToString().Split(',').ToList() : collection["Percentage"].ToList();
                    var CommissionAmount = collection["comAmt"].ToString().Contains(",") ? collection["comAmt"].ToString().Split(',').ToList() : collection["comAmt"].ToList();
                    var CustomerId = collection["customerId"].ToString().Contains(",") ? collection["customerId"].ToString().Split(',').ToList() : collection["customerId"].ToList();
                    var CategoryId = collection["categoryId"].ToString().Contains(",") ? collection["categoryId"].ToString().Split(',').ToList() : collection["categoryId"].ToList();

                    for (int i = 0; i < collection["Qty"].Count; i++)
                    {
                        var Ids = collection["DcIds"][i] == "" ? Array.Empty<string>() : collection["DcIds"][i].Split(',');
                        if (Ids.Count() > 0)
                        {
                            var CommissionAgentPaymentDetail = new ARCommissionAgentPaymentGenerationDetails
                            {
                                CommissionAgentPaymentId = aRCommissionAgentPaymentViewModel.Id,
                                Qty = Convert.ToInt32(Qty[i]),
                                CustomerId = Convert.ToInt32(CustomerId[i]),
                                CategoryId = Convert.ToInt32(CategoryId[i]),
                                Amount = Convert.ToDecimal(Amount[i]),
                                CommissionPercentge = Convert.ToDecimal(CommissionPercentage[i]),
                                CommissionAmount = Convert.ToDecimal(CommissionAmount[i]),
                                Resp_Id = resp_Id,
                                CreatedDate = DateTime.Now.Date,
                                CreatedBy = userId,
                                IsDeleted = false,
                                CompanyId = companyId
                            };
                            var result = _dbContext.ARCommissionAgentPaymentGenerationDetails.Add(CommissionAgentPaymentDetail);
                            _dbContext.SaveChanges();
                            for (int z = 0; z < Ids.Count(); z++)
                            {
                                var data = new ARDeliveryChallanComAgentPayGenDetails
                                {
                                    CommissionAgentPaymentGenDetailsId = result.Entity.Id,
                                    DeliveryChallanItemId = Convert.ToInt32(Ids[z])
                                };
                                _dbContext.ARDeliveryChallanComAgentPayGenDetails.Add(data);
                                _dbContext.SaveChanges();
                            }
                        }
                    }
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Commission Agent Payment Generation No. {0} has been updated successfully.", commissionAgentPaymentGenUpdate.TransactionNo);
                }
            }
            catch (Exception ex)
            {
                string valuessss = null;
                var entries = _dbContext.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged);
                foreach (var entry in entries)
                {
                    foreach (var prop in entry.CurrentValues.Properties)
                    {
                        var val = prop.PropertyInfo.GetValue(entry.Entity);
                        valuessss = $"{prop.ToString()} ~ ({val?.ToString().Length})({val})" + "\n";

                    }
                }

                TempData["error"] = "true";
                TempData["message"] = "Something went wrong";
            }

            return RedirectToAction("Index", "CommissionAgentPaymentGeneration");
        }
        public IActionResult GetCityCommissionAgent(int CommissionAgentId)
        {
            var city = _dbContext.ARCommissionAgents.Include(p => p.City).Where(p => p.Id == CommissionAgentId).FirstOrDefault()?.City?.Name;
            return Ok(city);
        }
        public async Task<IActionResult> GetCustomersByCommissionAgent(int CommissionAgentId, string StartDate, string EndDate, int ProductTypeId)
        {
            DateTime _StartDate = Convert.ToDateTime(StartDate);
            DateTime _EndDate = Convert.ToDateTime(EndDate);

            var DeliveryChallanComAgentPayDetails = await _dbContext.ARDeliveryChallanComAgentPayGenDetails
                .Include(p => p.CommissionAgentPaymentGenerationDetails).ThenInclude(p => p.ARCommissionAgentPaymentGeneration).ThenInclude(p => p.ARCommissionAgent)
                .Where(p => p.CommissionAgentPaymentGenerationDetails.ARCommissionAgentPaymentGeneration.CommissionAgentId == CommissionAgentId).ToListAsync();


            var CommissionAgentPaymentGenerationDetails = _dbContext.ARCommissionAgentPaymentGenerationDetails
                .Include(p => p.ARCommissionAgentPaymentGeneration)
                .Where(p => p.IsDeleted != true && p.ARCommissionAgentPaymentGeneration.CommissionAgentId == CommissionAgentId && p.ARCommissionAgentPaymentGeneration.StartDate >= _StartDate && p.ARCommissionAgentPaymentGeneration.EndDate <= _EndDate);

            var InvCategories = _dbContext.InvItemCategories;
            var CommissionAgentCustomers = _dbContext.ARCommissionAgentCustomer;
            var CategoryLevels = from L1 in InvCategories
                                 join L2 in InvCategories on L1.Id equals L2.ParentId
                                 join L3 in InvCategories on L2.Id equals L3.ParentId
                                 join L4 in InvCategories on L3.Id equals L4.ParentId
                                 join item in _dbContext.InvItems on L4.Id equals item.CategoryId
                                 where L1.IsDeleted == false && L2.IsDeleted == false && L3.IsDeleted == false && L4.IsDeleted == false && item.IsDeleted == false
                                 select new
                                 {
                                     //Level2Id = L2.Id,
                                     Level3Id = L3.Id,
                                     Level4Id = L4.Id,
                                     ItemId = item.Id
                                 };
            var invoiceItems = _dbContext.ARInvoiceItems
                .Include(p => p.Invoice).ThenInclude(p => p.Customer).ThenInclude(p => p.SalesPerson)
                .Include(p => p.ARDeliveryChallanItem).ThenInclude(p => p.DC)
                .Include(p => p.ARDeliveryChallanItem).ThenInclude(p => p.ARSaleOrderItem).ThenInclude(p => p.Item)
                .Where(p =>
                   p.Invoice.Customer.ProductTypeId == ProductTypeId
                && p.IsDeleted != true && p.Invoice.IsDeleted != true && p.ARDeliveryChallanItem.IsDeleted != true && p.ARDeliveryChallanItem.DC.IsDeleted != true
                && CommissionAgentCustomers.Any(x => x.CommissionAgent_Id == CommissionAgentId && x.Customer_Id == p.Invoice.CustomerId)
                && p.ARDeliveryChallanItem.DC.DCDate.Date >= _StartDate.Date
                && p.ARDeliveryChallanItem.DC.DCDate.Date <= _EndDate.Date
                && p.Invoice.Status == "Approved" && p.ARDeliveryChallanItem.DC.Status == "Approved"
                && !DeliveryChallanComAgentPayDetails.Any(x => x.DeliveryChallanItemId == p.ARDeliveryChallanItem.Id)
                ).ToList()
                .GroupBy(p => new { p.Item.CategoryId, p.Invoice.CustomerId })
                .Select(p => new
                {
                    DCIds = p.Select(p => p.ARDeliveryChallanItem.Id),
                    Customer = p.FirstOrDefault().Invoice.Customer.Name,
                    CustomerId = p.FirstOrDefault().Invoice.CustomerId,
                    SalesPerson = p.FirstOrDefault().Invoice.Customer.SalesPerson.Name,
                    TotalAmount = p.Sum(p => p.LineTotal),
                    TotalQty = p.Sum(p => p.Qty),
                    CategoryIdLevel4 = p.FirstOrDefault().Item.CategoryId,
                    CategoryIdLevel3 = CategoryLevels.Where(x => x.Level4Id == p.FirstOrDefault().Item.CategoryId).Select(p => p.Level3Id).FirstOrDefault(),
                }).ToList().GroupBy(p => new { p.CategoryIdLevel3, p.CustomerId }).Select(p => new
                {
                    DCIds = p.FirstOrDefault().DCIds,
                    Customer = p.FirstOrDefault().Customer,
                    CustomerId = p.FirstOrDefault().CustomerId,
                    SalesPerson = p.FirstOrDefault().SalesPerson,
                    TotalAmount = p.Sum(p => p.TotalAmount),
                    TotalQty = p.Sum(p => p.TotalQty),
                    CategoryIdLevel3 = p.FirstOrDefault().CategoryIdLevel3,
                    CategoryNameLevel3 = InvCategories.Where(x => x.Id == p.FirstOrDefault().CategoryIdLevel3).FirstOrDefault().Name
                }).ToList();
            return Ok(invoiceItems);
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            var commissionAgentPaymentGeneration = _dbContext.ARCommissionAgentPaymentGeneration.Include(p => p.ARCommissionAgent)
             .Where(a => a.CompanyId == _companyId && a.Id == id && a.IsDeleted != true && a.IsApproved != true)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                Helpers.VoucherHelper voucher = new Helpers.VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Commission Agent Payment Generation # : {0} of {1} ",
                commissionAgentPaymentGeneration.TransactionNo,
                commissionAgentPaymentGeneration.ARCommissionAgent.Name
                );

                int voucherId;
                voucherMaster.VoucherType = "COMAPAYG";
                voucherMaster.VoucherDate = DateTime.Now;
                voucherMaster.Currency = "PKR";
                voucherMaster.Description = voucherDescription;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/CommissionAgentPaymentGeneration";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = (decimal)commissionAgentPaymentGeneration.GrandTotal;

                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = _dbContext.GLAccounts.Where(p => p.Name.ToUpper().Trim() == "COMMISSION EXPENSE").SingleOrDefault(p => p.IsDeleted != true).Id;
                voucherDetail.Sequence = 1;
                voucherDetail.Description = voucherDescription;
                voucherDetail.Debit = (decimal)commissionAgentPaymentGeneration.GrandTotal;
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                GLVoucherDetail voucherDetailItem = new GLVoucherDetail();
                voucherDetailItem.AccountId = _dbContext.GLAccounts.Where(p => p.Name.ToUpper().Trim() == "LOCAL COMMISSION PAYABLE").SingleOrDefault(p => p.IsDeleted != true).Id;
                voucherDetailItem.Sequence = 1;
                voucherDetailItem.Description = voucherDescription;
                voucherDetailItem.Debit = 0;
                voucherDetailItem.Credit = (decimal)commissionAgentPaymentGeneration.GrandTotal;
                voucherDetailItem.IsDeleted = false;
                voucherDetailItem.CreatedBy = _userId;
                voucherDetailItem.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetailItem);

                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    commissionAgentPaymentGeneration.VoucherId = voucherId;
                    commissionAgentPaymentGeneration.ApprovedBy = _userId;
                    commissionAgentPaymentGeneration.ApprovedDate = DateTime.Now;
                    commissionAgentPaymentGeneration.IsApproved = true;
                    //On approval updating Commission agent payment generation
                    TempData["error"] = "false";
                    TempData["message"] = "Customer Agent Payment Generation has been approved successfully";
                    var entry = _dbContext.ARCommissionAgentPaymentGeneration.Update(commissionAgentPaymentGeneration);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    _dbContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.InnerException.Message == null ? ex.Message.ToString() : ex.InnerException.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var commissionAgentPaymentGeneration = _dbContext.ARCommissionAgentPaymentGeneration
                            .Where(v => v.IsDeleted == false && v.Id == id && v.IsApproved == true && v.CompanyId == companyId).FirstOrDefault();
            if (commissionAgentPaymentGeneration == null)
            {
                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                var voucher = _dbContext.GLVouchers.Find(commissionAgentPaymentGeneration.VoucherId);
                if (voucher != null)
                {
                    var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == commissionAgentPaymentGeneration.VoucherId).ToList();
                    foreach (var item in voucherDetail)
                    {
                        var tracker = _dbContext.GLVoucherDetails.Remove(item);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        _dbContext.SaveChanges();
                    }
                    _dbContext.GLVouchers.Update(voucher);
                    _dbContext.SaveChanges();
                }
                commissionAgentPaymentGeneration.UnapprovedBy = userId;
                commissionAgentPaymentGeneration.UnapprovedDate = DateTime.Now;
                commissionAgentPaymentGeneration.IsApproved = false;
                var entry = _dbContext.ARCommissionAgentPaymentGeneration.Update(commissionAgentPaymentGeneration);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = string.Format("Transaction No. {0} has been Un-Approved successfully", commissionAgentPaymentGeneration.TransactionNo);
            }
            return RedirectToAction("Index", "CommissionAgentPaymentGeneration");
        }
        public IActionResult Delete(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var commissionAgentPaymentGeneration = _dbContext.ARCommissionAgentPaymentGeneration.Include(p => p.ARCommissionAgentPaymentGenerationDetails).ThenInclude(p => p.DeliveryChallanComAgentPayGenDetails).Where(p => p.Id == id).FirstOrDefault();
            commissionAgentPaymentGeneration.IsDeleted = true;
            commissionAgentPaymentGeneration.IsActive = false;
            commissionAgentPaymentGeneration.DeletedBy = userId;
            _dbContext.ARCommissionAgentPaymentGeneration.Update(commissionAgentPaymentGeneration);
            var result = _dbContext.SaveChanges();
            if (result > 0)
            {
                foreach (var item in commissionAgentPaymentGeneration.ARCommissionAgentPaymentGenerationDetails)
                {
                    item.IsDeleted = true;
                    item.DeletedBy = userId;
                    _dbContext.ARCommissionAgentPaymentGenerationDetails.Update(item);
                    _dbContext.ARDeliveryChallanComAgentPayGenDetails.RemoveRange(item.DeliveryChallanComAgentPayGenDetails);
                    _dbContext.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=PurchaseOrder&cId=", companyId, "&id={0}");
            var commissionAgentPaymentGeneration = _dbContext.ARCommissionAgentPaymentGeneration
                .Include(p => p.ARCommissionAgent)
                .Include(p => p.ARCommissionAgentPaymentGenerationDetails).ThenInclude(p => p.ARCustomers).ThenInclude(p => p.SalesPerson)
                .Include(p => p.ARCommissionAgentPaymentGenerationDetails).ThenInclude(p => p.InvItemCategories)
                .Where(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
            ViewBag.NavbarHeading = "Commission Agent Payment Generation";
            ViewBag.TitleStatus = "Approved";
            return View(commissionAgentPaymentGeneration);
        }
        public int Max()
        {
            int max = 0;
            var result = _aRCommissionAgentPaymentGenerationRepository.Get(x => x.IsActive == true && x.IsDeleted == false);
            if (result.Count() > 0)
            {
                max = _aRCommissionAgentPaymentGenerationRepository.Get().Select(x => x.TransactionNo).Max() + 1;
            }
            else
            {
                max = 1;
            }
            return max;
        }
    }
}