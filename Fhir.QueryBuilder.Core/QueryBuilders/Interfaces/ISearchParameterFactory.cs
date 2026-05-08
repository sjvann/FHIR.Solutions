namespace Fhir.QueryBuilder.QueryBuilders.Interfaces
{
    public interface ISearchParameterFactory
    {
        ISearchParameterBuilder? GetBuilder(string parameterType);
        IEnumerable<ISearchParameterBuilder> GetAllBuilders();
        IEnumerable<string> GetSupportedParameterTypes();
    }
}
