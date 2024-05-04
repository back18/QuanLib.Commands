using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public struct Scanner
    {
        public Scanner()
        {
            Text = string.Empty;
            Position = -1;
        }

        public Scanner(string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            Text = text;
            Position = -1;
        }

        public string Text { get; }

        public int Position { get; private set; }

        public readonly char Current => Text[Position];

        public readonly bool IsEndOfText => Position >= Text.Length;

        public readonly char PeekNext()
        {
            return Text[Position + 1];
        }

        public readonly bool TryPeekNext(out char result)
        {
            if (Position + 1 < Text.Length)
            {
                result = Text[Position + 1];
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public readonly char PeekPrevious()
        {
            return Text[Position - 1];
        }

        public readonly bool TryPeekPrevious(out char result)
        {
            if (Position - 1 >= 0)
            {
                result = Text[Position - 1];
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public bool MoveNext()
        {
            Position++;
            if (Position < Text.Length)
                return true;
            else
                return false;
        }

        public void Reset()
        {
            Position = -1;
        }

        public readonly FormatException CreateFormatException(string message)
        {
            return new FormatException($"在索引{Position}处的语法错误：{message}");
        }
    }
}
