using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.CommandLine
{
    public class CommandReaderResult
    {
        public CommandReaderResult(Command? command, IList<string> args)
        {
            ArgumentNullException.ThrowIfNull(args, nameof(args));

            Command = command;
            Args = args.AsReadOnly();
        }

        public CommandReaderResult(Command? command) : this(command, Array.Empty<string>()) { }

        public Command? Command { get; }

        public ReadOnlyCollection<string> Args { get; }
    }
}
