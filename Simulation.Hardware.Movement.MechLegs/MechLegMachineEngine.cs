using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using System;

namespace Simulation.Hardware.Movement.MechLegs
{
	internal class MechLegMachineEngine : MultiEntityViewsEngine<MechLegMachineView, MechLegView>, IQueryingEntityViewEngine, IEngine
	{
		[Inject]
		internal MechLegController controller
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

		protected override void Add(MechLegMachineView entityView)
		{
			controller.Register(entityView.ownerComponent.ownedByMe, entityView.ownerComponent.ownedByAi, entityView.get_ID(), entityView.rigidbodyComponent.rb, entityView.inputComponent.machineInput);
			entityView.rectifyingComponent.onFunctionalsEnabled.NotifyOnValueSet((Action<int, bool>)HandleOnFunctionalsEnabled);
			entityView.stunComponent.machineStunned.subscribers += HandleOnMachineStunned;
			RegisterLegsForMachine(entityView.ownerComponent.ownerMachineId);
		}

		private void HandleOnMachineStunned(IMachineStunComponent machineStunComponent, int machineId)
		{
			controller.SetMachineLegsEnabled(machineId, !machineStunComponent.stunned);
		}

		private void HandleOnFunctionalsEnabled(int machineId, bool active)
		{
			controller.SetMachineLegsEnabled(machineId, active);
		}

		protected override void Remove(MechLegMachineView entityView)
		{
			controller.Unregister(entityView.get_ID());
			entityView.rectifyingComponent.onFunctionalsEnabled.StopNotify((Action<int, bool>)HandleOnFunctionalsEnabled);
			entityView.stunComponent.machineStunned.subscribers -= HandleOnMachineStunned;
		}

		protected override void Add(MechLegView entityView)
		{
			if (IsMechLegMachineViewRegistered(entityView.ownerComponent.machineId))
			{
				RegisterLeg(entityView);
			}
		}

		protected override void Remove(MechLegView entityView)
		{
			entityView.disabledComponent.isPartDisabled.StopNotify((Action<int, bool>)OnLegEnabled);
		}

		private void OnLegEnabled(int id, bool enabled)
		{
			MechLegView mechLegView = default(MechLegView);
			if (entityViewsDB.TryQueryEntityView<MechLegView>(id, ref mechLegView))
			{
				int machineId = mechLegView.ownerComponent.machineId;
				CubeMechLeg leg = mechLegView.mechLegComponent as CubeMechLeg;
				if (enabled)
				{
					controller.RegisterLeg(leg, machineId);
				}
				else
				{
					controller.UnregisterLeg(leg, machineId);
				}
			}
		}

		private void RegisterLegsForMachine(int machineId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<MechLegView> val = entityViewsDB.QueryEntityViews<MechLegView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				MechLegView mechLegView = val.get_Item(i);
				if (mechLegView.ownerComponent.machineId == machineId)
				{
					RegisterLeg(mechLegView);
				}
			}
		}

		private void RegisterLeg(MechLegView entityView)
		{
			entityView.disabledComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnLegEnabled);
			if (entityView.disabledComponent.enabled)
			{
				CubeMechLeg leg = entityView.mechLegComponent as CubeMechLeg;
				controller.RegisterLeg(leg, entityView.ownerComponent.machineId);
			}
		}

		private bool IsMechLegMachineViewRegistered(int machineId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<MechLegMachineView> val = entityViewsDB.QueryEntityViews<MechLegMachineView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				if (val.get_Item(i).ownerComponent.ownerMachineId == machineId)
				{
					return true;
				}
			}
			return false;
		}
	}
}
