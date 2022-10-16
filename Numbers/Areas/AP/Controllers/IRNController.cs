using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AP;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class IRNController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly APIRNRepository _APIRNRepository;
        private readonly APIRNDetailsRepository _APIRNDetailsRepository;
        public IRNController(NumbersDbContext context, APIRNRepository APIRNRepository, APIRNDetailsRepository APIRNDetailsRepository)
        {
            _dbContext = context;
            _APIRNRepository = APIRNRepository;
            _APIRNDetailsRepository = APIRNDetailsRepository;
        }
        public IActionResult Index(int id)
        {
            APIRNVM IRNVM = new APIRNVM();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var responcibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x=>x.Resp_Id == resp_Id).Resp_Name;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted != true && x.IsActive != false).ToList(), "Id", "Name");
            //ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => x.CompanyId == companyId && x.IsActive != false).ToList(), "Id", "Name");
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            if (id == 0)
            {
                var result = _dbContext.APIRN.Where(x => x.CompanyId == companyId).ToList();
                if (result.Count > 0)
                {
                    ViewBag.Id = _dbContext.APIRN.Select(x => x.IRNNo).Max() + 1;
                    TempData["MaxIRNNo"] = ViewBag.Id;
                }
                else
                {
                    ViewBag.Id = 1;
                }
                //ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => x.CompanyId == companyId && x.IsActive != false).ToList(), "Id", "Name");
                ViewBag.Vendor = new SelectList(responcibility == "Yarn Purchase" ?
                        (from Suppliers in _dbContext.APSuppliers.Where(x => x.IsActive == true && x.Account.Code == "2.02.04.0003" /*&& x.CompanyId == companyId*/).Include(a => a.Account) select Suppliers) :
                        (from Suppliers in _dbContext.APSuppliers.Where(x => x.IsActive == true && x.Account.Code != "2.02.04.0003" /*&& x.CompanyId == companyId*/).Include(a => a.Account) select Suppliers), "Id", "Name");
            }
            else
            {
                ViewBag.IRNNo = id;
                IRNVM.APIRN = _APIRNRepository.Get(x => x.Id == id).FirstOrDefault();
                var model = _APIRNDetailsRepository.Get(x => x.IRNID == id).ToList();
                ViewBag.IGP = new SelectList(_dbContext.APIGP.Where(x => x.VendorId == IRNVM.APIRN.VendorID).ToList(), "Id", "IGP");
                var typeId = (from x in _dbContext.APIGP where x.IGP == IRNVM.APIRN.IGPNo select x.POTypeId).FirstOrDefault();
                ViewBag.Type = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == typeId).ConfigValue;
                IRNVM.APIRNDetails = new List<APIRNDetails>();
                IRNVM.UOMName = new List<string>();
                IRNVM.Balc = new List<decimal>();
                IRNVM.Rcd = new List<decimal>();
                //ViewBag.IGPList =  new SelectList(_dbContext.APIGP.Where(x => x.VendorId == IRNVM.APIRN.VendorID && x.CompanyId == companyId && x.IsDeleted !=true && x.IsActive !=false && x.IsApproved && (x.IsIRN == false || x.IGP == IRNVM.APIRN.IGPNo)).ToList(), "IGP", "IGP");
                ViewBag.IGPList = new SelectList(_dbContext.APIGP.Where(x => x.IGP == IRNVM.APIRN.IGPNo && x.CompanyId == companyId).ToList(), "IGP", "IGP");
                ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => x.Id == IRNVM.APIRN.VendorID && x.IsActive != false).ToList(), "Id", "Name");
                foreach (var grp in model)
                {
                    string UOmName = null;
                    string brand = null;
                    UOmName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.UOM)).ConfigValue;
                    if (grp.BrandId != 0)
                    {
                        brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.BrandId)).ConfigValue;
                    }
                    decimal balc = _dbContext.APIGPDetails.FirstOrDefault(x => x.Id == grp.IGPDetailId).BalQty;
                    decimal rcd = _dbContext.APIGPDetails.FirstOrDefault(x => x.Id == grp.IGPDetailId).RCDQty;
                    grp.CategoryName = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.CategoryId).Name;
                    IRNVM.Balc.Add(balc);
                    IRNVM.Rcd.Add(rcd);
                    IRNVM.UOMName.Add(UOmName);
                    if (brand != null)
                    {
                        IRNVM.Brand.Add(brand);
                    }
                    IRNVM.APIRNDetails.Add(grp);
                }
            }
            return View(IRNVM);
        }
        public IActionResult GetIGPLis(int id)
        {
            if (id != 0)
            {
                List<ItemListViewModel> itemListViewModel = new List<ItemListViewModel>();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

                var igpId = (from m in _dbContext.APIGP where m.IGP == id && m.CompanyId == companyId && m.IsApproved == true && m.IsDeleted == false select m).FirstOrDefault();
                var IGPList = (from a in _dbContext.APIGP.Where(x => x.Id == igpId.Id && x.IsApproved == true) join b in _dbContext.APIGPDetails on a.Id equals b.IGPId where b.IGPId == igpId.Id && b.IsIRNCreated == false select b).ToList();
                foreach (var frp in IGPList)
                {
                    ItemListViewModel model = new ItemListViewModel();
                    model.APIGPDetails = frp;
                    model.type = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(igpId.POTypeId)).ConfigValue;
                    model.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(frp.UOM)).ConfigValue;
                    model.Category = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == frp.CategoryId).Name;
                    if (frp.BrandId != 0) { 
                        model.Brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(frp.BrandId)).ConfigValue;
                    }
                    itemListViewModel.Add(model);
                }
                return Ok(itemListViewModel);
            }
            else
            {
                return Ok();
            }
        }
        public IActionResult GetIRN()
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

                var searchIRNNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchIGPNo = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchIRNDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchVendor = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchApprovedBy = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var IRNData = (from tempcustomer in _dbContext.APIRN.Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    IRNData = IRNData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    IRNData = IRNData.Where(m => m.IRNDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.IGPNo.ToString().Contains(searchValue)
                //                                    || m.IRNNo.ToString().Contains(searchValue)
                //                                    || _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.VendorID)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                    || _dbContext.Users.FirstOrDefault(x => x.Id == m.CreatedBy).UserName.ToUpper().Contains(searchValue.ToUpper())
                //                                  );

                //}
                IRNData = !string.IsNullOrEmpty(searchIRNNo) ? IRNData.Where(m => m.IRNNo.ToString().ToUpper().Contains(searchIRNNo.ToUpper())) : IRNData;
                IRNData = !string.IsNullOrEmpty(searchIGPNo) ? IRNData.Where(m => m.IGPNo.ToString().ToUpper().Contains(searchIGPNo.ToUpper())) : IRNData;
                IRNData = !string.IsNullOrEmpty(searchIRNDate) ? IRNData.Where(m => m.IRNDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchIRNDate.ToUpper())) : IRNData;
                IRNData = !string.IsNullOrEmpty(searchVendor) ? IRNData.Where(m => _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.VendorID)).Name.ToUpper().Contains(searchVendor.ToUpper())) : IRNData;
                IRNData = !string.IsNullOrEmpty(searchCreatedBy) ? IRNData.Where(m => _dbContext.Users.FirstOrDefault(x => x.Id == m.CreatedBy).UserName.ToUpper().Contains(searchCreatedBy.ToUpper())) : IRNData;
                IRNData = !string.IsNullOrEmpty(searchApprovedBy) ? IRNData.Where(m => m.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == m.ApprovedBy).UserName.ToUpper().Contains(searchApprovedBy.ToUpper()):false) : IRNData;

                recordsTotal = IRNData.Count();
                var data = IRNData.ToList();
                if (pageSize == -1)
                {
                    data = IRNData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = IRNData.Skip(skip).Take(pageSize).ToList();
                }
                List<APIRNVM> Details = new List<APIRNVM>();
                foreach (var grp in data)
                {
                    APIRNVM aPIRNVM = new APIRNVM();
                    aPIRNVM.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.VendorID).Name;
                    aPIRNVM.IRNDate = grp.IRNDate.ToString(Helpers.CommonHelper.DateFormat);
                    aPIRNVM.CreatedBy = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).UserName;
                    aPIRNVM.ApprovedBy = grp.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == grp.ApprovedBy).UserName : "";
                    aPIRNVM.APIRN = grp;
                    aPIRNVM.APIRN.Approve = approve;
                    aPIRNVM.APIRN.Unapprove = unApprove;
                    Details.Add(aPIRNVM);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult List()
        {
            var iRNList = new List<APIRNVM>();
            var model = _dbContext.APIRN.Where(x => x.IsDeleted == false).ToList();
            foreach (var grp in model)
            {
                APIRNVM aPIRNVM = new APIRNVM();
                aPIRNVM.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.VendorID).Name;
                //    aPIRNVM.DepartmentName = _dbContext.GLDivision.FirstOrDefault(x => x.Id == grp.DepartmentId).Name;
                aPIRNVM.APIRN = grp;
                iRNList.Add(aPIRNVM);
            }

            ViewBag.NavbarHeading = "List of Inspection and Receipt Note(IRN)";
            return View(iRNList);
        }
        public int GetMaxIRN(int companyId)
        {
            int IRN = 1;
            var IRNno = _dbContext.APIRN.Where(c => c.CompanyId == companyId).ToList();
            if (IRNno.Count > 0)
            {
                IRN = IRNno.Max(r => r.IRNNo);
                return IRN + 1;
            }
            else
            {
                return IRN;
            }
        }
        public async Task<IActionResult> Submit(APIRNVM APIRNVM, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string module = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == resp_Id select c.Resp_Name).FirstOrDefault();

            List<APIGPDetails> Apdetails = new List<APIGPDetails>();
            if (APIRNVM.APIRN.Id == 0)
            {
                try
                {
                    APIRNVM.APIRN.CreatedBy = userId;
                    APIRNVM.APIRN.CreatedDate = DateTime.UtcNow;
                    APIRNVM.APIRN.CompanyId = companyId;
                    APIRNVM.APIRN.Resp_ID = resp_Id;
                    APIRNVM.APIRN.IsActive = true;
                    APIRNVM.APIRN.Status = "Created";
                    APIRNVM.APIRN.OperatingId = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APIRNVM.APIRN.IGPNo).OperatingId;
                    APIRNVM.APIRN.IRNNo = GetMaxIRN(companyId);
                    await _APIRNRepository.CreateAsync(APIRNVM.APIRN);
                    List<APIRNDetails> Details = new List<APIRNDetails>();
                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        APIRNDetails irnDetails = new APIRNDetails();
                        irnDetails.ItemCode = Convert.ToString(collection["code"][i]);
                        irnDetails.ItemDiscription = Convert.ToString(collection["description"][i]);
                        irnDetails.ItemID = Convert.ToInt32(collection["itemId"][i]);
                        irnDetails.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;
                        irnDetails.Received_Qty = Convert.ToDecimal(collection["received"][i]);
                        irnDetails.PrID = Convert.ToInt32(collection["poNo"][i]);
                        irnDetails.PrDetailId = Convert.ToInt32(collection["prDetailId"][i]);
                        irnDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id.ToString();
                        irnDetails.Rejected_Qty = Convert.ToDecimal(collection["rQty"][i]);
                        irnDetails.Accepted_Qty = Convert.ToDecimal(collection["AQty"][i]);
                        irnDetails.IGP_Qty = Convert.ToDecimal(collection["igpQty"][i]);
                        irnDetails.Reason_OF_Rejection = Convert.ToString(collection["rof"][i]);
                        irnDetails.IGPDetailId = Convert.ToInt32(collection["igpDetailId"][i]);
                        irnDetails.OGPBalQty = Convert.ToInt32(collection["rQty"][i]);
                        irnDetails.OGPQty = Convert.ToInt32(collection["rQty"][i]);
                        var categoryId = _dbContext.InvItems.Where(x => x.Id == irnDetails.ItemID).Select(x => x.CategoryId).FirstOrDefault();
                        irnDetails.CategoryId = categoryId;
                        //irnDetails.CategoryId = _dbContext.InvItemCategories.FirstOrDefault(x => x.Name == Convert.ToString(collection["category"][i])).Id;
                        irnDetails.IRNID = APIRNVM.APIRN.Id;
                        Details.Add(irnDetails);
                        APIGPDetails aPIGPDetails = _dbContext.APIGPDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["igpDetailId"][i]));
                        aPIGPDetails.BalQty = irnDetails.Rejected_Qty;
                        aPIGPDetails.RCDQty = irnDetails.Received_Qty;
                        if (aPIGPDetails.BalQty == 0)
                        {
                            APIGP aPIGP = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APIRNVM.APIRN.IGPNo);
                            aPIGP.IsIRN = true;
                            _dbContext.Update(aPIGP);
                            aPIGPDetails.IsIRNCreated = true;
                        }
                        else
                        {
                            APIGP aPIGP = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APIRNVM.APIRN.IGPNo);
                            aPIGP.IsIRN = false;
                            _dbContext.Update(aPIGP);
                            aPIGPDetails.IsIRNCreated = false;
                        }
                        Apdetails.Add(aPIGPDetails);

                    };
                    _dbContext.SaveChanges();
                    await _APIRNDetailsRepository.CreateRangeAsync(Details);
                    TempData["error"] = "false";
                    TempData["message"] = "IRN " + APIRNVM.APIRN.IRNNo + " has been created successfully.";
                }
                catch (Exception ex)
                {
                    await _APIRNRepository.DeleteAsync(APIRNVM.APIRN);
                    var DeleteList = _APIRNDetailsRepository.Get(x => x.IRNID == APIRNVM.APIRN.Id).ToList();
                    await _APIRNDetailsRepository.DeleteRangeAsync(DeleteList);
                    TempData["error"] = "true";
                    TempData["message"] = "Went something wrong.";
                }
            }
            else
            {
                APIRN aPIRN = _APIRNRepository.Get(x => x.Id == APIRNVM.APIRN.Id).FirstOrDefault();
                aPIRN.UpdatedBy = userId;
                aPIRN.UpdatedDate = DateTime.UtcNow;
                aPIRN.CompanyId = companyId;
                aPIRN.Resp_ID = resp_Id;
                aPIRN.IsActive = true;
                aPIRN.OperatingId = APIRNVM.APIRN.OperatingId;
                aPIRN.VendorID = APIRNVM.APIRN.VendorID;
                aPIRN.IGPNo = APIRNVM.APIRN.IGPNo;
                aPIRN.Remarks = APIRNVM.APIRN.Remarks;
                aPIRN.IRNDate = APIRNVM.APIRN.IRNDate;
                aPIRN.IRNNo = APIRNVM.APIRN.IRNNo;
                await _APIRNRepository.UpdateAsync(aPIRN);
                var UpdateList = new List<APIRNDetails>();
                var foundDetail = _APIRNDetailsRepository.Get(a => a.IRNID == APIRNVM.APIRN.Id).ToList();
                if (!ReferenceEquals(APIRNVM.APIRNDetails, null))
                {
                    foreach (var det in foundDetail)
                    {
                        bool result = APIRNVM.APIRNDetails.Exists(s => s.Id == det.Id);
                        if (!result)
                        {
                            await _APIRNDetailsRepository.DeleteAsync(det);
                        }
                    }

                    for (int i = 0; i < APIRNVM.APIRNDetails.Count; i++)
                    {
                        APIRNDetails detail = foundDetail.FirstOrDefault(x => x.Id == APIRNVM.APIRNDetails[i].Id);
                        if (!ReferenceEquals(detail, null))
                        {
                            detail.ItemCode = APIRNVM.APIRNDetails[i].ItemCode;
                            detail.ItemDiscription = APIRNVM.APIRNDetails[i].ItemDiscription;
                            detail.ItemID = APIRNVM.APIRNDetails[i].ItemID;
                            detail.Accepted_Qty = APIRNVM.APIRNDetails[i].Accepted_Qty;
                            detail.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == APIRNVM.UOMName[i]).Id.ToString();
                            detail.IGP_Qty = APIRNVM.APIRNDetails[i].IGP_Qty;
                            detail.Reason_OF_Rejection = APIRNVM.APIRNDetails[i].Reason_OF_Rejection;
                            detail.PrID = APIRNVM.APIRNDetails[i].PrID;
                            detail.PrDetailId = APIRNVM.APIRNDetails[i].PrDetailId;
                            detail.CategoryId = APIRNVM.APIRNDetails[i].CategoryId;
                            detail.IRNID = APIRNVM.APIRN.Id;
                            UpdateList.Add(detail);
                            APIGPDetails aPIGPDetails = _dbContext.APIGPDetails.FirstOrDefault(x => x.Id == detail.IGPDetailId);
                            aPIGPDetails.BalQty = APIRNVM.Balc[i];
                            aPIGPDetails.RCDQty = APIRNVM.Rcd[i];
                            if (aPIGPDetails.BalQty == 0)
                            {
                                APIGP aPIGP = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APIRNVM.APIRN.IGPNo);
                                aPIGP.IsIRN = true;
                                _dbContext.Update(aPIGP);
                                aPIGPDetails.IsIRNCreated = true;
                            }
                            else
                            {
                                APIGP aPIGP = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APIRNVM.APIRN.IGPNo);
                                aPIGP.IsIRN = false;
                                _dbContext.Update(aPIGP);
                                aPIGPDetails.IsIRNCreated = false;
                            }
                            Apdetails.Add(aPIGPDetails);
                        }
                    }
                }
                List<APIRNDetails> Details = new List<APIRNDetails>();
                for (int i = 0; i < collection["id"].Count; i++)
                {
                    APIRNDetails irnDetails = new APIRNDetails();
                    irnDetails.ItemCode = Convert.ToString(collection["code"][i]);
                    irnDetails.ItemDiscription = Convert.ToString(collection["description"][i]);
                    irnDetails.ItemID = Convert.ToInt32(collection["itemId"][i]);
                    irnDetails.Received_Qty = Convert.ToDecimal(collection["received"][i]);
                    irnDetails.PrID = Convert.ToInt32(collection["poNo"][i]);
                    irnDetails.PrDetailId = Convert.ToInt32(collection["prDetailId"][i]);
                    irnDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id.ToString();
                    irnDetails.Rejected_Qty = Convert.ToDecimal(collection["rQty"][i]);
                    irnDetails.OGPBalQty = Convert.ToInt32(collection["rQty"][i]);
                    irnDetails.OGPQty = Convert.ToInt32(collection["rQty"][i]);
                    irnDetails.Accepted_Qty = Convert.ToDecimal(collection["AQty"][i]);
                    irnDetails.IGP_Qty = Convert.ToDecimal(collection["igpQty"][i]);
                    irnDetails.IGPDetailId = Convert.ToInt32(collection["igpDetailId"][i]);
                    irnDetails.CategoryId = _dbContext.InvItemCategories.FirstOrDefault(x => x.Name == Convert.ToString(collection["category"][i])).Id;
                    irnDetails.IRNID = APIRNVM.APIRN.Id;
                    Details.Add(irnDetails);
                    APIGPDetails aPIGPDetails = _dbContext.APIGPDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["igpDetailId"][i]));
                    aPIGPDetails.BalQty = irnDetails.Rejected_Qty;
                    aPIGPDetails.RCDQty = irnDetails.Received_Qty;
                    if (aPIGPDetails.BalQty == 0)
                    {
                        APIGP aPIGP = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APIRNVM.APIRN.IGPNo);
                        aPIGP.IsIRN = true;
                        _dbContext.Update(aPIGP);
                        aPIGPDetails.IsIRNCreated = true;
                    }
                    else
                    {
                        APIGP aPIGP = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APIRNVM.APIRN.IGPNo);
                        aPIGP.IsIRN = false;
                        _dbContext.Update(aPIGP);
                        aPIGPDetails.IsIRNCreated = false;
                    }
                    Apdetails.Add(aPIGPDetails);
                };
                await _APIRNDetailsRepository.UpdateRangeAsync(UpdateList);
                await _APIRNDetailsRepository.CreateRangeAsync(Details);
                TempData["error"] = "false";
                TempData["message"] = "IRN has been updated successfully.";
            }
            return RedirectToAction("Index", "IRN");
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APIRN aPIRN = _APIRNRepository.Get(x => x.Id == id).FirstOrDefault();
            aPIRN.ApprovedBy = _userId;
            aPIRN.ApprovedDate = DateTime.UtcNow;
            aPIRN.IsApproved = true;
            await _APIRNRepository.UpdateAsync(aPIRN);
            return RedirectToAction("List", "IRN");
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APIRN aPIRN = _APIRNRepository.Get(x => x.Id == id).FirstOrDefault();
            aPIRN.ApprovedBy = null;
            aPIRN.UnApprovedBy = _userId;
            aPIRN.UnApprovedDate = DateTime.UtcNow;
            aPIRN.IsApproved = false;
            await _APIRNRepository.UpdateAsync(aPIRN);
            return RedirectToAction("List", "IRN");
        }
        public async Task<IActionResult> Delete(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            List<APIGPDetails> items = new List<APIGPDetails>();
            bool check = _dbContext.APIRNDetails.Any(x => x.IRNID == id);
            if (check)
            {
                APIRN aPIRN = _APIRNRepository.Get(x => x.Id == id).FirstOrDefault();
                aPIRN.IsDeleted = true;
                APIGP aPIGP = _dbContext.APIGP.FirstOrDefault(x => x.IGP == aPIRN.IGPNo);
                aPIGP.IsIRN = false;
                _dbContext.Update(aPIGP);
                await _APIRNRepository.UpdateAsync(aPIRN);
                var Detail = _APIRNDetailsRepository.Get(x => x.IRNID == id).ToList();
                if (!ReferenceEquals(Detail, null))
                {
                    foreach (var grp in Detail)
                    {
                        APIGPDetails aPIGPDetails = _dbContext.APIGPDetails.FirstOrDefault(x => x.Id == grp.IGPDetailId);
                        aPIGPDetails.IsIRNCreated = false;
                        aPIGPDetails.BalQty = aPIGPDetails.BalQty + grp.Accepted_Qty;
                        aPIGPDetails.RCDQty = aPIGPDetails.RCDQty - grp.Accepted_Qty;
                        items.Add(aPIGPDetails);
                    }
                    _dbContext.UpdateRange(items);
                    await _APIRNDetailsRepository.DeleteRangeAsync(Detail);

                    TempData["error"] = "false";
                    TempData["message"] = "IRN has been deleted successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Please UnApproved GRN.";
                }
            }
            return RedirectToAction("List", "IRN");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var aPIRN = _dbContext.APIRN.Include(i => i.Vendor).Where(i => i.Id == id).FirstOrDefault();
            var aPIRNDetails = _dbContext.APIRNDetails
                                .Include(i => i.Category)
                                .Where(i => i.IRNID == id)
                                .ToList();
            ViewBag.NavbarHeading = "Inspection and Receipt Note (IRN)";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = aPIRNDetails;
            return View(aPIRN);
        }
    }
}
