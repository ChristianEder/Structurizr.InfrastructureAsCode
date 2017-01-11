namespace Structurizr.InfrastructureAsCode
{
    public class ContainerInfrastructureConfiguration<TElement> where TElement : ContainerInfrastructureConfigurationElement
    {
        public void Add(TElement element) { }
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