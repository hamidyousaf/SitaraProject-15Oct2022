using Microsoft.AspNetCore.Http;
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
    public class InwardGatePassRepo
    {
        private readonly NumbersDbContext _dbContext;
        public InwardGatePassRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        public async Task<bool> Create(GRInwardGatePassViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                //Add Master
                GRInwardGatePass model = new GRInwardGatePass();
                model.TransactionNo = modelRepo.GRInwardGatePass.TransactionNo;
                model.TransactionDate = modelRepo.GRInwardGatePass.TransactionDate;
                model.WeavingContractId = modelRepo.GRInwardGatePass.WeavingContractId;
                model.PurchaseContractId = modelRepo.GRInwardGatePass.PurchaseContractId;
                model.LotNo = modelRepo.GRInwardGatePass.LotNo;
                model.TotalRecievedQuantity = Convert.ToDecimal(collection["TotalReceived"]);
                model.TotalMeasureQuantity = Convert.ToDecimal(collection["TotalMeasure"]);
                model.TotalActualQuantity = Convert.ToDecimal(collection["TotalActual"]);
                model.Quantity = Convert.ToDecimal(collection["TotalActual"]);
                model.IsActive = true;
                model.IsDeleted = false;
                model.CompanyId = modelRepo.GRInwardGatePass.CompanyId;
                model.Resp_Id = modelRepo.GRInwardGatePass.Resp_Id;
                model.CreatedBy = modelRepo.GRInwardGatePass.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                _dbContext.GRInwardGatePass.Add(model);
                _dbContext.SaveChanges();

                //Add Detail
                foreach (var item in modelRepo.GRInwardGatePassDetail)
                {
                    GRInwardGatePassDetail detail = new GRInwardGatePassDetail();
                    detail.GRIGPId = model.Id;
                    detail.SrNo = item.SrNo;
                    detail.ReceivedQuantity = item.ReceivedQuantity;
                    detail.MeasureQuantity = item.MeasureQuantity;
                    detail.ActualQuantity = item.ActualQuantity;
                    //update contract qty balance
                    if (model.WeavingContractId != 0)
                    {
                        GRWeavingContract WeavingContract = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == model.WeavingContractId);
                        WeavingContract.BalanceContractQty = WeavingContract.BalanceContractQty - detail.ActualQuantity;
                        _dbContext.GRWeavingContracts.Update(WeavingContract);
                    }
                    if (model.PurchaseContractId != 0)
                    {
                        GRPurchaseContract PurchaseContract = _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == model.PurchaseContractId);
                        PurchaseContract.BalanceContractQty = PurchaseContract.BalanceContractQty - Convert.ToInt32(detail.ActualQuantity);
                        _dbContext.GRPurchaseContract.Update(PurchaseContract);
                    }

                    _dbContext.GRInwardGatePassDetails.Add(detail);
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
            var result = _dbContext.GRInwardGatePass.Where(x=>x.IsDeleted != true).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x=>x.TransactionNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Update(GRInwardGatePassViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                GRInwardGatePass model = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == modelRepo.GRInwardGatePass.Id);
                model.TransactionNo = modelRepo.GRInwardGatePass.TransactionNo;
                model.TransactionDate = modelRepo.GRInwardGatePass.TransactionDate;
                model.WeavingContractId = modelRepo.GRInwardGatePass.WeavingContractId;
                model.PurchaseContractId = modelRepo.GRInwardGatePass.PurchaseContractId;
                model.LotNo = modelRepo.GRInwardGatePass.LotNo;
                model.TotalRecievedQuantity = Convert.ToDecimal(collection["TotalReceived"]);
                model.TotalMeasureQuantity = Convert.ToDecimal(collection["TotalMeasure"]);
                model.TotalActualQuantity = Convert.ToDecimal(collection["TotalActual"]);
                model.Quantity = Convert.ToDecimal(collection["TotalActual"]);
                model.IsActive = true;
                model.IsDeleted = false;
                model.UpdatedBy = modelRepo.GRInwardGatePass.UpdatedBy;
                model.UpdatedDate = DateTime.Now;
                _dbContext.GRInwardGatePass.Update(model);
                var existingDetail = _dbContext.GRInwardGatePassDetails.Where(x => x.GRIGPId == modelRepo.GRInwardGatePass.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.GRInwardGatePassDetail.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        //Handling Balance
                        if (model.WeavingContractId != 0)
                        {
                            GRWeavingContract WeavingContract = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == model.WeavingContractId);
                            WeavingContract.BalanceContractQty = WeavingContract.BalanceContractQty + detail.ActualQuantity;
                            _dbContext.GRWeavingContracts.Update(WeavingContract);
                        }
                        if (model.PurchaseContractId != 0)
                        {
                            GRPurchaseContract PurchaseContract = _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == model.PurchaseContractId);
                            PurchaseContract.BalanceContractQty = PurchaseContract.BalanceContractQty + Convert.ToInt32(detail.ActualQuantity);
                            _dbContext.GRPurchaseContract.Update(PurchaseContract);
                        }
                        
                        //----------
                        _dbContext.GRInwardGatePassDetails.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.GRInwardGatePassDetail)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        GRInwardGatePassDetail iGPItems = new GRInwardGatePassDetail();
                        iGPItems.GRIGPId = modelRepo.GRInwardGatePass.Id;
                        iGPItems.SrNo = detail.SrNo;
                        iGPItems.ReceivedQuantity = detail.ReceivedQuantity;
                        iGPItems.MeasureQuantity = detail.MeasureQuantity;
                        iGPItems.ActualQuantity = detail.ActualQuantity;
                        //Handling Balance
                        if (model.WeavingContractId != 0)
                        {
                            GRWeavingContract WeavingContract = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == model.WeavingContractId);
                            WeavingContract.BalanceContractQty = WeavingContract.BalanceContractQty - detail.ActualQuantity;
                            _dbContext.GRWeavingContracts.Update(WeavingContract);
                        }
                        if (model.PurchaseContractId != 0)
                        {
                            GRPurchaseContract PurchaseContract = _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == model.PurchaseContractId);
                            PurchaseContract.BalanceContractQty = PurchaseContract.BalanceContractQty - Convert.ToInt32(detail.ActualQuantity);
                            _dbContext.GRPurchaseContract.Update(PurchaseContract);
                        }





                        await _dbContext.SaveChangesAsync();
                        //----------
                        await _dbContext.GRInwardGatePassDetails.AddAsync(iGPItems);

                    }
                    else   //Updating Records
                    {
                        var saleReturnItemsData = _dbContext.GRInwardGatePassDetails.FirstOrDefault(x => x.Id == detail.Id);
                        saleReturnItemsData.SrNo = detail.SrNo;
                        saleReturnItemsData.ReceivedQuantity = detail.ReceivedQuantity;
                        saleReturnItemsData.MeasureQuantity = detail.MeasureQuantity;
                        saleReturnItemsData.MeasureQuantity = detail.MeasureQuantity;
                        saleReturnItemsData.ActualQuantity = detail.ActualQuantity;
                        _dbContext.GRInwardGatePassDetails.Update(saleReturnItemsData);
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
        public async Task<bool> Delete(int id)
        {
            var deleteItem = _dbContext.GRInwardGatePass.Where(v => v.IsDeleted == false && v.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.GRInwardGatePass.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                var details = _dbContext.GRInwardGatePassDetails.Where(x => x.GRIGPId == id);

                if (deleteItem.WeavingContractId != 0)
                {
                    foreach (var item in details)
                    {
                        GRWeavingContract weavingContract = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == deleteItem.WeavingContractId);
                        weavingContract.BalanceContractQty = weavingContract.BalanceContractQty + item.ActualQuantity;
                        _dbContext.GRWeavingContracts.Update(weavingContract);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                if (deleteItem.PurchaseContractId != 0)
                {
                    foreach (var item in details)
                    {
                        GRPurchaseContract purchaseContract = _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == deleteItem.PurchaseContractId);
                        purchaseContract.BalanceContractQty = purchaseContract.BalanceContractQty + Convert.ToInt32(item.ActualQuantity);
                        _dbContext.GRPurchaseContract.Update(purchaseContract);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
        [HttpGet]
        public string GenerateLotNo(int companyId)
        {
            string transactionNo = "100001";
            var result = _dbContext.GRInwardGatePass.Where(x => x.CompanyId == companyId && x.IsDeleted != true).ToList();
            if (result.Count() > 0)
            {
                transactionNo = (Convert.ToInt32(result.Max(x => x.LotNo)) + 1).ToString();
            }
            return transactionNo;
        }
    }
}
