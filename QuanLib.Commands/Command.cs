using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class Command
    {
        public Command(CommandIdentifier identifier, ICommandFunction commandFunction, PrivilegeLevel privilegeLevel, Func<object?, string>? formatMessageHandler = null)
        {
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));
            ArgumentNullException.ThrowIfNull(commandFunction, nameof(commandFunction));

            Identifier = identifier;
            CommandFunction = commandFunction;
            PrivilegeLevel = privilegeLevel;
            _formatMessage = formatMessageHandler ?? FormatMessage;
        }

        private readonly Func<object?, string> _formatMessage;

        public CommandIdentifier Identifier { get; }

        public ICommandFunction CommandFunction { get; }

        public PrivilegeLevel PrivilegeLevel { get; }

        public string Execute(CommandSender commandSender, params string[] args)
        {
            ArgumentNullException.ThrowIfNull(commandSender, nameof(commandSender));
            ArgumentNullException.ThrowIfNull(args, nameof(args));

            if (commandSender.PrivilegeLevel > PrivilegeLevel)
                throw new UnauthorizedAccessException("命令发送人的权限等级低于命令要求的权限等级");

            return _formatMessage.Invoke(CommandFunction.Execute(args));
        }

        public override string ToString()
        {
            return Identifier.ToString();
        }

        private static string FormatMessage(object? obj)
        {
            return obj?.ToString() ?? string.Empty;
        }
    }
}
