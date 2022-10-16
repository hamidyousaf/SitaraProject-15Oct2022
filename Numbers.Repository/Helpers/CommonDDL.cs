using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Repository.Helpers
{
   public static class CommonDDL
    {
        public static List<SelectListItem> GetCity(List<AppCitiy> appCitiy)
        {
            List<SelectListItem> citySelectList = new List<SelectListItem>();
            foreach (var city in appCitiy)
            {
                citySelectList.Add(new SelectListItem() { Text = city.Name, Value = city.Id.ToString() });
            }

            return citySelectList;
        }

        public static List<SelectListItem> GetCountry(List<AppCountry> appCountry)
        {
            List<SelectListItem> countrySelectList = new List<SelectListItem>();
            foreach (var country in appCountry)
            {
                countrySelectList.Add(new SelectListItem() { Text = country.Name, Value = country.Id.ToString() });
            }

            return countrySelectList;
        }




    }
}
