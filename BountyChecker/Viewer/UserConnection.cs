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
        public List<ConnectionInfoEthereum> ConnectionListEthereum { get; set; }
        public List<ConnectionInfoBitcoin> ConnectionListBitcoin { get; set; }
        public string Username { get; set; }
        public List<string> EthAddressList { get; set; }
        public List<string> BtcAddressList { get; set; }
        public string BtcTalkProfileLink { get; set; }

        public AbuserAlertLevel AbuserLevel
        {
            get
            {
                if (ConnectionListEthereum != null) { 
                    if (ConnectionListEthereum.Count >= 1 && ConnectionListEthereum.Count <= 2)
                        return AbuserAlertLevel.Low;
                    else if (ConnectionListEthereum.Count >= 3 && ConnectionListEthereum.Count <= 5)
                        return AbuserAlertLevel.Medium;
                    else if (ConnectionListEthereum.Count >= 6 && ConnectionListEthereum.Count <= 9)
                        return AbuserAlertLevel.High;
                    else if (ConnectionListEthereum.Count >= 10)
                        return AbuserAlertLevel.VeryHigh;
                    else
                        return AbuserAlertLevel.None;
                }
                else
                {
                    if (ConnectionListBitcoin.Count >= 1 && ConnectionListBitcoin.Count <= 2)
                        return AbuserAlertLevel.Low;
                    else if (ConnectionListBitcoin.Count >= 3 && ConnectionListBitcoin.Count <= 5)
                        return AbuserAlertLevel.Medium;
                    else if (ConnectionListBitcoin.Count >= 6 && ConnectionListBitcoin.Count <= 9)
                        return AbuserAlertLevel.High;
                    else if (ConnectionListBitcoin.Count >= 10)
                        return AbuserAlertLevel.VeryHigh;
                    else
                        return AbuserAlertLevel.None;
                }
            }
        }



    }
}
