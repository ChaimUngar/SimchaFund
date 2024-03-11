using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class ContributorsPageViewModel
    {
        public List<ContributorViewModel> Contributors { get; set; }
        public string Message { get; set; }
        public decimal Total { get; set; }
    }
}

