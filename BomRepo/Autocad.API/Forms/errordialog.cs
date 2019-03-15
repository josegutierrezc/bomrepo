using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using BomRepo.ErrorsCatalog;

namespace BomRepo.Autocad.API.Forms
{
    public partial class errordialog : Form
    {
        private ErrorDefinition errordefinition;
        private string title;

        public errordialog()
        {
            InitializeComponent();
        }
        public errordialog(ErrorDefinition errordefinition, string title) {
            InitializeComponent();
            this.title = title;
            this.errordefinition = errordefinition;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void errordialog_Shown(object sender, EventArgs e)
        {
            pbIcon.Image = errordefinition.Classification == ErrorDefinitionClassification.Critical ? imageList.Images[0] : errordefinition.Classification == ErrorDefinitionClassification.Warning ? imageList.Images[1] : imageList.Images[2];
            lblTitle.Text = title;
            tbErrorDescription.Text += "Code: " + errordefinition.Code + Environment.NewLine;
            tbErrorDescription.Text += "Description: " + errordefinition.UserDescription;
            SystemSounds.Exclamation.Play();
        }
    }
}
