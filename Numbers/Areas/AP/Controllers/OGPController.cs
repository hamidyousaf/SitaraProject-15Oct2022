using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    public class OGPController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly APOGPRepository _APOGPRepository;
        private readonly APOGPDetailsRepository _APOGPDetailsRepository;
        public OGPController(NumbersDbContext context, APOGPRepository APOGPRepository, APOGPDetailsRepository APOGPDetailsRepository)
        {
            _dbContext = context;
            _APOGPRepository = APOGPRepository;
            _APOGPDetailsRepository = APOGPDetailsRepository;
        }
        public IActionResult Index(int id)
        {
            APOGPViewModel IRNVM = new APOGPViewModel();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted != true && x.IsActive != false).ToList(), "Id", "Name");
            //ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => x.CompanyId == companyId && x.IsActive != false).ToList(), "Id", "Name");
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            if (id == 0)
            {
                var result = _dbContext.APOGP.Where(x => x.CompanyId == companyId).ToList();
                if (result.Count > 0)
                {
                    ViewBag.Id = _dbContext.APOGP.Select(x => x.OGPNo).Max() + 1;
                    TempData["MaxOGPNo"] = ViewBag.Id;
                }
                else
                {
                    ViewBag.Id = 1;
                }
                ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => /*x.CompanyId == companyId &&*/ x.IsActive != false).ToList(), "Id", "Name");
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create OutWard Gate Pass (OGP)";
            }
            else
            {
                ViewBag.IRNNo = id;
                IRNVM.APOGP = _APOGPRepository.Get(x => x.Id == id).FirstOrDefault();
                var model = _APOGPDetailsRepository.Get(x => x.OGPId == id).Include(x=>x.Item).ThenInclude(x=>x.UOM).ToList();
                IRNVM.APOGPDetails = new List<APOGPDetails>();
                IRNVM.Balc = new List<decimal>();
                ViewBag.IGPList = new SelectList(_dbContext.APIRN.Where(x => x.Id == IRNVM.APOGP.IRNId && x.CompanyId == companyId).ToList(), "IRNNo", "IRNNo");
                ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => x.Id == IRNVM.APOGP.VendorID && x.IsActive != false).ToList(), "Id", "Name");
                foreach (var grp in model)
                {
                    string brand = null;
                    if (grp.BrandId != 0)
                    {
                        brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.BrandId)).ConfigValue;
                    }
                    decimal balc = _dbContext.APIRNDetails.FirstOrDefault(x => x.Id == grp.IRNDetailId).OGPBalQty;
                    if (brand != null)
                    {
                        IRNVM.Brand.Add(brand);
                    }
                    IRNVM.Balc.Add(balc);
                    IRNVM.APOGPDetails.Add(grp);
                }
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update OutWard Gate Pass (OGP)";
            }
            return View(IRNVM);
        }
        public IActionResult GetIRNLis(int id)
        {
            if (id != 0)
            {
                List<ItemListViewModel> itemListViewModel = new List<ItemListViewModel>();
                var igpId = (from m in _dbContext.APIRN where m.Id == id && m.IsApproved == true && m.IsDeleted == false select m).FirstOrDefault();
                var IGPList = (from a in _dbContext.APIRN.Where(x => x.Id == igpId.Id && x.IsApproved == true) join b in _dbContext.APIRNDetails.Include(x=>x.Item).Where(x=>x.Rejected_Qty > 0) on a.Id equals b.IRNID where b.IRNID == igpId.Id && b.IsOGPCreated == false select b).ToList();
                foreach (var frp in IGPList)
                {
                    ItemListViewModel model = new ItemListViewModel();
                    model.APIRNDetails = frp;
                    //model.type = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(igpId.POTypeId)).ConfigValue;
                    model.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(frp.UOM)).ConfigValue;
                    if (frp.BrandId != 0)
                    {
                        model.BrandName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(frp.BrandId)).ConfigValue;
                    }
                    //model.Category = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == frp.CategoryId).Name;
                    itemListViewModel.Add(model);
                }
                return Ok(IGPList);
            }
            else
            {
                return Ok();
            }
        }
        public IActionResult GetOGP()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var searchOGPNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchIRNNo = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchOGPDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchVendor = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchApprovedBy = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from tempcustomer in _dbContext.APOGP.Include(x=>x.Vendor).Include(x=>x.User).Include(x=>x.IRN).Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                Data = !string.IsNullOrEmpty(searchOGPNo) ? Data.Where(m => m.OGPNo.ToString().ToLower().Contains(searchOGPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchIRNNo) ? Data.Where(m => m.IRN.IRNNo.ToString().ToLower().Contains(searchIRNNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchOGPDate) ? Data.Where(m => m.OGPDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchOGPDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchVendor) ? Data.Where(m => m.Vendor.Name.ToString().ToUpper().Contains(searchVendor.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchCreatedBy) ? Data.Where(m => m.User.UserName.ToString().ToUpper().Contains(searchCreatedBy.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchApprovedBy) ? Data.Where(m => m.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == m.ApprovedBy).UserName.ToUpper().Contains(searchVendor.ToUpper()) : false) : Data;
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
                List<APOGPViewModel> Details = new List<APOGPViewModel>();
                foreach (var grp in data)
                {
                    APOGPViewModel APOGPViewModel = new APOGPViewModel();
                    APOGPViewModel.OGPDate = grp.OGPDate.ToString(Helpers.CommonHelper.DateFormat);
                    APOGPViewModel.APOGP = grp;
                    APOGPViewModel.APOGP.ApprovalUser = grp.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == grp.ApprovedBy).UserName : "" ;
                    APOGPViewModel.APOGP.Approve = approve;
                    APOGPViewModel.APOGP.Unapprove = unApprove;
                    Details.Add(APOGPViewModel);

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
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=OGPBP&cId=", companyId, "&Id=");

            var iRNList = new List<APOGPViewModel>();
            var model = _dbContext.APOGP.Where(x => x.IsDeleted == false).ToList();
            foreach (var grp in model)
            {
                APOGPViewModel APOGPViewModel = new APOGPViewModel();
                APOGPViewModel.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.VendorID).Name;
                //    APOGPViewModel.DepartmentName = _dbContext.GLDivision.FirstOrDefault(x => x.Id == grp.DepartmentId).Name;
                APOGPViewModel.APOGP = grp;
                iRNList.Add(APOGPViewModel);
            }

            ViewBag.NavbarHeading = "List of OutWard Gate Pass (OGP)";
            return View(iRNList);
        }
        public int GetMaxIRN(int companyId)
        {
            int IRN = 1;
            var IRNno = _dbContext.APOGP.Where(c => c.CompanyId == companyId).ToList();
            if (IRNno.Count > 0)
            {
                IRN = IRNno.Max(r => r.OGPNo);
                return IRN + 1;
            }
            else
            {
                return IRN;
            }
        }
        public async Task<IActionResult> Submit(APOGPViewModel APOGPViewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            APOGPDetails[] APOGPDetails = JsonConvert.DeserializeObject<APOGPDetails[]>(collection["ItemDetail"]);
            List<APIRNDetails> Apdetails = new List<APIRNDetails>();
            string module = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == resp_Id select c.Resp_Name).FirstOrDefault();
            if (APOGPViewModel.APOGP.Id == 0)
            {
                try
                {
                    APOGPViewModel.APOGP.CreatedBy = userId;
                    APOGPViewModel.APOGP.CreatedDate = DateTime.UtcNow;
                    APOGPViewModel.APOGP.CompanyId = companyId;
                    APOGPViewModel.APOGP.Resp_ID = resp_Id;
                    APOGPViewModel.APOGP.IsActive = true;
                    APOGPViewModel.APOGP.Status = "Created";
                    APOGPViewModel.APOGP.OGPNo = GetMaxIRN(companyId);
                    APOGPViewModel.APOGP.OperatingId = _dbContext.APIGP.FirstOrDefault(x => x.IGP == APOGPViewModel.APOGP.OGPNo).OperatingId;
                    await _APOGPRepository.CreateAsync(APOGPViewModel.APOGP);
                    List<APOGPDetails> Details = new List<APOGPDetails>();
                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        APOGPDetails irnDetails = new APOGPDetails();
                        irnDetails.OGPId = APOGPViewModel.APOGP.Id;
                        irnDetails.PrID = Convert.ToInt32(collection["PrId"][i]);
                        irnDetails.ItemId = Convert.ToInt32(collection["itemId"][i]);
                        irnDetails.IRNRejectedQty = Convert.ToInt32(collection["iRNRejectedQty"][i]);
                        irnDetails.ReceivedQty = Convert.ToInt32(collection["OGPQty"][i]);
                        irnDetails.BalanceQty = Convert.ToInt32(collection["BalQty"][i]);
                        irnDetails.IRNDetailId = Convert.ToInt32(collection["iRNDetailId"][i]);
                        irnDetails.PrDetailId = Convert.ToInt32(collection["PrDetailId"][i]);
                        irnDetails.IRNId = Convert.ToInt32(collection["iRNId"][i]);
                        irnDetails.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;
                        Details.Add(irnDetails);

                        APIRNDetails aPIRNDetails = _dbContext.APIRNDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(irnDetails.IRNDetailId));
                        aPIRNDetails.OGPBalQty = aPIRNDetails.OGPBalQty - irnDetails.ReceivedQty;

                        if (aPIRNDetails.OGPBalQty == 0)
                        {
                            APIRN aPIRN = _dbContext.APIRN.FirstOrDefault(x => x.Id == APOGPViewModel.APOGP.IRNId);
                            aPIRN.IsOGP = true;
                            _dbContext.Update(aPIRN);
                            aPIRNDetails.IsOGPCreated = true;
                        }
                        else
                        {
                            APIRN aPIGP = _dbContext.APIRN.FirstOrDefault(x => x.Id == APOGPViewModel.APOGP.IRNId);
                            aPIGP.IsOGP = false;
                            _dbContext.Update(aPIGP);
                            aPIRNDetails.IsOGPCreated = false;
                        }
                        Apdetails.Add(aPIRNDetails);

                    };
                    _dbContext.SaveChanges();
                    await _APOGPDetailsRepository.CreateRangeAsync(Details);
                    TempData["error"] = "false";
                    TempData["message"] = "OGP " + APOGPViewModel.APOGP.OGPNo + " has been created successfully.";
                }
                catch (Exception ex)
                {
                    await _APOGPRepository.DeleteAsync(APOGPViewModel.APOGP);
                    var DeleteList = _APOGPDetailsRepository.Get(x => x.OGPId == APOGPViewModel.APOGP.Id).ToList();
                    await _APOGPDetailsRepository.DeleteRangeAsync(DeleteList);
                    TempData["error"] = "true";
                    TempData["message"] = "Went something wrong.";
                }
            }
            else
            {
                try
                {
                    APOGP aPOGP = _APOGPRepository.Get(x => x.Id == APOGPViewModel.APOGP.Id).FirstOrDefault();
                    aPOGP.OGPNo = APOGPViewModel.APOGP.OGPNo;
                    aPOGP.OGPDate = APOGPViewModel.APOGP.OGPDate;
                    aPOGP.IRNId = APOGPViewModel.APOGP.IRNId;
                    aPOGP.DriverName = APOGPViewModel.APOGP.DriverName;
                    aPOGP.TransportCompany = APOGPViewModel.APOGP.TransportCompany;
                    aPOGP.VehicleType = APOGPViewModel.APOGP.VehicleType;
                    aPOGP.Vehicle = APOGPViewModel.APOGP.Vehicle;
                    aPOGP.Bility = APOGPViewModel.APOGP.Bility;
                    aPOGP.Remarks = APOGPViewModel.APOGP.Remarks;
                    aPOGP.IsDeleted = false;
                    aPOGP.UpdatedBy = userId;
                    aPOGP.UpdatedDate = DateTime.Now;
                    aPOGP.CompanyId = companyId;
                    aPOGP.Resp_ID = resp_Id;
                    aPOGP.IsActive = true;
                    await _APOGPRepository.UpdateAsync(aPOGP);
                    var existingDetail = _APOGPDetailsRepository.Get(a => a.OGPId == APOGPViewModel.APOGP.Id).ToList();
                    //Deleting monthly target limit
                    foreach (var detail in existingDetail)
                    {
                        bool isExist = APOGPDetails.Any(x => x.Id == detail.Id);
                        if (!isExist)
                        {
                            //Handling Balance
                            var iRNDet = _dbContext.APIRNDetails.FirstOrDefault(x => x.Id == detail.IRNDetailId);
                            iRNDet.OGPBalQty = iRNDet.OGPBalQty + detail.ReceivedQty;
                            APIRN aPIRN = _dbContext.APIRN.FirstOrDefault(x => x.Id == iRNDet.IRNID);
                            aPIRN.IsOGP = false;
                            _dbContext.Update(aPIRN);
                            iRNDet.IsOGPCreated = false;
                            _dbContext.Update(iRNDet);
                            await _dbContext.SaveChangesAsync();
                            //----------
                            _dbContext.APOGPDetails.Remove(detail);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                    //Inserting/Updating monthly limit
                    foreach (var detail in APOGPDetails)
                    {
                        if (detail.Id == 0) //Inserting New Records
                        {
                            APOGPDetails irnDetails = new APOGPDetails();
                            irnDetails.OGPId = APOGPViewModel.APOGP.Id;
                            irnDetails.PrID = detail.PrID;
                            irnDetails.ItemId = detail.ItemId;
                            irnDetails.IRNRejectedQty = detail.IRNRejectedQty;
                            irnDetails.ReceivedQty = detail.ReceivedQty;
                            irnDetails.BalanceQty = detail.BalanceQty;
                            irnDetails.IRNDetailId = detail.IRNDetailId;
                            irnDetails.PrDetailId = detail.PrDetailId;
                            irnDetails.IRNId = detail.IRNId;
                            //Handling Balance
                            var iRNDet = _dbContext.APIRNDetails.FirstOrDefault(x => x.Id == irnDetails.IRNDetailId);
                            iRNDet.OGPBalQty = iRNDet.OGPBalQty - detail.ReceivedQty;

                            if (iRNDet.OGPBalQty == 0)
                            {
                                APIRN aPIRN = _dbContext.APIRN.FirstOrDefault(x => x.Id == APOGPViewModel.APOGP.IRNId);
                                aPIRN.IsOGP = true;
                                _dbContext.Update(aPIRN);
                                iRNDet.IsOGPCreated = true;
                            }
                            else
                            {
                                APIRN aPIGP = _dbContext.APIRN.FirstOrDefault(x => x.Id == APOGPViewModel.APOGP.IRNId);
                                aPIGP.IsOGP = false;
                                _dbContext.Update(aPIGP);
                                iRNDet.IsOGPCreated = false;
                            }
                            await _dbContext.SaveChangesAsync();
                            //----------
                            await _dbContext.APOGPDetails.AddAsync(irnDetails);
                        }
                        else   //Updating Records
                        {
                            var aPOGPDetails = _dbContext.APOGPDetails.FirstOrDefault(x => x.Id == detail.Id);
                            var irnDet = _dbContext.APIRNDetails.FirstOrDefault(x=>x.Id == detail.IRNDetailId);
                            if (aPOGPDetails.ReceivedQty > detail.ReceivedQty)
                            {
                                irnDet.OGPBalQty = aPOGPDetails.ReceivedQty - detail.ReceivedQty;
                                _dbContext.APIRNDetails.Update(irnDet);
                            }
                            if (aPOGPDetails.ReceivedQty < detail.ReceivedQty)
                            {
                                irnDet.OGPBalQty = aPOGPDetails.ReceivedQty + irnDet.OGPBalQty - detail.ReceivedQty;
                                _dbContext.APIRNDetails.Update(irnDet);
                            }
                            if (irnDet.OGPBalQty == 0)
                            {
                                APIRN aPIRN = _dbContext.APIRN.FirstOrDefault(x => x.Id == APOGPViewModel.APOGP.IRNId);
                                aPIRN.IsOGP = true;
                                _dbContext.Update(aPIRN);
                                irnDet.IsOGPCreated = true;
                            }
                            else
                            {
                                APIRN aPIGP = _dbContext.APIRN.FirstOrDefault(x => x.Id == APOGPViewModel.APOGP.IRNId);
                                aPIGP.IsOGP = false;
                                
                                _dbContext.Update(aPIGP);
                                irnDet.IsOGPCreated = false;
                            }
                            await _dbContext.SaveChangesAsync();
                            aPOGPDetails.OGPId = APOGPViewModel.APOGP.Id;
                            aPOGPDetails.PrID = detail.PrID;
                            aPOGPDetails.ItemId = detail.ItemId;
                            aPOGPDetails.IRNRejectedQty = detail.IRNRejectedQty;
                            aPOGPDetails.ReceivedQty = detail.ReceivedQty;
                            aPOGPDetails.BalanceQty = detail.BalanceQty;
                            aPOGPDetails.IRNDetailId = detail.IRNDetailId;
                            aPOGPDetails.PrDetailId = detail.PrDetailId;
                            aPOGPDetails.IRNId = detail.IRNId;
                            _dbContext.APOGPDetails.Update(aPOGPDetails);
                        }
                        await _dbContext.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "OGP");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    string message = ex.Message.ToString();
                }
            }
            return RedirectToAction("Index", "OGP");
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APOGP APOGP = _APOGPRepository.Get(x => x.Id == id).FirstOrDefault();
            APOGP.ApprovedBy = _userId;
            APOGP.ApprovedDate = DateTime.UtcNow;
            APOGP.IsApproved = true;
            await _APOGPRepository.UpdateAsync(APOGP);
            return RedirectToAction("List", "OGP");
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APOGP APOGP = _APOGPRepository.Get(x => x.Id == id).FirstOrDefault();
            APOGP.ApprovedBy = null;
            APOGP.UnApprovedBy = _userId;
            APOGP.UnApprovedDate = DateTime.UtcNow;
            APOGP.IsApproved = false;
            await _APOGPRepository.UpdateAsync(APOGP);
            return RedirectToAction("List", "OGP");
        }
        public async Task<IActionResult> Delete(int id)
        {

            var aPOGP = _dbContext.APOGP.Find(id);
            aPOGP.IsDeleted = true;
            var entry = _dbContext.APOGP.Update(aPOGP);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            var aPIRNDetails = _dbContext.APIRNDetails.Where(x => x.IRNID == aPOGP.IRNId);
            var modelDetails = _dbContext.APOGPDetails.Where(x => x.OGPId == aPOGP.Id).ToList();
            //Handling balance
            foreach (var detail in modelDetails)
            {
                foreach (var IRNDetails in aPIRNDetails)
                {
                    if (detail.IRNDetailId == IRNDetails.Id)
                    {
                        IRNDetails.OGPBalQty = IRNDetails.OGPBalQty + detail.ReceivedQty;

                        APIRN aPIRN = _dbContext.APIRN.FirstOrDefault(x => x.Id == IRNDetails.IRNID);
                        aPIRN.IsOGP = false;
                        _dbContext.Update(aPIRN);
                        IRNDetails.IsOGPCreated = false;
                        _dbContext.Update(IRNDetails);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            //----------
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("List", "OGP");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var APOGP = _dbContext.APOGP.Include(i => i.Vendor).Where(i => i.Id == id).FirstOrDefault();
            var APOGPDetails = _dbContext.APOGPDetails
                                .Include(i => i.Item).ThenInclude(x=>x.UOM)
                                .Include(i => i.IRN)
                                .Where(i => i.OGPId == id)
                                .ToList();
            ViewBag.TotalQuantity = APOGPDetails.Sum(x=>x.ReceivedQty);
            ViewBag.NavbarHeading = "OutWard Gate Pass (OGP)";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = APOGPDetails;
            return View(APOGP);
        }
    }
}
