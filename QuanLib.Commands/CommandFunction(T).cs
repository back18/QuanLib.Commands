using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandFunction<T> : ICommandFunction where T : Delegate
    {
        public CommandFunction(T @delegate)
        {
            MethodInfo methodInfo = @delegate.GetMethodInfo();
            List<CommandArgument> arguments = [];
            ParameterInfo[] parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
                arguments.Add(new(parameters[i], i));

            IsStatic = methodInfo.IsStatic;
            Arguments = arguments.AsReadOnly();
            _delegate = @delegate;
        }

        private readonly T _delegate;

        public bool IsStatic { get; }

        public ReadOnlyCollection<CommandArgument> Arguments { get; }

        public object? Execute(params string[] args)
        {
            object[] arguments = CommandFunctionHelper.ParseArguments(this, args);

            try
            {
                return _delegate.DynamicInvoke(arguments);
            }
            catch (TargetInvocationException targetInvocationException) when (targetInvocationException.InnerException is not null)
            {
                throw new AggregateException("命令执行过程中引发了异常", targetInvocationException.InnerException);
            }
            catch (Exception ex)
            {
                throw new AggregateException("命令执行过程中引发了异常", ex);
            }
        }
    }
}
