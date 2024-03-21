using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands.Exceptions
{
    public class CommandArgumentCountException : CommandArgumentException
    {
        public CommandArgumentCountException() : base() { }

        public CommandArgumentCountException(string? message) : base(message) { }

        public CommandArgumentCountException(string? message, Exception innerException) : base(message, innerException) { }

        public CommandArgumentCountException(int presetCount, int actualCount) : base()
        {
            PresetCount = presetCount;
            ActualCount = actualCount;
        }

        public CommandArgumentCountException(int presetCount, int actualCount, string? message) : base(message)
        {
            PresetCount = presetCount;
            ActualCount = actualCount;
        }

        public int? PresetCount { get; }

        public int? ActualCount { get; }

        protected override string DefaultMessage => "命令参数的数量不是预期的";

        public override string Message
        {
            get
            {
                if (PresetCount is not null && ActualCount is not null)
                    return base.Message + $"（应为 {PresetCount} 个参数，实际为 {ActualCount} 个参数）";
                else
                    return base.Message;
            }
        }
    }
}
