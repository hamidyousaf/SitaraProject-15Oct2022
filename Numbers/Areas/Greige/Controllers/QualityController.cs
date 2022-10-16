using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class QualityController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public QualityController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        //[HttpGet]
        public IActionResult Index(int Id)
        {
            GRQualityViewModel qualityViewModel = new GRQualityViewModel();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            qualityViewModel.GRCategoryLOV = new SelectList(_dbContext.GRCategory.Where(x => x.IsDeleted != true && x.Status == "Approved" ).ToList(), "Id", "Description");
            qualityViewModel.GRConstructionLOV = new SelectList(_dbContext.GRConstruction.Where(x => x.IsDeleted != true && x.Status == "Approved" ).ToList(), "Id", "Description");
            qualityViewModel.LoomTypeLOV = configValues.GetConfigValues("Greige", "Loom Type", companyId);
            if (Id != 0)
            {
                qualityViewModel.GRQuality = _dbContext.GRQuality.Where(x => x.Id == Id ).FirstOrDefault();

            }

            return View(qualityViewModel);
        }

       
        public IActionResult Details(int Id)
        {
            GRQualityViewModel qualityViewModel = new GRQualityViewModel();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            qualityViewModel.GRCategoryLOV = new SelectList(_dbContext.GRCategory.Where(x => x.IsDeleted != true && x.Status == "Approved" ).ToList(), "Id", "Description");
            qualityViewModel.GRConstructionLOV = new SelectList(_dbContext.GRConstruction.Where(x => x.IsDeleted != true && x.Status == "Approved" ).ToList(), "Id", "Description");
            qualityViewModel.LoomTypeLOV = configValues.GetConfigValues("Greige", "Loom Type", companyId);
            if (Id != 0)
            {
                qualityViewModel.GRQuality = _dbContext.GRQuality.Where(x => x.Id == Id ).FirstOrDefault();

            }

            return View(qualityViewModel);
        }





        [HttpPost]
        public async Task<IActionResult> Create(GRQualityViewModel viewmodel)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var items = _dbContext.InvItems.Where(x => x.IsDeleted == false).ToList();
            GRQuality quality = new GRQuality();
            if (viewmodel.GRQuality.Id == 0)
            {
                int TransactionNo = 1;
                var list = _dbContext.GRQuality.ToList();
                if (list.Count != 0)
                {
                    TransactionNo = list.Select(x => x.TransactionNo).Max() + 1;
                }
                quality.TransactionNo = TransactionNo;
                quality.TransactionDate = viewmodel.GRQuality.TransactionDate;
                quality.GRCategoryId = viewmodel.GRQuality.GRCategoryId;
                quality.GRConstructionId = viewmodel.GRQuality.GRConstructionId;
                quality.Width = viewmodel.GRQuality.Width;
                quality.LoomTypeId = viewmodel.GRQuality.LoomTypeId;

                var construction = _dbContext.GRConstruction.FirstOrDefault(x=>x.Id == viewmodel.GRQuality.GRConstructionId);
                var loomtype = _dbContext.AppCompanyConfigs.FirstOrDefault(x=>x.Id == viewmodel.GRQuality.LoomTypeId).ConfigValue;
                var description = string.Format("{0} {1} {2}", construction.Description, viewmodel.GRQuality.Width, loomtype);
                quality.Description = description;
                var warpCount = Convert.ToDouble(construction.Warp.Name.Substring(0, construction.Warp.Name.IndexOf("-")));
                var weftCount = Convert.ToDouble(construction.Weft.Name.Substring(0, construction.Weft.Name.IndexOf("-")));

                var warpWeightPerMeter = ((Convert.ToDouble(construction.Reed) * Convert.ToDouble(viewmodel.GRQuality.Width) * 1.0936) / warpCount / 20 / 40);
                var weftWeightPerMeter = ((Convert.ToDouble(construction.Pick) * Convert.ToDouble(viewmodel.GRQuality.Width) * 1.0936) / weftCount / 20 / 40);
                quality.GSM =Convert.ToDecimal( warpWeightPerMeter + weftWeightPerMeter);
                quality.IsActive = true;
                quality.IsDeleted = false;
                quality.CompanyId = companyId;
                quality.Resp_Id = resp_Id;
                quality.CreatedBy = userId;
                quality.CreatedDate = DateTime.Now;
                bool istrue = CheckQuality(quality.Description);
                if (!istrue)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Greige Quality already exist!";
                    return RedirectToAction("Index");
                }
                _dbContext.GRQuality.Add(quality);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Quality has been saved successfully.";
            }
            else
            {
                quality = _dbContext.GRQuality.Find(viewmodel.GRQuality.Id);
                quality.TransactionNo = viewmodel.GRQuality.TransactionNo;
                quality.TransactionDate = viewmodel.GRQuality.TransactionDate;
                quality.GRCategoryId = viewmodel.GRQuality.GRCategoryId;
                quality.GRConstructionId = viewmodel.GRQuality.GRConstructionId;
                quality.Width = viewmodel.GRQuality.Width;
                quality.LoomTypeId = viewmodel.GRQuality.LoomTypeId;

                var construction = _dbContext.GRConstruction.FirstOrDefault(x => x.Id == viewmodel.GRQuality.GRConstructionId);
                var loomtype = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == viewmodel.GRQuality.LoomTypeId).ConfigValue;
                var description = string.Format("{0} {1} {2}", construction.Description, viewmodel.GRQuality.Width, loomtype);
                quality.Description = description;
                var warpCount = Convert.ToDouble(construction.Warp.Name.Substring(0, construction.Warp.Name.IndexOf("-")));
                var weftCount = Convert.ToDouble(construction.Weft.Name.Substring(0, construction.Weft.Name.IndexOf("-")));

                var warpWeightPerMeter = ((Convert.ToDouble(construction.Reed) * Convert.ToDouble(viewmodel.GRQuality.Width) * 1.0936) / warpCount / 20 / 40);
                var weftWeightPerMeter = ((Convert.ToDouble(construction.Pick) * Convert.ToDouble(viewmodel.GRQuality.Width) * 1.0936) / weftCount / 20 / 40);
                quality.GSM = Convert.ToDecimal(warpWeightPerMeter + weftWeightPerMeter);
                quality.IsActive = true;
                quality.IsDeleted = false;
                quality.CompanyId = companyId;
                quality.Resp_Id = resp_Id;
                quality.CreatedBy = userId;
                quality.CreatedDate = DateTime.Now;
                bool istrue = CheckQuality(quality.Description);
                if (!istrue)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Greige Quality already exist!";
                    return RedirectToAction("Index");
                }
                _dbContext.GRQuality.Update(quality);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Quality has been Updated successfully.";

            }
            return RedirectToAction(nameof(Index));
        }

        public bool CheckQuality(string desc)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRQuality model = _dbContext.GRQuality.Where(x => x.Description == desc ).FirstOrDefault();

            if (model != null)
            {
                return false;

            }
            return true;
        }
        public IActionResult GetList()
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id ).FirstOrDefault().Approve;
            var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            //var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var searchTransactionNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
            var searchTransactionDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
            var searchDescription = Request.Form["columns[2][search][value]"].FirstOrDefault();
            var searchGSM = Request.Form["columns[3][search][value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var Data = _dbContext.GRQuality.Where(x => x.IsDeleted == false );
            if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
            {
                Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
            }

            Data = !string.IsNullOrEmpty(searchTransactionNo) ? Data.Where(m => m.TransactionNo.ToString().Contains(searchTransactionNo)) : Data;
            Data = !string.IsNullOrEmpty(searchTransactionDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransactionDate.ToUpper())) : Data;
            Data = !string.IsNullOrEmpty(searchDescription) ? Data.Where(m => m.Description.ToString().ToUpper().Contains(searchDescription.ToUpper())) : Data;
            Data = !string.IsNullOrEmpty(searchGSM) ? Data.Where(m => m.GSM.ToString().ToUpper().Contains(searchGSM.ToUpper())) : Data;

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

           // var data = Data.Skip(skip).Take(pageSize).ToList();

            List<GRQuality> details = new List<GRQuality>();
            foreach (var item in data)
            {
                var quality = new GRQuality();
                quality.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                quality.quality = item;
                quality.quality.Approve = approve;
                quality.quality.Unapprove = unApprove;
                details.Add(quality);

                //item.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = details.OrderByDescending(x => x.TransactionNo), };
            return Ok(jsonData);
        }

        


        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRQuality model = _dbContext.GRQuality.Include(x=>x.GRConstruction).Include(x=>x.GRCategory).Where(x=>x.Id==id ).FirstOrDefault();
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            if (model.ItemId == 0)
            {
                if (model.GRCategory.ItemCategoryId != 0 && model.GRConstruction.ItemCategoryId != 0)
                {

                    var itemModel = await CreateItem(model);
                    if (itemModel != null)
                    {
                        model.ItemId = itemModel.Id;
                    }
                    else
                    {
                        model.ItemId = 0;
                    }
                }
                else
                {
                    model.ItemId = 0;
                }

            }
            else
            {
                await UpdateItem(model);

            }
            _dbContext.GRQuality.Update(model);
           await _dbContext.SaveChangesAsync();
            TempData["error"] = "false";
            TempData["message"] = "Quality has been approved successfully.";
            return RedirectToAction("Index", "Quality");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRQuality model = _dbContext.GRQuality.Where(x => x.Id==id ).FirstOrDefault();


            var checkCatgryRfrnc = _dbContext.GRWeavingContracts.Where(x => x.GreigeQualityId == id || x.GreigeQualityLoomId == id ).ToList();
            var checkCatgryRfrnc2 = _dbContext.GRPurchaseContract.Where(x => x.PurchaseGRQualityId == id || x.ContractGRQualityId == id ).ToList();
            if (checkCatgryRfrnc.Count == 0)
            {
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRQuality.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Greige Quality has been UnApproved successfully.";

            }
            if (checkCatgryRfrnc.Count != 0)
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in Weaving contract..!";
            }
            if (checkCatgryRfrnc2.Count != 0)
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in Purchase contract..!";
            }

 
            return RedirectToAction("Index", "Quality");
        }
        public IActionResult Delete(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var quality = new GRQuality { Id = id};
            if (quality != null)
            {
                var record = _dbContext.GRQuality.Find(quality.Id);
                record.IsActive = false;
                record.IsDeleted = true;
                _dbContext.GRQuality.Update(record); 
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "Greige Quality has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }


        public async Task<InvItem> CreateItem(GRQuality model)
        {
            try
            {
                int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var result = _dbContext.InvItems.Where(x => x.IsActive).ToList();
                // model = _dbContext.GRQuality.Include(x=>x.).Where(x => x.Id == model.Id).FirstOrDefault();
                var itemCategory = _dbContext.InvItemCategories.Where(x => x.Id == model.GRConstruction.ItemCategoryId).FirstOrDefault();
                 var code = "";
                var barCode = "";
                var categoryCode = itemCategory.Code;
                var categoryBarCode = itemCategory.Code.Substring(0, 5);
                if (result.Count > 0)
                {
                      categoryCode = itemCategory.Code;
                      categoryBarCode = itemCategory.Code.Substring(0, 5) ;
                    string Code = _dbContext.InvItems.Select(x => x.Code).LastOrDefault();
                    string a = Code.Split('-')[1];
                    // string b = Code.Substring(a, 0);
                    int c = Convert.ToInt16(a) + 1;
                    string BarCode = _dbContext.InvItems.Select(x => x.Barcode).LastOrDefault().Split('-')[1];
                    //string e = Code.Substring(0, d);
                    int f = Convert.ToInt16(BarCode) + 1;
                    code = Convert.ToDouble(c).ToString("000000");
                    barCode = Convert.ToDouble(f).ToString("00000000000");

                }
                else
                {
                    //ViewBag.Code = "000001";
                    //ViewBag.Barcode = "00000000001";
                    code = "000001";
                    barCode = "00000000001";
                }
                var item = new InvItem();
                item.Barcode =string.Concat(categoryBarCode,"-",barCode) ;
                item.Code = string.Concat(categoryCode, " -", code);
                item.Name = model.Description;
                item.SalesRate = 0;
                item.PurchaseRate = 0;
                item.Unit = _dbContext.AppCompanyConfigs.Where(x=>x.BaseId==13 && x.ConfigValue== "GSM").FirstOrDefault().Id;
                item.InvItemAccountId = _dbContext.InvItemAccounts.Where(x => x.IsDeleted == false && x.Name == "Greige Account").FirstOrDefault().Id;
                item.CategoryId = model.GRConstruction.ItemCategoryId;
                item.GLConsumptionAccountId = 0;
                item.Description = model.Description;
                item.Width = Convert.ToString(model.Width);
                item.UOMWeight = _dbContext.AppCompanyConfigs.Where(x => x.BaseId == 13 && x.ConfigValue == "GSM").FirstOrDefault().Id;
                //Emtry data
                item.ManufacturedId = 0;
                item.StockAccountId =0;
                item.CustomerStockAccountId = 0;
                item.SaleAccountId = 0;
                item.CostofSaleAccountId = 0;
                item.ItemType = 0;
                item.DiscountPercentage = 0;
                item.ManufacturedCode = "";
                item.SerialNo = "";
                item.ModelNo = "";
                item.Length = "";
                item.Width = "";
                item.Radius = "";
                item.UOMHeight = 0;
                item.UOMLength = 0;
                item.UOMRadius = 0;
                item.UOMSize = 0;
                item.UOMWidth =0;
                item.Season = 0;
                item.FabricConstruction = model.GRConstructionId;
                item.Brand = "";
                item.IsSaleable = true;
                  item.IsPurchaseable = true;
                item.Height = "";
                item.IsExpired = false;
                item.IsLabTest = true;
                 item.IsLocation = "Multiple";
                item.LotNo = "";
                item.Make = "";
                item.Material = "";
                item.MaxStockLevel = 0;
                item.MinStockLevel = 0;
                item.ReorderStockLevel = 0;
                item.WareHouse = _dbContext.AppCompanyConfigs.Where(x =>x.IsDeleted==false & x.ConfigValue == "GD Greige").FirstOrDefault().Id;
                item.FabricConstruction = model.GRConstructionId;
                item.Weight = "";
                item.PackQty = 0;
                item.PackUnit =0;
                item.OrderType = "";
                item.Size = "";
                item.Material = "";
                item.Color = "";
                item.LocationIfTrue = 0;
                item.Model = "";

                item.IsActive = true;
                item.CompanyId = model.CompanyId;
                item.IsDeleted = false;
                item.CreatedBy = model.CreatedBy;
                item.CreatedDate = DateTime.Now;
                _dbContext.InvItems.Add(item);
                await _dbContext.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                var obj = new InvItem();
                return obj;
            }
        }
        [HttpPost]
        public async Task<InvItem> UpdateItem(GRQuality model)
        {
            try
            {
                int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var item = _dbContext.InvItems.Where(x => x.Id == model.ItemId).FirstOrDefault();
                item.Name = model.Description;
                item.CategoryId = model.GRConstruction.ItemCategoryId;
                item.Description = model.Description;
                item.Width = Convert.ToString(model.Width);
                item.FabricConstruction = model.GRConstructionId;
                //Emtry data

                item.UpdatedBy = model.CreatedBy;
                item.UpdatedDate = DateTime.Now;
                _dbContext.InvItems.Update(item);
                await _dbContext.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                var obj = new InvItem();
                return obj;
            }
        }

    }
}
