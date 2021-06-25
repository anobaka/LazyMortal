namespace Bootstrap.Components.Doc.Swagger.ApiVisibility
{
    public interface IApiVisibilityAttribute<out TRealm>
    {
        TRealm[] Realms { get; }
    }
}
