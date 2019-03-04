using System;

namespace Svelto.ServiceLayer
{
	public interface IServiceAnswer<Success> : IServiceFailedAnswer
	{
		Action<Success> succeed
		{
			get;
		}
	}
	public interface IServiceAnswer : IServiceFailedAnswer
	{
		Action succeed
		{
			get;
		}
	}
}
