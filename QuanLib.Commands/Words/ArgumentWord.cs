using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Words
{
    public class ArgumentWord : Word
    {
        public ArgumentWord(string text, int startIndex, CommandArgument argument, object? value) : base(text, startIndex)
        {
            ArgumentNullException.ThrowIfNull(argument, nameof(argument));

            Argument = argument;
            Value = value;
        }

        public CommandArgument Argument { get; }

        public object? Value { get; }
    }
}
