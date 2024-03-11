using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class HomePageViewModel
    {
        public List<SimchaViewModel> Simchas { get; set; }
        public int TotalContributors { get; set; }
        public string Message { get; set; }
    }
}
