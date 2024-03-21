using QuanLib.Commands.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandFunction
    {
        public CommandFunction(object? owner, MethodInfo methodInfo)
        {
            ArgumentNullException.ThrowIfNull(methodInfo, nameof(methodInfo));

            List<CommandArgument> arguments = [];
            ParameterInfo[] parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
                arguments.Add(new(parameters[i], i));

            Arguments = arguments.AsReadOnly();
            _owner = owner;
            _methodInfo = methodInfo;
        }

        private readonly object? _owner;

        private readonly MethodInfo _methodInfo;

        public bool IsStatic => _methodInfo.IsStatic;

        public ReadOnlyCollection<CommandArgument> Arguments { get; }

        public object? Execute(params string[] args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));

            if (args.Length != Arguments.Count)
                throw new CommandArgumentCountException(Arguments.Count, args.Length);

            object[] objects = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    objects[i] = Arguments[i].Parser.Parse(args[i], null);
                }
                catch (Exception ex)
                {
                    throw new CommandArgumentFormatException(Arguments[i], args[i], null, ex);
                }
            }

            try
            {
                return _methodInfo.Invoke(_owner, objects);
            }
            catch (Exception ex)
            {
                throw new CommandException(null, ex);
            }
        }
    }
}
