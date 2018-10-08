using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BountyChecker
{
    class xmlBounty
    {
        private XDocument m_bountyXml;
        private string m_fileName;
        public xmlBounty(string filename)
        {
            if (File.Exists(filename) == true)
                m_bountyXml = XDocument.Load(filename);
            else
            {
                m_bountyXml = new XDocument(new XComment("Bounty Users Connections"), new XElement("Root"));
                m_bountyXml.Save(filename);
            }

            m_fileName = filename;
        }

        public void AddUser(string userId, string ethAddress)
        {

            //Che if the user is already there:
            bool user_exists = m_bountyXml.Element("Root").Elements("User").Attributes("UserId").Any(att => att.Value == userId);

            if (user_exists == false)
            {

                XElement newUser = new XElement("User");
                newUser.SetAttributeValue("UserId", userId);

                XElement newUserAddress = new XElement("EthereumAddress");
                newUserAddress.SetAttributeValue("Address", ethAddress);

                newUser.Add(newUserAddress);

                m_bountyXml.Root.Add(newUser);
                m_bountyXml.Save(m_fileName);
            }
            else
            {
                IEnumerable<XElement> users = (from el in m_bountyXml.Root.Elements("User")
                                               where (string)el.Attribute("UserId") == userId
                                               select el);
                XElement user = users.ToList<XElement>()[0];
                bool address_exists = user.Elements("EthereumAddress").Attributes("Address").Any(att => att.Value == ethAddress);

                if (address_exists == false)
                {
                    XElement newUserAddress = new XElement("EthereumAddress");
                    newUserAddress.SetAttributeValue("Address", ethAddress);

                    user.Add(newUserAddress);
                    m_bountyXml.Save(m_fileName);
                }

            }

            
        }
        public void AddUserEthConnection(string userId, string ethAddress, string connectedUserId, string connectedAddress, string campaignName,JObject connectedInfo)
        {
            IEnumerable<XElement> users = (from el in m_bountyXml.Root.Elements("User")
                                           where (string)el.Attribute("UserId") == userId
                                           select el);
            XElement user = users.ToList<XElement>()[0];

            IEnumerable<XElement> ethAddresses = (from el in user.Elements("EthereumAddress")
                                           where (string)el.Attribute("Address") == ethAddress
                                                  select el);

            XElement address = ethAddresses.ToList<XElement>()[0];

            //fileResult.WriteLine(addressList[connected.Address] + " -> " + element["tokenName"].Value<string>() + "(" + element["tokenSymbol"].Value<string>() + ")" + "https://etherscan.io/tx/" + element["hash"].Value<string>() + " "  + element["from"].Value<string>() + " " + element["to"].Value<string>());
            bool connection_exists = user.Elements("Connection").Attributes("TxID").Any(att => att.Value == connectedInfo["hash"].Value<string>());

            if (connection_exists == false)
            {
                XElement newConnection = new XElement("Connection");
                newConnection.SetAttributeValue("UserId", connectedUserId);
                newConnection.SetAttributeValue("Campaign", campaignName);
                newConnection.SetAttributeValue("Address", connectedAddress);
                newConnection.SetAttributeValue("TokenName", connectedInfo["tokenName"].Value<string>());
                newConnection.SetAttributeValue("TokenSymbol", connectedInfo["tokenSymbol"].Value<string>());
                newConnection.SetAttributeValue("Transaction", connectedInfo["hash"].Value<string>());
                newConnection.SetAttributeValue("Description", connectedInfo["from"].Value<string>() + " " + connectedInfo["to"].Value<string>());



                address.Add(newConnection);
                m_bountyXml.Save(m_fileName);
            }
        }
    }
}
