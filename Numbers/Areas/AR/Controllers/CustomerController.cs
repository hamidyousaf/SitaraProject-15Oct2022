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
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;
using Numbers.Entity.ViewModels;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly CustomerRepository _customerRepository;
        public CustomerController(NumbersDbContext context, CustomerRepository customerRepository)
        {
            _dbContext = context;
            _customerRepository = customerRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //ViewBag.NavbarHeading = "List of Customers";
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //List<ARCustomer> customer = new List<ARCustomer>();
            //customer = _dbContext.ARCustomers.Where(x=>x.CompanyId==companyId && x.IsActive==true).ToList();
            //return View(customer);
            var customers = new List<ARCustomer>();
           // var customers = await _dbContext.ARCustomers.Where(x => x.IsActive == true && x.IsDeleted == false).OrderByDescending(x => x.Id).ToListAsync();
            ViewBag.NavbarHeading = "List of Customers";
            return View(customers);
        }
        public IActionResult Create(int? id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            ARCustomer customer;
            ViewBag.AppCountries = new SelectList(_dbContext.AppCountries.OrderBy(c => c.Id), "Id", "Name");
            //ViewBag.SalesPersons = new ConfigValues(_dbContext).GetConfigValues("AP", "SalesPerson", Convert.ToInt32(companyId));
            ViewBag.CustomerCategory = new ConfigValues(_dbContext).GetConfigValues("AR", "CustomerCategory", Convert.ToInt32(companyId));
            ViewBag.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId));
            InventoryHelper helper = new InventoryHelper(_dbContext, HttpContext.Session.GetInt32("CompanyId").Value);
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2/* && x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()

                                                          select new
                                                          {
                                                              Id = ac.Id,
                                                              Name = ac.Code + " - " + ac.Name
                                                          }, "Id", "Name");
            ViewBag.Categories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                select new
                                                {
                                                    Id = ac.Id,
                                                    Name = ac.Code + " - " + ac.Name
                                                }, "Id", "Name");

            //ViewBag.listOfItemCategories = helper.GetCategoriesSelectLists();
            if (id != null)
            {
                customer = _dbContext.ARCustomers.Find(id);

                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == customer.CountryId).ToList(), "Id", "Name");
                ViewBag.ShippingCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == customer.ShippingCountry).ToList(), "Id", "Name");
                customer.ARShippingDetail = _dbContext.ARShippingDetail.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                customer.ARContactPerson = _dbContext.ARContactPerson.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                ViewBag.ItemCategory = (from c in _dbContext.ARSuplierItemsGroup where c.ARCustomerId == id select c).ToList();
                ViewBag.Categories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                    where !_dbContext.ARSuplierItemsGroup.Where(x => x.ARCustomerId == id).Any(s => ac.Id.ToString().Contains(s.CategoryId.ToString()))
                                                    select new
                                                    {
                                                        Id = ac.Id,
                                                        Name = ac.Code + " - " + ac.Name
                                                    }, "Id", "Name");
            }
            if (id == null)
            {
                customer = new ARCustomer();
                ViewBag.AccountId = 610;
                ViewBag.EntityState = "Create";
                ViewBag.SalesPersons = new SelectList(from ac in _dbContext.ARSalePerson.Where(x => x.IsDelete != true && x.IsActive != false /*&& x.CompanyId == companyId */&& x.EndDate.Date >= DateTime.Now.Date).OrderBy(x => x.Name).ToList()
                                                      select new
                                                      {
                                                          Id = ac.ID,
                                                          Name = ac.Name
                                                      }, "Id", "Name");
                ViewBag.NavbarHeading = "Create Customer";
                ViewBag.ItemCategory = (from c in _dbContext.ARSuplierItemsGroup where c.ApSuplierId == id select c).ToList();
                ViewBag.Id = null;
                customer.ARShippingDetail = new List<ARShippingDetail>();
                customer.ARContactPerson = new List<ARContactPerson>();
                customer.ARSuplierItemsGroup = new List<ARSuplierItemsGroup>();
                customer.CreditLimitList = new List<ARCreditLimit>();

                return View(customer);

            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Customer";
                ViewBag.SalesPersons = new SelectList(from ac in _dbContext.ARSalePerson.Where(x => x.IsDelete == false && x.CompanyId == companyId && x.EndDate.Date >= DateTime.Now.Date).OrderBy(x => x.Name).ToList()
                                                      select new
                                                      {
                                                          Id = ac.ID,
                                                          Name = ac.Name
                                                      }, "Id", "Name");
                var data = _dbContext.ARCustomers.Find(id);
                ViewBag.AccountId = data.AccountId;
                ViewBag.CityId = data.CityId;
                ViewBag.ShippingCityId = data.ShippingCity;
                data.ARShippingDetail = _dbContext.ARShippingDetail.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                data.ARContactPerson = _dbContext.ARContactPerson.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                data.ARSuplierItemsGroup = _dbContext.ARSuplierItemsGroup.Where(u => u.ARCustomerId == data.Id).ToList();
                data.CreditLimitList = _dbContext.ARCreditLimit.Where(x => x.ARCustomerId == data.Id && x.IsDeleted == false).OrderBy(x => x.IsClosed).ToList();

                return View(data);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(ARCustomer customer, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ARContactPerson[] contactperson = JsonConvert.DeserializeObject<ARContactPerson[]>(collection["ContactPerson1"]);
            ARShippingDetail[] shippingDetails = JsonConvert.DeserializeObject<ARShippingDetail[]>(collection["ShippingDetail"]);
            ARCreditLimit[] creditLimit = JsonConvert.DeserializeObject<ARCreditLimit[]>(collection["CredtLimit"]);
            List<ARSuplierItemsGroup> CDetails = new List<ARSuplierItemsGroup>();
            if (customer.Id == 0)
            {
                customer.CompanyId = companyId;
                customer.ResponsibilityId = resp_Id;
                customer.CreatedBy = userId;
                customer.CreatedDate = DateTime.Now;
                customer.IsDeleted = false;

                customer.CreditLimit = Convert.ToString(customer.CreditLimit);

                //if AccountId == 0 then we will create auto AccountId
                if (customer.AccountId == 0)
                {
                    var configvalue = new ConfigValues(_dbContext);
                    int customerControlAccount = configvalue.CreateGLAccountByParentCode("Customer", customer.Name, companyId, userId);
                    customer.AccountId = customerControlAccount;
                }
                _dbContext.ARCustomers.Add(customer);
                await _dbContext.SaveChangesAsync();

                foreach (var item in contactperson)
                {
                    ARContactPerson model = new ARContactPerson();
                    model.PersonName = item.PersonName;
                    model.Designation = item.Designation;
                    model.PhoneNo = item.PhoneNo;
                    model.Ext = item.Ext;
                    model.CellNo = item.CellNo;
                    model.Email = item.Email;
                    model.CustomerId = customer.Id;
                    model.SupplierId = 0;
                    model.IsDeleted = false;
                    model.IsActive = true;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.Type = "Customer";
                    _dbContext.ARContactPerson.Add(model);
                    await _dbContext.SaveChangesAsync();

                }
                foreach (var item in shippingDetails)
                {
                    ARShippingDetail model = new ARShippingDetail();
                    model.Location = item.Location;
                    model.Address = item.Address;
                    model.PhoneNo = item.PhoneNo;

                    model.CustomerId = customer.Id;
                    model.SupplierId = 0;
                    model.IsDeleted = false;
                    model.IsActive = true;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.Type = "Customer";
                    _dbContext.ARShippingDetail.Add(model);
                    await _dbContext.SaveChangesAsync();

                }
                for (int i = 0; i < collection["categoryId"].Count; i++)
                {
                    ARSuplierItemsGroup aRSuplierItemsGroup = new ARSuplierItemsGroup();
                    aRSuplierItemsGroup.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                    aRSuplierItemsGroup.Type = "Customer";
                    aRSuplierItemsGroup.ARCustomerId = customer.Id;
                    CDetails.Add(aRSuplierItemsGroup);
                }
                await _dbContext.SaveChangesAsync();


                foreach (var item in creditLimit)
                {
                    ARCreditLimit model = new ARCreditLimit();
                    model.FromDate = Convert.ToDateTime(item.FromDate);
                    model.ToDate = Convert.ToDateTime(item.ToDate);
                    model.CreditLimit = item.CreditLimit;
                    model.CompanyId = companyId;
                    model.IsActive = true;
                    model.IsExpired = item.IsExpired;
                    model.IsClosed = item.IsClosed;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.IsDeleted = false;
                    model.ARCustomerId = customer.Id;
                    model.IsCreditLimitExceed = true;
                    model.RemainingBalance = Convert.ToDecimal(item.CreditLimit);
                    _dbContext.ARCreditLimit.Add(model);
                    await _dbContext.SaveChangesAsync();

                }

                TempData["error"] = "false";
                TempData["message"] = "Customer has been saved successfully.";
            }
            else
            {
                //avoiding null insertion in model attributes
                var data = _dbContext.ARCustomers.Find(customer.Id);
                data.Name = customer.Name;
                data.CNIC = customer.CNIC;
                data.Phone1 = customer.Phone1;
                data.Phone2 = customer.Phone2;
                data.GSTNo = customer.GSTNo;
                data.NTNo = customer.NTNo;
                data.Email = customer.Email;
                data.Web = customer.Web;
                data.ContactPerson = customer.ContactPerson;
                data.Fax = customer.Fax;
                data.Address = customer.Address;
                data.CountryId = customer.CountryId;
                data.CityId = customer.CityId;
                data.ShippingAddress = customer.ShippingAddress;
                data.ShippingCountry = customer.ShippingCountry;
                data.ShippingCity = customer.ShippingCity;
                data.IsActive = customer.IsActive;
                data.CustomerCategoryID = customer.CustomerCategoryID;
                data.SalesPersonId = customer.SalesPersonId;
                data.ProductTypeId = customer.ProductTypeId;
                //data.CreditLimit = customer.CreditLimit;
                data.UpdatedBy = userId;
                data.CompanyId = companyId;
                data.ResponsibilityId = resp_Id;
                data.UpdatedDate = DateTime.Now;
                var entry = _dbContext.ARCustomers.Update(data);
                await _dbContext.SaveChangesAsync();
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                TempData["error"] = "false";
                TempData["message"] = "Customer has been updated successfully.";

                var removeShipping = _dbContext.ARShippingDetail.Where(u => u.CustomerId == customer.Id).ToList();
                var removeContactPerson = _dbContext.ARContactPerson.Where(u => u.CustomerId == customer.Id).ToList();
                var removeCategory = _dbContext.ARSuplierItemsGroup.Where(u => u.ARCustomerId == customer.Id).ToList();
                var removeCreditLimit = _dbContext.ARCreditLimit.Where(u => u.ARCustomerId == customer.Id).ToList();

                foreach (var item in removeShipping)
                {
                    _dbContext.ARShippingDetail.Remove(item);
                    _dbContext.SaveChanges();
                }

                foreach (var item in removeContactPerson)
                {
                    _dbContext.ARContactPerson.Remove(item);
                    _dbContext.SaveChanges();
                }

                foreach (var item in removeCategory)
                {
                    _dbContext.ARSuplierItemsGroup.Remove(item);
                    _dbContext.SaveChanges();
                }

                //foreach (var item in removeCreditLimit)
                //{
                //    _dbContext.ARCreditLimit.Remove(item);
                //    _dbContext.SaveChanges();
                //}

                foreach (var item in contactperson)
                {
                    ARContactPerson model = new ARContactPerson();
                    model.PersonName = item.PersonName;
                    model.Designation = item.Designation;
                    model.PhoneNo = item.PhoneNo;
                    model.Ext = item.Ext;
                    model.CellNo = item.CellNo;
                    model.Email = item.Email;
                    model.CustomerId = customer.Id;
                    model.SupplierId = 0;
                    model.IsDeleted = false;
                    model.IsActive = true;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.Type = "Customer";
                    _dbContext.ARContactPerson.Add(model);
                    await _dbContext.SaveChangesAsync();

                }
                foreach (var item in shippingDetails)
                {
                    ARShippingDetail model = new ARShippingDetail();
                    model.Location = item.Location;
                    model.Address = item.Address;
                    model.PhoneNo = item.PhoneNo;
                    model.CustomerId = customer.Id;
                    model.SupplierId = 0;
                    model.IsDeleted = false;
                    model.IsActive = true;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.Type = "Customer";
                    _dbContext.ARShippingDetail.Add(model);
                    await _dbContext.SaveChangesAsync();

                }
                for (int i = 0; i < collection["categoryId"].Count; i++)
                {
                    ARSuplierItemsGroup aRSuplierItemsGroup = new ARSuplierItemsGroup();
                    aRSuplierItemsGroup.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                    aRSuplierItemsGroup.Type = "Customer";
                    aRSuplierItemsGroup.ARCustomerId = customer.Id;
                    CDetails.Add(aRSuplierItemsGroup);
                }
                foreach (var item in creditLimit.Where(p=>p.Id == 0))
                {
                    
                    ARCreditLimit model = new ARCreditLimit();
                    model.FromDate = Convert.ToDateTime(item.FromDate);
                    model.ToDate = Convert.ToDateTime(item.ToDate);
                    model.CreditLimit = item.CreditLimit.Trim();
                    model.CompanyId = companyId;
                    model.IsActive = true;
                    model.IsClosed = item.IsClosed;
                    model.IsExpired = item.IsExpired;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.IsDeleted = false;
                    model.ARCustomerId = customer.Id;
                    model.RemainingBalance = Convert.ToDecimal(item.CreditLimit.Trim());
                    if (removeCreditLimit.Count > 0)
                    {
                        if (Convert.ToDecimal(item.CreditLimit.Trim()) > Convert.ToDecimal(removeCreditLimit.LastOrDefault().CreditLimit.Trim()))
                        {
                            model.IsCreditLimitExceed = true;
                        }
                        else if (Convert.ToDecimal(item.CreditLimit.Trim()) == Convert.ToDecimal(removeCreditLimit.LastOrDefault().CreditLimit.Trim()))
                        {
                            model.IsCreditLimitExceed = null;
                        }
                        else
                        {
                            model.IsCreditLimitExceed = false;
                        }
                    }
                    else
                    {
                        model.IsCreditLimitExceed = false;
                    }

                    //if (Convert.ToDecimal(item.CreditLimit.Trim()) > Convert.ToDecimal(removeCreditLimit.LastOrDefault().CreditLimit.Trim()))
                    //{
                    //    model.IsCreditLimitExceed = true;
                    //}
                    //else if (Convert.ToDecimal(item.CreditLimit.Trim()) == Convert.ToDecimal(removeCreditLimit.LastOrDefault().CreditLimit.Trim()))
                    //{
                    //    model.IsCreditLimitExceed = null;
                    //}
                    //else
                    //{
                    //    model.IsCreditLimitExceed = false;
                    //}

                    _dbContext.ARCreditLimit.Add(model);
                    _dbContext.SaveChanges();
                }

            }
            await _dbContext.ARSuplierItemsGroup.AddRangeAsync(CDetails);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> DeleteCustomer(int Id)
        {

            var customer = await _dbContext.ARCustomers.Where(n => n.Id == Id).FirstAsync();
            if (customer != null)
            {
                customer.IsActive = false;
                customer.IsDeleted = true;
                _dbContext.ARCustomers.Update(customer);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Customer has been deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CloseCreditLimit(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var creditLimit = await _dbContext.ARCreditLimit.Where(n => n.Id == id).FirstAsync();
            if (creditLimit != null)
            {
                creditLimit.IsClosed = true;
                creditLimit.UpdatedBy = userId;
                creditLimit.UpdatedDate = DateTime.Now;
                _dbContext.ARCreditLimit.Update(creditLimit);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Create", "Customer", new { Id = creditLimit.ARCustomerId });
        }

        public async Task<IActionResult> ExpireCreditLimit(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var creditLimit = _dbContext.ARCreditLimit.Where(n => n.Id == id).FirstOrDefault();
            if (creditLimit != null)
            {
                creditLimit.IsExpired = true;
                creditLimit.UpdatedBy = userId;
                creditLimit.UpdatedDate = DateTime.Now;
                _dbContext.ARCreditLimit.Update(creditLimit);
                await _dbContext.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetSlaePerson(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            //var item = (from c in _dbContext.ARSalePersonCity.Where(x => x.City_ID == Id)
            var item = (from c in _dbContext.ARSalePersonItemCategory.Where(x => x.CityId == Id)
                        join Sp in _dbContext.ARSalePerson.Where(x => x.EndDate.Date >= DateTime.Now.Date) on c.SalePerson_ID equals Sp.ID
                        where Sp.IsActive && Sp.IsDelete == false && Sp.CompanyId == companyId
                        select new
                        {
                            id = Sp.ID,
                            text = Sp.Name
                        }).GroupBy(x => x.id).Select(x => new { 
                            id = x.Select(a => a.id).FirstOrDefault(),
                            text = x.Select(a => a.text).FirstOrDefault()
                        }).ToList();
            return Ok(item);

        }

        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchId = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchName = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchAccount = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchAddress = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchPhone1 = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchNTNno = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchGSTNo = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var CustomersData = (from Customers in _dbContext.ARCustomers.Where(x => x.IsDeleted == false /*&& x.CompanyId == companyId*/) select Customers);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    CustomersData = CustomersData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    CustomersData = CustomersData.Where(m => m.Id.ToString().Contains(searchValue)
                //                                    || m.Name.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || (m.AccountId != 0 ? _dbContext.GLAccounts.FirstOrDefault(x => x.Id == Convert.ToInt32(m.AccountId)).Name.ToUpper().Contains(searchValue.ToUpper()) : m.Id.ToString().Contains(searchValue))
                //                                    || (m.Address != null ? m.Address.ToString().ToUpper().Contains(searchValue.ToUpper()) : m.Id.ToString().Contains(searchValue))
                //                                    || (m.Phone1 != null ? m.Phone1.ToString().Contains(searchValue) : m.Id.ToString().Contains(searchValue))
                //                                    || (m.Phone2 != null ? m.Phone2.ToString().Contains(searchValue) : m.Id.ToString().Contains(searchValue))
                //                                    || (m.NTNo != null ? m.NTNo.ToString().Contains(searchValue) : m.Id.ToString().Contains(searchValue))
                //                                    || (m.GSTNo != null ? m.GSTNo.ToString().Contains(searchValue) : m.Id.ToString().Contains(searchValue))
                //                                  );

                //}
                CustomersData = !string.IsNullOrEmpty(searchId) ? CustomersData.Where(m => m.Id.ToString().Contains(searchId)) : CustomersData;
                CustomersData = !string.IsNullOrEmpty(searchName) ? CustomersData.Where(m => m.Name.ToString().ToUpper().Contains(searchName.ToUpper())) : CustomersData;
                CustomersData = !string.IsNullOrEmpty(searchAccount) ? CustomersData.Where(m => (m.AccountId != 0 ? _dbContext.GLAccounts.FirstOrDefault(x => x.Id == Convert.ToInt32(m.AccountId)).Name.ToUpper().Contains(searchAccount.ToUpper()) : m.Id.ToString().Contains(searchAccount))) : CustomersData;
                CustomersData = !string.IsNullOrEmpty(searchAddress) ? CustomersData.Where(m => (m.Address != null ? m.Address.ToString().ToUpper().Contains(searchAddress.ToUpper()) : m.Id.ToString().Contains(searchAddress))) : CustomersData;
                CustomersData = !string.IsNullOrEmpty(searchPhone1) ? CustomersData.Where(m => (m.Phone1 != null ? m.Phone1.ToString().Contains(searchPhone1) : m.Id.ToString().Contains(searchPhone1))) : CustomersData;
                CustomersData = !string.IsNullOrEmpty(searchNTNno) ? CustomersData.Where(m => (m.NTNo != null ? m.NTNo.ToString().Contains(searchNTNno) : m.Id.ToString().Contains(searchNTNno))) : CustomersData;
                CustomersData = !string.IsNullOrEmpty(searchGSTNo) ? CustomersData.Where(m => (m.GSTNo != null ? m.GSTNo.ToString().Contains(searchGSTNo) : m.Id.ToString().Contains(searchGSTNo))) : CustomersData;

                recordsTotal = CustomersData.Count();
                var data = CustomersData.ToList();
                if (pageSize == -1)
                {
                    data = CustomersData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = CustomersData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARCustomerViewModel> Details = new List<ARCustomerViewModel>();
                foreach (var grp in data)
                {
                    ARCustomerViewModel aRCustomerViewModel = new ARCustomerViewModel();
                    aRCustomerViewModel.AccountName = (grp.AccountId != 0 ? _dbContext.GLAccounts.FirstOrDefault(x => x.Id == grp.AccountId).Name : "-");
                    aRCustomerViewModel.ARCustomer = grp;

                    Details.Add(aRCustomerViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IActionResult Detail(int id)
        {
             var companyId = HttpContext.Session.GetInt32("CompanyId");
            ARCustomer customer;
            ViewBag.AppCountries = new SelectList(_dbContext.AppCountries.OrderBy(c => c.Id), "Id", "Name");
            //ViewBag.SalesPersons = new ConfigValues(_dbContext).GetConfigValues("AP", "SalesPerson", Convert.ToInt32(companyId));
            ViewBag.CustomerCategory = new ConfigValues(_dbContext).GetConfigValues("AR", "CustomerCategory", Convert.ToInt32(companyId));
            ViewBag.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId));
            InventoryHelper helper = new InventoryHelper(_dbContext, HttpContext.Session.GetInt32("CompanyId").Value);
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2/* && x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()

                                                          select new
                                                          {
                                                              Id = ac.Id,
                                                              Name = ac.Code + " - " + ac.Name
                                                          }, "Id", "Name");
            ViewBag.Categories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                select new
                                                {
                                                    Id = ac.Id,
                                                    Name = ac.Code + " - " + ac.Name
                                                }, "Id", "Name");

            //ViewBag.listOfItemCategories = helper.GetCategoriesSelectLists();
            if (id != null)
            {
                customer = _dbContext.ARCustomers.Find(id);

                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == customer.CountryId).ToList(), "Id", "Name");
                ViewBag.ShippingCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == customer.ShippingCountry).ToList(), "Id", "Name");
                customer.ARShippingDetail = _dbContext.ARShippingDetail.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                customer.ARContactPerson = _dbContext.ARContactPerson.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                ViewBag.ItemCategory = (from c in _dbContext.ARSuplierItemsGroup where c.ARCustomerId == id select c).ToList();
                ViewBag.Categories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                    where !_dbContext.ARSuplierItemsGroup.Where(x => x.ARCustomerId == id).Any(s => ac.Id.ToString().Contains(s.CategoryId.ToString()))
                                                    select new
                                                    {
                                                        Id = ac.Id,
                                                        Name = ac.Code + " - " + ac.Name
                                                    }, "Id", "Name");
            }
            if (id == null)
            {
                customer = new ARCustomer();
                ViewBag.AccountId = 610;
                ViewBag.EntityState = "Create";
                ViewBag.SalesPersons = new SelectList(from ac in _dbContext.ARSalePerson.Where(x => x.IsDelete != true && x.IsActive != false && x.CompanyId == companyId && x.EndDate.Date >= DateTime.Now.Date).OrderBy(x => x.Name).ToList()
                                                      select new
                                                      {
                                                          Id = ac.ID,
                                                          Name = ac.Name
                                                      }, "Id", "Name");
                ViewBag.NavbarHeading = "Create Customer";
                ViewBag.ItemCategory = (from c in _dbContext.ARSuplierItemsGroup where c.ApSuplierId == id select c).ToList();
                ViewBag.Id = null;
                customer.ARShippingDetail = new List<ARShippingDetail>();
                customer.ARContactPerson = new List<ARContactPerson>();
                customer.ARSuplierItemsGroup = new List<ARSuplierItemsGroup>();
                customer.CreditLimitList = new List<ARCreditLimit>();

                return View(customer);

            }
            else
            {
                ViewBag.EntityState = "Detail";
                ViewBag.NavbarHeading = "Detail Customer";
                ViewBag.SalesPersons = new SelectList(from ac in _dbContext.ARSalePerson.Where(x => x.IsDelete == false && x.CompanyId == companyId && x.EndDate.Date >= DateTime.Now.Date).OrderBy(x => x.Name).ToList()
                                                      select new
                                                      {
                                                          Id = ac.ID,
                                                          Name = ac.Name
                                                      }, "Id", "Name");
                var data = _dbContext.ARCustomers.Find(id);
                ViewBag.AccountId = data.AccountId;
                ViewBag.CityId = data.CityId;
                ViewBag.ShippingCityId = data.ShippingCity;
                data.ARShippingDetail = _dbContext.ARShippingDetail.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                data.ARContactPerson = _dbContext.ARContactPerson.Where(a => a.IsDeleted == false && a.CustomerId == id).ToList();
                data.ARSuplierItemsGroup = _dbContext.ARSuplierItemsGroup.Where(u => u.ARCustomerId == data.Id).ToList();
                data.CreditLimitList = _dbContext.ARCreditLimit.Where(x => x.ARCustomerId == data.Id && x.IsDeleted == false).OrderBy(x => x.IsClosed).ToList();

                return View(data);
            }
        }
    }
}