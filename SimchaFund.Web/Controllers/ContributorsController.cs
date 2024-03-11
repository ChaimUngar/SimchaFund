using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;

namespace SimchaFund.Web.Controllers
{
    public class ContributorsController : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress; Initial Catalog=SimchaFund; Integrated Security=true;";

        public IActionResult Index()
        {
            var db = new SimchaFundDb(_connectionString);
            var vm = new ContributorsPageViewModel();

            List<Contributor> contributors = db.GetAllContributors();
            
            List<ContributorViewModel> contributorWithBalance = contributors.Select(c => new ContributorViewModel
            {
                Contributor = c,
                Balance = db.GetDepositsForContributor(c.Id) + (db.GetContributionsForContributor(c.Id) * -1)

            }).ToList();

            vm.Contributors = contributorWithBalance;
            decimal total =  db.GetBalance();
            vm.Total = total;

            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }

            return View(vm);
        }

        [HttpPost]
        public IActionResult New(Contributor c, decimal initialDeposit)
        {
            var db = new SimchaFundDb(_connectionString);
            db.AddContributor(c);

            db.CreateBalance(c, initialDeposit);
            TempData["message"] = "Contributor added successfully!";
            return Redirect("/contributors/index");
        }

        //public IActionResult Search(string text)
        //{
        //    var db = new SimchaFundDb(_connectionString);

        //}

        [HttpPost]
        public IActionResult Deposit(Deposit d)
        {
            var db = new SimchaFundDb(_connectionString);
            db.AddDeposit(d);
            TempData["message"] = "Deposit added successfully!";
            return Redirect("/contributors/index");
        }

        public IActionResult History(int contribId)
        {
            var db = new SimchaFundDb(_connectionString);

            List<History> simchas = db.GetSimchaHistory(contribId);
            List<History> deposits = db.GetDepositHistory(contribId);

            simchas.AddRange(deposits);

            var vm = new HistoryViewModel
            {
                History = simchas.OrderByDescending(h => h.Date).ToList(),
                CurrentBalance = db.GetDepositsForContributor(contribId) + (db.GetContributionsForContributor(contribId) * -1),
                Name = db.GetContributorName(contribId)
            };

            return View(vm);
        }

       

        
    }
}
