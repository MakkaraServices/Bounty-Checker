using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker.Viewer
{
    public class ConnectionInfoBitcoin
    {
        public UserConnection UserConnected { get; set; }
        public string Campaign { get; set; }
        public string BtcAmount { get; set; }
        public string Transaction { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

    }
}
