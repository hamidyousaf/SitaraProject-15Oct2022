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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Numbers.Areas.GL.Controllers
{
    [Area("GL")]
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
           
            ViewBag.NavbarHeading = "Chart of Account";
            return View();
        }
        public IActionResult Edit(int id)
        {
            ViewBag.subAccounts = new SelectList(from ac in _dbContext.GLSubAccounts.Where(x => x.IsDeleted == false && x.IsActive==true && x.IsApproved==true).ToList()
                                                 select new
                                                 {
                                                     Id = ac.Id,
                                                     Name = ac.Code + " - " + ac.Description
                                                 }, "Id", "Name");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ChartofAccountViewModel accountViewModel = new ChartofAccountViewModel();
            var account = _dbContext.GLAccounts.Where(a => a.Id == id && a.IsDeleted == false && a.CompanyId==companyId).FirstOrDefault();
            if (account == null)
                return NotFound();

            var parentAccount = _dbContext.GLAccounts.Where(a => a.Id == account.ParentId && a.IsDeleted == false && a.CompanyId == companyId).FirstOrDefault();
            ViewBag.Title = "Edit account";
            ViewBag.Action = "Edit";
            ViewBag.Level1 = _level1;
            ViewBag.Level2 = _level2;
            ViewBag.Level3 = _level3;
            ViewBag.Level4 = _level4;
            ViewBag.Splitter = _splitter;

            accountViewModel.Id = id;
            accountViewModel.Name = account.Name;
            accountViewModel.Active = (account.IsActive ? "on" : "");
            accountViewModel.RequireCostCenter = (account.RequireCostCenter ? "on" : "");
            accountViewModel.RequireSubAccount = (account.RequireSubAccount ? "on" : "");
            accountViewModel.AccountLevel = account.AccountLevel;
            accountViewModel.SubAccountId =  new string[] { "1", "3" };// JsonConvert.DeserializeObject(account.SubAccountId);
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
                IEnumerable<GLSubAccount> subAccount = _dbContext.GLSubAccounts.Where(s => s.IsDeleted == false && s.IsActive == true && s.IsApproved == true);
                accountViewModel.SubAccountList = subAccount;
            }
            return View("PopupAccount", accountViewModel);
        }
        public IActionResult CreateSibling(int id)
        {
            ViewBag.subAccounts = new SelectList(from ac in _dbContext.GLSubAccounts.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList()
                                                 select new
                                                 {
                                                     Id = ac.Id,
                                                     Name = ac.Code + " - " + ac.Description
                                                 }, "Id", "Name");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ChartofAccountViewModel accountViewModel = new ChartofAccountViewModel();
            var parentAccount = _dbContext.GLAccounts.Where(a => a.Id == id && a.IsDeleted == false && a.CompanyId == companyId).FirstOrDefault(); //get parentId
            if (parentAccount == null)
                return NotFound("Parent account not found, please refresh page");

            //get last account code
            var account = _dbContext.GLAccounts
                        .Where(a => a.ParentId == id && a.CompanyId == companyId && a.IsDeleted == false)
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
            accountViewModel.RequireSubAccount = "on";

            //Get Sub accounts 
            if (newAccountLevel == 4)
            {
                IEnumerable<GLSubAccount> subAccount = _dbContext.GLSubAccounts.Where(s => s.IsDeleted == false && s.IsActive == true && s.IsApproved == true);
                accountViewModel.SubAccountList = subAccount;
            }
            return View("PopupAccount", accountViewModel);
        }
        public IActionResult CreateChild(int id)
        {
            ViewBag.subAccounts = new SelectList(from ac in _dbContext.GLSubAccounts.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList()
                                                 select new
                                                 {
                                                     Id = ac.Id,
                                                     Name = ac.Code + " - " + ac.Description
                                                 }, "Id", "Name");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ChartofAccountViewModel accountViewModel = new ChartofAccountViewModel();

            //get parent account code
            var parentAccount = _dbContext.GLAccounts.Where(a => a.Id == id && a.CompanyId == companyId && a.IsDeleted == false).FirstOrDefault();
            if (parentAccount == null)
                return NotFound();

            //get last account code
            var account = _dbContext.GLAccounts
                        .Where(a => a.ParentId == id && a.CompanyId == companyId && a.IsDeleted == false)
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
            accountViewModel.RequireSubAccount = "on";

            //Get Sub accounts 
            if (newAccountLevel == 4)
            {
                IEnumerable<GLSubAccount> subAccount = _dbContext.GLSubAccounts.Where(s => s.IsDeleted == false && s.IsActive == true && s.IsApproved == true);
                accountViewModel.SubAccountList = subAccount;
            }
            return View("PopupAccount", accountViewModel);
        }
        [HttpPost]
        public async Task <IActionResult> Post(int id, ChartofAccountViewModel accountModel,IFormCollection collection)
        {

            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                if (accountModel.Action == "Create")
                {
                    string newCode = string.Concat(accountModel.ParentCode, accountModel.Code);
                    var accountDuplicate = _dbContext.GLAccounts.Where(a => a.Code == newCode && a.CompanyId == companyId && a.IsDeleted == false);
                    if (accountDuplicate.Count() != 0)
                    {
                        returnResponse.Add("success", false);
                        returnResponse.Add("message", string.Format("Account with code [{0}] already exist, please enter new code.", newCode));
                        return Ok(returnResponse);
                    }
                    GLAccount account = new GLAccount();
                    account.AccountLevel = accountModel.AccountLevel;
                    account.Code = newCode;
                    account.ParentId = accountModel.ParentId;
                    account.CompanyId = companyId;
                    account.CreatedBy = userId;
                    account.CreatedDate = DateTime.Now;
                    if (account.AccountLevel == 4)
                    { //only 4 level account have property of active/in-active
                        account.IsActive = (accountModel.Active == "on" ? true : false);
                        account.RequireCostCenter = (accountModel.RequireCostCenter == "on" ? true : false);
                        account.RequireSubAccount = (accountModel.RequireSubAccount == "on" ? true : false);
                       // account.SubAccountId = JsonConvert.SerializeObject(accountModel.SubAccountId);
                        account.SubAccountId =Convert.ToString( collection["SubAccountId"]);
                    }
                    else
                        account.IsActive = true;

                    account.Name = accountModel.Name;
                    account.IsDeleted = false;
                    _dbContext.GLAccounts.Add(account);
                   await _dbContext.SaveChangesAsync();
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
                        account.SubAccountId = Convert.ToString(collection["SubAccountId"]);//JsonConvert.SerializeObject(accountModel.SubAccountId);
                    }


                   var entry = _dbContext.GLAccounts.Update(account);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
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
                return CreatedAtAction(nameof(Post), 1);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        public IActionResult AccountCreated()
        {
            return View();
        }

        [HttpGet]
        public IEnumerable<AccountTree> GetChartofAccount()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var accounts = _dbContext.GLAccounts.Where(a => a.IsDeleted == false /*&& a.Company.Id == companyId*/);
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
        public IActionResult GetAllAccounts(int? accountLevel = 4)
        {
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == HttpContext.Session.GetInt32("CompanyId").Value && a.AccountLevel == accountLevel
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
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //if (q == "")
            //{
            //    return Ok(new
            //    {
            //        message = "Validation Required",
            //        error = "Missing query {q}"

            //    });
            //}
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.AccountLevel == 4
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
        public IActionResult GetThirdAccounts(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //if (q == "")
            //{
            //    return Ok(new
            //    {
            //        message = "Validation Required",
            //        error = "Missing query {q}"

            //    });
            //}
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == companyId && a.AccountLevel == 3
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
           
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var account = _dbContext.GLAccounts.Where(a => a.IsDeleted == false /*&& a.Company.Id == companyId*/ && a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(account);
        }

        [HttpGet]
        public IActionResult GetSubAccounts(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //if (q == "")
            //{
            //    return Ok(new
            //    {
            //        message = "Validation Required",
            //        error = "Missing query {q}"

            //    });
            //}
            var accounts = _dbContext.GLSubAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == companyId && a.IsActive == true && a.IsApproved == true
                                                && (a.Code.Equals(q) || a.Description.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Description),
                                                   code = a.Code,
                                                   name = a.Description
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(accounts);
        }

        [HttpGet]
        public IActionResult GetSubAccount(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var account = _dbContext.GLSubAccounts.Where(a => a.IsDeleted == false && a.Company.Id == companyId && a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Description)
                                               })
                                               .FirstOrDefault();
            return Ok(account);
        }

        [HttpGet]
        public async Task <IActionResult> Delete(int id)
        {            
            try
            {
                //check if account is already in use
                var accountUsedInVoucher = _dbContext.GLVoucherDetails.Where(v => v.AccountId == id && v.IsDeleted == false).FirstOrDefault();
                var accountUsedInBankCash = _dbContext.GLBankCashAccounts.Where(b => b.AccountId == id ).FirstOrDefault();
                var accountUsedInItemAccounts = _dbContext.InvItemAccounts.Where(i => i.GLAssetAccountId == id || i.GLCostofSaleAccountId == id || i.GLSaleAccountId == id).FirstOrDefault();
                var accountUsedByCustomers = _dbContext.ARCustomers.Where(c => c.AccountId == id && c.IsDeleted!=true).FirstOrDefault();
                var accountUsedBySuppliers = _dbContext.APSuppliers.Where(s => s.AccountId == id ).FirstOrDefault();
                if (accountUsedInVoucher == null && accountUsedInBankCash == null && accountUsedInItemAccounts == null && accountUsedByCustomers == null && accountUsedBySuppliers == null)
                {
                    string userId = HttpContext.Session.GetString("UserId");

                    var account = _dbContext.GLAccounts.Where(a => a.Id == id).FirstOrDefault();
                    account.IsDeleted = true;
                    account.UpdatedBy = userId;
                    account.UpdatedDate = DateTime.Now;
                    var entry = _dbContext.GLAccounts.Update(account);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    returnResponse.Add("success", true);
                    returnResponse.Add("message", "Account has been deleted!");
                    return Ok(returnResponse);
                }
                else
                {
                    //return NotFound(string.Format("Cannot delete account because it is used in Voucher#: {0} and Bank Cash: {1}",accountUsedInVoucher.VoucherId,accountUsedInBankCash.AccountName));
                    return NotFound("This Account is in Use and cannot be Deleted.");
                }
            }
            catch(Exception exc)
            {
                return NotFound(exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString());
            }
        }

        [HttpGet]
        public IActionResult GetCashBankAccounts(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == companyId && a.AccountLevel == 4
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
