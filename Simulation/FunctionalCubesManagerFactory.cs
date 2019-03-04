using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

namespace Simulation
{
	internal sealed class FunctionalCubesManagerFactory
	{
		[Inject]
		public WeakReference<IEntitySystemContext> systemContext
		{
			private get;
			set;
		}

		public object BuildManager(Type cubesManagerType, params object[] constructorParams)
		{
			object obj = Activator.CreateInstance(cubesManagerType, constructorParams);
			if (obj is IComponent)
			{
				systemContext.get_Target().AddComponent(obj as IComponent);
			}
			return obj;
		}

		public object BuildManager(Type cubesManagerType)
		{
			object obj = Activator.CreateInstance(cubesManagerType);
			if (obj is IComponent)
			{
				systemContext.get_Target().AddComponent(obj as IComponent);
			}
			return obj;
		}
	}
}
