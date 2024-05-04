using QuanLib.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.CommandLine
{
    public struct Palette
    {
        public Palette()
        {
            IdentifierColor = new(Console.BackgroundColor, ConsoleColor.Blue);
            ArgumentColor = new(Console.BackgroundColor, ConsoleColor.Green);
            ArgumentInfoColor = new(Console.BackgroundColor, ConsoleColor.Cyan);
            CueWordColor = new(Console.BackgroundColor, ConsoleColor.DarkGray);
            PreSelectedWordColor = new(Console.BackgroundColor, ConsoleColor.DarkGray);
            SelectedWordColor = new(ConsoleColor.White, ConsoleColor.Magenta);
            WarnColor = new(Console.BackgroundColor, ConsoleColor.Yellow);
            ErrorColor = new(Console.BackgroundColor, ConsoleColor.Red);
        }

        public static readonly Palette Default = new();

        public FontColor IdentifierColor { get; set; }

        public FontColor ArgumentColor { get; set; }

        public FontColor ArgumentInfoColor { get; set; }

        public FontColor CueWordColor { get; set; }

        public FontColor PreSelectedWordColor { get; set; }

        public FontColor SelectedWordColor { get; set; }

        public FontColor WarnColor { get; set; }

        public FontColor ErrorColor { get; set; }
    }
}
