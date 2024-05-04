using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Words
{
    public class IdentifierWord : Word
    {
        public IdentifierWord(string text, int startIndex, IdentifierNode identifierNode) : base(text, startIndex)
        {
            ArgumentNullException.ThrowIfNull(identifierNode, nameof(identifierNode));

            IdentifierNode = identifierNode;
        }

        public IdentifierNode IdentifierNode { get; }
    }
}
