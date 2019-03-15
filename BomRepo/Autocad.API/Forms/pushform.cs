using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BomRepo.BRXXXXX.DTO;
using BomRepo.REST.Client;
using BomRepo.ErrorsCatalog;

namespace BomRepo.Autocad.API.Forms
{
    public partial class pushform : Form
    {
        private string costumernumber;
        private string username;
        private ProjectDTO project;
        private string containername;
        private SortedDictionary<string, PartPlacementDTO> containerparts;

        public pushform()
        {
            InitializeComponent();
            costumernumber = string.Empty;
            username = string.Empty;
            project = null;
            containername = string.Empty;
            containerparts = new SortedDictionary<string, PartPlacementDTO>();
        }

        public pushform(string costumernumber, string username, ProjectDTO project, string containername, SortedDictionary<string, PartPlacementDTO> containerparts) {
            InitializeComponent();
            this.costumernumber = costumernumber;
            this.username = username;
            this.project = project;
            this.containername = containername;
            this.containerparts = containerparts;
        }

        private async void btnPush_Click(object sender, EventArgs e)
        {
            bool pushed = await BomRepoServiceClient.Instance.PushContainer(costumernumber, username, project.Number, containername, containerparts.Values.ToList());
            if (!pushed) {
                errordialog errord = new errordialog(BomRepoServiceClient.Instance.Error, "Unable to push this container to your repository.");
                errord.ShowDialog();
                return;
            }

            MessageBox.Show("Container was successfully pushed to your repository.", "BomRepo - Container pushed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }

        private void pushform_Shown(object sender, EventArgs e)
        {
            lblProjectNumber.Text = project.Number + " - " + project.Name;
            lblContainerName.Text = containername.ToUpper();
            foreach (KeyValuePair<string, PartPlacementDTO> kvp in containerparts) {
                PartPlacementDTO pp = kvp.Value;
                ListViewItem lvi = new ListViewItem(pp.PartName.ToUpper());
                lvi.Tag = pp;
                lvi.SubItems.Add(pp.Qty.ToString());

                if (pp.PartName.ToUpper() == containername.ToUpper())
                {
                    lvi.ToolTipText = ErrorCatalog.SelfContainedError.UserDescription.Replace("@1", containername);
                    lvi.ForeColor = Color.Red;
                }
                else lvi.ToolTipText = "No errors detected.";

                lvParts.Items.Add(lvi);
            }
            btnPush.Enabled = containerparts.Count() != 0;
        }
    }
}
