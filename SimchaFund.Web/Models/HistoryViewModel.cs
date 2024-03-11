using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class HistoryViewModel
    {
        public List<History> History { get; set; }
        public string Name { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
