using System;

namespace Svelto.ES.Legacy
{
	public interface IEngine
	{
		Type[] AcceptedComponents();

		void Add(IComponent obj);

		void Remove(IComponent obj);
	}
}
