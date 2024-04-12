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
                throw new ArgumentException($"命令预期参数数量为{commandFunction.Arguments.Count}，实际参数数量为{args.Length}");

            object[] result = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    result[i] = commandFunction.Arguments[i].Parser.Parse(args[i], null);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"无法将命令参数“{args[i]}”解析为“{commandFunction.Arguments[i].Type}”类型", ex);
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
