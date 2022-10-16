using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;

namespace Numbers.Areas.AP.Controllers
{
    [Authorize]
    [Area("AP")]
    public class ApiController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ApiController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult GetSuppliers(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var suppliers = _dbContext.APSuppliers.Where(
                                                (a => a.Name.Contains(q) && a.CompanyId == companyId && a.IsActive
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Id, " - ", a.Name)
                                               })
                                               .OrderBy(a => a.id)
                                               .ToList();
            return Ok(suppliers);
        }
        [HttpGet]
        public IActionResult GetSupplier(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var supplier = _dbContext.APSuppliers.Where(a => a.Id == id && a.CompanyId == companyId).FirstOrDefault();
            return Ok(supplier);
        }


        [HttpGet]
        public IActionResult GetIGP(int vendorId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var IGP = new SelectList(_dbContext.APIGP.Where(x => x.VendorId == vendorId && x.CompanyId == companyId && x.IsDeleted == false && x.IsApproved && x.IsIRN == false).ToList(), "IGP", "IGP");
            return Ok(IGP);
        }
        [HttpGet]
        public IActionResult GetIGPID(int vendorId, int igp)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var IGP = new SelectList(_dbContext.APIGP.Where(x => x.VendorId == vendorId && x.CompanyId == companyId && x.IsDeleted == false && x.IsApproved && (x.IsIRN == false || x.IGP == igp)).ToList(), "IGP", "IGP");
            return Ok(IGP);
        }
        [HttpGet]
        public IActionResult GetCostCenter()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
            return Ok(CostCenter);
        }

        [HttpGet]
        public IActionResult GetPurchaseInvoices()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoices = _dbContext.APPurchases.Include(i => i.Supplier).Where(i => i.CompanyId == companyId && i.IsDeleted == false)
                                               .Select(i => new
                                               {
                                                   id = i.Id,
                                                   text = string.Concat("Invoice No: ", i.PurchaseNo, "    Supplier: ", i.Supplier.Name)
                                               })
                                               .ToList();
            return Ok(invoices);
        }
        public IActionResult GetPurchaseInvoice(int id)//ajax call
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = _dbContext.APPurchases.Include(i => i.Supplier).Where(i => i.Id == id)
                                               .Select(i => new
                                               {
                                                   id = i.Id,
                                                   text = i.Supplier.Name
                                               })
                                               .FirstOrDefault();
            return Ok(invoice);
        }
        public IActionResult GetGRNBalanceQty(int id)//ajax call
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = _dbContext.APIRNDetails.Where(x => x.Id == id)
                                               .FirstOrDefault();
            return Ok(invoice);
        }
        public IActionResult GetPRBalanceQty(int id)//ajax call
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = _dbContext.APGRNItem.Where(x => x.Id == id)
                                               .FirstOrDefault();
            return Ok(invoice);
        }
        public IActionResult GetInvoiceData(int id)//ajax call for filling data on APSupplierPayment
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = _dbContext.APPurchases.Include(i => i.Supplier).Where(i => i.Id == id).FirstOrDefault();
            return Ok(invoice);
        }

        [HttpGet]
        public IActionResult GetUnpaidInvoicesBySupplierId(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = _dbContext.APPurchases.Where(i => i.SupplierId == id && i.CompanyId == companyId && i.IsDeleted == false && i.Status == "Approved").ToList();
            return Ok(invoice);
        }
        [HttpGet]
        public IActionResult GetDepartments()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Departments = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            return Ok(Departments);
        }
        public IActionResult GetCities()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var cities = _dbContext.AppCities
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(cities);
        }

        public IActionResult GetCountries()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var cities = _dbContext.AppCountries
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(cities);
        }
        [HttpGet]
        public IActionResult GetDepartmentsForReport()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Departments = _dbContext.GLDivision.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();

            return Ok(Departments);
        }
        public IActionResult GetCostCenterForReport()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var CostCenters = _dbContext.CostCenter.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Description
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();

            return Ok(CostCenters);
        }

        public IActionResult GetSuppliersForReport()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var suppliers = _dbContext.APSuppliers.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(suppliers);
        }
        public IActionResult GetItems()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Items = _dbContext.InvItems.Where(a => a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name)
                                               });
            return Ok(Items);
        }
        [HttpGet]
        public IActionResult GetSubDepartments(int id)
        {
            var subDeparments = new SelectList(_dbContext.GLSubDivision.Where(x => x.GLDivisionId == id && x.IsDeleted == false).ToList(), "Id", "Name");
            return Ok(subDeparments);
        }
        [HttpGet]
        public IActionResult GetCostsCenter(int id)
        {
            var subDeparments = new SelectList(_dbContext.CostCenter.Where(x => x.SubDivisionId == id && x.IsDeleted == false).ToList(), "Id", "Description");
            return Ok(subDeparments);
        }
        [HttpGet]
        public IActionResult GetIGPLOV(int vendorId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;

            int[] igpNo = _dbContext.APIRN.Where(x => x.IsDeleted != true && x.CompanyId == companyId).Select(x => x.IGPNo).ToArray();
            var IGP = new SelectList(_dbContext.APIGP.Where(x => x.VendorId == vendorId && x.Resp_ID == resp_Id && !igpNo.Contains(x.IGP) && x.CompanyId == companyId && x.IsDeleted == false && x.IsApproved && x.IsIRN == false).ToList(), "IGP", "IGP");

            //var IGPNo = (from APIGP in _dbContext.APIGP.Where(x => x.IsDeleted != true)
            //             join APIGPDetail in _dbContext.APIGPDetails.ToList() on APIGP.Id equals APIGPDetail.IGPId
            //             where APIGP.VendorId == vendorId && APIGPDetail.RCDQty == 0
            //             select APIGP).Select(x => x.IGP).ToList();
            //var IGP = new SelectList(_dbContext.APIGP.Where(x => IGPNo.Contains(x.IGP) && x.CompanyId == companyId && x.IsDeleted == false && x.IsApproved).ToList(), "IGP", "IGP");

            return Ok(IGP);


        }
        [HttpGet]
        public IActionResult GetIRNLOV(int vendorId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            int[] igpNo = _dbContext.APOGP.Where(x => x.IsDeleted != true).Select(x => x.IRNId).ToArray();
            var IGP = new SelectList(_dbContext.APIRNDetails.Include(x=>x.IRN).Where(x => x.IRN.VendorID == vendorId && x.IRN.CompanyId == companyId && x.IsOGPCreated == false && x.OGPBalQty > 0 && x.IRN.IsDeleted == false && x.IRN.IsApproved && x.IRN.IsOGP == false).GroupBy(x => new { x.IRN.Id, x.IRN.IRNNo })
                .Select(x=> new ListOfValue { Id = x.Select(x => x.IRN.Id).FirstOrDefault(), IRNNo = x.Select(x => x.IRN.IRNNo).FirstOrDefault() }).ToList(), "Id", "IRNNo");

            //var IGPNo = (from APIGP in _dbContext.APIGP.Where(x => x.IsDeleted != true)
            //             join APIGPDetail in _dbContext.APIGPDetails.ToList() on APIGP.Id equals APIGPDetail.IGPId
            //             where APIGP.VendorId == vendorId && APIGPDetail.RCDQty == 0
            //             select APIGP).Select(x => x.IGP).ToList();
            //var IGP = new SelectList(_dbContext.APIGP.Where(x => IGPNo.Contains(x.IGP) && x.CompanyId == companyId && x.IsDeleted == false && x.IsApproved).ToList(), "IGP", "IGP");

            return Ok(IGP);

        }
    }
}