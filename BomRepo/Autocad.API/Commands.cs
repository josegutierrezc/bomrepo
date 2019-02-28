using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using BomRepo.BRXXXX.DTO;
using System.Collections.Generic;

namespace BomRepo.Autocad.API
{
    public class Commands
    {
        private const string assemblyVersion = "1.0.0.0";

        /// <summary>
        /// brconnect authenticates user against the web service
        /// </summary>
        [CommandMethod("brconnect")]
        public void brconnect()
        {
        }

        /// <summary>
        /// brdefaultproject defines a default project to work with.
        /// </summary>
        [CommandMethod("brdefaultproject")]
        public void brdefaultproject()
        {
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
            acEditor.WriteMessage("BRCOUNT command running ...\n\r");

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
                KeyValuePair<bool, string> assemblyNameValidationResult = Helper.IsValidPartName(assemblyName);
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
            KeyValuePair<List<string>, SortedDictionary<string, PartReferenceDTO>> partReferences = Helper.GetPartReferencesFromSelection(acDb, promptSelectionResult.Value);
            List<string> errorLog = partReferences.Key;
            SortedDictionary<string, PartReferenceDTO> dictAssemblyParts = partReferences.Value;

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
            foreach (KeyValuePair<string, PartReferenceDTO> kvp in dictAssemblyParts)
            {
                data.Add(new string[2] { kvp.Value.Qty.ToString(), kvp.Value.Name });
                totalCount += kvp.Value.Qty;
            }
            data.Add(new string[2] { totalCount.ToString(), "TOTAL COUNT" });

            //Define table data alignment. Table style taken directly from the default table style in document
            CellAlignment titleAlignment = CellAlignment.MiddleCenter;
            CellAlignment headerAlignment = CellAlignment.MiddleCenter;
            CellAlignment[] dataColumnAlignment = new CellAlignment[2] { CellAlignment.MiddleCenter, CellAlignment.MiddleLeft };

            //Insert Table
            Helper.InsertTable(acDb, promptPointResult.Value, assemblyName, header, data, titleAlignment, headerAlignment, dataColumnAlignment);

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

