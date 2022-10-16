using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Areas.Inventory;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public SupplierController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Suppliers";
            return View();
        }
        [HttpGet]
        public IActionResult Create(int? id)
        {
            var comapnyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.AppCountries = new SelectList(_dbContext.AppCountries.OrderBy(c=>c.Name).ToList(), "Id", "Name");
            ViewBag.BussinessTypes = new SelectList(_dbContext.AppCompanyConfigs.OrderBy(c=>c.ConfigValue).Where(x=>x.BaseId==1089).ToList(), "Id", "ConfigValue");
            InventoryHelper helper = new InventoryHelper(_dbContext, HttpContext.Session.GetInt32("CompanyId").Value);
             ViewBag.listOfItemCategories = helper.GetCategoriesSelectLists();
           
            ViewBag.listOfLevelCategories = new SelectList(
                _dbContext.InvItemCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.ParentId == null).ToList()
                , "Id", "Name");
            if (id==null)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Supplier";
                ViewBag.Id = null;

                APSupplierVM aPSupplierVM = new APSupplierVM();
                aPSupplierVM.APSupplier = new APSupplier();
                aPSupplierVM.ARContactPerson = new List<ARContactPerson>();
                aPSupplierVM.ARShippingDetail = new List<ARShippingDetail>();
            
                return View(aPSupplierVM);
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Supplier";

                APSupplierVM aPSupplierVM = new APSupplierVM();
                APSupplier supplier = _dbContext.APSuppliers.Find(id);
                aPSupplierVM.APSupplier = supplier;

                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == supplier.Country).ToList(), "Id", "Name");
                ViewBag.Id = _dbContext.APSuppliers.Find(id).AccountId;
                ViewBag.ItemCategory = (from c in _dbContext.ARSuplierItemsGroup where c.ApSuplierId== id select c).ToList();
                aPSupplierVM.ARShippingDetail = _dbContext.ARShippingDetail.Where(a => a.IsDeleted == false && a.SupplierId == id).ToList();
                aPSupplierVM.ARContactPerson = _dbContext.ARContactPerson.Where(a => a.IsDeleted == false && a.SupplierId == id).ToList();
              
                return View(aPSupplierVM);
            }
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var comapnyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.AppCountries = new SelectList(_dbContext.AppCountries.OrderBy(c => c.Name).ToList(), "Id", "Name");
            ViewBag.BussinessTypes = new SelectList(_dbContext.AppCompanyConfigs.OrderBy(c => c.ConfigValue).Where(x => x.BaseId == 1089).ToList(), "Id", "ConfigValue");
            InventoryHelper helper = new InventoryHelper(_dbContext, HttpContext.Session.GetInt32("CompanyId").Value);
            ViewBag.listOfItemCategories = helper.GetCategoriesSelectLists();

            ViewBag.listOfLevelCategories = new SelectList(
                _dbContext.InvItemCategories.Where(x => x.IsActive == true && x.IsDeleted == false && x.CompanyId == comapnyId && x.ParentId == null).ToList()
                , "Id", "Name");
           
                

                APSupplierVM aPSupplierVM = new APSupplierVM();
                APSupplier supplier = _dbContext.APSuppliers.Find(id);
                aPSupplierVM.APSupplier = supplier;

                ViewBag.AppCity = _dbContext.AppCities.Where(c => c.CountryId == supplier.Country && c.Id == supplier.City).Select(x=>x.Name).FirstOrDefault();
                ViewBag.AppCountary = _dbContext.AppCountries.Where(c => c.Id == supplier.Country).Select(x=>x.Name).FirstOrDefault();
                ViewBag.Id = _dbContext.APSuppliers.Find(id).AccountId;
                ViewBag.ItemCategory = (from c in _dbContext.ARSuplierItemsGroup where c.ApSuplierId == id select c).ToList();
                aPSupplierVM.ARShippingDetail = _dbContext.ARShippingDetail.Where(a => a.IsDeleted == false && a.SupplierId == id).ToList();
                aPSupplierVM.ARContactPerson = _dbContext.ARContactPerson.Where(a => a.IsDeleted == false && a.SupplierId == id).ToList();
            
                return View(aPSupplierVM);
            
        }

        [HttpPost]
        public async Task<IActionResult> Create(APSupplierVM supplier,IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            //ARContactPerson[] contactperson = JsonConvert.DeserializeObject<ARContactPerson[]>(collection["ContactPerson1"]);
            //ARShippingDetail[] shippingDetails = JsonConvert.DeserializeObject<ARShippingDetail[]>(collection["ShippingDetail"]);
            if (supplier.APSupplier.Id == 0)
            {
                try
                {

                    supplier.APSupplier.CreatedBy = userId;
                    supplier.APSupplier.CreatedDate = DateTime.UtcNow;
                    supplier.APSupplier.CompanyId = companyId;
                    supplier.APSupplier.IsActive = true;
                    supplier.APSupplier.BussinessType = Convert.ToInt32(collection["BussinessType"]);
                    supplier.APSupplier.Country = Convert.ToInt32(collection["Country"]);
                    supplier.APSupplier.City = Convert.ToInt32(collection["City"]); 
                    supplier.APSupplier.AccountId = Convert.ToInt32(collection["AccountId"]);
                    //if (supplier.APSupplier.AccountId == 0)
                    //{
                    //    var configvalue = new ConfigValues(_dbContext);
                    //    int supplierControlAccount = configvalue.CreateGLAccountByParentCode("Supplier", supplier.APSupplier.Name, companyId, userId);
                    //    supplier.APSupplier.AccountId = supplierControlAccount;
                    //}
                    await _dbContext.APSuppliers.AddAsync(supplier.APSupplier);
                    await _dbContext.SaveChangesAsync();

                    List<ARContactPerson> Details = new List<ARContactPerson>();
                    List<ARShippingDetail> SDetails = new List<ARShippingDetail>();
                    List<ARSuplierItemsGroup> CDetails = new List<ARSuplierItemsGroup>();
                    for (int i = 0; i < collection["id"].Count; i++)
                    {
                        ARContactPerson CPDetails = new ARContactPerson();
                        CPDetails.PersonName = Convert.ToString(collection["pname"][i]);
                        CPDetails.Designation = Convert.ToString(collection["designation"][i]);
                        CPDetails.PhoneNo = Convert.ToString(collection["phone"][i]);
                        CPDetails.Ext = Convert.ToString(collection["ext"][i]);
                        //CPDetails.CellNo = Convert.ToString(collection["cell"][i]);
                        CPDetails.Email = Convert.ToString(collection["email"][i]);
                        CPDetails.CustomerId = 0;
                        CPDetails.SupplierId = supplier.APSupplier.Id;
                        CPDetails.IsDeleted = false;
                        CPDetails.IsActive = true;
                        CPDetails.CreatedBy = userId;
                        CPDetails.CreatedDate = DateTime.Now;
                        CPDetails.Type = "Supplier";
                        Details.Add(CPDetails);
                    };
                    //for (int i = 0; i < collection["sid"].Count; i++)
                    //{
                    //    ARShippingDetail ShipDetails = new ARShippingDetail();
                    //    ShipDetails.Location = Convert.ToString(collection["slocation"][i]);
                    //    ShipDetails.Address = Convert.ToString(collection["saddress"][i]);
                    //    ShipDetails.PhoneNo = Convert.ToString(collection["sphone"][i]);
                    //    ShipDetails.CustomerId = 0;
                    //    ShipDetails.SupplierId = supplier.APSupplier.Id;
                    //    ShipDetails.IsDeleted = false;
                    //    ShipDetails.IsActive = true;
                    //    ShipDetails.CreatedBy = userId;
                    //    ShipDetails.CreatedDate = DateTime.Now;
                    //    ShipDetails.Type = "Supplier";
                    //    SDetails.Add(ShipDetails);
                    //};
                    for (int i = 0; i < collection["categoryId"].Count; i++)
                    {
                        ARSuplierItemsGroup aRSuplierItemsGroup = new ARSuplierItemsGroup();
                        aRSuplierItemsGroup.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                        aRSuplierItemsGroup.CategoryLevId = Convert.ToInt32(collection["categoryLevId"][i]);
                        aRSuplierItemsGroup.Type = "Supplier";
                        aRSuplierItemsGroup.ApSuplierId = supplier.APSupplier.Id;
                        CDetails.Add(aRSuplierItemsGroup);
                    }

                    await _dbContext.ARContactPerson.AddRangeAsync(Details);
                    //await _dbContext.ARShippingDetail.AddRangeAsync(SDetails);
                    await _dbContext.ARSuplierItemsGroup.AddRangeAsync(CDetails);
                    _dbContext.SaveChanges();
                    TempData["error"] = "false";
                    TempData["message"] = "Vendor has been created successfully.";
                }
                catch (Exception ex)
                {
                    //await _APIRNRepository.DeleteAsync(APIRNVM.APIRN);
                    //var DeleteList = _APIRNDetailsRepository.Get(x => x.IRNID == APIRNVM.APIRN.Id).ToList();
                    //await _APIRNDetailsRepository.DeleteRangeAsync(DeleteList);
                    //TempData["error"] = "true";
                    //TempData["message"] = "Went something wrong.";
                }
                //supplier.APSupplier.CompanyId = companyId;
                //supplier.APSupplier.CreatedBy = userId;
                //supplier.APSupplier.CreatedDate = DateTime.Now;
                ////if AccountId == 0 then we will create auto AccountId
                //if (supplier.APSupplier.AccountId == 0)
                //{
                //    var configvalue = new ConfigValues(_dbContext);
                //    int supplierControlAccount = configvalue.CreateGLAccountByParentCode("Supplier", supplier.APSupplier.Name, companyId, userId);
                //    supplier.APSupplier.AccountId = supplierControlAccount;
                //}
                //supplier.APSupplier.City = Convert.ToInt32(collection["City"]);
                //supplier.APSupplier.Country = Convert.ToInt32(collection["Country"]);

                //_dbContext.APSuppliers.Add(supplier.APSupplier);
                //await _dbContext.SaveChangesAsync();
                //TempData["error"] = "false";
                //TempData["message"] = "Supplier has been saved successfully.";

                //if (contactperson != null)
                //{
                //    foreach (var item in contactperson)
                //    {
                //        ARContactPerson model = new ARContactPerson();
                //        model.PersonName = item.PersonName;
                //        model.Designation = item.Designation;
                //        model.PhoneNo = item.PhoneNo;
                //        model.Ext = item.Ext;
                //        model.CellNo = item.CellNo;
                //        model.Email = item.Email;
                //        model.CustomerId = 0;
                //        model.SupplierId = supplier.APSupplier.Id;
                //        model.IsDeleted = false;
                //        model.IsActive = true;
                //        model.CreatedBy = userId;
                //        model.CreatedDate = DateTime.Now;
                //        model.Type = "Supplier";
                //        _dbContext.ARContactPerson.Add(model);
                //        await _dbContext.SaveChangesAsync();

                //    }
                //}
                //if (shippingDetails != null)
                //{
                //    foreach (var item in shippingDetails)
                //    {
                //        ARShippingDetail model = new ARShippingDetail();
                //        model.Location = item.Location;
                //        model.Address = item.Address;
                //        model.PhoneNo = item.PhoneNo;

                //        model.CustomerId = 0;
                //        model.SupplierId = supplier.APSupplier.Id;
                //        model.IsDeleted = false;
                //        model.IsActive = true;
                //        model.CreatedBy = userId;
                //        model.CreatedDate = DateTime.Now;
                //        model.Type = "Supplier";
                //        _dbContext.ARShippingDetail.Add(model);
                //        await _dbContext.SaveChangesAsync();

                //    }
                //}
                
            }

            else
            {
                //avoiding null insertion in model attributes
                var data = _dbContext.APSuppliers.Find(supplier.APSupplier.Id);
                data.Name = supplier.APSupplier.Name;
                data.Phone1 = supplier.APSupplier.Phone1;
                data.Phone2 = supplier.APSupplier.Phone2;
                data.GSTNo = supplier.APSupplier.GSTNo;
                data.NTNNo = supplier.APSupplier.NTNNo;
                //data.Email = supplier.APSupplier.Email;
                data.Web = supplier.APSupplier.Web;
                data.ContactPerson = supplier.APSupplier.ContactPerson;
                //data.Fax = supplier.APSupplier.Fax;
                data.Address = supplier.APSupplier.Address;
                data.Country = supplier.APSupplier.Country;
                data.City = supplier.APSupplier.City;
                data.IsActive = supplier.APSupplier.IsActive;
                data.UpdatedBy = userId;
                data.CompanyId = companyId;
                data.UpdatedDate = DateTime.Now;
                data.BussinessType = Convert.ToInt32(collection["BussinessType"]);
                data.City = Convert.ToInt32(collection["City"]);
                data.Country = Convert.ToInt32(collection["Country"]);
                data.AccountId = Convert.ToInt32(collection["AccountId"]);
                //if (supplier.APSupplier.AccountId == 0)
                //{
                //    var configvalue = new ConfigValues(_dbContext);
                //    int supplierControlAccount = configvalue.CreateGLAccountByParentCode("Supplier", supplier.APSupplier.Name, companyId, userId);
                //    data.AccountId = supplierControlAccount;
                //}
                data.Type = supplier.APSupplier.Type;
                data.Registered = supplier.APSupplier.Registered;
                data.Status = supplier.APSupplier.Status;
                data.RegistrationDate = supplier.APSupplier.RegistrationDate;
                data.ChamberMemebership = supplier.APSupplier.ChamberMemebership;
                var entry = _dbContext.APSuppliers.Update(data);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                TempData["error"] = "false";
                TempData["message"] = "Supplier has been updated successfully.";



                var removeShipping = _dbContext.ARShippingDetail.Where(u => u.SupplierId == supplier.APSupplier.Id).ToList();
                var removeContactPerson = _dbContext.ARContactPerson.Where(u => u.SupplierId == supplier.APSupplier.Id).ToList();
                var removeCategory = _dbContext.ARSuplierItemsGroup.Where(u => u.ApSuplierId == supplier.APSupplier.Id).ToList();
                if (removeShipping != null)
                {
                    foreach (var item in removeShipping)
                    {
                        _dbContext.ARShippingDetail.Remove(item);
                        _dbContext.SaveChanges();
                    }
                }
                if (removeContactPerson != null)
                {
                    foreach (var item in removeContactPerson)
                    {
                        _dbContext.ARContactPerson.Remove(item);
                        _dbContext.SaveChanges();
                    }
                }
                if (removeCategory != null)
                {
                    foreach (var item in removeCategory)
                    {
                        _dbContext.ARSuplierItemsGroup.Remove(item);
                        _dbContext.SaveChanges();
                    }
                }

                List<ARContactPerson> Details = new List<ARContactPerson>();
                List<ARShippingDetail> SDetails = new List<ARShippingDetail>();
                List<ARSuplierItemsGroup> CDetails = new List<ARSuplierItemsGroup>();

                for (int i = 0; i < collection["id"].Count; i++)
                {
                    ARContactPerson CPDetails = new ARContactPerson();
                    CPDetails.PersonName = Convert.ToString(collection["pname"][i]);
                    CPDetails.Designation = Convert.ToString(collection["designation"][i]);
                    CPDetails.PhoneNo = Convert.ToString(collection["phone"][i]);
                    CPDetails.Ext = Convert.ToString(collection["ext"][i]);
                    //CPDetails.CellNo = Convert.ToString(collection["cell"][i]);
                    CPDetails.Email = Convert.ToString(collection["email"][i]);
                    CPDetails.CustomerId = 0;
                    CPDetails.SupplierId = supplier.APSupplier.Id;
                    CPDetails.IsDeleted = false;
                    CPDetails.IsActive = true;
                    CPDetails.CreatedBy = userId;
                    CPDetails.CreatedDate = DateTime.Now;
                    CPDetails.Type = "Supplier";
                    Details.Add(CPDetails);
                };
                for (int i = 0; i < collection["sid"].Count; i++)
                {
                    ARShippingDetail ShipDetails = new ARShippingDetail();
                    ShipDetails.Location = Convert.ToString(collection["slocation"][i]);
                    ShipDetails.Address = Convert.ToString(collection["saddress"][i]);
                    ShipDetails.PhoneNo = Convert.ToString(collection["sphone"][i]);
                    ShipDetails.CustomerId = 0;
                    ShipDetails.SupplierId = supplier.APSupplier.Id;
                    ShipDetails.IsDeleted = false;
                    ShipDetails.IsActive = true;
                    ShipDetails.CreatedBy = userId;
                    ShipDetails.CreatedDate = DateTime.Now;
                    ShipDetails.Type = "Supplier";
                    SDetails.Add(ShipDetails);
                };
                int countr = collection["categoryId"].Count;
                for (int i = 0; i < countr; i++)
                {
                    ARSuplierItemsGroup aRSuplierItemsGroup = new ARSuplierItemsGroup();
                    aRSuplierItemsGroup.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                    aRSuplierItemsGroup.CategoryLevId = Convert.ToInt32(collection["categoryLevId"][i]);
                    aRSuplierItemsGroup.ApSuplierId = supplier.APSupplier.Id;
                    CDetails.Add(aRSuplierItemsGroup);

                }

                await _dbContext.ARContactPerson.AddRangeAsync(Details);
                await _dbContext.ARShippingDetail.AddRangeAsync(SDetails);
                await _dbContext.ARSuplierItemsGroup.AddRangeAsync(CDetails);
                _dbContext.SaveChanges();


                //if (contactperson != null)
                //{
                //    foreach (var item in contactperson)
                //    {
                //        ARContactPerson model = new ARContactPerson();
                //        model.PersonName = item.PersonName;
                //        model.Designation = item.Designation;
                //        model.PhoneNo = item.PhoneNo;
                //        model.Ext = item.Ext;
                //        model.CellNo = item.CellNo;
                //        model.Email = item.Email;
                //        model.CustomerId = 0;
                //        model.SupplierId = supplier.APSupplier.Id;
                //        model.IsDeleted = false;
                //        model.IsActive = true;
                //        model.CreatedBy = userId;
                //        model.CreatedDate = DateTime.Now;
                //        model.Type = "Supplier";
                //        _dbContext.ARContactPerson.Add(model);
                //        await _dbContext.SaveChangesAsync();

                //    }
                //}
                //if (shippingDetails != null)
                //{
                //    foreach (var item in shippingDetails)
                //    {
                //        ARShippingDetail model = new ARShippingDetail();
                //        model.Location = item.Location;
                //        model.Address = item.Address;
                //        model.PhoneNo = item.PhoneNo;

                //        model.CustomerId = 0;
                //        model.SupplierId = supplier.APSupplier.Id;
                //        model.IsDeleted = false;
                //        model.IsActive = true;
                //        model.CreatedBy = userId;
                //        model.CreatedDate = DateTime.Now;
                //        model.Type = "Supplier";
                //        _dbContext.ARShippingDetail.Add(model);
                //        await _dbContext.SaveChangesAsync();

                //    }
                //}
                //var rownber = collection["id"].Count;
                //var existingcaogory = (from c in _dbContext.ARSuplierItemsGroup where c.ApSuplierId == supplier.APSupplier.Id select c).ToList();
                //List<int> myList = new List<int>();
                //for (int i = 0; i < rownber; i++)
                //{
                //    int id = Convert.ToInt32(collection["id"][i]);
                //    myList.Add(id);
                //}
                //if (myList.Count > 0)
                //{
                //    foreach (var cat in existingcaogory)
                //    {
                //        bool result = myList.Exists(s => s == cat.ID);
                //        if (!result)
                //        {
                //            _dbContext.ARSuplierItemsGroup.Remove(cat);
                //            _dbContext.SaveChanges();
                //        }
                //    }
                //}
                //var itemDetails = collection["CategoryID"].Count;
                //for (int i = 0; i < itemDetails; i++)
                //{
                //    ARSuplierItemsGroup aRSuplierItemsGroup = new ARSuplierItemsGroup();
                //    aRSuplierItemsGroup.CategoryId = Convert.ToInt32(collection["CategoryID"][i]);
                //    aRSuplierItemsGroup.ApSuplierId = supplier.APSupplier.Id;
                //    _dbContext.ARSuplierItemsGroup.Add(aRSuplierItemsGroup);
                //    _dbContext.SaveChanges();
                //}

            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteSupplier(int Id)
        {

            var supplier = await _dbContext.APSuppliers.Where(n => n.Id == Id).FirstAsync();
            if (supplier != null)
            {
                supplier.IsActive = false;
                _dbContext.APSuppliers.Update(supplier);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetSupplier()
       {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;
                var responcibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x=>x.Resp_Id == resp_Id).Resp_Name;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var searchId = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchName = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchAccounts = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchAddress = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchPhone = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchApprovedBy = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                
                var SuppliersData = responcibility == "Yarn Purchase" ? 
                        (from Suppliers in _dbContext.APSuppliers.Include(x=>x.CreatedUser).Include(x=>x.ApprovalUser).Where(x => x.IsActive == true && x.Account.Code == "2.02.04.0003" && x.CompanyId == companyId).Include(a => a.Account) select Suppliers):
                        (from Suppliers in _dbContext.APSuppliers.Include(x => x.CreatedUser).Include(x => x.ApprovalUser).Where(x => x.IsActive == true && x.Account.Code != "2.02.04.0003" && x.CompanyId == companyId).Include(a => a.Account) select Suppliers);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    SuppliersData = SuppliersData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                SuppliersData = !string.IsNullOrEmpty(searchId) ? SuppliersData.Where(m => m.Id.ToString().Contains(searchId)) : SuppliersData;
                SuppliersData = !string.IsNullOrEmpty(searchName) ? SuppliersData.Where(m => m.Name.ToString().ToUpper().Contains(searchName.ToUpper())) : SuppliersData;
                SuppliersData = !string.IsNullOrEmpty(searchAccounts) ? SuppliersData.Where(m => (m.Account.Code.ToString().ToUpper() + " - " + m.Account.Name.ToString().ToUpper()).Contains(searchAccounts.ToUpper())) : SuppliersData;
                SuppliersData = !string.IsNullOrEmpty(searchAddress) ? SuppliersData.Where(m => m.Address != null ? m.Address.ToString().ToUpper().Contains(searchAddress.ToUpper()): false) : SuppliersData;
                SuppliersData = !string.IsNullOrEmpty(searchPhone) ? SuppliersData.Where(m => m.Phone1 != null ? m.Phone1.ToString().ToUpper().Contains(searchPhone.ToUpper()): false) : SuppliersData;
                SuppliersData = !string.IsNullOrEmpty(searchCreatedBy) ? SuppliersData.Where(m => m.CreatedUser.UserName.ToString().ToUpper().Contains(searchCreatedBy.ToUpper())) : SuppliersData;
                SuppliersData = !string.IsNullOrEmpty(searchApprovedBy) ? SuppliersData.Where(m => m.ApprovedBy != null ? m.ApprovalUser.UserName.ToString().ToUpper().Contains(searchApprovedBy.ToUpper()): false) : SuppliersData;
                recordsTotal = SuppliersData.Count();
                var data = SuppliersData.ToList();
                if (pageSize == -1)
                {
                    data = SuppliersData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = SuppliersData.Skip(skip).Take(pageSize).ToList();
                }
                List<APSupplierVM> Details = new List<APSupplierVM>();
                foreach (var grp in data)
                {
                    APSupplierVM aPSupplierVM = new APSupplierVM();
                    aPSupplierVM.GLAccount = grp.Account.Code + " - " + grp.Account.Name;
                    aPSupplierVM.APSupplier = grp;
                    aPSupplierVM.APSupplier.Approve = approve;
                    aPSupplierVM.APSupplier.Unapprove = unApprove;

                    Details.Add(aPSupplierVM);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APSupplier aPSupplier = _dbContext.APSuppliers.FirstOrDefault(x=>x.Id == id);
            if (aPSupplier != null)
            {
                aPSupplier.ApprovedBy = _userId;
                aPSupplier.ApprovedDate = DateTime.UtcNow;
                aPSupplier.IsApproved = true;
                _dbContext.APSuppliers.Update(aPSupplier);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Supplier has been approved successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            bool aPPurchaseOrder = _dbContext.APPurchaseOrders.Any(x => x.SupplierId.Equals(id) && x.IsDeleted != true && x.CompanyId == _companyId);
            if (!aPPurchaseOrder)
            {
                APSupplier aPSupplier = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == id);
                if (aPSupplier != null)
                {
                    aPSupplier.ApprovedBy = null;
                    aPSupplier.UnApprovedBy = _userId;
                    aPSupplier.UnApprovedDate = DateTime.UtcNow;
                    aPSupplier.IsApproved = false;
                    _dbContext.APSuppliers.Update(aPSupplier);
                    await _dbContext.SaveChangesAsync();
                    TempData["error"] = "false";
                    TempData["message"] = "Supplier has been unapproved successfully.";
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "true";
            TempData["message"] = "This Supplier is used in transactions.";
            return RedirectToAction(nameof(Index));
        }
    }
}