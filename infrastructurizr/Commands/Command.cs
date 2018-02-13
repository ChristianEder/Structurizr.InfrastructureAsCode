using System.Collections.Generic;

namespace infrastructurizr.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract IEnumerable<CommandParameter> Parameters { get; }

        public abstract void Execute();

        public void SetParameter(string name, string value)
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class CommandParameter
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Type { get; }
        public abstract bool IsMandatory { get; }
        public string Value { get; set; }

        public bool IsSatisfied()
        {
            return !IsMandatory || !string.IsNullOrEmpty(Value);
        }
    }
}
