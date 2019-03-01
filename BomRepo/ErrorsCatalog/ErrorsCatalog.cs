using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.ErrorsCatalog
{
    public static class ErrorCatalog
    {
        public static ErrorDefinition NullPartName = new ErrorDefinition("CAD.PN.100", "Invalid part name", "Part name cannot be null or empty.", "");
        public static ErrorDefinition InvalidCharactersInPartName = new ErrorDefinition("CAD.PN.101", "Invalid part name", "Part name can only contain letters, numbers and one dash '-' symbol.", "");
        public static ErrorDefinition OnlyDashWasFoundInPartName = new ErrorDefinition("CAD.PN.102", "Invalid part name", "Part name cannot only contain the dash '-' symbol.", "");
        public static ErrorDefinition MoreThanOnlyDashWasFoundInPartName = new ErrorDefinition("CAD.PN.103", "Invalid part name", "More than one dash '-' symbol was found in Part name.", "");
        public static ErrorDefinition MissingCloseParenthesisInPartReferenceName = new ErrorDefinition("CAD.PR.101", "Invalid part reference name", "Missing close parenthesis ')' symbol.", "");
        public static ErrorDefinition MissingOpenParenthesisInPartReferenceName = new ErrorDefinition("CAD.PR.102", "Invalid part reference name", "Missing open parenthesis '(' symbol.", "");
        public static ErrorDefinition WrongOpenParenthesisLocationInPartReferenceName = new ErrorDefinition("CAD.PR.103", "Invalid part reference name", "Open parenthesis '(' symbol is always the first character in the part reference name when more than one (1) quantity is required.", "");
        public static ErrorDefinition IntegerNumberRequiredInQuantity = new ErrorDefinition("CAD.PR.104", "Integer number required", "An integer number is required between open and close parenthesis.", "");
        public static ErrorDefinition BomRepoServiceUnavailable = new ErrorDefinition("REST.API.101", "Service unavailable", "An error occurred trying to connect with the remote repository server. Please contact your System Administrator.", "");
        public static ErrorDefinition MissingCredentials = new ErrorDefinition("CAD.US.101", "User logged off", "There is no user connected to the service. Please use command BRCONNECT to start your session.", "");
        public static ErrorDefinition ServiceUnavailableOrInvalidCredentials = new ErrorDefinition("CAD.US.102", "Service unavailable or invalid user credentials", "An error occurred trying to connect with the remote repository server. Before contact your System Administrator please try to execute the command BRCONNECT again and verify you are successfully connected to the service.", "");
        public static ErrorDefinition InvalidCredentials = new ErrorDefinition("CAD.US.103", "Invalid username and/or password", "Username and/or password are incorrect.", "");
    }

    public class ErrorDefinition
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string UserDescription { get; set; }
        public string DeveloperDescription { get; set; }
        public ErrorDefinition(string Code, string Title, string UserDescription, string DeveloperDescription)
        {
            this.Code = Code;
            this.Title = Title;
            this.UserDescription = UserDescription;
            this.DeveloperDescription = DeveloperDescription;
        }
        public string GetUserDescription()
        {
            return "Error " + Code + " " + Title + ": " + UserDescription + "\n\r";
        }
    }
}
