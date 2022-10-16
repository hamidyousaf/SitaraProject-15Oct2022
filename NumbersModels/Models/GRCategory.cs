using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class GRCategory
	{
        private IQueryable<GRCategory> GRCategories;

        public int Id { get; set; }
        public int ItemCategoryId { get; set; }
		public int TransactionNo { get; set; }
		public DateTime TransactionDate { get; set; }
		public int MinThreads { get; set; }
		public int MaxThreads { get; set; }
		public int MinWidth { get; set; }
		public int MaxWidth { get; set; }
		public string  Description { get; set; }
		public int CompanyId { get; set; }
		public bool IsDeleted { get; set; }

		[MaxLength(450)]
		public string CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		public string ApprovedBy { get; set; }

		public DateTime ApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public string Status { get; set; }

		[NotMapped]
		public GRCategory category { get; set; }

		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }



	}
}