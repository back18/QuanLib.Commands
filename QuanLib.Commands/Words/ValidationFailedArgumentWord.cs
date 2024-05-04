using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Words
{
    public class ValidationFailedArgumentWord : ArgumentWord, IErrorWord
    {
        public ValidationFailedArgumentWord(string text, int startIndex, CommandArgument argument, object? value) : base(text, startIndex, argument, value) { }

        public Exception? Exception
        {
            get
            {
                foreach (ValidationAttribute validationAttribute in  Argument.ValidationAttributes)
                {
                    if (!validationAttribute.IsValid(Value))
                        return new ValidationException(new ValidationResult(validationAttribute.FormatErrorMessage(Argument.Name), [Argument.Name]), validationAttribute, Value);
                }

                return null;
            }
        }

        public override bool IsErrorWord => true;
    }
}
