using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering
{
    public interface IInfrastructureRenderer
    {
        Task Render(SoftwareSystemWithInfrastructure system);
    }
}