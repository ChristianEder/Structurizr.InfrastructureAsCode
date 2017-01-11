using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Structurizr.InfrastructureAsCode
{
    public class ContainerInfrastructureConfiguration<TElement>
        : IEnumerable<TElement>
        where TElement : ContainerInfrastructureConfigurationElement
    {
        private readonly List<TElement> _elements = new List<TElement>();

        public void Add(TElement element)
        {
            _elements.Add(element);
        }

        public IEnumerable<ContainerInfrastructureConfigurationElementValue> Values => _elements.Select(e => e.Value);
        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public abstract class ContainerInfrastructureConfigurationElement
    {
        public ContainerInfrastructureConfigurationElementValue Value { get; set; }
    }

    public abstract class ContainerInfrastructureConfigurationElementValue { }

    public class ContainerInfrastructureConfigurationElementValue<T> : ContainerInfrastructureConfigurationElementValue
    {
        public T Value { get; }

        public ContainerInfrastructureConfigurationElementValue(T value)
        {
            Value = value;
        }
    }
}