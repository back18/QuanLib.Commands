using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public interface ICommandFunction
    {
        public bool IsStatic { get; }

        public object? Execute(params string[] args);

        public ReadOnlyCollection<CommandArgument> Arguments { get; }
    }
}
