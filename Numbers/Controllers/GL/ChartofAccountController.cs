using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
//using Numbers.Models;
//using Numbers.ViewModels;
using Numbers.Helpers;
using Newtonsoft;
namespace Numbers.Controllers
{
    [Authorize]
    public class ChartofAccountController : Controller
    {
        
        private readonly NumbersDbContext _dbContext;
        private readonly int _level1, _level2, _level3, _level4;
        private readonly string _splitter;
        Dictionary<string, object> returnResponse = new Dictionary<string, object>();
        public ChartofAccountController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
            _level1 = 1;
            _level2 = 2;
            _level3 = 2;
            _level4 = 4;
            _splitter = ".";

            //Get Chart of accounts config
            //CompanyConfigController config = new CompanyConfigController(dbContext);
            //_level1 = config.GetConfigByName("Chart of Account Configuration", "Level 1");

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {

            ChartofAccountViewModel accountViewModel = new ChartofAccountViewModel();
            var account = _dbContext.GLAccounts.Where(a => a.Id == id && a.IsDeleted == false).FirstOrDefault();
            if (account == null)
                return NotFound();

            var parentAccount = _dbContext.GLAccounts.Where(a => a.Id == account.ParentId && a.IsDeleted == false).FirstOrDefault();
            ViewBag.Title = "Edit account";
            ViewBag.Action = "Edit";
            ViewBag.Level1 = _level1;
            ViewBag.Level2 = _level2;
            ViewBag.Level3 = _level3;
            ViewBag.Level4 = _level4;
            ViewBag.Splitter = _splitter;

            accountViewModel.Id = id;
            accountViewModel.Name = account.Name;
            accountViewModel.Active = (account.IsActive ? "on": "");
            accountViewModel.RequireCostCenter = (account.RequireCostCenter ? "on" : ""); 
            accountViewModel.RequireSubAccount = (account.RequireSubAccount ? "on" : ""); 
            accountViewModel.AccountLevel = account.AccountLevel;
            accountViewModel.SubAccountId = new string[] { "1", "3" };// JsonConvert.DeserializeObject(account.SubAccountId);
            accountViewModel.SubAccount = account.SubAccountId;
            if (parentAccount != null)
            {
                accountViewModel.ParentCode = parentAccount.Code;
                accountViewModel.Code = account.Code.Substring(parentAccount.Code.Length, account.Code.Length - parentAccount.Code.Length);
            }
            else
                accountViewModel.Code = account.Code;

            //Get Sub accounts 
            if (accountViewModel.AccountLevel == 4)
            {
                IEnumerable<GLSubAccount> subAccount = _dbContext.GLSubAccounts.Where(s => s.IsDeleted == false);
                accountViewModel.SubAccountList = subAccount;
            }
            return View("PopupAccount", accountViewModel);
        }
        public IActionResult CreateSibling(int id)
        {

            ChartofAccountViewModel accountViewModel = new ChartofAccountViewModel();
            var parentAccount = _dbContext.GLAccounts.Where(a => a.Id == id && a.IsDeleted == false).FirstOrDefault(); //get parentId
            if (parentAccount == null)
                return NotFound("Parent account not found, please refresh page");

            //get last account code
            var account = _dbContext.GLAccounts
                        .Where(a => a.ParentId == id && a.CompanyId == 1 && a.IsDeleted == false)
                        .OrderByDescending(a => a.Code)
                        .FirstOrDefault();

            string newCode;
            int newCodeLength;
            short newAccountLevel = (short)(parentAccount.AccountLevel + 1);
            if (newAccountLevel == 1)
                newCodeLength = _level1;
            else if (newAccountLevel == 2)
                newCodeLength = _level2;
            else if (newAccountLevel == 3)
                newCodeLength = _level3;
            else
                newCodeLength = _level4;
            if (account == null) //means no child row, start with 1
            {
                newCode = "1";
                newCode = newCode.PadLeft(newCodeLength, '0');
            }
            else  //increment 1 into last code
            {
                newCode = account.Code.Substring(account.Code.Length - newCodeLength, newCodeLength);
                int c = Convert.ToInt16(newCode) + 1;
                newCode = c.ToString();
                newCode = newCode.PadLeft(newCodeLength, '0');
            }
            //var account = _dbContext.GLAccounts.Where(a => a.Id == account.ParentId && a.IsDeleted == false).FirstOrDefault();
            ViewBag.Title = "Create account";
            ViewBag.Action = "Create";
            ViewBag.Level1 = _level1;
            ViewBag.Level2 = _level2;
            ViewBag.Level3 = _level3;
            ViewBag.Level4 = _level4;
            ViewBag.Splitter = _splitter;

            accountViewModel.ParentId = id;
            accountViewModel.AccountLevel = newAccountLevel;
            accountViewModel.ParentCode = parentAccount.Code;
            accountViewModel.Code = newCode;
            accountViewModel.NewCodeLength = newCodeLength;
            accountViewModel.Name = "";

            //Get Sub accounts 
            if (newAccountLevel == 4)
            {
                IEnumerable<GLSubAccount> subAccount = _dbContext.GLSubAccounts.Where(s => s.IsDeleted == false);
                accountViewModel.SubAccountList = subAccount;
            }
            return View("PopupAccount", accountViewModel);
        }
        public IActionResult CreateChild(int id)
        {

            ChartofAccountViewModel accountViewModel = new ChartofAccountViewModel();

            //get parent account code
            var parentAccount = _dbContext.GLAccounts.Where(a => a.Id == id && a.CompanyId==1 && a.IsDeleted == false).FirstOrDefault();
            if (parentAccount == null)
                return NotFound();

            //get last account code
            var account = _dbContext.GLAccounts
                        .Where(a => a.ParentId == id && a.CompanyId==1 && a.IsDeleted == false)
                        .OrderByDescending(a => a.Code)
                        .FirstOrDefault();

            string newCode;
            int newCodeLength;
            short newAccountLevel = (short)(parentAccount.AccountLevel + 1);
            if (newAccountLevel == 1)
                newCodeLength = _level1;
            else if (newAccountLevel == 2)
                newCodeLength = _level2;
            else if (newAccountLevel == 3)
                newCodeLength = _level3;
            else
                newCodeLength = _level4;
            if (account == null) //means no child row, start with 1
            {
                newCode = "1";
                newCode = newCode.PadLeft(newCodeLength, '0');
            }
            else  //increment 1 into last code
            {
                newCode = account.Code.Substring(account.Code.Length - newCodeLength, newCodeLength);
                int c = Convert.ToInt16(newCode) + 1;
                newCode = c.ToString();
                newCode = newCode.PadLeft(newCodeLength, '0');
            }
            //var account = _dbContext.GLAccounts.Where(a => a.Id == account.ParentId && a.IsDeleted == false).FirstOrDefault();
            ViewBag.Title = "Create account";
            ViewBag.Action = "Create";
            ViewBag.Level1 = _level1;
            ViewBag.Level2 = _level2;
            ViewBag.Level3 = _level3;
            ViewBag.Level4 = _level4;
            ViewBag.Splitter = _splitter;

            accountViewModel.ParentId = id;
            accountViewModel.AccountLevel = newAccountLevel;
            accountViewModel.ParentCode = parentAccount.Code;
            accountViewModel.Code = newCode;
            accountViewModel.NewCodeLength = newCodeLength;
            accountViewModel.Name = "";

            //Get Sub accounts 
            if (newAccountLevel == 4)
            {
                IEnumerable<GLSubAccount> subAccount = _dbContext.GLSubAccounts.Where(s => s.IsDeleted == false);
                accountViewModel.SubAccountList = subAccount;
            }
            return View("PopupAccount", accountViewModel);
        }
        [HttpPost]
        public IActionResult Post(int id, ChartofAccountViewModel accountModel)
        {
           
            try
            {
                if (accountModel.Action == "Create")
                {
                    string newCode = string.Concat(accountModel.ParentCode, accountModel.Code);
                    var accountDuplicate = _dbContext.GLAccounts.Where(a => a.Code == newCode && a.CompanyId==1 && a.IsDeleted == false);
                    if (accountDuplicate.Count() !=0)
                    {
                        returnResponse.Add("success", false);
                        returnResponse.Add("message", string.Format("Account with code [{0}] already exist, please enter new code.", newCode));
                        return Ok(returnResponse);
                    }
                    GLAccount account = new GLAccount();
                    account.AccountLevel = accountModel.AccountLevel;
                    account.Code = newCode;
                    account.ParentId = accountModel.ParentId;
                    account.CompanyId = 1;
                    account.CreatedBy = 1;
                    account.CreatedDate = DateTime.Now;
                    if (account.AccountLevel == 4)
                    { //only 4 level account have property of active/in-active
                        account.IsActive = (accountModel.Active == "on" ? true : false);
                        account.RequireCostCenter = (accountModel.RequireCostCenter == "on" ? true : false);
                        account.RequireSubAccount = (accountModel.RequireSubAccount == "on" ? true : false);
                        account.SubAccountId = JsonConvert.SerializeObject(accountModel.SubAccountId);
                    }
                    else
                        account.IsActive = true;

                    account.Name = accountModel.Name;
                    account.IsDeleted = false;
                    _dbContext.GLAccounts.Add(account);
                    _dbContext.SaveChanges();
                    returnResponse.Add("success", true);
                    returnResponse.Add("message", string.Concat("Account has been created <br>", account.Code, " - ", account.Name));
                    return Ok(returnResponse);
                }
                else if (accountModel.Action == "Edit")
                {
                    GLAccount account = new GLAccount();
                    account = _dbContext.GLAccounts.Find(accountModel.Id);
                    account.Name = accountModel.Name;
                    if (accountModel.AccountLevel == 4)
                    {
                        account.IsActive = (accountModel.Active == "on" ? true : false);
                        account.RequireCostCenter = (accountModel.RequireCostCenter == "on" ? true : false);
                        account.RequireSubAccount = (accountModel.RequireSubAccount == "on" ? true : false);
                        account.SubAccountId = JsonConvert.SerializeObject(accountModel.SubAccountId);
                    }


                    _dbContext.GLAccounts.Update(account);
                    _dbContext.SaveChanges();
                    returnResponse.Add("success", true);
                    returnResponse.Add("message", string.Concat("Account has been updated <br>", account.Code, " - ", account.Name));
                    return Ok(returnResponse);
                }
                else
                {
                    var parentAccount = _dbContext.GLAccounts.Find(id);
                    string newCode = string.Concat(parentAccount.Code, _splitter, accountModel.Code);

                    //check for duplication
                    var accountDuplicate = _dbContext.GLAccounts.Where(a => a.Code == newCode && a.IsDeleted == false);
                    if (accountDuplicate != null)
                    {
                        throw new Exception(string.Format("Account with code [{0}] already exist, please enter new code.", newCode));
                    }
                    else
                    {

                    }

                }
                return CreatedAtAction(nameof(Post),1);
            }
            catch(Exception exc)
            {
                throw exc;
            }
        }
        public IActionResult AccountCreated()
        {
            return View();
        }
        [HttpGet()]
        public IEnumerable<AccountTree> GetChartofAccount()
        {
            var accounts = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.Company.Id == 1);
            List<AccountTree> trees = new List<AccountTree>();
            foreach (var account in accounts)
            {
                AccountTree tree = new AccountTree();
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
        public IActionResult GetAllAccounts(int? accountLevel=4) 
        {
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == 1 && a.AccountLevel == accountLevel
                                               )
                                               .Select(a => new
                                               {
                                                   Id = a.Id,
                                                   Name = string.Concat(a.Code, " - ", a.Name)
                                               })
                                               .OrderBy(a => a.Name)
                                               .ToList();

            return Ok(accounts);
        }
        [HttpGet]
        public IActionResult GetAccounts(string q = "")
        {
            if (q == "")
            {
                return Ok(new {
                    message = "Validation Required",
                    error = "Missing query {q}"

                });
            }
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == 1 && a.AccountLevel == 4
                                                && (a.Code.Contains(q) || a.Name.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(accounts);
        }
        [HttpGet]
        public IActionResult Get(int id)
        {
            var account = _dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.Company.Id == 1 && a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(account);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            //check if account is already in use
            var duplicateCheck = _dbContext.GLVoucherDetails.Where(a => a.AccountId == id && a.IsDeleted == false).Count();
            int userId = Utility.ActiveUserId;

            var account = _dbContext.GLAccounts.Where(a => a.Id == id).FirstOrDefault();
            account.IsDeleted = true;
            account.UpdatedBy = 1;
            account.UpdatedDate = DateTime.Now;
            _dbContext.SaveChanges();
            returnResponse.Add("success", true);
            returnResponse.Add("message", "Account has been deleted!");
            return Ok(returnResponse);
        }
        [HttpGet]
        public IActionResult GetCashBankAccounts(string q = "")
        {
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == 1 && a.AccountLevel == 4 
                                                && (a.Code.Contains(q) || a.Name.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(accounts);
        }
    }
    public class AccountTree
    {
        public int Id { get; set; }
        public string Parent { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }
}