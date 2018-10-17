namespace Structurizr.InfrastructureAsCode.Azure.Model
{
    public interface IHaveResourceId
    {
        string ResourceIdReference { get; }
        string ResourceIdReferenceContent { get; }
    }
}