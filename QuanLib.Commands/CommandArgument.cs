using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandArgument
    {
        public CommandArgument(ParameterInfo parameterInfo, int index)
        {
            ArgumentNullException.ThrowIfNull(parameterInfo, nameof(parameterInfo));
            ThrowHelper.ArgumentOutOfMin(0, index, nameof(index));

            Index = index;
            Type = parameterInfo.ParameterType;

            DisplayAttribute? displayAttribute = parameterInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute is not null)
            {
                Name = displayAttribute.Name ?? string.Empty;
                Description = displayAttribute.Description ?? string.Empty;
            }
            else
            {
                displayAttribute = parameterInfo.ParameterType.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute is not null)
                {
                    Name = displayAttribute.Name ?? string.Empty;
                    Description = displayAttribute.Description ?? string.Empty;
                }
                else
                {
                    Name = parameterInfo.Name ?? string.Empty;
                    Description = string.Empty;
                }
            }

            Parser = ParserBuilder.FromType(parameterInfo.ParameterType);

            List<ValidationAttribute> validationAttributes = [];
            IEnumerable<Attribute> attributes = parameterInfo.GetCustomAttributes();
            foreach (Attribute attribute in attributes)
            {
                if (attribute is ValidationAttribute validationAttribute)
                    validationAttributes.Add(validationAttribute);
            }

            ValidationAttributes = validationAttributes.AsReadOnly();
        }

        public int Index { get; }

        public Type Type { get; }

        public string Name { get; }

        public string Description { get; }

        public Parser<object> Parser { get; }

        public ReadOnlyCollection<ValidationAttribute> ValidationAttributes { get; }
    }
}
