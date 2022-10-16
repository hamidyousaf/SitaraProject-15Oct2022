using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Planning
{
    public class SeasonalPlanningRepo
    {
        private readonly NumbersDbContext _dbContext;
        public SeasonalPlanningRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.SeasonalPlaning.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x => x.TransactionNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Create(SeasonalPlaningViewModel modelRepo)
        {
            try
            {
                //Add Master
                SeasonalPlaning model = new SeasonalPlaning();
                model.TransactionNo = modelRepo.SeasonalPlaning.TransactionNo;
                model.TransactionDate = modelRepo.SeasonalPlaning.TransactionDate;
                model.SeasonId = modelRepo.SeasonalPlaning.SeasonId;
                model.CreatedBy = modelRepo.SeasonalPlaning.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                model.IsActive = true;
                model.IsDeleted = false;
                model.CompanyId = modelRepo.SeasonalPlaning.CompanyId;
                model.Resp_Id = modelRepo.SeasonalPlaning.Resp_Id;
                _dbContext.SeasonalPlaning.Add(model);
                _dbContext.SaveChanges();

                var season = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == model.SeasonId).ConfigValue;
                var startWith = season.Substring(0, 1).ToUpper();
                var year = Convert.ToInt32(season.Split('-').Last().Trim());
                var x = new DateTime().AddYears(year - 1);
                var startDate = "";
                var endDate = "";
                switch (startWith)
                {
                    case "S":
                        startDate = x.AddMonths(3).ToString("d-MMM-yyyy");
                        endDate = x.AddMonths(8).ToString("d-MMM-yyyy");
                        break;
                    case "W":
                        startDate = x.AddMonths(9).ToString("d-MMM-yyyy");
                        endDate = x.AddMonths(15).AddDays(-1).ToString("d-MMM-yyyy");
                        break;
                }

                //Add Detail
                foreach (var item in modelRepo.SeasonalPlaningDetail)
                {
                    SeasonalPlaningDetail detail = new SeasonalPlaningDetail();
                    detail.SeasonalPlaningId = model.Id;
                    detail.SpecificationId = item.SpecificationId;
                    detail.FourthItemCategoryId = item.FourthItemCategoryId;
                    detail.GreigeQualityId = item.GreigeQualityId;
                    detail.SeasonId = item.SeasonId;
                    detail.StartDate = Convert.ToDateTime(startDate);
                    detail.EndDate = Convert.ToDateTime(endDate);
                    detail.Volume = item.Volume;
                    detail.DesignCount = item.DesignCount;
                    detail.DesignPerVolume = item.DesignPerVolume;
                    detail.DesignRun = item.DesignRun;
                    detail.FabricConsumption = item.FabricConsumption;
                    detail.BalanceDesignCount = item.DesignCount;
                    //detail.BalanceDesignRun = item.DesignRun;
                    detail.BalanceFabricConsumption = item.FabricConsumption;

                    _dbContext.SeasonalPlaningDetail.Add(detail);
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
        [HttpPost]
        public async Task<bool> Update(SeasonalPlaningViewModel modelRepo)
        {
            try
            {
                SeasonalPlaning model = _dbContext.SeasonalPlaning.FirstOrDefault(x => x.Id == modelRepo.SeasonalPlaning.Id);
                model.TransactionNo = modelRepo.SeasonalPlaning.TransactionNo;
                model.TransactionDate = modelRepo.SeasonalPlaning.TransactionDate;
                model.SeasonId = modelRepo.SeasonalPlaning.SeasonId;
                model.UpdatedBy = modelRepo.SeasonalPlaning.UpdatedBy;
                model.UpdatedDate = DateTime.Now.Date;
                _dbContext.SeasonalPlaning.Update(model);

                var season = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == model.SeasonId).ConfigValue;
                var startWith = season.Substring(0, 1).ToUpper();
                var year = Convert.ToInt32(season.Split('-').Last().Trim());
                var x = new DateTime().AddYears(year - 1);
                var startDate = "";
                var endDate = "";
                switch (startWith)
                {
                    case "S":
                        startDate = x.AddMonths(3).ToString("d-MMM-yyyy");
                        endDate = x.AddMonths(8).ToString("d-MMM-yyyy");
                        break;
                    case "W":
                        startDate = x.AddMonths(9).ToString("d-MMM-yyyy");
                        endDate = x.AddMonths(15).AddDays(-1).ToString("d-MMM-yyyy");
                        break;
                }

                var existingDetail = _dbContext.SeasonalPlaningDetail.Where(x => x.SeasonalPlaningId == modelRepo.SeasonalPlaning.Id).ToList();
                //Deleting detail
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.SeasonalPlaningDetail.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.SeasonalPlaningDetail.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.SeasonalPlaningDetail)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        SeasonalPlaningDetail Items = new SeasonalPlaningDetail();
                        Items.SeasonalPlaningId = model.Id;
                        Items.SpecificationId = detail.SpecificationId;
                        Items.FourthItemCategoryId = detail.FourthItemCategoryId;
                        Items.GreigeQualityId = detail.GreigeQualityId;
                        Items.SeasonId = detail.SeasonId;
                        Items.StartDate = Convert.ToDateTime(startDate);
                        Items.EndDate = Convert.ToDateTime(endDate);
                        Items.Volume = detail.Volume;
                        Items.DesignCount = detail.DesignCount;
                        Items.DesignPerVolume = detail.DesignPerVolume;
                        Items.DesignRun = detail.DesignRun;
                        Items.FabricConsumption = detail.FabricConsumption;
                        Items.BalanceDesignCount = detail.DesignCount;
                        //Items.BalanceDesignRun = detail.DesignRun;
                        Items.BalanceFabricConsumption = detail.FabricConsumption;
                        await _dbContext.SeasonalPlaningDetail.AddAsync(Items);
                    }
                    else   //Updating Records
                    {
                        SeasonalPlaningDetail Items = _dbContext.SeasonalPlaningDetail.FirstOrDefault(x => x.Id == detail.Id);
                        Items.SpecificationId = detail.SpecificationId;
                        Items.FourthItemCategoryId = detail.FourthItemCategoryId;
                        Items.GreigeQualityId = detail.GreigeQualityId;
                        Items.SeasonId = detail.SeasonId;
                        Items.StartDate = Convert.ToDateTime(startDate);
                        Items.EndDate = Convert.ToDateTime(endDate);
                        Items.Volume = detail.Volume;
                        Items.DesignCount = detail.DesignCount;
                        Items.DesignPerVolume = detail.DesignPerVolume;
                        Items.DesignRun = detail.DesignRun;
                        Items.FabricConsumption = detail.FabricConsumption;
                        Items.BalanceDesignCount = detail.DesignCount;
                        //Items.BalanceDesignRun = detail.DesignRun;
                        Items.BalanceFabricConsumption = detail.FabricConsumption;
                        _dbContext.SeasonalPlaningDetail.Update(Items);
                    }
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
            var deleteItem = _dbContext.SeasonalPlaning.Where(v => v.IsDeleted == false && v.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.SeasonalPlaning.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
