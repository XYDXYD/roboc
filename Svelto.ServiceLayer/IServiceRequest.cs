namespace Svelto.ServiceLayer
{
	public interface IServiceRequest
	{
		void Execute();
	}
	public interface IServiceRequest<in TDependency> : IServiceRequest
	{
		void Inject(TDependency dependency);
	}
}
