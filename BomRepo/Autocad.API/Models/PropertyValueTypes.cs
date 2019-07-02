using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRepo.Autocad.API.Models
{
    public class PropertyValueTypes
    {
        public static PropertyValueType String = new PropertyValueType() { Id = 1, Name = "string" };
        public static PropertyValueType Integer = new PropertyValueType() { Id = 2, Name = "integer" };
        public static PropertyValueType Decimal = new PropertyValueType() { Id = 3, Name = "decimal" };
        public static PropertyValueType Boolean = new PropertyValueType() { Id = 4, Name = "boolean" };
        public static PropertyValueType Date = new PropertyValueType() { Id = 5, Name = "date" };
        public static PropertyValueType[] All = { null, String, Integer, Decimal, Boolean, Date };
        public static string AllToString() {
            string all = string.Empty;
            foreach (PropertyValueType pvt in All)
                if (pvt != null) all += pvt.Id.ToString() + ":" + pvt.Name + ",";
            if (all != string.Empty) all = all.Substring(0, all.Length - 1);
            return all;
        }
    }

    public class PropertyValueType {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
