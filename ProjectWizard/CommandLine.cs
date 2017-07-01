using System;

namespace QuantConnect.ProjectWizard
{
    internal class CommandLineAttribute : Attribute
    {
        public string Action { get; }
        public string[] Parameters { get; }

        public CommandLineAttribute(string action, params string[] parameters)
        {
            Action = action;
            Parameters = parameters ?? new string[0];
        }

        public string ParameterHelp
        {
            get
            {
                var help = "";
                foreach (var parameter in this.Parameters)
                {
                    help += $"[{parameter}] ";
                }

                return help;
            }
        }
    }
}