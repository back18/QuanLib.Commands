using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public partial class CommandIdentifier
    {
        public CommandIdentifier(string identifiers)
        {
            ArgumentException.ThrowIfNullOrEmpty(identifiers, nameof(identifiers));
            
            string[] identifiersArray = identifiers.Split(' ');
            ValidateIdentifiers(identifiersArray);
            Identifiers = identifiersArray.AsReadOnly();
        }

        public CommandIdentifier(string[] identifiers)
        {
            ValidateIdentifiers(identifiers);
            Identifiers = identifiers.AsReadOnly();
        }

        public ReadOnlyCollection<string> Identifiers { get; }

        public bool StartsWith(CommandIdentifier commandIdentifier)
        {
            ArgumentNullException.ThrowIfNull(commandIdentifier, nameof(commandIdentifier));

            return ToString().StartsWith(commandIdentifier.ToString());
        }

        public bool EndsWith(CommandIdentifier commandIdentifier)
        {
            ArgumentNullException.ThrowIfNull(commandIdentifier, nameof(commandIdentifier));

            return ToString().EndsWith(commandIdentifier.ToString());
        }

        private static void ValidateIdentifiers(string[] identifiers)
        {
            CollectionValidator.ValidateNullOrEmpty(identifiers, nameof(identifiers));
            ThrowHelper.ArrayLengthOutOfMin(1, identifiers, nameof(identifiers));

            foreach (var identifier in identifiers)
            {
                if (!IdentifierRegex().IsMatch(identifier))
                    throw new FormatException($"命令标识符“{identifier}”格式错误，只能由数字、字母、下划线组成");
            }
        }

        [GeneratedRegex("^[a-zA-Z0-9_]*$")]
        private static partial Regex IdentifierRegex();

        public override string ToString()
        {
            return string.Join(' ', Identifiers);
        }
    }
}
