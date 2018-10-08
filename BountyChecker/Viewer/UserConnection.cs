using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker.Viewer
{
    public enum AbuserAlertLevel
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4
    }
    public class UserConnection
    {
        public List<ConnectionInfo> ConnectionList { get; set; }
        public string Username { get; set; }
        public List<string> EthAddressList { get; set; }
        public string BtcTalkProfileLink { get; set; }

        public AbuserAlertLevel AbuserLevel
        {
            get
            {

                if (ConnectionList.Count >= 1 && ConnectionList.Count <= 2)
                    return AbuserAlertLevel.Low;
                else if (ConnectionList.Count >= 3 && ConnectionList.Count <= 5)
                    return AbuserAlertLevel.Medium;
                else if (ConnectionList.Count >= 6 && ConnectionList.Count <= 9)
                    return AbuserAlertLevel.High;
                else if (ConnectionList.Count >= 10)
                    return AbuserAlertLevel.VeryHigh;
                else
                    return AbuserAlertLevel.None;
            }
        }



    }
}
