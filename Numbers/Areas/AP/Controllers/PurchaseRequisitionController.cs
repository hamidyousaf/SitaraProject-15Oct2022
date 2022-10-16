using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Numbers.Repository.AP;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AppModule;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class PurchaseRequisitionController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly APPurchaseRequisitionDetailsRepository _APPurchaseRequisitionDetailsRepository;
        private readonly APPurchaseRequisitionRepository _APPurchaseRequisitionRepository;
        public PurchaseRequisitionController(NumbersDbContext context, APPurchaseRequisitionRepository APPurchaseRequisitionRepository, APPurchaseRequisitionDetailsRepository APPurchaseRequisitionDetailsRepository)
        {
            _dbContext = context;
            _APPurchaseRequisitionRepository = APPurchaseRequisitionRepository;
            _APPurchaseRequisitionDetailsRepository = APPurchaseRequisitionDetailsRepository;
        }
        public IActionResult Index()
        {
            var apPurchaseRequistionList = new List<APPurchaseRequisitionViewModel>();
            //var model = unitofwork.SysApprovalGroupRepository.Get(x => x.IsActive).ToList(); 
            //var model = (from a in _dbContext.APPurchaseRequisition where a.IsActive select a).ToList();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=PRBasePrint&cId=", companyId, "&id=");
            var model = _dbContext.APPurchaseRequisition.Where(x => x.IsDeleted == false).ToList();
            foreach (var grp in model)
            {
                APPurchaseRequisitionViewModel aPPurchaseRequisition = new APPurchaseRequisitionViewModel();
                aPPurchaseRequisition.UserName = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).UserName;
                aPPurchaseRequisition.DepartmentName = _dbContext.GLDivision.FirstOrDefault(x => x.Id == grp.DepartmentId).Name;
                aPPurchaseRequisition.PrDate = grp.PrDate.ToShortDateString();
                aPPurchaseRequisition.APPurchaseRequisition = grp;
                apPurchaseRequistionList.Add(aPPurchaseRequisition);
            }
            ViewBag.NavbarHeading = "List of Purchase Requisitions";
            return View(apPurchaseRequistionList);
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string UserId = HttpContext.Session.GetString("UserId");
            
            var configValues = new ConfigValues(_dbContext);
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.Requisition = configValues.GetConfigValues("AP", "Purchase Requisition", companyId);
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted != true).ToList(), "Id", "Name");
            
            ViewBag.UserName = _dbContext.Users.FirstOrDefault(x => x.Id == UserId).UserName;
            var resposibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            ViewBag.Responsibility = resposibility;
            string CompanyName = HttpContext.Session.GetString("CompanyName");
            ViewBag.CompanyName = CompanyName;
            List<InvItemCategories> ItemCategory = _dbContext.Sys_ResponsibilityItemCategory.Include(x => x.ItemCategory).Where(x => x.ResponsibilityId == resp_Id).Select(x => x.ItemCategory).ToList();
            List<InvItem> ItemByResp = new List<InvItem>();
            foreach (InvItemCategories item in ItemCategory)
            {
                var itemList = _dbContext.InvItems.Include(x => x.Category).Where(x => x.IsDeleted == false && x.Category.Code.StartsWith(item.Code)).ToList();

                ItemByResp.AddRange(itemList.ToList());
            }
            ViewBag.Itemlist =
                ItemByResp.ToList().Select(a => new
                {
                    id = a.Id,
                    text = string.Concat(a.Code, " - ", a.Name)
                });


            APPurchaseRequisitionViewModel aPPurchaseItemViewModel = new APPurchaseRequisitionViewModel();
            if (id == 0)
            {
                if (resposibility == "Yarn Purchase")
                {
                    ViewBag.GreigeReqNo = new SelectList(_dbContext.GRGriegeRequisition.Where(x => !x.IsDeleted && x.IsApproved && !x.IsUsed && x.IsCreatePR).ToList().OrderByDescending(x => x.Id), "Id", "TransactionNo");
                }
                var result = _dbContext.APPurchaseRequisition.Where(x => x.IsActive).ToList();
                if (result.Count > 0)
                {
                    ViewBag.Id = _dbContext.APPurchaseRequisition.Select(x => x.PrNo).Max() + 1;
                }
                else
                {
                    ViewBag.Id = 1;
                }
            }
            else
            {
                aPPurchaseItemViewModel.APPurchaseRequisition = _APPurchaseRequisitionRepository.Get(x => x.Id == id).FirstOrDefault();
                TempData["SelectedPR"] = id;
                if (resposibility == "Yarn Purchase")
                {
                    ViewBag.GreigeReqNo = new SelectList(_dbContext.GRGriegeRequisition.Where(x => !x.IsDeleted && x.Id == aPPurchaseItemViewModel.APPurchaseRequisition.GreigeRequisitionId).ToList().OrderByDescending(x => x.Id), "Id", "TransactionNo");
                }
                var model = _APPurchaseRequisitionDetailsRepository.Get(x => x.APPurchaseRequisitionId == id).ToList();
                ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.DivisionId == aPPurchaseItemViewModel.APPurchaseRequisition.DepartmentId && x.IsDeleted != true && x.IsActive != false).ToList(), "Id", "Description");
                aPPurchaseItemViewModel.APPurchaseRequisitionDetails = new List<APPurchaseRequisitionDetails>();
                aPPurchaseItemViewModel.UOmName = new List<string>();
                aPPurchaseItemViewModel.Stock = new List<decimal>();
                ViewBag.CostCnter = new SelectList(_dbContext.CostCenter.Where(x => x.CompanyId == companyId && x.DivisionId == aPPurchaseItemViewModel.APPurchaseRequisition.DepartmentId && x.IsDeleted != true && x.IsActive != false).ToList(), "Id", "Description");
                ViewBag.SubDivision = new SelectList(_dbContext.GLSubDivision.Where(a => a.IsDeleted == false && a.Id== aPPurchaseItemViewModel.APPurchaseRequisition.SubDepartmentId).ToList(), "Id", "Name");
                foreach (var grp in model)
                {
                    string UOmName = null;
                    UOmName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == grp.UOM).ConfigValue;
                    decimal stock = _dbContext.InvItems.FirstOrDefault(x => x.Id == grp.ItemId).StockQty;
                    aPPurchaseItemViewModel.UOmName.Add(UOmName);
                    aPPurchaseItemViewModel.Stock.Add(stock);
                    aPPurchaseItemViewModel.APPurchaseRequisitionDetails.Add(grp);
                }

            }
            return View(aPPurchaseItemViewModel);
        }
        public IActionResult GetItemList(int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.UOMs = configValues.GetConfigValues("Inventory", "UOM", companyId);
            List<ItemListViewModel> itemListViewModel = new List<ItemListViewModel>();
            var ItemList = _dbContext.InvItems.Where(x => x.Id == itemId).ToList();
            var purchaseOrderItem = _dbContext.APPurchaseOrderItems.Include(p => p.PO).LastOrDefault(x => x.ItemId == itemId);
            DateTime OldDate = DateTime.Today.AddDays(-90);
            var invIssue = _dbContext.InvStoreIssueItems.Include(p => p.StoreIssue).Where(x => x.StoreIssue.IssueDate < DateTime.Now && x.StoreIssue.IssueDate >= OldDate && OldDate < DateTime.Now && x.ItemId == itemId && x.StoreIssue.TransactionType == "Issue").ToList();

            var invReturn = _dbContext.InvStoreIssueItems.Include(p => p.StoreIssue).Where(x => x.StoreIssue.IssueDate < DateTime.Now && x.StoreIssue.IssueDate >= OldDate && OldDate < DateTime.Now && x.ItemId == itemId && x.StoreIssue.TransactionType == "Issue Return").ToList();
            foreach (var frp in ItemList)
            {
                ItemListViewModel model = new ItemListViewModel();
                model.InvItems = frp;
                if (purchaseOrderItem != null)
                {
                    model.LastPODate = purchaseOrderItem.PO.PODate.ToString(Helpers.CommonHelper.DateFormat);
                    model.LastPOQty = purchaseOrderItem.Qty;
                }
                else
                {
                    model.LastPODate = "";
                    model.LastPOQty = Convert.ToDecimal(0.0000);
                }
                model.Comsumption = invIssue.Sum(x => x.Qty) - invReturn.Sum(x => x.ReturnQty) == 0 ? Convert.ToDecimal(0.0000) : Convert.ToDecimal(invIssue.Sum(x => x.Qty) - invReturn.Sum(x => x.ReturnQty));
                model.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == frp.Unit).ConfigValue;
                model.RequiredDate = DateTime.Now.AddDays(4).ToString(Helpers.CommonHelper.DateFormat);
                if (frp.PackUnit != 0)
                    model.PackUnit = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == frp.PackUnit).ConfigValue;
                itemListViewModel.Add(model);
                //  model.UOM=_dbContext.Ba
            }
            return Ok(itemListViewModel);
        }
        public IActionResult GetRequisition()
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var searchPrNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchPrDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchDepartment = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchApprovedBy = Request.Form["columns[4][search][value]"].FirstOrDefault();



                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //var customerData = (from pr in _dbContext.APPurchaseRequisition.Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id)
                //                                    join requisitionDep in _dbContext.SysUserDepartments.Where(x => x.IsDeleted == false) on pr.CreatedBy equals requisitionDep.UserId
                //                                    join currentUserDep in _dbContext.SysUserDepartments.Where(x => x.IsDeleted == false && x.UserId==userId) on requisitionDep.UserId equals currentUserDep.UserId
                //                                    select  pr).Distinct() ;
                var customerData = ((from pr in _dbContext.APPurchaseRequisition.Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id && x.CompanyId == companyId)
                                  //  join requisitionDep in _dbContext.SysUserDepartments.Where(x => x.IsDeleted == false) on pr.CreatedBy equals requisitionDep.UserId
                                    join currentUserDep in _dbContext.SysUserDepartments.Where(x => x.IsDeleted == false && x.UserId == userId) on pr.DepartmentId equals currentUserDep.DepartmentId 
                                    join currentUserSubDep in _dbContext.SysUserDepartments.Where(x => x.IsDeleted == false && x.UserId == userId) on pr.SubDepartmentId equals currentUserSubDep.SubDepartmentId
                                    select pr ).Distinct().Union(from pr in _dbContext.APPurchaseRequisition.Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id && x.CreatedBy==userId)
                                                          select pr)).Distinct();
                if (_dbContext.Users.Where(x => x.Id == userId).Select(x => x.AllDepartment).FirstOrDefault())
                {
                    customerData = (from tempcustomer in _dbContext.APPurchaseRequisition.Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id) select tempcustomer);
                }
                //var customerData = (from tempcustomer in _dbContext.APPurchaseRequisition.Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                customerData = !string.IsNullOrEmpty(searchPrNo) ? customerData.Where(m => m.PrNo.ToString().Contains(searchPrNo)) : customerData;
                customerData = !string.IsNullOrEmpty(searchPrDate) ? customerData.Where(m => m.PrDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchPrDate.ToUpper())) : customerData;
                customerData = !string.IsNullOrEmpty(searchDepartment) ? customerData.Where(m => (m.Department.ToString().ToUpper()).Contains(searchDepartment.ToUpper())) : customerData;            
                customerData = !string.IsNullOrEmpty(searchCreatedBy) ? customerData.Where(m => m.CreatedUser.UserName.ToString().ToUpper().Contains(searchCreatedBy.ToUpper())) : customerData;
                customerData = !string.IsNullOrEmpty(searchApprovedBy) ? customerData.Where(m => m.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == m.ApprovedBy).UserName.ToUpper().Contains(searchApprovedBy.ToUpper()) : false) : customerData;
                recordsTotal = customerData.Count();
                var data = customerData.ToList();
                recordsTotal = customerData.Count();
               
                if (pageSize == -1)
                {
                    data = customerData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = customerData.Skip(skip).Take(pageSize).ToList();
                }

                List<APPurchaseRequisitionViewModel> Details = new List<APPurchaseRequisitionViewModel>();
                foreach (var grp in data)
                {
                    APPurchaseRequisitionViewModel requistion = new APPurchaseRequisitionViewModel();
                    requistion.DepartmentName = _dbContext.GLDivision.FirstOrDefault(x => x.Id == grp.DepartmentId).Name;
                    requistion.UserName = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).UserName;
                    requistion.PrDate = grp.PrDate.ToString(Helpers.CommonHelper.DateFormat);
                    requistion.APPurchaseRequisition = grp;
                    requistion.APPurchaseRequisition.Auser = grp.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == grp.ApprovedBy).UserName : ""; 
                    requistion.APPurchaseRequisition.Approve = approve;
                    requistion.APPurchaseRequisition.Unapprove = unApprove;
                    Details.Add(requistion);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(APPurchaseRequisitionViewModel APPurchaseRequisitions, IFormFile File, IFormCollection collection)
        {
            int companyCode = HttpContext.Session.GetInt32("CompanyCode").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            if (APPurchaseRequisitions.APPurchaseRequisition.Id == 0)
            {
                try
                {
                    APPurchaseRequisitions.APPurchaseRequisition.CreatedBy = userId;
                    APPurchaseRequisitions.APPurchaseRequisition.CreatedDate = DateTime.UtcNow;
                    APPurchaseRequisitions.APPurchaseRequisition.CompanyId = companyId;
                    APPurchaseRequisitions.APPurchaseRequisition.Resp_ID = resp_Id;
                    APPurchaseRequisitions.APPurchaseRequisition.IsActive = true;
                    APPurchaseRequisitions.APPurchaseRequisition.Attachment = await UploadFile(File);
                    List<IFormFile> fileList = new List<IFormFile>();
                    fileList = collection.Files.Where(x => x.Name != File.Name).ToList();
                    GRGriegeRequisition requisition = _dbContext.GRGriegeRequisition.FirstOrDefault(x=>x.Id == APPurchaseRequisitions.APPurchaseRequisition.GreigeRequisitionId);
                    if (requisition != null)
                    {
                        requisition.IsUsed = true;
                        _dbContext.GRGriegeRequisition.Update(requisition);
                        _dbContext.SaveChanges();
                    }
                    var year = DateTime.Now.ToString("yy");
                    var month = DateTime.Now.ToString("MM");
                    var result = _dbContext.APPurchaseRequisition.Where(x => x.IsActive && x.CreatedDate.ToString("MM") == DateTime.Now.ToString("MM")).ToList();
                    if (result.Count > 0)
                    {
                        var trans = (Convert.ToInt32(result.Select(x => x.PrNo).LastOrDefault().Split("-")[2]) + 1).ToString("0000");
                        var transactionNo = $"{companyCode}-{year}{month}-{trans}";
                        APPurchaseRequisitions.APPurchaseRequisition.PrNo = transactionNo;
                    }
                    else
                    {
                        var transactionNo = $"{companyCode}-{year}{month}-0001";
                        APPurchaseRequisitions.APPurchaseRequisition.PrNo = transactionNo;
                    }
                    await _APPurchaseRequisitionRepository.CreateAsync(APPurchaseRequisitions.APPurchaseRequisition);
                    List<APPurchaseRequisitionDetails> Details = new List<APPurchaseRequisitionDetails>();
                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        APPurchaseRequisitionDetails requisitionDetails = new APPurchaseRequisitionDetails();
                        if (fileList.Count != 0)
                        {
                            IFormFile file = collection.Files[i];
                            requisitionDetails.Code = Convert.ToString(collection["code"][i]);
                            requisitionDetails.Description = Convert.ToString(collection["description"][i]);
                            requisitionDetails.Quantity = Convert.ToDecimal(collection["quantity"][i]);
                            requisitionDetails.Brand = Convert.ToString(collection["brand"][i]);
                            requisitionDetails.RequiredDate = Convert.ToDateTime(collection["requiredDate"][i]);
                            requisitionDetails.TechanicalInfo = Convert.ToString(collection["techanicalInfo"][i]);
                            requisitionDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id;
                            requisitionDetails.ItemSpecification = Convert.ToString(collection["itemSpecification"][i]);
                            requisitionDetails.ItemId = Convert.ToInt32(collection["itemId"][i]);
                            //requisitionDetails.CostCenterId = Convert.ToInt32(collection["cosId"][i]);
                            requisitionDetails.APPurchaseRequisitionId = APPurchaseRequisitions.APPurchaseRequisition.Id;
                            requisitionDetails.Attachment = await UploadFile(file);
                            if (collection["poDate"][i] != "null")
                                requisitionDetails.LastPODate = Convert.ToDateTime(collection["poDate"][i]);
                            requisitionDetails.LastPOQty = Convert.ToDecimal(collection["poQty"][i]);
                            requisitionDetails.Consumption = Convert.ToDecimal(collection["cons"][i]);
                            Details.Add(requisitionDetails);
                        }
                        else
                        {
                            requisitionDetails.Code = Convert.ToString(collection["code"][i]);
                            requisitionDetails.Description = Convert.ToString(collection["description"][i]);
                            requisitionDetails.Quantity = Convert.ToDecimal(collection["quantity"][i]);
                            requisitionDetails.Brand = Convert.ToString(collection["brand"][i]);
                            requisitionDetails.RequiredDate = Convert.ToDateTime(collection["requiredDate"][i]);
                            requisitionDetails.TechanicalInfo = Convert.ToString(collection["techanicalInfo"][i]);
                            requisitionDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id;
                            requisitionDetails.ItemSpecification = Convert.ToString(collection["itemSpecification"][i]);
                            requisitionDetails.ItemId = Convert.ToInt32(collection["itemId"][i]);
                            //requisitionDetails.CostCenterId = Convert.ToInt32(collection["cosId"][i]);
                            requisitionDetails.APPurchaseRequisitionId = APPurchaseRequisitions.APPurchaseRequisition.Id;
                            if (collection["poDate"][i] != "null" && collection["poDate"][i] != "")
                            {
                                requisitionDetails.LastPODate = Convert.ToDateTime(collection["poDate"][i]);
                                requisitionDetails.LastPOQty = Convert.ToDecimal(collection["poQty"][i]);
                                requisitionDetails.Consumption = Convert.ToDecimal(collection["cons"][i]);
                            }

                            Details.Add(requisitionDetails);
                        }
                    };
                    await _APPurchaseRequisitionDetailsRepository.CreateRangeAsync(Details);
                    TempData["error"] = "false";
                    int MaxPrNo = Convert.ToInt32(_dbContext.APPurchaseRequisition.Select(x => x.PrNo).Max());
                    TempData["message"] = "Purchase Requisition " + Convert.ToString(MaxPrNo) + " has been created successfully.";
                }
                catch (Exception ex)
                {
                    await _APPurchaseRequisitionRepository.DeleteAsync(APPurchaseRequisitions.APPurchaseRequisition);
                    var DeleteList = _APPurchaseRequisitionDetailsRepository.Get(x => x.APPurchaseRequisitionId == APPurchaseRequisitions.APPurchaseRequisition.Id).ToList();
                    await _APPurchaseRequisitionDetailsRepository.DeleteRangeAsync(DeleteList);

                }
            }
            else
            {
                APPurchaseRequisition aPPurchaseRequisition = _APPurchaseRequisitionRepository.Get(x => x.Id == APPurchaseRequisitions.APPurchaseRequisition.Id).FirstOrDefault();
                aPPurchaseRequisition.UpdatedBy = userId;
                aPPurchaseRequisition.UpdatedDate = DateTime.UtcNow;
                aPPurchaseRequisition.CompanyId = companyId;
                aPPurchaseRequisition.Resp_ID = resp_Id;
                APPurchaseRequisitions.APPurchaseRequisition.IsActive = true;
                aPPurchaseRequisition.OperationId = APPurchaseRequisitions.APPurchaseRequisition.OperationId;
                aPPurchaseRequisition.PrDate = APPurchaseRequisitions.APPurchaseRequisition.PrDate;
                //aPPurchaseRequisition.PrNo = APPurchaseRequisitions.APPurchaseRequisition.PrNo;
                aPPurchaseRequisition.RefrenceNo = APPurchaseRequisitions.APPurchaseRequisition.RefrenceNo;
                aPPurchaseRequisition.Remarks = APPurchaseRequisitions.APPurchaseRequisition.Remarks;
                aPPurchaseRequisition.DepartmentId = APPurchaseRequisitions.APPurchaseRequisition.DepartmentId;
                aPPurchaseRequisition.SubDepartmentId = APPurchaseRequisitions.APPurchaseRequisition.SubDepartmentId;
                aPPurchaseRequisition.RequisitionTypeId = APPurchaseRequisitions.APPurchaseRequisition.RequisitionTypeId;
                if (!ReferenceEquals(File, null))
                    APPurchaseRequisitions.APPurchaseRequisition.Attachment = await UploadFile(File);
                List<IFormFile> fileList = new List<IFormFile>();
                fileList = collection.Files.Where(x => x.Name != File.Name).ToList();
                await _APPurchaseRequisitionRepository.UpdateAsync(aPPurchaseRequisition);
                var AddList = new List<APPurchaseRequisitionDetails>();
                var UpdateList = new List<APPurchaseRequisitionDetails>();
                var foundDetail = _dbContext.APPurchaseRequisitionDetails.Where(a => a.APPurchaseRequisitionId == APPurchaseRequisitions.APPurchaseRequisition.Id).ToList();
                if (!ReferenceEquals(APPurchaseRequisitions.APPurchaseRequisitionDetails, null))
                {
                    int z = 0;
                    foreach (var det in foundDetail)
                    {
                        //bool result = APPurchaseRequisitions.APPurchaseRequisitionDetails.Exists(s => s.Id == det.Id);
                        bool result = Convert.ToInt32(collection["APPurchaseRequisitionDetails[" + z + "].Id"]) == det.Id;
                        if (!result)
                        {
                            await _APPurchaseRequisitionDetailsRepository.DeleteAsync(det);
                        }
                        z++;
                    }

                    for (int i = 0; i < APPurchaseRequisitions.APPurchaseRequisitionDetails.Count; i++)
                    {
                        APPurchaseRequisitionDetails detail = foundDetail.FirstOrDefault(x => x.Id == APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Id);
                        if (!ReferenceEquals(detail, null))
                        {
                            if (fileList.Count != 0)
                            {
                                IFormFile file = collection.Files[i];
                                detail.APPurchaseRequisitionId = APPurchaseRequisitions.APPurchaseRequisition.Id;
                                detail.Brand = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Brand;
                                detail.Code = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Code;
                                detail.Description = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Description;
                                detail.ItemSpecification = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].ItemSpecification;
                                detail.Quantity = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Quantity;
                                detail.TechanicalInfo = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].TechanicalInfo;
                                detail.ItemId = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].ItemId;
                                detail.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == APPurchaseRequisitions.UOmName[i]).Id;
                                detail.Attachment = await UploadFile(file);
                                detail.LastPOQty = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].LastPOQty;
                                detail.LastPODate = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].LastPODate;
                                detail.Consumption = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Consumption;
                                //detail.CostCenterId = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].CostCenterId;
                                UpdateList.Add(detail);
                            }
                            else
                            {
                                detail.APPurchaseRequisitionId = APPurchaseRequisitions.APPurchaseRequisition.Id;
                                detail.Brand = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Brand;
                                detail.Code = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Code;
                                detail.Description = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Description;
                                detail.ItemSpecification = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].ItemSpecification;
                                detail.Quantity = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Quantity;
                                detail.TechanicalInfo = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].TechanicalInfo;
                                detail.ItemId = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].ItemId;
                                detail.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == APPurchaseRequisitions.UOmName[i]).Id;
                                detail.LastPOQty = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].LastPOQty;
                                detail.LastPODate = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].LastPODate;
                                detail.Consumption = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].Consumption;
                                //detail.CostCenterId = APPurchaseRequisitions.APPurchaseRequisitionDetails[i].CostCenterId;
                                UpdateList.Add(detail);
                            }
                        }
                    }
                }
                else
                {
                    if (foundDetail.Count > 0)
                    {
                        await _APPurchaseRequisitionDetailsRepository.DeleteRangeAsync(foundDetail);
                    }
                }
                List<APPurchaseRequisitionDetails> Details = new List<APPurchaseRequisitionDetails>();
                for (int i = 0; i < collection["id"].Count; i++)
                {
                    APPurchaseRequisitionDetails requisitionDetails = new APPurchaseRequisitionDetails();
                    if (fileList.Count != 0)
                    {
                        IFormFile file = collection.Files[i];
                        requisitionDetails.Code = Convert.ToString(collection["code"][i]);
                        requisitionDetails.Description = Convert.ToString(collection["description"][i]);
                        requisitionDetails.Quantity = Convert.ToDecimal(collection["quantity"][i]);
                        requisitionDetails.Brand = Convert.ToString(collection["brand"][i]);
                        requisitionDetails.RequiredDate = Convert.ToDateTime(collection["requiredDate"][i]);
                        requisitionDetails.TechanicalInfo = Convert.ToString(collection["techanicalInfo"][i]);
                        requisitionDetails.ItemSpecification = Convert.ToString(collection["itemSpecification"][i]);
                        requisitionDetails.ItemId = Convert.ToInt32(collection["itemId"][i]);
                        //requisitionDetails.CostCenterId = Convert.ToInt32(collection["cosId"][i]);
                        requisitionDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id;
                        requisitionDetails.APPurchaseRequisitionId = APPurchaseRequisitions.APPurchaseRequisition.Id;
                        requisitionDetails.Attachment = await UploadFile(file);
                        if (collection["poDate"][i] != "null")
                            requisitionDetails.LastPODate = Convert.ToDateTime(collection["poDate"][i]);
                        requisitionDetails.LastPOQty = Convert.ToDecimal(collection["poQty"][i]);
                        requisitionDetails.Consumption = Convert.ToDecimal(collection["cons"][i]);
                        Details.Add(requisitionDetails);
                    }
                    else
                    {
                        requisitionDetails.Code = Convert.ToString(collection["code"][i]);
                        requisitionDetails.Description = Convert.ToString(collection["description"][i]);
                        requisitionDetails.Quantity = Convert.ToDecimal(collection["quantity"][i]);
                        requisitionDetails.Brand = Convert.ToString(collection["brand"][i]);
                        requisitionDetails.RequiredDate = Convert.ToDateTime(collection["requiredDate"][i]);
                        requisitionDetails.TechanicalInfo = Convert.ToString(collection["techanicalInfo"][i]);
                        requisitionDetails.ItemId = Convert.ToInt32(collection["itemId"][i]);
                        requisitionDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id;
                        requisitionDetails.ItemSpecification = Convert.ToString(collection["itemSpecification"][i]);
                        //requisitionDetails.CostCenterId = Convert.ToInt32(collection["cosId"][i]);
                        requisitionDetails.APPurchaseRequisitionId = APPurchaseRequisitions.APPurchaseRequisition.Id;
                        if (collection["poDate"][i] != "null" && collection["poDate"][i] != "")
                            requisitionDetails.LastPODate = Convert.ToDateTime(collection["poDate"][i]);
                        requisitionDetails.LastPOQty = Convert.ToDecimal(collection["poQty"][i]);
                        requisitionDetails.Consumption = Convert.ToDecimal(collection["cons"][i]);
                        Details.Add(requisitionDetails);
                    }
                };
                await _APPurchaseRequisitionDetailsRepository.UpdateRangeAsync(UpdateList);
                await _APPurchaseRequisitionDetailsRepository.CreateRangeAsync(Details);
                TempData["error"] = "false";
                TempData["message"] = "Purchase Requisition has been updated successfully.";
            }
            return RedirectToAction("Index", "PurchaseRequisition");
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {
                APPurchaseRequisition aPPurchaseRequisition = _APPurchaseRequisitionRepository.Get(x => x.Id == id).FirstOrDefault();
                await _APPurchaseRequisitionRepository.DeleteAsync(aPPurchaseRequisition);
                var Detail = _APPurchaseRequisitionDetailsRepository.Get(x => x.APPurchaseRequisitionId == id).ToList();
                if (!ReferenceEquals(Detail, null))
                    await _APPurchaseRequisitionDetailsRepository.DeleteRangeAsync(Detail);
                TempData["error"] = "false";
                TempData["message"] = "Purchase Requisition has been deleted successfully.";
            }
            return RedirectToAction("Index", "PurchaseRequisition");
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APPurchaseRequisition aPPurchaseRequisition = _APPurchaseRequisitionRepository.Get(x => x.Id == id).FirstOrDefault();
            aPPurchaseRequisition.ApprovedBy = _userId;
            aPPurchaseRequisition.ApprovedDate = DateTime.UtcNow;
            aPPurchaseRequisition.IsApproved = true;
            await _APPurchaseRequisitionRepository.UpdateAsync(aPPurchaseRequisition);
            TempData["error"] = "false";
            TempData["message"] = "Purchase Requisition has been approved successfully.";
            return RedirectToAction("Index", "PurchaseRequisition");
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            bool check = _dbContext.APPurchaseRequisitionDetails.Any(x => x.APPurchaseRequisitionId == id && x.IsCSCreated == false && x.IsPOCreated == false);
            if (check)
            {
                APPurchaseRequisition aPPurchaseRequisition = _APPurchaseRequisitionRepository.Get(x => x.Id == id).FirstOrDefault();
                aPPurchaseRequisition.UnApprovedBy = _userId;
                aPPurchaseRequisition.UnApprovedDate = DateTime.UtcNow;
                aPPurchaseRequisition.IsApproved = false;
                aPPurchaseRequisition.ApprovedBy = null;
                await _APPurchaseRequisitionRepository.UpdateAsync(aPPurchaseRequisition);
                TempData["error"] = "false";
                TempData["message"] = "Purchase Requisition has been UnApproved sucessfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Please UnApproved Po and Cs";
            }
            return RedirectToAction("Index", "PurchaseRequisition");
        }
        public async Task<string> UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\item-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(Fstream);
                        var fullPath = "/uploads/item-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }
        public IActionResult GetCostCeter(int id)
        {
            var subDeparments = new SelectList(_dbContext.CostCenter.Where(x => x.DivisionId == id && x.IsDeleted == false).ToList(), "Id", "Description");
            return Ok(subDeparments);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            var purchaseRequisition = _dbContext.APPurchaseRequisition
                .Include(a => a.Department)
                .Include(a => a.RequisitionType)
                .Include(a => a.Operation)
                .Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();
            var PurchaseRequisitionDetails = _dbContext.APPurchaseRequisitionDetails
                //.Include(i => i.CostCenter)
                .Where(i => i.APPurchaseRequisitionId == id).ToList();
            ViewBag.RequestBy = _dbContext.Users.FirstOrDefault(x => x.Id == purchaseRequisition.CreatedBy).FullName;
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=PurchaseOrder&cId=", companyId, "&id={0}");
            ViewBag.NavbarHeading = "Purchase Requisition";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = PurchaseRequisitionDetails;
            return View(purchaseRequisition);
        }
        public IActionResult GetGreigeRequisitions(int GreigeRequisitionId)
        {
            if (GreigeRequisitionId != 0)
            {
                List<GRGriegeRequisitionDetails> requisitionDetails = _dbContext.GRGriegeRequisitionDetails
                    .Include(x=>x.GriegeQuality)
                        .ThenInclude(x=>x.GRConstruction)
                    .Where(x => x.GRRequisitionId == GreigeRequisitionId).ToList();
                var weftData = requisitionDetails.GroupBy(x => x.GriegeQuality.GRConstruction.WeftId).Select(x => new ListOfValue
                {
                    Id = x.Select(a => a.GriegeQuality.GRConstruction.WeftId).FirstOrDefault(),
                    Quantity = x.Select(a => a.WeftBag).Sum()
                });
                var warpData = requisitionDetails.GroupBy(x => x.GriegeQuality.GRConstruction.WarpId).Select(x => new ListOfValue
                {
                    Id = x.Select(a => a.GriegeQuality.GRConstruction.WarpId).FirstOrDefault(),
                    Quantity = x.Select(a => a.WarpBag).Sum()
                });
                var mergedList = warpData.Union(weftData).ToList();
                var mergedData = mergedList.GroupBy(x => x.Id).Select(a => new ListOfValue { 
                    Id = a.Select(x=>x.Id).FirstOrDefault(),
                    Quantity = a.Select(x=>x.Quantity).Sum()
                });
                DateTime OldDate = DateTime.Today.AddDays(-90);
                List<InventoryViewModel> item = new List<InventoryViewModel>();
                foreach (var det in mergedData)
                {
                    InventoryViewModel model = new InventoryViewModel();
                    var invIssue = _dbContext.InvStoreIssueItems.Include(p => p.StoreIssue).Where(x => x.StoreIssue.IssueDate < DateTime.Now && x.StoreIssue.IssueDate >= OldDate && OldDate < DateTime.Now && x.ItemId == det.Id && x.StoreIssue.TransactionType == "Issue").ToList();
                    var invReturn = _dbContext.InvStoreIssueItems.Include(p => p.StoreIssue).Where(x => x.StoreIssue.IssueDate < DateTime.Now && x.StoreIssue.IssueDate >= OldDate && OldDate < DateTime.Now && x.ItemId == det.Id && x.StoreIssue.TransactionType == "Issue Return").ToList();
                    model.InvItem = _dbContext.InvItems.Include(x=>x.UOM).FirstOrDefault(x => x.Id == det.Id);
                    model.Quantity = det.Quantity;
                    var purchaseOrderItem = _dbContext.APPurchaseOrderItems.Include(p => p.PO).LastOrDefault(x => x.ItemId == model.InvItem.Id);

                    if (purchaseOrderItem != null)
                    {
                        model.LastPODate = purchaseOrderItem.PO.PODate.ToString(Helpers.CommonHelper.DateFormat);
                        model.LastPOQty = purchaseOrderItem.Qty;
                    }
                    else
                    {
                        model.LastPODate = "";
                        model.LastPOQty = Convert.ToDecimal(0.0000);
                    }
                    model.Comsumption = invIssue.Sum(x => x.Qty) - invReturn.Sum(x => x.ReturnQty) == 0 ? Convert.ToDecimal(0.0000) : Convert.ToDecimal(invIssue.Sum(x => x.Qty) - invReturn.Sum(x => x.ReturnQty));
                    model.RequiredDate = DateTime.Now.AddDays(4).ToString(Helpers.CommonHelper.DateFormat);

                    item.Add(model);
                }
                return Ok(item);
            }
            return Ok();
        }
    }
}
