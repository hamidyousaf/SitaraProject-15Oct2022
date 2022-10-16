using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Planning
{
    public class ProductionOrderRepo
    {
        private readonly NumbersDbContext _dbContext;
        public ProductionOrderRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public string Max(int companyId)
        {
            string cyear = DateTime.Now.ToString("yy");
            string transNo = $"00001/{cyear}";
            var result = _dbContext.ProductionOrders.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
            if (result.Count() > 0)
            {
                string[] trans = result.Select(x => x.TransactionNo).LastOrDefault().Split("/");
                var tNo = Convert.ToInt32(trans[0]) + 1; 
                var tYear = trans[1];
               // var tYear = trans[1];
                if (tYear != cyear)
                {
                    return $"00001/{cyear}";
                }
                transNo = $"{tNo.ToString("00000")}/{cyear}";
            }
            return transNo;
        }
        [HttpPost]
        public async Task<bool> Create(ProductionOrderViewModel modelRepo)
        {
            try
            {
                //Add Master
                ProductionOrder model = new ProductionOrder();
                model.TransactionNo = modelRepo.ProductionOrder.TransactionNo;
                model.TransactionDate = modelRepo.ProductionOrder.TransactionDate;
                model.MonthlyPlanningId = modelRepo.ProductionOrder.MonthlyPlanningId;
                //model.SecondItemCategoryId = modelRepo.ProductionOrder.SecondItemCategoryId;
                model.ProcessId = modelRepo.ProductionOrder.ProcessId;
                model.VendorId = modelRepo.ProductionOrder.VendorId;
                model.TotalQty = modelRepo.ProductionOrder.TotalQty;
                model.PlanOf = modelRepo.ProductionOrder.PlanOf;
                model.MonthlyQuantity = modelRepo.ProductionOrder.MonthlyQuantity;
                model.Status = "Created";
                model.CreatedBy = modelRepo.ProductionOrder.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                model.IsActive = true;
                model.IsDeleted = false;
                model.CompanyId = modelRepo.ProductionOrder.CompanyId;
                model.Resp_Id = modelRepo.ProductionOrder.Resp_Id;
                _dbContext.ProductionOrders.Add(model);
                _dbContext.SaveChanges();
                //Add Detail
                foreach (var item in modelRepo.ProductionOrderItems)
                {
                    var suiteMeters = modelRepo.ProductionOrderItems.FirstOrDefault(x=>x.GroupId == item.GroupId).SuitMeters;

                    ProductionOrderItem detail = new ProductionOrderItem();
                    detail.ProductionOrderId = model.Id;
                    detail.FourthItemCategoryId = item.FourthItemCategoryId;
                    detail.ItemCategorySecondId = item.ItemCategorySecondId;
                    detail.ItemId = item.ItemId;
                    detail.TypeId = item.TypeId;
                    detail.MPDetailId = item.MPDetailId;
                    detail.Month = item.Month;
                    detail.VersionId = item.VersionId;
                    detail.ProcessTypeId = item.ProcessTypeId;
                    detail.VersionQuantity = item.VersionQuantity;
                    detail.VersionConversion = item.VersionConversion;
                    detail.ColorVariations = item.ColorVariations;
                    detail.GreigeQualityDesc = item.GreigeQualityDesc;
                    detail.GreigeQualityId = item.GreigeQualityId;
                    detail.Pcs = item.Pcs;
                    detail.SuitMeters = item.SuitMeters == 0 ? suiteMeters: item.SuitMeters;
                    detail.GroupId = item.GroupId;

                    //Handle Balance Quantity
                    PlanMonthlyPlanningItems items = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.Id == detail.MPDetailId);
                    if (items != null && item.SuitMeters != 0)
                    {
                        items.MonthlyFabricConsBalance = items.MonthlyFabricConsBalance - Convert.ToInt32(item.SuitMeters);
                        _dbContext.PlanMonthlyPlanningItems.Update(items);
                        _dbContext.SaveChanges();
                    }
                    //----------
                    
                    _dbContext.ProductionOrderItems.Add(detail);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        public async Task<bool> Delete(int id)
        {
            var productionOrder = _dbContext.ProductionOrders.Include(x=>x.ProductionOrderItems).FirstOrDefault(x => x.Id == id);
            productionOrder.IsDeleted = true;
            productionOrder.IsActive = false;
            var entry = _dbContext.ProductionOrders.Update(productionOrder);
            var data = productionOrder.ProductionOrderItems.GroupBy(x => x.GroupId).Select(x => new
            {
                MPDetailId = x.Select(a => a.MPDetailId).FirstOrDefault(),
                SuitMeters = x.Select(a => a.SuitMeters).FirstOrDefault()
            });
            foreach (var item in data)
            {
                PlanMonthlyPlanningItems items = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.Id == item.MPDetailId);
                if (items != null)
                {
                    items.MonthlyFabricConsBalance = items.MonthlyFabricConsBalance + Convert.ToInt32(item.SuitMeters);
                    _dbContext.PlanMonthlyPlanningItems.Update(items);
                    _dbContext.SaveChanges();
                }

            }
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Approve(int id, string userId)
        {
            try
            {
                var data = _dbContext.ProductionOrders.Find(id);
                data.Status = "Approved";
                data.ApprovedBy = userId;
                //data.TransferToCompany = data.CompanyId == 15 ? 14 : data.CompanyId == 11 ? 1 : 0;
                data.ApprovedDate = DateTime.Now;
                data.IsApproved = true;
                _dbContext.ProductionOrders.Update(data);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(ProductionOrderViewModel modelRepo)
        {
            try
            {
                ProductionOrder model = _dbContext.ProductionOrders.FirstOrDefault(x => x.Id == modelRepo.ProductionOrder.Id);
                model.TransactionNo = modelRepo.ProductionOrder.TransactionNo;
                model.TransactionDate = modelRepo.ProductionOrder.TransactionDate;
                model.MonthlyPlanningId = modelRepo.ProductionOrder.MonthlyPlanningId;
                //model.SecondItemCategoryId = modelRepo.ProductionOrder.SecondItemCategoryId;
                model.ProcessId = modelRepo.ProductionOrder.ProcessId;
                model.VendorId = modelRepo.ProductionOrder.VendorId;
                model.TotalQty = modelRepo.ProductionOrder.TotalQty;
                model.PlanOf = modelRepo.ProductionOrder.PlanOf;
                model.MonthlyQuantity = modelRepo.ProductionOrder.MonthlyQuantity;
                model.UpdatedBy = modelRepo.ProductionOrder.UpdatedBy;
                model.UpdatedDate = DateTime.Now.Date;
                _dbContext.ProductionOrders.Update(model);

                var existingDetail = _dbContext.ProductionOrderItems.Where(x => x.ProductionOrderId == modelRepo.ProductionOrder.Id).ToList();
                //Deleting detail
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.ProductionOrderItems.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        PlanMonthlyPlanningItems items = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.Id == detail.MPDetailId);
                        List<ProductionOrderItem> x = _dbContext.ProductionOrderItems.Where(x => x.GroupId == detail.GroupId && x.ProductionOrderId ==  model.Id).ToList();
                        if (items != null && detail.SuitMeters != 0 && x.Count > 0)
                        {
                            items.MonthlyFabricConsBalance = items.MonthlyFabricConsBalance + Convert.ToInt32(detail.SuitMeters);
                            _dbContext.PlanMonthlyPlanningItems.Update(items);

                        }

                        _dbContext.ProductionOrderItems.RemoveRange(x);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.ProductionOrderItems)
                {
                    var suiteMeters = modelRepo.ProductionOrderItems.FirstOrDefault(x => x.GroupId == detail.GroupId).SuitMeters;
                    if (detail.Id == 0) //Inserting New Records
                    {
                        ProductionOrderItem Items = new ProductionOrderItem();
                        Items.ProductionOrderId = model.Id;
                        Items.FourthItemCategoryId = detail.FourthItemCategoryId;
                        Items.ItemCategorySecondId = detail.ItemCategorySecondId;
                        Items.ItemId = detail.ItemId;
                        Items.TypeId = detail.TypeId;
                        Items.MPDetailId = detail.MPDetailId;
                        Items.Month = detail.Month;
                        Items.VersionId = detail.VersionId;
                        Items.ProcessTypeId = detail.ProcessTypeId;
                        Items.VersionQuantity = detail.VersionQuantity;
                        Items.VersionConversion = detail.VersionConversion;
                        Items.ColorVariations = detail.ColorVariations;
                        Items.GreigeQualityId = detail.GreigeQualityId;
                        Items.GreigeQualityDesc = detail.GreigeQualityDesc;
                        Items.Pcs = detail.Pcs;
                        Items.SuitMeters = detail.SuitMeters == 0 ? suiteMeters : detail.SuitMeters;
                        Items.GroupId = detail.GroupId;
                        //Handle Balance Quantity
                        PlanMonthlyPlanningItems items = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.Id == Items.MPDetailId);
                        if (items != null && detail.SuitMeters != 0)
                        {
                            items.MonthlyFabricConsBalance = items.MonthlyFabricConsBalance - Convert.ToInt32(detail.SuitMeters);
                            _dbContext.PlanMonthlyPlanningItems.Update(items);
                            _dbContext.SaveChanges();
                        }
                        //----------
                        await _dbContext.ProductionOrderItems.AddAsync(Items);
                    }
                    else   //Updating Records
                    {
                        ProductionOrderItem Items = _dbContext.ProductionOrderItems.FirstOrDefault(x => x.Id == detail.Id);

                        //Handle Balance Quantity
                        if (Items.SuitMeters < detail.SuitMeters && detail.SuitMeters != 0)
                        {
                            PlanMonthlyPlanningItems monthlyPlanningItems = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.Id == Items.MPDetailId);
                            monthlyPlanningItems.MonthlyFabricConsBalance = monthlyPlanningItems.MonthlyFabricConsBalance - (detail.SuitMeters - Items.SuitMeters);
                        }
                        if (Items.SuitMeters > detail.SuitMeters && detail.SuitMeters != 0)
                        {
                            PlanMonthlyPlanningItems monthlyPlanningItems = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.Id == Items.MPDetailId);
                            monthlyPlanningItems.MonthlyFabricConsBalance = monthlyPlanningItems.MonthlyFabricConsBalance + (Items.SuitMeters - detail.SuitMeters);
                        }
                        //-----

                        Items.ProductionOrderId = model.Id;
                        Items.FourthItemCategoryId = detail.FourthItemCategoryId;
                        Items.ItemId = detail.ItemId;
                        Items.TypeId = detail.TypeId;
                        Items.MPDetailId = detail.MPDetailId;
                        Items.Month = detail.Month;
                        Items.VersionId = detail.VersionId;
                        Items.VersionConversion = detail.VersionConversion;
                        Items.ProcessTypeId = detail.ProcessTypeId;
                        Items.VersionQuantity = detail.VersionQuantity;
                        Items.ColorVariations = detail.ColorVariations;
                        Items.GreigeQualityId = detail.GreigeQualityId;
                        Items.GreigeQualityDesc = detail.GreigeQualityDesc;
                        Items.Pcs = detail.Pcs;
                        Items.SuitMeters = detail.SuitMeters == 0 ? suiteMeters : detail.SuitMeters;
                        Items.GroupId = detail.GroupId;
                        _dbContext.ProductionOrderItems.Update(Items);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
    }
}
