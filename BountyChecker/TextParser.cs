using BountyChecker.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BountyChecker
{
    static class TextParser
    {
        // TO read data from the google spreadsheet replace '/edit#gid=<some_number>' with '/preview?usp=embed_googleplus' a the end of the link.﻿        
        public static async Task<Dictionary<string,string>> GetUserCampaignEthAddress(Campaign campaign, IProgress<ProgressStatus> progressinfo)
        {
            Dictionary<string, string> addressList = new Dictionary<string, string>();
            System.IO.StreamReader fileRead =
                      new System.IO.StreamReader(campaign.SpreadSheet);
            string line;
            int totalLines = 0;
            int currentLine = 0;
            bool foundUser = false;
            string userForumProfile = "";

            // Find the total lines for the report
            var file = await fileRead.ReadToEndAsync(); // big string
            var lines = file.Split(new char[] { '\n' });           // big array
            totalLines = lines.Count();
            currentLine = 1;
            fileRead.DiscardBufferedData();
            fileRead.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

            line = await fileRead.ReadLineAsync(); 
            while (line != null)
            {

                
                if (foundUser == false)
                {
                    userForumProfile = getUserIdBitcointalk(line);
                    if (userForumProfile != "")
                        foundUser = true;
                }

                string lineEthAddress = getEthAddress(line);

                if (lineEthAddress != "" && addressList.ContainsKey(lineEthAddress.ToLower()) == false && foundUser == true)
                {
                    addressList.Add(lineEthAddress.ToLower(), userForumProfile);
                    foundUser = false;
                }
                
                line = await fileRead.ReadLineAsync();

                           
                ReportProgressStatus(progressinfo, totalLines, currentLine,
                   campaign.Name + " - Read line " + currentLine + "/" + totalLines,
                    Report.LineParser);
                currentLine++;
            }

            fileRead.Close();


            return addressList;
        }

        public static async Task<Dictionary<string, string>> GetUserCampaignBtcAddress(Campaign campaign, IProgress<ProgressStatus> progressinfo)
        {
            Dictionary<string, string> addressList = new Dictionary<string, string>();
            System.IO.StreamReader fileRead =
                      new System.IO.StreamReader(campaign.SpreadSheet);
            string line;
            int totalLines = 0;
            int currentLine = 0;
            bool foundUser = false;
            string userForumProfile = "";

            // Find the total lines for the report
            var file = await fileRead.ReadToEndAsync(); // big string
            var lines = file.Split(new char[] { '\n' });           // big array
            totalLines = lines.Count();
            currentLine = 1;
            fileRead.DiscardBufferedData();
            fileRead.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

            line = await fileRead.ReadLineAsync();
            while (line != null)
            {


                if (foundUser == false)
                {
                    userForumProfile = getUserIdBitcointalk(line);
                    if (userForumProfile != "")
                        foundUser = true;
                }

                string lineBtcAddress = getBtcAddress(line);

                if (lineBtcAddress != "" && addressList.ContainsKey(lineBtcAddress) == false && foundUser == true)
                {
                    addressList.Add(lineBtcAddress, userForumProfile);
                    foundUser = false;
                }

                line = await fileRead.ReadLineAsync();


                ReportProgressStatus(progressinfo, totalLines, currentLine,
                   campaign.Name + " - Read line " + currentLine + "/" + totalLines,
                    Report.LineParser);
                currentLine++;
            }

            fileRead.Close();


            return addressList;
        }


        public static string getEthAddress(string line)
        {
            try
            {
              
                Match m = Regex.Match(line, @"0x[a-fA-F0-9]{40}");

                if (m.Success)
                    return m.Value;
                else
                    return "";
                //int posEthAdd = line.IndexOf("0x");
                //if (line.Contains("0x") == true && line.Length-posEthAdd >= 42)
                //    return line.Substring(posEthAdd, 42).ToLower();


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return "";


        }

        public static string getBtcAddress(string line)
        {
            try
            {

                Match m = Regex.Match(line, @"(bc1|[13])[a-zA-HJ-NP-Z0-9]{25,39}");



                if (m.Success)
                {
                    return m.Value;
                }                    
                else
                    return "";
                //int posEthAdd = line.IndexOf("0x");
                //if (line.Contains("0x") == true && line.Length-posEthAdd >= 42)
                //    return line.Substring(posEthAdd, 42).ToLower();


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return "";


        }

        public static string getUserIdBitcointalk(string line)
        {
            try
            {

             //Match m = Regex.Match(line, @"u\=(\d+)");
                Match m = Regex.Match(line, @"https\:\/\/bitcointalk\.org\/index\.php\?action\=profile\;u\=(\d+)");

                if (m.Success)
                    return m.Value;
                else
                    return "";

                //int posEthAdd = line.IndexOf("0x");
                //if (line.Contains("0x") == true && line.Length-posEthAdd >= 42)
                //    return line.Substring(posEthAdd, 42).ToLower();


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return "";


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
