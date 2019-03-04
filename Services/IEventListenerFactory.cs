using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services
{
	internal interface IEventListenerFactory
	{
		Dictionary<Type, IServiceEventListenerBase> CreateAllEventListeners();
	}
}
