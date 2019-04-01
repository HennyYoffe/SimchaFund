using ClassLibrary1;
using HW54_SimchaFund_Mar26.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HW54_SimchaFund_Mar26.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
            IEnumerable<Simcha> simchas = mgr.GetSimchos();
            List<IndexViewModel> vm = new List<IndexViewModel>();
            foreach(Simcha simcha in simchas)
            {
                vm.Add(new IndexViewModel
                {
                    Simcha = simcha,
                    ContributerCount = mgr.GetCountContributers(),
                    AmountOfContributers = mgr.GetAmountOfPeopleGiving(simcha.Id),
                });
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult SubmitSimcha(Simcha simcha)
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
            mgr.AddSimcha(simcha);
            return Redirect("/home/index");
        }
        public ActionResult GetDonationsForSimcha(int id)
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
            IEnumerable<Contributer> contributers = mgr.GetContributers();
            int num = 0;
            List<GetDonationsForSimchaViewModel> vm = new List<GetDonationsForSimchaViewModel>();
            foreach (Contributer c in contributers)
            {
               GetDonationsForSimchaViewModel gdfsvm =  new GetDonationsForSimchaViewModel
                {
                    Contributer = c,
                    Amount = mgr.GetAmountDonated(id, c.Id),
                   SimchaId = id,
                   NumberInList = num,
                };

                if(gdfsvm.Amount > 0)
                {
                    gdfsvm.Donate = true;
                }
                else
                {
                    gdfsvm.Amount = 5;
                    gdfsvm.Donate = false;
                }
                vm.Add(gdfsvm);
                num++;
            }
            GetDonationsForSimchaViewModelList vml = new GetDonationsForSimchaViewModelList();
            vml.GetDonations = vm;
            vml.SimchaName = mgr.GetSimchaNameForId(id);
            
            return View(vml);
        }       
        public ActionResult UpdateContributions (List<Donations> getDonations)
        {
            bool first = true;
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
                foreach(Donations d in getDonations)
            {
                if(d.Donated == true)
                {
                    
                    mgr.AddDonation(d,first);
                    if (first)
                    {
                        first = false;
                    }
                }
                
            }
            return Redirect("/home/index");
        }
        public ActionResult Contributers()
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
            IEnumerable<Contributer> contributers = mgr.GetContributers();
            return View(contributers);
        }
        [HttpPost]
        public ActionResult SubmitContributer(Contributer contributer,decimal initialdeposit)
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
          
           int id =  mgr.AddContributer(contributer);
            mgr.AddDeposit(initialdeposit, id, contributer.Date);
            return Redirect("/home/contributers");
        }
        [HttpPost]
        public ActionResult SubmitDeposit(int contributerid, decimal amount, DateTime date)
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
            mgr.AddDeposit(amount, contributerid, date);
            return Redirect("/home/contributers");
        }
        public ActionResult ShowContributerHistory(int id)
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
           IEnumerable<Contributer> contributers = mgr.GetContributers();
            foreach(Contributer c in contributers)
            {
                if(c.Id == id)
                {
                    mgr.CombineAndSortDepositsAndDonations(c);
                    return View(c);
                }
                
            }
            return Redirect("/home/contributers");

        }
        public ActionResult EditContributer(Contributer contributer)
        {
            SimchaFundManager mgr = new SimchaFundManager(Properties.Settings.Default.SFConStr);
            mgr.EditContributer(contributer);
            return Redirect("/home/contributers");
        }
        }
}
