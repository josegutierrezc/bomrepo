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

namespace BomRepo.Autocad.API.Forms
{
    public partial class projectselectionform : Form
    {
        public ProjectDTO SelectedProject { get; set; }
        private List<ProjectDTO> projects;

        public projectselectionform()
        {
            InitializeComponent();
            projects = new List<ProjectDTO>();
        }

        public projectselectionform(List<ProjectDTO> projects, ProjectDTO SelectedProject) {
            InitializeComponent();
            this.projects = projects;
            this.SelectedProject = SelectedProject;
        }

        private void projectselectionform_Shown(object sender, EventArgs e)
        {
            foreach (ProjectDTO project in projects) {
                int index = dgvProjects.Rows.Add();
                DataGridViewRow dr = dgvProjects.Rows[index];
                dr.Tag = project;
                dr.Cells[0].Value = project.Number;
                dr.Cells[1].Value = project.Name;
                dr.Cells[2].Value = project.ProjectStatusName;
                if (SelectedProject != null)
                    if (project.Number == SelectedProject.Number) dr.Selected = true;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectedProject = dgvProjects.SelectedRows[0].Tag as ProjectDTO;
            DialogResult = DialogResult.OK;
        }

        private void dgvProjects_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            btnSelect.Enabled = true;
        }

        private void dgvProjects_Paint(object sender, PaintEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Rows.Count == 0)
            {
                using (Graphics g = e.Graphics)
                {
                    Rectangle rect = new Rectangle(new Point(), new Size(dgv.Width, dgv.Height + dgv.RowTemplate.Height));
                    g.FillRectangle(Brushes.Transparent, rect);
                    g.DrawString("No projects to display", new Font(dgv.Font.Name, dgv.Font.Size), Brushes.Black, rect, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }
        }

        private void dgvProjects_DoubleClick(object sender, EventArgs e)
        {
            btnSelect_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
