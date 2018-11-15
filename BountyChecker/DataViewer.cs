using BountyChecker.Viewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BountyChecker
{
    //Read from the xml the data and show it in a sensible manner
    public class DataViewer
    {
        public enum Kind
        {
            Eth,
            Btc
        }
        private XDocument m_results;
        System.IO.StreamWriter fileResult;
        private Dictionary<string, UserConnection> muserDictionary;
        private BitcoinTalkScraper btcScraper;
        private BountyFileLoader bountyFileLoader;
        public DataViewer(string xmlFileProjectResult,string outputfile, BountyFileLoader bt, Kind kind)
        {
            btcScraper = new BitcoinTalkScraper();
            //fileResult = new System.IO.StreamWriter(outputfile);
            bountyFileLoader = bt;
            m_results = XDocument.Load(xmlFileProjectResult);
            if (kind == Kind.Eth)
            {
                muserDictionary = GetUserDictionaryEthereum();
                ComputeConnectionsEthereum();
            }
                
            else
            {
                muserDictionary = GetUserDictionaryBitcoin();
                ComputeConnectionsBitcoin();
            }

            
        }


        private void ComputeConnectionsEthereum()
        {
            IEnumerable<XElement> users = m_results.Root.Elements("User");

            foreach (XElement user in users)
            {
                UserConnection userConnection = muserDictionary[user.Attribute("UserId").Value];               

                foreach (XElement address in user.Elements("EthereumAddress"))
                {
                    userConnection.EthAddressList.Add(address.Attribute("Address").Value);

                    foreach (XElement connection in address.Elements("Connection"))
                    {
                        ConnectionInfoEthereum userConnected = MakeConnectionInstanceEthereum(connection);
                        userConnection.ConnectionListEthereum.Add(userConnected);
                    }
                }
            }

        }

        private void ComputeConnectionsBitcoin()
        {
            IEnumerable<XElement> users = m_results.Root.Elements("User");

            foreach (XElement user in users)
            {
                UserConnection userConnection = muserDictionary[user.Attribute("UserId").Value];

                foreach (XElement address in user.Elements("BitcoinAddress"))
                {
                    userConnection.BtcAddressList.Add(address.Attribute("Address").Value);

                    foreach (XElement connection in address.Elements("Connection"))
                    {
                        ConnectionInfoBitcoin userConnected = MakeConnectionInstanceBitcoin(connection);
                        userConnection.ConnectionListBitcoin.Add(userConnected);
                    }
                }
            }

        }

        public void ShowConnections()
        {
            var myList = muserDictionary.ToList();

            //myList.OrderBy(c => c.Value.).ThenBy(c => c.Value.ConnectionListEthereum.Count());

            myList.Sort((pair1, pair2) => pair2.Value.ConnectionListEthereum.Count().CompareTo(pair1.Value.ConnectionListEthereum.Count()));
            //myList.Sort((pair1, pair2) => pair2.Value.AbuserLevel.CompareTo(pair1.Value.AbuserLevel));

            foreach (KeyValuePair<string, UserConnection> user in myList)
            {
                fileResult.WriteLine(user.Key + " Connections: " + user.Value.ConnectionListEthereum.Count() + " Addresses: " + user.Value.EthAddressList.Count());
            }

            fileResult.Close();

            /*foreach (KeyValuePair<string, UserConnection> user in muserDictionary)
                Console.WriteLine(user.Key + " Connections: " + user.Value.ConnectionListEthereum.Count() + " Addresses: " + user.Value.EthAddressList.Count());
                */
        }

        private void CreateHtmlOtputDetailsEthereum(string folderPath)
        {
            
            bool shownUser = false;
            Dictionary<string, string> listAbuser;
            System.IO.StreamWriter fileOutput;

            if (Directory.Exists(folderPath + "\\Details") == false)
                Directory.CreateDirectory(folderPath + "\\Details");

            foreach (KeyValuePair<string, UserConnection> user in muserDictionary)
            {
                fileOutput = new System.IO.StreamWriter(folderPath + "\\Details\\" + user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", "") + ".html");
                listAbuser = new Dictionary<string, string>();
                foreach (ConnectionInfoEthereum connection in user.Value.ConnectionListEthereum)
                {
                    
                        if (shownUser == false)
                        {
                            shownUser = true;
                            string username = btcScraper.GetUserNameById(user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                            fileOutput.WriteLine("<font size=5><b>User:</b><a href=\"" + user.Key + "\"> <b>" + username + "</b></a><br/>");
                            fileOutput.WriteLine("<b>Connected with:</b></font><br/>");
                            
                        }

                        if (listAbuser.ContainsKey(connection.UserConnected.BtcTalkProfileLink) == false)
                            listAbuser.Add(connection.UserConnected.BtcTalkProfileLink, "");

                        string usernameConnected = btcScraper.GetUserNameById(connection.UserConnected.BtcTalkProfileLink.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                            fileOutput.WriteLine("<hr>"
                                 + "<b>Category:</b> " + connection.Campaign + "<br/> "
                                + "<a href=\"" + connection.UserConnected.BtcTalkProfileLink + "\"><b>" + usernameConnected + "</b></a><br/>" 
                                + "<b>Token -> " + connection.TokenName + "(" + connection.TokenSymbol + ")</b><br/>"
                                + "<b>Transaction</b><a href=\"https://etherscan.io/tx/" + connection.Transaction + "\"> " + connection.Transaction + " </a><br/>"
                                + "<b>Addresses</b> " + connection.Description + "<br/>");
                    
                }

                shownUser = false;

                fileOutput.WriteLine("<hr><font size=5><b>Recap users involved:</b></font><br/>");

                foreach (KeyValuePair<string, string> curretnUser in listAbuser)
                {
                    string username = btcScraper.GetUserNameById(curretnUser.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                    fileOutput.WriteLine("<a href=\"" + curretnUser.Key + "\"><b>" + username + "</b></a><br/>");
                }
                fileOutput.Close();
            }


            

        }

        private void CreateHtmlOtputDetailsBitcoin(string folderPath)
        {

            bool shownUser = false;
            Dictionary<string, string> listAbuser;
            System.IO.StreamWriter fileOutput;

            if (Directory.Exists(folderPath + "\\Details") == false)
                Directory.CreateDirectory(folderPath + "\\Details");

            foreach (KeyValuePair<string, UserConnection> user in muserDictionary)
            {
                fileOutput = new System.IO.StreamWriter(folderPath + "\\Details\\" + user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", "") + ".html");
                listAbuser = new Dictionary<string, string>();
                foreach (ConnectionInfoBitcoin connection in user.Value.ConnectionListBitcoin)
                {

                    if (shownUser == false)
                    {
                        shownUser = true;
                        string username = btcScraper.GetUserNameById(user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                        fileOutput.WriteLine("<font size=5><b>User:</b><a href=\"" + user.Key + "\"> <b>" + username + "</b></a><br/>");
                        fileOutput.WriteLine("<b>Connected with:</b></font><br/>");

                    }

                    if (listAbuser.ContainsKey(connection.UserConnected.BtcTalkProfileLink) == false)
                        listAbuser.Add(connection.UserConnected.BtcTalkProfileLink, "");

                    string usernameConnected = btcScraper.GetUserNameById(connection.UserConnected.BtcTalkProfileLink.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));

                    string connectionType = "";

                    if (connection.Type.Contains("InputInput"))
                        connectionType = "<b>Type:</b><font color=\"red\">" + connection.Type + "</font><br/>";
                    else
                        connectionType = "<b>Type:</b><font color=\"orange\">" + connection.Type + "</font><br/>";

                    fileOutput.WriteLine("<hr>"
                         + "<b>Category:</b> " + connection.Campaign + "<br/> "
                        + "<a href=\"" + connection.UserConnected.BtcTalkProfileLink + "\"><b>" + usernameConnected + "</b></a><br/>"
                        + "<b>Btc Amount -> " + connection.BtcAmount + "</b><br/>"
                        + "<b>Transaction</b><a href=\"https://www.blockchain.com/btc/tx/" + connection.Transaction + "\"> " + connection.Transaction + " </a><br/>"
                        + "<b>Addresses</b> " + connection.Description + "<br/>"                       
                        + connectionType);

                }

                shownUser = false;

                fileOutput.WriteLine("<hr><font size=5><b>Recap users involved:</b></font><br/>");

                foreach (KeyValuePair<string, string> curretnUser in listAbuser)
                {
                    string username = btcScraper.GetUserNameById(curretnUser.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                    fileOutput.WriteLine("<a href=\"" + curretnUser.Key + "\"><b>" + username + "</b></a><br/>");
                }
                fileOutput.Close();
            }




        }

        public void CreateHtmlOtputIndexEthereum(string folderPath)
        {
            Dictionary<string, int> mainUserConnectedDictionary = new Dictionary<string, int>();
            Dictionary<string, int> userConnectedDictionary;
            System.IO.StreamWriter fileOutput = new System.IO.StreamWriter(folderPath + "index.html");

            fileOutput.WriteLine("<b><font size=5>" + bountyFileLoader.SpreadSheetList[0].Projectname + "</font></b><br/>");
            foreach (Campaign campaign in bountyFileLoader.SpreadSheetList)
            {
                
                fileOutput.WriteLine("<b>Spreadheet link: <a href=\"" + campaign.LinkOriginalSpreadSheet + "\">" + campaign.Name + "</a></b><br/>");
            }

            foreach (KeyValuePair<string, UserConnection> user in muserDictionary)
            {
                mainUserConnectedDictionary.Add(user.Key, 0);
                userConnectedDictionary = new Dictionary<string, int>();

                foreach (ConnectionInfoEthereum userConn in user.Value.ConnectionListEthereum)
                {
                    if (userConnectedDictionary.ContainsKey(userConn.UserConnected.BtcTalkProfileLink) == false)
                        userConnectedDictionary.Add(userConn.UserConnected.BtcTalkProfileLink, 1);
                    else
                        userConnectedDictionary[userConn.UserConnected.BtcTalkProfileLink]++;
                }

                mainUserConnectedDictionary[user.Key] = userConnectedDictionary.Count;
            }

            var myList = mainUserConnectedDictionary.ToList();
            
            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            
            foreach (KeyValuePair<string, int> user in myList)
            {
                string addresess = string.Join(",", muserDictionary[user.Key].EthAddressList.ToArray());
                fileOutput.WriteLine("<hr><a href=\"" + "Details\\" + user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", "") + ".html\">" + btcScraper.GetUserNameById(user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", "")) + "</a> <b>Users connected:</b> " + user.Value +
                    " <b>Transactions:</b> " + muserDictionary[user.Key].ConnectionListEthereum.Count() + " <b>Address:</b> " + addresess);
            }
            fileOutput.Close();
        }

        public void CreateHtmlOtputIndexBitcoin(string folderPath)
        {
            Dictionary<string, int> mainUserConnectedDictionary = new Dictionary<string, int>();
            Dictionary<string, int> userConnectedDictionary;
            System.IO.StreamWriter fileOutput = new System.IO.StreamWriter(folderPath + "index.html");

            fileOutput.WriteLine("<b><font size=5>" + bountyFileLoader.SpreadSheetList[0].Projectname + "</font></b><br/>");
            foreach (Campaign campaign in bountyFileLoader.SpreadSheetList)
            {

                fileOutput.WriteLine("<b>Spreadheet link: <a href=\"" + campaign.LinkOriginalSpreadSheet + "\">" + campaign.Name + "</a></b><br/>");
            }

            foreach (KeyValuePair<string, UserConnection> user in muserDictionary)
            {
                mainUserConnectedDictionary.Add(user.Key, 0);
                userConnectedDictionary = new Dictionary<string, int>();

                foreach (ConnectionInfoBitcoin userConn in user.Value.ConnectionListBitcoin)
                {
                    if (userConnectedDictionary.ContainsKey(userConn.UserConnected.BtcTalkProfileLink) == false)
                        userConnectedDictionary.Add(userConn.UserConnected.BtcTalkProfileLink, 1);
                    else
                        userConnectedDictionary[userConn.UserConnected.BtcTalkProfileLink]++;
                }

                mainUserConnectedDictionary[user.Key] = userConnectedDictionary.Count;
            }

            var myList = mainUserConnectedDictionary.ToList();

            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            foreach (KeyValuePair<string, int> user in myList)
            {
                string addresess = string.Join(",", muserDictionary[user.Key].BtcAddressList.ToArray());
                fileOutput.WriteLine("<hr><a href=\"" + "Details\\" + user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", "") + ".html\">" + btcScraper.GetUserNameById(user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", "")) + "</a> <b>Users connected:</b> " + user.Value +
                    " <b>Transactions:</b> " + muserDictionary[user.Key].ConnectionListBitcoin.Count() + " <b>Address:</b> " + addresess);
            }
            fileOutput.Close();
        }


        public void ShowConnectionsPerCategory(string category)
        {
            if (category == null)
                return;

            bool shownUser = false;
            bool userInCategory = false;
            List<string> listAbuser = new List<string>();
            Campaign currentCampaign=null;

            foreach (Campaign campaign in bountyFileLoader.SpreadSheetList)
            {
                if (campaign.Name.ToLower().Equals(category.ToLower()) == true)
                    currentCampaign = campaign;
            }


            fileResult.WriteLine("[b][size=18pt]" + currentCampaign.Projectname + " - " + category + "[/size][/b]");
            fileResult.WriteLine("[b]Spreadheet link: " + currentCampaign.LinkOriginalSpreadSheet + "[/b]");
            fileResult.WriteLine("");

            foreach (KeyValuePair<string, UserConnection> user in muserDictionary)
            {
                foreach (ConnectionInfoEthereum connection in user.Value.ConnectionListEthereum)
                {
                    if (connection.Campaign.ToLower().Equals(category.ToLower()))
                    {
                        userInCategory = true;
                        if (shownUser == false)
                        {
                            listAbuser.Add(user.Key);
                            shownUser = true;
                            string username = btcScraper.GetUserNameById(user.Key.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                            fileResult.WriteLine("[b]User:[/b] " + user.Key + " [b]" + username + "[/b]");
                            fileResult.WriteLine("[b]Connected with:[/b] ");
                            fileResult.WriteLine("[list]");
                        }
                        string usernameConnected = btcScraper.GetUserNameById(connection.UserConnected.BtcTalkProfileLink.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                        fileResult.WriteLine("[li]" + connection.UserConnected.BtcTalkProfileLink + " ([b]" + usernameConnected + "[/b]) [b]Token -> " + connection.TokenName + "(" + connection.TokenSymbol + ")[/b]"
                            + " [b]Transaction[/b] https://etherscan.io/tx/" + connection.Transaction 
                            + " [b]Addresses[/b] " + connection.Description + "[/li]");


                    }
                }
                if (shownUser == true)
                    fileResult.WriteLine("[/list]");


                if (userInCategory == true)
                    fileResult.WriteLine("");

                userInCategory = false;
                shownUser = false;
            }


            fileResult.WriteLine("[b]Recap users involved[/b]");
            fileResult.WriteLine("[list]");
            foreach (string user in listAbuser)
            {
                string username = btcScraper.GetUserNameById(user.Replace("https://bitcointalk.org/index.php?action=profile;u=", ""));
                fileResult.WriteLine("[li]" + user + "([b]" + username  + "[/b])[/li]");
            }
            fileResult.WriteLine("[/list]");
            fileResult.Close();
        }

        private Dictionary<string, UserConnection> GetUserDictionaryEthereum()
        {
            IEnumerable<XElement> users = m_results.Root.Elements("User");
            Dictionary<string, UserConnection> userDictionary = new Dictionary<string, UserConnection>();
            foreach (XElement user in users)
            {
                UserConnection newUser = MakeUserConnectionInstanceEthereum(user);
                userDictionary.Add(newUser.BtcTalkProfileLink,newUser);
            }

            return userDictionary;
        }

        private Dictionary<string, UserConnection> GetUserDictionaryBitcoin()
        {
            IEnumerable<XElement> users = m_results.Root.Elements("User");
            Dictionary<string, UserConnection> userDictionary = new Dictionary<string, UserConnection>();
            foreach (XElement user in users)
            {
                UserConnection newUser = MakeUserConnectionInstanceBitcoin(user);
                userDictionary.Add(newUser.BtcTalkProfileLink, newUser);
            }

            return userDictionary;
        }

        public void CreateHtmlOutputEthereum(string folderPath)
        {
            this.CreateHtmlOtputIndexEthereum(folderPath);
            this.CreateHtmlOtputDetailsEthereum(folderPath);
        }

        public void CreateHtmlOutputBitcoin(string folderPath)
        {
            this.CreateHtmlOtputIndexBitcoin(folderPath);
            this.CreateHtmlOtputDetailsBitcoin(folderPath);
        }

        private UserConnection MakeUserConnectionInstanceEthereum(XElement data)
        {
            UserConnection newUser = new UserConnection();
            newUser.ConnectionListEthereum = new List<ConnectionInfoEthereum>();
            newUser.EthAddressList = new List<string>();

            newUser.BtcTalkProfileLink = data.Attribute("UserId").Value;
            newUser.Username = ""; //Get username function;

            return newUser;
        }

        private UserConnection MakeUserConnectionInstanceBitcoin(XElement data)
        {
            UserConnection newUser = new UserConnection();
            newUser.ConnectionListBitcoin = new List<ConnectionInfoBitcoin>();
            newUser.BtcAddressList = new List<string>();

            newUser.BtcTalkProfileLink = data.Attribute("UserId").Value;
            newUser.Username = ""; //Get username function;

            return newUser;
        }

        private ConnectionInfoEthereum MakeConnectionInstanceEthereum(XElement data)
        {
            ConnectionInfoEthereum connection = new ConnectionInfoEthereum();

            connection.UserConnected = muserDictionary[data.Attribute("UserId").Value];
            connection.Campaign = data.Attribute("Campaign").Value;
            connection.TokenName = data.Attribute("TokenName").Value;
            connection.TokenSymbol = data.Attribute("TokenSymbol").Value;
            connection.Transaction = data.Attribute("Transaction").Value;
            connection.Description = data.Attribute("Description").Value;


            return connection;
        }

        private ConnectionInfoBitcoin MakeConnectionInstanceBitcoin(XElement data)
        {
            ConnectionInfoBitcoin connection = new ConnectionInfoBitcoin();

            connection.UserConnected = muserDictionary[data.Attribute("UserId").Value];
            connection.Campaign = data.Attribute("Campaign").Value;
            connection.BtcAmount = data.Attribute("BTC").Value;
            connection.Transaction = data.Attribute("Transaction").Value;
            connection.Description = data.Attribute("Description").Value;
            connection.Type = data.Attribute("ConnectionType").Value;

            return connection;
        }
    }
}
