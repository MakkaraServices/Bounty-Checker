using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BountyChecker
{
    class Etherscan
    {
        string erc20Transaction = "http://api.etherscan.io/api?module=account&action=tokentx&startblock=0&endblock=999999999&sort=asc&apikey=";
        string jsnString = "";
        WebClient webClient;
     
        public Etherscan(string etherscanKey)
        {
            webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
            erc20Transaction += etherscanKey + "&address=";
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


        public string GetTransactions(string ethAddress)
        {
            retry:
            try
            {
                Thread.Sleep(200);

                var response = webClient.DownloadString(erc20Transaction + ethAddress);

                return response;                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Retrying to get: " + erc20Transaction + ethAddress);
                goto retry;
            }
        }

        public Connection CheckIfAnyTrasactionWithOtherAddress(Dictionary<string,string>  addressList, string address_to_Check)
        {
            foreach (KeyValuePair<string,string> adress in addressList)
            {
                if (adress.Key.ToLower().Equals(address_to_Check.ToLower()) == true)
                {
                    return new Connection(true, adress.Key);
                }
            }

            return new Connection(false, null);
        }

        public static void SaveKeyOnRegistry(string apikey)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\BountyChecker");

                key.SetValue("ApiKey", apikey);
                key.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR saving to registry: " +  e.Message);
            }

        }

        public static string ReadKeyFromRegistry()
        {
            string keyvalue = "PASTE_ETHERSCAN_API_HERE";

            try
            {
               
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BountyChecker");

                if (key != null)
                {
                    keyvalue = key.GetValue("ApiKey").ToString();
                    key.Close();
                }

                return keyvalue;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR reading from tegistry: " + e.Message);
                return keyvalue;
            }

        }
    }
}
