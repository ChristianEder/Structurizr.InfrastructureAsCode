using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structurizr.InfrastructureAsCode
{
    public class Configuration<TElement>
        : IEnumerable<TElement>
        where TElement : ConfigurationElement
    {
        private readonly List<TElement> _elements = new List<TElement>();

        public void Add(TElement element)
        {
            _elements.Add(element);
        }

        public IEnumerable<IConfigurationValue> Values => _elements.Select(e => e.Value);
        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(TElement element)
        {
            _elements.Remove(element);
        }
    }

    public abstract class ConfigurationElement
    {
        public IConfigurationValue Value { get; set; }
        public string Name { get; set; }
    }

    public interface IConfigurationValue
    {
        object Value { get;  }
        bool ShouldBeStoredSecure { get; }
    }

    public abstract class ConfigurationValue : IConfigurationValue
    {
        public virtual object Value { get;  }
        public abstract bool ShouldBeStoredSecure { get; }
    }

    public class FixedConfigurationValue<T> : IConfigurationValue
    {
        public virtual T Value { get; }

        public FixedConfigurationValue(T value)
        {
            Value = value;
        }

        object IConfigurationValue.Value => Value;
        public bool ShouldBeStoredSecure { get; set; }
    }
}