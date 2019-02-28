using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API
{
    public static class ErrorCatalog
    {
        public static ErrorDefinition NullPartName = new ErrorDefinition("PN100", "Invalid part name", "Part name cannot be null or empty.");
        public static ErrorDefinition InvalidCharactersInPartName = new ErrorDefinition("PN101", "Invalid part name", "Part name can only contain letters, numbers and one dash '-' symbol.");
        public static ErrorDefinition OnlyDashWasFoundInPartName = new ErrorDefinition("PN102", "Invalid part name", "Part name cannot only contain the dash '-' symbol.");
        public static ErrorDefinition MoreThanOnlyDashWasFoundInPartName = new ErrorDefinition("PN103", "Invalid part name", "More than one dash '-' symbol was found in Part name.");
        public static ErrorDefinition MissingCloseParenthesisInPartReferenceName = new ErrorDefinition("PR101", "Invalid part reference name", "Missing close parenthesis ')' symbol.");
        public static ErrorDefinition MissingOpenParenthesisInPartReferenceName = new ErrorDefinition("PR102", "Invalid part reference name", "Missing open parenthesis '(' symbol.");
        public static ErrorDefinition WrongOpenParenthesisLocationInPartReferenceName = new ErrorDefinition("PR103", "Invalid part reference name", "Open parenthesis '(' symbol is always the first character in the part reference name when more than one (1) quantity is required.");
        public static ErrorDefinition IntegerNumberRequiredInQuantity = new ErrorDefinition("PR104", "Integer number required", "An integer number is required between open and close parenthesis.");
    }

    public class ErrorDefinition
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ErrorDefinition(string Code, string Title, string Description)
        {
            this.Code = Code;
            this.Title = Title;
            this.Description = Description;
        }
        public string GetUserDescription()
        {
            return "Error " + Code + " " + Title + ": " + Description + "\n\r";
        }
    }
}
