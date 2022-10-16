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
    public class InwardGatePassRepo
    {
        private readonly NumbersDbContext _dbContext;
        public InwardGatePassRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int SIGPMaxNo(int companyId)
        {
            int maxSaleOrderNo = 1;
            var orders = _dbContext.ARInwardGatePass.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxSaleOrderNo = orders.Max(o => o.IGPNo);
                return maxSaleOrderNo + 1;
            }
            else
            {
                return maxSaleOrderNo;
            }
        }
        [HttpPost]
        public async Task<bool> Create(InwardGatePassViewModel modelRepo)
        {
            try
            {
                ARInwardGatePass SIGP = new ARInwardGatePass();
                SIGP.IGPNo = modelRepo.IGPNo;
                SIGP.IGPDate = modelRepo.IGPDate;
                SIGP.WarehouseId = modelRepo.WarehouseId;
                SIGP.CustomerId = modelRepo.CustomerId;
                SIGP.BuiltyNo = modelRepo.BuiltyNo;
                SIGP.OGPId = modelRepo.OGPId;
                SIGP.Bails = modelRepo.Bails;
                SIGP.BaleBalance = modelRepo.Bails;
                SIGP.Status = "Created";
                SIGP.IsActive = true;
                SIGP.IsDeleted = false;
                SIGP.CompanyId = modelRepo.CompanyId;
                SIGP.Resp_ID = modelRepo.Resp_ID;
                SIGP.CreatedBy = modelRepo.CreatedBy;
                SIGP.CreatedDate = DateTime.Now.Date;
                _dbContext.ARInwardGatePass.Add(SIGP);
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
        public async Task<bool> Update(InwardGatePassViewModel modelRepo)
        {
            try
            {
                ARInwardGatePass SIGP = _dbContext.ARInwardGatePass.Find(modelRepo.Id);
                SIGP.IGPNo = modelRepo.IGPNo;
                SIGP.IGPDate = modelRepo.IGPDate;
                SIGP.WarehouseId = modelRepo.WarehouseId;
                SIGP.CustomerId = modelRepo.CustomerId;
                SIGP.BuiltyNo = modelRepo.BuiltyNo;
                SIGP.OGPId = modelRepo.OGPId;
                SIGP.BaleBalance = modelRepo.Bails;
                SIGP.Bails = modelRepo.Bails;
                SIGP.CompanyId = modelRepo.CompanyId;
                SIGP.Resp_ID = modelRepo.Resp_ID;
                SIGP.UpdatedBy = modelRepo.UpdatedBy;
                SIGP.UpdatedDate = DateTime.Now.Date;
                _dbContext.ARInwardGatePass.Update(SIGP);
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
        public InwardGatePassViewModel GetById(int id)
        {
            ARInwardGatePass listOrder = _dbContext.ARInwardGatePass.Find(id);
            var viewModel = new InwardGatePassViewModel();
            viewModel.Id = listOrder.Id;
            viewModel.IGPNo = listOrder.IGPNo;
            viewModel.IGPDate = listOrder.IGPDate;
            viewModel.OGPId = listOrder.OGPId;
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
                var aRInwardGatePass = _dbContext.ARInwardGatePass.Find(id);
                aRInwardGatePass.Status = "Approved";
                aRInwardGatePass.ApprovedBy = userId;
                aRInwardGatePass.ApprovedDate = DateTime.Now;
                aRInwardGatePass.IsApproved = true;
                _dbContext.ARInwardGatePass.Update(aRInwardGatePass);
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
            var aRInwardGatePass = _dbContext.ARInwardGatePass.Find(id);
            aRInwardGatePass.IsDeleted = true;
            var entry = _dbContext.ARInwardGatePass.Update(aRInwardGatePass);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
