using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Windows.Forms;
using System.IdentityModel.Tokens;

using BomRepo.BRXXXXX.DTO;
using BomRepo.BRMaster.DTO;
using BomRepo.REST.Client;
using BomRepo.Autocad.API.Forms;
using BomRepo.ErrorsCatalog;

namespace BomRepo.Autocad.API
{
    public class Commands
    {
        private const string assemblyVersion = "1.0.0.0";
        private static UserDTO connecteduser;
        private static ProjectDTO selectedproject;

        /// <summary>
        /// brconnect authenticates user against the web service
        /// </summary>
        [CommandMethod("brconnect")]
        public async void brconnect()
        {
            //Show Messages
            AutocadHelper.ShowCommandLineMessage(new string[1] { "BOMRepo version " + assemblyVersion });

            //Prepare Login Form
            loginform form = new loginform(string.Empty, string.Empty);
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK) {
                AutocadHelper.ShowCommandLineMessage(new string[1] { "BRCONNECT command cancelled." });
                return;
            }

            //Try to connect
            bool connected = await BomRepoServiceClient.Instance.Connect(form.Username, form.Password);

            //Reset active project
            selectedproject = null;

            //Show error if not connected
            if (!connected) {
                AutocadHelper.ShowErrorMessage(BomRepoServiceClient.Instance.Error);
                AutocadHelper.ShowCommandLineMessage(new string[1] { "BRCONNECT successfully executed." });
                return;
            }

            //Try to get user information. If an error was generated is because one of three major reasons
            //1. User does not exist
            //2. User is sysadmin user
            //3. User does not have any costumer associated. In this case a warning should be shown
            connecteduser = await BomRepoServiceClient.Instance.GetUser(form.Username);

            //Verify if errors;
            if (BomRepoServiceClient.Instance.ErrorOccurred) {
                connecteduser = null;
                AutocadHelper.ShowErrorMessage(BomRepoServiceClient.Instance.Error);
                AutocadHelper.ShowCommandLineMessage(new string[1] { "BRCONNECT successfully executed." });
                return;
            }

            //Select a project if requested
            if (form.SelectProject)
            {
                ProjectDTO tempproject = await AutocadHelper.SelectProject(connecteduser.Costumers[0].Number, selectedproject);
                if (tempproject == null)
                    MessageBox.Show("Hi " + connecteduser.Firstname + ", you are now connected to BomRepo services.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else {
                    selectedproject = tempproject;
                    string projectname = selectedproject.Number + " '" + selectedproject.Name + "'";
                    AutocadHelper.ShowCommandLineMessage(new string[1] { projectname + " selected." });
                }
            }
            else {
                MessageBox.Show("Hi " + connecteduser.Firstname + ", you are now connected to BomRepo services.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Show command prompt message
            AutocadHelper.ShowCommandLineMessage(new string[1] { "BRCONNECT successfully executed." });
        }

        /// <summary>
        /// brselectproject select a project to work with.
        /// </summary>
        [CommandMethod("brselectproject")]
        public async void brselectproject()
        {
            //Show Messages
            AutocadHelper.ShowCommandLineMessage(new string[1] { "BOMRepo version " + assemblyVersion });

            //Verify if user is connected
            if (connecteduser == null) {
                AutocadHelper.ShowErrorMessage(ErrorCatalog.UserNotConnected);
                AutocadHelper.ShowCommandLineMessage(new string[1] { "BRSELECTPROJECT successfully executed." });
                return;
            }

            //Get project
            ProjectDTO tempproject = await AutocadHelper.SelectProject(connecteduser.Costumers[0].Number, selectedproject); 
            if (tempproject == null) return;

            selectedproject = tempproject;
            string projectname = selectedproject.Number + " '" + selectedproject.Name + "'";

            AutocadHelper.ShowCommandLineMessage(new string[2] { projectname + " selected." , "BRSELECTPROJECT successfully executed." });
        }

        /// <summary>
        /// bractiveproject returns the name of the active project.
        /// </summary>
        [CommandMethod("bractiveproject")]
        public void bractiveproject() {
            //Prepare message to show
            string message = selectedproject == null ? "No active project" : selectedproject.Number + " '" + selectedproject.Name + "'";

            //Show Messages
            AutocadHelper.ShowCommandLineMessage(new string[3] { "BOMRepo version " + assemblyVersion, message, "BRSELECTPROJECT successfully executed." });
        }

        [CommandMethod("brmyrepo")]
        public async void brmyrepo() {
            //Show Messages
            AutocadHelper.ShowCommandLineMessage(new string[1] { "BOMRepo version " + assemblyVersion });

            //Verify if user is connected
            if (connecteduser == null)
            {
                AutocadHelper.ShowErrorMessage(ErrorCatalog.UserNotConnected);
                AutocadHelper.ShowCommandLineMessage(new string[1] { "BRSELECTPROJECT successfully executed." });
                return;
            }

            //Get All projects
            List<ProjectDTO> projects = await BomRepoServiceClient.Instance.GetProjects(connecteduser.Costumers[0].Number);

            //Show data
            getrepoform form = new getrepoform(connecteduser.Costumers[0].Number, connecteduser.Username, projects, selectedproject.Number);
            form.ShowDialog();
        }

        /// <summary>
        /// brcount asks for an assembly name, then allow the selection of text and mtext objects. All selected objects
        /// are count and a table is created and inserted in the drawing showing the assembly name and all the parts count.
        /// </summary>
        [CommandMethod("brcount")]
        public void brcount()
        {
            //Prepares editor
            Editor acEditor = acadApp.DocumentManager.CurrentDocument.Editor;

            //Show api version
            acEditor.WriteMessage("BOMRepo version " + assemblyVersion + "\n\r");

            //Prepare assembly selection options, only DBText and MTEXT objects are allowed
            PromptEntityOptions promptAssemblyOptions = new PromptEntityOptions("Select assembly name");
            promptAssemblyOptions.SetRejectMessage("Error: Only DBText and MTEXT objects are allowed \n\r");
            promptAssemblyOptions.AddAllowedClass(typeof(MText), true);
            promptAssemblyOptions.AddAllowedClass(typeof(DBText), true);

            //Prompt for assembly object
            PromptEntityResult promptAssemblyResult = acEditor.GetEntity(promptAssemblyOptions);
            if (promptAssemblyResult.Status != PromptStatus.OK) return;

            //Prompt for objects
            PromptSelectionResult promptSelectionResult = acEditor.GetSelection();
            if (promptSelectionResult.Status != PromptStatus.OK) return;

            //Initialize variables
            string assemblyName = string.Empty;

            //Get Database object
            Database acDb = acadApp.DocumentManager.CurrentDocument.Database;

            //Get assembly name
            using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                //Get assembly name
                Entity entity = acTrans.GetObject(promptAssemblyResult.ObjectId, OpenMode.ForRead) as Entity;
                if (entity is DBText)
                {
                    DBText dbText = acTrans.GetObject(promptAssemblyResult.ObjectId, OpenMode.ForRead) as DBText;
                    assemblyName = dbText.TextString.ToUpper();
                }
                else
                {
                    MText mText = acTrans.GetObject(promptAssemblyResult.ObjectId, OpenMode.ForRead) as MText;
                    assemblyName = mText.Text;
                }

                //Validate assembly name
                KeyValuePair<bool, string> assemblyNameValidationResult = AutocadHelper.IsValidPartName(assemblyName);
                if (!assemblyNameValidationResult.Key)
                {
                    acEditor.WriteMessage(assemblyNameValidationResult.Value);
                    return;
                }
                else
                {
                    assemblyName = assemblyName.ToUpper();
                    acEditor.WriteMessage("Assembly name: " + assemblyName + "\n\r");
                }

                acTrans.Commit();
            }

            //Get PartReferences from selection set
            KeyValuePair<List<string>, SortedDictionary<string, PartPlacementDTO>> partReferences = AutocadHelper.GetPartReferencesFromSelection(acDb, promptSelectionResult.Value);
            List<string> errorLog = partReferences.Key;
            SortedDictionary<string, PartPlacementDTO> dictAssemblyParts = partReferences.Value;

            //Show errors and stop execution in case errors were found
            if (errorLog.Count != 0)
            {
                foreach (string errorDescription in errorLog)
                    acEditor.WriteMessage(errorDescription);
                return;
            }

            //Show message indicating everything is ok
            acEditor.WriteMessage("Object were successfully selected \n\r");

            //Ask for Table insertion point
            PromptPointResult promptPointResult = acEditor.GetPoint("Select the point to insert the parts table");
            if (promptPointResult.Status != PromptStatus.OK) return;

            //Prepare all data required
            string[] header = new string[2] { "QTY", "PART NAME" };
            List<string[]> data = new List<string[]>();

            int totalCount = 0;
            foreach (KeyValuePair<string, PartPlacementDTO> kvp in dictAssemblyParts)
            {
                data.Add(new string[2] { kvp.Value.Qty.ToString(), kvp.Value.PartName });
                totalCount += kvp.Value.Qty;
            }
            data.Add(new string[2] { totalCount.ToString(), "TOTAL COUNT" });

            //Define table data alignment. Table style taken directly from the default table style in document
            CellAlignment titleAlignment = CellAlignment.MiddleCenter;
            CellAlignment headerAlignment = CellAlignment.MiddleCenter;
            CellAlignment[] dataColumnAlignment = new CellAlignment[2] { CellAlignment.MiddleCenter, CellAlignment.MiddleLeft };

            //Insert Table
            AutocadHelper.InsertTable(acDb, promptPointResult.Value, assemblyName, header, data, titleAlignment, headerAlignment, dataColumnAlignment);

            //Show successful message
            acEditor.WriteMessage("\n\r" + dictAssemblyParts.Count.ToString() + " unique part(s) was/were found. " + totalCount.ToString() + " total part(s) was/were count.\n\r");
            acEditor.WriteMessage("BRCOUNT successfully executed.\n\r");
        }

        /// <summary>
        /// brpush has the same functionality of wpcount but it takes all the counts and push it to a branch in the online repository. Branch name is the
        /// username. If the assembly already exists in the branch it will be replaced with this new data.
        /// </summary>
        [CommandMethod("brpush")]
        public void brpush()
        {
            //Show Messages
            AutocadHelper.ShowCommandLineMessage(new string[1] { "BOMRepo version " + assemblyVersion });

            //Verify if user is connected
            if (connecteduser == null)
            {
                AutocadHelper.ShowErrorMessage(ErrorCatalog.UserNotConnected);
                AutocadHelper.ShowCommandLineMessage(new string[1] { "BRPUSH successfully executed." });
                return;
            }

            //Verify if project is selected
            if (selectedproject == null) {
                AutocadHelper.ShowErrorMessage(ErrorCatalog.ProjectNotSelected);
                AutocadHelper.ShowCommandLineMessage(new string[1] { "BRPUSH successfully executed." });
                return;
            }

            //Prepares editor
            Editor acEditor = acadApp.DocumentManager.CurrentDocument.Editor;

            //Prepare assembly selection options, only DBText and MTEXT objects are allowed
            PromptEntityOptions promptAssemblyOptions = new PromptEntityOptions("Select assembly name");
            promptAssemblyOptions.SetRejectMessage("Error: Only DBText and MTEXT objects are allowed \n\r");
            promptAssemblyOptions.AddAllowedClass(typeof(MText), true);
            promptAssemblyOptions.AddAllowedClass(typeof(DBText), true);

            //Prompt for assembly object
            PromptEntityResult promptAssemblyResult = acEditor.GetEntity(promptAssemblyOptions);
            if (promptAssemblyResult.Status != PromptStatus.OK) return;

            //Prompt for objects
            PromptSelectionResult promptSelectionResult = acEditor.GetSelection();
            if (promptSelectionResult.Status != PromptStatus.OK) return;

            //Initialize variables
            string assemblyName = string.Empty;

            //Get Database object
            Database acDb = acadApp.DocumentManager.CurrentDocument.Database;

            //Get assembly name
            using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                //Get assembly name
                Entity entity = acTrans.GetObject(promptAssemblyResult.ObjectId, OpenMode.ForRead) as Entity;
                if (entity is DBText)
                {
                    DBText dbText = acTrans.GetObject(promptAssemblyResult.ObjectId, OpenMode.ForRead) as DBText;
                    assemblyName = dbText.TextString.ToUpper();
                }
                else
                {
                    MText mText = acTrans.GetObject(promptAssemblyResult.ObjectId, OpenMode.ForRead) as MText;
                    assemblyName = mText.Text;
                }

                //Validate assembly name
                KeyValuePair<bool, string> assemblyNameValidationResult = AutocadHelper.IsValidPartName(assemblyName);
                if (!assemblyNameValidationResult.Key)
                {
                    acEditor.WriteMessage(assemblyNameValidationResult.Value);
                    return;
                }
                else
                {
                    assemblyName = assemblyName.ToUpper();
                    acEditor.WriteMessage("Assembly name: " + assemblyName + "\n\r");
                }

                acTrans.Commit();
            }

            //Get PartReferences from selection set
            KeyValuePair<List<string>, SortedDictionary<string, PartPlacementDTO>> partReferences = AutocadHelper.GetPartReferencesFromSelection(acDb, promptSelectionResult.Value);
            List<string> errorLog = partReferences.Key;
            SortedDictionary<string, PartPlacementDTO> dictAssemblyParts = partReferences.Value;

            //Show errors and stop execution in case errors were found
            if (errorLog.Count != 0)
            {
                foreach (string errorDescription in errorLog)
                    acEditor.WriteMessage(errorDescription);
                return;
            }

            //Show message indicating everything is ok
            acEditor.WriteMessage("Object were successfully selected \n\r");

            //Generate a random part properties list here. In a near future, we can go inside a block and get all the properties from there
            //and sent it to the publish form matching the real properties with the ones selected by the user in cad
            foreach (KeyValuePair<string, PartPlacementDTO> kvp in dictAssemblyParts) {
                kvp.Value.PartProperties = new List<PartPropertyDTO>();
                kvp.Value.PartProperties.Add(new PartPropertyDTO() { PropertyName = "Length", Value = "12.500" });
                kvp.Value.PartProperties.Add(new PartPropertyDTO() { PropertyName = "Width", Value = "0.000" });
                kvp.Value.PartProperties.Add(new PartPropertyDTO() { PropertyName = "Height", Value = "0.000" });
                kvp.Value.PartProperties.Add(new PartPropertyDTO() { PropertyName = "Thinkness", Value = "0.000" });
            }

            //Transmit data to repository
            pushform form = new pushform(connecteduser.Costumers[0].Number, connecteduser.Username, selectedproject, assemblyName, dictAssemblyParts);
            if (form.ShowDialog() != DialogResult.OK) return;

            //Ask for Table insertion point
            PromptPointResult promptPointResult = acEditor.GetPoint("Select the point to insert the parts table");
            if (promptPointResult.Status != PromptStatus.OK) return;

            //Prepare all data required
            string[] header = new string[2] { "QTY", "PART NAME" };
            List<string[]> data = new List<string[]>();

            int totalCount = 0;
            foreach (KeyValuePair<string, PartPlacementDTO> kvp in dictAssemblyParts)
            {
                data.Add(new string[2] { kvp.Value.Qty.ToString(), kvp.Value.PartName });
                totalCount += kvp.Value.Qty;
            }
            data.Add(new string[2] { totalCount.ToString(), "TOTAL COUNT" });

            //Define table data alignment. Table style taken directly from the default table style in document
            CellAlignment titleAlignment = CellAlignment.MiddleCenter;
            CellAlignment headerAlignment = CellAlignment.MiddleCenter;
            CellAlignment[] dataColumnAlignment = new CellAlignment[2] { CellAlignment.MiddleCenter, CellAlignment.MiddleLeft };

            //Insert Table
            AutocadHelper.InsertTable(acDb, promptPointResult.Value, assemblyName, header, data, titleAlignment, headerAlignment, dataColumnAlignment);

            //Show successful message
            acEditor.WriteMessage("\n\r" + dictAssemblyParts.Count.ToString() + " unique part(s) was/were found. " + totalCount.ToString() + " total part(s) was/were count.\n\r");
            acEditor.WriteMessage("BRPUSH successfully executed.\n\r");
        }

        /// <summary>
        /// brget retrieve assembly information from main or username branch repository and show it in a table
        /// </summary>
        [CommandMethod("brget")]
        public void brget()
        {
        }

        /// <summary>
        /// brclear cleans the content of an existing assembly part in the username branch.
        /// </summary>
        [CommandMethod("brclear")]
        public void brclear()
        {
        }

        /// <summary>
        /// brdelete deletes an existing assembly and all its content if it is not related with any other assembly
        /// </summary>
        [CommandMethod("brdelete")]
        public void brdelete()
        {
        }
    }
}

