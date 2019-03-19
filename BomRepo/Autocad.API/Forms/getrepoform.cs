using BomRepo.BRXXXXX.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BomRepo.REST.Client;

namespace BomRepo.Autocad.API.Forms
{
    public partial class getrepoform : Form
    {
        private string costumernumber;
        private string username;
        private List<ProjectDTO> projects;
        private ProjectDTO selectedproject;
        private string userselectedprojectnumber;
        public getrepoform()
        {
            InitializeComponent();
            projects = new List<ProjectDTO>();
        }

        public getrepoform(string costumernumber, string username, List<ProjectDTO> projects, string userselectedprojectnumber) {
            InitializeComponent();
            this.costumernumber = costumernumber;
            this.username = username;
            this.projects = projects;
            this.userselectedprojectnumber = userselectedprojectnumber;
        }

        private void getrepoform_Shown(object sender, EventArgs e)
        {
            cbProjects.Enabled = false;
            if (projects.Count() == 0) return;
            foreach (ProjectDTO p in projects) {
                cbProjects.Items.Add(new KeyValuePair<string, ProjectDTO>(p.Number + " - " + p.Name, p));
                if (p.Number == userselectedprojectnumber) selectedproject = p;
            }
            cbProjects.DataSource = cbProjects.Items;
            cbProjects.DisplayMember = "Key";
            cbProjects.ValueMember = "Value";

            //The execution order below is important, please do not modify unless you know what you are doing. 
            //1. Combo SelectedValue is changed then if selectedproject != selectedvalue then a SelectedIndexChanged event will be fired
            //but because cbProject is not enabled nothing will happen.
            //2. Populate listview with the selectedproject
            //3. Enable Combo
            cbProjects.SelectedValue = selectedproject;
            PopulateListView();
            cbProjects.Enabled = true;
        }

        private void cbProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbProjects.Enabled) return;
            selectedproject = cbProjects.SelectedValue as ProjectDTO;
            PopulateListView();
        }

        #region Helpers
        private async void PopulateListView()
        {
            List<PartsContainerDTO> parts = await BomRepoServiceClient.Instance.GetUserRepository(costumernumber, username, selectedproject.Number);

            lvParts.Items.Clear();
            lvParts.Groups.Clear();
            foreach (PartsContainerDTO pc in parts)
            {
                ListViewGroup lvg = new ListViewGroup(pc.ParentPartName);
                int total = 0;
                lvParts.Groups.Add(lvg);
                foreach (PartPlacementDTO pp in pc.Placements)
                {
                    ListViewItem lvi = new ListViewItem(pp.PartName);
                    lvi.Tag = pp;
                    lvi.SubItems.Add(pp.Qty.ToString());
                    lvi.Group = lvg;
                    lvParts.Items.Add(lvi);
                    total += pp.Qty;
                }

                ListViewItem totallvi = new ListViewItem("Total");
                totallvi.SubItems.Add(total.ToString());
                totallvi.Group = lvg;
                totallvi.Font = new Font(lvParts.Font.Name, lvParts.Font.Size, FontStyle.Bold);
                lvParts.Items.Add(totallvi);
            }
        }
        #endregion

        private void btnCopyClipboard_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (ListViewGroup g in lvParts.Groups) {
                sb.AppendLine(g.Header);
                foreach (ListViewItem lvi in g.Items)
                    sb.AppendLine(lvi.Text + '\t' + lvi.SubItems[1].Text);
                sb.AppendLine(string.Empty);
            }
            Clipboard.SetText(sb.ToString());
        }
    }
}
