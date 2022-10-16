using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Setup;

using Numbers.Repository.Helpers;
using Numbers.Areas.Inventory;
using Numbers.Entity.ViewModels;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    public class ApiController : ControllerBase
    {
        private readonly NumbersDbContext _dbContext;
        public ApiController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult GetCustomers(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var customers = _dbContext.ARCustomers.Where(
                                                (a => a.CompanyId == companyId && a.IsActive == true && a.IsDeleted != true
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   //text = string.Concat(a.Id, " - ", a.Name)
                                                   text = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(customers);
        }
 
        [HttpGet]
        public IActionResult GetCustomer(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var customer = _dbContext.ARCustomers.Where(a => a.Id == id && a.CompanyId == companyId && a.IsActive == true && a.IsDeleted != true)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Id, " - ", a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(customer);
        }

        public IActionResult GetCustomers(int ProductId, int CategoryId)
        {
            List<int> Category = _dbContext.ARSuplierItemsGroup.Where(x => x.CategoryId == CategoryId).Select(x => x.ARCustomerId).ToList();
            var Customers = _dbContext.ARCustomers.Include(x => x.City).Where(x => Category.Contains(x.Id) && x.IsDeleted != true).ToList();
            var Categories = _dbContext.InvItemCategories.AsQueryable();
            var CategoriesData = from L1 in Categories
                                 join L2 in Categories on L1.Id equals L2.ParentId
                                 join L3 in Categories on L2.Id equals L3.ParentId
                                 join L4 in Categories on L3.Id equals L4.ParentId
                                 join item in _dbContext.InvItems on L4.Id equals item.CategoryId
                                 where L2.Id == CategoryId && item.IsDeleted != true && L4.IsDeleted != true && L3.IsDeleted != true
                                 select new
                                 {
                                     id = item.Id,
                                     text = string.Concat(item.Name, " - ", item.Code)
                                 };

            return Ok(new { Customers = Customers, categories = CategoriesData });
        }

        public IActionResult GetItemsByCategoryId(int categoryId)
        {
            var items = _dbContext.InvItems.Include(x => x.Category).Where(x => x.CategoryId == categoryId && x.IsDeleted != true).ToList();
           
            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetCreditLimitByCustomer(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<ARReceipt> receipts = _dbContext.ARReceipts.Where(x => x.Status == "Approved" && x.CustomerId == id).Select(a => new ARReceipt
            {
                GrandTotal = a.GrandTotal
            }).ToList();

            List<ARSaleOrder> Saleorder = _dbContext.ARSaleOrders.Where(x => x.Status == "Approved" && x.CustomerId == id).ToList();
            if (receipts.Count > 0 || Saleorder.Count > 0)
            {
                //List<int> SaleOrderList = (from b in _dbContext.ARSaleOrders
                //                           join c in _dbContext.ARSaleOrderItems on b.Id equals c.SaleOrderId
                //                           join d in _dbContext.ARInvoiceItems on c.Id equals d.SalesOrderItemId
                //                           join e in _dbContext.ARInvoices on d.InvoiceId equals e.Id
                //                           where e.CompanyId == companyId && e.CustomerId == id
                //                           select b.Id).ToList();
                //List<ARSaleOrder> Saleorder = _dbContext.ARSaleOrders.Where(x =>x.Status== "Approved" && x.CustomerId == id).ToList();
                ////List<ARSaleOrder> Saleorder = _dbContext.ARSaleOrders.Where(x => x.CustomerId == id).Select(a => new ARSaleOrder
                ////{
                ////    GrandTotal = a.GrandTotal
                ////}).ToList();
                ///
                DateTime date = DateTime.Now;
                List<ARCreditLimit> customer = _dbContext.ARCreditLimit.Where(a => a.ARCustomerId == id && a.IsClosed == false && a.ToDate.Date >= date.Date && a.CompanyId == companyId && a.IsActive == true && a.IsDeleted != true).ToList();
                //.Select(a => new
                //{
                //    id = a.Id,
                //    creditLimit = a.CreditLimit,
                //    ledgerDebit = Ledger.Sum(x => x.Debit) + Saleorder.Sum(x => x.GrandTotal),
                //    ledgerCredit = Ledger.Sum(x => x.Credit),
                //    ledgerbalance = (Convert.ToDecimal(Ledger.Sum(x => x.Debit)) + Convert.ToDecimal(Saleorder.Sum(x => x.GrandTotal))) - Convert.ToDecimal(Ledger.Sum(x => x.Credit))
                //})
                //.ToList();
                ARCreditLimit a = new ARCreditLimit();
                a.CreditLimitList = customer.Sum(x => Convert.ToDecimal(x.CreditLimit));

                a.ledgerbalance = customer.Sum(x => Convert.ToDecimal(x.CreditLimit)) + Convert.ToDecimal(receipts.Sum(x => x.GrandTotal)) - Convert.ToDecimal(Saleorder.Sum(x => x.Total));
                return Ok(a);
            }
            else
            {
                DateTime date = DateTime.Now;
                List<ARCreditLimit> customer = _dbContext.ARCreditLimit.Where(a => a.ARCustomerId == id && a.ToDate.Date >= date.Date && a.IsClosed == false /*&& a.CompanyId == companyId*/ && a.IsActive == true && a.IsDeleted != true).ToList();
                //.Select(new a
                ////{
                ////    //id = a.Id,
                ////    //creditLimit = a.CreditLimit,
                ////    ////ledgerDebit = Ledger.Sum(x => x.Debit) ,
                ////    ////ledgerCredit = Ledger.Sum(x => x.Credit),
                ////    ////ledgerbalance = (Convert.ToDecimal(Ledger.Sum(x => x.Debit))) - Convert.ToDecimal(Ledger.Sum(x => x.Credit))
                ////}
                //)

                ARCreditLimit a = new ARCreditLimit();
                a.CreditLimitList = customer.Sum(x => Convert.ToDecimal(x.CreditLimit));
                return Ok(a);
            }


        }
        [HttpGet]
        public IActionResult GetRecoveryPercentageByCustomer(int customerId, int itemCategoryId, DateTime SaleOrderDate)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
             var voucherCustomer = _dbContext.GLVouchers.Where(x => x.Status == "Approved" && x.ModuleName.Contains("AR/") && x.ReferenceId == customerId && x.IsDeleted != true && (x.VoucherType == "RCPT" || x.VoucherType == "CDISADJ")).Select(x => x.Id).ToList();

            var configValues = new ConfigValues(_dbContext);
            var CategoryTypeList = configValues.GetConfigValues("AR", "Category Type", companyId);
            DateTime sago3month = SaleOrderDate.AddMonths(-3);
            var TotalSumReceipts = _dbContext.ARReceipts.Where(x => x.Status == "Approved" && x.CustomerId == customerId && x.ReceiptDate >= sago3month && x.ReceiptDate <= SaleOrderDate).Sum(p => p.GrandTotal);
            // var TotalSumSaleInvoices = _dbContext.ARInvoices.Where(x => x.Status == "Approved" && x.CustomerId == customerId).Sum(p => p.GrandTotal);
            var customerDebit = _dbContext.GLVoucherDetails.FromSql("select d.* from GLVouchers m , GLVoucherDetails d where m.Id = d.VoucherId  and m.VoucherType not in ('RCPT', 'CDISADJ') and m.Status = 'Approved' and m.IsDeleted = 0 and m.ModuleName like 'AR/%' and m.ReferenceId = {0} and m.VoucherDate<={1} ", customerId,SaleOrderDate).ToList();

            var customerCredit = _dbContext.GLVoucherDetails.FromSql("select d.* from GLVouchers m , GLVoucherDetails d where m.Id = d.VoucherId  and m.VoucherType in ('RCPT', 'CDISADJ') and m.Status = 'Approved' and m.IsDeleted = 0 and m.ModuleName like 'AR/%' and m.ReferenceId = {0} and m.VoucherDate<={1} ", customerId, SaleOrderDate).ToList();

            decimal debit = 0;
            decimal credit = 0;
            if (customerDebit!=null)
            {
                debit = customerDebit.Sum(x=>x.Debit);
            }
             if(customerCredit!=null)
            {
                credit = customerCredit.Sum(x => x.Credit);
            }
            var CustomerBalance = debit - credit;
            decimal percentage = 0;
            decimal percentagePerWeek = 0;
            decimal weeks =Convert.ToDecimal(12.85);
            if (TotalSumReceipts != 0 && CustomerBalance != 0)
            {
                percentage = (TotalSumReceipts / CustomerBalance) * 100;
                percentagePerWeek = (percentage / weeks);
                if(percentagePerWeek<0)
                {
                    percentagePerWeek = 0;
                }
            }

            var RecoveryPercentageType = _dbContext.ARRecoveryPercentageItem.Include(p => p.ARRecoveryPercentage).Where(p => p.ARRecoveryPercentage.ItemCategory_Id == itemCategoryId && p.FromPerc <= percentagePerWeek && percentagePerWeek <= p.ToPerc && p.ARRecoveryPercentage.IsDeleted != true).FirstOrDefault();

            if (RecoveryPercentageType != null)
            {
                var CategoryType = CategoryTypeList.FirstOrDefault(p => p.Value == RecoveryPercentageType?.CategoryType_Id.ToString()).Text;
                if (CategoryType != "C")
                {
                    return Ok(new { CategoryType = CategoryType, RecoveryPercentage = Math.Round(percentagePerWeek, 2) });
                }
                else
                {
                    return Ok(new { CategoryType = "C", RecoveryPercentage = Math.Round(percentagePerWeek, 2) });
                }
            }
            return Ok(new { CategoryType = "NotFound" });
        }
        public IActionResult GetAllCustomers()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var cities = _dbContext.ARCustomers.Where(c => c.IsActive == true && c.IsDeleted != true).Select(c => new
            {
                id = c.Id,
                text = c.Name
            })
                                               .OrderBy(c => c.text)
                                               .ToList();
            return Ok(cities);
        }

        [HttpGet]
        public IActionResult GetInvItems(string q = "")
        {
            q = q ?? "";
            q = q.ToUpper();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            List<InvItemCategories> ItemCategory = _dbContext.Sys_ResponsibilityItemCategory.Include(x => x.ItemCategory).Where(x => x.ResponsibilityId == resp_Id).Select(x => x.ItemCategory).ToList();
            List<InvItem> ItemByResp = new List<InvItem>();
            foreach (InvItemCategories item in ItemCategory)
            {
                var itemList = _dbContext.InvItems.Include(x => x.Category).Where(x => /*x.CompanyId == companyId &&*/ x.IsDeleted == false && x.Category.Code.StartsWith(item.Code)).ToList();

                ItemByResp.AddRange(itemList.ToList());
            }
            var i = ItemByResp.Where(
                                                (a => (a.Code.ToUpper().Contains(q) || a.Name.ToUpper().Contains(q)) /*&& a.CompanyId == companyId*/
                                               )).ToList();
            var items = ItemByResp.Where(
                                                (a => (a.Code.ToUpper().Contains(q) || a.Name.ToUpper().Contains(q)) /*&& a.CompanyId == companyId*/
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Code,
                                                   text = a.Code + " - " + a.Name,
                                                })
                                               .OrderBy(a => a.code)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetItems(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            var i = _dbContext.InvItems.Where(
                                                (a => (a.Code.Contains(q) || a.Name.Contains(q)) && a.CompanyId == companyId
                                               )).ToList();
            var items = _dbContext.InvItems.Where(
                                                (a => (a.Code.Contains(q) || a.Name.Contains(q)) && a.CompanyId == companyId
                                               ))
                                               .Include(a => a.Category)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Code,
                                                   text = a.Name,
                                                   type = a.ItemType,
                                                   category = string.Format("{0} --> {1} --> {2}", a.Category.Parent.Parent.Name, a.Category.Parent.Name, a.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetItem(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var item = _dbContext.InvItems.Where(a => a.Id == id && a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Name
                                               })
                                               .FirstOrDefault();
            return Ok(item);
        }
        public IActionResult GetCities()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var cities = _dbContext.AppCities.Select(c => new
            {
                id = c.Id,
                text = c.Name
            })
                                               .OrderBy(c => c.text)
                                               .ToList();
            return Ok(cities);
        }
        public IActionResult GetCustomersForReport()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var customers = _dbContext.ARCustomers.Where((c => c.CompanyId == companyId && c.IsActive == true))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(customers);
        }
        public async Task<IActionResult> GetSalesPersonForReport()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //var SalesPerson = new ConfigValues(_dbContext).GetConfigValues("AP", "SalesPerson", Convert.ToInt32(companyId));
            var SalesPerson = await _dbContext.AppCompanyConfigs.Where(a => a.BaseId == 39 && a.CompanyId == companyId && a.IsActive == true).Select(c => new
            {
                id = c.Id,
                text = c.ConfigValue
            })
                                               .OrderBy(a => a.text).ToListAsync();
            return Ok(SalesPerson);
        }


        [HttpGet]
        public IActionResult GetCity(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var city = _dbContext.AppCities.Where(a => a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Name
                                               })
                                               .FirstOrDefault();
            return Ok(city);
        }
        [HttpGet]
        public IActionResult GetSaleOrders()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var orders = _dbContext.ARSaleOrders.Include(o => o.Customer).Where(a => a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat("Order#: ", a.SaleOrderNo, "    Cusstomer: ", a.Customer.Name)
                                               })
                                               .ToList();
            return Ok(orders);
        }
        public IActionResult GetSaleOrder(int id)//ajax call
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var order = _dbContext.ARSaleOrders.Include(o => o.Customer).Where(a => a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Customer.Name
                                               })
                                               .FirstOrDefault();
            return Ok(order);
        }
        public IActionResult GetOrder(int id)//ajax call for filling data on delivery challan
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var order = _dbContext.ARSaleOrders.Include(o => o.Customer).Where(a => a.Id == id).FirstOrDefault();
            return Ok(order);
        }
        public IActionResult GetTaxValues(int id)
        {
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var values = appTaxRepo.GetById(id);
            if (values != null)
                return Ok(values);
            else
            {
                return Ok("NotFound");
            }
        }

        //ajax calling for filling ARReceipt Invoices data 
        [HttpGet]
        public IActionResult GetSaleInvoices()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoices = _dbContext.ARInvoices.Include(i => i.Customer).Where(i => i.CompanyId == companyId && i.IsDeleted == false)
                                               .Select(i => new
                                               {
                                                   id = i.Id,
                                                   text = string.Concat("Invoice No: ", i.InvoiceNo, "    Supplier: ", i.Customer.Name)
                                               })
                                               .ToList();
            return Ok(invoices);
        }
        public IActionResult GetSaleInvoice(int id)//ajax call
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = _dbContext.ARInvoices.Include(i => i.Customer).Where(i => i.Id == id)
                                               .Select(i => new
                                               {
                                                   id = i.Id,
                                                   text = i.Customer.Name
                                               })
                                               .FirstOrDefault();
            return Ok(invoice);
        }
        public IActionResult GetInvoiceData(int id)//ajax call for filling data on ARReceipt
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = _dbContext.ARInvoices.Include(i => i.Customer).Where(i => i.Id == id).FirstOrDefault();
            return Ok(invoice);
        }


        [HttpGet]
        public IActionResult GetCustomerBalance(int customerId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var configValues = new ConfigValues(_dbContext);

            //  var TotalSumReceipts = _dbContext.ARReceipts.Where(x => x.Status == "Approved" && x.CustomerId == customerId ).Sum(p => p.GrandTotal);

            var customerDebit = _dbContext.GLVoucherDetails.FromSql("select d.* from GLVouchers m , GLVoucherDetails d where m.Id = d.VoucherId  and m.VoucherType not in ('RCPT', 'CDISADJ') and m.Status = 'Approved' and m.IsDeleted = 0 and m.ModuleName like 'AR/%' and m.ReferenceId = {0}  ", customerId).ToList();

            var customerCredit = _dbContext.GLVoucherDetails.FromSql("select d.* from GLVouchers m , GLVoucherDetails d where m.Id = d.VoucherId  and m.VoucherType in ('RCPT', 'CDISADJ') and m.Status = 'Approved' and m.IsDeleted = 0 and m.ModuleName like 'AR/%' and m.ReferenceId = {0} ", customerId).ToList();


            decimal debit = 0;
            decimal credit = 0;
            if (customerDebit != null)
            {
                debit = customerDebit.Sum(x=>x.Debit);
            }
            if (customerCredit != null)
            {
                credit = customerCredit.Sum(x => x.Credit);
            }
            var customerbalance = debit - credit;
           
                return Ok(customerbalance);
          
        }


        [HttpGet]
        public async Task<IActionResult> GetCustomersCityAndCategoryWise(int CityId, int CategoryId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var customerCategory = await _dbContext.ARSuplierItemsGroup.Include(x => x.ARCustomer).Where(x => x.CategoryId == CategoryId && x.ARCustomer.CityId == CityId && x.ARCustomer.IsActive != false && x.ARCustomer.IsDeleted != true).Select(c => new
            {
                id = c.ARCustomer.Id,
                text = c.ARCustomer.Id + " - " + c.ARCustomer.Name + " - " + c.ARCustomer.Address 
            })
             .OrderBy(a => a.text).ToListAsync();

            return Ok(customerCategory);
        }

        [HttpGet]
        public IActionResult GetSalePersonbyCustomer(int CustomerId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var SalePerson =  _dbContext.ARCustomers.Where(a => a.Id == CustomerId && a.CompanyId == companyId).Select(c => new
            {
                text = c.SalesPersonId
            })
             .OrderBy(a => a.text);
            var SalePersonName = _dbContext.ARSalePerson.Where(a => a.ID == Convert.ToInt32(SalePerson.Select(a => a.text).FirstOrDefault())).FirstOrDefault();
            if (SalePersonName != null)
            {
                return Ok(SalePersonName.Name);
            }
            return Ok("");
            
        }
        public async Task<IActionResult> GetAllCustomersCategories()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var customerCategory = await _dbContext.AppCompanyConfigs.Where(a => a.BaseId == 40 && a.CompanyId == companyId).Select(c => new
            {
                id = c.Id,
                text = c.ConfigValue
            })
            .OrderBy(a => a.text).ToListAsync();
            return Ok(customerCategory);
        }
        //[HttpGet]
        //public IActionResult GetUnpaidInvoicesByCustomerId(int id)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var invoice = _dbContext.ARInvoices.Where(i => i.CustomerId == id && i.CompanyId == companyId && i.IsDeleted == false && i.Status == "Approved").ToList();

        //    return Ok(invoice);
        //}
        [HttpGet]
        public string GetBrand(int id)
        {
            if (id != 0)
            {
                var brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == id);
                if (brand != null)
                {
                    return brand.ConfigValue;
                }
                return "";
            }
            return "";

        }
    }

   
}