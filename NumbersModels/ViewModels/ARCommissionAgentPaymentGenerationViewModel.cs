using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ARCommissionAgentPaymentGenerationViewModel
    {
        public int Id { get; set; }
        [DisplayName("Trans.#")]
        public int TransactionNo { get; set; }
        [DisplayName("Commission Agent")]
        public int? CommissionAgentId { get; set; }
        [DisplayName("Product Type")]
        public int? ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        [DisplayName("Start Date")]
        public string StartDate { get; set; }
        [DisplayName("End Date")]
        public string EndDate { get; set; }
        [DisplayName("Grand Total")]
        public decimal? GrandTotal { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool? IsApproved { get; set; }
        public int? Resp_Id { get; set; }
        public string CommissionAgentName { get; set; }
        public string Status { get; set; }
        public int? CompanyId { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        public SelectList ProductType { get; set; }
        public SelectList CommissionAgents { get; set; }
        public virtual ARCommissionAgent ARCommissionAgent { get; set; }
        public virtual IQueryable<ARCommissionAgentPaymentGenerationDetails> ARCommissionAgentPaymentGenerationDetails { get; set; }
    }
}
