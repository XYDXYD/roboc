using RCNetwork.Events;
using Simulation.Hardware.Modules.Emp.Observers;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;

namespace Simulation.Hardware
{
	internal sealed class MachineStunManagementEngine : IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		private NetworkStunnedMachineEffectDependency _dependency = new NetworkStunnedMachineEffectDependency();

		private MachineStunnedData _machineStunnedData = new MachineStunnedData(0, isStunned_: false);

		[Inject]
		internal InputController inputController
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient networkManager
		{
			private get;
			set;
		}

		[Inject]
		internal MachineStunnedObservable machineStunnedObservable
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<MachineStunNode> enumerator = entityViewsDB.QueryEntityViews<MachineStunNode>().GetEnumerator();
			while (enumerator.MoveNext())
			{
				MachineStunNode current = enumerator.get_Current();
				IMachineStunComponent stunComponent = current.stunComponent;
				IMachineOwnerComponent ownerComponent = current.ownerComponent;
				if (!stunComponent.stunned || (!ownerComponent.ownedByMe && !ownerComponent.ownedByAi))
				{
					continue;
				}
				int value = current.get_ID();
				if ((stunComponent.stunTimer -= deltaSec) <= 0f)
				{
					if (!current.ownerComponent.ownedByAi)
					{
						inputController.Enabled = true;
					}
					stunComponent.stunned = false;
					stunComponent.machineStunned.Dispatch(ref value);
					if (!current.ownerComponent.ownedByAi)
					{
						_dependency.SetValues(value, isStunned_: false, current.ownerComponent.ownerId);
						networkManager.SendEventToServer(NetworkEvent.BroadcastSpawnEmpMachineEffect, _dependency);
						_machineStunnedData.SetValues(value, isStunned_: false);
						machineStunnedObservable.Dispatch(ref _machineStunnedData);
					}
				}
			}
		}
	}
}
