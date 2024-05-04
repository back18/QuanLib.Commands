using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Words
{
    public interface IErrorWord
    {
        public Exception? Exception { get; }
    }
}
