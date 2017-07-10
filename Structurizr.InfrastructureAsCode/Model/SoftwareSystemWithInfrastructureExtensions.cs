using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Structurizr.InfrastructureAsCode
{
    public static class SoftwareSystemWithInfrastructureExtensions
    {
        public static IEnumerable<IHaveInfrastructure> ElementsWithInfrastructure(this SoftwareSystemWithInfrastructure softwareSystem)
        {
            List<object> visited = new List<object>();
            List<IHaveInfrastructure> elements = new List<IHaveInfrastructure>();

            AddChildren(softwareSystem, elements, visited);

            return elements.Distinct();
        }

        private static void AddChildren(object element, List<IHaveInfrastructure> elements, List<object> visited)
        {
            if (ReferenceEquals(null, element) || visited.Contains(element))
            {
                return;
            }
            visited.Add(element);
            if (element is IHaveInfrastructure)
            {
                elements.Add((IHaveInfrastructure)element);
            }

            foreach (var property in element.GetType().GetProperties().Where(IsValidType))
            {
                var childValue = property.GetValue(element);
                if (ReferenceEquals(null, childValue))
                {
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable)childValue;
                    foreach (var child in children)
                    {
                        AddChildren(child, elements, visited);
                    }
                }
                else
                {
                    AddChildren(childValue, elements, visited);
                }
            }
        }

        private static bool IsValidType(PropertyInfo p)
        {
            if (p.PropertyType.IsPrimitive || p.PropertyType == typeof(string))
            {
                return false;
            }
            return p.PropertyType.IsClass || p.PropertyType.IsInterface;
        }
    }
}