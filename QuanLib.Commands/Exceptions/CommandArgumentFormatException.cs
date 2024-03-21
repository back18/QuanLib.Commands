using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Exceptions
{
    public class CommandArgumentFormatException : CommandArgumentException
    {
        public CommandArgumentFormatException() : base() { }

        public CommandArgumentFormatException(string? message) : base(message) { }

        public CommandArgumentFormatException(string? message, Exception innerException) : base(message, innerException) { }

        public CommandArgumentFormatException(CommandArgument? argument, object? value) : base(argument, value) { }

        public CommandArgumentFormatException(CommandArgument? argument, object? value, string? message) : base(argument, value, message) { }

        public CommandArgumentFormatException(CommandArgument? argument, object? value, string? message, Exception innerException) : base(argument, value, message, innerException) { }

        protected override string DefaultMessage => "命令参数解析失败";
    }
}
