using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Commands.Words;

namespace QuanLib.Commands
{
    public class WordCollection : IReadOnlyList<Word>
    {
        public WordCollection(IList<Word> words)
        {
            ArgumentNullException.ThrowIfNull(words, nameof(words));

            _items = words.AsReadOnly();
        }

        private readonly ReadOnlyCollection<Word> _items;

        public Word this[int index] => _items[index];

        public int Count => _items.Count;

        public int GetFirstArgumentWordIndex()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] is ArgumentWord)
                    return i;
            }

            return -1;
        }

        public ArgumentWord? GetFirstArgumentWord()
        {
            int index = GetFirstArgumentWordIndex();
            if (index != -1)
                return _items[index] as ArgumentWord;
            else
                return null;
        }

        public int IndexOf(Word word)
        {
            return _items.IndexOf(word);
        }

        public int TextIndex2WordIndex(int textIndex)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (textIndex >= _items[i].StartIndex)
                    return i;
            }

            return -1;
        }

        public T[] GetWords<T>() where T : Word
        {
            List<T> result = [];
            foreach(Word word in _items)
            {
                if (word is T t)
                    result.Add(t);
            }
            return result.ToArray();
        }

        public string GetIdentifier()
        {
            return string.Join(' ', GetWords<IdentifierWord>().Select(s => s.Text));
        }

        public string[] GetArgumentTexts()
        {
            List<string> args = [];
            Word? word = GetFirstArgumentWord();

            while (true)
            {
                if (word is null)
                    break;
                args.Add(word.UnescapedText);
                word = word.Next;
            }

            return args.ToArray();
        }

        public IEnumerator<Word> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
