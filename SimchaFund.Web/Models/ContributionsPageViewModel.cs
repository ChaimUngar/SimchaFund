using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class ContributionsPageViewModel
    {
        public List<ContributorViewModel> Contributors { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
    }
}
