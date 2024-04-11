using QuanLib.Commands.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public static class CommandFunctionHelper
    {
        public static object[] ParseArguments(ICommandFunction commandFunction, params string[] args)
        {
            if (args.Length != commandFunction.Arguments.Count)
                throw new CommandArgumentCountException(commandFunction.Arguments.Count, args.Length);

            object[] result = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    result[i] = commandFunction.Arguments[i].Parser.Parse(args[i], null);
                }
                catch (Exception ex)
                {
                    throw new CommandArgumentFormatException(commandFunction.Arguments[i], args[i], null, ex);
                }
            }

            return result;
        }

        public static ICommandFunction FromDelegat<T>(T @delegate) where T : Delegate
        {
            return new CommandFunction<T>(@delegate);
        }

        public static ICommandFunction FromMethodInfo(object? owner, MethodInfo methodInfo)
        {
            return new CommandFunction(owner, methodInfo);
        }
    }
}
