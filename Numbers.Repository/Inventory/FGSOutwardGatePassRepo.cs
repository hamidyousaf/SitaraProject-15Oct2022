using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Inventory
{
   public class FGSOutwardGatePassRepo
    {
        //private readonly NumbersDbContext _dbContext;
        //public FGSOutwardGatePassRepo(NumbersDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}
        //[HttpPost]
        //        public async Task<bool> Create(FGSOutwardGatePassViewModel modelRepo, IFormCollection collection)
        //        {
        //            try
        //            {
        //                FGSOutwardGatePassDetails[] fGSOutwardGatePassDetails = JsonConvert.DeserializeObject<fGSOutwardGatePassDetails[]>(collection["Detail"]);
        //                InvBOM invBOM = new InvBOM();
        //                invBOM.Id = modelRepo.InvBOM.Id;
        //                invBOM.TransactionNo = modelRepo.InvBOM.TransactionNo;
        //                invBOM.TransactionDate = modelRepo.InvBOM.TransactionDate;
        //                invBOM.SecondItemCategoryId = modelRepo.InvBOM.SecondItemCategoryId;
        //                invBOM.FourthItemCategoryId = modelRepo.InvBOM.FourthItemCategoryId;
        //                invBOM.CreatedBy = modelRepo.InvBOM.CreatedBy;
        //                invBOM.CreatedDate = DateTime.Now.Date;
        //                invBOM.IsActive = true;
        //                invBOM.IsDeleted = false;
        //                invBOM.Status = "Created";
        //                invBOM.CompanyId = modelRepo.InvBOM.CompanyId;
        //                _dbContext.InvBOM.Add(invBOM);
        //                _dbContext.SaveChanges();
        //                foreach (var item in InvBOMDetail)
        //                {
        //                    InvBOMDetail invBOMDetail = new InvBOMDetail();
        //                    invBOMDetail.BOMId = invBOM.Id;
        //                    invBOMDetail.ItemId = item.ItemId;
        //                    invBOMDetail.Quantity = item.Quantity;
        //                    invBOMDetail.UOMId = item.UOMId;
        //                    invBOMDetail.NatureId = item.NatureId;
        //                    _dbContext.InvBOMDetail.Add(invBOMDetail);
        //                    await _dbContext.SaveChangesAsync();
        //                }
        //                await _dbContext.SaveChangesAsync();
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.InnerException.Message);
        //                string message = ex.Message.ToString();
        //                return false;
        //            }
        //        }
    }
}
