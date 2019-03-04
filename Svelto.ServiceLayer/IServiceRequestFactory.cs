namespace Svelto.ServiceLayer
{
	public interface IServiceRequestFactory
	{
		TRequestType Create<TRequestType>() where TRequestType : class, IServiceRequest;

		TRequestType Create<TRequestType, TDependency>(TDependency param) where TRequestType : class, IServiceRequest<TDependency>;
	}
}
