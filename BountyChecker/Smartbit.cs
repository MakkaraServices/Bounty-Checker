
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BountyChecker
{
    public class Smartbit
    {
        //

        string bitcoinTransaction = "https://api.smartbit.com.au/v1/blockchain/address/";
        string bitcoinTransactionDetails = "https://api.smartbit.com.au/v1/blockchain/tx/";
        string TransactionCount = "?limit=";

        string jsnString = "";
        WebClient webClient;

        public Smartbit()
        {
            webClient = new WebClient();
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; MSIE 9.0; Windows NT 9.0; en-US)";
            //webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
        }

        public string GetHtmlFromUrl(string url)
        {
            retry:
            try
            {
                var response = webClient.DownloadString(url);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Retrying to get: " + url);
                goto retry;
            }
        }

        public string GetHtmlFromUrlLimit(string url, int limit, out int changedLimit)
        {
            string OriginalUrl = url;
            changedLimit = limit;
            retry:
            try
            {
                int limitPos = OriginalUrl.IndexOf("&limit=");
                url= OriginalUrl.Remove(limitPos, OriginalUrl.Length - limitPos);

                string response = webClient.DownloadString(url + "&limit=" + changedLimit);

                return response;
            }
            catch (Exception ex)
            {
                changedLimit = changedLimit / 2; 
                Console.WriteLine(ex.Message);
                Console.WriteLine("Retrying to get: " + url + "&limit=" + changedLimit);
                goto retry;
            }
        }


        public string GetTransactions(string btcAddress, int limit)
        {
            retry:
            try
            {
                Thread.Sleep(200);
                var response = webClient.DownloadString(bitcoinTransaction + btcAddress + TransactionCount + limit);

                return response;
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
                webClient = new WebClient();
                Console.WriteLine(ex.Message);
                Console.WriteLine("Retrying to get: " + bitcoinTransaction + btcAddress + " limit: " + limit);
                goto retry;
            }
        }

        public int GetTransactionsCount(string btcAddress)
        {
            retry:
            try
            {
                Thread.Sleep(500);

                var response = webClient.DownloadString(bitcoinTransaction + btcAddress);

                JObject jsonTransactions = JObject.Parse(response.ToString());
                
                return Int32.Parse(jsonTransactions["address"]["total"]["transaction_count"].Value<string>());
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
                webClient = new WebClient();
                Console.WriteLine(ex.Message);
                Console.WriteLine("Retrying to get: " + bitcoinTransaction + btcAddress);
                goto retry;
            }
        }

        public string GetTransactionDetails(string transaction)
        {
            retry:
            try
            {
                Thread.Sleep(200);

                var response = webClient.DownloadString(bitcoinTransactionDetails + transaction);

                return response;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Retrying to get: " + bitcoinTransactionDetails + transaction);
                goto retry;
            }
        }

        public Connection CheckIfAnyTrasactionWithOtherAddress(Dictionary<string, string> addressList, string address_to_Check)
        {
            foreach (KeyValuePair<string, string> adress in addressList)
            {
                if (adress.Key.ToLower().Equals(address_to_Check.ToLower()) == true)
                {
                    return new Connection(true, adress.Key);
                }
            }

            return new Connection(false, null);
        }
    }
}

