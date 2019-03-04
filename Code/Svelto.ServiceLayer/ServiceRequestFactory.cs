using System;
using System.Collections.Generic;

namespace Svelto.ServiceLayer
{
	public class ServiceRequestFactory : IServiceRequestFactory
	{
		private interface IHoldValue
		{
			IServiceRequest CreateInstance();

			IServiceRequest CreateInstance(object param);
		}

		private class Value<RequestClass> : IHoldValue where RequestClass : IServiceRequest, new()
		{
			public IServiceRequest CreateInstance()
			{
				return new RequestClass();
			}

			public IServiceRequest CreateInstance(object param)
			{
				throw new ArgumentException();
			}
		}

		private class Value<RequestClass, Dependency> : IHoldValue where RequestClass : IServiceRequest<Dependency>, new()
		{
			public IServiceRequest CreateInstance()
			{
				return new RequestClass();
			}

			public IServiceRequest CreateInstance(object param)
			{
				IServiceRequest<Dependency> serviceRequest = new RequestClass();
				if (serviceRequest == null)
				{
					throw new ServiceRequestFactoryArgumentException("Not valid RequestService Class or incompatible dependency");
				}
				Dependency val = (Dependency)param;
				if (val == null)
				{
					throw new ServiceRequestFactoryArgumentException("incompatible dependency");
				}
				serviceRequest.Inject(val);
				return serviceRequest;
			}
		}

		private readonly Dictionary<Type, IHoldValue> _requestMap = new Dictionary<Type, IHoldValue>();

		public RequestInterface Create<RequestInterface>() where RequestInterface : class, IServiceRequest
		{
			IHoldValue holdValue = RetrieveObjectType<RequestInterface>();
			return holdValue.CreateInstance() as RequestInterface;
		}

		public RequestInterface Create<RequestInterface, Dependency>(Dependency param) where RequestInterface : class, IServiceRequest<Dependency>
		{
			IHoldValue holdValue = RetrieveObjectType<RequestInterface>();
			return holdValue.CreateInstance(param) as RequestInterface;
		}

		protected void AddRelation<RequestInterface, RequestClass, Dependency>() where RequestInterface : IServiceRequest<Dependency> where RequestClass : RequestInterface, new()
		{
			_requestMap[typeof(RequestInterface)] = new Value<RequestClass, Dependency>();
		}

		protected void AddRelation<RequestInterface, RequestClass>() where RequestInterface : IServiceRequest where RequestClass : RequestInterface, new()
		{
			_requestMap[typeof(RequestInterface)] = new Value<RequestClass>();
		}

		private IHoldValue RetrieveObjectType<RequestInterface>()
		{
			Type typeFromHandle = typeof(RequestInterface);
			if (!_requestMap.ContainsKey(typeFromHandle))
			{
				throw new ServiceRequestFactoryArgumentException("Request not registered");
			}
			IHoldValue holdValue = _requestMap[typeFromHandle];
			if (holdValue == null)
			{
				throw new ServiceRequestFactoryArgumentException("Request not found");
			}
			return holdValue;
		}
	}
}
