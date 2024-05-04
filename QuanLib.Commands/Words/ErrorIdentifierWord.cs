using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Words
{
    public class ErrorIdentifierWord : Word, IErrorWord
    {
        public ErrorIdentifierWord(string text, int startIndex) : base(text, startIndex) { }

        public Exception? Exception { get; internal set; }

        public override bool IsErrorWord => true;
    }
}
