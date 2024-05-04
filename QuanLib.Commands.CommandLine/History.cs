using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.CommandLine
{
    public class History
    {
        public History()
        {
            _items = [];
        }

        private readonly List<string> _items;

        public int Count => _items.Count;

        public int Position { get; private set; }

        public string Current => _items[Position];

        public bool IsReadToStart => Position == 0;

        public bool IsReadToEnd => Position == _items.Count - 1;

        public void Previous()
        {
            Position = Math.Clamp(Position - 1, 0, _items.Count - 1);
        }

        public void Next()
        {
            Position = Math.Clamp(Position + 1, 0, _items.Count - 1);
        }

        public void Add(string item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            if (_items.Count > 0 && item == _items[^1])
                return;

            _items.Remove(item);

            if (_items.Count > 0 && _items[^1] == string.Empty)
                _items[^1] = item;
            else
                _items.Add(item);

            Position = _items.Count - 1;
        }
    }
}
