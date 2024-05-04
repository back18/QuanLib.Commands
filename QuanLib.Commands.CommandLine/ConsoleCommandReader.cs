using QuanLib.Commands.Words;
using QuanLib.Consoles;
using QuanLib.Consoles.Events;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.CommandLine
{
    public class ConsoleCommandReader : ConsoleTextReader
    {
        public ConsoleCommandReader(CommandManager commandManager, ILoggerGetter? loggerGetter = null) : base(loggerGetter)
        {
            ArgumentNullException.ThrowIfNull(commandManager, nameof(commandManager));

            CommandManager = commandManager;
            CommandParser = new(commandManager);
            Palette = Palette.Default;
            _outputBuffer = new(CursorPosition.Current);
            _wordCollection = new(Array.Empty<Word>());
            _history = new();
            _selectedIndexs = [];

            KeyEventHandler.Subscribe(new(ConsoleModifiers.None, ConsoleKey.Tab), HandleTabKey);
            KeyEventHandler.Subscribe(new(ConsoleModifiers.None, ConsoleKey.UpArrow), HandleUpArrowKey);
            KeyEventHandler.Subscribe(new(ConsoleModifiers.None, ConsoleKey.DownArrow), HandleDownArrowKey);
            KeyEventHandler.Subscribe(new(ConsoleModifiers.Control, ConsoleKey.LeftArrow), HandleControlLeftArrowKey);
            KeyEventHandler.Subscribe(new(ConsoleModifiers.Control, ConsoleKey.RightArrow), HandleControlRigthArrowKey);
        }

        private readonly ConsoleTextBuffer _outputBuffer;

        protected WordCollection _wordCollection;

        private readonly History _history;

        private readonly Dictionary<int, int> _selectedIndexs;

        private CommandReaderResult? _result;

        public CommandManager CommandManager { get; }

        public CommandParser CommandParser { get; }

        public Palette Palette { get; set; }

        public Word? CurrentWord
        {
            get
            {
                int index = _wordCollection.TextIndex2WordIndex(_textBuffer.Index);
                if (index != -1)
                    return _wordCollection[index];
                else
                    return null;
            }
        }

        public int CurrentWordIndex => _wordCollection.TextIndex2WordIndex(_textBuffer.Index);

        public CommandReaderResult GetResult()
        {
            if (_result is null)
                throw new InvalidOperationException("命令读取器未读取完成");

            return _result;
        }

        protected override void OnKeyRead(ConsoleKeyReader sender, ConsoleKeyInfoEventArgs e)
        {
            ClearText();
            HandleKeyEvent(e.ConsoleKeyInfo);
            Update();
            WriteText();
        }

        protected override void WriteText()
        {
            CommandManager.TryGetValue(_wordCollection.GetIdentifier(), out var command);

            Word? currentWord = CurrentWord;
            List<ConsoleText> wordTexts = [];
            wordTexts.Add(new("> ", FontColor.Current));
            foreach (Word word in _wordCollection)
            {
                if (word is ErrorIdentifierWord or ParsingFailedArgumentWord or ValidationFailedArgumentWord)
                    wordTexts.Add(new(word.Text, Palette.ErrorColor));
                else if (word is IdentifierWord)
                    wordTexts.Add(new(word.Text, Palette.IdentifierColor));
                else if (word is ArgumentWord)
                    wordTexts.Add(new(word.Text, Palette.ArgumentColor));
                else
                    wordTexts.Add(new(word.Text, FontColor.Current));

                if (word == currentWord)
                {
                    string[] preSelectedTexts = word.GetPreSelectedTexts();
                    if (preSelectedTexts.Length > 0)
                        wordTexts.Add(new(preSelectedTexts[GetSelectedIndex(CurrentWordIndex, new(0, preSelectedTexts.Length - 1))][word.Text.Length..], Palette.CueWordColor));

                    wordTexts.Add(new(" <-", FontColor.Current));

                    if (word is ArgumentWord argumentWord)
                        wordTexts.Add(new($"{argumentWord.Argument.Name}({ObjectFormatter.Format(argumentWord.Argument.Type)})", Palette.ArgumentInfoColor));

                    if (word is IErrorWord errorWord && errorWord.Exception is Exception exception)
                        wordTexts.Add(new($"({ObjectFormatter.Format(exception)})", Palette.ErrorColor));
                }

                wordTexts.Add(ConsoleText.SpaceOfCurrentColor);
            }

            _textBuffer.ExpressionConsoleHeight();
            _textBuffer.InitialPosition.Offset(-2, 0).Apply();
            _outputBuffer.Clear();
            _outputBuffer.SetInitialPosition(_textBuffer.InitialPosition.Offset(-2, 0));

            foreach (ConsoleText wordText in wordTexts)
            {
                _outputBuffer.Write(wordText.Text);
                wordText.WriteToConsole();
            }

            int outputRemainingHeight = Console.BufferHeight - 1 - _outputBuffer.EndPosition.Y;
            if (outputRemainingHeight < 0)
                _outputBuffer.OffsetBuffer(0, outputRemainingHeight);

            int textRemainingHeight = Console.BufferHeight - 1 - _textBuffer.EndPosition.Y;
            int heightDifference = _outputBuffer.Height - _textBuffer.Height;
            if (heightDifference > textRemainingHeight)
                _textBuffer.OffsetBuffer(0, -heightDifference);
            
            _textBuffer.CurrentPosition.Apply();
        }

        protected override void ClearText()
        {
            Console.CursorTop = _outputBuffer.InitialPosition.Y;
            Console.CursorLeft = 0;

            string whiteSpace = new(' ', _outputBuffer.Width);
            for (int i = 0; i < _outputBuffer.Height; i++)
                Console.Write(whiteSpace);

            _textBuffer.CurrentPosition.Apply();
        }

        protected virtual void Update()
        {
            _wordCollection = CommandParser.Parse(Text);
        }

        protected override void OnStarted(IRunnable sender, EventArgs e)
        {
            if (Console.CursorLeft != 0)
            {
                _textBuffer.OffsetBuffer(-Console.CursorLeft, 1);
                _outputBuffer.OffsetBuffer(-Console.CursorLeft, 1);
                Console.WriteLine();
            }

            _textBuffer.SetInitialPosition(CursorPosition.Current.Offset(2, 0));
            _textBuffer.Clear();
            _textBuffer.Update();
            _outputBuffer.SetInitialPosition(CursorPosition.Current);
            _outputBuffer.Clear();
            _outputBuffer.Update();
            _selectedIndexs.Clear();

            ClearText();
            Update();
            WriteText();
        }

        protected override void OnStopped(IRunnable sender, EventArgs e)
        {
            _outputBuffer.EndPosition.Apply();
            Console.WriteLine();
        }

        private int GetSelectedIndex(int wordIndex, IndexRange preSelectedRange)
        {
            ThrowHelper.ArgumentOutOfMin(0, wordIndex, nameof(wordIndex));

            _selectedIndexs.TryAdd(wordIndex, 0);
            _selectedIndexs[wordIndex] = Math.Clamp(_selectedIndexs[wordIndex], preSelectedRange.Start, preSelectedRange.End);
            return _selectedIndexs[wordIndex];
        }

        private void SetSelectedIndex(int wordIndex, int value, IndexRange preSelectedRange)
        {
            _selectedIndexs[wordIndex] = Math.Clamp(value, preSelectedRange.Start, preSelectedRange.End);
        }

        private void OffsetSelectedIndex(int wordIndex, int offset, IndexRange preSelectedRange)
        {
            SetSelectedIndex(wordIndex, GetSelectedIndex(wordIndex, preSelectedRange) + offset, preSelectedRange);
        }

        protected override void HandleEnterKey()
        {
            string identifier = _wordCollection.GetIdentifier();
            CommandManager.TryGetValue(identifier, out var command);
            string[] args = _wordCollection.GetArgumentTexts();
            _result = new(command, args);
            _history.Add(Text);
            IsRunning = false;
        }

        protected virtual void HandleTabKey()
        {
            Word? currentWord = CurrentWord;
            if (currentWord is not null)
            {
                string[] preSelectedTexts = currentWord.GetPreSelectedTexts();
                if (preSelectedTexts.Length > 0)
                {
                    _textBuffer.SetPosition(currentWord.EndIndex);
                    for (int i = 0; i < currentWord.Text.Length; i++)
                        _textBuffer.Backspace();
                    _textBuffer.Write(preSelectedTexts[GetSelectedIndex(CurrentWordIndex, new(0, preSelectedTexts.Length - 1))]);
                }
            }
        }

        protected override void HandleControlBackspaceKey()
        {
            Word? currentWord = CurrentWord;
            if (currentWord is not null)
            {
                while (_textBuffer.Index > currentWord.StartIndex)
                    _textBuffer.Backspace();
            }
        }

        protected virtual void HandleUpArrowKey()
        {
            Word? currentWord = CurrentWord;
            if (currentWord is not null)
            {
                string[] preSelectedTexts = currentWord.GetPreSelectedTexts();
                if (preSelectedTexts.Length > 0)
                    OffsetSelectedIndex(CurrentWordIndex, -1, new(0, preSelectedTexts.Length - 1));
            }
        }

        protected virtual void HandleDownArrowKey()
        {
            Word? currentWord = CurrentWord;
            if (currentWord is not null)
            {
                string[] preSelectedTexts = currentWord.GetPreSelectedTexts();
                if (preSelectedTexts.Length > 0)
                    OffsetSelectedIndex(CurrentWordIndex, 1, new(0, preSelectedTexts.Length - 1));
            }
        }

        protected virtual void HandleControlLeftArrowKey()
        {
            if (_history.Count > 0)
            {
                if (_history.IsReadToEnd)
                    _history.Add(Text);
                _history.Previous();
                _textBuffer.Clear();
                _textBuffer.Write(_history.Current);
            }
        }

        protected virtual void HandleControlRigthArrowKey()
        {
            if (_history.Count > 0)
            {
                if (_history.IsReadToEnd)
                    _history.Add(Text);
                _history.Next();
                _textBuffer.Clear();
                _textBuffer.Write(_history.Current);
            }
        }
    }
}
