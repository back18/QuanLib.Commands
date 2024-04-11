using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class Command
    {
        public Command(CommandIdentifier identifier, ICommandFunction commandFunction, PrivilegeLevel privilegeLevel)
        {
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));
            ArgumentNullException.ThrowIfNull(commandFunction, nameof(commandFunction));

            Identifier = identifier;
            CommandFunction = commandFunction;
            PrivilegeLevel = privilegeLevel;
        }

        public CommandIdentifier Identifier { get; }

        public ICommandFunction CommandFunction { get; }

        public PrivilegeLevel PrivilegeLevel { get; }

        public override string ToString()
        {
            return Identifier.ToString();
        }
    }
}
