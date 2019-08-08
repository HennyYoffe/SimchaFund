using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW54_SimchaFund_Mar26.Models
{
    public class IndexViewModel
    {
        public Simcha Simcha { get; set; }
        public int AmountOfContributers { get; set; }
        public int ContributerCount { get; set; }
    }
    public class GetDonationsForSimchaViewModel
    {
        public Contributer Contributer { get; set; }
        public decimal Amount { get; set; }
        public bool Donate { get; set; }
      public int SimchaId { get; set; }
        public int NumberInList { get; set; }
    }
    public class GetDonationsForSimchaViewModelList
    {
        public List<GetDonationsForSimchaViewModel> GetDonations { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
    }
}