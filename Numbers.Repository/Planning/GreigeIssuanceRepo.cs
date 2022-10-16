using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Planning
{
    public class GreigeIssuanceRepo
    {
        private readonly NumbersDbContext _dbContext;
        private HttpContext HttpContext { get; }
        public GreigeIssuanceRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public GreigeIssuanceRepo(NumbersDbContext context, HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.GreigeIssuance.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x => x.TransactionNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Create(GreigeIssuanceViewModel modelRepo)
        {
            try
            {
                //Add Master
                GreigeIssuance model = new GreigeIssuance();
                model.TransactionNo = modelRepo.GreigeIssuance.TransactionNo;
                model.TransactionDate = modelRepo.GreigeIssuance.TransactionDate;
                model.SpecificationId = modelRepo.GreigeIssuance.SpecificationId;
                model.process = modelRepo.GreigeIssuance.process;
                model.VendorId = modelRepo.GreigeIssuance.VendorId;
                model.IssueTypeId = modelRepo.GreigeIssuance.IssueTypeId;
                model.WareHouseId = modelRepo.GreigeIssuance.WareHouseId;
                model.CreatedBy = modelRepo.GreigeIssuance.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                model.IsActive = true;
                model.IsDeleted = false;
                model.CompanyId = modelRepo.GreigeIssuance.CompanyId;
                model.Resp_Id = modelRepo.GreigeIssuance.Resp_Id;
                _dbContext.GreigeIssuance.Add(model);
                _dbContext.SaveChanges();
                
                //Add Detail
                foreach (var item in modelRepo.GreigeIssuanceDetails)
                {
                    GreigeIssuanceDetail detail = new GreigeIssuanceDetail();
                    detail.GreigeIssuanceId = model.Id;
                    detail.ProductionOrderDetailId = item.ProductionOrderDetailId;
                    detail.ProductionId = item.ProductionId;
                   
                    detail.GreigeQualityId = item.GreigeQualityId;
                    detail.RequiredQty = item.RequiredQty;
                    detail.AvailableQty = item.AvailableQty;
                    detail.IssuanceQty = item.IssuanceQty;
                    

                    //var season = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == detail.SeasonId).ConfigValue;
                    //var startWith = season.Substring(0, 1).ToUpper();
                    //var year = Convert.ToInt32(season.Split('-').Last().Trim());
                    //var x = new DateTime().AddYears(year - 1);
                    //var startDate = "";
                    //var endDate = "";
                    //switch (startWith)
                    //{
                    //    case "S":
                    //        startDate = x.AddMonths(3).ToString("d-MMM-yyyy");
                    //        endDate = x.AddMonths(8).ToString("d-MMM-yyyy");
                    //        break;
                    //    case "W":
                    //        startDate = x.AddMonths(9).ToString("d-MMM-yyyy");
                    //        endDate = x.AddMonths(14).ToString("d-MMM-yyyy");
                    //        break;
                    //}
                   
                    _dbContext.GreigeIssuanceDetail.Add(detail);
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
        [HttpPost]
        public async Task<bool> Update(GreigeIssuanceViewModel modelRepo)
        {
            try
            {
                GreigeIssuance model = _dbContext.GreigeIssuance.FirstOrDefault(x => x.Id == modelRepo.GreigeIssuance.Id);
                model.TransactionNo = modelRepo.GreigeIssuance.TransactionNo;
                model.TransactionDate = modelRepo.GreigeIssuance.TransactionDate;
                model.SpecificationId = modelRepo.GreigeIssuance.SpecificationId;
                model.process = modelRepo.GreigeIssuance.process;
                model.VendorId = modelRepo.GreigeIssuance.VendorId;
                model.IssueTypeId = modelRepo.GreigeIssuance.IssueTypeId;
                model.WareHouseId = modelRepo.GreigeIssuance.WareHouseId;
                model.UpdatedBy = modelRepo.GreigeIssuance.UpdatedBy;
                model.UpdatedDate = DateTime.Now.Date;

                _dbContext.GreigeIssuance.Update(model);
                var existingDetail = _dbContext.GreigeIssuanceDetail.Where(x => x.GreigeIssuanceId == modelRepo.GreigeIssuance.Id).ToList();
                //Deleting detail
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.GreigeIssuanceDetails.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.GreigeIssuanceDetail.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.GreigeIssuanceDetails)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        GreigeIssuanceDetail Items = new GreigeIssuanceDetail();
                        Items.GreigeIssuanceId = model.Id;
                        Items.ProductionOrderDetailId = detail.ProductionOrderDetailId;
                        Items.GreigeQualityId = detail.GreigeQualityId;
                        Items.RequiredQty = detail.RequiredQty;
                        Items.AvailableQty = detail.AvailableQty;
                        Items.IssuanceQty = detail.IssuanceQty;
                        await _dbContext.GreigeIssuanceDetail.AddAsync(Items);

                    }
                    else   //Updating Records
                    {
                        GreigeIssuanceDetail Items = _dbContext.GreigeIssuanceDetail.FirstOrDefault(x => x.Id == detail.Id);
                        Items.ProductionOrderDetailId = detail.ProductionOrderDetailId;
                        Items.GreigeIssuanceId = model.Id;
                        Items.GreigeQualityId = detail.GreigeQualityId;
                        Items.RequiredQty = detail.RequiredQty;
                        Items.AvailableQty = detail.AvailableQty;
                        Items.IssuanceQty = detail.IssuanceQty;        
                        _dbContext.GreigeIssuanceDetail.Update(Items);
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
            var deleteItem = _dbContext.GreigeIssuance.Where(v => v.IsDeleted == false && v.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.GreigeIssuance.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            GreigeIssuance issuance = _dbContext.GreigeIssuance
                .Include(x => x.Vendor)
                .Include(x => x.GreigeIssuanceDetail)
                    .ThenInclude(x => x.GreigeQuality)
                        .ThenInclude(x => x.Item)
                            .ThenInclude(x => x.InvItemAccount)
           .Where(a => !a.IsApproved && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
           .FirstOrDefault();
            try
            {
                //Create Voucher  
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Greige Issue # : {0} ",
                issuance.TransactionNo);

                int voucherId = 0;
                voucherMaster.VoucherType = "GIssue";
                voucherMaster.VoucherDate = issuance.TransactionDate;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Greige/GreigeIssuance";
                voucherMaster.ModuleId = id;

                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                #region Debit
                var quantity = issuance.GreigeIssuanceDetail.Sum(x => x.IssuanceQty);
                var rate = issuance.GreigeIssuanceDetail.Sum(x => x.GreigeQuality.Item.StockValue);
                var amount = quantity * rate;
                var GLWIPAccountId = issuance.GreigeIssuanceDetail.Max(x => x.GreigeQuality.Item.InvItemAccount.GLWIPAccountId);
                var GLAssetAccountId = issuance.GreigeIssuanceDetail.Max(x => x.GreigeQuality.Item.InvItemAccount.GLAssetAccountId);

                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = GLWIPAccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = amount;
                //voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = issuance.Vendor.AccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = amount;
                //voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion Debit
                //credit entry

                #region Credit

                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = GLAssetAccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                //voucherDetail.Debit = 0;
                voucherDetail.Credit = amount;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = 347;  //GL Account : RESERVE ACCOUNT
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                //voucherDetail.Debit = 0;
                voucherDetail.Credit = amount;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion Credit

                //Create Voucher 
                var helper = new Numbers.Repository.Helpers.VoucherHelper(_dbContext, HttpContext);
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        GreigeIssuance model = _dbContext.GreigeIssuance.Find(id);
                        model.VoucherId = voucherId;
                        model.ApprovedBy = userId;
                        model.ApprovedDate = DateTime.UtcNow;
                        model.IsApproved = true;
                        var entry = _dbContext.GreigeIssuance.Update(model);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                //transaction.Rollback();
                return false;
            }
        }

    }
}
