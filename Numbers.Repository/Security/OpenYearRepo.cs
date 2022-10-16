using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Security
{
    public class OpenYearRepo
    {
        private readonly NumbersDbContext _dbContext;
        private readonly string _userId;
        private readonly int _companyId;

        public OpenYearRepo(NumbersDbContext dbContext, string userId, int companyId)
        {
            _userId = userId;
            _companyId = companyId;
            _dbContext = dbContext;
        }
        public HROpenMonthViewModel GetYear()
        {
            HROpenMonthViewModel model = new HROpenMonthViewModel();
            string Period_ID = "";
            var periods = _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId).ToList();
            if (periods.Count != 0)
            {
                periods = _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.PayrollOpen == true).ToList();
                var data = periods.Select(x => x.Code).LastOrDefault();
                if (data != null)
                {
                    string period = data.Substring(4, 2);
                    if (period == "12")
                        Period_ID = (Convert.ToInt32(data) + 89).ToString();
                    else
                        Period_ID = (Convert.ToInt32(data) + 1).ToString();
                    periods = _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.Code == data).ToList();
                    if (periods.Count != 0)
                    {
                        //model.PeriodId = periods.Select(x => x.Code).FirstOrDefault();
                        model.PeriodId =Period_ID;
                        var year = periods.Select(x => x.Description).FirstOrDefault();
                        if (periods.FirstOrDefault().Description.Length > 4)
                        {
                              year = periods.Select(x => x.Description).FirstOrDefault().Substring(4, 4);
                        }else
                        {
                              year = periods.Select(x => x.Description).FirstOrDefault();
                        }
                        var monthname = periods.Select(x => x.Description).FirstOrDefault().Substring(0,3);
                        model.MonthDescrption = monthname + "-" + (Convert.ToInt32(year)).ToString();
                        model.ShortDescription =   (Convert.ToInt32(year)).ToString();
                       // model.MonthDescrption = (Convert.ToInt32(year)).ToString();
                        var Totalmodules = _dbContext.AppModules.ToList().Count;
                        var TotalPeriod = _dbContext.AppPeriods.Where(x=>x.Description==model.MonthDescrption).ToList().Count;
                        DateTime month = (periods.Select(x => x.EndDate).LastOrDefault());
                        model.EndDate = Convert.ToDateTime(month);
                        //DateTime.DaysInMonth(2020, 02)
                        model.EndDate = getnewDate(model.EndDate);
                        if (Totalmodules == TotalPeriod)
                        {
                            month = (periods.Select(x => x.EndDate).LastOrDefault()).AddHours(1);
                            model.EndDate = Convert.ToDateTime(month);
                            model.MonthDescrption = monthname + "-" + (Convert.ToInt32(year) + 1).ToString();
                            model.ShortDescription =(Convert.ToInt32(year) + 1).ToString();
                            // model.MonthDescrption =  (Convert.ToInt32(year) + 1).ToString();
                        }
                    }
                }
            }
            return model;
        }

        public DateTime getnewDate(DateTime EndDate)
        {
            int m = EndDate.Month;
            int y = EndDate.Year;
            var d = DateTime.DaysInMonth(y, m);
            var dateFirstrDay = new DateTime(y, m, 1);
            var dateLastDay = new DateTime(y, m, d);
            EndDate = dateLastDay;
            return EndDate;
        }

        public async Task<bool> Create(HROpenMonthViewModel model, IFormCollection collection)
        {
            bool status = false;
            try
            {
                var Periods = new AppPeriod();
                   Periods.Code = model.PeriodId;
                    Periods.Description = model.MonthDescrption;
                    Periods.ShortDescription = model.ShortDescription;
                    Periods.PayrollOpen = true;
                    Periods.ModuleId = model.ModuleId;
                Periods.StartDate = DateTime.Now;
                //Periods.EndDate = DateTime.Now.AddYears(1);
                Periods.EndDate = model.EndDate;
                Periods.CompanyId = _companyId;
                Periods.CreatedBy = _userId;
                Periods.IsGLClosed = false;
                Periods.PayrollClose = false;
                Periods.PayrollOpen = true;
                Periods.CreatedDate = DateTime.Now;
                Periods.Type = "Year";
                _dbContext.AppPeriods.Add(Periods);
                    await _dbContext.SaveChangesAsync();
              
                return true;

            }
            catch (Exception ex)
            {
                string msg = ex.InnerException.Message.ToString();
                return false;
            }
        }

        public async Task<bool> Close(int id)
        {
            var periodClose = _dbContext.AppPeriods.Find(id);
            periodClose.IsGLClosed = true;
            periodClose.PayrollOpen = false;
            var entry = _dbContext.AppPeriods.Update(periodClose);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
