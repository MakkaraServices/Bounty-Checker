using BountyChecker.Reporting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker
{
    //This compare the address and verify if transaction are found 
    static class AddressComparer
    {


        public static async Task<bool> CheckAddressList(string projectName,Campaign campaign, IProgress<ProgressStatus> progressinfo, string etherscanKey)
        {
            int counter = 0;
            Etherscan ethreScan = new Etherscan(etherscanKey);
            Connection connected = null;
            int currentAddressProgress = 0;
            int totalAddress= campaign.Partecipants.Count;
            int currentTransactionProgress = 0;
            int totalTransaction = 0;
            string runResults = "results.txt";
            System.IO.StreamWriter fileResult =
                        new System.IO.StreamWriter(runResults);

            xmlBounty bountydata = new xmlBounty(projectName + "_results.xml");

            foreach (KeyValuePair<string, string> address in campaign.Partecipants)
            {
                await fileResult.WriteLineAsync(address.Value + " connected with:");
                
                string stringJson = ethreScan.GetTransactions(address.Key);

                JObject json = JObject.Parse(stringJson);
                JArray transactions = (JArray)json["result"];
                currentTransactionProgress = 0;
                totalTransaction = transactions.Count;

                foreach (JObject element in json["result"])
                {

                    if (element["from"].Value<string>() == address.Key)
                        connected = ethreScan.CheckIfAnyTrasactionWithOtherAddress(campaign.Partecipants, element["to"].Value<string>());
                    else if (element["to"].Value<string>() == address.Key)
                        connected = ethreScan.CheckIfAnyTrasactionWithOtherAddress(campaign.Partecipants, element["from"].Value<string>());

                    if (connected.Result == true)
                    {
                        bountydata.AddUser(address.Value, address.Key);

                        fileResult.WriteLine(campaign.Partecipants[connected.Address] + " -> " + element["tokenName"].Value<string>() + "(" + element["tokenSymbol"].Value<string>() + ")" + "https://etherscan.io/tx/" + element["hash"].Value<string>() + " " + element["from"].Value<string>() + " " + element["to"].Value<string>());
                        //Console.WriteLine(addressList[connected.Address] + " -> " + element["tokenName"].Value<string>() + "(" + element["tokenSymbol"].Value<string>() + ")" + "https://etherscan.io/tx/" + element["hash"].Value<string>() + " " + element["from"].Value<string>() + " " + element["to"].Value<string>());
                        bountydata.AddUserEthConnection(address.Value, address.Key, campaign.Partecipants[connected.Address], connected.Address, campaign.Name,element);
                    }

                    currentTransactionProgress++;
                    ReportProgressStatus(progressinfo, totalTransaction, currentTransactionProgress,
                   campaign.Name + " - Checked transactions " + currentTransactionProgress + "/" + totalTransaction,
                    Report.Transaction);
                }
                currentAddressProgress++;
                ReportProgressStatus(progressinfo, totalAddress, currentAddressProgress,
                    campaign.Name + " - Verified " + address.Key + " - " + currentAddressProgress + "/" + totalAddress, 
                    Report.Address);


                counter++;
            }

            fileResult.Close();


            return true;
        }

        private static void ReportProgressStatus(IProgress<ProgressStatus> progressinfo, int total, int done, string description, Report type)
        {
            ProgressStatus progressStatus = new ProgressStatus
            {
                Total = total,
                Done = done,
                Description = description,
                ReportType = type
            };
            progressinfo.Report(progressStatus);
        }
    }
}
