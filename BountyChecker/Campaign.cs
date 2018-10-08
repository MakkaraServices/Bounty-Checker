using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker
{
    public class Campaign
    {
        public Dictionary<string, string> Partecipants { get; set; }
        public string Name { get; set; }
        public string SpreadSheet { get; set; }
        public string LinkOriginalSpreadSheet { get; set; }
        public string Projectname { get; set; }

        public Campaign(string name, string spreadSheet, string projectname, string linkOriginalSpreadSheet)
        {
            Partecipants = new Dictionary<string, string>();
            Name = name;
            SpreadSheet = spreadSheet;
            Projectname = projectname;
            LinkOriginalSpreadSheet = linkOriginalSpreadSheet;
        }
    }
}
