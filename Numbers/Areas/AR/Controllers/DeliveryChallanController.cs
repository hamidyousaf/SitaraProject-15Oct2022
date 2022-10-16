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
using Numbers.Repository.AR;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;
using Numbers.Repository.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class DeliveryChallanController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public DeliveryChallanController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var deliveryChallanRepo = new DeliveryChallanRepo(_dbContext);

            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=OGPBasePrint&cId=", companyId, "&id=");


            IEnumerable<ARDeliveryChallan> list = deliveryChallanRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Delivery Challans";
            return View(list);

        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var deliveryChallanRepo = new DeliveryChallanRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.ItemsSearch = new SelectList(_dbContext.InvItems.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.IsActive == true).ToList(), "Id", "Name");
            ViewBag.SalesCategory = new SelectList(_dbContext.ARCustomers.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.IsActive == true).ToList(), "Id", "Name");
            ViewBag.Customer = new SelectList(_dbContext.ARCustomers.Where(a => /*a.CompanyId == companyId &&*/ a.IsDeleted != true && a.IsActive != false).ToList(), "Id", "Name");
            ViewBag.VehicleType = configValues.GetConfigValues("AR", "Vehicle Type", companyId);
            ViewBag.TransportCompany = configValues.GetConfigValues("AR", "Transport Company", companyId);
            ViewBag.Counter = 0;
            ViewBag.SOId = deliveryChallanRepo.SaleOrderNo();
            //var bales = _dbContext.BaleInformation.ToList();
            //ViewBag.Bales = new SelectList(_dbContext.BaleInformation.Where(x=>x.UsedFNumber != true).ToList()), "Id", "DesignCode");

            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Delivery Challan";
                ViewBag.Model = deliveryChallanRepo.GetSOItems();
                //TempData["DCNo"] = deliveryChallanRepo.DeliveryChallanCountNo(companyId);
                return View(new ARDeliveryChallanViewModel());
            }
            else
            {
                ARDeliveryChallanViewModel model = deliveryChallanRepo.GetById(id);
                model.ARDeliveryChallanItemList = _dbContext.ARDeliveryChallanItems
                    .Include(x=>x.ARSaleOrderItem).ThenInclude(x=>x.SaleOrder)
                    .Include(x=>x.Item)
                    .Where(u => Convert.ToInt32(u.DeliveryChallanId) == id).ToList();


                ViewBag.Model = deliveryChallanRepo.GetSOItems();
                ViewBag.DCItems = deliveryChallanRepo.GetDCItems(id);

                if (model.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                }
                foreach (var item in model.ARDeliveryChallanItemList)
                {
                    int i = item.Item.Code.IndexOf('-');
                    var code = item.Item.Code.Substring(0, i);
                    item.ItemBrand = _dbContext.InvItemCategories.FirstOrDefault(i => i.Code == code.Trim()).Name;
                    
                }
                
                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(ARDeliveryChallanViewModel model, IFormCollection collection, IFormFile Attachment)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var deliveryChallanRepo = new DeliveryChallanRepo(_dbContext);
            /*if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {*/
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId; 
                model.ResponsibilityId = resp_Id; 
                bool isSuccess = await deliveryChallanRepo.Create(model, collection, Attachment, companyId, userId);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Delivery Challan No. {0} has been created successfully.", deliveryChallanRepo.MaxDeliveryChallan(companyId));
                    //for (int i = 0; i < Convert.ToInt32(collection["rowcounter"]); i++)
                    //{
                    //    var deliveryChallanItem = new ARDeliveryChallanItem();
                    //    deliveryChallanItem.Id = Convert.ToInt32(collection["Id"][i]);
                    //    deliveryChallanItem.SaleOrderNo = Convert.ToDecimal(collection["SaleOrderNo"][i]);
                    //    deliveryChallanItem.ItemDescription = collection["ItemDescription"][i];
                    //    deliveryChallanItem.UOM = collection["Uom"][i];
                    //    //deliveryChallanItem.StoreDetail = Convert.ToInt32(collection["Store"][i]);
                    //    //deliveryChallanItem.SaleOrder = collection["SaleOrder"][i];
                    //    deliveryChallanItem.DCBalance = Convert.ToDecimal(collection["DCBalance"][i]);
                    //    deliveryChallanItem.Qty = Convert.ToDecimal(collection["Qty"][i]);
                    //    _dbContext.ARDeliveryChallanItems.Add(deliveryChallanItem);
                    //    await _dbContext.SaveChangesAsync();
                    //}
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.ResponsibilityId = resp_Id;
                bool isSuccess = await deliveryChallanRepo.Update(model, collection, Attachment, companyId, userId);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Delivery Challan No. {0} has been updated successfully.", model.DCNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        //ajax call
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
                              IsDeleted = p.IsDeleted
                          }).Where(a => a.Id == id && a.IsDeleted == false).FirstOrDefault();
            return Ok(result);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var deliveryChallanRepo = new DeliveryChallanRepo(_dbContext);
            bool isSuccess = await deliveryChallanRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Delivery Challan has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ApproveDeliveryChallan(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var deliveryChallanRepo = new DeliveryChallanRepo(_dbContext);
            bool isSuccess = await deliveryChallanRepo.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Delivery Challan has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetDC()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
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
                var searchValueDCNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchValueDCCus = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchValueDCDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchValueTranComp = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchValueBuiltyNo = Request.Form["columns[4][search][value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var DCData = (from DC in _dbContext.ARDeliveryChallans.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select DC);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    if (sortColumn == "CustomerName")
                    {
                        DCData = DCData.OrderBy("CustomerId" + " " + sortColumnDirection);
                    }
                    else
                    {
                        DCData = DCData.OrderBy(sortColumn + " " + sortColumnDirection);
                    }
                }
                DCData = !string.IsNullOrEmpty(searchValueDCNo) ? DCData.Where(m => m.DCNo.ToString().ToLower().Contains(searchValueDCNo.ToLower())) : DCData;
                DCData = !string.IsNullOrEmpty(searchValueDCCus) ? DCData.Where(m => m.Customer.Name.ToString().ToLower().Contains(searchValueDCCus.ToLower())) : DCData;
                DCData = !string.IsNullOrEmpty(searchValueDCDate) ? DCData.Where(m => m.DCDate.ToString(Helpers.CommonHelper.DateFormat).ToLower().Contains(searchValueDCDate.ToLower())) : DCData;
                DCData = !string.IsNullOrEmpty(searchValueTranComp) ? DCData.Where(m => (m.TransportCompany != null ? m.TransportCompany.ToString().ToLower().Contains(searchValueTranComp.ToLower()) : false)) : DCData;
                DCData = !string.IsNullOrEmpty(searchValueBuiltyNo) ? DCData.Where(m => m.BuiltyNo.ToString().Contains(searchValueBuiltyNo)) : DCData;
                recordsTotal = DCData.Count();
                var data = DCData.ToList();
                if (pageSize == -1)
                {
                    data = DCData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = DCData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARDeliveryChallanViewModel> Details = new List<ARDeliveryChallanViewModel>();
                foreach (var grp in data)
                {
                    ARDeliveryChallanViewModel aRDeliveryChallanViewModel = new ARDeliveryChallanViewModel();
                    aRDeliveryChallanViewModel.DDate = grp.DCDate.ToString(Helpers.CommonHelper.DateFormat);
                    aRDeliveryChallanViewModel.CustomerName = (_dbContext.ARCustomers.Where(x => x.Id == grp.CustomerId).FirstOrDefault().Name);
                    aRDeliveryChallanViewModel.TransportCompany = (_dbContext.AppCompanyConfigs.Where(x => x.Id ==Convert.ToInt32( grp.TransportCompany)).FirstOrDefault().ConfigValue);
                    aRDeliveryChallanViewModel.ARDeliveryChallans = grp;
                    aRDeliveryChallanViewModel.ARDeliveryChallans.Approve = approve;
                    aRDeliveryChallanViewModel.ARDeliveryChallans.Unapprove = unApprove;
                    Details.Add(aRDeliveryChallanViewModel);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //[HttpPost]
        //public IActionResult UpdateBuiltyNumber(int id, string builtyNumber, DateTime builtyDate)
        //{
        //    var model = _dbContext.ARDeliveryChallans.Find(id);
        //    try
        //    {
        //        model.BuiltyNo = builtyNumber;
        //        model.BuiltyDate = builtyDate.Date;
        //        _dbContext.ARDeliveryChallans.Update(model);
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> UpdateBuiltyNumber(int id, string builtyNumber, DateTime builtyDate)
        {
            var model = _dbContext.ARDeliveryChallans.Find(id);
            try
            {
               
                model.BuiltyNo = builtyNumber;
                model.BuiltyDate = builtyDate.Date;
                _dbContext.ARDeliveryChallans.Update(model);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {

            }
            finally{

                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var customer = _dbContext.ARCustomers.Where(m => m.Id == model.CustomerId).FirstOrDefault();
                string userId = HttpContext.Session.GetString("UserId");
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                var InvController = new InvoiceController(_dbContext);
                List<ARInvoiceViewModel> modelDc = InvController.GetSaleOrdersByCustomerIdForAutoInv(model.CustomerId,model.DCNo,companyId);
                List<ARInvoiceViewModel> list = new List<ARInvoiceViewModel>();
                foreach (var item in modelDc)
                {
                    list.Add(InvController.GetSaleOrderItemsForAutoInv(item.SalesOrderItemId, id, companyId));
                }
                //var dcItems = _dbContext.ARDeliveryChallanItems.Where(x => x.DeliveryChallanId == id).ToList();
               
                var invoice = new ARInvoice();
                list = list.Where(x => x.DCNo  == Convert.ToString(model.DCNo)).ToList();
                invoice.Id = model.Id;
               // invoice.InvoiceNo = TransactionNo;
                invoice.InvoiceDate = model.DCDate;
                invoice.InvoiceDueDate = model.DCDate;
               // invoice.WareHouseId = model.WareHouseId;
                invoice.WareHouseId = 0;
                invoice.CustomerId = model.CustomerId;
                invoice.SalesPersonId = customer.SalesPersonId;
                invoice.ReferenceNo = "";
               // invoice.CustomerPONo = model.CustomerPONo;
                invoice.CustomerPONo = "";
                //invoice.OGPNo = model.OGPNo;
                invoice.OGPNo = "";
                invoice.Vehicle = model.VehicleNo;
                invoice.Remarks = model.Remarks;
                //invoice.Currency = model.Currency;
                //invoice.CurrencyExchangeRate = model.CurrencyExchangeRate;
                invoice.TransactionType = "Sale";
                invoice.InvoiceType = "INV";
                invoice.Total = Convert.ToDecimal(list.Sum(x=>x.Total_));
               // invoice.DiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
                invoice.SalesTaxAmount = Convert.ToDecimal(0);
                invoice.ExciseTaxAmount = Convert.ToDecimal(0);
                invoice.GrandTotal = Convert.ToDecimal(list.Sum(x => x.Total_));
                invoice.CreatedBy = userId;
                invoice.CreatedDate = DateTime.Now;
                invoice.CompanyId = companyId;
                invoice.ResponsibilityId = resp_Id;
                invoice.Status = "Created";
                invoice.CostCenter = 0;

                var items = new ARInvoiceItem();
                
               var invoiceId = await CreateInvoice(invoice, list);
                if (invoiceId!=0)
                {
                  await  InvController.ApproveAuto(invoiceId,companyId,userId);
                }
            }
           


            return Ok();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewBag.EntityState = "Detail";
            ViewBag.NavbarHeading = "Detail Delivery Challan";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            var configValuesVahicle = new ConfigValues(_dbContext);
            var Vahicle = configValuesVahicle.GetConfigValues("AR", "Vehicle Type", companyId);
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=DeliveryChallan&cId=", companyId, "&id={0}");

            var model = _dbContext.ARDeliveryChallans
                .Include(p => p.ARDeliveryChallanItems)
                .ThenInclude(p => p.Item)
                .Include(p => p.ARDeliveryChallanItems)
                .ThenInclude(p => p.ARSaleOrderItem)
                .ThenInclude(p=>p.SaleOrder)
                .Include(p => p.Customer)
                .Where(p => p.Id == id).FirstOrDefault();
            ViewBag.Vahicle = Vahicle.FirstOrDefault(p => p.Value == model.VehicleType.ToString()).Text;
            foreach (var item in model.ARDeliveryChallanItems)
            {
                int i = item.Item.Code.IndexOf('-');
                var code = item.Item.Code.Substring(0, i);
                item.ItemBrand = _dbContext.InvItemCategories.FirstOrDefault(i => i.Code == code.Trim()).Name;
            }
            if(model.TransportCompany!=null && model.TransportCompany != "")
            {
            model.TransportCompany = _dbContext.AppCompanyConfigs.Where(x => x.Id == Convert.ToInt32( model.TransportCompany)).FirstOrDefault().ConfigValue;
            }
            ViewBag.TotalQty = model.ARDeliveryChallanItems.Sum(x=>x.Qty);
            return View(model);
        }
        [HttpGet]
        public string GetBaleType(int? baleId)
        {
            if (baleId != 0)
            {
                var model = _dbContext.BaleInformation.Find(baleId);
                if (model != null)
                {
                    return model.BaleType;
                }
                return "NaN";
            }
            return "";
        }

        [HttpGet]
        public string GetBaleNo(int? baleId)
        {
            if (baleId != 0)
            {
                var model = _dbContext.BaleInformation.Find(baleId);
                if (model != null)
                {
                    return model.BaleNumber;
                }
                return "NaN";
            }
            return "";
        }
        public async Task<int> CreateInvoice(ARInvoice model, List<ARInvoiceViewModel> collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;

            var customer = _dbContext.ARCustomers.Where(m => m.Id == model.CustomerId).FirstOrDefault();
            var TransactionNo = 1;
            var max = _dbContext.ARInvoices.Where(x => x.CompanyId == companyId).ToList();
            if (max.Count != 0)
            {
                TransactionNo = _dbContext.ARInvoices.Where(x => x.CompanyId == companyId).Max(x => x.InvoiceNo) + 1;
            }
            if (collection !=null)
            {
                ARInvoice invoice = new ARInvoice();
                try
                {

                //Master Table data
                //invoice.Id = model.Id;
                invoice.InvoiceNo = TransactionNo;
                invoice.InvoiceDate = model.InvoiceDate;
                invoice.InvoiceDueDate = model.InvoiceDueDate;
                invoice.WareHouseId = model.WareHouseId;
                invoice.CustomerId = model.CustomerId;
                invoice.SalesPersonId = customer.SalesPersonId;
                invoice.ReferenceNo = model.ReferenceNo;
                invoice.CustomerPONo = model.CustomerPONo;
                invoice.OGPNo = model.OGPNo;
                invoice.Vehicle = model.Vehicle;
                invoice.Remarks = model.Remarks;
                invoice.Currency = model.Currency;
                invoice.CurrencyExchangeRate = model.CurrencyExchangeRate;
                invoice.TransactionType = "Sale";
                invoice.InvoiceType = "INV";
                invoice.Total = Convert.ToDecimal(model.Total);
                invoice.DiscountAmount = Convert.ToDecimal(model.DiscountAmount);
                invoice.SalesTaxAmount = Convert.ToDecimal(model.SalesTaxAmount);
                invoice.ExciseTaxAmount = Convert.ToDecimal(model.ExciseTaxAmount);
                invoice.GrandTotal = Convert.ToDecimal(model.Total);
                invoice.CreatedBy = userId;
                invoice.CreatedDate = DateTime.Now;
                invoice.CompanyId = companyId;
                invoice.ResponsibilityId = resp_Id;
                invoice.Status = "Created";
                invoice.CostCenter = model.CostCenter;

                _dbContext.ARInvoices.Add(invoice);
                await _dbContext.SaveChangesAsync();

                    foreach (var item in collection)
                    {
                        var invoiceItem = new ARInvoiceItem();
                        invoiceItem.InvoiceId = invoice.Id;
                        invoiceItem.DCNo = item.DCNo;
                        invoiceItem.ItemId = item.ItemId;
                        invoiceItem.Qty = item.Qty;
                        invoiceItem.AvgRate = _dbContext.InvItems.Find(invoiceItem.ItemId).AvgRate;
                        invoiceItem.Rate = item.Rate;

                        invoiceItem.Meters = item.Meters;
                        invoiceItem.TotalMeter = item.TotalMeter;
                        invoiceItem.TotalMeterAmount = item.TotalMeterAmount;

                        invoiceItem.Stock = item.Stock;
                        invoiceItem.IssueRate = item.IssueRate;
                        invoiceItem.CostofSales = Convert.ToDecimal(item.IssueRate) * Convert.ToDecimal(item.Qty);
                        invoiceItem.Total = Convert.ToDecimal(item.Total_);
                        //invoiceItem.TaxSlab = Convert.ToInt32(collection["TaxSlab"][i]);
                        invoiceItem.DiscountPercentage = Convert.ToDecimal(item.DiscountPercentage);
                        invoiceItem.DiscountAmount = Convert.ToDecimal(item.DiscountAmount);
                        invoiceItem.SalesTaxPercentage = Convert.ToDecimal(item.SalesTaxPercentage);
                        invoiceItem.SalesTaxAmount = Convert.ToDecimal(item.SalesTaxAmount);
                        invoiceItem.ExciseTaxPercentage = Convert.ToDecimal(item.ExciseTaxPercentage);
                        invoiceItem.ExciseTaxAmount = Convert.ToDecimal(item.ExciseTaxAmount);
                        var total = Convert.ToDecimal(item.SalesTaxAmount) + Convert.ToDecimal(item.Total_);
                        invoiceItem.LineTotal = total;
                        invoiceItem.Remarks = item.Remarks;
                        invoiceItem.DCItemId = Convert.ToInt32(item.DCItemId);
                        invoiceItem.SalesOrderItemId = Convert.ToInt32(item.SalesOrderItemId);
                        invoiceItem.DetailCostCenter = Convert.ToInt32(item.DetailCostCenter);
                        if (invoiceItem.DetailCostCenter == 0)
                        {
                            invoiceItem.DetailCostCenter = invoice.CostCenter;
                        }
                        _dbContext.ARInvoiceItems.Add(invoiceItem);
                        await _dbContext.SaveChangesAsync();

                        var deliverItems = _dbContext.ARDeliveryChallanItems.Where(x => x.Id == invoiceItem.DCItemId).FirstOrDefault();
                        deliverItems.SaleQty = deliverItems.SaleQty + invoiceItem.Qty;
                        _dbContext.ARDeliveryChallanItems.Update(deliverItems);
                        await _dbContext.SaveChangesAsync();
                    }

                    return invoice.Id;
                }
                catch (Exception e)
                {

                }

                TempData["error"] = "false";
                TempData["message"] = string.Format("Invoice No. {0} has been created successfully", max);
                return invoice.Id;
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "No any Invoice has been Created. It must contain atleast one item";
                return 0;
            }
        }

    }
}