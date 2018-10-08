using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BountyChecker
{
    //Read the template xml file containing all the raw spreadheet information about a bounty.
    public class BountyFileLoader
    {
        public List<Campaign> SpreadSheetList { get; set; }

        public BountyFileLoader(string file)
        {
            XDocument xmlBountySpreadSheets = XDocument.Load(file); ;
            
            SpreadSheetList = new List<Campaign>();

            foreach (XElement fileCategory in xmlBountySpreadSheets.Element("Campaign").Elements("SpreadSheet"))
            {
                string filePath = fileCategory.Attribute("File").Value;
                string nameCategory = fileCategory.Attribute("Category").Value;
                string linkSpreadSheet = fileCategory.Attribute("OriginalSpreadSheetLink").Value;
                string projectName = xmlBountySpreadSheets.Element("Campaign").Attribute("Name").Value;
                if (File.Exists(filePath) == true)
                {
                    Campaign cp = new Campaign(nameCategory, filePath, projectName, linkSpreadSheet);
                    SpreadSheetList.Add(cp);
                }
            }
        }

       
    }
}
