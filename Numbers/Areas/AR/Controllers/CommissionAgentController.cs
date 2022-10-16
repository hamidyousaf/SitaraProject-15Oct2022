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
    public class CommissionAgentController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly CustomerRepository _customerRepository;
        public CommissionAgentController(NumbersDbContext context, CustomerRepository customerRepository)
        {
            _dbContext = context;
            _customerRepository = customerRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _dbContext.ARCommissionAgents.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).ToListAsync();
            ViewBag.NavbarHeading = "List of Commission Agents";
            return View(customers);
        }
        public IActionResult Create(int? id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            ARCommissionAgent customer;
            // var cust = _dbContext.ARCommissionAgentCustomer.ToList();
            var newData = new SelectList(from c in _dbContext.ARCustomers.Where(x => x.IsActive == true && x.IsDeleted == false).ToList()
                                         where !_dbContext.ARCommissionAgentCustomer.Any(s => c.Id.ToString().Contains(s.Customer_Id.ToString()))
                                         select new
                                         {
                                             Id = c.Id,
                                             Name = c.Name
                                         }, "Id", "Name");

            ViewBag.CustomerList = newData;
            //ViewBag.Customers = new SelectList(_dbContext.ARCustomers.OrderBy(c=>c.Name).ToList(), "Id", "Name");
            ViewBag.AppCountries = new SelectList(_dbContext.AppCountries.OrderBy(c => c.Id ).ToList(), "Id", "Name");

            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                            select new
                                                            {
                                                                Id = ac.Id,
                                                                Name = ac.Code + " - " + ac.Name
                                                            }, "Id", "Name");

            ViewBag.CustomerCategory = new SelectList(_dbContext.ARCustomers.ToList(), "Id", "Name");
            ViewBag.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId));
            InventoryHelper helper = new InventoryHelper(_dbContext, HttpContext.Session.GetInt32("CompanyId").Value);
            //var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                          select new
                                                          {
                                                              Id = ac.Id,
                                                              Name = ac.Code + " - " + ac.Name
                                                          }, "Id", "Name");

            ViewBag.CityId = null;
            if (id != null)
            {
                //
                  var CustomerDetaiil  = _dbContext.ARCommissionAgentCustomer.Include(p => p.Customer).ThenInclude(p => p.SalesPerson).Where(p => p.CommissionAgent_Id == id).Select(p => new AgentCustomerDTO()
                {
                    CustomerId = p.Customer_Id,
                    CustomerName = p.Customer.Name,
                    SalesPerson = p.Customer.SalesPerson.Name,
                    City = p.Customer.City.Name,
                    
                    ItemCategoryId=p.CategoryId,
                    ItemCategoryNames = _dbContext.InvItemCategories.Where(x => x.Id == p.CategoryId).Select(p => p.Name).ToList()
                });
                ViewBag.Customers = CustomerDetaiil;
                customer = _dbContext.ARCommissionAgents.Find(id);
                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == customer.CountryId).ToList(), "Id", "Name");

                //var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                select new
                                                                {
                                                                    Id = ac.Id,
                                                                    Name = ac.Code + " - " + ac.Name
                                                                }, "Id", "Name");

                ViewBag.CategoryId = CustomerDetaiil.Select(p => p.ItemCategoryId).FirstOrDefault();
                //ViewBag.Customer = (from c in _dbContext.ARCommissionAgentCustomer where c.CommissionAgent_Id == id select c).ToList();
            }
            if (id == null)
            {
                customer = new ARCommissionAgent();

                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == customer.CountryId).ToList(), "Id", "Name");
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Commission Agents";
                ViewBag.Id = null;
                customer.IsActive = true;
                return View(customer);
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Commission Agents";
                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == 1).ToList(), "Id", "Name");
                
                var data = _dbContext.ARCommissionAgents.Find(id);
                
                ViewBag.CityId = data.CityId;
                return View(data);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(ARCommissionAgent customer, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            List<ARCommissionAgentCustomer> customersDetails = new List<ARCommissionAgentCustomer>();
            if (customer.Id == 0)
            {
                customer.CompanyId = companyId;
                customer.ResponsibilityId = resp_Id;
                customer.CreatedBy = userId;
                customer.CreatedDate = DateTime.Now;
                customer.IsDeleted = false;
                customer.ItemCategoryId = customer.ItemCategoryId;
                _dbContext.ARCommissionAgents.Add(customer);
                await _dbContext.SaveChangesAsync();
                for (int i = 0; i < collection["customerId"].Count; i++)
                {
                    ARCommissionAgentCustomer aRCommissionAgentCustomer = new ARCommissionAgentCustomer();
                    aRCommissionAgentCustomer.Customer_Id = Convert.ToInt32(collection["customerId"][i]);
                    aRCommissionAgentCustomer.CommissionAgent_Id = customer.Id;
                    aRCommissionAgentCustomer.CategoryId= Convert.ToInt32(collection["categoryId"][i]);
                    customersDetails.Add(aRCommissionAgentCustomer);
                }
                _dbContext.ARCommissionAgentCustomer.AddRange(customersDetails);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Commission Agent has been saved successfully.";
            }
            else
            {
                //avoiding null insertion in model attributes
                var data = _dbContext.ARCommissionAgents.Find(customer.Id);
                data.Name = customer.Name;
                data.CNIC = customer.CNIC;
                data.Phone1 = customer.Phone1;
                data.ItemCategoryId = customer.ItemCategoryId;
                data.CityId = customer.CityId;
                data.Address = customer.Address;
                data.CountryId = customer.CountryId;
                data.ResponsibilityId = customer.ResponsibilityId;
                data.CityId = customer.CityId;
                data.CustomerId = customer.CustomerId;
                data.CommissionPer = customer.CommissionPer; ;
                data.IsActive = customer.IsActive;
                data.SalesPersonId = customer.SalesPersonId;
                data.StartDate = customer.StartDate;
                data.EndDate = customer.EndDate;
                data.UpdatedBy = userId;
                data.CompanyId = companyId;
                data.UpdatedDate = DateTime.Now;
                data.ItemCategoryId = customer.ItemCategoryId;
                var entry = _dbContext.ARCommissionAgents.Update(data);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                _dbContext.ARCommissionAgentCustomer.RemoveRange(_dbContext.ARCommissionAgentCustomer.Where(x => x.CommissionAgent_Id == customer.Id));
                await _dbContext.SaveChangesAsync();
                for (int i = 0; i < collection["customerId"].Count; i++)
                {
                    ARCommissionAgentCustomer aRCommissionAgentCustomer = new ARCommissionAgentCustomer();
                    aRCommissionAgentCustomer.Customer_Id = Convert.ToInt32(collection["customerId"][i]);
                    aRCommissionAgentCustomer.CommissionAgent_Id = customer.Id;
                    // aRCommissionAgentCustomer.CategoryId = Convert.ToInt32(collection["cat_Id"][i]);
                    aRCommissionAgentCustomer.CategoryId = Convert.ToInt32(collection["categoryId"][i]);
                    customersDetails.Add(aRCommissionAgentCustomer);
                }
                _dbContext.ARCommissionAgentCustomer.AddRange(customersDetails);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Commission Agent has been updated successfully.";

            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteCustomer(int Id)
        {
            var customer = await _dbContext.ARCommissionAgents.Where(n => n.Id == Id).FirstAsync();
            if (customer != null)
            {
                customer.IsActive = false;
                customer.IsDeleted = true;
                _dbContext.ARCommissionAgents.Update(customer);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Commission Agent has been Deleted successfully.";

            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetCustomerDetails(int cityId , int categoryId)
        {
           
            
            int[] CustomersIds = _dbContext.ARCommissionAgentCustomer.Where(x => x.CategoryId !=0).Select(x => Convert.ToInt32(String.Concat(x.Customer_Id,x.CategoryId))).ToArray();
            //int[] CategoryIds = _dbContext.ARCommissionAgentCustomer.Where(x => x.Customer_Id== Convert.ToInt32(CustomersIds)).Select(x => x.CategoryId).ToArray();

            var customer = (from c in _dbContext.ARCustomers 
                             join cig in _dbContext.ARSuplierItemsGroup on c.Id equals cig.ARCustomerId
                             join cat in _dbContext.InvItemCategories on cig.CategoryId equals cat.Id
                             join sp in _dbContext.ARSalePerson on c.SalesPersonId equals sp.ID
                             join city in _dbContext.AppCities on c.CityId equals city.Id
                           
                             where !CustomersIds.Contains(Convert.ToInt32(String.Concat(c.Id, cig.CategoryId))) &&  c.CityId == cityId && cat.Id == categoryId 
                            select  new {                                
                                 SalesPerson = sp.Name,
                                 ItemCategory = cat.Name,
                                 City = city.Name,
                                 Customer = c.Name,
                                 CustomerId=c.Id,
                                 CategoryId=cat.Id
                            }).Distinct().ToList();
           
            return Ok(customer);
        }

        public IActionResult GetCommissionAgent()
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
                var searchAddress = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchPhone1 = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchComm = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var CommissionAgentsData = (from CommissionAgents in _dbContext.ARCommissionAgents.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select CommissionAgents);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    CommissionAgentsData = CommissionAgentsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    CommissionAgentsData = CommissionAgentsData.Where(m => m.Id.ToString().Contains(searchValue)
                //                                    || m.Name.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Address.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Phone1.ToString().Contains(searchValue)
                //                                    || m.CommissionPer.ToString().Contains(searchValue)
                //                                  );
                //}
                CommissionAgentsData = !string.IsNullOrEmpty(searchId) ? CommissionAgentsData.Where(m => m.Id.ToString().Contains(searchId)) : CommissionAgentsData;
                CommissionAgentsData = !string.IsNullOrEmpty(searchName) ? CommissionAgentsData.Where(m => m.Name.ToString().ToUpper().Contains(searchName.ToUpper())) : CommissionAgentsData;
                CommissionAgentsData = !string.IsNullOrEmpty(searchAddress) ? CommissionAgentsData.Where(m => m.Address.ToString().ToUpper().Contains(searchAddress.ToUpper())) : CommissionAgentsData;
                CommissionAgentsData = !string.IsNullOrEmpty(searchPhone1) ? CommissionAgentsData.Where(m => m.Phone1.ToString().Contains(searchPhone1)) : CommissionAgentsData;
                CommissionAgentsData = !string.IsNullOrEmpty(searchComm) ? CommissionAgentsData.Where(m => m.CommissionPer.ToString().Contains(searchComm)) : CommissionAgentsData;
                recordsTotal = CommissionAgentsData.Count();
                var data = CommissionAgentsData.ToList();
                if (pageSize == -1)
                {
                    data = CommissionAgentsData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = CommissionAgentsData.Skip(skip).Take(pageSize).ToList();
                }
                List<CommissionAgentViewModel> Details = new List<CommissionAgentViewModel>();
                foreach (var grp in data)
                {
                    CommissionAgentViewModel commissionAgentViewModel = new CommissionAgentViewModel();
                    commissionAgentViewModel.ARCommissionAgent = grp;

                    Details.Add(commissionAgentViewModel);

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
            ARCommissionAgent customer;
            // var cust = _dbContext.ARCommissionAgentCustomer.ToList();
            var newData = new SelectList(from c in _dbContext.ARCustomers.Where(x => x.IsActive == true && x.IsDeleted == false).ToList()
                                         where !_dbContext.ARCommissionAgentCustomer.Any(s => c.Id.ToString().Contains(s.Customer_Id.ToString()))
                                         select new
                                         {
                                             Id = c.Id,
                                             Name = c.Name
                                         }, "Id", "Name");

            ViewBag.CustomerList = newData;
            //ViewBag.Customers = new SelectList(_dbContext.ARCustomers.OrderBy(c=>c.Name).ToList(), "Id", "Name");
            ViewBag.AppCountries = new SelectList(_dbContext.AppCountries.OrderBy(c => c.Id).ToList(), "Id", "Name");

            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                            select new
                                                            {
                                                                Id = ac.Id,
                                                                Name = ac.Code + " - " + ac.Name
                                                            }, "Id", "Name");

            ViewBag.CustomerCategory = new SelectList(_dbContext.ARCustomers.ToList(), "Id", "Name");
            ViewBag.ProductType = new ConfigValues(_dbContext).GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId));
            InventoryHelper helper = new InventoryHelper(_dbContext, HttpContext.Session.GetInt32("CompanyId").Value);
            //var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                          select new
                                                          {
                                                              Id = ac.Id,
                                                              Name = ac.Code + " - " + ac.Name
                                                          }, "Id", "Name");

            ViewBag.CityId = null;
            if (id != 0)
            {
                //
                var CustomerDetaiil = _dbContext.ARCommissionAgentCustomer.Include(p => p.Customer).ThenInclude(p => p.SalesPerson).Where(p => p.CommissionAgent_Id == id).Select(p => new AgentCustomerDTO()
                {
                    CustomerId = p.Customer_Id,
                    CustomerName = p.Customer.Name,
                    SalesPerson = p.Customer.SalesPerson.Name,
                    City = p.Customer.City.Name,

                    ItemCategoryId = p.CategoryId,
                    ItemCategoryNames = _dbContext.InvItemCategories.Where(x => x.Id == p.CategoryId).Select(p => p.Name).ToList()
                });
                ViewBag.Customers = CustomerDetaiil;
                customer = _dbContext.ARCommissionAgents.Find(id);
                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == customer.CountryId).ToList(), "Id", "Name");

         

                ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                select new
                                                                {
                                                                    Id = ac.Id,
                                                                    Name = ac.Code + " - " + ac.Name
                                                                }, "Id", "Name");

                ViewBag.CategoryId = CustomerDetaiil.Select(p => p.ItemCategoryId).FirstOrDefault();
            }
            if (id != 0)
            {
              
                ViewBag.EntityState = "Detail";
                ViewBag.NavbarHeading = "Detail Commission Agents";
                ViewBag.AppCities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == 1).ToList(), "Id", "Name");

                var data = _dbContext.ARCommissionAgents.Find(id);

                ViewBag.CityId = data.CityId;
                return View(data);
            }
            return View();
        }

    }
}