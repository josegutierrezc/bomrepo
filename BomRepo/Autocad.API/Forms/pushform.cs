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
using BomRepo.EntityNamePattern;

namespace BomRepo.Autocad.API.Forms
{
    public partial class pushform : Form
    {
        private string costumernumber;
        private string username;
        private ProjectDTO project;
        private string containername;
        private SortedDictionary<string, PartPlacementDTO> containerparts;
        private List<EntityDTO> projectentities;
        private List<EntityPropertyDTO> projectentityproperties;

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

        private async void pushform_Shown(object sender, EventArgs e)
        {
            try
            {
                //Change cursor
                Cursor.Current = Cursors.WaitCursor;

                //Get all project entities
                projectentities = await BomRepoServiceClient.Instance.GetProjectEntities(costumernumber, project.Number);
                if (projectentities == null) {
                    errordialog error = new errordialog(BomRepoServiceClient.Instance.Error, "BomRepo - Error");
                    error.ShowDialog();
                    return;
                }
                
                //Get all project properties
                projectentityproperties = await BomRepoServiceClient.Instance.GetEntityProperties(costumernumber, project.Number, null);
                if (projectentityproperties == null)
                {
                    errordialog error = new errordialog(BomRepoServiceClient.Instance.Error, "BomRepo - Error");
                    error.ShowDialog();
                    return;
                }

                //Create entity properties dictionary
                //Dictionary<entity.id, List<PartPropertyDTO>>
                Dictionary<int, List<PropertyDTO>> entityproperties = new Dictionary<int, List<PropertyDTO>>();
                foreach (EntityPropertyDTO ep in projectentityproperties) {
                    List<PropertyDTO> properties = null;
                    if (!entityproperties.TryGetValue(ep.EntityId, out properties)) {
                        properties = new List<PropertyDTO>();
                        entityproperties.Add(ep.EntityId, properties);
                    }
                    properties.Add(ep.Property);
                }

                //Create a grid columns dictionary for later datagridcolumns creation Dictionary<propertyname, columnindex>
                Dictionary<string, int> gridcolumns = new Dictionary<string, int>();

                //Here we will try to find an entity for every container part and match autocad properties with entity properties. If an autocad property
                //does not match with an entity property then it will be included but with color red. If an entity property is not found in autocad then
                //it will be shown normally and we will let the user to type the value.
                List<DataElement> gridelements = new List<DataElement>();
                foreach (KeyValuePair<string, PartPlacementDTO> kvp in containerparts) {
                    string partname = kvp.Key;
                    PartPlacementDTO placement = kvp.Value;

                    //Create a new grid data element
                    DataElement element = new DataElement();
                    element.Entity = GetEntityByPattern(partname); ;
                    element.PartName = partname;
                    element.Qty = placement.Qty;
                    element.Properties = new List<PartPropertyDTO>();

                    //Add it to the list
                    gridelements.Add(element);

                    //If entity was not found then no properties need to be matched
                    if (element.Entity != null) {
                        //Lets add every entity required property and see if the autocad part has it
                        foreach (PropertyDTO prop in entityproperties[element.Entity.Id]) {
                            //Lets check if this property was assigned from autocad. If if does then get it.
                            PartPropertyDTO cadprop = null;
                            foreach (PartPropertyDTO pp in placement.PartProperties)
                            {
                                if (prop.Name.ToUpper() == pp.PropertyName.ToUpper())
                                {
                                    cadprop = pp;
                                    break;
                                }
                            }

                            //If property was not found in autocad then add a new one but empty
                            if (cadprop == null) {
                                cadprop = new PartPropertyDTO();
                                cadprop.PropertyId = prop.Id;
                                cadprop.PropertyName = prop.Name;
                                cadprop.Value = string.Empty;
                            }

                            //Add property to the element
                            element.Properties.Add(cadprop);

                            //If column does not exists then add it to the grid and dictionary
                            if (!gridcolumns.ContainsKey(prop.Name.ToUpper()))
                            {
                                //Define column type, format and alignment
                                int colindex = 0;
                                if (prop.IsBoolean)
                                {
                                    colindex = dgvParts.Columns.Add(new DataGridViewCheckBoxColumn());
                                }
                                else if (prop.IsDateTime)
                                {
                                    colindex = dgvParts.Columns.Add(new DataGridViewTextBoxColumn());
                                    dgvParts.Columns[colindex].DefaultCellStyle.Format = "d";
                                    dgvParts.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                }
                                else if (prop.IsDouble)
                                {
                                    colindex = dgvParts.Columns.Add(new DataGridViewTextBoxColumn());
                                    dgvParts.Columns[colindex].DefaultCellStyle.Format = "N4";
                                    dgvParts.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                }
                                else if (prop.IsInteger)
                                {
                                    colindex = dgvParts.Columns.Add(new DataGridViewTextBoxColumn());
                                    dgvParts.Columns[colindex].DefaultCellStyle.Format = "N0";
                                    dgvParts.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                }
                                else
                                {
                                    colindex = dgvParts.Columns.Add(new DataGridViewTextBoxColumn());
                                    dgvParts.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                }

                                //Set column title and width
                                dgvParts.Columns[colindex].HeaderText = prop.Name;
                                dgvParts.Columns[colindex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                                //Add it to the column dictionary
                                gridcolumns.Add(prop.Name.ToUpper(), colindex);
                            }
                        }    
                    }
                }

                //Increase the size of the window if more than 2 properties were added
                if (gridcolumns.Count() > 2) this.Width += 80 - (gridcolumns.Count() - 2);

                //Populate datagridview
                foreach (DataElement element in gridelements) {
                    int rowindex = dgvParts.Rows.Add();
                    List<int> colindices = gridcolumns.Values.ToList();
                    DataGridViewRow dr = dgvParts.Rows[rowindex];
                    dr.Cells[0].Value = element.Qty;
                    dr.Cells[1].Value = element.PartName;
                    dr.Cells[1].ErrorText = element.Entity == null ? "No definition was found that match this part name pattern." : string.Empty;
                    foreach (PartPropertyDTO property in element.Properties) {
                        dr.Cells[gridcolumns[property.PropertyName.ToUpper()]].Value = property.Value;
                        colindices.Remove(gridcolumns[property.PropertyName.ToUpper()]);
                    }

                    //Disable unused cells
                    foreach (int colindex in colindices)
                    {
                        dr.Cells[colindex].ReadOnly = true;
                        dr.Cells[colindex].Style.BackColor = Color.WhiteSmoke;
                    }
                }

                lblProjectNumber.Text = project.Number + " - " + project.Name;
                lblContainerName.Text = containername.ToUpper();
                btnPush.Enabled = containerparts.Count() != 0;
            }
            catch (Exception error)
            {
                MessageBox.Show("An unhandled error occurred. " + error.Message, "BomRepo - Unhandled error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally {
                Cursor.Current = Cursors.WaitCursor;
            }
        }

        #region Helpers
        private EntityDTO GetEntityByPattern(string partname) {
            foreach (EntityDTO entity in projectentities) {
                if (EntityNamePattern.EntityNamePattern.MatchPattern(entity.NamePattern, partname)) return entity;
            }
            return null;
        }
        #endregion
    }

    public class DataElement {
        public EntityDTO Entity { get; set; }
        public string PartName { get; set; }
        public int Qty { get; set; }
        public List<PartPropertyDTO> Properties { get; set; }
    }
}
