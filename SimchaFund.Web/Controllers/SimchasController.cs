using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace SimchaFund.Web.Controllers
{
    public class SimchasController : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress; Initial Catalog=SimchaFund; Integrated Security=true;";

        public IActionResult Index()
        {
            var db = new SimchaFundDb(_connectionString);
            var vm = new HomePageViewModel();

            vm.TotalContributors = db.GetTotalContributorCount();

            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }

            List<Simcha> simchas = db.GetAllSimchas();

            List<SimchaViewModel> simchasWithInfo = simchas.Select(s => new SimchaViewModel
            {
                Simcha = s,
                ContributorCount = db.GetContributorAmntBySimcha(s.Id),
                TotalMoney = db.GetTotalFundsForSimcha(s.Id)
            }).ToList();

            vm.Simchas = simchasWithInfo;

            return View(vm);
        }

        [HttpPost]
        public IActionResult New(Simcha simcha)
        {
            //string date = simcha.Date.ToString();
            var db = new SimchaFundDb(_connectionString);

            if(!String.IsNullOrEmpty(simcha.SimchaName) && simcha.Date != default)
            {
                db.InsertSimcha(simcha);
                TempData["message"] = "New simcha created! Mazel Tov!!";
            }

            return Redirect("/simchas/index");
        }

        //public IActionResult Contributions(int simchaId)
        //{
        //    var db = new SimchaFundDb(_connectionString);
        //    List<Contributor> contributors = db.GetContributorsBySimcha();
        //    var vm = new UpdatePageViewModel();

        //    return View(new UpdatePageViewModel
        //    {
        //        ContribInfo = contributors.Select(c => new UpdateViewModel
        //        {
        //            Contributor = c,
        //            Balance = db.GetDepositsForContributor(c.Id) + (db.GetContributionsForContributor(c.Id) * -1)
                    

        //        }).ToList(),

        //        SimchaName = db.GetSimchaName(simchaId),
        //        SimchaId = simchaId,
        //    }); 
        //}

        //public IActionResult Update(int simchaId, List<UpdateViewModel> contributors)
        //{
        //    var db = new SimchaFundDb(_connectionString);
        //    db.Update(simchaId, contributors);
        //    return Redirect("/simchas");
        //}

        public IActionResult Contributions(int simchaId)
        {
            var db = new SimchaFundDb(_connectionString);
            List<Contributor> contributors = db.GetAllContributors();
            List<int> ids = db.GetIdsBySimcha(simchaId);



            List<ContributionsViewModel> info = contributors.Select(c => new ContributionsViewModel
            {
                Contributor = c,
                Balance = db.GetDepositsForContributor(c.Id) + (db.GetContributionsForContributor(c.Id) * -1),
                Include = ids.Contains(c.Id)

            }).ToList();

            return View(new UpdatePageViewModel
            {
                ContribInfo = info,
                SimchaId = simchaId,
                SimchaName = db.GetSimchaName(simchaId)
            });
        }

        [HttpPost]
        public IActionResult Update(int simchaId, List<Update> contributors)
        {
            var db = new SimchaFundDb(_connectionString);
            db.Update(simchaId, contributors);
            TempData["message"] = "Contributions updated successfully";
            return Redirect("/simchas");
        }
    }
}