using System.Threading.Tasks;

namespace Structurizr.InfrastructureAsCode.InfrastructureRendering
{
    public interface IInfrastructureRenderer<TEnvironment> where TEnvironment : IInfrastructureEnvironment
    {
        Task Render(Model model, TEnvironment environment);
    }
}