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
                _name = value;
            }
        }

        protected virtual  bool IsNameValid(string name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }
    }
}