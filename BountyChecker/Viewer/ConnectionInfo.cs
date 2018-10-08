using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker.Viewer
{
    public class ConnectionInfo
    {
        public UserConnection UserConnected { get; set; }
        public string Campaign { get; set; }
        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }
        public string Transaction { get; set; }
        public string Description { get; set; }


    }
}
