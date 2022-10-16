using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Greige
{
    public class StackingRepo
    {
        private readonly NumbersDbContext _dbContext;
        public StackingRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<GRStacking> GetAll(int companyId)
        {
            IEnumerable<GRStacking> listRepo = _dbContext.GRStackings.Where(v => v.IsDeleted == false && v.CompanyId == companyId)
                .OrderByDescending(v => v.Id).ToList();
            return listRepo;
        }

        [HttpPost]
        public async Task<bool> Create(GRStackingViewModel modelRepo)
        {
            try
            {
                //Add Master
                GRStacking model = new GRStacking();
                model = modelRepo.GRStacking;
                model.TransactionNo = modelRepo.GRStacking.TransactionNo;
                model.TransactionDate = modelRepo.GRStacking.TransactionDate;
                model.WeavingContractId = modelRepo.GRStacking.WeavingContractId;
                model.PurchaseContractId = modelRepo.GRStacking.PurchaseContractId;
                model.IsActive = true;
                model.IsDeleted = false;
                model.CompanyId = modelRepo.GRStacking.CompanyId;
                model.Resp_Id = modelRepo.GRStacking.Resp_Id;
                model.CreatedBy = modelRepo.GRStacking.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                _dbContext.GRStackings.Add(model);
                _dbContext.SaveChanges();

                //Add Detail
                foreach (var item in modelRepo.GRStackingItem)
                {
                    GRStackingItem detail = new GRStackingItem();
                    detail.GRGRNId = model.Id;
                    detail.WareHouseId = item.WareHouseId;
                    detail.LocationId = item.LocationId;
                    detail.Quantity = item.Quantity;
                    detail.BalQty = item.BalQty;

                    _dbContext.GRStackingItems.Add(detail);
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
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.GRStackings.Where(x=>x.IsDeleted != true).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x=>x.TransactionNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Update(GRStackingViewModel modelRepo)
        {
            try
            {
                var model = _dbContext.GRStackings.FirstOrDefault(x => x.Id == modelRepo.GRStacking.Id);
                //model = modelRepo.GRStacking;
                model.TransactionNo = modelRepo.GRStacking.TransactionNo;
                model.TransactionDate = modelRepo.GRStacking.TransactionDate;
                model.GRNId = modelRepo.GRStacking.GRNId;
                model.WeavingContractId = modelRepo.GRStacking.WeavingContractId;
                model.PurchaseContractId = modelRepo.GRStacking.PurchaseContractId;
                model.VendorId = modelRepo.GRStacking.VendorId;
                model.VendorName = modelRepo.GRStacking.VendorName;
                model.WeavingContractId = modelRepo.GRStacking.WeavingContractId;
                model.PurchaseContractId = modelRepo.GRStacking.PurchaseContractId;
                model.GreigeContractQuality = modelRepo.GRStacking.GreigeContractQuality;
                model.GreigeContractQualityLoom = modelRepo.GRStacking.GreigeContractQualityLoom;
                model.IsActive = true;
                model.IsDeleted = false;
                model.UpdatedBy = modelRepo.GRStacking.UpdatedBy;
                model.UpdatedDate = DateTime.Now;
                _dbContext.GRStackings.Update(model);
                await _dbContext.SaveChangesAsync();
                var existingDetail = _dbContext.GRStackingItems.Where(x => x.GRGRNId == modelRepo.GRStacking.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.GRStackingItem.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        //Handling Balance
                        //var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == saleReturn.IGPId);
                        //igp.BaleBalance = igp.BaleBalance + detail.BalesBalance;
                        //_dbContext.ARInwardGatePass.Update(igp);
                        //----------
                        _dbContext.GRStackingItems.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.GRStackingItem)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        GRStackingItem iGPItems = new GRStackingItem();
                        iGPItems.GRGRNId = modelRepo.GRStacking.Id;
                        iGPItems.WareHouseId = detail.WareHouseId;
                        iGPItems.LocationId = detail.LocationId;
                        iGPItems.Quantity = detail.Quantity;
                        iGPItems.BalQty = detail.BalQty;
                        await _dbContext.GRStackingItems.AddAsync(iGPItems);

                    }
                    else   //Updating Records
                    {
                        var saleReturnItemsData = _dbContext.GRStackingItems.FirstOrDefault(x => x.Id == detail.Id);
                        saleReturnItemsData.WareHouseId = detail.WareHouseId;
                        saleReturnItemsData.LocationId = detail.LocationId;
                        saleReturnItemsData.Quantity = detail.Quantity;
                        saleReturnItemsData.BalQty = detail.BalQty;
                        _dbContext.GRStackingItems.Update(saleReturnItemsData);
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
    }
}
