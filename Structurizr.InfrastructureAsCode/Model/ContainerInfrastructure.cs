using System;
using System.Collections.Generic;

namespace Structurizr.InfrastructureAsCode
{
    public abstract class ContainerInfrastructure
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (!IsNameValid(value))
                {
                    throw new ArgumentException($"The given name \"{value}\" is not valid.");
                }
                var oldName = _name;
                _name = value;

                OnNameChanged(oldName, value);
            }
        }

        protected virtual void OnNameChanged(string oldName, string newName)
        {
        }

        protected virtual  bool IsNameValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }
    }
}