using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Numbers.Views.Voucher
{
    public class PostController : Controller
    {
        [HttpPost]
        public IActionResult Index(Voucher data)
        {

            //return View(data);
            return Ok(data);
        }
        
    }
    public class Voucher
    {
        public int Id { get; set; }
        public DateTime VoucherDate { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public VoucherDetail Detail { get; set; }
    }
    public class VoucherDetail
    {
        public int Id { get; set; }
        public int AccountName { get; set; }
        public int SubAccount { get; set; }
        public int CostCenter { get; set; }
        public int Debit { get; set; }
        public int Credit { get; set; }
    }
}