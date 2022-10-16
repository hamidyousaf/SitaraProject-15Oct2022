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

namespace Numbers.Repository.Inventory
{
    public class BomAccessoriesRepo
    {
        private readonly NumbersDbContext _dbContext;
        public BomAccessoriesRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        public async Task<bool> Create(BomAccessoriesViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                BomAccessoriesDetail[] InvBOMDetail = JsonConvert.DeserializeObject<BomAccessoriesDetail[]>(collection["Detail"]);
                BomAccessories invBOM = new BomAccessories();
                invBOM.Id = modelRepo.BomAccessories.Id;
                invBOM.TransactionNo = modelRepo.BomAccessories.TransactionNo;
                invBOM.TransactionDate = modelRepo.BomAccessories.TransactionDate;
                invBOM.SecondItemCategoryId = modelRepo.BomAccessories.SecondItemCategoryId;
                invBOM.FourthItemCategoryId = modelRepo.BomAccessories.FourthItemCategoryId;
                invBOM.Kameez = modelRepo.BomAccessories.Kameez;
                invBOM.Shalwar = modelRepo.BomAccessories.Shalwar;
                invBOM.Dupatta = modelRepo.BomAccessories.Dupatta;
                invBOM.MeterItem = modelRepo.BomAccessories.MeterItem;
                invBOM.BoltItem = modelRepo.BomAccessories.BoltItem;
                invBOM.DozenItem = modelRepo.BomAccessories.DozenItem;
                invBOM.MeterDozen = modelRepo.BomAccessories.MeterDozen;
                invBOM.MeterBolt = modelRepo.BomAccessories.MeterBolt;
                invBOM.BaleItem = modelRepo.BomAccessories.BaleItem;
                invBOM.DozenBale = modelRepo.BomAccessories.DozenBale;
                invBOM.BoltBale = modelRepo.BomAccessories.BoltBale;
                invBOM.BaleMeter = modelRepo.BomAccessories.BaleMeter;
                invBOM.CartonTypeId = modelRepo.BomAccessories.CartonTypeId;
                invBOM.SpCarton = modelRepo.BomAccessories.SpCarton;
                invBOM.CartonMeter = modelRepo.BomAccessories.CartonMeter;
                invBOM.Remarks = modelRepo.BomAccessories.Remarks;
                invBOM.CreatedBy = modelRepo.BomAccessories.CreatedBy;
                invBOM.CreatedDate = DateTime.Now.Date;
                invBOM.IsActive = true;
                invBOM.IsDeleted = false;
                invBOM.Status = "Created";
                invBOM.UpdatedBy = invBOM.UpdatedBy;
                invBOM.ApprovedBy = invBOM.ApprovedBy;
                invBOM.UnApprovedBy = invBOM.UnApprovedBy;
                invBOM.CompanyId = modelRepo.BomAccessories.CompanyId;
                invBOM.ItemType = modelRepo.BomAccessories.ItemType;
                _dbContext.BomAccessories.Add(invBOM);
                _dbContext.SaveChanges();
                foreach (var item in InvBOMDetail)
                {
                    BomAccessoriesDetail invBOMDetail = new BomAccessoriesDetail();
                    invBOMDetail.BOMId = invBOM.Id;
                    invBOMDetail.GeneralAccessoriesId = invBOM.Id;
                    invBOMDetail.ItemSpecifiedId = invBOM.Id;
                    invBOMDetail.ItemId = item.ItemId;
                    invBOMDetail.Quantity = item.Quantity;
                    invBOMDetail.UOMId = item.UOMId;
                    invBOMDetail.NatureId = item.NatureId;
                    _dbContext.BomAccessoriesDetail.Add(invBOMDetail);
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
        public async Task<bool> Delete(int id)
        {
            var deleteAccount = _dbContext.BomAccessories.Find(id);
            deleteAccount.IsDeleted = true;
            var entry = _dbContext.BomAccessories.Update(deleteAccount);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
        [HttpPost]
        public async Task<bool> Update(BomAccessoriesViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                BomAccessoriesDetail[] InvBOMDetail = JsonConvert.DeserializeObject<BomAccessoriesDetail[]>(collection["Detail"]);
                BomAccessories invBOM = _dbContext.BomAccessories.FirstOrDefault(x => x.Id == modelRepo.BomAccessories.Id);
                invBOM.TransactionNo = modelRepo.BomAccessories.TransactionNo;
                invBOM.TransactionDate = modelRepo.BomAccessories.TransactionDate;
                invBOM.SecondItemCategoryId = modelRepo.BomAccessories.SecondItemCategoryId;
                invBOM.FourthItemCategoryId = modelRepo.BomAccessories.FourthItemCategoryId;
                invBOM.Kameez = modelRepo.BomAccessories.Kameez;
                invBOM.Shalwar = modelRepo.BomAccessories.Shalwar;
                invBOM.Dupatta = modelRepo.BomAccessories.Dupatta;
                invBOM.MeterItem = modelRepo.BomAccessories.MeterItem;
                invBOM.BoltItem = modelRepo.BomAccessories.BoltItem;
                invBOM.DozenItem = modelRepo.BomAccessories.DozenItem;
                invBOM.MeterDozen = modelRepo.BomAccessories.MeterDozen;
                invBOM.MeterBolt = modelRepo.BomAccessories.MeterBolt;
                invBOM.BaleItem = modelRepo.BomAccessories.BaleItem;
                invBOM.DozenBale = modelRepo.BomAccessories.DozenBale;
                invBOM.BoltBale = modelRepo.BomAccessories.BoltBale;
                invBOM.BaleMeter = modelRepo.BomAccessories.BaleMeter;
                invBOM.CartonTypeId = modelRepo.BomAccessories.CartonTypeId;
                invBOM.SpCarton = modelRepo.BomAccessories.SpCarton;
                invBOM.CartonMeter = modelRepo.BomAccessories.CartonMeter;
                invBOM.Remarks = modelRepo.BomAccessories.Remarks;
                invBOM.UpdatedBy = modelRepo.BomAccessories.UpdatedBy;
                invBOM.UpdatedDate = DateTime.Now.Date;
                invBOM.IsActive = true;
                invBOM.IsDeleted = false;
                invBOM.CompanyId = modelRepo.BomAccessories.CompanyId;
                invBOM.ItemType = modelRepo.BomAccessories.ItemType;
                _dbContext.BomAccessories.Update(invBOM);
                _dbContext.SaveChanges();
                var existingDetail = _dbContext.BomAccessoriesDetail.Where(x => x.BOMId == invBOM.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = InvBOMDetail.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.BomAccessoriesDetail.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in InvBOMDetail)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        BomAccessoriesDetail invBOMDetail = new BomAccessoriesDetail();
                        invBOMDetail.BOMId = invBOM.Id;
                        invBOMDetail.GeneralAccessoriesId = detail.GeneralAccessoriesId;
                        invBOMDetail.ItemSpecifiedId = detail.ItemSpecifiedId;
                        invBOMDetail.ItemId = detail.ItemId;
                        invBOMDetail.Quantity = detail.Quantity;
                        invBOMDetail.UOMId = detail.UOMId;
                        invBOMDetail.NatureId = detail.NatureId;
                        _dbContext.BomAccessoriesDetail.Add(invBOMDetail);
                        await _dbContext.SaveChangesAsync();

                    }
                    else   //Updating Records
                    {
                        var invBOMDetail = _dbContext.BomAccessoriesDetail.FirstOrDefault(x => x.Id == detail.Id);
                        invBOMDetail.BOMId = invBOM.Id;
                        invBOMDetail.GeneralAccessoriesId = detail.GeneralAccessoriesId;
                        invBOMDetail.ItemSpecifiedId = detail.ItemSpecifiedId;
                        invBOMDetail.ItemId = detail.ItemId;
                        invBOMDetail.Quantity = detail.Quantity;
                        invBOMDetail.UOMId = detail.UOMId;
                        invBOMDetail.NatureId = detail.NatureId;
                        _dbContext.BomAccessoriesDetail.Update(invBOMDetail);
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