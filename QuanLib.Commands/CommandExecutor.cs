using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandExecutor
    {
        public CommandExecutor(CommandManager commandManager)
        {
            ArgumentNullException.ThrowIfNull(commandManager, nameof(commandManager));

            CommandManager = commandManager;
        }

        public CommandManager CommandManager { get; }
    }
}
