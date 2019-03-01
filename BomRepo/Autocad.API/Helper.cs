using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Geometry;
using BomRepo.BRXXXX.DTO;
using BomRepo.ErrorsCatalog;

namespace BomRepo.Autocad.API
{
    public static class AutocadHelper
    {
        public static KeyValuePair<bool, string> IsValidPartName(string PartName)
        {
            if (PartName.Length == 0) return new KeyValuePair<bool, string>(false, ErrorCatalog.NullPartName.GetUserDescription());
            if (PartName.Length == 1 && PartName[0] == '-') return new KeyValuePair<bool, string>(false, ErrorCatalog.OnlyDashWasFoundInPartName.GetUserDescription());
            int totalDashes = 0;
            foreach (char c in PartName)
            {
                if (c == '-')
                {
                    totalDashes += 1;
                    if (totalDashes > 1) return new KeyValuePair<bool, string>(false, ErrorCatalog.MoreThanOnlyDashWasFoundInPartName.GetUserDescription());
                    continue;
                }
                if (!char.IsLetterOrDigit(c)) return new KeyValuePair<bool, string>(false, ErrorCatalog.InvalidCharactersInPartName.GetUserDescription());
            }
            return new KeyValuePair<bool, string>(true, string.Empty);
        }
        public static KeyValuePair<bool, string> IsValidPartReferenceName(string PartReferenceName)
        {
            if (PartReferenceName.Length == 0) return new KeyValuePair<bool, string>(false, ErrorCatalog.NullPartName.GetUserDescription());

            //Convert PartReferenceName to UpperCase
            PartReferenceName = PartReferenceName.ToUpper();

            //Get parenthesis positions first and validate
            int indexOfOpenP = PartReferenceName.IndexOf('(');
            int indexOfCloseP = PartReferenceName.IndexOf(')');
            if (indexOfOpenP != -1 & indexOfCloseP == -1) return new KeyValuePair<bool, string>(false, ErrorCatalog.MissingCloseParenthesisInPartReferenceName.GetUserDescription());
            if (indexOfOpenP == -1 & indexOfCloseP != -1) return new KeyValuePair<bool, string>(false, ErrorCatalog.MissingOpenParenthesisInPartReferenceName.GetUserDescription());

            //Get qty
            int qty = 1;
            if (indexOfOpenP != -1)
            {
                if (indexOfOpenP != 0) return new KeyValuePair<bool, string>(false, ErrorCatalog.WrongOpenParenthesisLocationInPartReferenceName.GetUserDescription());

                string strQty = PartReferenceName.Substring(1, indexOfCloseP - 1);
                if (!int.TryParse(strQty, out qty)) return new KeyValuePair<bool, string>(false, ErrorCatalog.IntegerNumberRequiredInQuantity.GetUserDescription());

                PartReferenceName = PartReferenceName.Remove(0, indexOfCloseP + 1);
            }

            return IsValidPartName(PartReferenceName);
        }
        public static KeyValuePair<string, PartReferenceDTO> CreatePartReferenceFromPartReferenceName(string PartReferenceName)
        {
            //Convert to upper
            PartReferenceName = PartReferenceName.ToUpper();

            //Validate part reference name
            KeyValuePair<bool, string> partReferenceNameValidationResult = IsValidPartReferenceName(PartReferenceName);
            if (!partReferenceNameValidationResult.Key) return new KeyValuePair<string, PartReferenceDTO>(PartReferenceName + " " + partReferenceNameValidationResult.Value, null);

            //Get quantity
            int qty = 0;
            int indexOfCloseP = PartReferenceName.IndexOf(')');
            if (indexOfCloseP == -1)
                qty = 1;
            else
            {
                string qtyStr = PartReferenceName.Substring(1, indexOfCloseP - 1);
                if (!int.TryParse(qtyStr, out qty))
                    return new KeyValuePair<string, PartReferenceDTO>(ErrorCatalog.IntegerNumberRequiredInQuantity.GetUserDescription(), null);
                PartReferenceName = PartReferenceName.Remove(0, indexOfCloseP + 1);
            }

            //Create PartReference object
            PartReferenceDTO reference = new PartReferenceDTO();
            reference.Name = PartReferenceName;
            reference.Qty = qty;

            //return object
            return new KeyValuePair<string, PartReferenceDTO>(string.Empty, reference);
        }
        public static KeyValuePair<bool, string> InsertAssemblyCountTable(Database DB, string AssemblyName, List<PartReferenceDTO> Parts)
        {
            return new KeyValuePair<bool, string>(true, string.Empty);
        }
        public static bool IsModelSpaceActive()
        {
            return acadApp.DocumentManager.MdiActiveDocument.Database.TileMode;
        }
        public static void InsertTable(Database acDb, Point3d InsertionPoint, string Title, string[] Header, List<string[]> Data, CellAlignment TitleAlignment, CellAlignment HeaderAlignment, CellAlignment[] DataColumnAlignment)
        {
            //Define table style
            double titleRowHeight = 0.5;
            double dataRowHeight = 0.5;

            //Create Table
            Table tb = new Table();
            tb.TableStyle = acDb.Tablestyle;
            tb.InsertRows(1, dataRowHeight, 1);
            tb.InsertColumns(1, 4, Header.Length - 1);
            tb.Position = InsertionPoint;

            //Add Title
            tb.Rows[0].Height = titleRowHeight;
            tb.Rows[0].Alignment = TitleAlignment;
            tb.Cells[0, 0].TextString = Title;

            //Add Header
            tb.Rows[1].Alignment = HeaderAlignment;
            for (int col = 0; col <= Header.Length - 1; col += 1)
                tb.Cells[1, col].TextString = Header[col];

            //Add Data
            for (int row = 0; row <= Data.Count - 1; row += 1)
            {
                tb.InsertRows(tb.Rows.Count, dataRowHeight, 1);
                int currentRow = tb.Rows.Count - 1;
                for (int col = 0; col <= Data[row].Length - 1; col += 1)
                {
                    tb.Cells[currentRow, col].Alignment = DataColumnAlignment[col];
                    tb.Cells[currentRow, col].TextString = Data[row][col];
                }
            }

            //Insert table in drawing
            using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                BlockTable blkTbl = acTrans.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blkTblRec = null;
                if (IsModelSpaceActive()) //Model Space
                    blkTblRec = acTrans.GetObject(blkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                else //Paper Space
                    blkTblRec = acTrans.GetObject(blkTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;
                blkTblRec.AppendEntity(tb);
                acTrans.AddNewlyCreatedDBObject(tb, true);
                acTrans.Commit();
            }
        }
        public static KeyValuePair<List<string>, SortedDictionary<string, PartReferenceDTO>> GetPartReferencesFromSelection(Database acDb, SelectionSet Selection)
        {
            SortedDictionary<string, PartReferenceDTO> dictAssemblyParts = new SortedDictionary<string, PartReferenceDTO>();
            List<string> errorLog = new List<string>();
            using (Transaction acTrans = acDb.TransactionManager.StartTransaction())
            {
                //Get selected objects
                foreach (ObjectId objId in Selection.GetObjectIds())
                {
                    Entity entity = acTrans.GetObject(objId, OpenMode.ForRead) as Entity;

                    //Get PartReferenceName
                    string partReferenceName = string.Empty;
                    if (entity is DBText)
                    {
                        DBText dbText = acTrans.GetObject(objId, OpenMode.ForRead) as DBText;
                        partReferenceName = dbText.TextString;
                    }
                    else
                    {
                        MText mText = acTrans.GetObject(objId, OpenMode.ForRead) as MText;
                        partReferenceName = mText.Text;
                    }

                    //Get PartReference object and add it to the collection if part reference name is valid or add error in log if it is not valid
                    KeyValuePair<string, PartReferenceDTO> result = CreatePartReferenceFromPartReferenceName(partReferenceName);
                    if (result.Value == null)
                        errorLog.Add(result.Key);
                    else
                    {
                        if (dictAssemblyParts.ContainsKey(result.Value.Name))
                            dictAssemblyParts[result.Value.Name].Qty += result.Value.Qty;
                        else
                            dictAssemblyParts.Add(result.Value.Name, result.Value);
                    }
                }

                acTrans.Commit();
            }
            return new KeyValuePair<List<string>, SortedDictionary<string, PartReferenceDTO>>(errorLog, dictAssemblyParts);
        }
        public static void ShowCommandLineMessage(string[] Messages) {
            //Prepares editor
            Editor acEditor = acadApp.DocumentManager.CurrentDocument.Editor;

            foreach (string message in Messages) acEditor.WriteMessage(message + "\n\r");
        }
    }
}

