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

        public IEnumerable<ConfigurationValue> Values => _elements.Select(e => e.Value);
        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class ConfigurationElement
    {
        public ConfigurationValue Value { get; set; }
    }

    public abstract class ConfigurationValue { }

    public class ConfigurationValue<T> : ConfigurationValue
    {
        public T Value { get; }

        public ConfigurationValue(T value)
        {
            Value = value;
        }
    }
}