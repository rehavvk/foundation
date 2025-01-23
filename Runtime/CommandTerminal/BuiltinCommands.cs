namespace Rehawk.Foundation.CommandTerminal
{
    public static class BuiltinCommands
    {
        [RegisterCommand(Help = "Clear the command console", MaxArgCount = 0)]
        private static void CommandClear(CommandArg[] args)
        {
            Terminal.Buffer.Clear();
        }

        [RegisterCommand(Help = "Display help information about a command", MaxArgCount = 1)]
        private static void CommandHelp(CommandArg[] args)
        {
            if (args.Length == 0)
            {
                foreach (var command in Terminal.Shell.Commands)
                {
                    Terminal.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
                }

                return;
            }

            string commandName = args[0].String.ToLower();

            if (!Terminal.Shell.Commands.ContainsKey(commandName))
            {
                Terminal.Shell.IssueErrorMessage("Command {0} could not be found.", commandName);
                return;
            }

            var info = Terminal.Shell.Commands[commandName];

            if (info.help == null)
            {
                Terminal.Log("{0} does not provide any help documentation.", commandName);
            }
            else if (info.hint == null)
            {
                Terminal.Log(info.help);
            }
            else
            {
                Terminal.Log("{0}\nUsage: {1}", info.help, info.hint);
            }
        }

        [RegisterCommand(Help = "Quit running application", MaxArgCount = 0)]
        private static void CommandQuit(CommandArg[] args)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}