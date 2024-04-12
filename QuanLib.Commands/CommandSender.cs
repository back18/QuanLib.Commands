using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Commands
{
    public class CommandSender
    {
        public CommandSender(string name, PrivilegeLevel privilegeLevel)
        {
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

            Name = name;
            PrivilegeLevel = privilegeLevel;
        }

        public string Name { get; }

        public PrivilegeLevel PrivilegeLevel { get; }
    }
}
