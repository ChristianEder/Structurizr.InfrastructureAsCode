using System;
using System.Collections.Generic;
using System.Linq;

namespace infrastructurizr.Commands
{
    public abstract class Command
    {
        private List<CommandParameter> _parameters;
        public abstract string Name { get; }
        public abstract string Description { get; }

        public List<CommandParameter> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = GetType()
                        .GetProperties()
                        .Where(p => typeof(CommandParameter).IsAssignableFrom(p.PropertyType))
                        .Select(p => p.GetValue(this))
                        .Cast<CommandParameter>()
                        .ToList();
                }
                return _parameters;
            }
        }

        public abstract void Execute();

        public void SetParameter(string name, string value)
        {
            var parameter =
                Parameters.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (parameter != null)
            {
                parameter.Set(value);
            }
            else
            {
                Parameters.Add(string.IsNullOrWhiteSpace(value)
                    ? new SwitchParameter(name, "").Set(value)
                    : new StringParameter(name, "").Set(value));
            } 
        }
    }

    public abstract class CommandParameter
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Type { get; }
        public abstract bool IsMandatory { get; }
        public bool HasValue { get; protected set; }
        public abstract CommandParameter Set(string value);

        public bool IsSatisfied()
        {
            return !IsMandatory || HasValue;
        }
    }

    public class CommandParameter<T> : CommandParameter
    {
        private readonly TryParse _tryParse;
        private T _value;

        public delegate bool TryParse(string value, out T parsed);

        public T Value => _value;

        public override string Name { get; }
        public override string Description { get; }
        public override string Type { get; }
        public override bool IsMandatory { get; }

        public CommandParameter(TryParse tryParse, string name, string description, bool isMandatory = false)
        {
            _tryParse = tryParse;
            Name = name;
            Description = description;
            Type = typeof(T).Name;
            IsMandatory = isMandatory;
        }

        public override CommandParameter Set(string value)
        {
            HasValue = _tryParse(value, out _value);
            return this;
        }
    }

    public class StringParameter : CommandParameter<string>
    {
        public StringParameter(string name, string description, bool isMandatory = false)
            : base(Parse, name, description, isMandatory)
        {
        }
        
        private static bool Parse(string value, out string parsed)
        {
            parsed = value;
            return true;
        }
    }

    public class SwitchParameter : CommandParameter
    {
        public SwitchParameter(string name, string description, bool isMandatory = false)
        {
            Name = name;
            Description = description;
            Type = "Switch";
            IsMandatory = isMandatory;
        }

        public override string Name { get; }
        public override string Description { get; }
        public override string Type { get; }
        public override bool IsMandatory { get; }
        public bool IsSet { get; private set; }

        public override CommandParameter Set(string value)
        {
            HasValue = IsSet = true;
            return this;
        }
    }
}
