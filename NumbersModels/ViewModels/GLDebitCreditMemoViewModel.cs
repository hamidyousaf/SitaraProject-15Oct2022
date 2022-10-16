using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GLDebitCreditMemoViewModel
    {
        public SelectList Suppliers { get; set; }
        public SelectList WareHouse { get; set; }
        public SelectList SubAccounts { get; set; }
        public SelectList Department { get; set; }
        public SelectList SubDepartment { get; set; }
        public SelectList CostCenter { get; set; }
        public SelectList OperatingUnit { get; set; }
        public SelectList TransactionType { get; set; }
        public SelectList Tax { get; set; }
        public SelectList Party { get; set; }
        public string PartyType { get; set; }
        public string TransactionDate { get; set; }
        public string DocumentDate { get; set; }
        public GLDebitCreditMemo GLDebitCreditMemo { get; set; } = new GLDebitCreditMemo();
        public GLDebitCreditMemoDetail GLDebitCreditMemoDetail { get; set; } = new GLDebitCreditMemoDetail();
        public List<GLDebitCreditMemoDetail> GLDebitCreditMemoDetails { get; set; } = new List<GLDebitCreditMemoDetail>();
    }
}
