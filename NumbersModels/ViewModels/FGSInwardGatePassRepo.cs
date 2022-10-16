using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Entity.ViewModels
{
    public class FGSInwardGatePassRepo
    {
        private readonly NumbersDbContext _dbContext;

        public FGSInwardGatePassRepo(NumbersDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.FGSInwardGatePasses.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x => x.IGPNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Create(FGSInwardGatePassViewModel modelRepo)
        {
            try
            {
                //Add Master
                FGSInwardGatePass model = new FGSInwardGatePass();
                model.IGPNo = modelRepo.FGSInwardGatePass.IGPNo;
                model.IGPDate = modelRepo.FGSInwardGatePass.IGPDate;
                model.WarehouseId = modelRepo.FGSInwardGatePass.WarehouseId;

                model.VendorId = modelRepo.FGSInwardGatePass.VendorId;
                model.OGPId = modelRepo.FGSInwardGatePass.OGPId;
                model.DriverName = modelRepo.FGSInwardGatePass.DriverName;
                model.VehicleTypeId = modelRepo.FGSInwardGatePass.VehicleTypeId;
                model.VehicleNo = modelRepo.FGSInwardGatePass.VehicleNo;
                model.Remarks = modelRepo.FGSInwardGatePass.Remarks;
                model.FileAttachment = modelRepo.FGSInwardGatePass.FileAttachment;
                model.CreatedBy = modelRepo.FGSInwardGatePass.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                model.IsActive = true;
                model.IsDeleted = false;
                model.Status = "Created";
                model.CompanyId = modelRepo.FGSInwardGatePass.CompanyId;
                model.Resp_Id = modelRepo.FGSInwardGatePass.Resp_Id;
                _dbContext.FGSInwardGatePasses.Add(model);
                _dbContext.SaveChanges();

                //Add Detail
                foreach (var item in modelRepo.FGSInwardGatePassDetails)
                {
                    FGSInwardGatePassDetail detail = new FGSInwardGatePassDetail();
                    detail.FGSInwardGatePassId = model.Id;
                    detail.ProductionOrderId = item.ProductionOrderId;
                    detail.ItemId = item.ItemId;
                    detail.BaleId = item.BaleId;
                    detail.FGSOutwardGatePassId = item.FGSOutwardGatePassId;
                    detail.FGSOutwardGatePassDetailId = item.FGSOutwardGatePassDetailId;
                    detail.MeterPerBales = item.MeterPerBales;
                    detail.BaleType = item.BaleType;
                    detail.BaleNo = item.BaleNo;
                    detail.LotNo = item.LotNo;

                    _dbContext.FGSInwardGatePassDetails.Add(detail);
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
        public async Task<bool> Update(FGSInwardGatePassViewModel modelRepo)
        {
            try
            {
                FGSInwardGatePass model = _dbContext.FGSInwardGatePasses.FirstOrDefault(x => x.Id == modelRepo.FGSInwardGatePass.Id);
                model.IGPNo = modelRepo.FGSInwardGatePass.IGPNo;
                model.IGPDate = modelRepo.FGSInwardGatePass.IGPDate;
                model.WarehouseId = modelRepo.FGSInwardGatePass.WarehouseId;
                model.VendorId = modelRepo.FGSInwardGatePass.VendorId;
                model.OGPId = modelRepo.FGSInwardGatePass.OGPId;
                model.DriverName = modelRepo.FGSInwardGatePass.DriverName;
                model.VehicleTypeId = modelRepo.FGSInwardGatePass.VehicleTypeId;
                model.VehicleNo = modelRepo.FGSInwardGatePass.VehicleNo;
                model.Remarks = modelRepo.FGSInwardGatePass.Remarks;
                model.FileAttachment = modelRepo.FGSInwardGatePass.FileAttachment;
                model.UpdatedBy = modelRepo.FGSInwardGatePass.UpdatedBy;
                model.UpdatedDate = DateTime.Now;

                _dbContext.FGSInwardGatePasses.Update(model);
                var existingDetail = _dbContext.FGSInwardGatePassDetails.Where(x => x.FGSInwardGatePassId == modelRepo.FGSInwardGatePass.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.FGSInwardGatePassDetails.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.FGSInwardGatePassDetails.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.FGSInwardGatePassDetails)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        FGSInwardGatePassDetail Items = new FGSInwardGatePassDetail();
                        Items.FGSInwardGatePassId = model.Id;
                        Items.ProductionOrderId = detail.ProductionOrderId;
                        Items.ItemId = detail.ItemId;
                        Items.BaleId = detail.BaleId;
                        Items.FGSOutwardGatePassId = detail.FGSOutwardGatePassId;
                        Items.FGSOutwardGatePassDetailId = detail.FGSOutwardGatePassDetailId;
                        Items.MeterPerBales = detail.MeterPerBales;
                        Items.BaleType = detail.BaleType;
                        Items.BaleNo = detail.BaleNo;
                        Items.LotNo = detail.LotNo;
                        await _dbContext.FGSInwardGatePassDetails.AddAsync(detail);

                    }
                    else   //Updating Records
                    {
                        FGSInwardGatePassDetail Items = _dbContext.FGSInwardGatePassDetails.FirstOrDefault(x => x.Id == detail.Id);
                        Items.ProductionOrderId = detail.ProductionOrderId;
                        Items.ItemId = detail.ItemId;
                        Items.BaleId = detail.BaleId;
                        Items.FGSOutwardGatePassId = detail.FGSOutwardGatePassId;
                        Items.FGSOutwardGatePassDetailId = detail.FGSOutwardGatePassDetailId;
                        Items.MeterPerBales = detail.MeterPerBales;
                        Items.BaleType = detail.BaleType;
                        Items.BaleNo = detail.BaleNo;
                        Items.LotNo = detail.LotNo;
                        _dbContext.FGSInwardGatePassDetails.Update(Items);
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
            var deleteItem = _dbContext.FGSInwardGatePasses.Where(v => v.IsDeleted == false && v.Id == id && v.CompanyId == companyId).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.FGSInwardGatePasses.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
