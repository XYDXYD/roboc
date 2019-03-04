using System;

namespace Svelto.ServiceLayer
{
	public interface IServiceFailedAnswer
	{
		Action<ServiceBehaviour> failed
		{
			get;
		}
	}
}
