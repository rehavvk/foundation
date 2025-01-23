using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Foundation.CommandTerminal
{
    public struct CommandInfo
    {
        public Action<CommandArg[]> proc;
        public int maxArgCount;
        public int minArgCount;
        public string help;
        public string hint;
    }

    public struct CommandArg
    {
        public string String { get; set; }

        public int Int
        {
            get
            {
                int intValue;

                if (int.TryParse(String, out intValue))
                {
                    return intValue;
                }

                TypeError("int");
                return 0;
            }
        }

        public float Float
        {
            get
            {
                float floatValue;

                if (float.TryParse(String, out floatValue))
                {
                    return floatValue;
                }

                TypeError("float");
                return 0;
            }
        }

        public bool Bool
        {
            get
            {
                if (string.Compare(String, "TRUE", ignoreCase: true) == 0)
                {
                    return true;
                }

                if (string.Compare(String, "FALSE", ignoreCase: true) == 0)
                {
                    return false;
                }

                TypeError("bool");
                return false;
            }
        }

        public override string ToString()
        {
            return String;
        }

        void TypeError(string expectedType)
        {
            Terminal.Shell.IssueErrorMessage(
                "Incorrect type for {0}, expected <{1}>",
                String, expectedType
            );
        }
    }

    public class CommandShell
    {
        private Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo>();
        private List<CommandArg> arguments = new List<CommandArg>(); // Cache for performance

        public string IssuedErrorMessage { get; private set; }

        public Dictionary<string, CommandInfo> Commands
        {
            get { return commands; }
        }

        /// <summary>
        /// Uses reflection to find all RegisterCommand attributes
        /// and adds them to the commands dictionary.
        /// </summary>
        public void RegisterCommands()
        {
            var rejectedCommands = new Dictionary<string, CommandInfo>();
            var methodFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(methodFlags))
                {
                    var attribute = Attribute.GetCustomAttribute(
                        method, typeof(RegisterCommandAttribute)) as RegisterCommandAttribute;

                    if (attribute == null)
                    {
                        if (method.Name.StartsWith("FRONTCOMMAND", StringComparison.CurrentCultureIgnoreCase))
                        {
                            // Front-end Command methods don't implement RegisterCommand, use default attribute
                            attribute = new RegisterCommandAttribute();
                        }
                        else
                        {
                            continue;
                        }
                    }

                    ParameterInfo[] methodsParams = method.GetParameters();

                    string commandName = InferFrontCommandName(method.Name);
                    Action<CommandArg[]> proc;

                    if (attribute.Name == null)
                    {
                        // Use the method's name as the command's name
                        commandName = InferCommandName(commandName == null ? method.Name : commandName);
                    }
                    else
                    {
                        commandName = attribute.Name;
                    }

                    if (methodsParams.Length != 1 || methodsParams[0].ParameterType != typeof(CommandArg[]))
                    {
                        // Method does not match expected Action signature,
                        // this could be a command that has a FrontCommand method to handle its arguments.
                        rejectedCommands.Add(commandName.ToLower(), CommandFromParamInfo(methodsParams, attribute.Help));
                        continue;
                    }

                    // Convert MethodInfo to Action.
                    // This is essentially allows us to store a reference to the method,
                    // which makes calling the method significantly more performant than using MethodInfo.Invoke().
                    proc = (Action<CommandArg[]>) Delegate.CreateDelegate(typeof(Action<CommandArg[]>), method);
                    AddCommand(commandName, proc, attribute.MinArgCount, attribute.MaxArgCount, attribute.Help, attribute.Hint);
                }
            }

            HandleRejectedCommands(rejectedCommands);
        }

        /// <summary>
        /// Parses an input line into a command and runs that command.
        /// </summary>
        public void RunCommand(string line)
        {
            string remaining = line;
            IssuedErrorMessage = null;
            arguments.Clear();

            while (remaining != "")
            {
                CommandArg argument = EatArgument(ref remaining);

                if (argument.String != "")
                {
                    arguments.Add(argument);
                }
            }

            if (arguments.Count == 0)
            {
                // Nothing to run
                return;
            }

            string commandName = arguments[0].String.ToLower();
            arguments.RemoveAt(0); // Remove command name from arguments

            if (!commands.ContainsKey(commandName))
            {
                IssueErrorMessage("Command {0} could not be found", commandName);
                return;
            }

            RunCommand(commandName, arguments.ToArray());
        }

        public void RunCommand(string commandName, CommandArg[] arguments)
        {
            var command = commands[commandName];
            int argCount = arguments.Length;
            string errorMessage = null;
            int requiredArg = 0;

            if (argCount < command.minArgCount)
            {
                if (command.minArgCount == command.maxArgCount)
                {
                    errorMessage = "exactly";
                }
                else
                {
                    errorMessage = "at least";
                }

                requiredArg = command.minArgCount;
            }
            else if (command.maxArgCount > -1 && argCount > command.maxArgCount)
            {
                // Do not check max allowed number of arguments if it is -1
                if (command.minArgCount == command.maxArgCount)
                {
                    errorMessage = "exactly";
                }
                else
                {
                    errorMessage = "at most";
                }

                requiredArg = command.maxArgCount;
            }

            if (errorMessage != null)
            {
                string pluralFix = requiredArg == 1 ? "" : "s";

                IssueErrorMessage(
                    "{0} requires {1} {2} argument{3}",
                    commandName,
                    errorMessage,
                    requiredArg,
                    pluralFix
                );

                if (command.hint != null)
                {
                    IssuedErrorMessage += $"\n    -> Usage: {command.hint}";
                }

                return;
            }

            command.proc(arguments);
        }

        public void AddCommand(string name, CommandInfo info)
        {
            name = name.ToLower();

            if (commands.ContainsKey(name))
            {
                IssueErrorMessage("Command {0} is already defined.", name);
                return;
            }

            commands.Add(name, info);
        }

        public void AddCommand(string name, Action<CommandArg[]> proc, int minArgs = 0, int maxArgs = -1, string help = "", string hint = null)
        {
            var info = new CommandInfo()
            {
                proc = proc,
                minArgCount = minArgs,
                maxArgCount = maxArgs,
                help = help,
                hint = hint
            };

            AddCommand(name, info);
        }

        public void IssueErrorMessage(string format, params object[] message)
        {
            IssuedErrorMessage = string.Format(format, message);
        }

        private string InferCommandName(string methodName)
        {
            string commandName;
            int index = methodName.IndexOf("COMMAND", StringComparison.CurrentCultureIgnoreCase);

            if (index >= 0)
            {
                // Method is prefixed, suffixed with, or contains "COMMAND".
                commandName = methodName.Remove(index, 7);
            }
            else
            {
                commandName = methodName;
            }

            return commandName;
        }

        private string InferFrontCommandName(string methodName)
        {
            int index = methodName.IndexOf("FRONT", StringComparison.CurrentCultureIgnoreCase);
            return index >= 0 ? methodName.Remove(index, 5) : null;
        }

        private void HandleRejectedCommands(Dictionary<string, CommandInfo> rejectedCommands)
        {
            foreach (var command in rejectedCommands)
            {
                if (commands.ContainsKey(command.Key))
                {
                    commands[command.Key] = new CommandInfo()
                    {
                        proc = commands[command.Key].proc,
                        minArgCount = command.Value.minArgCount,
                        maxArgCount = command.Value.maxArgCount,
                        help = command.Value.help
                    };
                }
                else
                {
                    IssueErrorMessage("{0} is missing a front command.", command);
                }
            }
        }

        private CommandInfo CommandFromParamInfo(ParameterInfo[] parameters, string help)
        {
            int optionalArgs = 0;

            foreach (var param in parameters)
            {
                if (param.IsOptional)
                {
                    optionalArgs += 1;
                }
            }

            return new CommandInfo()
            {
                proc = null,
                minArgCount = parameters.Length - optionalArgs,
                maxArgCount = parameters.Length,
                help = help
            };
        }

        private CommandArg EatArgument(ref string s)
        {
            var arg = new CommandArg();
            int spaceIndex = s.IndexOf(' ');

            if (spaceIndex >= 0)
            {
                arg.String = s.Substring(0, spaceIndex);
                s = s.Substring(spaceIndex + 1); // Remaining
            }
            else
            {
                arg.String = s;
                s = "";
            }

            return arg;
        }
    }
}