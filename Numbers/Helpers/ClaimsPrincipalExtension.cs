using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//using Numbers.Models;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;

namespace Numbers.Helpers
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetFullName(this ClaimsPrincipal principal)
        {
            var FullName = principal.Claims.FirstOrDefault(c => c.Type == "FullName");
            return FullName?.Value;
        }
        public static string GetUserName(this ClaimsPrincipal principal)
        {
            var userName = principal.Identity.Name;
            return userName;
        }

        public static string GetUserPhoto(this ClaimsPrincipal principal)
        {
            var userPhoto = principal.Claims.FirstOrDefault(c => c.Type == "Photo");
            return userPhoto?.Value;
        }
        public static string GetRole(this ClaimsPrincipal principal)
        {

            var userRole = principal.Claims.FirstOrDefault(c => c.Type == "UserRole");
            return userRole?.Value;
            
        }

    }
}
