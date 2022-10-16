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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Security
{
    public class OpenMonthRepo
    {
        private readonly NumbersDbContext _dbContext;
        private readonly string _userId;
        private readonly int _companyId;

        public OpenMonthRepo(NumbersDbContext dbContext, string userId, int companyId)
        {
            _userId = userId;
            _companyId = companyId;
            _dbContext = dbContext;
        }
        //public HROpenMonthViewModel Getmonth()
        //{
        //    HROpenMonthViewModel model = new HROpenMonthViewModel();
        //    string Period_ID = "";
        //    var periods = _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.PayrollOpen == false && x.PayrollClose == false).ToList();
        //    if (periods.Count != 0)
        //    {
        //        periods = _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.PayrollOpen == true).ToList();
        //        var data = periods.Select(x => x.Code).LastOrDefault();
        //        if (data != null)
        //        {
        //            string period = data.Substring(4, 2);
        //            if (period == "12")
        //                Period_ID = (Convert.ToInt32(data) + 89).ToString();
        //            else
        //                Period_ID = (Convert.ToInt32(data) + 1).ToString();
        //            periods = _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.Code == Period_ID).ToList();
        //            if (periods.Count != 0)
        //            {
        //                model.PeriodId = periods.Select(x => x.Code).FirstOrDefault();
        //                model.MonthDescrption = periods.Select(x => x.Description).FirstOrDefault();
        //            }
        //        }
        //    }
        //    return model;
        //}

        public HROpenMonthViewModel Getmonth()
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
                        model.PeriodId = Period_ID;
                        var year = periods.Select(x => x.Description).FirstOrDefault();
                        if (periods.FirstOrDefault().Description.Length > 4)
                        {
                            year = periods.Select(x => x.Description).FirstOrDefault().Substring(4, 4);
                        }
                        else
                        {
                            year = periods.Select(x => x.Description).FirstOrDefault();
                        }
                        //var monthname = periods.Select(x => x.Description).FirstOrDefault().Substring(0, 3);
                        var monthname = Convert.ToDateTime(periods.Select(x=>x.EndDate).FirstOrDefault()).ToString("MMM", CultureInfo.InvariantCulture);
                        model.MonthDescrption = monthname + "-" + (Convert.ToInt32(year)).ToString();
                        model.ShortDescription = monthname ;
                        var Totalmodules = _dbContext.AppModules.ToList().Count;
                        var TotalPeriod = _dbContext.AppPeriods.Where(x => x.Description == model.MonthDescrption).ToList().Count;
                        DateTime month = (periods.Select(x => x.EndDate).LastOrDefault());
                        model.EndDate = Convert.ToDateTime(month);
                        //DateTime.DaysInMonth(2020, 02)
                        model.EndDate = getnewDate(model.EndDate);
                        if (Totalmodules == TotalPeriod)
                        {
                              month =( periods.Select(x => x.EndDate).LastOrDefault()).AddMonths(1);
                            model.EndDate = Convert.ToDateTime( month) ;
                              monthname = Convert.ToDateTime(month).ToString("MMM", CultureInfo.InvariantCulture);
                            model.MonthDescrption = monthname + "-" + (Convert.ToInt32(year)).ToString();
                            model.ShortDescription = monthname ;
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
                //Periods.EndDate = DateTime.Now.AddMonths(1);
                Periods.EndDate = model.EndDate;
                Periods.CompanyId = _companyId;
                Periods.CreatedBy = _userId;
                Periods.IsGLClosed = false;
                Periods.PayrollClose = false;
                Periods.PayrollOpen = true;
                Periods.CreatedDate = DateTime.Now;
                Periods.Type = "Month";
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


        //public async Task<bool> Create(HROpenMonthViewModel model, IFormCollection collection)
        //{
        //    bool status = false;
        //    try
        //    {
        //        var Groups = _dbContext.AppCompanyConfigs.Where(x => x.BaseId == 21 && x.CompanyId == _companyId && x.IsDeleted == false).ToList();
        //        for (int i = 0; i < Groups.Count; i++)
        //        {
        //            DateTime Period_start_date = (from app in _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.Code == model.PeriodId) select app.StartDate).FirstOrDefault();
        //            DateTime Period_end_date = (from app in _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.Code == model.PeriodId) select app.EndDate).FirstOrDefault();
        //            TimeSpan span = Period_end_date - Period_start_date;
        //            double noOfDays = span.TotalDays;
        //            HRRouster rouster = new HRRouster();
        //            rouster.PeriodId = model.PeriodId;
        //            rouster.GroupId = Groups[i].Id;
        //            rouster.Closed = false;
        //            rouster.CreatedBy = _userId;
        //            rouster.CreatedDate = DateTime.Now;
        //            _dbContext.HRRousters.Add(rouster);
        //            await _dbContext.SaveChangesAsync();
        //            var id = rouster.Id;
        //            int ShiftId = (from s in _dbContext.HRShifts.Where(s => s.IsDeleted == false) select s.Id).FirstOrDefault();
        //            for (int j = 0; j <= noOfDays; j++)
        //            {
        //                HRRousterGroupSchedule rousteritems = new HRRousterGroupSchedule();
        //                rousteritems.RousterId = id;
        //                rousteritems.ShiftId = Convert.ToInt32(ShiftId);
        //                rousteritems.GroupId = rouster.GroupId.ToString();
        //                rousteritems.PeriodId = model.PeriodId;
        //                rousteritems.RousterDate = Period_start_date;
        //                rousteritems.CreatedBy = _userId;
        //                rousteritems.CreatedDate = DateTime.Now;
        //                _dbContext.HRRousterGroupSchedules.Add(rousteritems);
        //                await _dbContext.SaveChangesAsync();
        //                Period_start_date = Period_start_date.AddDays(1);
        //            }

        //        }

        //        for (int i = 0; i < Groups.Count; i++)
        //        {
        //            DateTime Period_start_date = (from app in _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.Code == model.PeriodId) select app.StartDate).FirstOrDefault();
        //            DateTime Period_end_date = (from app in _dbContext.AppPeriods.Where(x => x.CompanyId == _companyId && x.Code == model.PeriodId) select app.EndDate).FirstOrDefault();
        //            TimeSpan span = Period_end_date - Period_start_date;
        //            double noOfDays = span.TotalDays;
        //            HRRestdayRoster rest = new HRRestdayRoster();
        //            rest.Period_id = model.PeriodId;
        //            rest.Group_ID = Groups[i].Id;
        //            rest.Closed = false;

        //            _dbContext.HRRestdayRoster.Add(rest);
        //            await _dbContext.SaveChangesAsync();
        //            var id = rest.Rouster_id;
        //            int ShiftId = (from s in _dbContext.HRShifts.Where(s => s.IsDeleted == false) select s.Id).FirstOrDefault();
        //            for (int j = 0; j <= noOfDays; j++)
        //            {
        //                HRRestdayRosterItems rousteritem = new HRRestdayRosterItems();
        //                rousteritem.Rouster_id = id;
        //                rousteritem.ShiftId = Convert.ToInt32(ShiftId);
        //                rousteritem.Group_ID = rest.Group_ID;
        //                rousteritem.Period_ID = model.PeriodId;
        //                rousteritem.Rouster_Date = Period_start_date;

        //                _dbContext.HRRestdayRosterItems.Add(rousteritem);
        //                await _dbContext.SaveChangesAsync();
        //                Period_start_date = Period_start_date.AddDays(1);
        //            }
        //            status = true;
        //        }
        //        //if (status == true)
        //        //{
        //            var Periods = _dbContext.AppPeriods.Where(x => x.Code == model.PeriodId && x.CompanyId == _companyId).FirstOrDefault();
        //            Periods.PayrollOpen = true;
        //            Periods.ModuleId = model.ModuleId;
        //            Periods.Type = "Month";
        //            _dbContext.AppPeriods.Add(Periods);
        //            await _dbContext.SaveChangesAsync();
        //        //}
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.InnerException.Message.ToString();
        //        return false;
        //    }
        //}

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
