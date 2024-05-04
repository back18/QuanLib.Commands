using QuanLib.Commands.Words;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandParser
    {
        public CommandParser(CommandManager commandManager)
        {
            ArgumentNullException.ThrowIfNull(commandManager, nameof(commandManager));

            CommandManager = commandManager;
        }

        public CommandManager CommandManager { get; }

        public WordCollection Parse(string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            string[] wordTexts;
            try
            {
                wordTexts = WordSplitter.Split(text);
            }
            catch (FormatException formatException)
            {
                return new([new ErrorIdentifierWord(text, 0) { Exception = formatException }]);
            }

            Context context = new(this, wordTexts);

            while (true)
            {
                Word? word = ParseNextWord(context);
                if (word is null)
                    break;
                context.AddNextWord(word);
            }

            return new(context.GetWords());
        }

        private static Word? ParseNextWord(Context context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            if (context.NextWordText is null)
                return null;

            //处理冗余标识符
            if (context.CurrentWord is ErrorIdentifierWord || (context.Command is not null && context.ArgumentCount >= context.Command.CommandFunction.Arguments.Count))
            {
                return new ErrorIdentifierWord(context.NextWordText, context.NextWordTextStartIndex)
                {
                    Exception = new FormatException("命令的标识符或参数的数量超过预期数量")
                };
            }

            //处理命令标识符
            if (context.CurrentWord is null or IdentifierWord && context.CurrentNode.TryGetChildNode(context.NextWordText, out var childNode))
            {
                return new IdentifierWord(context.NextWordText, context.NextWordTextStartIndex, childNode)
                {
                    AllowedTexts = context.CurrentNode.GetChildNodeIdentifiers().AsReadOnly()
                };
            }

            //处理命令参数
            if (context.Command is not null)
            {
                CommandArgument argument = context.Command.CommandFunction.Arguments[context.ArgumentCount];
                string unescapedText = Word.Unescape(context.NextWordText);

                if (!argument.Parser.TryParse(unescapedText, null, out var value))
                    return new ParsingFailedArgumentWord(context.NextWordText, context.NextWordTextStartIndex, argument, null);

                foreach (ValidationAttribute validationAttribute in argument.ValidationAttributes)
                {
                    if (!validationAttribute.IsValid(value))
                        return new ValidationFailedArgumentWord(context.NextWordText, context.NextWordTextStartIndex, argument, value);
                }

                return new ArgumentWord(context.NextWordText, context.NextWordTextStartIndex, argument, value);
            }

            //处理其他情况

            return new ErrorIdentifierWord(context.NextWordText, context.NextWordTextStartIndex)
            {
                AllowedTexts = context.CurrentNode.GetChildNodeIdentifiers().AsReadOnly()
            };
        }

        private class Context
        {
            public Context(CommandParser owner, IList<string> wordTexts)
            {
                ArgumentNullException.ThrowIfNull(owner, nameof(owner));
                ArgumentNullException.ThrowIfNull(wordTexts, nameof(wordTexts));

                _owner = owner;
                WordTexts = wordTexts.AsReadOnly();
                _words = [];
            }

            private readonly CommandParser _owner;

            private readonly List<Word> _words;

            public ReadOnlyCollection<string> WordTexts { get; }

            public IdentifierNode CurrentNode => (_words.LastOrDefault(item => item is IdentifierWord) as IdentifierWord)?.IdentifierNode ?? _owner.CommandManager.RootNode;

            public Word? PreviousWord => _words.Count >= 2 ? _words[^2] : null;

            public Word? CurrentWord => _words.Count >= 1 ? _words[^1] : null;

            public string? NextWordText => WordTexts.Count > _words.Count ? WordTexts[_words.Count] : null;

            public int NextWordTextStartIndex => CurrentWord is null ? 0 : CurrentWord.EndIndex + 1;

            public int ArgumentCount => _words.Count(word => word is ArgumentWord);

            public Command? Command
            {
                get
                {
                    string identifier = string.Join(' ', _words.Where(s => s is IdentifierWord).Select(s => s.Text));
                    _owner.CommandManager.TryGetValue(identifier, out var command);
                    return command;
                }
            }

            public Word[] GetWords()
            {
                return _words.ToArray();
            }

            public void AddNextWord(Word word)
            {
                ArgumentNullException.ThrowIfNull(word, nameof(word));

                if (_words.Contains(word))
                    throw new InvalidOperationException("尝试重复添加命令词");

                word.Previous = CurrentWord;
                if (CurrentWord is not null)
                    CurrentWord.Next = word;

                _words.Add(word);
            }
        }
    }
}
