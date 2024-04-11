using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandManager : IReadOnlyDictionary<string, Command>
    {
        public CommandManager()
        {
            RootNode = new("__root__");
            _items = [];
        }

        private readonly Dictionary<string, Command> _items;

        public Command this[string key] => _items[key];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<Command> Values => _items.Values;

        public int Count => _items.Count;

        public IdentifierNode RootNode { get; }

        public void Register(Command command)
        {
            ArgumentNullException.ThrowIfNull(command, nameof(command));

            _items.Add(command.Identifier.ToString(), command);
            RootNode.AddChildNode(command.Identifier);
        }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out Command value)
        {
            return _items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, Command>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
