﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BountyChecker
{
    class Connection
    {
        public bool Result { get; set; }
        public string Address { get; set; }

        public Connection(bool result, string address)
        {
            Result = result;
            Address = address;
        }
    }
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
            return jsnString;
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

    }
}