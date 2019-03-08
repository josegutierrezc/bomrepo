using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BomRepo.Autocad.API.Forms
{
    public partial class loginform : Form
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool SelectProject { get; set; }
        public loginform()
        {
            InitializeComponent();
        }
        public loginform(string username, string password) {
            InitializeComponent();
            this.Username = username;
            this.Password = password;
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            Username = tbUsername.Text;
            Password = tbPassword.Text;
            SelectProject = cbSelectProject.Checked;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
