using BountyChecker.Loader;
using BountyChecker.Reporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BountyChecker
{
    public partial class Form1 : Form
    {
        Form1 m_mainForm;
        string ProjectSelected;
        string CampaignSelected;
        public Form1()
        {
            InitializeComponent();
            m_mainForm = this;
        }

        static async Task<bool> PerformCheck(Form1 form)
        {
            

            ProgressReporter<ProgressStatus> progressDispa = new ProgressReporter<ProgressStatus>();
            progressDispa.ProgressChanged += new EventHandler<ProgressStatus>(form.Task_ProgressChanged);

            //Main folder where you find the spreadsheet raw data.
            string localData = @"Bounties\" + form.ProjectSelected + "\\" + form.ProjectSelected + "SpreadSheet.xml";
            BountyFileLoader bl = new BountyFileLoader(localData);
            string projectName = form.ProjectSelected;
            
            try
            {
                foreach (Campaign campaign in bl.SpreadSheetList)
                    campaign.Partecipants = await Task.Run(() => TextParser.GetUserCampaignEthAddress(campaign, progressDispa));

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            bool result = false;
            foreach (Campaign campaign in bl.SpreadSheetList)
                result = await Task.Run(() => AddressComparer.CheckAddressList(projectName,campaign, progressDispa, form.textBox1.Text));
            
            return result;
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            // Start the HandleFile method.
            await PerformCheck(this.m_mainForm);

            string outputFile = @"Bounties\" + this.ProjectSelected + "\\";
            string localData = @"Bounties\" + this.ProjectSelected + "\\" + this.ProjectSelected + "SpreadSheet.xml";
            BountyFileLoader bl = new BountyFileLoader(localData);
            DataViewer view = new DataViewer(ProjectSelected + "_results.xml", outputFile, bl);

            view.CreateHtmlOutput(outputFile);

            //view.ShowConnectionsPerCategory(CampaignSelected);
            System.Diagnostics.Process.Start(outputFile + "index.html");
            button1.Enabled = true;
        }

        public void Task_ProgressChanged(object sender, ProgressStatus e)
        {
            switch (e.ReportType)
            {
                case Report.Address:
                    progressBar1.Invoke(new Action(() => progressBar1.Maximum = e.Total));
                    progressBar1.Invoke(new Action(() => progressBar1.Value = e.Done));
                    label1.Invoke(new Action(() => label1.Text = e.Description));
                    break;
                case Report.Transaction:
                    progressBar2.Invoke(new Action(() => progressBar2.Maximum = e.Total));
                    progressBar2.Invoke(new Action(() => progressBar2.Value = e.Done));
                    label2.Invoke(new Action(() => label2.Text = e.Description));
                    break;
                case Report.LineParser:
                    progressBar1.Invoke(new Action(() => progressBar1.Maximum = e.Total));
                    progressBar1.Invoke(new Action(() => progressBar1.Value = e.Done));
                    label1.Invoke(new Action(() => label1.Text = e.Description));
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            this.comboBox1.SelectedIndexChanged += new System.EventHandler(ComboBox1_SelectedIndexChanged);

            Dictionary<string, string> list = ProjectLoader.GetProjects();

            foreach (KeyValuePair<string, string> project in list)
                comboBox1.Items.Add(project.Key);

        }


        private void ComboBox1_SelectedIndexChanged(object sender,
        System.EventArgs e)
        {
            this.ProjectSelected = comboBox1.SelectedItem.ToString();

            List<string> list = ProjectLoader.GetProjectsCampaigns(this.ProjectSelected);

        }
    }
}
