using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandBuilder
    {
        private CommandIdentifier? _identifier = null;
        private ICommandFunction? _commandFunction = null;
        private PrivilegeLevel _privilegeLevel = PrivilegeLevel.User;
        private Func<object?, string>? _formatMessageHandler = null;

        public CommandBuilder On(string identifiers)
        {
            _identifier = new(identifiers);
            return this;
        }

        public CommandBuilder On(string[] identifiers)
        {
            _identifier = new(identifiers);
            return this;
        }

        public CommandBuilder On(CommandIdentifier identifier)
        {
            ArgumentNullException.ThrowIfNull(identifier, nameof(identifier));

            _identifier = identifier;
            return this;
        }

        public CommandBuilder Execute<T>(T @delegate) where T : Delegate
        {
            _commandFunction = new CommandFunction<T>(@delegate);
            return this;
        }

        public CommandBuilder Execute(object? owner, MethodInfo methodInfo)
        {
            _commandFunction = new CommandFunction(owner, methodInfo);
            return this;
        }

        public CommandBuilder Execute(ICommandFunction commandFunction)
        {
            ArgumentNullException.ThrowIfNull(_commandFunction, nameof(commandFunction));

            _commandFunction = commandFunction;
            return this;
        }

        public CommandBuilder Allow(PrivilegeLevel privilegeLevel)
        {
            _privilegeLevel = privilegeLevel;
            return this;
        }

        public CommandBuilder SetFormatMessageHandler(Func<object?, string>? formatMessageHandler)
        {
            _formatMessageHandler = formatMessageHandler;
            return this;
        }

        public Command Build()
        {
            ArgumentNullException.ThrowIfNull(_identifier, nameof(_identifier));
            ArgumentNullException.ThrowIfNull(_commandFunction, nameof(_commandFunction));

            return new(_identifier, _commandFunction, _privilegeLevel, _formatMessageHandler);
        }
    }
}
