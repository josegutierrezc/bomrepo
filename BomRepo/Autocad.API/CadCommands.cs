using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using BomRepo.Autocad.API.Models;

namespace BomRepo.Autocad.API
{
    public class CadCommands
    {
        #region Private Variables
        private string costumernumber { get; set; }
        private bool frommemory { get; set; }
        private Project selectedproject { get; set; }
        #endregion

        #region Public Variables
        public string ConsoleOutput { get; set; }
        #endregion

        #region Constructors
        public CadCommands(string CostumerNumber, bool FromMemory) {
            frommemory = FromMemory;
            costumernumber = CostumerNumber;
            selectedproject = null;
        }
        public CadCommands(string CostumerNumber, string ProjectNumber, bool FromMemory) {
            frommemory = FromMemory;
            costumernumber = CostumerNumber;
            selectedproject = DataRepository.Instance(frommemory).GetProjectByNumber(costumernumber, ProjectNumber);
        }
        #endregion

        public void ShowAllProjects() {
            if (frommemory) {
                ShowCommandHeader("BRGETPROJECTS");
                foreach (Project p in DataRepository.Instance(frommemory).GetAllProjects(costumernumber)) {
                    string active = p.InactiveSince == null ? "true" : "false";
                    Console.WriteLine("Number:\t" + p.Number);
                    Console.WriteLine("Name:\t" + p.Name);
                    Console.WriteLine("Active:\t" + active);
                    Console.WriteLine("---------------------------------------");
                }
                Console.WriteLine("");
            }
        }

        public void GetDefaultProject() {
            if (frommemory)
            {
                ShowCommandHeader("BRGETDEFAULTPROJECT");
                if (selectedproject == null)
                    Console.WriteLine("No default project has been selected");
                else {
                    string active = selectedproject.InactiveSince == null ? "true" : "false";

                    Console.WriteLine("Number \tName \t\t\tActive");
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine(selectedproject.Number + "\t" + selectedproject.Name + "\t\t" + active);
                }
                Console.WriteLine("");
            }
        }

        public void SetDefaultProject() {
            if (frommemory) {
                ShowCommandHeader("BRSETDEFAULTPROJECT");
                Console.Write("Type Project number: ");
                string projectnumber = Console.ReadLine();
                Project project = DataRepository.Instance(frommemory).GetProjectByNumber(costumernumber, projectnumber);
                if (project == null)
                    Console.WriteLine("Project does not exist");
                else {
                    selectedproject = project;
                    Console.WriteLine("Set");
                }
                Console.WriteLine("");
            }
        }

        public void GetProperties() {
            ShowCommandHeader("BRGETPROPERTIES");
            if (selectedproject == null) {
                ShowNoProjectSelected();
                return;
            }

            if (frommemory) {
                foreach (Property prop in DataRepository.Instance(frommemory).GetAllProperties(costumernumber, selectedproject.Number)) {
                    string proptype = prop.IsString ? "string" : prop.IsDateTime ? "date" : prop.IsDouble ? "decimal" : prop.IsInteger ? "integer" : "boolean";
                    Console.WriteLine("Name:\t" + prop.Name);
                    Console.WriteLine("Type:\t" + proptype);
                    Console.WriteLine("----------------------------");
                }
                Console.WriteLine("");
            }
        }

        public void AddProperty() {
            ShowCommandHeader("BRADDPROPERTY");
            if (selectedproject == null) {
                ShowNoProjectSelected();
                return;
            }

            if (frommemory) {
                Property entity = new Property();

                bool addnew = true;
                while (addnew) {
                    Console.Write("Property name: ");
                    entity.Name = Console.ReadLine();

                    Console.Write("Type value type id (" + PropertyValueTypes.AllToString() + ")");
                    string strtypeid = Console.ReadLine();
                    int typeId = 0;
                    if (int.TryParse(strtypeid, out typeId))
                    {
                        entity.SetPropertyType(typeId);
                        int added = DataRepository.Instance(frommemory).AddProperty(costumernumber, selectedproject.Number, entity);
                        if (DataRepository.Instance(frommemory).ErrorOccurred)
                            Console.WriteLine("Property was not added. " + DataRepository.Instance(frommemory).ErrorDescription);
                        else
                            Console.WriteLine("Added");

                        Console.Write("Do you want to add one more (Y/N): ");
                        string onemore = Console.ReadLine();

                        if (onemore.ToLower() == "y") addnew = true; else addnew = false;
                    }
                    else
                    {
                        Console.WriteLine("Invalid type id. Please type a number between 1 and 5 for definition value type.");
                        addnew = true;
                    }
                }
            }
        }

        public void RemoveProperty() {
            ShowCommandHeader("BRREMOVEPROPERTY");
            if (selectedproject == null)
            {
                ShowNoProjectSelected();
                return;
            }

            if (frommemory) {
                foreach (Property prop in DataRepository.Instance(frommemory).GetAllProperties(costumernumber, selectedproject.Number))
                    Console.WriteLine("Id: " + prop.Id + " Name: " + prop.Name);
                Console.WriteLine("");
                Console.Write("Type property Id to remove: ");
                string strid = Console.ReadLine();

                bool removed = DataRepository.Instance(frommemory).RemoveProperty(Convert.ToInt32(strid));
                if (removed)
                    Console.WriteLine("Removed");
                else
                    Console.WriteLine("Property could not be removed. " + DataRepository.Instance(frommemory).ErrorDescription);
                Console.WriteLine("");
            }
        }

        public void AssignProperty() {
            ShowCommandHeader("BRASSIGNROPERTY");
            if (selectedproject == null)
            {
                ShowNoProjectSelected();
                return;
            }

            if (frommemory) {
                Console.WriteLine("Assign property:");
                foreach (Property prop in DataRepository.Instance(frommemory).GetAllProperties(costumernumber, selectedproject.Number))
                    Console.WriteLine("Id: " + prop.Id + " Name: " + prop.Name);
                Console.Write("Type property Id: ");
                string strpropid = Console.ReadLine();

                int propId = 0;
                if (!int.TryParse(strpropid, out propId)) {

                }

                Console.WriteLine("");
                Console.WriteLine("To part definitions: ");
                foreach (PartDefinition pd in DataRepository.Instance(frommemory).GetAllWherePropertyDoesNotBelong(costumernumber, selectedproject.Number, propId))
                    Console.WriteLine("Id: " + pd.Id + " Name: " + pd.Name);
                Console.Write("Type Part Definition Id: ");
                string pdstrid = Console.ReadLine();

            }
        }

        public void GetPartDefinitions() {
            ShowCommandHeader("BRGETDEFINITIONS");
            if (selectedproject == null) {
                ShowNoProjectSelected();
                return;
            }

            if (frommemory) {
                foreach (PartDefinition pd in DataRepository.Instance(frommemory).GetAllPartDefinitions(costumernumber, selectedproject.Number)) {
                    string properties = string.Empty;
                    foreach (Property prop in DataRepository.Instance(frommemory).GetPropertiesByPartDefinitionId(pd.Id))
                        properties += prop.Name + ", ";
                    if (properties != string.Empty) properties = properties.Substring(0, properties.Length - 2); else properties = "None";

                    Console.WriteLine("Name:\t\t" + pd.Name);
                    Console.WriteLine("Description:\t" + pd.Description);
                    Console.WriteLine("Pattern:\t" + pd.Pattern);
                    Console.WriteLine("Properties:\t" + properties);
                    Console.WriteLine("-----------------------------------------");
                }
                Console.WriteLine("");
            }
        }

        #region Helpers
        private void ShowCommandHeader(string CommandName) {
            Console.WriteLine("");
            Console.WriteLine("BOMREPO Commands v1.0");
            Console.WriteLine("Executing " + CommandName.ToUpper() + " command ...");
            Console.WriteLine("");
        }
        private void ShowNoProjectSelected() {
            Console.WriteLine("No project has been selected");
            Console.WriteLine("");
        }
        #endregion


    }
}
