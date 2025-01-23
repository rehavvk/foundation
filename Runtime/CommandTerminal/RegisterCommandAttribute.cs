using System;
using System.Reflection;

namespace Rehawk.Foundation.CommandTerminal
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegisterCommandAttribute : Attribute
    {
        private int minArgCount = 0;
        private int maxArgCount = -1;

        public int MinArgCount
        {
            get { return minArgCount; }
            set { minArgCount = value; }
        }

        public int MaxArgCount
        {
            get { return maxArgCount; }
            set { maxArgCount = value; }
        }

        public string Name { get; set; }
        public string Help { get; set; }
        public string Hint { get; set; }

        public RegisterCommandAttribute(string commandName = null)
        {
            Name = commandName;
        }
    }
}