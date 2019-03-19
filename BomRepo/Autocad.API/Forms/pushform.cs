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
        private PartPlacementDTO cadcontainer;
        private SortedDictionary<string, PartPlacementDTO> containerparts;
        private List<EntityDTO> projectentities;
        private List<EntityPropertyDTO> projectentityproperties;

        public pushform()
        {
            InitializeComponent();
            costumernumber = string.Empty;
            username = string.Empty;
            project = null;
            cadcontainer = null;
            containerparts = new SortedDictionary<string, PartPlacementDTO>();
        }

        public pushform(string costumernumber, string username, ProjectDTO project, PartPlacementDTO cadcontainer, SortedDictionary<string, PartPlacementDTO> containerparts) {
            InitializeComponent();
            this.costumernumber = costumernumber;
            this.username = username;
            this.project = project;
            this.cadcontainer = cadcontainer;
            this.containerparts = containerparts;
        }

        private async void btnPush_Click(object sender, EventArgs e)
        {
            bool pushed = await BomRepoServiceClient.Instance.PushContainer(costumernumber, username, project.Number, cadcontainer.PartName, containerparts.Values.ToList());
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

                //Prepare a list of errors
                //Dictionary<errorsection, List<KeyValuePair<partname, errordescription>>>
                //errorsection = { "container", "parts" }
                Dictionary<string, List<KeyValuePair<string, string>>> errorsdict = new Dictionary<string, List<KeyValuePair<string, string>>>();
                errorsdict.Add("container", new List<KeyValuePair<string, string>>());
                errorsdict.Add("parts", new List<KeyValuePair<string, string>>());

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

                //Find an entity that matches container name and create a DataElement for container
                DataElement containergridelement = new DataElement();
                containergridelement.Entity = GetEntityByPattern(cadcontainer.PartName);
                containergridelement.PartName = cadcontainer.PartName;
                containergridelement.Qty = 1;
                containergridelement.Properties = new List<PartPropertyDTO>();

                //Create a grid columns dictionary for container so later we can create datagridcolumns Dictionary<propertyname, columnindex>
                Dictionary<string, int> containergridcolumns = new Dictionary<string, int>();

                //Add columns if an entity was found
                if (containergridelement.Entity != null) {
                    if (entityproperties.ContainsKey(containergridelement.Entity.Id)) {
                        foreach (PropertyDTO prop in entityproperties[containergridelement.Entity.Id])
                        {
                            //Lets check if this property was assigned from autocad. If if does then get it.
                            PartPropertyDTO cadprop = null;
                            foreach (PartPropertyDTO pp in cadcontainer.PartProperties)
                            {
                                if (prop.Name.ToUpper() == pp.PropertyName.ToUpper())
                                {
                                    cadprop = pp;
                                    break;
                                }
                            }

                            //If property was not found in autocad then add a new one but empty
                            if (cadprop == null)
                            {
                                cadprop = new PartPropertyDTO();
                                cadprop.PropertyId = prop.Id;
                                cadprop.PropertyName = prop.Name;
                                cadprop.Value = string.Empty;
                            }

                            //Add property to the element
                            containergridelement.Properties.Add(cadprop);

                            //Add column
                            containergridcolumns.Add(prop.Name.ToUpper(), AddDGVColumn(dgvContainer, prop));
                        }
                    }
                }

                //Populate container datagrid
                int rindex = dgvContainer.Rows.Add();
                DataGridViewRow crow = dgvContainer.Rows[rindex];
                crow.Cells[0].Value = containergridelement.PartName.ToUpper();

                //Search for errors
                if (containergridelement.Entity == null)
                {
                    crow.Cells[0].ErrorText = "No definition was found that match this part name pattern.";
                    errorsdict["container"].Add(new KeyValuePair<string, string>(containergridelement.PartName, crow.Cells[0].ErrorText));
                }
                else if (!containergridelement.Entity.IsContainer) {
                    crow.Cells[0].ErrorText = "Container part is required.";
                    errorsdict["container"].Add(new KeyValuePair<string, string>(containergridelement.PartName, crow.Cells[0].ErrorText));
                }

                //Populate datagrid
                foreach (PartPropertyDTO prop in containergridelement.Properties) {
                    crow.Cells[containergridcolumns[prop.PropertyName.ToUpper()]].Value = prop.Value;
                }

                //Create a grid columns dictionary for later datagridcolumns creation Dictionary<propertyname, columnindex>
                Dictionary<string, int> partsgridcolumns = new Dictionary<string, int>();

                //Here we will try to find an entity for every container part and match autocad properties with entity properties. If an autocad property
                //does not match with an entity property then it will be included but with color red. If an entity property is not found in autocad then
                //it will be shown normally and we will let the user to type the value.
                List<DataElement> partgridelements = new List<DataElement>();
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
                    partgridelements.Add(element);

                    //If entity was not found then no properties need to be matched
                    if (element.Entity != null) {
                        //Lets add every entity required property and see if the autocad part has it
                        if (entityproperties.ContainsKey(element.Entity.Id)) {
                            foreach (PropertyDTO prop in entityproperties[element.Entity.Id])
                            {
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
                                if (cadprop == null)
                                {
                                    cadprop = new PartPropertyDTO();
                                    cadprop.PropertyId = prop.Id;
                                    cadprop.PropertyName = prop.Name;
                                    cadprop.Value = string.Empty;
                                }

                                //Add property to the element
                                element.Properties.Add(cadprop);

                                //If column does not exists then add it to the grid and dictionary
                                if (!partsgridcolumns.ContainsKey(prop.Name.ToUpper()))
                                {
                                    //Add it to the column dictionary
                                    partsgridcolumns.Add(prop.Name.ToUpper(), AddDGVColumn(dgvParts, prop));
                                }
                            }
                        }    
                    }
                }

                //Increase the size of the window if more than 2 properties were added
                if (partsgridcolumns.Count() > 2) this.Width += 80 - (partsgridcolumns.Count() - 2);

                //Populate datagridview
                foreach (DataElement element in partgridelements) {
                    int rowindex = dgvParts.Rows.Add();
                    List<int> colindices = partsgridcolumns.Values.ToList();
                    DataGridViewRow dr = dgvParts.Rows[rowindex];
                    dr.Cells[0].Value = element.Qty;
                    dr.Cells[1].Value = element.PartName;
                    if (element.Entity == null) {
                        dr.Cells[1].ErrorText = ErrorCatalog.EntityDoesNotExist.Title;
                        errorsdict["parts"].Add(new KeyValuePair<string, string>(element.PartName, dr.Cells[1].ErrorText));
                    }
                    
                    foreach (PartPropertyDTO property in element.Properties) {
                        dr.Cells[partsgridcolumns[property.PropertyName.ToUpper()]].Value = property.Value;
                        dr.Cells[partsgridcolumns[property.PropertyName.ToUpper()]].Tag = property;
                        colindices.Remove(partsgridcolumns[property.PropertyName.ToUpper()]);
                    }

                    //Disable unused cells
                    foreach (int colindex in colindices)
                    {
                        dr.Cells[colindex].ReadOnly = true;
                        dr.Cells[colindex].Style.BackColor = Color.WhiteSmoke;
                    }
                }

                lblProjectNumber.Text = project.Number + " - " + project.Name;
                btnPush.Enabled = errorsdict["container"].Count() == 0 & errorsdict["parts"].Count() == 0 & partgridelements.Count() != 0;
                btnErrors.Enabled = !btnPush.Enabled;
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
        private int AddDGVColumn(DataGridView dgv, PropertyDTO property) {
            //Define column type, format and alignment
            int colindex = 0;
            if (property.IsBoolean)
            {
                colindex = dgv.Columns.Add(new DataGridViewCheckBoxColumn());
            }
            else if (property.IsDateTime)
            {
                colindex = dgv.Columns.Add(new DataGridViewTextBoxColumn());
                dgv.Columns[colindex].DefaultCellStyle.Format = "d";
                dgv.Columns[colindex].ValueType = typeof(DateTime);
                dgv.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            else if (property.IsDouble)
            {
                colindex = dgv.Columns.Add(new DataGridViewTextBoxColumn());
                dgv.Columns[colindex].DefaultCellStyle.Format = "N4";
                dgv.Columns[colindex].ValueType = typeof(double);
                dgv.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else if (property.IsInteger)
            {
                colindex = dgv.Columns.Add(new DataGridViewTextBoxColumn());
                dgv.Columns[colindex].DefaultCellStyle.Format = "N0";
                dgv.Columns[colindex].ValueType = typeof(int);
                dgv.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            else
            {
                colindex = dgv.Columns.Add(new DataGridViewTextBoxColumn());
                dgv.Columns[colindex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dgv.Columns[colindex].ValueType = typeof(string);
            }

            //Set column title and width
            dgv.Columns[colindex].HeaderText = property.Name;
            dgv.Columns[colindex].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            return colindex;
        }
        #endregion

        private void dgvParts_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string[] splittedvaluetype = dgvParts.Columns[e.ColumnIndex].ValueType.ToString().Split('.');
            string valuetype = splittedvaluetype[splittedvaluetype.Length - 1].ToLower();
            MessageBox.Show("Value of type " + valuetype + " is required.", "BomRepo - Validation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;
            return;
        }
    }

    public class DataElement {
        public EntityDTO Entity { get; set; }
        public string PartName { get; set; }
        public int Qty { get; set; }
        public List<PartPropertyDTO> Properties { get; set; }
    }
}
