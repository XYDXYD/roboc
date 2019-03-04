using System;
using System.Collections.Generic;
using System.Reflection;

namespace Svelto.ES.Legacy
{
	public class EnginesRoot : IEnginesRoot
	{
		private Dictionary<Type, List<IEngine>> _engines;

		public EnginesRoot()
		{
			_engines = new Dictionary<Type, List<IEngine>>();
		}

		public void AddEngine(IEngine engine)
		{
			Type[] array = engine.AcceptedComponents();
			foreach (Type key in array)
			{
				if (!_engines.ContainsKey(key))
				{
					List<IEngine> list = new List<IEngine>();
					list.Add(engine);
					_engines.Add(key, list);
				}
				else
				{
					_engines[key].Add(engine);
				}
			}
		}

		public void RemoveEngine(IEngine engine)
		{
			Type[] array = engine.AcceptedComponents();
			foreach (Type key in array)
			{
				if (_engines.ContainsKey(key))
				{
					_engines.Remove(key);
				}
			}
		}

		public void AddComponent(IComponent component)
		{
			Type type = component.GetType();
			List<Type> list = new List<Type>(type.GetInterfaces());
			list.AddRange(type.GetNestedTypes(BindingFlags.Public));
			list.Add(type);
			foreach (Type item in list)
			{
				if (_engines.ContainsKey(item))
				{
					_engines[item].ForEach(delegate(IEngine engine)
					{
						engine.Add(component);
					});
				}
			}
		}

		public void RemoveComponent(IComponent component)
		{
			Type type = component.GetType();
			List<Type> list = new List<Type>(type.GetInterfaces());
			list.AddRange(type.GetNestedTypes(BindingFlags.Public));
			list.Add(type);
			foreach (Type item in list)
			{
				if (_engines.ContainsKey(item))
				{
					_engines[item].ForEach(delegate(IEngine engine)
					{
						engine.Remove(component);
					});
				}
			}
		}
	}
}
