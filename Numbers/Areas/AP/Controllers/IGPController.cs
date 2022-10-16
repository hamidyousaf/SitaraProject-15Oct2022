using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using Numbers.Repository.AP;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VoucherHelper = Numbers.Repository.Helpers.VoucherHelper;

namespace Numbers.Controllers
{
    [Area("AP")]
    [Authorize]
    public class IGPController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly APIGPRepository _APIGPRepository;
        private readonly APIGPDetailsRepository _APIGPDetailsRepository;
        public IGPController(NumbersDbContext context, APIGPRepository APIGPRepository, APIGPDetailsRepository APIGPDetailsRepository)
        {
            _dbContext = context;
            _APIGPRepository = APIGPRepository;
            _APIGPDetailsRepository = APIGPDetailsRepository;
        }
        public IActionResult Index(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var responcibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x=>x.Resp_Id == resp_Id).Resp_Name;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.Type = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == resp_Id select c.TypeId).FirstOrDefault();
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.PoType = configValues.GetConfigValues("AP", "igpOrder Type", companyId);
            ViewBag.FreightType = configValues.GetConfigValues("AP", "Freight Type", companyId);
            var aPIGPist = new APIGPVm();
            //ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => x.IsActive).ToList(), "Id", "Name");
            ViewBag.Vendor = new SelectList(responcibility == "Yarn Purchase" ?
                        (from Suppliers in _dbContext.APSuppliers.Where(x => x.IsActive == true && x.Account.Code == "2.02.04.0003" /*&& x.CompanyId == companyId*/).Include(a => a.Account) select Suppliers) :
                        (from Suppliers in _dbContext.APSuppliers.Where(x => x.IsActive == true && x.Account.Code != "2.02.04.0003"/* && x.CompanyId == companyId*/).Include(a => a.Account) select Suppliers), "Id", "Name");
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            if (id == 0)
            {
                var result = _APIGPRepository.Get(x => x.IsActive).ToList();
                if (result.Count > 0)
                {
                    ViewBag.Id = _dbContext.APIGP.Select(x => x.IGP).Max() + 1;
                    TempData["MaxIGPNo"] = ViewBag.Id;
                }
                else
                {
                    ViewBag.Id = 1;
                }
                ViewBag.EntityState = "Save";
            }
            else
            {
                aPIGPist.APIGP = _APIGPRepository.Get(x => x.Id == id).FirstOrDefault();
                ViewBag.Id = aPIGPist.APIGP.IGP;
                ViewBag.EntityState = "Update";
                var model = _APIGPDetailsRepository.Get(x => x.IGPId == id).ToList();
                aPIGPist.APIGPDetails = new List<APIGPDetails>();
                aPIGPist.UOMName = new List<string>();
                aPIGPist.Balc = new List<decimal>();
                aPIGPist.Rcd = new List<decimal>();
                foreach (var grp in model)
                {
                    string UOmName = null;
                    string brand = null;
                    decimal balc = 0;
                    decimal rcd = 0;
                    UOmName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.UOM)).ConfigValue;
                    if (grp.BrandId != 0)
                    {
                        brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.BrandId)).ConfigValue;
                    }
                    if (grp.PoDetailId != 0)
                    {
                        balc = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == grp.PoDetailId).IGPBalc;
                        rcd = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == grp.PoDetailId).IGPRcd;
                    }
                    else
                    {
                        rcd = grp.PoQty;
                    }
                    grp.CategoryName = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.CategoryId).Name;
                    var TotalRecieved = _dbContext.APIGPDetails.Where(x=>x.PoNo == grp.PoNo && x.ItemId == grp.ItemId).Sum(x=>x.RCDQty);

                    aPIGPist.Balc.Add(balc);
                    aPIGPist.Rcd.Add(rcd);
                    aPIGPist.UOMName.Add(UOmName);
                    aPIGPist.APIGPDetails.Add(grp);
                    aPIGPist.TotalRecieved.Add(TotalRecieved);
                    if (brand != null)
                    {
                        aPIGPist.Brand.Add(brand);
                    }
                }
            }

            //model.APIGP = (from APIGP in _dbContext.APIGP select APIGP).ToList();
            //model.APIGPDetails = (from APIGPDetails in _dbContext.APIGPDetails select APIGPDetails).ToList();
            return View(aPIGPist);
        }
        public IActionResult List()
        {
            var IGPList = new List<APIGPVm>();
            var companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=IGPBasePrint&cId=", companyId, "&id=");
            ViewBag.ReportPathVoucher = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            var model = _APIGPRepository.Get(x => x.IsDeleted == false).ToList();
            foreach (var grp in model)
            {
                APIGPVm aPIGPVm = new APIGPVm();
                aPIGPVm.Vendor = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.VendorId).Name;
                aPIGPVm.APIGP = grp;
                IGPList.Add(aPIGPVm);
            }
            ViewBag.NavbarHeading = "List of Inward Gate Pass(IGP)";
            return View(IGPList);
        }
        public IActionResult FindPo(int id, string type)
        {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (id != 0 && type != "Import")
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                var Po = (from a in _dbContext.APPurchaseOrderItems.Where(x => x.IsDeleted == false && x.IGPBalc > 0 && x.IsClosed!=true && x.IsIGP == false)
                          join b in _dbContext.APPurchaseOrders on a.POId equals b.Id
                          where b.SupplierId == id
                          && b.IsDeleted == false && b.Status == "Approved" && b.Status!="Closed" && b.Resp_ID == resp_Id && b.CompanyId == companyId
                          select new
                          {
                              a,
                              b
                          }).ToList().OrderByDescending(x=>x.b.Id);
                foreach (var er in Po)
                {
                    er.a.Date = er.a.DeliveryDate.ToString(Helpers.CommonHelper.DateFormat);
                    er.b.APPurchaseOrderItems = null;
                }
                return Ok(Po);
            }
            else
            {
                var Po = (from a in _dbContext.APShipmentDetails.Where(x => x.IsIGP == false)
                          join b in _dbContext.APShipment on a.APShipmentId equals b.Id
                          where b.Vendor == Convert.ToString(id) && b.IsApproved && b.CompanyId == companyId
                          select new
                          {
                              a,    
                              b
                          }).ToList();
                foreach (var er in Po)
                {
                    er.a.Date = er.b.CreatedDate.ToString(Helpers.CommonHelper.DateFormat);
                }
                return Ok(Po);
            }
        }
        public IActionResult GetPODetails(int id, string type)
        {
            List<ItemListViewModel> itemListViewModel = new List<ItemListViewModel>();
            if (type != "Import")
            {
                var PoList = _dbContext.APPurchaseOrderItems.Where(x => x.Id == id).ToList();
                foreach (var frp in PoList)
                {
                    ItemListViewModel model = new ItemListViewModel();
                    model.DilveryDate = frp.DeliveryDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.orderItem = frp;
                    model.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(frp.UOM)).ConfigValue;
                    model.Category = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == frp.Category).Name;
                    model.CategoryId = frp.Category;
                    if (frp.BrandId!=0 )
                    {
                        model.Brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x=>x.Id== Convert.ToInt32(frp.BrandId)).ConfigValue;
                    }
                    model.TotalRecieved = _dbContext.APIGPDetails.Where(x => x.PoDetailId == id && x.ItemId == frp.ItemId).Sum(x=>x.RCDQty);
                    itemListViewModel.Add(model);
                }
            }
            else
            {
                var LC = _dbContext.APShipmentDetails.Where(x => x.Id == id).FirstOrDefault();
                var Date = _dbContext.APShipment.Where(x => x.Id == LC.APShipmentId).FirstOrDefault();
                ItemListViewModel model = new ItemListViewModel();
                model.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(LC.UOM)).ConfigValue;
                model.Category = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == LC.Category).Name;
                model.CategoryId = LC.Category;
                model.DilveryDate = Date.CreatedDate.ToString(Helpers.CommonHelper.DateFormat);
                //model.orderItem.FCValue = LC.FCValue;
                model.orderItem = new APPurchaseOrderItem();
                model.orderItem.Id = LC.Id;
                model.orderItem.PrDetailId = LC.PrDetailId;
                model.orderItem.ItemCode = LC.ItemCode;
                model.orderItem.ItemId = LC.ItemId;
                model.orderItem.ItemDescription = LC.Description;
                model.orderItem.POId = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == LC.PoDetailId).POId;
                model.orderItem.Qty = LC.ShippedQty;
                model.BuilityNo = _dbContext.APShipment.FirstOrDefault(x => x.Id == Date.Id).BuiltyNo;
                model.orderItem.IGPRcd = LC.ShippedQty;
                model.orderItem.IGPBalc = 0;
                model.shipmentDetail = LC;
                itemListViewModel.Add(model);
            }
            return Ok(itemListViewModel);
        }
        public IActionResult GetIGP()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=IGPBasePrint&cId=", companyId, "&id={0}");
            ViewBag.ReportPathVoucher = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
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

                var searchIGP = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchIGPDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchName = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchBilty = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchVehicleNO = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchDC = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchUnloadingAmount = Request.Form["columns[6][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[7][search][value]"].FirstOrDefault();
                var searchApprovedBy = Request.Form["columns[8][search][value]"].FirstOrDefault();




                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var IGPData = (from tempcustomer in _dbContext.APIGP.Include(x => x.CreatedUser).Include(x => x.ApprovalUser).Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    IGPData = IGPData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    IGPData = IGPData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                IGPData = !string.IsNullOrEmpty(searchIGP) ? IGPData.Where(m => m.IGP.ToString().Contains(searchIGP)) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchIGPDate) ? IGPData.Where(m => m.IGPDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchIGPDate.ToUpper())) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchName) ? IGPData.Where(m => (m.VendorId.ToString().ToUpper()).Contains(searchName.ToUpper())) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchBilty) ? IGPData.Where(m => m.Bility != null ? (m.Bility.ToString().ToUpper()).Contains(searchBilty.ToUpper()) : false) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchVehicleNO) ? IGPData.Where(m => m.Vehicle != null ? (m.Vehicle.ToString().ToUpper()).Contains(searchVehicleNO.ToUpper()) : false) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchDC) ? IGPData.Where(m => m.DC != null ? (m.DC.ToString().ToUpper()).Contains(searchDC.ToUpper()) : false) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchUnloadingAmount) ? IGPData.Where(m => (m.UnloadingAmount.ToString().ToUpper()).Contains(searchUnloadingAmount.ToUpper())) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchCreatedBy) ? IGPData.Where(m => m.CreatedUser.UserName.ToString().ToUpper().Contains(searchCreatedBy.ToUpper())) : IGPData;
                IGPData = !string.IsNullOrEmpty(searchApprovedBy) ? IGPData.Where(m => m.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == m.ApprovedBy).UserName.ToUpper().Contains(searchApprovedBy.ToUpper()) : false) : IGPData;
                recordsTotal = IGPData.Count();
                var data = IGPData.ToList();
                recordsTotal = IGPData.Count();
               
                if (pageSize == -1)
                {
                    data = IGPData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = IGPData.Skip(skip).Take(pageSize).ToList();
                }
                List<APIGPVm> Details = new List<APIGPVm>();
                foreach (var grp in data)
                {
                    APIGPVm aPIGP = new APIGPVm();
                    aPIGP.Vendor = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.VendorId).Name;
                    aPIGP.IGPDate = grp.IGPDate.ToString(Helpers.CommonHelper.DateFormat);
                    aPIGP.CreatedBy = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).UserName;
                    aPIGP.VoucherId = grp.VoucherId;
                    aPIGP.APIGP = grp;
                    aPIGP.APIGP.Auser = grp.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == grp.ApprovedBy).UserName : "";
                    aPIGP.APIGP.Approve = approve;
                    aPIGP.APIGP.Unapprove = unApprove;
                    aPIGP.Vehicle = grp.Vehicle;
                    Details.Add(aPIGP);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public int GetMaxIGP(int companyId)
        {
            int GRN = 1;
            var GRNNo = _dbContext.APIGP.Where(c => c.CompanyId == companyId).ToList();
            if (GRNNo.Count > 0)
            {
                GRN = GRNNo.Max(r => r.IGP);
                return GRN + 1;
            }
            else
            {
                return GRN;
            }
        }
        public async Task<IActionResult> Submit(APIGPVm APIGPVms, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string module = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == resp_Id select c.Resp_Name).FirstOrDefault();
            List<APPurchaseOrderItem> orderItems = new List<APPurchaseOrderItem>();
            List<APShipmentDetail> shipmentDetails = new List<APShipmentDetail>();
            if (APIGPVms.APIGP.Id == 0)
            {
                try
                {
                    APIGPVms.APIGP.CreatedBy = userId;
                    APIGPVms.APIGP.CreatedDate = DateTime.UtcNow;
                    APIGPVms.APIGP.CompanyId = companyId;
                    APIGPVms.APIGP.Resp_ID = resp_Id;
                    APIGPVms.APIGP.IsActive = true;
                    APIGPVms.APIGP.IGP = GetMaxIGP(companyId);
                    await _APIGPRepository.CreateAsync(APIGPVms.APIGP);
                    List<APIGPDetails> Details = new List<APIGPDetails>();
                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        APIGPDetails igpDetails = new APIGPDetails();
                        igpDetails.ItemCode = Convert.ToString(collection["code"][i]);
                        igpDetails.ItemDiscription = Convert.ToString(collection["description"][i]);
                        igpDetails.ItemId = Convert.ToInt32(collection["itemId"][i]);
                        igpDetails.PoDlvDate = Convert.ToDateTime(collection["poDlvDate"][i]);
                        igpDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id.ToString();
                        igpDetails.PoQty = Convert.ToDecimal(collection["poQty"][i]);
                        igpDetails.IGPQty = Convert.ToDecimal(collection["igpQty"][i]);
                        igpDetails.RCDQty = Convert.ToDecimal(collection["igpQty"][i]);
                        igpDetails.BalQty = Convert.ToDecimal(collection["balQty"][i]);
                        igpDetails.PoDetailId = Convert.ToInt32(collection["poDetailId"][i]);
                        igpDetails.ShipmentDetailId = Convert.ToInt32(collection["lcDetailId"][i]);
                        igpDetails.PrDetailId = Convert.ToInt32(collection["prDetailId"][i]);
                        igpDetails.Packages = Convert.ToDecimal(collection["package"][i]);
                        igpDetails.PackagesQty = Convert.ToDecimal(collection["packageQty"][i]);
                        igpDetails.PoNo = Convert.ToInt32(collection["poId"][i]);
                        igpDetails.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                        igpDetails.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;
                        //igpDetails.CategoryId = _dbContext.InvItemCategories.FirstOrDefault(x => x.Name == Convert.ToString(collection["category"][i])).Id;
                        igpDetails.IGPId = APIGPVms.APIGP.Id;
                        Details.Add(igpDetails);
                        if (igpDetails.PoDetailId != 0)
                        {
                            APPurchaseOrderItem aPPurchaseOrderItem = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["poDetailId"][i]));
                            var igpBlnce = aPPurchaseOrderItem.IGPBalc - Convert.ToDecimal(collection["igpQty"][i]);
                            var igpRcd = aPPurchaseOrderItem.IGPRcd + Convert.ToDecimal(collection["igpQty"][i]);
                            aPPurchaseOrderItem.IGPBalc = igpBlnce;
                            aPPurchaseOrderItem.IGPRcd = igpRcd;
                            if (aPPurchaseOrderItem.IGPBalc == 0)
                                aPPurchaseOrderItem.IsIGP = true;
                            else
                                aPPurchaseOrderItem.IsIGP = false;
                            orderItems.Add(aPPurchaseOrderItem);
                        }
                        else
                        {
                            APShipmentDetail aPShipmentDetail = _dbContext.APShipmentDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["lcDetailId"][i]));
                            aPShipmentDetail.IsIGP = true;
                            //aPPurchaseOrderItem.IsIGP = true;
                            shipmentDetails.Add(aPShipmentDetail);
                        }
                    };
                    _dbContext.SaveChanges();
                    await _APIGPDetailsRepository.CreateRangeAsync(Details);
                    TempData["error"] = "false";
                    TempData["message"] = "IGP "+ APIGPVms.APIGP.IGP + " has been created successfully.";
                }
                catch (Exception ex)
                {
                    await _APIGPRepository.DeleteAsync(APIGPVms.APIGP);
                    var DeleteList = _APIGPDetailsRepository.Get(x => x.IGPId == APIGPVms.APIGP.Id).ToList();
                    await _APIGPDetailsRepository.DeleteRangeAsync(DeleteList);
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong";
                }
            }
            else
            {
                APIGP aPIGP = _APIGPRepository.Get(x => x.Id == APIGPVms.APIGP.Id).FirstOrDefault();
                aPIGP.UpdatedBy = userId;
                aPIGP.UpdatedDate = DateTime.UtcNow;
                aPIGP.CompanyId = companyId;
                aPIGP.Resp_ID = resp_Id;
                aPIGP.IsActive = true;
                aPIGP.OperatingId = APIGPVms.APIGP.OperatingId;
                aPIGP.VendorId = APIGPVms.APIGP.VendorId;
                aPIGP.Bility = APIGPVms.APIGP.Bility;
                aPIGP.DC = APIGPVms.APIGP.DC;
                aPIGP.Remarks = APIGPVms.APIGP.Remarks;
                aPIGP.DriverName = APIGPVms.APIGP.DriverName;
                aPIGP.TransportCompany = APIGPVms.APIGP.TransportCompany;
                aPIGP.VehicleType = APIGPVms.APIGP.VehicleType;
                aPIGP.Vehicle = APIGPVms.APIGP.Vehicle;
                aPIGP.IGPDate = APIGPVms.APIGP.IGPDate;
                aPIGP.IGP = APIGPVms.APIGP.IGP;
                aPIGP.FreightTypeId = APIGPVms.APIGP.FreightTypeId;
                aPIGP.POTypeId = APIGPVms.APIGP.POTypeId;
                aPIGP.FreightAmount = APIGPVms.APIGP.FreightAmount;
                await _APIGPRepository.UpdateAsync(aPIGP);
                var UpdateList = new List<APIGPDetails>();
                var rownber = collection["ChildId"].Count;
                List<int> ChildList = new List<int>();
                for (int i = 0; i < rownber; i++)
                {
                    int id = Convert.ToInt32(collection["ChildId"][i]);
                    //var userid = (from u in _dbContext.Users where u.UserName == userName select u.Id).FirstOrDefault();
                    ChildList.Add(id);
                }
                var foundDetail = _APIGPDetailsRepository.Get(a => a.IGPId == APIGPVms.APIGP.Id).ToList();
                if (!ReferenceEquals(ChildList, null))
                {
                    for (int i = 0; i < foundDetail.Count; i++)
                    {
                        bool result = ChildList.Exists(s => s == foundDetail[i].Id);
                        if (!result)
                        {
                            await _APIGPDetailsRepository.DeleteAsync(foundDetail[i]);
                            if (Convert.ToInt32(collection["poDetailId"][i]) != 0)
                            {
                                APPurchaseOrderItem aPPurchaseOrderItem = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["poDetailId"][i]));
                                aPPurchaseOrderItem.IsIGP = false;
                                orderItems.Add(aPPurchaseOrderItem);
                            }
                            else
                            {
                                APShipmentDetail aPShipmentDetail = _dbContext.APShipmentDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["lcDetailId"][i]));
                                aPShipmentDetail.IsIGP = false;
                                //aPPurchaseOrderItem.IsIGP = true;
                                shipmentDetails.Add(aPShipmentDetail);
                            }
                        }
                    }
                    for (int i = 0; i < ChildList.Count; i++)
                    {
                        APIGPDetails detail = foundDetail.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["ChildId"][i]));
                        if (!ReferenceEquals(detail, null))
                        {
                            detail.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                            detail.ItemDiscription = Convert.ToString(collection["ItemDiscription"][i]);
                            detail.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                            detail.PoDlvDate = Convert.ToDateTime(collection["PoDlvDate"][i]);
                            detail.ShipmentDetailId = Convert.ToInt32(collection["LCDetailId"][i]);
                            detail.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["UOM"][i])).Id.ToString();
                            detail.PoQty = Convert.ToDecimal(collection["PoQty"][i]);
                            var prevIGPQty = detail.IGPQty;
                            detail.RCDQty = Convert.ToDecimal(collection["igpQty"][i]);
                            detail.BalQty = Convert.ToDecimal(collection["balQty"][i]);
                            detail.IGPQty = Convert.ToDecimal(collection["IgpQty"][i]);
                            detail.PoDetailId = Convert.ToInt32(collection["PoDetailId"][i]);
                            var id = detail.PoDetailId;
                            detail.PrDetailId = Convert.ToInt32(collection["PrDetailId"][i]);
                            detail.Packages = Convert.ToDecimal(collection["package"][i]);
                            detail.PackagesQty = Convert.ToDecimal(collection["PackageQty"][i]);
                            detail.PoNo = Convert.ToInt32(collection["PoId"][i]);
                            detail.CategoryId = Convert.ToInt32(collection["CategoryId"][i]);
                            //detail.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;

                            //detail.CategoryId = _dbContext.InvItemCategories.FirstOrDefault(x => x.Name == Convert.ToString(collection["Category"][i])).Id;
                            detail.IGPId = APIGPVms.APIGP.Id;
                            UpdateList.Add(detail);
                            if (id != 0)
                            {
                                APPurchaseOrderItem aPPurchaseOrderItem = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == detail.PoDetailId);
                                var igpBlnce = aPPurchaseOrderItem.IGPBalc + prevIGPQty;
                                var igpRcd = aPPurchaseOrderItem.IGPRcd - prevIGPQty;
                                igpBlnce = igpBlnce - Convert.ToDecimal(collection["IgpQty"][i]);
                                aPPurchaseOrderItem.IGPBalc = igpBlnce;
                                aPPurchaseOrderItem.IGPRcd = igpRcd + Convert.ToDecimal(collection["IgpQty"][i]);
                                if (aPPurchaseOrderItem.IGPBalc == 0)
                                    aPPurchaseOrderItem.IsIGP = true;
                                else
                                    aPPurchaseOrderItem.IsIGP = false;
                                orderItems.Add(aPPurchaseOrderItem);
                            }
                            else
                            {
                                APShipmentDetail aPShipmentDetail = _dbContext.APShipmentDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["lcDetailId"][i]));
                                aPShipmentDetail.IsIGP = true;
                                //aPPurchaseOrderItem.IsIGP = true;
                                shipmentDetails.Add(aPShipmentDetail);
                            }
                        }
                    }
                }
                List<APIGPDetails> Details = new List<APIGPDetails>();
                for (int i = 0; i < collection["id"].Count; i++)
                {
                    APIGPDetails igpDetails = new APIGPDetails();
                    igpDetails.ItemCode = Convert.ToString(collection["code"][i]);
                    igpDetails.ItemDiscription = Convert.ToString(collection["description"][i]);
                    igpDetails.ItemId = Convert.ToInt32(collection["itemId"][i]);
                    igpDetails.PoDlvDate = Convert.ToDateTime(collection["poDlvDate"][i]);
                    igpDetails.UOM = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.ConfigValue == Convert.ToString(collection["uOM"][i])).Id.ToString();
                    igpDetails.PoQty = Convert.ToDecimal(collection["poQty"][i]);
                    igpDetails.IGPQty = Convert.ToDecimal(collection["igpQty"][i]);
                    igpDetails.PoDetailId = Convert.ToInt32(collection["poDetailId"][i]);
                    var id = igpDetails.PoDetailId;
                    igpDetails.PrDetailId = Convert.ToInt32(collection["prDetailId"][i]);
                    igpDetails.ShipmentDetailId = Convert.ToInt32(collection["lcDetailId"][i]);
                    igpDetails.Packages = Convert.ToDecimal(collection["package"][i]);
                    igpDetails.PackagesQty = Convert.ToDecimal(collection["packageQty"][i]);
                    igpDetails.PoNo = Convert.ToInt32(collection["poId"][i]);
                    igpDetails.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                    //igpDetails.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;

                    //igpDetails.CategoryId = _dbContext.InvItemCategories.FirstOrDefault(x => x.Name == Convert.ToString(collection["category"][i])).Id;
                    igpDetails.IGPId = APIGPVms.APIGP.Id;
                    Details.Add(igpDetails);
                    APPurchaseOrderItem aPPurchaseOrderItem = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["poDetailId"][i]));

                    if (id != 0)
                    {
                        var igpBlnce = aPPurchaseOrderItem.IGPBalc - Convert.ToDecimal(collection["igpQty"][i]);
                        var igpRcd = aPPurchaseOrderItem.IGPRcd + Convert.ToDecimal(collection["igpQty"][i]);
                        aPPurchaseOrderItem.IGPBalc = igpBlnce;
                        aPPurchaseOrderItem.IGPRcd = igpRcd;
                        if (aPPurchaseOrderItem.IGPBalc == 0)
                            aPPurchaseOrderItem.IsIGP = true;
                        else
                            aPPurchaseOrderItem.IsIGP = false;
                        orderItems.Add(aPPurchaseOrderItem);
                    }
                    else
                    {
                        APShipmentDetail aPShipmentDetail = _dbContext.APShipmentDetails.FirstOrDefault(x => x.Id == Convert.ToInt32(collection["lcDetailId"][i]));
                        aPShipmentDetail.IsIGP = true;
                        //aPPurchaseOrderItem.IsIGP = true;
                        shipmentDetails.Add(aPShipmentDetail);
                    }

                };
                await _APIGPDetailsRepository.UpdateRangeAsync(UpdateList);
                await _APIGPDetailsRepository.CreateRangeAsync(Details);
                TempData["error"] = "false";
                TempData["message"] = "IGP has been updated successfully.";
            }
            return RedirectToAction("Index", "IGP");
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var configValues = new ConfigValues(_dbContext);
            var OrgName = configValues.GetOrgName(resp_Id, "Operating Unit", _companyId);
            var appCompanySetup = _dbContext.AppCompanySetups.ToList();
            //Approve Voucher
            APIGP igp= _dbContext.APIGP
           .Include(c => c.Vendor)
           .Where(a => a.IsApproved == false &&   a.CompanyId == _companyId && a.Id == id && a.IsDeleted == false)
           .FirstOrDefault();
            if (igp.FreightAmount > 0 || igp.UnloadingAmount > 0)
            {
                try
                {
                    //Create Voucher
                    int voucherId = 0;
                    //Own Booking 
                    GLVoucher voucherMaster = new GLVoucher();
                    List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                    string voucherDescription = string.Format(
                    "Igp # : {0} of  " +
                    "{1} {2}",
                    igp.Id,
                    igp.Vendor.Name, igp.Remarks);
                    voucherMaster.VoucherType = "IGP";
                    voucherMaster.VoucherDate = igp.IGPDate;
                    voucherMaster.Reference = "";
                    voucherMaster.Currency = "PKR";
                    voucherMaster.CurrencyExchangeRate = 1;
                    voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherMaster.Status = "Approved";
                    voucherMaster.IsSystem = true;
                    voucherMaster.CompanyId = _companyId;
                    voucherMaster.ModuleName = "AP/IGP";
                    voucherMaster.ModuleId = id;
                    voucherMaster.Amount = igp.FreightAmount;
                    voucherMaster.ApprovedDate = DateTime.Now;
                    voucherMaster.ApprovedBy = _userId;
                    //Voucher Details
                    //Credit Entry


                    if (igp.FreightAmount != 0)
                    {
                        GLVoucherDetail voucherDetail = new GLVoucherDetail();
                        var accountId = (from setup in appCompanySetup.Where(x => x.Name == "IGP Freight Credit Account")
                                         join account in _dbContext.GLAccounts.Where(x => !x.IsDeleted) on setup.Value equals account.Code
                                         select account.Id).FirstOrDefault();
                        //voucherDetail.AccountId = _dbContext.GLAccounts.Where(x => x.Name == "CASH IN HAND AT FACTORY").FirstOrDefault().Id;
                        //if (OrgName != "Sitara Hamza")
                        //{
                        //    voucherDetail.AccountId = _dbContext.GLAccounts.Where(x => x.Name == "Cash in Hand in Factory").FirstOrDefault().Id;
                        //}
                        //else
                        //{
                        //    voucherDetail.AccountId = _dbContext.GLAccounts.Where(x => x.Code == "3.02.01.0002").FirstOrDefault().Id;
                        //}
                        voucherDetail.AccountId = accountId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = igp.FreightAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                        //Debit Entry For Freight Amount
                        voucherDetail = new GLVoucherDetail();

                        accountId = (from setup in appCompanySetup.Where(x => x.Name == "IGP Freight Debit Account")
                                     join account in _dbContext.GLAccounts.Where(x => !x.IsDeleted) on setup.Value equals account.Code
                                     select account.Id).FirstOrDefault();
                        //voucherDetail.AccountId = accountId.FirstOrDefault().Id;
                        voucherDetail.AccountId = accountId;
                        voucherDetail.Sequence = 10;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = igp.FreightAmount;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);

                    }

                    //Credit Entry
                    // GLVoucherDetail voucherDetail = new GLVoucherDetail();

                    //voucherDetail.AccountId = _dbContext.GLAccounts.Where(x => x.Name == "CASH IN HAND AT FACTORY").FirstOrDefault().Id;

                    if (igp.UnloadingAmount != 0)
                    {
                        GLVoucherDetail voucherDetail = new GLVoucherDetail();
                        var accountId = (from setup in appCompanySetup.Where(x => x.Name == "IGP Unloading Credit Account")
                                         join account in _dbContext.GLAccounts.Where(x => !x.IsDeleted) on setup.Value equals account.Code
                                         select account.Id).FirstOrDefault();
                        voucherDetail.AccountId = accountId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = igp.UnloadingAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);

                        // Debit Entry For Unloading Amount

                       voucherDetail = new GLVoucherDetail();

                       accountId= (from setup in appCompanySetup.Where(x => x.Name == "IGP Unloading Debit Account")
                         join account in _dbContext.GLAccounts.Where(x => !x.IsDeleted) on setup.Value equals account.Code
                         select account.Id).FirstOrDefault();
                        voucherDetail.AccountId = accountId;
                        voucherDetail.Sequence = 10;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = igp.UnloadingAmount;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }

                    //Create Voucher 
                    var helper = new VoucherHelper(_dbContext, HttpContext);
                    voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                    if (voucherId != 0)
                    {
                        //APIGP aPIGP = _APIGPRepository.Get(x => x.Id == id).FirstOrDefault();
                        igp.ApprovedBy = _userId;
                        igp.ApprovedDate = DateTime.UtcNow;
                        igp.IsApproved = true;
                        igp.VoucherId = voucherId;
                        await _APIGPRepository.UpdateAsync(igp);
                        //entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        using (var transaction = _dbContext.Database.BeginTransaction())
                        {
                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();
                            //return true;
                        }
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "IGP has been approved successfully.";
                    return RedirectToAction("List", "IGP");
                }
                catch (Exception ex)
                {
                    var message = ex.Message.ToString();
                    TempData["error"] = "true";
                    TempData["message"] = "IGP not approved.";
                }
            }
            else {
                igp.ApprovedBy = _userId;
                igp.ApprovedDate = DateTime.UtcNow;
                igp.IsApproved = true;
                await _APIGPRepository.UpdateAsync(igp);
                TempData["error"] = "false";
                TempData["message"] = "IGP has been approved successfully with 0 freight amount.";
                return RedirectToAction("List", "IGP");
            }
            TempData["error"] = "true";
            TempData["message"] = "IGP not approved successfully.";
            

            return RedirectToAction("List", "IGP");
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
           
         
            bool check = _dbContext.APIGPDetails.Any(x => x.IGPId == id && x.IsIRNCreated == false);
            if (check)
            {
                var voucher = _dbContext.APIGP
                         .Where(v => v.IsDeleted == false && v.Id == id && v.IsApproved == true && v.CompanyId == _companyId).FirstOrDefault();
                if (voucher != null)
                {
                    var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == voucher.VoucherId).ToList();
                    foreach (var item in voucherDetail)
                    {
                        var tracker = _dbContext.GLVoucherDetails.Remove(item);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }


                    await _dbContext.SaveChangesAsync();

                }
                APIGP aPIGP = _APIGPRepository.Get(x => x.Id == id).FirstOrDefault();
                aPIGP.ApprovedBy = null;
                aPIGP.UnApprovedBy = _userId;
                aPIGP.UnApprovedDate = DateTime.UtcNow;
                aPIGP.IsApproved = false;
                await _APIGPRepository.UpdateAsync(aPIGP);
                TempData["error"] = "false";
                TempData["message"] = "IGP has been UnApproved sucessfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Please UnApproved IRN.";
            }

            return RedirectToAction("List", "IGP");
        }
        public async Task<IActionResult> Delete(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            List<APPurchaseOrderItem> items = new List<APPurchaseOrderItem>();
            bool check = _dbContext.APIGPDetails.Any(x => x.IGPId == id && x.IsIRNCreated == false);
            if (check)
            {
                APIGP aPIGP = _APIGPRepository.Get(x => x.Id == id).FirstOrDefault();
                aPIGP.IsDeleted = true;
                await _APIGPRepository.UpdateAsync(aPIGP);
                var Detail = _APIGPDetailsRepository.Get(x => x.IGPId == id).ToList();
                if (!ReferenceEquals(Detail, null))
                {
                    foreach (var grp in Detail)
                    {
                        APPurchaseOrderItem orderItem = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == grp.PoDetailId);
                        orderItem.IsIGP = false;
                        orderItem.IGPBalc = orderItem.IGPBalc + grp.IGPQty;
                        orderItem.IGPRcd = orderItem.IGPBalc - grp.IGPQty;
                        items.Add(orderItem);
                    }
                    _dbContext.UpdateRange(items);
                    await _APIGPDetailsRepository.DeleteRangeAsync(Detail);
                }
                TempData["error"] = "false";
                TempData["message"] = "IGP has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Please UnApproved IRN.";
            }
            return RedirectToAction("List", "IGP");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var aPIGP = _dbContext.APIGP
                .Include(i => i.Vendor)
                .Include(i => i.POType)
                .Include(i => i.Operating)
                .Where(i => i.Id == id).FirstOrDefault();
            var aPIGPDetails = _dbContext.APIGPDetails
                                .Include(i => i.Category)
                                .Include(i => i.APPurchaseOrder)
                                .Where(i => i.IGPId == id )
                                .ToList();

            APIGPVm aPIGPVm = new APIGPVm();
            aPIGPVm.APIGPDetails = new List<APIGPDetails>();
            aPIGPVm.Balc = new List<decimal>();
            aPIGPVm.Rcd = new List<decimal>();
            foreach (var grp in aPIGPDetails)
            {
                decimal balc = 0;
                decimal rcd = 0;
                if (grp.PoDetailId != 0)
                {
                    balc = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == grp.PoDetailId).IGPBalc;
                    rcd = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == grp.PoDetailId).IGPRcd;
                }
                else
                {
                    rcd = grp.PoQty;
                }
                grp.CategoryName = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.CategoryId).Name;

                aPIGPVm.Balc.Add(balc);
                aPIGPVm.Rcd.Add(rcd);
                aPIGPVm.APIGPDetails.Add(grp);
            }
            ViewBag.NavbarHeading = "Inward Gate Pass(IGP)";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = aPIGPVm;
            return View(aPIGP);
        }
        [HttpGet]
        public IActionResult BalanceCheck(int IGPQty, int PODetailId)
        {
            if (PODetailId != 0)
            {
                var aPPurchaseOrderItems = _dbContext.APPurchaseOrderItems.FirstOrDefault(x => x.Id == PODetailId);
                if (aPPurchaseOrderItems != null)
                {
                    ErrorMessage errorMessage = new ErrorMessage();
                    if (aPPurchaseOrderItems.IGPBalc < IGPQty)
                    {
                        errorMessage.Status = false;
                        errorMessage.Message = "IGP Quantity is greater than IGP Balance Quantity!";
                        return Json(errorMessage);
                    }
                    return Json(errorMessage);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUnloadingAmount(int id, decimal freightAmount, decimal unloadingAmount, DateTime unloadingDate)
        {
            var model = _dbContext.APIGP.Find(id);
            model.FreightAmount = freightAmount;
            model.UnloadingAmount = unloadingAmount;
            model.UnloadingDate = unloadingDate.Date;
            _dbContext.APIGP.Update(model);
           await  _dbContext.SaveChangesAsync();
           //await Approve(id);
            return Ok();
        }

    }
}
