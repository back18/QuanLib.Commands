using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Exceptions
{
    public class CommandException : ExceptionBase
    {
        public CommandException() : base() { }

        public CommandException(string? message) : base(message) { }

        public CommandException(string? message, Exception innerException) : base(message, innerException) { }

        protected override string DefaultMessage => "命令执行过程中引发了异常";
    }
}
