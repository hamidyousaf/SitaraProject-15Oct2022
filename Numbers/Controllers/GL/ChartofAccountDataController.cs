using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Numbers.Models;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;

namespace Numbers.Controllers
{
    [Authorize]
    [Route("api/{controller}/{action}")]
    public class ChartofAccountDataController : Controller
    {
        private readonly NumbersDbContext dbContext;
        public ChartofAccountDataController(NumbersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet()]
        public IEnumerable<JsonTree> GetChartofAccount()
        {
            var accounts = dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.Company.Id == 1);
            List<JsonTree> trees = new List<JsonTree>();
            foreach (var account in accounts)
            {
                JsonTree tree = new JsonTree();
                tree.Id = account.Id;
                tree.Parent = (account.ParentId == 0 ? "#" : account.ParentId.ToString());
                tree.Text = string.Concat(account.Code, " - ", account.Name);
                if (!account.IsActive)
                    tree.Icon = "far fa-file-minus";
                else if (account.AccountLevel == 4)
                    tree.Icon = "far fa-file";
                else
                    tree.Icon = "far fa-folder";
                trees.Add(tree);
            }
            return trees;
        }
        [HttpGet("{accountId}")]
        public Dictionary<string, GLAccount> GetAccount(int accountId)
        {
            Dictionary<string, GLAccount> list = new Dictionary<string, GLAccount>();
            var account = dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.Id == accountId && a.Company.Id == 1).FirstOrDefault();
            if (account == null)
                return null;

            var parentAccount = dbContext.GLAccounts.Where(p => p.IsDeleted == false && p.Id == account.ParentId).FirstOrDefault();

            list.Add("account",account);
            list.Add("parentAccount",parentAccount);

            return list;

        }
        public bool CreateAccount()
        {
            return false;
        }
    }

    public class JsonTree
    {
        public int Id { get; set; }
        public string Parent { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }
}