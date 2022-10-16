using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
   public class InvBOMViewModel
    {
        public InvBOM InvBOM { get; set; } = new InvBOM();
        public List<InvBOMDetail> InvBOMDetail { get; set; }
        public string Date { get; set; }
    }
}
