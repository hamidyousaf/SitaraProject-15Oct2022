using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.Helpers
{
    public class AppSession
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public static string GetUserId
        {
            get
            {
                return HttpContextAccessor.HttpContext.Session.GetString("UserId");
            }
        }
        public static int GetCompanyId
        {
            get
            {                

                return HttpContextAccessor.HttpContext.Session.GetInt32("CompanyId").Value;
            }
        }
        public static IHttpContextAccessor GetHttpContextAccessor
        {
            get
            {
                return HttpContextAccessor;
            }
        }

        public static IHttpContextAccessor HttpContextAccessor { get => _httpContextAccessor; set => _httpContextAccessor = value; }
    }
}
public interface ISessionValueProvider
{
    string NewCompanyId { get; set; }
    string NewUserId { get; set; }
    IHttpContextAccessor NewHttpContextAccessor { get; set; }
}
