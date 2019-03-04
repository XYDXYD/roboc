using Svelto.ServiceLayer;
using System;

internal class ServiceException : Exception
{
	public ServiceBehaviour _serviceBehaviour;

	public ServiceException(ServiceBehaviour serviceBehaviour)
	{
		_serviceBehaviour = serviceBehaviour;
	}
}
