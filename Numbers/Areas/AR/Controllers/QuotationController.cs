using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class QuotationController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public QuotationController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<ARQuotation> quotation = new List<ARQuotation>();
            quotation = _dbContext.ARQuotations.Where(x => x.CompanyId == companyId && x.IsActive == true).ToList();
            return View(quotation);
        }

        public IActionResult Create(int? id)
        {
            ARQuotation quotation;
            quotation = _dbContext.ARQuotations.Find(id);
            ARQuotation Arquotations = new ARQuotation();
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            ViewBag.Customer = new SelectList(_dbContext.ARCustomers.OrderBy(c => c.Id).ToList(), "Id", "Name");
            ViewBag.ShipTo = new SelectList(_dbContext.AppCities.OrderBy(c => c.CountryId).ToList(), "Id", "Name");
            ViewBag.ItemsSearch = new SelectList((from i in _dbContext.InvItems.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.IsActive == true)
                                                  select new
                                                  {
                                                      Id = i.Id,
                                                      Name = i.Code + " " + i.Name
                                                  }).ToList(), "Id", "Name");

            ViewBag.CustomerWarranty = new ConfigValues(_dbContext).GetConfigValues("AR", "Warranty", Convert.ToInt32(companyId));
            ViewBag.DeliveryMode = new ConfigValues(_dbContext).GetConfigValues("AR", "Delivery Mode", Convert.ToInt32(companyId));
            ViewBag.PaymentMode = new ConfigValues(_dbContext).GetConfigValues("AR", "Payment Mode", Convert.ToInt32(companyId));
            ViewBag.DeliveryDays = new ConfigValues(_dbContext).GetConfigValues("AR", "DeliveryDays", Convert.ToInt32(companyId));
            ViewBag.SalesPersons = new ConfigValues(_dbContext).GetConfigValues("AP", "SalesPerson", Convert.ToInt32(companyId));
            ViewBag.RateDays = new ConfigValues(_dbContext).GetConfigValues("AR", "Rate Days", Convert.ToInt32(companyId));
            ViewBag.Currency = new SelectList(_dbContext.AppCurrencies.OrderBy(c => c.Id).ToList(), "Id", "Symbol");
            ViewBag.SalesTaxPer = new SelectList(_dbContext.AppTaxes.OrderBy(c => c.Id).ToList(), "Id", "SalesTaxPercentage");


            if (id == null)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Quotation";
                ViewBag.Id = null;
                TempData["No"] = GetMaxQuotation(Convert.ToInt32(companyId));
                Arquotations.ARQuotationsDetailList = new List<ARQuotationDetail>();
                return View(Arquotations);
            }
            else
            {

                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Quotation";
               
                ARQuotation model = _dbContext.ARQuotations.Find(id);
                model.ARQuotationsDetailList = _dbContext.ARQuotationDetail.Where(a => a.QuotaionId == id && a.IsDeleted == false).ToList();

                TempData["No"] = model.QuotationNo;

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ARQuotation quotation, ARQuotationDetail aRQuotationDetail, IFormCollection collection, IFormFile img)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            if (quotation.Id == 0)
            {
                quotation.CompanyId = companyId;
                quotation.CreatedBy = userId;
                quotation.CreatedDate = DateTime.Now;
                quotation.IsDeleted = false;
                quotation.IsActive = true;

                //if AccountId == 0 then we will create auto AccountId
                if (quotation.Id == 0)
                {
                    var configvalue = new ConfigValues(_dbContext);
                    if (img != null && img.Length > 0)
                    {
                        quotation.Photo = UploadFile(img);
                    }
                }
                
                _dbContext.ARQuotations.Add(quotation);
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = "Customer has been saved successfully.";
                var rownber = Convert.ToInt32(collection["rowcounter"]);
                for (int i = 0; i < rownber; i++)
                {
                    var arQuotationDetail = new ARQuotationDetail();
                    arQuotationDetail.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    arQuotationDetail.ItemDescription = collection["ItemDescription"][i];
                    arQuotationDetail.QuotaionId = quotation.Id;
                    arQuotationDetail.UOM = (collection["UOM"][i]);
                    arQuotationDetail.Qty = Convert.ToInt32(collection["Qty"][i]);
                    arQuotationDetail.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    arQuotationDetail.Discount = Convert.ToDecimal(collection["Discount"][i]);
                    arQuotationDetail.DiscuontedRate = Convert.ToDecimal(collection["DiscuontedRate"][i]);
                    arQuotationDetail.Value = Convert.ToDecimal(collection["Value"][i]);
                    arQuotationDetail.PkrValue = Convert.ToDecimal(collection["PkrValue"][i]);
                    arQuotationDetail.Percentage = Convert.ToInt32(collection["stPercentage"][i]);
                    arQuotationDetail.Amount = Convert.ToDecimal(collection["stAmount"][i]);

                    arQuotationDetail.TotalVAlue = Convert.ToDecimal(collection["TotalVAlue"][i]);
                    var dilvrydate = Convert.ToString(collection["DeliveryDate"][i]);
                    arQuotationDetail.DeliveryDate = Convert.ToDateTime(dilvrydate);
                    //  arQuotationDetail.DeliveryDate = Convert.ToDateTime(collection["DeliveryDate"][i]);

                    _dbContext.ARQuotationDetail.Add(arQuotationDetail);
                    await _dbContext.SaveChangesAsync();

                }
            }
            else
            {
                var data = _dbContext.ARQuotations.Find(quotation.Id);
                //data.Id = id;
               data.QuotationNo = quotation.QuotationNo;
                data.QuotationDate = quotation.QuotationDate;
                data.DeliveryMode = Convert.ToInt32(collection["DeliveryMode"]);
                data.Currency = collection["Currency"];
                data.CustomerQuotationDate = quotation.CustomerQuotationDate;
                data.PaymentMode = quotation.PaymentMode;
                data.ExchRate = quotation.ExchRate;

                data.ExchDate = quotation.ExchDate;
                data.ShipTo = quotation.ShipTo;
                data.SalePerson = quotation.SalePerson;
                data.Warranty = quotation.Warranty;
                data.RateDays = quotation.RateDays;
                data.DeliveryDays = quotation.DeliveryDays;
                data.Remarks = quotation.Remarks;

                data.UpdatedBy = userId;
                data.CompanyId = companyId;
                data.UpdatedDate = DateTime.Now;
                if (img != null && img.Length > 0)
                {
                    data.Photo = UploadFile(img);
                }
                else
                {
                    _dbContext.Entry(data).Property(x => x.Photo).IsModified = false;
                }

                var entry = _dbContext.ARQuotations.Update(data);
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = "Quotation has been updated Successfully.";
                var removerow = _dbContext.ARQuotationDetail.Where(a => a.QuotaionId == data.Id).ToList();

                foreach(var item in removerow)
                {
                    ARQuotationDetail detail = _dbContext.ARQuotationDetail.Find(item.Id);
                    _dbContext.ARQuotationDetail.Remove(detail);
                    _dbContext.SaveChanges();

                }
                var rownber = Convert.ToInt32(collection["rowcounter"]);
                for (int i = 0; i < rownber; i++)
                {
                    var arQuotationDetail = new ARQuotationDetail();
                    arQuotationDetail.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    arQuotationDetail.ItemDescription = collection["ItemDescription"][i];
                    arQuotationDetail.QuotaionId = quotation.Id;
                    arQuotationDetail.UOM = (collection["UOM"][i]);
                    arQuotationDetail.Qty = Convert.ToInt32(collection["Qty"][i]);
                    arQuotationDetail.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    arQuotationDetail.Discount = Convert.ToDecimal(collection["Discount"][i]);
                    arQuotationDetail.DiscuontedRate = Convert.ToDecimal(collection["DiscuontedRate"][i]);
                    arQuotationDetail.Value = Convert.ToDecimal(collection["Value"][i]);
                    arQuotationDetail.PkrValue = Convert.ToDecimal(collection["PkrValue"][i]);


                    arQuotationDetail.TotalVAlue = Convert.ToDecimal(collection["TotalVAlue"][i]);
                    var dilvrydate = Convert.ToString(collection["DeliveryDate"][i]);

                      arQuotationDetail.DeliveryDate = Convert.ToDateTime(dilvrydate);

                    _dbContext.ARQuotationDetail.Add(arQuotationDetail);
                    await _dbContext.SaveChangesAsync();

                }
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult GetSaleOrdersByCustomerId(int saleOrderItemId, int counter, int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var invoice = GetAllSaleOrdersByCustomerId(id, skipIds, companyId);
            List<ARInvoiceViewModel> list = new List<ARInvoiceViewModel>();
            foreach (var item in invoice)
            {
                var model = new ARInvoiceViewModel();
                DateTime validDate = item.SaleOrder.SaleOrderDate.AddDays(item.SaleOrder.Validity);
                if (validDate >= DateTime.Now)
                {
                    //model.PurchaseInvoiceId = item.Purchase.Id;
                    model.SalesOrderItemId = item.Id;
                    model.InvoiceNo = item.SaleOrder.SaleOrderNo;
                    model.SaleOrderDate = item.SaleOrder.SaleOrderDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.DueDate = validDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.ItemId = item.ItemId;
                    model.ItemCode = item.Item.Code;
                    model.ItemDescription = item.Item.Name;

                    if (item.BaleId != null)
                    {
                        //model.BaleId = item.Bale.Id;
                        model.BaleType = _dbContext.BaleInformation.FirstOrDefault(x=>x.Id == item.BaleId).BaleType;
                    }
                    else
                    {
                        model.BaleType = "";
                    }

                    model.UOMName = item.Item.UOM.ConfigValue;
                    model.UOM = item.Item.UOM.Id;
                    model.SaleOrderQty = item.Qty - item.DCQty;
                    model.Meters = item.Meters;
                    if (model.SaleOrderQty != 0)
                    {
                        list.Add(model);
                    }

                    //model.Item = item.Item;
                    //model.Remarks = configValues.GetUom(item.Item.Unit);
                    //model.Qty = item.Qty - item.SaleQty;
                    //model.BaleNumber = item.BaleNumber;
                    //model.Rate = item.Rate;
                    //model.BaleType = item.BaleType;
                    //model.GrandTotal = Math.Round(model.Rate * model.Qty, 2);
                    //model.TotalDiscountAmount = item.SaleQty;
                }

            }
            return PartialView("_SaleOrderPopUp", list.ToList());

        }

        public List<ARSaleOrderItem> GetAllSaleOrdersByCustomerId(int id, int[] skipIds, int companyId)
        {
            var purchaseItems = _dbContext.ARSaleOrderItems.Include(i => i.SaleOrder).Include(i => i.Item).ThenInclude(x=>x.UOM)//.Include(x=>x.Bale)
                .Where(i => i.SaleOrder.CompanyId == companyId && i.IsDeleted == false && i.SaleOrder.CustomerId == id
                        && i.Qty > i.SaleQty&&i.SaleOrder.Status== "Approved" && i.IsExpired==false)
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return purchaseItems;
        }


        //ajax to
        public IActionResult getItems(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            var userId = HttpContext.Session.GetString("UserId");

            var Item = _dbContext.InvItems.Where(a => a.Id == id && a.IsDeleted == false).FirstOrDefault();

            var result = (from p in _dbContext.InvItems
                          select new
                          {
                              Id = p.Id,
                              unit = p.Unit,
                              unitName = _dbContext.AppCompanyConfigs.Where(a => a.Id == p.Unit && a.IsDeleted == false).Select(a => a.ConfigValue),
                              name = p.Name,
                              SalesRate = p.SalesRate,
                              IsDeleted = p.IsDeleted
                          }).Where(a => a.Id == id && a.IsDeleted == false).FirstOrDefault();
            return Ok(result);
        }



        //file upload
        public string UploadFile(IFormFile img)
        {
            string fileslist = "";
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\quotation-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(Fstream);
                        var fullPath = "/uploads/quotation-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }

        public async Task<IActionResult> DeleteQuotation(int Id)
        {

            var quotation = await _dbContext.ARQuotations.Where(n => n.Id == Id).FirstAsync();
            if (quotation != null)
            {
                quotation.IsActive = false;
                quotation.IsDeleted = true;
                _dbContext.ARQuotations.Update(quotation);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetSaleTaxPer(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            var userId = HttpContext.Session.GetString("UserId");
            var saletaxper = _dbContext.AppTaxes.Where(a => a.Id == id).ToList();

            //var result = (from p in _dbContext.AppTaxes
            //              select new
            //              {
            //                  Id = p.Id,
            //                  percentage = p.SalesTaxPercentage,
            //                  IsDeleted = p.IsDeleted
            //                  test = p.Name
            //              }).Where(a => a.Id == id && a.IsDeleted == false).ToList();

            var result= _dbContext.AppTaxes.Where(a => a.IsDeleted == false).ToList();
            return Ok(result);
        }

        public IActionResult GetSaleOrderItems(int saleOrderItemId, int counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = GetSaleOrderItem(saleOrderItemId);
            ViewBag.Counter = counter;
            ViewBag.ItemId = item.ItemId;
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            return Ok(item);
        }

        public ARInvoiceViewModel GetSaleOrderItem(int id)
        {
            var item = _dbContext.ARSaleOrderItems
                       .Include(i => i.SaleOrder)
                       .Include(i => i.Item).ThenInclude(x=>x.UOM)
                       .Where(i => i.Id == id && i.IsDeleted == false)
                       .FirstOrDefault();
             
            ARInvoiceViewModel viewModel = new ARInvoiceViewModel();
            //viewModel.SalesOrderItemId = item.Id;
            //viewModel.ItemId = item.ItemId;
            //viewModel.ItemDescription = item.Item.Name;
            //int i = item.Item.Code.IndexOf('-');
            //var code = item.Item.Code.Substring(0,i);
            //viewModel.ItemBrand = _dbContext.InvItemCategories.FirstOrDefault(i => i.Code == code.Trim()).Name;
            //viewModel.UnitName = _dbContext.AppCompanyConfigs.Where(u => u.Id == item.Item.Unit).Select(a => a.ConfigValue).FirstOrDefault();
            //viewModel.UOM = item.Item.Unit;
            //viewModel.BaleNumber = item.BaleNumber;
            //viewModel.Stock = item.Item.StockQty;
            //viewModel.SaleOrderQty = item.Qty;
            //viewModel.Balance = item.Qty - item.SaleQty;



            viewModel.SalesOrderItemId = item.Id;
            viewModel.InvoiceNo = item.SaleOrder.SaleOrderNo;
            viewModel.SaleOrderDate = item.SaleOrder.SaleOrderDate.ToString(Helpers.CommonHelper.DateFormat);
            viewModel.ItemId = item.ItemId;
            viewModel.ItemCode = item.Item.Code;
            viewModel.ItemDescription = item.Item.Name;


            //if (item.BaleId != null)
            //{
            //    viewModel.BaleId = item.Bale.Id;
            //    viewModel.BaleType = item.Bale.BaleType;
            //}
            //else
            //{
            //    viewModel.BaleType = "";
            //}
            viewModel.UOMName = item.Item.UOM.ConfigValue;
            viewModel.UOM = item.Item.UOM.Id;

            viewModel.AvailableStock = item.AvailableStock == null ? 0 : item.AvailableStock;
            viewModel.SaleOrderQty = item.Qty - item.DCQty;
            viewModel.BalanceStock = item.BalanceStock == null ? 0 : item.AvailableStock; 
            viewModel.BaleInformation = _dbContext.BaleInformation.Where(x => x.ItemId == item.ItemId && x.Meters == item.Meters && x.BaleType==item.BaleType && x.UsedFNumber==false).ToList(); 

               
            //viewModel.Balance = item.Item.StockQty - item.SaleQty;


            //viewModel.Qty = item.Qty - item.SaleQty;
            //viewModel.IssueRate = item.Item.AvgRate;
            //viewModel.CostofSales = item.Item.AvgRate * (item.Qty - item.SaleQty);
            //viewModel.Rate = item.Rate;
            //viewModel.Total_ = Math.Round(viewModel.Rate * viewModel.Qty, 2);
            //viewModel.LineTotal = item.LineTotal;
            //viewModel.TaxSlab = item.TaxId;
            //viewModel.SalesTaxAmount = item.TaxAmount;
            return viewModel;
        }
        public int GetMaxQuotation(int companyId)
        {
            int maxQuotation = 1;
            var orders = _dbContext.ARQuotations.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxQuotation = orders.Max(o => Convert.ToInt32(o.QuotationNo));
                return maxQuotation + 1;
            }
            else
            {
                return maxQuotation;
            }
        }

    }
}
