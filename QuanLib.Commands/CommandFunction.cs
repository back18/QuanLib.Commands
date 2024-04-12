using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandFunction : ICommandFunction
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
            object[] arguments = CommandFunctionHelper.ParseArguments(this, args);

            try
            {
                return _methodInfo.Invoke(_owner, arguments);
            }
            catch (Exception ex)
            {
                throw new AggregateException("命令执行过程中引发了异常", ex);
            }
        }
    }
}
