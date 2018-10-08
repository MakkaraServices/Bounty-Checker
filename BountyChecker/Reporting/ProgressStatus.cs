using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker.Reporting
{
    public enum Report
    {
        LineParser,
        Address,
        Transaction

    }
    public class ProgressStatus
    {
        public int Total { get; set; }
        public int Done { get; set; }
        public string Description { get; set; }
        public Report ReportType { get; set; }
    }
}
