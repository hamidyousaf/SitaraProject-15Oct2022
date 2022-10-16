using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AR
{
    public class SaleReturnRepository
    {
        private readonly NumbersDbContext _dbContext;
        public SaleReturnRepository(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public SaleReturnViewModel GetById(int id)
        {
            SaleReturn saleReturn = _dbContext.SaleReturn.Find(id);
            var viewModel = new SaleReturnViewModel();
            viewModel.Id = saleReturn.Id;
            viewModel.TransactionNo = saleReturn.TransactionNo;
            viewModel.SRDate = saleReturn.SRDate;
            viewModel.CustomerId = saleReturn.CustomerId;
            //viewModel.BalanceQuantity = saleReturn.BalanceQuantity;
            viewModel.BuiltyNo = saleReturn.BuiltyNo;
            viewModel.Bails = saleReturn.Bails;
            viewModel.IGPId = saleReturn.IGPId;
            return viewModel;
        }
        public int MaxNo(int companyId)
        {
            int maxNo = 1;
            var orders = _dbContext.SaleReturn.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxNo = orders.Max(o => o.TransactionNo);
                return maxNo + 1;
            }
            else
            {
                return maxNo;
            }
        }
        [HttpPost]
        public async Task<bool> Create(SaleReturnViewModel modelRepo)
        {
            try
            {
                //Add Master
                SaleReturn saleReturn = new SaleReturn();
                saleReturn.TransactionNo = modelRepo.TransactionNo;
                saleReturn.SRDate = modelRepo.SRDate;
                saleReturn.CustomerId = modelRepo.CustomerId;
                saleReturn.IGPId = modelRepo.IGPId;
                saleReturn.BuiltyNo = modelRepo.BuiltyNo;
                saleReturn.Bails = modelRepo.Bails;
                saleReturn.BalanceQuantity = modelRepo.Bails;
                saleReturn.TotalMeters = modelRepo.TotalMeters;
                saleReturn.TotalBales = modelRepo.TotalBales;
                saleReturn.Status = "Created";
                saleReturn.IsActive = true;
                saleReturn.IsDeleted = false;
                saleReturn.CompanyId = modelRepo.CompanyId;
                saleReturn.Resp_ID = modelRepo.Resp_ID;
                saleReturn.CreatedBy = modelRepo.CreatedBy;
                saleReturn.CreatedDate = DateTime.Now.Date;
                saleReturn.Resp_ID = modelRepo.Resp_ID;
                _dbContext.SaleReturn.Add(saleReturn);
                _dbContext.SaveChanges();
                
                //Add Detail
                foreach (var item in modelRepo.SaleReturnItemsList)
                {
                    SaleReturnItems saleReturnItems = new SaleReturnItems();
                    saleReturnItems.SaleReturnId = saleReturn.Id;
                    saleReturnItems.SecondItemCategory = item.SecondItemCategory;
                    saleReturnItems.ThirdItemCategory = item.ThirdItemCategory;
                    saleReturnItems.FourthItemCategory = item.FourthItemCategory;
                    saleReturnItems.Meters = item.Meters;
                    saleReturnItems.MetersBalance = item.Meters;
                    saleReturnItems.Bales = item.Bales;
                    saleReturnItems.BalesBalance = item.Bales;

                    //update balance in IGP
                    ARInwardGatePass aRInwardGatePass = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == modelRepo.IGPId);
                    aRInwardGatePass.BaleBalance = aRInwardGatePass.BaleBalance - saleReturnItems.Bales;
                    _dbContext.ARInwardGatePass.Update(aRInwardGatePass);

                    _dbContext.SaleReturnItems.Add(saleReturnItems);
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
        public async Task<bool> Approve(int id, string userId)
        {
            try
            {
                var saleReturn = _dbContext.SaleReturn.Find(id);
                saleReturn.Status = "Approved";
                saleReturn.ApprovedBy = userId;
                saleReturn.ApprovedDate = DateTime.Now;
                saleReturn.IsApproved = true;
                _dbContext.SaleReturn.Update(saleReturn);
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
        public async Task<bool> Update(SaleReturnViewModel modelRepo)
        {
            try
            {
                SaleReturn saleReturn = _dbContext.SaleReturn.FirstOrDefault(x => x.Id == modelRepo.Id);
                saleReturn.TransactionNo = modelRepo.TransactionNo;
                saleReturn.SRDate = modelRepo.SRDate;
                saleReturn.CustomerId = modelRepo.CustomerId;
                saleReturn.IGPId = modelRepo.IGPId;
                saleReturn.BuiltyNo = modelRepo.BuiltyNo;
                saleReturn.Bails = modelRepo.Bails;
                saleReturn.BalanceQuantity = modelRepo.BalanceQuantity;
                saleReturn.TotalMeters = modelRepo.TotalMeters;
                saleReturn.IsActive = true;
                saleReturn.IsDeleted = false;
                saleReturn.UpdatedBy = modelRepo.UpdatedBy;
                saleReturn.UpdatedDate = DateTime.Now;
                _dbContext.SaleReturn.Update(saleReturn);
                var existingDetail = _dbContext.SaleReturnItems.Where(x => x.SaleReturnId == modelRepo.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.SaleReturnItemsList.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        //Handling Balance
                        var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x=>x.Id == saleReturn.IGPId);
                        igp.BaleBalance = igp.BaleBalance + detail.BalesBalance;
                        _dbContext.ARInwardGatePass.Update(igp);
                        //----------
                        _dbContext.SaleReturnItems.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.SaleReturnItemsList)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        SaleReturnItems saleReturnItems = new SaleReturnItems();
                        saleReturnItems.SaleReturnId = modelRepo.Id;
                        saleReturnItems.SecondItemCategory = detail.SecondItemCategory;
                        saleReturnItems.ThirdItemCategory = detail.ThirdItemCategory;
                        saleReturnItems.FourthItemCategory = detail.FourthItemCategory;
                        saleReturnItems.Meters = detail.Meters;
                        saleReturnItems.MetersBalance = detail.Meters;
                        saleReturnItems.Bales = detail.Bales;
                        saleReturnItems.BalesBalance = detail.Bales;
                        //Handling Balance
                        var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == saleReturn.IGPId);
                        igp.BaleBalance = igp.BaleBalance - detail.Bales;
                        _dbContext.ARInwardGatePass.Update(igp);
                        //----------
                        await _dbContext.SaleReturnItems.AddAsync(saleReturnItems);

                    }
                    else   //Updating Records
                    {
                        var saleReturnItemsData = _dbContext.SaleReturnItems.FirstOrDefault(x => x.Id == detail.Id);
                        saleReturnItemsData.SecondItemCategory = detail.SecondItemCategory;
                        saleReturnItemsData.ThirdItemCategory = detail.ThirdItemCategory;
                        saleReturnItemsData.FourthItemCategory = detail.FourthItemCategory;
                        saleReturnItemsData.Meters = detail.Meters;
                        saleReturnItemsData.MetersBalance = detail.Meters;
                        saleReturnItemsData.Bales = detail.Bales;
                        saleReturnItemsData.BalesBalance = detail.Bales;
                        _dbContext.SaleReturnItems.Update(saleReturnItemsData);
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
            var saleReturn = _dbContext.SaleReturn.Find(id);
            saleReturn.IsDeleted = true;
            var entry = _dbContext.SaleReturn.Update(saleReturn);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == saleReturn.IGPId);
            var modelDetails = _dbContext.SaleReturnItems.Where(x => x.SaleReturnId == saleReturn.Id).ToList();
            //Handling balance
            foreach (var detail in modelDetails)
            {
                igp.BaleBalance = igp.BaleBalance + detail.BalesBalance;
                _dbContext.ARInwardGatePass.Update(igp);
                await _dbContext.SaveChangesAsync();
            }
            //----------
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
