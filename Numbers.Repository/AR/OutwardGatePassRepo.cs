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
    public class OutwardGatePassRepo
    {
        private readonly NumbersDbContext _dbContext;
        public OutwardGatePassRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int SIGPMaxNo(int companyId)
        {
            int maxSaleOrderNo = 1;
            var orders = _dbContext.AROutwardGatePass.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxSaleOrderNo = orders.Max(o => o.OGPNo);
                return maxSaleOrderNo + 1;
            }
            else
            {
                return maxSaleOrderNo;
            }
        }
        [HttpPost]
        public async Task<bool> Create(OutwardGatePassViewModel modelRepo)
        {
            try
            {
                AROutwardGatePass SIGP = new AROutwardGatePass();
                SIGP.OGPNo = modelRepo.OGPNo;
                SIGP.OGPDate = modelRepo.OGPDate;
                SIGP.WarehouseId = modelRepo.WarehouseId;
                SIGP.CustomerId = modelRepo.CustomerId;
                SIGP.BuiltyNo = modelRepo.BuiltyNo;
                SIGP.Bails = modelRepo.Bails;
                SIGP.OGPQty = modelRepo.OGPQty;
                SIGP.SIGPId = modelRepo.SIGPId;
                SIGP.Status = "Created";
                SIGP.IsActive = true;
                SIGP.IsDeleted = false;
                SIGP.CompanyId = modelRepo.CompanyId;
                SIGP.Resp_ID = modelRepo.Resp_ID;
                SIGP.CreatedBy = modelRepo.CreatedBy;
                SIGP.CreatedDate = DateTime.Now.Date;
                _dbContext.AROutwardGatePass.Add(SIGP);
                //Handle Balance Quantity
                // Balance = TotalQuantity - (TotalRecieved + CurrentQuantity) -->Balance(AddPreviuesTable), TotalQuantity(GetPreviousTable), TotalRecieved(GetCurrentTable), CurrentQuantity(AddCurrentTable)
                ARSaleReturnInwardGatePass IGP = _dbContext.ARSaleReturnInwardGatePass.FirstOrDefault(x=>x.Id == SIGP.SIGPId);
                var TotalRecieved = _dbContext.AROutwardGatePass.Where(x=>x.SIGPId == SIGP.SIGPId && x.IsActive == true && x.IsDeleted == false).Sum(x=>x.OGPQty);
                if (IGP != null)
                {
                    IGP.BailsBalance = IGP.Bails - (Convert.ToInt32(TotalRecieved) + Convert.ToInt32(SIGP.OGPQty));
                    _dbContext.ARSaleReturnInwardGatePass.Update(IGP);
                    _dbContext.SaveChanges();
                }
                //----------
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
        public async Task<bool> Update(OutwardGatePassViewModel modelRepo)
        {
            try
            {
                AROutwardGatePass SIGP = _dbContext.AROutwardGatePass.Find(modelRepo.Id);
                //Handle Balance Quantity
                // Balance = TotalQuantity - (TotalRecieved + CurrentQuantity) -->Balance(AddPreviuesTable), TotalQuantity(GetPreviousTable), TotalRecieved(GetCurrentTable), CurrentQuantity(AddCurrentTable)
                ARSaleReturnInwardGatePass IGP = _dbContext.ARSaleReturnInwardGatePass.FirstOrDefault(x => x.Id == SIGP.SIGPId);
                if (modelRepo.OGPQty > SIGP.OGPQty)
                {
                    IGP.BailsBalance = IGP.BailsBalance - Convert.ToInt32(modelRepo.OGPQty - SIGP.OGPQty);
                    _dbContext.ARSaleReturnInwardGatePass.Update(IGP);
                }
                else if (modelRepo.OGPQty < SIGP.OGPQty)
                {
                    IGP.BailsBalance = IGP.BailsBalance + Convert.ToInt32(SIGP.OGPQty - modelRepo.OGPQty);
                    _dbContext.ARSaleReturnInwardGatePass.Update(IGP);
                }
                else
                {

                }
                //----------
                SIGP.OGPNo = modelRepo.OGPNo;
                SIGP.OGPDate = modelRepo.OGPDate;
                SIGP.WarehouseId = modelRepo.WarehouseId;
                SIGP.CustomerId = modelRepo.CustomerId;
                SIGP.BuiltyNo = modelRepo.BuiltyNo;
                SIGP.Bails = modelRepo.Bails;
                SIGP.SIGPId = modelRepo.SIGPId;
                SIGP.OGPQty = modelRepo.OGPQty;
                SIGP.CompanyId = modelRepo.CompanyId;
                SIGP.Resp_ID = modelRepo.Resp_ID;
                SIGP.UpdatedBy = modelRepo.UpdatedBy;
                SIGP.UpdatedDate = DateTime.Now.Date;
                _dbContext.AROutwardGatePass.Update(SIGP);

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
        public OutwardGatePassViewModel GetById(int id)
        {
            AROutwardGatePass listOrder = _dbContext.AROutwardGatePass.Find(id);
            var viewModel = new OutwardGatePassViewModel();
            viewModel.Id = listOrder.Id;
            viewModel.OGPNo = listOrder.OGPNo;
            viewModel.OGPDate = listOrder.OGPDate;
            viewModel.SIGPId = listOrder.SIGPId;
            viewModel.WarehouseId = listOrder.WarehouseId;
            viewModel.CustomerId = listOrder.CustomerId;
            viewModel.BuiltyNo = listOrder.BuiltyNo;
            viewModel.Bails = listOrder.Bails;
            viewModel.OGPQty = listOrder.OGPQty;
            return viewModel;
        }
        public async Task<bool> Approve(int id, string userId)
        {
            try
            {
                var aROutwardGatePass = _dbContext.AROutwardGatePass.Find(id);
                aROutwardGatePass.Status = "Approved";
                aROutwardGatePass.ApprovedBy = userId;
                aROutwardGatePass.ApprovedDate = DateTime.Now;
                aROutwardGatePass.IsApproved = true;
                _dbContext.AROutwardGatePass.Update(aROutwardGatePass);
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
            var aROutwardGatePass = _dbContext.AROutwardGatePass.Find(id);
            aROutwardGatePass.IsDeleted = true;
            var entry = _dbContext.AROutwardGatePass.Update(aROutwardGatePass);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            //Handle Balance Quantity
            ARSaleReturnInwardGatePass record = _dbContext.ARSaleReturnInwardGatePass.FirstOrDefault(x=>x.Id == aROutwardGatePass.SIGPId);
            record.BailsBalance = record.BailsBalance + Convert.ToInt32(aROutwardGatePass.OGPQty);
            _dbContext.ARSaleReturnInwardGatePass.Update(record);
            //---------
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
