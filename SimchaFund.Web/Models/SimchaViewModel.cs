using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class SimchaViewModel
    {
        public Simcha Simcha { get; set; }
        public int ContributorCount { get; set; }
        public decimal TotalMoney { get; set; }
    }
}
