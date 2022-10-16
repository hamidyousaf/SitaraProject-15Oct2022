using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.Planning.Controllers
{
    [Authorize]
    [Area("Planning")]
    public class InvoiceController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public InvoiceController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            PlanInvoiceViewModel viewModel = new PlanInvoiceViewModel();
            //var repo = new GRNInvoiceRepo(_dbContext);
            if (id != 0)
            {
                //ViewBag.NavbarHeading = "Update Invoice";
                //viewModel = repo.GetById(id);
                //ViewBag.GRNNo = new SelectList(_dbContext.GRGRNS.Where(x => x.Id == viewModel.GRNId).ToList(), "Id", "TransactionNo");
                return View(viewModel);
            }
            ViewBag.NavbarHeading = "Create Processing Invoice";
            //var GRNId = _dbContext.GRGRNInvoices.Where(x => !x.IsDeleted).Select(x => x.GRNId).ToList();
            ////ViewBag.GRNNo = new SelectList(_dbContext.GRGRNS.Where(x => !GRNId.Contains(x.Id) && !x.IsDeleted && x.IsApproved).ToList().OrderByDescending(x => x.Id), "Id", "TransactionNo");
            var productionOrder = _dbContext.GreigeIssuance.Include(x => x.Specification).Where(x => !x.IsDeleted && x.IsApproved && x.CompanyId==companyId).Select(x => new ListOfValue { Id = x.SpecificationId, Name = x.Specification.TransactionNo }).ToList().OrderByDescending(x => x.Id)
                .GroupBy(x => x.Id).Select(a => new ListOfValue { Id = a.Select(b => b.Id).FirstOrDefault(), Name = a.Select(b => b.Name).FirstOrDefault() });
            viewModel.ProductionOrderLOV = new SelectList(productionOrder, "Id", "Name");
            return View(viewModel);
        }
        public IActionResult GetDataFromIssuance(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            PlanInvoiceViewModel viewModel = new PlanInvoiceViewModel();
            var data = _dbContext.GreigeIssuance.Include(x=>x.Specification).Include(x=>x.Vendor)
                .Include(x=>x.GreigeIssuanceDetail).ThenInclude(x=>x.GreigeQuality)
                .Include(x=>x.GreigeIssuanceDetail).ThenInclude(x=>x.ProductionOrderDetail)
                .Where(x=>x.SpecificationId == id && !x.IsDeleted && x.IsApproved && x.CompanyId==companyId).ToList()
                
                
                .GroupBy(x=>x.SpecificationId).Select(x => new {
                    GreigeIssuanceId = x.Select(a=>a.Id).FirstOrDefault(),
                    VendorId = x.Select(a=>a.VendorId).FirstOrDefault(),
                    VendorName = x.Select(a=>a.Vendor.Name).FirstOrDefault(),
                    VendorAddress = x.Select(a=>a.Vendor.Address).FirstOrDefault() ?? "",
                    GreigeIssuanceDetails = x.Select(a =>a.GreigeIssuanceDetail).ToList()
                })
                ;

            return Ok(viewModel);
        }
    }
}
