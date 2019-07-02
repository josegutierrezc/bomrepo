using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API.Models
{
    public class Property
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsString { get; set; }
        public bool IsDouble { get; set; }
        public bool IsInteger { get; set; }
        public bool IsDateTime { get; set; }
        public bool IsBoolean { get; set; }
        public void SetPropertyType(int PropertyId)
        {
            IsString = false;
            IsInteger = false;
            IsDouble = false;
            IsBoolean = false;
            IsDateTime = false;
            if (PropertyId == PropertyValueTypes.String.Id) IsString = true;
            else if (PropertyId == PropertyValueTypes.Integer.Id) IsInteger = true;
            else if (PropertyId == PropertyValueTypes.Decimal.Id) IsDouble = true;
            else if (PropertyId == PropertyValueTypes.Boolean.Id) IsBoolean = true;
            else IsDateTime = true;
        }
    }
}
