﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.ErrorsCatalog
{
    public static class ErrorCatalog
    {
        public static ErrorDefinition NullPartName = new ErrorDefinition("CAD.PN.100", "Invalid part name", ErrorDefinitionClassification.Critical, "Part name cannot be null or empty.", "", false);
        public static ErrorDefinition InvalidCharactersInPartName = new ErrorDefinition("CAD.PN.101", "Invalid part name", ErrorDefinitionClassification.Critical, "Part name can only contain letters, numbers and one dash '-' symbol.", "", false);
        public static ErrorDefinition OnlyDashWasFoundInPartName = new ErrorDefinition("CAD.PN.102", "Invalid part name", ErrorDefinitionClassification.Critical, "Part name cannot only contain the dash '-' symbol.", "", false);
        public static ErrorDefinition MoreThanOnlyDashWasFoundInPartName = new ErrorDefinition("CAD.PN.103", "Invalid part name", ErrorDefinitionClassification.Critical, "More than one dash '-' symbol was found in Part name.", "", false);
        public static ErrorDefinition MissingCloseParenthesisInPartReferenceName = new ErrorDefinition("CAD.PR.101", "Invalid part reference name", ErrorDefinitionClassification.Critical, "Missing close parenthesis ')' symbol.", "", false);
        public static ErrorDefinition MissingOpenParenthesisInPartReferenceName = new ErrorDefinition("CAD.PR.102", "Invalid part reference name", ErrorDefinitionClassification.Critical, "Missing open parenthesis '(' symbol.", "", false);
        public static ErrorDefinition WrongOpenParenthesisLocationInPartReferenceName = new ErrorDefinition("CAD.PR.103", "Invalid part reference name", ErrorDefinitionClassification.Critical, "Open parenthesis '(' symbol is always the first character in the part reference name when more than one (1) quantity is required.", "", false);
        public static ErrorDefinition IntegerNumberRequiredInQuantity = new ErrorDefinition("CAD.PR.104", "Integer number required", ErrorDefinitionClassification.Critical, "An integer number is required between open and close parenthesis.", "", false);
        public static ErrorDefinition BomRepoServiceUnavailable = new ErrorDefinition("REST.API.101", "Service unavailable", ErrorDefinitionClassification.Critical, "An error occurred trying to connect with the remote repository server. Please contact your System Administrator.", "", false);
        public static ErrorDefinition MissingCredentials = new ErrorDefinition("CAD.US.101", "User logged off", ErrorDefinitionClassification.Informational, "There is no user connected to the service. Please use command BRCONNECT to start your session.", "", false);
        public static ErrorDefinition ServiceUnavailableOrInvalidCredentials = new ErrorDefinition("CAD.US.102", "Service unavailable or invalid credentials", ErrorDefinitionClassification.Critical, "An error occurred trying to connect with the remote repository server. Before contact your System Administrator please try to execute the command BRCONNECT again and verify you are successfully connected to the service.", "", false);
        public static ErrorDefinition InvalidCredentials = new ErrorDefinition("CAD.US.103", "Invalid credentials", ErrorDefinitionClassification.Informational, "Username and/or password are incorrect.", "", false);
        public static ErrorDefinition NoCostumerAssociation = new ErrorDefinition("SYS.101", "No costumer association", ErrorDefinitionClassification.Warning, "This user account is not associated to any costumer yet. Please contact your System Administrator.", "User account needs to be associated with costumer in database BRMaster, table CostumerUsers.", false);
        public static ErrorDefinition UserNotConnected = new ErrorDefinition("CAD.US.104", "No user connected", ErrorDefinitionClassification.Warning, "No user is connected to BomRepo services. Please execute command BRCONNECT.", "", false);
        public static ErrorDefinition ProjectNotSelected = new ErrorDefinition("CAD.US.105", "No project selected", ErrorDefinitionClassification.Warning, "No project has been selected. Please execute command BRSELECTPROJECT.", "", false);
        public static ErrorDefinition ProjectDoesNotExist = new ErrorDefinition("REST.API.102", "Project does not exist", ErrorDefinitionClassification.Warning, "Project does not exist.", "Project with id ProjectId or number ProjectNumber was not found in database.", true);
        public static ErrorDefinition CostumerDoesNotExist = new ErrorDefinition("REST.API.103", "Costumer does not exist", ErrorDefinitionClassification.Warning, "Costumer does not exist.", "Costumer number was not found in database.", true);
        public static ErrorDefinition UserDoesNotExist = new ErrorDefinition("REST.API.104", "User does not exist", ErrorDefinitionClassification.Warning, "User does not exist.", "User was not found in database.", true);
        public static ErrorDefinition EntityDoesNotExist = new ErrorDefinition("REST.API.105", "Entity does not exist", ErrorDefinitionClassification.Warning, "Entity does not exist.", "General Entity was not found in database.", true);
        public static ErrorDefinition ValidationFailed = new ErrorDefinition("BRXXXXXX.101", "Validation fail", ErrorDefinitionClassification.Critical, "Validation fail", string.Empty, true); //User this definition to return validation errors in all BRXXXXXX managers.
        public static ErrorDefinition CreateFrom(ErrorDefinition Definition, string DeveloperDescription) {
            return new ErrorDefinition(Definition.Code, Definition.Title, Definition.Classification, Definition.UserDescription, DeveloperDescription, Definition.OnlyForDeveloperEye);
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
        public ErrorDefinition(string Code, string Title, ErrorDefinitionClassification Classification, string UserDescription, string DeveloperDescription, bool OnlyForDeveloperEye)
        {
            this.Code = Code;
            this.Title = Title;
            this.Classification = Classification;
            this.UserDescription = UserDescription;
            this.DeveloperDescription = DeveloperDescription;
            this.OnlyForDeveloperEye = OnlyForDeveloperEye;
        }
        public string GetUserDescription()
        {
            return "Error " + Code + " " + Title + ": " + UserDescription + "\n\r";
        }
    }
}
