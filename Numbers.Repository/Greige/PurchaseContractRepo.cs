using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Greige
{
    public class PurchaseContractRepo
    {
        private readonly NumbersDbContext _dbContext;
        public PurchaseContractRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        public async Task<bool> Create(GRPurchaseContract model)
        {
            try
            {
                var item = new GRPurchaseContract();
                item = model;
                item.CompanyId = model.CompanyId;
                item.IsDeleted = false;
                item.IsActive = true;
                item.CreatedBy = model.CreatedBy;
                item.CreatedDate = DateTime.Now;
                item.BalanceContractQty = model.ContractQuantity;
                item.Status = "Created";
                item.GRRequisitionId = model.GRRequisitionId;

                // handle balance
                GRGriegeRequisitionDetails details = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.GRRequisitionId == item.GRRequisitionId && x.GriegeQualityId == item.PurchaseGRQualityId);
                details.BalanceQty = details.BalanceQty - Convert.ToInt32(item.ContractQuantity);
                _dbContext.GRGriegeRequisitionDetails.Update(details);
                //---

                _dbContext.GRPurchaseContract.Add(item);
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
        [HttpPost]
        public async Task<bool> Update(GRPurchaseContract model)
        {
            try
            {
                var obj = _dbContext.GRPurchaseContract.FirstOrDefault(x=>x.Id == model.Id);
                // Handle Balance Quantity
                GRGriegeRequisitionDetails requisition = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.GRRequisitionId == model.GRRequisitionId);
                if (obj.ContractQuantity < model.ContractQuantity)
                {
                    requisition.BalanceQty = requisition.BalanceQty - (Convert.ToInt32(model.ContractQuantity) - Convert.ToInt32(obj.ContractQuantity));
                }
                if (obj.ContractQuantity > model.ContractQuantity)
                {
                    requisition.BalanceQty = requisition.BalanceQty + (Convert.ToInt32(obj.ContractQuantity) - Convert.ToInt32(model.ContractQuantity));
                }
                ////-----
                obj.ContractNo = model.ContractNo;
                obj.ContractDate = model.ContractDate;
                obj.DeliveryDate = model.DeliveryDate;
                obj.VendorId = model.VendorId;
                obj.PurchaseGRQualityId = model.PurchaseGRQualityId;
                obj.ContractGRQualityId = model.ContractGRQualityId;
                obj.ContractQuantity = model.ContractQuantity;
                obj.BalanceContractQty = model.ContractQuantity;
                obj.RatePerMeter = model.RatePerMeter;
                obj.ExSalesTaxAmount = model.ExSalesTaxAmount;
                obj.SalesTaxId = model.SalesTaxId;
                obj.SalesTaxAmount = model.SalesTaxAmount;
                obj.InSalesTaxAmount = model.InSalesTaxAmount;
                obj.CompanyId = model.CompanyId;
                obj.UpdatedBy = model.UpdatedBy;
                obj.UpdatedDate = DateTime.Now;
                obj.IsDeleted = false;
                obj.IsActive = true;
                obj.GRRequisitionId = model.GRRequisitionId;
                var entry = _dbContext.GRPurchaseContract.Update(obj);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
        public async Task<bool> Delete(int id)
        {
            var model = _dbContext.GRPurchaseContract.Find(id);
            model.IsDeleted = true;
            model.IsActive = false;
            var entry = _dbContext.GRPurchaseContract.Update(model);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
