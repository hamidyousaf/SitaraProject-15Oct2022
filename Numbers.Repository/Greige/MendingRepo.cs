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
    public class MendingRepo
    {
        private readonly NumbersDbContext _dbContext;
        public MendingRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.GRMending.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x => x.TransactionNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Create(GRMendingViewModel modelRepo)
        {
            try
            {
                //Add Master
                GRMending model = new GRMending();
                model.TransactionNo = modelRepo.GRMending.TransactionNo;
                model.TransactionDate = modelRepo.GRMending.TransactionDate;
                model.GRIGPId = modelRepo.GRMending.GRIGPId;
                model.ReceivedQuantity = modelRepo.GRMending.ReceivedQuantity;
                model.LotNo = modelRepo.GRMending.LotNo;
                model.TotalRecievedQuantity = modelRepo.GRMending.TotalRecievedQuantity;
                model.TotalRejectedQuantity = modelRepo.GRMending.TotalRejectedQuantity;
                model.TotalFreshQuantity = modelRepo.GRMending.TotalFreshQuantity;
                model.TotalGradedQuantity = modelRepo.GRMending.TotalGradedQuantity;
                model.IsActive = true;
                model.IsDeleted = false;
                model.CompanyId = modelRepo.GRMending.CompanyId;
                model.Resp_Id = modelRepo.GRMending.Resp_Id;
                model.CreatedBy = modelRepo.GRMending.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                _dbContext.GRMending.Add(model);

                var x = modelRepo.GRMendingDetail.Sum(x => x.RejectedQuantity);
                if (x > 0)
                {
                    var weavingContractId = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == model.GRIGPId).WeavingContractId;
                    var purchaseContractId = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == model.GRIGPId).PurchaseContractId;
                    if (weavingContractId != 0)
                    {
                        GRWeavingContract weaving = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == weavingContractId);
                        weaving.BalanceContractQty = weaving.BalanceContractQty + x;
                        _dbContext.GRWeavingContracts.Update(weaving);
                    }
                    if (purchaseContractId != 0)
                    {
                        GRPurchaseContract purchase = _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == purchaseContractId);
                        purchase.BalanceContractQty = purchase.BalanceContractQty + Convert.ToInt32(x);
                        _dbContext.GRPurchaseContract.Update(purchase);
                    }
                }
                _dbContext.SaveChanges();
                //Add Detail
                foreach (var item in modelRepo.GRMendingDetail)
                {
                    GRMendingDetail detail = new GRMendingDetail();
                    detail.GRMendingId = model.Id;
                    detail.IGPDetailId = item.IGPDetailId;
                    detail.SrNo = item.SrNo;
                    detail.ReceivedQuantity = item.ReceivedQuantity;
                    detail.RejectedQuantity = item.RejectedQuantity;
                    detail.FreshQuantity = item.FreshQuantity;
                    detail.MendingQuantity = item.MendingQuantity;
                    detail.DamageTypeId = item.DamageTypeId;
                    //update balance in IGP

                    //ARInwardGatePass aRInwardGatePass = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == modelRepo.IGPId);
                    //aRInwardGatePass.BaleBalance = aRInwardGatePass.BaleBalance - saleReturnItems.Bales;
                    //_dbContext.ARInwardGatePass.Update(aRInwardGatePass);

                    _dbContext.GRMendingDetail.Add(detail);
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
        [HttpPost]
        public async Task<bool> Update(GRMendingViewModel modelRepo)
        {
            try
            {
                GRMending model = _dbContext.GRMending.FirstOrDefault(x => x.Id == modelRepo.GRMending.Id);
                model.TransactionNo = modelRepo.GRMending.TransactionNo;
                model.TransactionDate = modelRepo.GRMending.TransactionDate;
                model.GRIGPId = modelRepo.GRMending.GRIGPId;
                model.ReceivedQuantity = modelRepo.GRMending.ReceivedQuantity;
                model.LotNo = modelRepo.GRMending.LotNo;
                model.TotalRecievedQuantity = modelRepo.GRMending.TotalRecievedQuantity;
                model.TotalRejectedQuantity = modelRepo.GRMending.TotalRejectedQuantity;
                model.TotalFreshQuantity = modelRepo.GRMending.TotalFreshQuantity;
                model.TotalGradedQuantity = modelRepo.GRMending.TotalGradedQuantity;
                model.IsActive = true;
                model.IsDeleted = false;
                model.UpdatedBy = modelRepo.GRMending.UpdatedBy;
                model.UpdatedDate = DateTime.Now;
                _dbContext.GRMending.Update(model);
                var existingDetail = _dbContext.GRMendingDetail.Where(x => x.GRMendingId == modelRepo.GRMending.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.GRMendingDetail.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        //Handling Balance
                        //var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == saleReturn.IGPId);
                        //igp.BaleBalance = igp.BaleBalance + detail.BalesBalance;
                        //_dbContext.ARInwardGatePass.Update(igp);
                        //----------
                        _dbContext.GRMendingDetail.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit

                var existQuantity = Convert.ToInt32(_dbContext.GRMendingDetail.Where(x => x.GRMendingId == model.Id).Sum(x => x.RejectedQuantity));
                var updatedQuantity = Convert.ToInt32(modelRepo.GRMendingDetail.Sum(x => x.RejectedQuantity));
                var weavingContractId = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == model.GRIGPId).WeavingContractId;
                var purchaseContractId = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == model.GRIGPId).PurchaseContractId;
                GRWeavingContract weaving = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == weavingContractId);
                GRPurchaseContract purchase = _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == purchaseContractId);

                if (existQuantity > updatedQuantity)
                {
                    if (weavingContractId != 0)
                    {
                        weaving.BalanceContractQty = weaving.BalanceContractQty + (existQuantity - updatedQuantity);
                        _dbContext.GRWeavingContracts.Update(weaving);
                    }
                    if (purchaseContractId != 0)
                    {
                        purchase.BalanceContractQty = purchase.BalanceContractQty + (existQuantity - updatedQuantity); ;
                        _dbContext.GRPurchaseContract.Update(purchase);
                    }
                }
                if (existQuantity < updatedQuantity)
                {
                    if (weavingContractId != 0)
                    {
                        weaving.BalanceContractQty = weaving.BalanceContractQty - (updatedQuantity - existQuantity);
                        _dbContext.GRWeavingContracts.Update(weaving);
                    }
                    if (purchaseContractId != 0)
                    {
                        purchase.BalanceContractQty = purchase.BalanceContractQty - (updatedQuantity - existQuantity); ;
                        _dbContext.GRPurchaseContract.Update(purchase);
                    }
                }

                foreach (var detail in modelRepo.GRMendingDetail)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        GRMendingDetail Items = new GRMendingDetail();
                        Items.IGPDetailId = detail.IGPDetailId;
                        Items.SrNo = detail.SrNo;
                        Items.ReceivedQuantity = detail.ReceivedQuantity;
                        Items.RejectedQuantity = detail.RejectedQuantity;
                        Items.FreshQuantity = detail.FreshQuantity;
                        Items.MendingQuantity = detail.MendingQuantity;
                        Items.DamageTypeId = detail.DamageTypeId;
                        //Handling Balance
                        //var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == saleReturn.IGPId);
                        //igp.BaleBalance = igp.BaleBalance - detail.Bales;
                        //_dbContext.ARInwardGatePass.Update(igp);
                        //----------
                        await _dbContext.GRMendingDetail.AddAsync(Items);

                    }
                    else   //Updating Records
                    {
                        GRMendingDetail Items = _dbContext.GRMendingDetail.FirstOrDefault(x => x.Id == detail.Id);
                        Items.IGPDetailId = detail.IGPDetailId;
                        Items.SrNo = detail.SrNo;
                        Items.ReceivedQuantity = detail.ReceivedQuantity;
                        Items.RejectedQuantity = detail.RejectedQuantity;
                        Items.FreshQuantity = detail.FreshQuantity;
                        Items.MendingQuantity = detail.MendingQuantity;
                        Items.DamageTypeId = detail.DamageTypeId;
                        _dbContext.GRMendingDetail.Update(Items);
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
        public async Task<bool> Delete(int id,int companyId)
        {
            var deleteItem = _dbContext.GRMending.Where(v => v.IsDeleted == false && v.Id == id && v.CompanyId==companyId).FirstOrDefault();

            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                var x = Convert.ToInt32(_dbContext.GRMendingDetail.Where(x => x.GRMendingId == id).Sum(x => x.RejectedQuantity));
                if (x > 0)
                {
                    var weavingContractId = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == deleteItem.GRIGPId).WeavingContractId;
                    var purchaseContractId = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == deleteItem.GRIGPId).PurchaseContractId;
                    if (weavingContractId != 0)
                    {
                        GRWeavingContract weaving = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == weavingContractId);
                        weaving.BalanceContractQty = weaving.BalanceContractQty + x;
                        _dbContext.GRWeavingContracts.Update(weaving);
                    }
                    if (purchaseContractId != 0)
                    {
                        GRPurchaseContract purchase = _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == purchaseContractId);
                        purchase.BalanceContractQty = purchase.BalanceContractQty + Convert.ToInt32(x);
                        _dbContext.GRPurchaseContract.Update(purchase);
                    }
                }
                deleteItem.IsDeleted = true;
                var entry = _dbContext.GRMending.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
