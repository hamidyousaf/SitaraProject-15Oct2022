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
  public  class BOMRepository
    {
        private readonly NumbersDbContext _dbContext;
        public BOMRepository(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        public async Task<bool> Create(InvBOMViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                InvBOMDetail[] InvBOMDetail = JsonConvert.DeserializeObject<InvBOMDetail[]>(collection["Detail"]);
                InvBOM invBOM = new InvBOM();
                invBOM.Id = modelRepo.InvBOM.Id;
                invBOM.TransactionNo = modelRepo.InvBOM.TransactionNo;
                invBOM.TransactionDate = modelRepo.InvBOM.TransactionDate;
                invBOM.SecondItemCategoryId = modelRepo.InvBOM.SecondItemCategoryId;
                invBOM.FourthItemCategoryId = modelRepo.InvBOM.FourthItemCategoryId;
                invBOM.CreatedBy = modelRepo.InvBOM.CreatedBy;
                invBOM.CreatedDate = DateTime.Now.Date;
                invBOM.IsActive = true;
                invBOM.IsDeleted = false;
                invBOM.Status = "Created";
                invBOM.CompanyId = modelRepo.InvBOM.CompanyId;
                _dbContext.InvBOM.Add(invBOM);
                _dbContext.SaveChanges();
                foreach (var item in InvBOMDetail)
                {
                    InvBOMDetail invBOMDetail = new InvBOMDetail();
                    invBOMDetail.BOMId = invBOM.Id;
                    invBOMDetail.ItemId = item.ItemId;
                    invBOMDetail.Quantity = item.Quantity;
                    invBOMDetail.UOMId = item.UOMId;
                    invBOMDetail.NatureId = item.NatureId;
                    _dbContext.InvBOMDetail.Add(invBOMDetail);
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
            var deleteAccount = _dbContext.InvBOM.Find(id);
            deleteAccount.IsDeleted = true;
            var entry = _dbContext.InvBOM.Update(deleteAccount);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
        [HttpPost]
        public async Task<bool> Update(InvBOMViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                InvBOMDetail[] InvBOMDetail = JsonConvert.DeserializeObject<InvBOMDetail[]>(collection["Detail"]);
                InvBOM invBOM = _dbContext.InvBOM.FirstOrDefault(x => x.Id == modelRepo.InvBOM.Id);
                invBOM.TransactionNo = modelRepo.InvBOM.TransactionNo;
                invBOM.TransactionDate = modelRepo.InvBOM.TransactionDate;
                invBOM.SecondItemCategoryId = modelRepo.InvBOM.SecondItemCategoryId;
                invBOM.FourthItemCategoryId = modelRepo.InvBOM.FourthItemCategoryId;
                invBOM.UpdatedBy = modelRepo.InvBOM.UpdatedBy;
                invBOM.UpdatedDate = DateTime.Now.Date;
                invBOM.IsActive = true;
                invBOM.IsDeleted = false;
                invBOM.CompanyId = modelRepo.InvBOM.CompanyId;
                _dbContext.InvBOM.Update(invBOM);
                _dbContext.SaveChanges();
                var existingDetail = _dbContext.InvBOMDetail.Where(x => x.BOMId == invBOM.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = InvBOMDetail.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.InvBOMDetail.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in InvBOMDetail)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        InvBOMDetail invBOMDetail = new InvBOMDetail();
                        invBOMDetail.BOMId = invBOM.Id;
                        invBOMDetail.ItemId = detail.ItemId;
                        invBOMDetail.Quantity = detail.Quantity;
                        invBOMDetail.UOMId = detail.UOMId;
                        invBOMDetail.NatureId = detail.NatureId;
                        _dbContext.InvBOMDetail.Add(invBOMDetail);
                        await _dbContext.SaveChangesAsync();

                    }
                    else   //Updating Records
                    {
                        var invBOMDetail = _dbContext.InvBOMDetail.FirstOrDefault(x => x.Id == detail.Id);
                        invBOMDetail.BOMId = invBOM.Id;
                        invBOMDetail.ItemId = detail.ItemId;
                        invBOMDetail.Quantity = detail.Quantity;
                        invBOMDetail.UOMId = detail.UOMId;
                        invBOMDetail.NatureId = detail.NatureId;
                        _dbContext.InvBOMDetail.Update(invBOMDetail);
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