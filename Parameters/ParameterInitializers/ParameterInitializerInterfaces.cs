namespace Bible_Blazer_PWA.Parameters.ParameterInitializers
{
    public interface IConcreteParameterInitializer: IGenericParameterInitializer
    {
        Parameters Parameter { get; }
    }

    public interface IGenericParameterInitializer
    {
        string InitParam(string previousValue);
    }
}
