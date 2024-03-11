using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Balance
    {
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateOfAction { get; set; }
    }
}
