using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Greige
{
    public class GRNInvoiceRepo
    {
        private readonly NumbersDbContext _dbContext;
        private HttpContext HttpContext { get; }
        public GRNInvoiceRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GRNInvoiceRepo(NumbersDbContext context, HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }
        public GRNInvoiceViewModel GetById(int id)
        {
            GRGRNInvoice invoice = _dbContext.GRGRNInvoices.Include(x=>x.GRN).ThenInclude(x=>x.Vendor).FirstOrDefault(x=>x.Id == id);
            var viewModel = new GRNInvoiceViewModel();
            viewModel.Id = invoice.Id;
            viewModel.PurchaseNo = invoice.PurchaseNo;
            viewModel.PurchaseDate = invoice.PurchaseDate;
            viewModel.SupplierInvoiceNo = invoice.SupplierInvoiceNo;
            viewModel.SupplierInvoiceDate = invoice.SupplierInvoiceDate;
            viewModel.GRNId = invoice.GRNId;
            viewModel.Remarks = invoice.Remarks;
            viewModel.Vendor = invoice.GRN.Vendor.Name ?? "";
            viewModel.Address = invoice.GRN.Vendor.Address ?? "";
            viewModel.GRGRNInvoiceDetails = _dbContext.GRGRNInvoiceDetails.Include(x=>x.Item).Where(x => x.GRNId == id).ToArray();
            return viewModel;
        }
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.GRGRNInvoices.Where(x => x.IsDeleted != true).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x => x.PurchaseNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Create(GRNInvoiceViewModel modelRepo)
        {
            try
            {
                //Add Master
                GRGRNInvoice model = new GRGRNInvoice();
                model.PurchaseNo = modelRepo.PurchaseNo;
                model.PurchaseDate = modelRepo.PurchaseDate;
                model.SupplierInvoiceNo = modelRepo.SupplierInvoiceNo;
                model.SupplierInvoiceDate = modelRepo.SupplierInvoiceDate;
                model.GRNId = modelRepo.GRNId;
                model.Remarks = modelRepo.Remarks;
                model.Status = "Created";
                model.IsDeleted = false;
                model.CreatedBy = modelRepo.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                model.CompanyId = modelRepo.CompanyId;
                _dbContext.GRGRNInvoices.Add(model);
                _dbContext.SaveChanges();

                //Add Detail
                foreach (var item in modelRepo.GRGRNInvoiceDetails)
                {
                    GRGRNInvoiceDetail detail = new GRGRNInvoiceDetail();
                    detail.GRNId = model.Id;
                    detail.ItemId = item.ItemId;
                    detail.Quantity = item.Quantity;
                    detail.RatePerMeter = item.RatePerMeter;
                    detail.Amount = item.Amount;
                    detail.TotalPenaltyAmount = item.TotalPenaltyAmount;
                    detail.NetPenaltyAMount = item.NetPenaltyAMount;
                    detail.LessYarnPrice = item.LessYarnPrice;
                    detail.NetPayableAmount = item.NetPayableAmount;

                    _dbContext.GRGRNInvoiceDetails.Add(detail);
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
        public async Task<bool> Update(GRNInvoiceViewModel modelRepo)
        {
            try
            {
                GRGRNInvoice model = _dbContext.GRGRNInvoices.FirstOrDefault(x => x.Id == modelRepo.Id);
                model.PurchaseNo = modelRepo.PurchaseNo;
                model.PurchaseDate = modelRepo.PurchaseDate;
                model.SupplierInvoiceNo = modelRepo.SupplierInvoiceNo;
                model.SupplierInvoiceDate = modelRepo.SupplierInvoiceDate;
                model.GRNId = modelRepo.GRNId;
                model.Remarks = modelRepo.Remarks;
                model.IsDeleted = false;
                model.UpdatedBy = modelRepo.UpdatedBy;
                model.UpdatedDate = DateTime.Now;
                model.CompanyId = modelRepo.CompanyId;
                _dbContext.GRGRNInvoices.Update(model);
                var existingDetail = _dbContext.GRGRNInvoiceDetails.Where(x => x.GRNId == modelRepo.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.GRGRNInvoiceDetails.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.GRGRNInvoiceDetails.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.GRGRNInvoiceDetails)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        GRGRNInvoiceDetail Items = new GRGRNInvoiceDetail();

                        Items.GRNId = model.Id;
                        Items.ItemId = detail.ItemId;
                        Items.Quantity = detail.Quantity;
                        Items.RatePerMeter = detail.RatePerMeter;
                        Items.Amount = detail.Amount;
                        Items.TotalPenaltyAmount = detail.TotalPenaltyAmount;
                        Items.NetPenaltyAMount = detail.NetPenaltyAMount;
                        Items.LessYarnPrice = detail.LessYarnPrice;
                        Items.NetPayableAmount = detail.NetPayableAmount;

                        await _dbContext.GRGRNInvoiceDetails.AddAsync(Items);

                    }
                    else   //Updating Records
                    {
                        GRGRNInvoiceDetail Items = _dbContext.GRGRNInvoiceDetails.FirstOrDefault(x => x.Id == detail.Id);
                        Items.ItemId = detail.ItemId;
                        Items.Quantity = detail.Quantity;
                        Items.RatePerMeter = detail.RatePerMeter;
                        Items.Amount = detail.Amount;
                        Items.TotalPenaltyAmount = detail.TotalPenaltyAmount;
                        Items.NetPenaltyAMount = detail.NetPenaltyAMount;
                        Items.LessYarnPrice = detail.LessYarnPrice;
                        Items.NetPayableAmount = detail.NetPayableAmount;
                        _dbContext.GRGRNInvoiceDetails.Update(Items);
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
            var invoice = _dbContext.GRGRNInvoices.Find(id);
            invoice.IsDeleted = true;
            var entry = _dbContext.GRGRNInvoices.Update(invoice);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            GRGRNInvoice GRGRN = _dbContext.GRGRNInvoices
           .Where(a => a.Status != "Approved"  && a.Id == id && a.IsDeleted == false)
           .FirstOrDefault();
            try
            {
                //Create Voucher  
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "GRN Invoice # : {0} ",
                GRGRN.PurchaseNo);

                int voucherId = 0;
                voucherMaster.VoucherType = "G-GRNInv";
                voucherMaster.VoucherDate = GRGRN.PurchaseDate;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Greige/GRN";
                voucherMaster.ModuleId = id;

                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //debit entry
                #region Debit
                var costAccounts = (from li in _dbContext.GRGRNInvoices
                                    join grItems in _dbContext.GRGRNInvoiceDetails on li.Id equals grItems.GRNId
                                    where li.Id == id && li.IsDeleted == false
                                    select new
                                    {
                                        GRNInvoiceId = li.Id,
                                        Total = grItems.NetPayableAmount
                                    }).GroupBy(l => l.GRNInvoiceId)
                               .Select(li => new
                               {
                                   NetPayableAmount = li.Sum(c => c.Total),
                                   GRNId = li.FirstOrDefault().GRNInvoiceId //GRGRNId is temporarily containing GLAssetAccountId
                               }).ToList();
                var accountId = (from setup in _dbContext.AppCompanySetups.Where(x => x.Name == "Greige GRN Invoice Debit Account")
                                 join account in _dbContext.GLAccounts.Where(x => !x.IsDeleted) on setup.Value equals account.Code select account.Id).FirstOrDefault();

                foreach (var item in costAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    //voucherDetail.AccountId = item.GRGRNId;
                    voucherDetail.AccountId = accountId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = item.NetPayableAmount;
                    //voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Debit
                //credit entry supplier
                #region Credit
                var assetAccounts = (from li in _dbContext.GRGRNInvoices
                                     join grItems in _dbContext.GRGRNInvoiceDetails on li.Id equals grItems.GRNId
                                     where li.Id == id && li.IsDeleted == false
                                     select new
                                     {
                                         GRNInvoiceId = li.GRNId,
                                         Total = grItems.NetPayableAmount
                                     }).GroupBy(l => l.GRNInvoiceId)
                               .Select(li => new
                               {
                                   NetPayableAmount = li.Sum(c => c.Total),
                                   GRNId = li.FirstOrDefault().GRNInvoiceId //GRGRNId is temporarily containing GLAssetAccountId
                               }).ToList();
                foreach (var item in assetAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    if(item.GRNId!=0)
                    {
                        var weavingContract = _dbContext.GRGRNS.Include(x => x.WeavingContract).ThenInclude(x => x.Vendor).FirstOrDefault().Vendor.AccountId;
                        var purchaseContract = _dbContext.GRGRNS.Include(x => x.PurchaseContract).ThenInclude(x => x.Vendor).FirstOrDefault().Vendor.AccountId;
                        if (weavingContract != 0)
                        {
                            voucherDetail.AccountId = weavingContract;
                        }
                        else
                        {
                            voucherDetail.AccountId = purchaseContract;
                        }
                       
                    } 
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    //voucherDetail.Debit = 0;
                    voucherDetail.Credit = item.NetPayableAmount;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Credit
                //Create Voucher 
                var helper = new Numbers.Repository.Helpers.VoucherHelper(_dbContext, HttpContext);
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        //var GRGRN = _dbContext.GRGRNs.Find(id);
                        GRGRN.VoucherId = voucherId;
                        GRGRN.Status = "Approved";
                        GRGRN.ApprovedBy = userId;
                        GRGRN.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.GRGRNInvoices.Update(GRGRN);
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
