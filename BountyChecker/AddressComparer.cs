using BountyChecker.Reporting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Newtonsoft.Json;

namespace BountyChecker
{
    //This compare the address and verify if transaction are found 
    static class AddressComparer
    {


        public static async Task<bool> CheckAddressListEthereum(string projectName,Campaign campaign, IProgress<ProgressStatus> progressinfo, string etherscanKey)
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
                        bountydata.AddUserEth(address.Value, address.Key);

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

        public static async Task<bool> CheckAddressListBitcoin(string projectName, Campaign campaign, IProgress<ProgressStatus> progressinfo)
        {
            int counter = 0;
            Smartbit bitcoinScan = new Smartbit();
            Connection connected = null;
            int currentAddressProgress = 0;
            int totalAddress = campaign.Partecipants.Count;
            int currentTransactionProgress = 0;
            int totalTransaction = 0;
            string runResults = "results.txt";
            System.IO.StreamWriter fileResult =
                        new System.IO.StreamWriter(runResults);

            xmlBounty bountydata = new xmlBounty(projectName + "_results.xml");

            foreach (KeyValuePair<string, string> address in campaign.Partecipants)
            {
                await fileResult.WriteLineAsync(address.Value + " connected with:");

                totalTransaction = bitcoinScan.GetTransactionsCount(address.Key);

                if (totalTransaction == 0)
                    continue;

                int limit = 100;
                string stringJson = bitcoinScan.GetTransactions(address.Key, 100);
                JObject jsonTransactions = JObject.Parse(stringJson);

                bool firstRun = true;
                currentTransactionProgress = 0;
                

                while (jsonTransactions != null || firstRun == true)
                {
                    firstRun = false;

                    try
                    {
                        foreach (JToken element in jsonTransactions["address"]["transactions"])
                        {

                            string stringTransactionJson = bitcoinScan.GetTransactionDetails(element["txid"].ToString());
                            string transactionId = element["txid"].ToString();
                            //Console.WriteLine("Checking Transaction:" + transactionId);
                            JObject json = JObject.Parse(stringTransactionJson);
                            Dictionary<string, double> addressInput = new Dictionary<string, double>();

                            if (json["transaction"]["inputs"].Count() > 0)
                            {
                                if (json["transaction"]["inputs"][0].Count() > 0)
                                        foreach (JToken jsonAddress in json["transaction"]["inputs"])
                                        {
                                            if (addressInput.ContainsKey(jsonAddress["addresses"][0].Value<string>()) == false)
                                                addressInput.Add(jsonAddress["addresses"][0].Value<string>(), Double.Parse(jsonAddress["value"].Value<string>()));
                                        }
                            }

                            Dictionary<string, double> addressOutputList = new Dictionary<string, double>();
                            if (json["transaction"]["outputs"].Count() > 0)
                            {
                                if (json["transaction"]["outputs"][0].Count() > 0)
                                    if (json["transaction"]["outputs"][0]["addresses"].Count() > 0)
                                    {
                                        foreach (JToken jsonAddress in json["transaction"]["outputs"][0]["addresses"])
                                        {
                                            addressOutputList.Add(jsonAddress.Value<string>(), Double.Parse(json["transaction"]["outputs"][0]["value"].Value<string>()));
                                        }
                                    }

                            }
                            foreach (KeyValuePair<string, double> addressIn in addressInput)
                            {
                                if (addressInput.Count > 1 && addressIn.Key != address.Key) { 
                                    connected = bitcoinScan.CheckIfAnyTrasactionWithinInput(campaign.Partecipants, addressIn.Key);

                                    SaveConnection(campaign, connected, bountydata, address, fileResult, addressIn, transactionId, addressInput);
                                }

                                if (addressOutputList.ContainsKey(address.Key) && addressIn.Key != address.Key)
                                {

                                    connected = bitcoinScan.CheckIfAnyTrasactionWithOtherAddress(campaign.Partecipants, addressIn.Key);

                                    SaveConnection(campaign, connected, bountydata, address, fileResult, addressIn, transactionId, addressInput);
                                }

                            }

                            foreach (KeyValuePair<string, double> addressOut in addressOutputList)
                            {

                                if (addressInput.ContainsKey(address.Key) && addressOut.Key != address.Key)
                                {
                                    connected = bitcoinScan.CheckIfAnyTrasactionWithOtherAddress(campaign.Partecipants, addressOut.Key);

                                    SaveConnection(campaign, connected, bountydata, address, fileResult, addressOut, transactionId, addressInput);
                                }

                            }

                            currentTransactionProgress++;
                            ReportProgressStatus(progressinfo, totalTransaction, currentTransactionProgress,
                                campaign.Name + " - Checked transactions " + currentTransactionProgress + "/" + totalTransaction,
                                Report.Transaction);
                           
                        }
                    }                             
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                    string NextLink = jsonTransactions["address"]["transaction_paging"]["next_link"]
                        .Value<string>();

                    if (NextLink != null)
                    {
                        stringJson =
                            bitcoinScan.GetHtmlFromUrlLimit(NextLink, limit, out limit);
                        jsonTransactions = JObject.Parse(stringJson);
                    }
                    else
                    {
                        jsonTransactions = null;
                    }

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

        private static void SaveConnection(Campaign campaign, Connection connected, xmlBounty bountydata, KeyValuePair<string, string> address,
            StreamWriter fileResult, KeyValuePair<string, double> addressIn, string transactionId, Dictionary<string, double> addressInput)
        {
            if (connected != null && connected.Result == true)
            {
                bountydata.AddUserBtc(address.Value, address.Key);

                fileResult.WriteLine(campaign.Partecipants[connected.Address] + " -> Amount BTC: " + addressIn.Value +
                                     " - tx: " + "https://www.blockchain.com/btc/tx/" + transactionId + " " + addressInput +
                                     " " + address.Key + connected.ConnectionDescription);

                bountydata.AddUserBtcConnection(address.Value, address.Key, campaign.Partecipants[connected.Address],
                    connected.Address, campaign.Name, addressIn.Value.ToString(), transactionId, addressIn.Key, address.Key,connected.ConnectionDescription);
                connected = null;
            }
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
