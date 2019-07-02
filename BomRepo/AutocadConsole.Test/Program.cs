using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomRepo.Autocad.API;
using BomRepo.Autocad.API.Models;

namespace BomRepo.AutocadConsole.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BomRepo Autocad Console Test");

            CadCommands commands = new CadCommands("0000", "0000", true);
            bool finalize = false;
            while (!finalize) {
                Console.Write("Command > ");
                string command = Console.ReadLine();
                command = command.ToLower();

                if (command == "exit") return;
                if (command == "brgetprojects") commands.ShowAllProjects();
                else if (command == "brsetdefaultproject") commands.SetDefaultProject();
                else if (command == "brgetdefaultproject") commands.GetDefaultProject();
                else if (command == "brgetdefinitions") commands.GetPartDefinitions();
                else if (command == "brgetproperties") commands.GetProperties();
                else if (command == "braddproperty") commands.AddProperty();
                else if (command == "brremoveproperty") commands.RemoveProperty();
                else if (command == "brassignproperty") commands.AssignProperty();
                else Console.WriteLine("No recognized command");
            }
        }
    }
}
