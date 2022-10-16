using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APSupplierVM
    {
        public APSupplier APSupplier { get; set; }

        public List<ARContactPerson> ARContactPerson { get; set; }
        public List<ARShippingDetail> ARShippingDetail { get; set; }
        public List<ARSuplierItemsGroup> ARSuplierItemsGroup { get; set; }
        public string GLAccount { get; set; }
    }
}
