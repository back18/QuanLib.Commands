using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class IdentifierNode
    {
        public IdentifierNode(string identifier)
        {
            ArgumentException.ThrowIfNullOrEmpty(identifier, nameof(identifier));

            Identifier = identifier;
            _childNodes = [];
        }

        private readonly Dictionary<string, IdentifierNode> _childNodes;

        public string Identifier { get; }

        public int Index { get; }

        public IdentifierNode? ParentNode { get; private set; }

        public IdentifierNode[] GetChildNodes()
        {
            return _childNodes.Values.ToArray();
        }

        public void AddChildNode(CommandIdentifier commandIdentifier)
        {
            ArgumentNullException.ThrowIfNull(commandIdentifier, nameof(commandIdentifier));

            IdentifierNode identifierNode = this;
            foreach (string identifier in commandIdentifier.Identifiers)
            {
                if (!identifierNode.ContainsChildNode(identifier))
                    identifierNode.AddChildNode(identifier);

                identifierNode = identifierNode._childNodes[identifier];
            }
        }

        public void AddChildNode(string identifier)
        {
            IdentifierNode identifierNode = new(identifier)
            {
                ParentNode = this
            };

            _childNodes.Add(identifier, identifierNode);
        }

        public bool RemoveChildNode(string identifier)
        {
            if (_childNodes.TryGetValue(identifier, out var identifierNode) && _childNodes.Remove(identifier))
            {
                identifierNode.ParentNode = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsChildNode(string identifier)
        {
            return _childNodes.ContainsKey(identifier);
        }

        public bool TryGetChildNode(string identifier, [MaybeNullWhen(false)] out IdentifierNode result)
        {
            return _childNodes.TryGetValue(identifier, out result);
        }

        public string ToString(char separator)
        {
            return ParentNode is not null ? ParentNode.ToString() + separator + Identifier : Identifier;
        }

        public override string ToString()
        {
            return ToString(' ');
        }
    }
}
