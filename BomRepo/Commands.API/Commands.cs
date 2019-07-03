using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;

namespace Commands.API
{
    public class Commands
    {
        #region Variables
        public CompanyDTO CurrentCompany { get; set; }
        public UserDTO CurrentUser { get; set; }
        #endregion

        #region Constructors
        public Commands() { }
        #endregion

        #region Connect
        public ResultObject brconnect() { return null; }
        public ResultObject brdisconnect() { return null; }
        public ResultObject brcurrentuser() { return null; }
        #endregion

        #region Company
        public ResultObject brgetcompanies() { return null; }
        public ResultObject grpushcompany(CompanyDTO Company) { return null; }
        public ResultObject grputcompany(string CompanyNumber, CompanyDTO Company) { return null; }
        public ResultObject grdeletecompany(string CompanyNumber) { return null; }
        #endregion

        #region Branches
        public ResultObject brgetbranches(string ProjectNumber) { return null; }
        public ResultObject brgetbranch(string ProjectNumber, string BranchName) { return null; }
        public ResultObject brpushbranch(string ProjectNumber, ProjectBranchDTO Branch) { return null; }
        public ResultObject brputbranch(string ProjectNumber, ProjectBranchDTO Branch) { return null; }
        public ResultObject brdeletebranch(string ProjectNumber, string BranchName) { return null; }
        public ResultObject brcommitbranch(string ProjectNumber, string BranchName) { return null; }
        #endregion

        #region Project Entities
        public ResultObject brgetentities(string ProjectNumber) { return null; }
        public ResultObject brgetentity(string ProjectNumber, int EntityId) { return null; }
        public ResultObject brpushentity(string ProjectNumber, ProjectEntityDTO Entity) { return null; }
        public ResultObject brputentity(string ProjectNumber, ProjectEntityDTO Entity) { return null; }
        public ResultObject brdeleteentity(string ProjectNumber, int EntityId) { return null; }
        #endregion

        #region Project Parts
        public ResultObject brgetparts(string ProjectNumber) { return null; }
        public ResultObject brgetpart(string ProjectNumber, string PartName) { return null; }
        public ResultObject brpushsimplepart(string ProjectNumber, ProjectSimplePartDTO Part) { return null; }
        public ResultObject brputsimplepart(string ProjectNumber, ProjectSimplePartDTO Part) { return null; }
        public ResultObject brpushcontainerpart(string ProjectNumber, ProjectContainerPartDTO Part) { return null; }
        public ResultObject brputcontainerpart(string ProjectNumber, ProjectContainerPartDTO Part) { return null; }
        public ResultObject brdeletepart(string ProjectNumber, string PartName) { return null; }
        public ResultObject brpushcontainerplacements(string ProjectNumber, List<ProjectContainerPartPlacementDTO> Placements) { return null; }
        #endregion

        #region Entity Properties
        public ResultObject brgetproperties() { return null; }
        public ResultObject brpushproperty(PropertyDTO Property) { return null; }
        public ResultObject brputproperty(int PropertyId, PropertyDTO Property) { return null; }
        public ResultObject brdeleteproperty(int PropertyId) { return null; }
        #endregion
    }
}
