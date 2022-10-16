using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Controllers;
using Numbers.Repository.Helpers;
using Numbers.Repository;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Controllers
{
    [Area("AusPak")]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly NotificationService _notificationRepository;
        private readonly BaseNotificationRepository<APCSRequestDetail> _BaseNotificationRepository;
        public NotificationController(NumbersDbContext context, NotificationService notificationRepository, BaseNotificationRepository<APCSRequestDetail> baseNotificationRepository )
        {
            _dbContext = context;
            _notificationRepository = notificationRepository;
            _BaseNotificationRepository = baseNotificationRepository;

        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Follow()
        {
            var userId = HttpContext.Session.GetString("UserId");
            int follow = _notificationRepository.GetTotalCS("");
            return Json(follow);
        }

        [HttpGet]
        public IActionResult GetBellNotification()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
           // var Menus = _dbContext.AppRoleMenus.Where(x => x.RoleId == (from a in _dbContext.UserRoles.Where(y => y.UserId == userId) select a.RoleId).FirstOrDefault());
            var role =_dbContext.Roles.Where(x=>x.Id == (from a in _dbContext.UserRoles.Where(y => y.UserId == userId)  select a.RoleId).FirstOrDefault()).FirstOrDefault();
            BellNotification notification = new BellNotification();
            //TempData["TotalFollowUp"] = new NotificationRepo(_dbContext, userId).GetTotalFollowUp(userId);
            //TempData["TotalCounselor"] = new NotificationRepo(_dbContext, userId).GetTotalStudent(userName);
            //TempData["TotalNotification"] = Convert.ToInt32(TempData["TotalFollowUp"]) + Convert.ToInt32(TempData["TotalCounselor"]);
           //notification.TotalFollowUP = new NotificationRepo(_dbContext, userId).GetTotalFollowUp(userId);
           //notification.TotlaCounselor = new NotificationRepo(_dbContext, userId).GetTotalStudent(userName);
           // notification.TotalApplication = new NotificationRepo(_dbContext, userId).GetTotalApplication(userId);
           // notification.TotalUnApproveApplication = new NotificationRepo(_dbContext, userId).GetTotalUnApproveApplication(userId);
 
            if (role.Name == "Visa Manager" || role.Name == "Admin")
            {
              //  notification.TotalAcceptance = new NotificationRepo(_dbContext, userId).GetTotalAcceptance(userId);
               // notification.TotalVisa = new NotificationRepo(_dbContext, userId).GetTotalVisa(userId);
            }
            else
            {
                notification.TotalAcceptance = "0";
                notification.TotalVisa = "0";
            }
                notification.TotalNotification =Convert.ToString(Convert.ToInt32(notification.TotalFollowUP) + Convert.ToInt32(notification.TotlaCounselor) + Convert.ToInt32(notification.TotalAcceptance) + Convert.ToInt32(notification.TotalVisa) + Convert.ToInt32(notification.TotalApplication) + Convert.ToInt32(notification.TotalUnApproveApplication));
            return Json (notification); 
        }

        [HttpGet]
        public IActionResult SaleOrderExpiration()
        {
            var saleOrder = _dbContext.ARSaleOrderItems.Where(x => x.Id == 0).ToList();
            try { 
            
            var baleInforModel = _dbContext.BaleInformation.Where(p => p.TempBales == 0).ToList();
              saleOrder = _dbContext.ARSaleOrderItems.Include(p => p.SaleOrder).Where(p => p.Qty > p.DCQty && p.IsDeleted != true && p.SaleOrder.SaleOrderDate.Date.AddDays(p.SaleOrder.Validity) < DateTime.Now.Date && p.IsExpired != true).ToList();
            foreach (var item in saleOrder)
            {
                var baleInfoModelList = new List<BaleInformation>();
                var baleInfoModel = new BaleInformation();
                var saleOrderItem = new ARSaleOrderItem();

                saleOrderItem = item;

                if (baleInforModel.Exists(p => p.ItemId == item.ItemId && p.Meters == item.Meters))
                {
                    foreach (var baleinfo in baleInforModel)
                    {
                        baleInfoModel = baleinfo;
                        baleInfoModel.TempBales = 1;
                        baleInfoModelList.Add(baleInfoModel);
                    }
                    saleOrderItem.IsExpired = true;
                }
                _dbContext.BaleInformation.UpdateRange(baleInfoModel);
                _dbContext.ARSaleOrderItems.Update(saleOrderItem);
                _dbContext.SaveChangesAsync();
            }

            return Json(saleOrder);
            }
            catch (Exception e)
            {
                return Json(saleOrder);
            }

        }

    }
    
}