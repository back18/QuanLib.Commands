using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public static class WordSplitter
    {
        public static string[] Split(string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            if (string.IsNullOrEmpty(text))
                return [string.Empty];

            List<string> result = [];
            Scanner scanner = new(text);
            StringBuilder stringBuilder = new();
            bool findQuotationMarks = false;
            bool requestEscape = false;

            while (scanner.MoveNext())
            {
                char c = scanner.Current;
                stringBuilder.Append(c);

                if (requestEscape)
                {
                    requestEscape = false;
                    continue;
                }

                if (c == '\\')
                {
                    requestEscape = true;
                }
                if (c == ' ')
                {
                    if (findQuotationMarks)
                        continue;

                    string word = stringBuilder.ToString().Trim(' ');
                    if (string.IsNullOrEmpty(word))
                        throw scanner.CreateFormatException("匹配到了连续的两个空格");

                    result.Add(word);
                    stringBuilder.Clear();
                    stringBuilder.Append(' ');
                }
                else if (c == '"')
                {
                    if (findQuotationMarks)
                    {
                        if (scanner.TryPeekNext(out var next) && next != ' ')
                            throw scanner.CreateFormatException("后置引号的下一个字符非空格");

                        findQuotationMarks = false;
                    }
                    else
                    {
                        if (scanner.TryPeekPrevious(out var previous) && previous != ' ')
                            throw scanner.CreateFormatException("前置引号的上一个字符非空格");

                        findQuotationMarks = true;
                    }
                }
            }

            if (requestEscape)
                throw scanner.CreateFormatException("找不到转义符的下一个字符");

            if (findQuotationMarks)
                throw scanner.CreateFormatException("找不到后置引号");

            if (stringBuilder.Length > 0)
            {
                string word = stringBuilder.ToString().Trim(' ');
                result.Add(word);
            }

            return result.ToArray();
        }
    }
}
