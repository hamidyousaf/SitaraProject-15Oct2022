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
using Newtonsoft.Json;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class ShipmentController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly APShipmentRepository _APShipmentRepository;
        private readonly APShipmentDetailsRepository _APShipmentDetailsRepository;
        public readonly APCustomInfoRepository _APCustomInfoRepository;
        public readonly APCustomInfoDetailRepository _APCustomInfoDetailRepository;
        public ShipmentController(NumbersDbContext context, APShipmentRepository aPShipmentRepository,APShipmentDetailsRepository aPShipmentDetailsRepository,APCustomInfoRepository aPCustomInfoRepository,APCustomInfoDetailRepository aPCustomInfoDetailRepository)
        {
            _dbContext = context;
            _APShipmentDetailsRepository = aPShipmentDetailsRepository;
            _APShipmentRepository = aPShipmentRepository;
            _APCustomInfoDetailRepository = aPCustomInfoDetailRepository;
            _APCustomInfoRepository = aPCustomInfoRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string UserId = HttpContext.Session.GetString("UserId");
            APShipmentViewModel aPShipmentViewModel = new APShipmentViewModel();
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Bank = new SelectList(_dbContext.GLBankCashAccounts.Where(x => x.IsActive == true).ToList(), "Id", "AccountName");
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.Requisition = configValues.GetConfigValues("AP", "Purchase Requisition", companyId);
            ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(x => x.IsActive == true).ToList(), "Id", "Name");
            ViewBag.UserName = _dbContext.Users.FirstOrDefault(x => x.Id == UserId).UserName;
            ViewBag.InvItemCategories = (from c in _dbContext.InvItemCategories select c).ToList();
            ViewBag.AppCountries = _dbContext.AppCountries.ToList();
            ViewBag.UOM = _dbContext.AppCompanyConfigs.Where(x => x.Module == "Inventory" && x.ConfigName == "UOM" && x.CompanyId == companyId).ToList();
            ViewBag.BOLType = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Module == "AP" && x.ConfigName == "BOL Type" && x.CompanyId == companyId).ToList(),"Id","ConfigValue");
            ViewBag.Terminal = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Module == "AP" && x.ConfigName == "Terminal" && x.CompanyId == companyId).ToList(),"Id","ConfigValue");
            ViewBag.ShipmentAgent = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Module == "AP" && x.ConfigName == "ShipmentAgent" && x.CompanyId == companyId).ToList(),"Id","ConfigValue"); 
            ViewBag.DischargePort = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Module == "AP" && x.ConfigName == "DischargePort" && x.CompanyId == companyId).ToList(),"Id","ConfigValue"); 
            ViewBag.Acceptance = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Module == "AP" && x.ConfigName == "Acceptance" && x.CompanyId == companyId).ToList(),"Id","ConfigValue");
            ViewBag.ClearingAgent = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Module == "AP" && x.ConfigName == "ClearingAgent" && x.CompanyId == companyId).ToList(), "Id", "ConfigValue");
            ViewBag.GLCode = new SelectList(_dbContext.GLAccounts.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.AccountLevel == 4 && a.Code.Contains("6.01.01")).Select(a => new
            {
                id = a.Id,
                text = string.Concat(a.Code, " - ", a.Name),
                code = a.Code,
                name = a.Name
            }).ToList(), "id", "text");
            // ViewBag.LCNumber = (from c in _dbContext.APLC where c.IsActive select c).ToList();
            if (id == 0)
            {
                ViewBag.LCNumber = new SelectList(_dbContext.APLC.Where(x => x.IsActive && x.IsApproved == true && x.IsShipment == false).OrderByDescending(x => x.Id).ToList(), "Id", "LCNo");
                var result = _dbContext.APShipment.Where(x => x.IsActive).ToList();
                var foundCustomInfo = _dbContext.APCustomInfo.Where(x => x.IsActive).ToList();
                if (result.Count > 0)
                {
                    var ShipmentNo = _dbContext.APShipment.Select(x => x.ShipmentNo).Max();
                    int Shipmentno = Convert.ToInt32(ShipmentNo) + 1;
                    //ViewBag.Id = _dbContext.APShipment.Select(x => x.ShipmentNo).Max() + 1;
                    ViewBag.Id = Shipmentno;
                }
                else
                {
                    ViewBag.Id = 1;
                }
                if (foundCustomInfo.Count > 0)
                {
                    var CustomNo = _dbContext.APCustomInfo.Select(x => x.CustomNo).Max();
                    int Customno = Convert.ToInt32(CustomNo) + 1;
                    //ViewBag.Id = _dbContext.APShipment.Select(x => x.ShipmentNo).Max() + 1;
                    ViewBag.Custom = Customno;
                }
                else
                {
                    ViewBag.Custom = 1;
                }
                return View(aPShipmentViewModel);
            }
            else
            {
                var aPShipment = _APShipmentRepository.Get(x => x.Id == id).FirstOrDefault();
                ViewBag.Expense = _dbContext.APGRNExpense.Where(x => x.LCId == aPShipment.LCNo).ToList();
                ViewBag.Accounts = _dbContext.GLAccounts.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.AccountLevel == 4).ToList();
                var aPShipmentDetail = _APShipmentDetailsRepository.Get(x => x.APShipmentId == id).ToList();
                var aPCustomInfo = _APCustomInfoRepository.Get(x => x.ShipmentId == aPShipment.Id).FirstOrDefault();
                var aPCustomInfoDetail = _APCustomInfoDetailRepository.Get(x => x.CustomInfo_Id == aPCustomInfo.Id).ToList();
                ViewBag.LCNumber = new SelectList(_dbContext.APLC.Where(x => x.IsActive && x.IsApproved == true && (x.IsShipment == false||x.Id== aPShipment.LCNo)).ToList(), "Id", "LCNo");
                aPShipmentViewModel.APShipment = aPShipment;
                aPShipmentViewModel.APShipmentDetails = aPShipmentDetail;
                aPShipmentViewModel.APCustomInfo = aPCustomInfo;
                aPShipmentViewModel.APCustomInfoDetails = aPCustomInfoDetail;
                return View(aPShipmentViewModel);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(APShipmentViewModel APShipmentVM, IFormFile File, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            if (APShipmentVM.APShipment.Id == 0)
            {
                APShipmentVM.APShipment.CreatedBy = userId;
                APShipmentVM.APShipment.CreatedDate = DateTime.UtcNow;
                APShipmentVM.APShipment.CompanyId = companyId;
                APShipmentVM.APShipment.Resp_Id = resp_Id;
                APShipmentVM.APShipment.IsActive = true;
                var LCNo = APShipmentVM.APShipment.LCNo;
                APShipmentVM.APShipment.Attachment = await UploadFile(File);
                var result = _dbContext.APShipment.Where(x => x.IsActive).ToList();
                var foundCustomInfo = _dbContext.APCustomInfo.Where(x => x.IsActive).ToList();
                if (result.Count > 0)
                {
                    var ShipmentNo = _dbContext.APShipment.Select(x => x.ShipmentNo).Max();
                    int Shipmentno = Convert.ToInt32(ShipmentNo) + 1;
                    //ViewBag.Id = _dbContext.APShipment.Select(x => x.ShipmentNo).Max() + 1;
                    APShipmentVM.APShipment.ShipmentNo= Shipmentno;
                }
                else
                {
                    ViewBag.Id = 1;
                }
                await _APShipmentRepository.CreateAsync(APShipmentVM.APShipment);
                var LC = _dbContext.APLC.Where(x => x.Id==LCNo).FirstOrDefault();
                //if(LC!=null)
                //    LC.IsShipment = true;
                //_dbContext.Update(LC);
                if (APShipmentVM.APCustomInfo.Id == 0)
                {
                    APShipmentVM.APCustomInfo.CreatedBy = userId;
                    APShipmentVM.APCustomInfo.CreatedDate = DateTime.UtcNow;
                    APShipmentVM.APCustomInfo.CompanyId = companyId;
                    APShipmentVM.APCustomInfo.Resp_Id = resp_Id;
                    APShipmentVM.APCustomInfo.IsActive = true;
                    APShipmentVM.APCustomInfo.ShipmentId = APShipmentVM.APShipment.Id;
                    await _APCustomInfoRepository.CreateAsync(APShipmentVM.APCustomInfo);
                }
                List<APShipmentDetail> aPShipmentDetailsLsit = new List<APShipmentDetail>();
                for (int i = 0; i < collection["ItemCde"].Count; i++)
                {
                    var shipmentDetail = new APShipmentDetail();
                    var poDetails = new APPurchaseOrderItem();

                    shipmentDetail.APShipmentId = APShipmentVM.APShipment.Id;
                    shipmentDetail.ItemCode = Convert.ToString(collection["ItemCde"][i]);
                    shipmentDetail.Origin = Convert.ToInt32(collection["OriginId"][i]);
                    shipmentDetail.PkrValue = Convert.ToDecimal(collection["Value"][i]);
                    shipmentDetail.POQty = Convert.ToDecimal(collection["pOQty"][i]);
                    shipmentDetail.ShippedQty = Convert.ToDecimal(collection["ShipedQty"][i]);
                    shipmentDetail.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    shipmentDetail.Remarks = Convert.ToString(collection["Remarks"][i]);
                    shipmentDetail.UOM = Convert.ToInt32(collection["UOMId"][i]);
                    shipmentDetail.HSCode = Convert.ToString(collection["HSCode"][i]);
                    shipmentDetail.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                    shipmentDetail.Description = Convert.ToString(collection["Description"][i]);
                    shipmentDetail.Category = Convert.ToInt32(collection["CategoryId"][i]);
                    shipmentDetail.PrDetailId = Convert.ToInt32(collection["PrDetailId"][i]);
                    shipmentDetail.PoDetailId = Convert.ToInt32(collection["PoDetailId"][i]);
                    shipmentDetail.ItemId = Convert.ToInt32(collection["ItemId"][i]);

                    poDetails = _dbContext.APPurchaseOrderItems.Where(x => x.Id == Convert.ToInt32(collection["PoDetailId"][i])).FirstOrDefault();
                    poDetails.ShipmentBalc = Convert.ToInt32(collection["balance"][i]) - Convert.ToDecimal(collection["ShipedQty"][i]);
                    poDetails.ShipmentRcd = poDetails.ShipmentRcd + Convert.ToDecimal(collection["ShipedQty"][i]);

                    if (poDetails.ShipmentBalc == 0) {
                        LC.IsShipment = true;
                    }

                    _dbContext.Add(shipmentDetail);
                    _dbContext.Update(poDetails);
                }
                List<APGRNExpense> expenses = new List<APGRNExpense>();
                APGRNExpense[] Expense = JsonConvert.DeserializeObject<APGRNExpense[]>(collection["expenseDetails"]);
                foreach (var item in Expense)
                {
                    APGRNExpense exp = new APGRNExpense();
                    exp.CreatedBy = userId;
                    exp.CreatedDate = DateTime.UtcNow;
                    exp.CompanyId = companyId;
                    exp.LCId = APShipmentVM.APShipment.LCNo;
                    exp.AccountName = item.AccountName;
                    exp.GLCode = item.GLCode;
                    exp.Remarks = item.Remarks;
                    exp.ExpenseAmount = item.ExpenseAmount;
                    exp.ExpenseFavour = item.ExpenseFavour;
                    exp.ExpenseDate = Convert.ToDateTime(item.ExpenseDate);
                    expenses.Add(exp);
                }
                _dbContext.AddRange(expenses);
                _dbContext.SaveChanges();
                List<APCustomInfoDetails> customs = new List<APCustomInfoDetails>();
                APCustomInfoDetails[] Custom = JsonConvert.DeserializeObject<APCustomInfoDetails[]>(collection["customDetails"]);
                foreach (var item in Custom)
                {
                    APCustomInfoDetails cus = new APCustomInfoDetails();
                    cus.PDCNo = item.PDCNo;
                    cus.Remarks = item.Remarks;
                    cus.Status = item.Status;
                    cus.PDCDate = item.PDCDate;
                    cus.PDCAmount = item.PDCAmount;
                    cus.CustomInfo_Id = APShipmentVM.APCustomInfo.Id;
                    cus.PDCBank_Id = item.PDCBank_Id;
                    customs.Add(cus);
                }
                _dbContext.AddRange(customs);

                _dbContext.SaveChanges();
                //await _APShipmentDetailsRepository.CreateRangeAsync(aPShipmentDetailsLsit);
                TempData["error"] = "false";
                TempData["message"] = "Shipment has been created successfully.";
            }
            else
            {
                APShipment aPShipment = new APShipment();
                aPShipment = _APShipmentRepository.Get(x => x.Id == APShipmentVM.APShipment.Id).FirstOrDefault();
                //aPShipment.LCNo = APShipmentVM.APShipment.LCNo;
                aPShipment.ShipmentAgent = APShipmentVM.APShipment.ShipmentAgent;
                aPShipment.ShipmentDate = APShipmentVM.APShipment.ShipmentDate;
                aPShipment.ShipmentNo = APShipmentVM.APShipment.ShipmentNo;
                aPShipment.Terminal = APShipmentVM.APShipment.Terminal;
                aPShipment.TransporterName = APShipmentVM.APShipment.TransporterName;
                aPShipment.UpdatedBy = userId;
                aPShipment.UpdatedDate = DateTime.Now;
                aPShipment.Vendor = APShipmentVM.APShipment.Vendor;
                aPShipment.Weight = APShipmentVM.APShipment.Weight;
                aPShipment.MaturityDate = APShipmentVM.APShipment.MaturityDate;
                aPShipment.CBM = APShipmentVM.APShipment.CBM;
                aPShipment.ContainerNo = APShipmentVM.APShipment.ContainerNo;
                aPShipment.Acceptance = APShipmentVM.APShipment.Acceptance;
                aPShipment.BOLDate = APShipmentVM.APShipment.BOLDate;
                aPShipment.BOLNo = APShipmentVM.APShipment.BOLNo;
                aPShipment.BOLType = APShipmentVM.APShipment.BOLType;
                aPShipment.BuiltyDate = APShipmentVM.APShipment.BuiltyDate;
                aPShipment.BuiltyNo = APShipmentVM.APShipment.BuiltyNo;
                aPShipment.Currency = APShipmentVM.APShipment.Currency;
                aPShipment.ExchangeRate = APShipmentVM.APShipment.ExchangeRate;
                aPShipment.Attachment = await UploadFile(File);
                await _APShipmentRepository.UpdateAsync(aPShipment);
                APCustomInfo aPCustomInfo = new APCustomInfo();
                aPCustomInfo = _dbContext.APCustomInfo.FirstOrDefault(x => x.ShipmentId == aPShipment.Id);
                aPCustomInfo.CustomNo = APShipmentVM.APCustomInfo.CustomNo;
                aPCustomInfo.GDNo = APShipmentVM.APCustomInfo.CustomNo;
                aPCustomInfo.GDDate = APShipmentVM.APCustomInfo.GDDate;
                aPCustomInfo.IGMNo = APShipmentVM.APCustomInfo.CustomNo;
                aPCustomInfo.IGMDate = APShipmentVM.APCustomInfo.IGMDate;
                aPCustomInfo.ClearingAgent_Id = APShipmentVM.APCustomInfo.ClearingAgent_Id;
                aPCustomInfo.SROBenefit_Id = APShipmentVM.APCustomInfo.SROBenefit_Id;
                aPCustomInfo.Remarks = APShipmentVM.APCustomInfo.Remarks;
                aPCustomInfo.AssetValue = APShipmentVM.APCustomInfo.AssetValue;
                aPCustomInfo.IsActive = true;
                aPCustomInfo.UpdatedBy = userId;
                aPCustomInfo.UpdatedDate = DateTime.UtcNow;
                aPCustomInfo.Resp_Id = resp_Id;
                aPCustomInfo.CompanyId = companyId;
                await _APCustomInfoRepository.UpdateAsync(aPCustomInfo);
                var rownber = collection["id"].Count;
                var existing_detail = _APShipmentDetailsRepository.Get(x => x.APShipmentId == APShipmentVM.APShipment.Id).ToList();
                List<int> myList = new List<int>();
                for (int i = 0; i < rownber; i++)
                {
                    int Id = Convert.ToInt32(collection["id"][i]);
                    myList.Add(Id);
                }
                if (myList.Count > 0)
                {
                    for(int j = 0; j < rownber; j++) {
                        foreach (var model in existing_detail)
                        {
                            bool result = myList.Exists(s => s == model.Id);
                            if (!result)
                            {
                                await _APShipmentDetailsRepository.DeleteAsync(model);
                            }
                            if(model.Id==Convert.ToInt32(collection["id"][j]))
                            {
                                model.ItemCode = Convert.ToString(collection["itemCode"][j]);
                                model.Origin = Convert.ToInt32(collection["originId"][j]);
                                model.PkrValue = Convert.ToDecimal(collection["value"][j]);
                                model.POQty = Convert.ToDecimal(collection["pOQty"][j]);
                                model.ShippedQty = Convert.ToDecimal(collection["shipedQty"][j]);
                                model.Rate = Convert.ToDecimal(collection["rate"][j]);
                                model.Remarks = Convert.ToString(collection["remarks"][j]);
                                model.UOM = Convert.ToInt32(collection["uOMId"][j]);
                                model.HSCode = Convert.ToString(collection["hSCode"][j]);
                                model.FCValue = Convert.ToDecimal(collection["fCValue"][j]);
                                model.Description = Convert.ToString(collection["description"][j]);
                                model.Category = Convert.ToInt32(collection["categoryId"][j]);
                                model.ItemId = Convert.ToInt32(collection["itemId"][j]);
                                await _APShipmentDetailsRepository.UpdateAsync(model);
                            }
                        }
                    }
                }
                if (rownber == 0)
                {
                    foreach (var model in existing_detail)
                    {
                        await _APShipmentDetailsRepository.DeleteAsync(model);
                    }
                }
                var LCNo = APShipmentVM.APShipment.LCNo;
                var LC = _dbContext.APLC.Where(x => x.Id == LCNo).FirstOrDefault();

                for (int i = 0; i < collection["ItemCde"].Count; i++)
                {
                    var shipmentDetail = new APShipmentDetail();
                    var poDetails = new APPurchaseOrderItem();

                    shipmentDetail.APShipmentId = APShipmentVM.APShipment.Id;
                    shipmentDetail.ItemCode = Convert.ToString(collection["ItemCde"][i]);
                    shipmentDetail.Origin = Convert.ToInt32(collection["OriginId"][i]);
                    shipmentDetail.PkrValue = Convert.ToDecimal(collection["Value"][i]);
                    shipmentDetail.POQty = Convert.ToDecimal(collection["POQty"][i]);
                    shipmentDetail.ShippedQty = Convert.ToDecimal(collection["ShipedQty"][i]);
                    shipmentDetail.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    shipmentDetail.Remarks = Convert.ToString(collection["Remarks"][i]);
                    shipmentDetail.UOM = Convert.ToInt32(collection["UOMId"][i]);
                    shipmentDetail.HSCode = Convert.ToString(collection["HSCode"][i]);
                    shipmentDetail.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                    shipmentDetail.Description = Convert.ToString(collection["Description"][i]);
                    shipmentDetail.Category = Convert.ToInt32(collection["CategoryId"][i]);
                    shipmentDetail.PrDetailId = Convert.ToInt32(collection["PrDetailId"][i]);
                    shipmentDetail.PoDetailId = Convert.ToInt32(collection["PoDetailId"][i]);
                    shipmentDetail.ItemId = Convert.ToInt32(collection["ItemId"][i]);

                    poDetails = _dbContext.APPurchaseOrderItems.Where(x => x.Id == Convert.ToInt32(collection["PoDetailId"][i])).FirstOrDefault();
                    poDetails.ShipmentBalc = Convert.ToInt32(collection["balance"][i]) - Convert.ToDecimal(collection["ShipedQty"][i]);
                    poDetails.ShipmentRcd = poDetails.ShipmentRcd + Convert.ToDecimal(collection["ShipedQty"][i]);

                    if (poDetails.ShipmentBalc == 0)
                    {
                        LC.IsShipment = true;
                    }

                    _dbContext.Add(shipmentDetail);
                    _dbContext.Update(poDetails);

                    await _APShipmentDetailsRepository.CreateAsync(shipmentDetail);
                }
                List<APGRNExpense> expenses = new List<APGRNExpense>();
                List<APGRNExpense> expensesUpdate = new List<APGRNExpense>();
                var ExpenseList = _dbContext.APGRNExpense.Where(x => x.LCId == APShipmentVM.APShipment.LCNo).ToList();
                List<APGRNExpense> aPGRNExpenses = new List<APGRNExpense>();
                APGRNExpense[] Expense = JsonConvert.DeserializeObject<APGRNExpense[]>(collection["expenseDetails"]);
                foreach (var itm in Expense)
                {
                    expensesUpdate.Add(itm);
                }
                foreach (var item in ExpenseList)
                {
                    bool isExis = expensesUpdate.Exists(x => x.Id == item.Id);
                    if (!isExis)
                    {
                        var delete = _dbContext.APGRNExpense.Where(x => x.Id == item.Id).FirstOrDefault();
                        _dbContext.Remove(delete);
                        _dbContext.SaveChanges();
                    }
                }
                foreach (var item in Expense)
                {
                    if (item.Id == 0)
                    {
                        APGRNExpense exp = new APGRNExpense();
                        exp.CreatedBy = userId;
                        exp.CreatedDate = DateTime.UtcNow;
                        exp.CompanyId = companyId;
                        exp.LCId = APShipmentVM.APShipment.LCNo;
                        exp.AccountName = item.AccountName;
                        exp.GLCode = item.GLCode;
                        exp.Remarks = item.Remarks;
                        exp.ExpenseAmount = item.ExpenseAmount;
                        exp.ExpenseDate = Convert.ToDateTime(item.ExpenseDate);
                        exp.ExpenseFavour = item.ExpenseFavour;
                        expenses.Add(exp);
                    }
                    else
                    {
                        APGRNExpense exp = _dbContext.APGRNExpense.Where(x => x.Id == item.Id).FirstOrDefault();
                        exp.UpdatedBy = userId;
                        exp.UpdatedDate = DateTime.UtcNow;
                        exp.CompanyId = companyId;
                        exp.LCId = APShipmentVM.APShipment.LCNo;
                        exp.AccountName = item.AccountName;
                        exp.GLCode = item.GLCode;
                        exp.Remarks = item.Remarks;
                        exp.ExpenseAmount = item.ExpenseAmount;
                        exp.ExpenseDate = Convert.ToDateTime(item.ExpenseDate);
                        exp.ExpenseFavour = item.ExpenseFavour;
                        _dbContext.Update(exp);
                        _dbContext.SaveChanges();
                    }
                }
                _dbContext.AddRange(expenses);
                APCustomInfoDetails[] Custom = JsonConvert.DeserializeObject<APCustomInfoDetails[]>(collection["customDetails"]);
                List<APCustomInfoDetails> CustomsList = new List<APCustomInfoDetails>();
                var CustomsExisting = _APCustomInfoDetailRepository.Get(x => x.CustomInfo_Id == APShipmentVM.APCustomInfo.Id).ToList();
                foreach (var x in Custom)
                {
                    CustomsList.Add(x);
                }

                foreach (var cust in CustomsExisting)
                {
                    bool result = CustomsList.Exists(x=>x.Id== cust.Id);
                    if (!result)
                    {
                        await _APCustomInfoDetailRepository.DeleteAsync(cust);
                    }
                }
                foreach (var item in Custom)
                {
                    APCustomInfoDetails cus = new APCustomInfoDetails();
                    if (item.Id == 0)
                    {
                        cus.PDCNo = item.PDCNo;
                        cus.Remarks = item.Remarks;
                        cus.Status = item.Status;
                        cus.PDCDate = item.PDCDate;
                        cus.PDCAmount = item.PDCAmount;
                        cus.CustomInfo_Id = APShipmentVM.APCustomInfo.Id;
                        cus.PDCBank_Id = item.PDCBank_Id;
                        await _APCustomInfoDetailRepository.CreateAsync(cus);
                    }
                    else
                    {
                        cus = _APCustomInfoDetailRepository.Get(x => x.Id == item.Id).FirstOrDefault();

                        cus.PDCNo = item.PDCNo;
                        cus.Remarks = item.Remarks;
                        cus.Status = item.Status;
                        cus.PDCDate = item.PDCDate;
                        cus.PDCAmount = item.PDCAmount;
                        cus.CustomInfo_Id = APShipmentVM.APCustomInfo.Id;
                        cus.PDCBank_Id = item.PDCBank_Id;
                        await _APCustomInfoDetailRepository.UpdateAsync(cus);
                    }
                }

            }
            
            return RedirectToAction("List", "Shipment");
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
        public IActionResult List()
        {
            return View();
            
        }
        public IActionResult GetShipment()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var customerData = (from tempcustomer in _dbContext.APShipment.Where(x => x.IsDeleted == false) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.ShipmentNo.ToString().Contains(searchValue)
                                                    
                                                    || m.ShipmentDate.ToShortDateString().Contains(searchValue)
                                                    || m.Vendor.ToString().Contains(searchValue)
                                                    || m.ShipmentAgent.ToString().Contains(searchValue)
                                                    || _dbContext.Users.FirstOrDefault(x => x.Id == m.CreatedBy).UserName.Contains(searchValue)
                                                  );

                }
                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();

                List<APShipmentViewModel> Details = new List<APShipmentViewModel>();
                foreach (var grp in data)
                {
                    APShipmentViewModel shipment = new APShipmentViewModel();
                    shipment.UserName = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).UserName;
                    shipment.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(grp.Vendor)).Name;
                    shipment.ShipDate= grp.ShipmentDate.ToString(Helpers.CommonHelper.DateFormat);
                    shipment.POId = _dbContext.APLC.FirstOrDefault(x => x.Id == grp.LCNo).POId;
                    shipment.PONo = _dbContext.APPurchaseOrders.FirstOrDefault(x => x.Id == shipment.POId).PONo;
                    shipment.Agent = (from c in _dbContext.AppCompanyConfigs where c.Id == Convert.ToInt32(grp.ShipmentAgent) select c.ConfigValue).FirstOrDefault();
                    shipment.APShipment = grp;
                    shipment.APShipment.LCNo = _dbContext.APLC.FirstOrDefault(x => x.Id == grp.LCNo).TransctionNo;
                    Details.Add(shipment);
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
            if (id != 0)
            {
                APShipment aPShipmentDetail = _APShipmentRepository.Get(x => x.Id == id).FirstOrDefault();
                await _APShipmentRepository.DeleteAsync(aPShipmentDetail);
                var Detail = _APShipmentDetailsRepository.Get(x => x.Id == id).ToList();
                if (!ReferenceEquals(Detail, null))
                    await _APShipmentDetailsRepository.DeleteRangeAsync(Detail);
                TempData["error"] = "false";
                TempData["message"] = "Purchase Requisition has been deleted successfully.";
            }
            return RedirectToAction("List");
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APShipment aPShipment = _APShipmentRepository.Get(x => x.Id == id).FirstOrDefault();
            aPShipment.ApprovedBy = _userId;
            aPShipment.ApprovedDate = DateTime.UtcNow;
            aPShipment.IsApproved = true;
            await _APShipmentRepository.UpdateAsync(aPShipment);
            TempData["error"] = "false";
            TempData["message"] = "Purchase Requisition has been approved successfully.";
            return RedirectToAction("List");
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            bool check = _dbContext.APShipment.Any(x => x.Id == id && x.IsActive == true && x.IsDeleted == false);
            if (check)
            {
                APShipment aPShipment = _APShipmentRepository.Get(x => x.Id == id).FirstOrDefault();
                aPShipment.UnApprovedBy = _userId;
                aPShipment.UnApprovedDate = DateTime.UtcNow;
                aPShipment.IsApproved = false;
                await _APShipmentRepository.UpdateAsync(aPShipment);
                TempData["error"] = "false";
                TempData["message"] = "Purchase Requisition has been UnApproved sucessfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("List");
        }
        [HttpGet]
        public IActionResult GetPO(int id)
        {
            var modl = (from p in _dbContext.APLC where p.Id == id select p).FirstOrDefault();
            var currency = (from p in _dbContext.APPurchaseOrders where p.Id == modl.POId select p).FirstOrDefault();
            var model = (from p in _dbContext.APPurchaseOrderItems where p.POId == modl.POId select p).ToList();
            List<ShipmentVM> shipmentVMList = new List<ShipmentVM>();
            foreach(var detail in model)
            {
                ShipmentVM shipmentVM = new ShipmentVM();
                shipmentVM.PoDetailId = detail.Id;
                shipmentVM.PrDetailId = detail.PrDetailId;
                shipmentVM.ItemId = detail.ItemId;
                shipmentVM.CategoryId = detail.Category;
                shipmentVM.Category = (from c in _dbContext.InvItemCategories where c.Id== detail.Category select c.Name).FirstOrDefault();
                shipmentVM.Description = detail.ItemDescription;
                shipmentVM.FCValue = detail.FCValue;
                shipmentVM.Value = detail.Value;
                shipmentVM.Rate = detail.Rate;
                shipmentVM.POQty = detail.Qty;
                shipmentVM.UOMId = Convert.ToInt32(detail.UOM);
                shipmentVM.UOM = (from u in _dbContext.AppCompanyConfigs where u.Id == Convert.ToInt32(detail.UOM) select u.ConfigValue).FirstOrDefault();
                shipmentVM.OriginId = detail.Origin;
                shipmentVM.Origin = (from u in _dbContext.AppCountries where u.Id==detail.Origin select u.Name).FirstOrDefault();
                shipmentVM.ItemCode = detail.ItemCode;
                shipmentVM.HSCode = detail.HSCode;
                shipmentVM.Balance = detail.ShipmentBalc;
                shipmentVM.Currency =  modl.CurrencyId;
                shipmentVM.Vendor =Convert.ToString( modl.VendorId);
                shipmentVM.CurrencyExchangeRate = currency.CurrencyExchangeRate;
                shipmentVMList.Add(shipmentVM);
            }
            return Ok(shipmentVMList);
        }
    }
}
