using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class ContributorViewModel
    {
        public Contributor Contributor { get; set; }
        public decimal Balance { get; set; }
        public string Message { get; set; }
    }
}
