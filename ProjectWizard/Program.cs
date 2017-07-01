using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace QuantConnect.ProjectWizard
{
    using Api;

    internal class Program
    {
        public static void Main(string[] args)
        {
            var configFile = File.Exists("secure.json") ? "secure.json" : "../../secure.json";
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
            var api = new Api();
            api.Initialize(config.UserId, config.AccessCode, "../../../Data"); // todo: figure out the right way to do this

            if (!args.Any())
            {
                Console.WriteLine("Usage: ");
            }

            var actions = new Actions(new ApiHelper(api));
            
            foreach (var method in typeof(Actions).GetTypeInfo().DeclaredMethods)
            {
                var commandLine = method.GetCustomAttribute<CommandLineAttribute>();
                if (commandLine == null) continue;

                if (args.Any())
                {
                    if (args[0] != commandLine.Action) continue;
                    
                    if (args.Length - 1 == commandLine.Parameters.Length)
                    {
                        method.Invoke(actions, args.Skip(1).Cast<object>().ToArray());
                    }
                    else
                    {
                        PrintUsage(commandLine);
                    }
                }
                else
                {
                    PrintUsage(commandLine);
                }
            }
        }

        private static void PrintUsage(CommandLineAttribute commandLine)
        {
            Console.WriteLine($"{commandLine.Action}: {commandLine.ParameterHelp}");
        }
    }
}