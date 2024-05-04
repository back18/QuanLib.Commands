using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Commands.Words
{
    public abstract class Word
    {
        protected Word(string text, int startIndex)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));
            ThrowHelper.ArgumentOutOfMin(0, startIndex, nameof(startIndex));

            Text = text;
            StartIndex = startIndex;
            AllowedTexts = ReadOnlyCollection<string>.Empty;
        }

        public ReadOnlyCollection<string> AllowedTexts { get; internal set; }

        public string Text { get; }

        public int StartIndex { get; }

        public int EndIndex => StartIndex + Text.Length;

        public virtual string UnescapedText => Unescape(Text);

        public virtual bool IsErrorWord => false;

        public Word? Previous { get; internal set; }

        public Word? Next { get; internal set; }

        public static string Unescape(string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            if (text.Length >= 2 && text.StartsWith('"') && text.EndsWith('"'))
                text = text[1..^1];

            try
            {
                return Regex.Unescape(text);
            }
            catch
            {
                return text;
            }
        }

        public string[] GetPreSelectedTexts()
        {
            return AllowedTexts.Where(w => w != Text && w.StartsWith(Text, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
