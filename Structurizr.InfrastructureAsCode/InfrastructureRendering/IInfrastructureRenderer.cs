using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering
{
    public interface IInfrastructureRenderer<in TEnvironment> where TEnvironment : IInfrastructureEnvironment
    {
        Task Render(Model model, TEnvironment environment);
    }
}