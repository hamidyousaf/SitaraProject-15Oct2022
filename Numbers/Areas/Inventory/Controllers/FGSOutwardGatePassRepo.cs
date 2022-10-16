using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.Inventory.Controllers
{
    internal class FGSOutwardGatePassRepo
    {
        private readonly NumbersDbContext _dbContext;

        public FGSOutwardGatePassRepo(NumbersDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.FGSOutwardGatePasses.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x => x.OGPNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Create(FGSOutwardGatePassViewModel modelRepo)
        {
            try
            {
                //Add Master
                FGSOutwardGatePass model = new FGSOutwardGatePass();
                model.OGPNo = modelRepo.FGSOutwardGatePass.OGPNo;
                model.OGPDate = modelRepo.FGSOutwardGatePass.OGPDate;
                model.WarehouseId = modelRepo.FGSOutwardGatePass.WarehouseId;
                model.CustomerId = modelRepo.FGSOutwardGatePass.CustomerId;
                model.SecondItemCategoryId = modelRepo.FGSOutwardGatePass.SecondItemCategoryId;
                model.FourthItemCategoryId = modelRepo.FGSOutwardGatePass.FourthItemCategoryId;
                model.Remarks = modelRepo.FGSOutwardGatePass.Remarks;
                model.CreatedBy = modelRepo.FGSOutwardGatePass.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                model.IsActive = true;
                model.IsDeleted = false;
                model.Status = "Created";
                model.CompanyId = modelRepo.FGSOutwardGatePass.CompanyId;
                model.Resp_ID = modelRepo.FGSOutwardGatePass.Resp_ID;
                _dbContext.FGSOutwardGatePasses.Add(model);
                _dbContext.SaveChanges();

                //Add Detail
                foreach (var item in modelRepo.FGSOutwardGatePassDetails)
                {
                    FGSOutwardGatePassDetails detail = new FGSOutwardGatePassDetails();
                    detail.FGSOutwardGatePassId = model.Id;
                    detail.PONoId = item.PONoId;
                    detail.ItemId = item.ItemId;
                    detail.BaleType = item.BaleType;
                    detail.BaleId = item.BaleId;
                    detail.MeterBale = item.MeterBale;
                    detail.BaleNo = item.BaleNo;
                    detail.LotNo = item.LotNo;

                    _dbContext.FGSOutwardGatePassDetails.Add(detail);
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
        [HttpPost]
        public async Task<bool> Update(FGSOutwardGatePassViewModel modelRepo)
        {
            try
            {
                FGSOutwardGatePass model = _dbContext.FGSOutwardGatePasses.FirstOrDefault(x => x.Id == modelRepo.FGSOutwardGatePass.Id);
                model.OGPNo = modelRepo.FGSOutwardGatePass.OGPNo;
                model.OGPDate = modelRepo.FGSOutwardGatePass.OGPDate;
                model.WarehouseId = modelRepo.FGSOutwardGatePass.WarehouseId;
                model.CustomerId = modelRepo.FGSOutwardGatePass.CustomerId;
                model.SecondItemCategoryId = modelRepo.FGSOutwardGatePass.SecondItemCategoryId;
                model.FourthItemCategoryId = modelRepo.FGSOutwardGatePass.FourthItemCategoryId;
                model.Remarks = modelRepo.FGSOutwardGatePass.Remarks;
                model.UpdatedBy = modelRepo.FGSOutwardGatePass.UpdatedBy;
                model.UpdatedDate = DateTime.Now;
                _dbContext.FGSOutwardGatePasses.Update(model);
                var existingDetail = _dbContext.FGSOutwardGatePassDetails.Where(x => x.FGSOutwardGatePassId == modelRepo.FGSOutwardGatePass.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.FGSOutwardGatePassDetails.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.FGSOutwardGatePassDetails.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.FGSOutwardGatePassDetails)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        FGSOutwardGatePassDetails Items = new FGSOutwardGatePassDetails();
                        Items.FGSOutwardGatePassId = model.Id;
                        Items.PONoId = detail.PONoId;
                        Items.ItemId = detail.ItemId;
                        Items.BaleType = detail.BaleType;
                        Items.BaleId = detail.BaleId;
                        Items.MeterBale = detail.MeterBale;
                        Items.BaleNo = detail.BaleNo;
                        Items.LotNo = detail.LotNo;
                        await _dbContext.FGSOutwardGatePassDetails.AddAsync(detail);

                    }
                    else   //Updating Records
                    {
                        FGSOutwardGatePassDetails Items = _dbContext.FGSOutwardGatePassDetails.FirstOrDefault(x => x.Id == detail.Id);
                        Items.PONoId = detail.PONoId;
                        Items.ItemId = detail.ItemId;
                        Items.BaleType = detail.BaleType;
                        Items.BaleId = detail.BaleId;
                        Items.MeterBale = detail.MeterBale;
                        Items.BaleNo = detail.BaleNo;
                        Items.LotNo = detail.LotNo;
                        _dbContext.FGSOutwardGatePassDetails.Update(Items);
                    }
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
        public async Task<bool> Delete(int id, int companyId)
        {
            var deleteItem = _dbContext.FGSOutwardGatePasses.Where(v => v.IsDeleted == false && v.Id == id && v.CompanyId == companyId).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.FGSOutwardGatePasses.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}