using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class SpawnEmpLocatorClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SpawnEmpLocatorDependency _dependency;

		private RemoteLocatorData _empLocatorData = new RemoteLocatorData(0, Vector3.get_zero());

		[Inject]
		internal RemoteSpawnEmpLocatorObservable spawnEmpLocatorObservable
		{
			private get;
			set;
		}

		public void Execute()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			_empLocatorData.SetValues(_dependency.ownerId, _dependency.position);
			spawnEmpLocatorObservable.Dispatch(ref _empLocatorData);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as SpawnEmpLocatorDependency);
			return this;
		}
	}
}
