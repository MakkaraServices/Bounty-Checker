using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BountyChecker
{
    class BitcoinTalkScraper
    {

        Dictionary<string, string> userFound = new Dictionary<string, string>();
        XDocument xmlUsers;
        string userXml = @"users.xml";

        public BitcoinTalkScraper()
        {
            if (System.IO.File.Exists(userXml) == false)
            {
                System.IO.File.Create(userXml).Dispose();
                using (TextWriter tw = new StreamWriter(userXml))
                {
                    tw.WriteLine("<Root/>");
                }
            }

            xmlUsers = XDocument.Load(userXml);

        }
        public string GetUserNameById(string id)
        {
           
            
            string Name = "";
            


            if (UserExits(id, out Name) == true)
                return Name;



            string html = GetHtmlFromUrl("https://bitcointalk.org/index.php?action=profile;u=" + id, 500);

            if (userFound.ContainsKey(id) == false)
            {
                GetUserNameBtcTalk(id, out Name);
                userFound.Add(id, Name);
                AddUser(id, Name);

            }
            else
                Name=userFound[id];



            return Name;
        }

        public string GetHtmlFromUrl(string url, int millisecondsWait)
        {
            
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

            retry:
            try
            {
                Thread.Sleep(millisecondsWait);
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


        private void GetUserNameBtcTalk(string id, out string name)
        {
            int millisecondsWait = 500;
            redoCurrentUser:

            //get the Section where was given
            string htmlStringGiver = GetHtmlFromUrl("https://bitcointalk.org/index.php?action=profile;u=" + id, millisecondsWait);

            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            document.LoadHtml(htmlStringGiver);
            HtmlNodeCollection userProfile = document.DocumentNode.SelectNodes("//table");


            name = null;
          
            try
            {



                HtmlNode part = userProfile[7];


                foreach (HtmlNode sections in part.ChildNodes)
                {
                    if (sections.ChildNodes.Count != 0)
                    {
                        HtmlNode cell, cellDesc;


                        
                            if (sections.SelectNodes("th|td").Count > 1)
                            {
                                cellDesc = sections.SelectNodes("th|td")[0];
                                cell = sections.SelectNodes("th|td")[1];
                            }
                            else
                                continue;

                            switch (cellDesc.InnerText.Trim())
                            {
                                case "Name:":
                                    name = cell.InnerText;
                                    break;
                            }
                        


                        return;
                    }
                }
            }
            catch (Exception ex)
            {

                if (htmlStringGiver.Contains("Too fast / overloaded (503)"))
                {
                    Console.WriteLine("NEED TO INCREMENT THE WAITING TIME, TOO FAST REQUEST SENT TO THE SERVER: Too fast / overloaded (503)");
                    millisecondsWait += 50;
                    Console.WriteLine("Time incremented and retrying current user");
                    goto redoCurrentUser;
                }
                else
                    Console.WriteLine("UID NOT FOUND: " + id + " (" + ex.Message + ")");

            }

        }

        private bool UserExits(string Id,  out string username)
        {
            List<XElement> elementList = xmlUsers.Document.Descendants("id" + Id).ToList();
            
            if (elementList.Count > 0)
            {
                if (elementList[0].Attribute("Name") == null)
                {
                    username = "";
                    return true;
                }
                username = elementList[0].Attribute("Name").Value;
                return true;
            }
            else
            {
                username = "";
                return false;
            }

        }

        private void AddUser(string Id, string Name)
        {
            string username;
            bool userExist = UserExits(Id,out username);

            if (userExist)
            {
                return;
            }
            else
            {
                XElement newElement = new XElement("id" + Id);
                newElement.SetAttributeValue("Name", Name);
                xmlUsers.Root.Add(newElement);
                Console.WriteLine("Added User: " + Name);
                xmlUsers.Save(userXml);
            }

        }
    }
}
