using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AR
{
    public class PackingRepo
    {
        private readonly NumbersDbContext _dbContext;
        public PackingRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public PackingViewModel GetById(int id)
        {
            ARPacking ARPacking = _dbContext.ARPacking.Find(id);
            var viewModel = new PackingViewModel();
            viewModel.Id = ARPacking.Id;
            viewModel.PackingNo = ARPacking.PackingNo;
            viewModel.WareHouseId = ARPacking.WareHouseId;
            viewModel.PackingDate = ARPacking.PackingDate;
            viewModel.CustomerId = ARPacking.CustomerId;
            viewModel.SRIId = ARPacking.SRIId;
            return viewModel;
        }
        public int MaxNo(int companyId)
        {
            int maxNo = 1;
            var orders = _dbContext.ARPacking.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxNo = orders.Max(o => o.PackingNo);
                return maxNo + 1;
            }
            else
            {
                return maxNo;
            }
        }
        [HttpPost]
        public async Task<bool> Create(PackingViewModel modelRepo,IFormCollection collection)
        {
            try
            {
                ARPackingItems[] ARPackingItems = JsonConvert.DeserializeObject<ARPackingItems[]>(collection["PackingDetail"]);

                ARPacking ARPacking = new ARPacking();
                ARPacking.PackingNo = modelRepo.PackingNo;
                ARPacking.PackingDate = modelRepo.PackingDate;
                ARPacking.CustomerId = modelRepo.CustomerId;
                ARPacking.SRIId = modelRepo.SRIId;
                ARPacking.WareHouseId = modelRepo.WareHouseId;
 
                ARPacking.Status = "Created";
                ARPacking.IsActive = true;
                ARPacking.IsDeleted = false;
                ARPacking.CompanyId = modelRepo.CompanyId;
                ARPacking.Resp_ID = modelRepo.Resp_ID;
                ARPacking.CreatedBy = modelRepo.CreatedBy;
                ARPacking.CreatedDate = DateTime.Now.Date;
                ARPacking.Resp_ID = modelRepo.Resp_ID;
                _dbContext.ARPacking.Add(ARPacking);
                _dbContext.SaveChanges();
              
                foreach (var item in ARPackingItems)
                {
                    ARPackingItems packingItems = new ARPackingItems();
                    packingItems.PackingId = ARPacking.Id;
                    packingItems.FourthItemCategoryId = item.FourthItemCategoryId;
                    packingItems.ItemId = item.ItemId;
                    packingItems.Qty = item.Qty;
                    packingItems.ReasonTypeId = item.ReasonTypeId;
                    packingItems.ReturnTypeId = item.ReturnTypeId;
                    packingItems.SeasonId = item.SeasonId;
                    _dbContext.ARPackingItems.Add(packingItems);

                    //Handle Balance Quantity
                    SaleReturnItems SRItems = _dbContext.SaleReturnItems.FirstOrDefault(x => x.SaleReturnId == ARPacking.SRIId && x.FourthItemCategory == packingItems.FourthItemCategoryId);
                    if (SRItems != null)
                    {
                        SRItems.MetersBalance = SRItems.MetersBalance - Convert.ToInt32(packingItems.Qty);
                        //SRItems.BalesBalance = SRItems.BalesBalance - Convert.ToInt32(packingItems.Qty);
                        _dbContext.SaleReturnItems.Update(SRItems);
                        _dbContext.SaveChanges();
                    }
                    //----------
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
                var ARPacking = _dbContext.ARPacking.Find(id);
                ARPacking.Status = "Approved";
                ARPacking.ApprovedBy = userId;
                ARPacking.ApprovedDate = DateTime.Now;
                ARPacking.IsApproved = true;
                _dbContext.ARPacking.Update(ARPacking);
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
        public async Task<bool> Update(PackingViewModel modelRepo,IFormCollection collection)
        {
            try
            {
                ARPackingItems[] ARPackingItems = JsonConvert.DeserializeObject<ARPackingItems[]>(collection["PackingDetail"]);
                var existingDetail = _dbContext.ARPackingItems.Where(x => x.PackingId == modelRepo.Id).ToList();

                foreach (var detail in existingDetail)
                {
                    bool isExist = ARPackingItems.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        //Handle Balance Quantity
                        var SRItems = _dbContext.SaleReturnItems.FirstOrDefault(x => x.SaleReturnId == modelRepo.SRIId && x.FourthItemCategory == detail.FourthItemCategoryId);
                        //SRItems.BalesBalance = SRItems.BalesBalance + Convert.ToInt32(detail.Qty);
                        SRItems.MetersBalance = SRItems.MetersBalance + Convert.ToInt32(detail.Qty);
                        _dbContext.SaleReturnItems.Update(SRItems);
                        //----------
                        _dbContext.ARPackingItems.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                ARPacking ARPacking = _dbContext.ARPacking.FirstOrDefault(x => x.Id == modelRepo.Id);
                ARPacking.PackingNo = modelRepo.PackingNo;
                ARPacking.PackingDate = modelRepo.PackingDate;
                ARPacking.WareHouseId = modelRepo.WareHouseId;
                ARPacking.CustomerId = modelRepo.CustomerId;
                ARPacking.SRIId = modelRepo.SRIId;
                ARPacking.IsActive = true;
                ARPacking.IsDeleted = false;
                ARPacking.UpdatedBy = modelRepo.UpdatedBy;
                ARPacking.UpdatedDate = DateTime.Now;
                _dbContext.ARPacking.Update(ARPacking);
              
                
                foreach (var detail in ARPackingItems)
                {
                    if (detail.Id == 0)  
                    {
                        ARPackingItems packingItems = new ARPackingItems();
                        packingItems.PackingId = modelRepo.Id;
                        packingItems.FourthItemCategoryId = detail.FourthItemCategoryId;
                        packingItems.Qty = detail.Qty;
                        packingItems.ItemId = detail.ItemId;
                        packingItems.ReasonTypeId = detail.ReasonTypeId;
                        packingItems.ReturnTypeId = detail.ReturnTypeId;
                        packingItems.SeasonId = detail.SeasonId;
                        await _dbContext.ARPackingItems.AddAsync(packingItems);
                        //Handle Balance Quantity
                        SaleReturnItems SRItems = _dbContext.SaleReturnItems.FirstOrDefault(x => x.SaleReturnId == ARPacking.SRIId && x.FourthItemCategory == packingItems.FourthItemCategoryId);
                        if (SRItems != null)
                        {
                            SRItems.MetersBalance = SRItems.MetersBalance - Convert.ToInt32(packingItems.Qty);
                            //SRItems.BalesBalance = SRItems.BalesBalance - Convert.ToInt32(packingItems.Qty);
                            _dbContext.SaleReturnItems.Update(SRItems);
                            _dbContext.SaveChanges();
                        }
                        //----------
                    }
                    else    
                    {
                        var ARPackingItemsData = _dbContext.ARPackingItems.FirstOrDefault(x => x.Id == detail.Id);
                        ARPackingItemsData.FourthItemCategoryId = detail.FourthItemCategoryId;
                        ARPackingItemsData.ItemId = detail.ItemId;
                        ARPackingItemsData.Qty = detail.Qty;
                        ARPackingItemsData.ReasonTypeId = detail.ReasonTypeId;
                        ARPackingItemsData.ReturnTypeId = detail.ReturnTypeId;
                        ARPackingItemsData.SeasonId = detail.SeasonId;
                        _dbContext.ARPackingItems.Update(ARPackingItemsData);
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
            var aRPacking = _dbContext.ARPacking.Find(id);
            aRPacking.IsDeleted = true;
            var entry = _dbContext.ARPacking.Update(aRPacking);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            
            //Handle Balance Quantity
            var aRPackingDetail = _dbContext.ARPackingItems.Where(x=>x.PackingId == aRPacking.Id).ToList();
            foreach (var item in aRPackingDetail)
            {
                SaleReturnItems SRItems = _dbContext.SaleReturnItems.FirstOrDefault(x=>x.SaleReturnId == aRPacking.SRIId && x.FourthItemCategory == item.FourthItemCategoryId);
                //SRItems.BalesBalance = SRItems.BalesBalance + Convert.ToInt32(item.Qty);
                SRItems.MetersBalance = SRItems.MetersBalance + Convert.ToInt32(item.Qty);
                _dbContext.SaleReturnItems.Update(SRItems);
                await _dbContext.SaveChangesAsync();
            }
            //----------
            
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
