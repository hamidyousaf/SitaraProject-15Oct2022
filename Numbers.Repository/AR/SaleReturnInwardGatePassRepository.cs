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
    public class SaleReturnInwardGatePassRepository
    {
        private readonly NumbersDbContext _dbContext;
        public SaleReturnInwardGatePassRepository(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int SIGPMaxNo(int companyId)
        {
            int maxSaleOrderNo = 1;
            var orders = _dbContext.ARSaleReturnInwardGatePass.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxSaleOrderNo = orders.Max(o => o.SIGPNo);
                return maxSaleOrderNo + 1;
            }
            else
            {
                return maxSaleOrderNo;
            }
        }
        [HttpPost]
        public async Task<bool> Create(SaleReturnInwardGatePassViewModel modelRepo)
        {
            try
            {
                ARSaleReturnInwardGatePass SIGP = new ARSaleReturnInwardGatePass();
                SIGP.SIGPNo = modelRepo.SIGPNo;
                SIGP.SIGPDate = modelRepo.SIGPDate;
                SIGP.BiltyDate = modelRepo.BiltyDate;
                SIGP.WarehouseId = modelRepo.WarehouseId;
                SIGP.CustomerId = modelRepo.CustomerId;
                SIGP.BuiltyNo = modelRepo.BuiltyNo;
                SIGP.Bails = modelRepo.Bails;
                SIGP.BailsBalance = modelRepo.Bails;
                SIGP.Status = "Created";
                SIGP.IsActive = true;
                SIGP.IsDeleted = false;
                SIGP.CompanyId = modelRepo.CompanyId;
                SIGP.Resp_ID = modelRepo.Resp_ID;
                SIGP.CreatedBy = modelRepo.CreatedBy;
                SIGP.CreatedDate = DateTime.Now.Date;
                _dbContext.ARSaleReturnInwardGatePass.Add(SIGP);
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
        public async Task<bool> Update(SaleReturnInwardGatePassViewModel modelRepo)
        {
            try
            {
                ARSaleReturnInwardGatePass SIGP = _dbContext.ARSaleReturnInwardGatePass.Find(modelRepo.Id);
                SIGP.SIGPNo = modelRepo.SIGPNo;
                SIGP.SIGPDate = modelRepo.SIGPDate;
                SIGP.BiltyDate = modelRepo.BiltyDate;
                SIGP.WarehouseId = modelRepo.WarehouseId;
                SIGP.CustomerId = modelRepo.CustomerId;
                SIGP.BuiltyNo = modelRepo.BuiltyNo;
                SIGP.Bails = modelRepo.Bails;
                SIGP.BailsBalance = modelRepo.Bails;
                SIGP.CompanyId = modelRepo.CompanyId;
                SIGP.Resp_ID = modelRepo.Resp_ID;
                SIGP.UpdatedBy = modelRepo.UpdatedBy;
                SIGP.UpdatedDate = DateTime.Now.Date;
                _dbContext.ARSaleReturnInwardGatePass.Update(SIGP);
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
        public SaleReturnInwardGatePassViewModel GetById(int id)
        {
            ARSaleReturnInwardGatePass listOrder = _dbContext.ARSaleReturnInwardGatePass.Find(id);
            var viewModel = new SaleReturnInwardGatePassViewModel();
            viewModel.Id = listOrder.Id;
            viewModel.SIGPNo = listOrder.SIGPNo;
            viewModel.SIGPDate = listOrder.SIGPDate;
            viewModel.BiltyDate = listOrder.BiltyDate;
            viewModel.WarehouseId = listOrder.WarehouseId;
            viewModel.CustomerId = listOrder.CustomerId;
            viewModel.BuiltyNo = listOrder.BuiltyNo;
            viewModel.Bails = listOrder.Bails;
            return viewModel;
        }
        public async Task<bool> Approve(int id, string userId)
        {
            try
            {
                var sIGPRepository = _dbContext.ARSaleReturnInwardGatePass.Find(id);
                sIGPRepository.Status = "Approved";
                sIGPRepository.ApprovedBy = userId;
                sIGPRepository.ApprovedDate = DateTime.Now;
                sIGPRepository.IsApproved = true;
                _dbContext.ARSaleReturnInwardGatePass.Update(sIGPRepository);
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
        public async Task<bool> Delete(int id)
        {
            var aRSaleReturnInwardGatePass = _dbContext.ARSaleReturnInwardGatePass.Find(id);
            aRSaleReturnInwardGatePass.IsDeleted = true;
            var entry = _dbContext.ARSaleReturnInwardGatePass.Update(aRSaleReturnInwardGatePass);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
