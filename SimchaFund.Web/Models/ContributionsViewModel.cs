using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class ContributionsViewModel
    {
        public Contributor Contributor { get; set; }
        public bool Include { get; set; }
        public decimal Balance { get; set; }
    }
}
