using System;
using System.Collections.Generic;
using System.Linq;
using infrastructurizr.Commands;
using infrastructurizr.Dependencies;

namespace infrastructurizr
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = FindCommand(args);
            if (command == null)
            {
                PrintHelp();
                Environment.Exit(1);
            }
            args = args.Skip(command.Name.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length).ToArray();

            if (args.Any(IsHelp))
            {
                PrintHelp(command);
                Environment.Exit(0);
            }
            ParseParameters(command, args);
            if (!command.Parameters.All(p => p.IsSatisfied()))
            {
                PrintHelp(command);
                Environment.Exit(1);
            }

            CheckDependencies();
            command.Execute();

#if DEBUG
            Console.ReadLine();
#endif
        }

        private static bool IsHelp(string a)
        {
            switch (a.ToLowerInvariant())
            {
                case "--help":
                case "-help":
                case "help":
                case "--h":
                case "-h":
                    return true;
                default:
                    return false;
            }
        }

        private static void ParseParameters(Command command, string[] args)
        {
            args = args.SkipWhile(a => !a.StartsWith("--")).ToArray();
            while (args.Any())
            {
                var parameterName = args.First().Substring(2);
                string parameterValue = null;
                var rest = args.Skip(1).ToArray();
                if (rest.Any())
                {
                    var values = rest.TakeWhile(a => !a.StartsWith("--")).ToArray();
                    rest = rest.Skip(values.Length).ToArray();
                    parameterValue = string.Join(" ", values);
                }

                command.SetParameter(parameterName, parameterValue);

                args = rest;
            }
        }

        private static Command FindCommand(IEnumerable<string> args)
        {
            var argString = string.Join(" ", args).ToLowerInvariant();
            if (string.IsNullOrEmpty(argString))
            {
                return null;
            }

            var commands = All<Command>().Where(c => argString.StartsWith(c.Name.ToLowerInvariant())).ToArray();
            if (commands.Length != 1)
            {
                return null;
            }
            return commands.Single();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Please provide a valid command name. Available commands are");
            foreach (var command in All<Command>())
            {
                Console.WriteLine($"- {command.Name}");
                Console.WriteLine($"  {command.Description}");
                Console.WriteLine($"  Example usage: z-azure {command.Name} [parameters]");
                Console.WriteLine($"  For more info: z-azure {command.Name} --help");
            }
        }

        private static void PrintHelp(Command command)
        {
            Console.WriteLine($"Help for \"{command.Name}\"");
            Console.WriteLine($"  Example usage: z-azure {command.Name} [parameters]");
            Console.WriteLine("Required parameters");
            PrintParametersHelp(command, p=> p.IsMandatory);
            Console.WriteLine("Optional parameters");
            PrintParametersHelp(command, p => !p.IsMandatory);
        }

        private static void PrintParametersHelp(Command command, Func<CommandParameter, bool> predicate)
        {
            foreach (var parameter in command.Parameters.Where(predicate))
            {
                Console.WriteLine($"  {parameter.Name}");
                Console.WriteLine($"    {parameter.Description}");
                Console.WriteLine($"    Type: {parameter.Type}");
            }
        }

        private static void CheckDependencies()
        {
            Console.WriteLine("### Checking dependencies");
            if (All<Dependency>().OrderBy(d => d.Priority).Any(d => !d.Assert()))
            {
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        private static IEnumerable<T> All<T>()
        {
            return typeof(Program).Assembly
                .GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<T>();
        }
    }


}
