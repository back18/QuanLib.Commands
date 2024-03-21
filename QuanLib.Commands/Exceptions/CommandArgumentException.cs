using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Exceptions
{
    public class CommandArgumentException : CommandException
    {
        public CommandArgumentException() : base() { }

        public CommandArgumentException(string? message) : base(message) { }

        public CommandArgumentException(string? message, Exception innerException) : base(message, innerException) { }

        public CommandArgumentException(CommandArgument? argument, object? value) : base()
        {
            Argument = argument;
            Value = value;
        }

        public CommandArgumentException(CommandArgument? argument, object? value, string? message) : base(message)
        {
            Argument = argument;
            Value = value;
        }

        public CommandArgumentException(CommandArgument? argument, object? value, string? message, Exception innerException) : base(message, innerException)
        {
            Argument = argument;
            Value = value;
        }

        public CommandArgument? Argument { get; }

        public object? Value { get; }

        protected override string DefaultMessage => "命令参数不是预期的";

        public override string Message
        {
            get
            {
                if (Argument is not null)
                {
                    string text = $"Name: {Argument.Name}, Index: {Argument.Index}";
                    if (Value is not null)
                        text += $", Value: {Value}";

                    return base.Message + $"（{text}）";
                }
                else
                {
                    return base.Message;
                }
            }
        }
    }
}
