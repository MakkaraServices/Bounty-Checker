using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BountyChecker.Loader
{
    public class ProjectLoader
    {
        static string Bounties = "Bounties";
        static string SpreadSheet = "SpreadSheet";
        public static Dictionary<string, string> GetProjects()
        {
            List<string>  folders = System.IO.Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + Bounties).ToList<string>();
            Dictionary<string,string> foldersCleaned = new Dictionary<string, string>();


            foreach (string folder in folders)
            {
                foldersCleaned.Add(folder.Replace(AppDomain.CurrentDomain.BaseDirectory + Bounties + "\\", ""), folder);
            }


            return foldersCleaned;
        }

        public static List<string> GetProjectsCampaigns(string projectName)
        {
            List<string> campaigns = new List<string>();



            DirectoryInfo d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + Bounties + "\\" + projectName + "\\" + SpreadSheet);
            FileInfo[] Files = d.GetFiles("*.txt"); 
            
            foreach (FileInfo file in Files)
            {
                campaigns.Add(file.Name.Replace(".txt",""));
            }

            return campaigns;
        }
    }
}
