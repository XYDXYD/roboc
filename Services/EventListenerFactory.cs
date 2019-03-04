using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace Services
{
	internal abstract class EventListenerFactory : IEventListenerFactory
	{
		private readonly Dictionary<Type, Type> _bindings = new Dictionary<Type, Type>();

		public Dictionary<Type, IServiceEventListenerBase> CreateAllEventListeners()
		{
			Dictionary<Type, IServiceEventListenerBase> dictionary = new Dictionary<Type, IServiceEventListenerBase>(_bindings.Count);
			Dictionary<Type, Type>.Enumerator enumerator = _bindings.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<Type, Type> current = enumerator.Current;
				if (!current.Value.IsInstanceOfType(typeof(IServiceEventListenerBase)))
				{
					object obj = Activator.CreateInstance(current.Value);
					dictionary.Add(current.Key, (IServiceEventListenerBase)obj);
				}
				else
				{
					Console.LogError($"Failed to create instance of {current.Value} as it does not inherit from PhotonEventListenerBase");
				}
			}
			return dictionary;
		}

		protected void Bind<TInterface, TImplementation, TDependency>() where TInterface : IServiceEventListener<TDependency> where TImplementation : TInterface, new()
		{
			_bindings.Add(typeof(TInterface), typeof(TImplementation));
		}

		protected void Bind<TInterface, TImplementation, TDependency1, TDependancy2>() where TInterface : IServiceEventListener<TDependency1, TDependancy2> where TImplementation : TInterface, new()
		{
			_bindings.Add(typeof(TInterface), typeof(TImplementation));
		}

		protected void Bind<TInterface, TImplementation>() where TInterface : IServiceEventListenerBase where TImplementation : TInterface, new()
		{
			_bindings.Add(typeof(TInterface), typeof(TImplementation));
		}
	}
}
