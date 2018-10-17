using System;

namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public interface IDependentConfigurationValue : IConfigurationValue
    {
        IHaveResourceId DependsOn { get; }
    }

    public abstract class DependentConfigurationValue<TDependentOn> : IDependentConfigurationValue
        where TDependentOn: IHaveResourceId
    {
        protected DependentConfigurationValue(TDependentOn dependentOn)
        {
            DependsOn = dependentOn;
        }

        public abstract object Value { get; }
        public abstract bool ShouldBeStoredSecure { get; }
        public TDependentOn DependsOn { get; }

        IHaveResourceId IDependentConfigurationValue.DependsOn => DependsOn;
        public override bool Equals(object other)
        {
            return Equals(other as DependentConfigurationValue<TDependentOn>);
        }

        protected bool Equals(DependentConfigurationValue<TDependentOn> other)
        {
            return !ReferenceEquals(null, other) && Equals(DependsOn, other.DependsOn);
        }

        public override int GetHashCode()
        {
            return DependsOn?.GetHashCode() ?? 0;
        }
    }
}