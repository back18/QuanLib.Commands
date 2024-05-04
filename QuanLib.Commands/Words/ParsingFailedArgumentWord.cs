using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Words
{
    public class ParsingFailedArgumentWord : ArgumentWord, IErrorWord
    {
        public ParsingFailedArgumentWord(string text, int startIndex, CommandArgument argument, object? value) : base(text, startIndex, argument, value) { }

        public Exception? Exception
        {
            get
            {
                try
                {
                    _ = Argument.Parser.Parse(UnescapedText, null);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }

        public override bool IsErrorWord => true;
    }
}
