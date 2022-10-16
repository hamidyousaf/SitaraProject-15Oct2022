using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Greige
{
    public class ContractRepo
    {
        private readonly NumbersDbContext _dbContext;
        public ContractRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<GRWeavingContract> GetAll(int companyId)
        {
            IEnumerable<GRWeavingContract> listRepo = _dbContext.GRWeavingContracts.Where(v => v.IsDeleted == false && v.CompanyId == companyId)
                .OrderByDescending(v => v.Id).ToList();
            return listRepo;
        }

        //for photo uploading function
        public async Task<string> UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\item-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await  img.CopyToAsync(Fstream);
                        var fullPath = "/uploads/item-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }
        
       
        public GRWeavingContract GetById(int id)
        {
            var item= _dbContext.GRWeavingContracts.Find(id);
            return item;
        }
       
        [HttpPost]
        public async Task<bool> Create(GRWeavingContract model, IFormFile Photo)
        {
            try
            { 
                var item = new GRWeavingContract();
                 item = model;
                item.TransactionDate = model.TransactionDate;
                item.DeliveryDate = model.DeliveryDate;
                //item.TransactionDate = model.TransactionDate;
                //item.TransactionNo = model.TransactionNo;
                //item.VendorId = model.VendorId;
                ////item.DeliveryDate = model.DeliveryDate;
                //item.GreigeQualityId = model.GreigeQualityId;
                //item.GreigeQualityLoomId = model.GreigeQualityLoomId;
                //item.NoOfLooms = model.NoOfLooms;
                //item.ContractQty = model.ContractQty;
                //item.Width = model.Width;
                //item.Picks = model.Picks;
                //item.RatePerPicks = model.RatePerPicks;
                //item.Reed = model.Reed;
                //item.AdditionalReed = model.AdditionalReed;
                //item.SaleTax = model.SaleTax;
                //item.Warp = model.Warp;
                //item.WarpCount = model.WarpCount;
                //item.WarpWeightPerMeter = model.WarpWeightPerMeter;
                //item.TotalWarpBags = model.TotalWarpBags;
                //item.WarpRatePound = model.WarpRatePound;
                //item.Weft = model.Weft;
                //item.WeftCount = model.WeftCount;
                //item.WeftRatePound = model.WeftRatePound;
                //item.WeftWeightPerMeter = model.WeftWeightPerMeter;
                //item.TotalWeftBags = model.TotalWeftBags;
                //item.RateOfConversionExcTax = model.RateOfConversionExcTax;
                //item.SaleTaxAmount = model.SaleTaxAmount;
                //item.RateOfConversionIncTax = model.RateOfConversionIncTax;
                //item.PriceOfYarn = model.PriceOfYarn;
                //item.ValueOfGreige = model.ValueOfGreige;
                //item.TotalContractAmount = model.TotalContractAmount;
                item.CompanyId = model.CompanyId;
                item.IsDeleted = false;
                item.CreatedBy = model.CreatedBy;
                item.BalanceContractQty = model.ContractQty;
                item.Status = "Created";
                item.GRRequisitionId = model.GRRequisitionId;
               // item.CreatedDate = DateTime.Now; 
                item.UpdatedBy = "";
                item.UpdatedDate = DateTime.Now;
                // handle balance
                GRGriegeRequisitionDetails details = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x=>x.GRRequisitionId == item.GRRequisitionId && x.GriegeQualityId == item.GreigeQualityId);
                details.BalanceQty = details.BalanceQty - Convert.ToInt32(item.ContractQty);
                _dbContext.GRGriegeRequisitionDetails.Update(details);
                //---
                _dbContext.GRWeavingContracts.Add(item);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(GRWeavingContract model, IFormFile Photo)
        {
            var obj = _dbContext.GRWeavingContracts.Find(model.Id);
            //if (obj.GreigeQualityId != model.GreigeQualityId)
            //{
            //    var updated = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x=>x.GRRequisitionId == obj.GRRequisitionId && x.GriegeQualityId == obj.GreigeQualityId);
            //    updated.IsUsed = false;
            //    var update = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.GRRequisitionId == model.GRRequisitionId && x.GriegeQualityId == model.GreigeQualityId);
            //    update.IsUsed = true;
            //}


            // Handle Balance Quantity
                GRGriegeRequisitionDetails requisition = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.GRRequisitionId == model.GRRequisitionId);
            if (obj.ContractQty < model.ContractQty)
            {
                requisition.BalanceQty = requisition.BalanceQty - (Convert.ToInt32(model.ContractQty) - Convert.ToInt32(obj.ContractQty));
            }
            if (obj.ContractQty > model.ContractQty)
            {
                requisition.BalanceQty = requisition.BalanceQty + (Convert.ToInt32(obj.ContractQty) - Convert.ToInt32(model.ContractQty));
            }
            ////-----
            _dbContext.GRGriegeRequisitionDetails.Update(requisition);
            obj.TransactionDate = model.TransactionDate;
            obj.VendorId = model.VendorId;
            obj.DeliveryDate = model.DeliveryDate;
            obj.GreigeQualityId = model.GreigeQualityId;
            obj.GreigeQualityLoomId = model.GreigeQualityLoomId;
            obj.NoOfLooms = model.NoOfLooms;
            obj.ContractQty = model.ContractQty;
            obj.BalanceContractQty = model.ContractQty;
            obj.Width = model.Width;
            obj.Picks = model.Picks;
            obj.RatePerPicks = model.RatePerPicks;
            obj.Reed = model.Reed;
            obj.AdditionalReed = model.AdditionalReed;
            obj.SaleTax = model.SaleTax;
            obj.Warp = model.Warp;
            obj.WarpCount = model.WarpCount;
            obj.WarpWeightPerMeter = model.WarpWeightPerMeter;
            obj.TotalWarpBags = model.TotalWarpBags;
            obj.WarpRatePound = model.WarpRatePound;
            obj.Weft = model.Weft;
            obj.WeftCount = model.WeftCount;
            obj.WeftRatePound = model.WeftRatePound;
            obj.WeftWeightPerMeter = model.WeftWeightPerMeter;
            obj.TotalWeftBags = model.TotalWeftBags;
            obj.RateOfConversionExcTax = model.RateOfConversionExcTax;
            obj.SaleTaxAmount = model.SaleTaxAmount;
            obj.RateOfConversionIncTax = model.RateOfConversionIncTax;
            obj.PriceOfYarn = model.PriceOfYarn;
            obj.ValueOfGreige = model.ValueOfGreige;
            obj.TotalContractAmount = model.TotalContractAmount;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            obj.GRRequisitionId = model.GRRequisitionId;





            var entry = _dbContext.GRWeavingContracts.Update(obj);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id , int companyId)
        {
            
            var deleteItem = _dbContext.GRWeavingContracts.Where(v => v.IsDeleted == false && v.Id == id &&v.CompanyId==companyId ).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.GRWeavingContracts.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;  
            }
        }
 
    }
}
