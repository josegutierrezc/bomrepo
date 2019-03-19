using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.ErrorsCatalog
{
    public static class ErrorCatalog
    {
        public static ErrorDefinition NullPartName = new ErrorDefinition("CAD.PN.100", "Invalid part name", ErrorDefinitionClassification.Critical, "Part name cannot be null or empty.", "", false, "Type a part name.");
        public static ErrorDefinition InvalidCharactersInPartName = new ErrorDefinition("CAD.PN.101", "Invalid part name", ErrorDefinitionClassification.Critical, "Part name can only contain letters, numbers and one dash '-' symbol.", "", false, "Remove all characters that are not allowed from the part name.");
        public static ErrorDefinition OnlyDashWasFoundInPartName = new ErrorDefinition("CAD.PN.102", "Invalid part name", ErrorDefinitionClassification.Critical, "Part name cannot only contain the dash '-' symbol.", "", false, "Add more characters to the part name.");
        public static ErrorDefinition MoreThanOnlyDashWasFoundInPartName = new ErrorDefinition("CAD.PN.103", "Invalid part name", ErrorDefinitionClassification.Critical, "More than one dash '-' symbol was found in Part name.", "", false, "Remove the extra dash '-' symbols from the part name.");
        public static ErrorDefinition MissingCloseParenthesisInPartReferenceName = new ErrorDefinition("CAD.PR.101", "Invalid part reference name", ErrorDefinitionClassification.Critical, "Missing close parenthesis ')' symbol.", "", false, "Add a close parenthesis ) at the end of the quantity.");
        public static ErrorDefinition MissingOpenParenthesisInPartReferenceName = new ErrorDefinition("CAD.PR.102", "Invalid part reference name", ErrorDefinitionClassification.Critical, "Missing open parenthesis '(' symbol.", "", false, "Add an open parenthesis at the beginning of the quantity.");
        public static ErrorDefinition WrongOpenParenthesisLocationInPartReferenceName = new ErrorDefinition("CAD.PR.103", "Invalid part reference name", ErrorDefinitionClassification.Critical, "Open parenthesis '(' symbol is always the first character in the part reference name when more than one (1) quantity is required.", "", false, "Move the open parenthesis from the current position.");
        public static ErrorDefinition IntegerNumberRequiredInQuantity = new ErrorDefinition("CAD.PR.104", "Integer number required", ErrorDefinitionClassification.Critical, "An integer number is required between open and close parenthesis.", "", false, "Remove all characters that are not numbers between the open '(' and close ')' parenthesis.");
        public static ErrorDefinition BomRepoServiceUnavailable = new ErrorDefinition("REST.API.101", "Service unavailable", ErrorDefinitionClassification.Critical, "An error occurred trying to connect with the remote repository server.", "", false, "Remote respository is not responding. Check your internet connection first then try again. If you get the same error please call your System Administrator.");
        public static ErrorDefinition MissingCredentials = new ErrorDefinition("CAD.US.101", "User logged off", ErrorDefinitionClassification.Informational, "There is no user connected to the service.", "", false, "Please use command BRCONNECT to start your session.");
        public static ErrorDefinition ServiceUnavailableOrInvalidCredentials = new ErrorDefinition("CAD.US.102", "Service unavailable or invalid credentials", ErrorDefinitionClassification.Critical, "An error occurred trying to connect with the remote repository server.", "", false, "Before contact your System Administrator please try to execute the command BRCONNECT again and verify you are successfully connected to the service.");
        public static ErrorDefinition InvalidCredentials = new ErrorDefinition("CAD.US.103", "Invalid credentials", ErrorDefinitionClassification.Informational, "Username and/or password are incorrect.", "", false, "Please verify again your are typing them correctly.");
        public static ErrorDefinition NoCostumerAssociation = new ErrorDefinition("SYS.101", "No costumer association", ErrorDefinitionClassification.Warning, "This user account is not associated to any costumer yet. Please contact your System Administrator.", "User account needs to be associated with costumer in database BRMaster, table CostumerUsers.", false, "Please call your System Administrator.");
        public static ErrorDefinition UserNotConnected = new ErrorDefinition("CAD.US.104", "No user connected", ErrorDefinitionClassification.Warning, "No user is connected to BomRepo services. ", "", false, "Please execute command BRCONNECT.");
        public static ErrorDefinition ProjectNotSelected = new ErrorDefinition("CAD.US.105", "No project selected", ErrorDefinitionClassification.Warning, "No project has been selected.", "", false, "Please execute command BRSELECTPROJECT.");
        public static ErrorDefinition ProjectDoesNotExist = new ErrorDefinition("REST.API.102", "Project does not exist", ErrorDefinitionClassification.Warning, "Project does not exist.", "Project with id ProjectId or number ProjectNumber was not found in database.", true, "Search for this project id in table Projects inside Costumer database.");
        public static ErrorDefinition CostumerDoesNotExist = new ErrorDefinition("REST.API.103", "Costumer does not exist", ErrorDefinitionClassification.Warning, "Costumer does not exist.", "Costumer number was not found in database.", true, "Search for this costumer id in table Costumers inside BRMaster database.");
        public static ErrorDefinition UserDoesNotExist = new ErrorDefinition("REST.API.104", "User does not exist", ErrorDefinitionClassification.Warning, "User does not exist.", "User was not found in database.", true, "Search for this username in table Users on BRMaster database.");
        public static ErrorDefinition EntityDoesNotExist = new ErrorDefinition("REST.API.105", "Entity does not exist", ErrorDefinitionClassification.Warning, "Entity does not exist.", "General Entity was not found in database.", true, "Search for this entity in table Entities on Costumer database.");
        public static ErrorDefinition ValidationFailed = new ErrorDefinition("BRX.101", "Validation fail", ErrorDefinitionClassification.Critical, "Validation fail", string.Empty, true, string.Empty); //Use this definition to return validation errors in all BRXXXXXX managers.
        public static ErrorDefinition WrongPartName = new ErrorDefinition("BRX.102", "Wrong part name", ErrorDefinitionClassification.Warning, "Part @1 is not written correctly. No part definition was found with this pattern.", string.Empty, false, "The name of the part has to match a unique pattern that must be associated with a definition of a part and a project. Check that you have correctly written the name of your part and that there is a definition already created for it.");
        public static ErrorDefinition SelfContainedError = new ErrorDefinition("BRX.103", "Container pointing to itself", ErrorDefinitionClassification.Warning, "Container @1 cannot be part of its own content.", string.Empty, false, "Remove all self references.");
        public static ErrorDefinition ContainerPartRequired = new ErrorDefinition("BRX.104", "Container part required", ErrorDefinitionClassification.Warning, "A container part is required and @1 is not.", string.Empty, false, "The part definition associated with this part is not defined as a container. Please change the part name.");
        public static ErrorDefinition UserBranchDoesNotExist = new ErrorDefinition("BRX.105", "User branch does not exist", ErrorDefinitionClassification.Warning, "Part @1 cannot be created because is assigned to a user branch that does not exist.", string.Empty, false, "Try to push all content again.");
        public static ErrorDefinition EntityProjectReferenceDoesNotExist = new ErrorDefinition("BRX.106", "Entity and Project reference does not exist", ErrorDefinitionClassification.Warning, "Part @1 cannot be created because a reference between its definition and project does not exist.", string.Empty, false, "Please contact your System Administrator.");
        public static ErrorDefinition CreateFrom(ErrorDefinition Definition, string DeveloperDescription) {
            return new ErrorDefinition(Definition.Code, Definition.Title, Definition.Classification, Definition.UserDescription, DeveloperDescription, Definition.OnlyForDeveloperEye, Definition.Resolution);
        }
    }

    public enum ErrorDefinitionClassification {
        Critical = 16,
        Warning = 48,
        Informational = 64
    }
    public class ErrorDefinition
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public ErrorDefinitionClassification Classification { get; set; }
        public string UserDescription { get; set; }
        public string DeveloperDescription { get; set; }
        public bool OnlyForDeveloperEye { get; set; }
        public string Resolution { get; set; }
        public ErrorDefinition(string Code, string Title, ErrorDefinitionClassification Classification, string UserDescription, string DeveloperDescription, bool OnlyForDeveloperEye, string Resolution)
        {
            this.Code = Code;
            this.Title = Title;
            this.Classification = Classification;
            this.UserDescription = UserDescription;
            this.DeveloperDescription = DeveloperDescription;
            this.OnlyForDeveloperEye = OnlyForDeveloperEye;
            this.Resolution = Resolution;
        }
        public string GetUserDescription()
        {
            return "Error " + Code + " " + Title + ": " + UserDescription + "\n\r";
        }
        public void ReplaceParameterValueInUserDescription(string Parameter, string Value) {
            UserDescription = UserDescription.Replace(Parameter, Value);
        }
    }
}
