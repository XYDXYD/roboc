using Svelto.ECS;
using Svelto.IoC;
using System;

namespace Simulation.Hardware.Movement.InsectLegs
{
	internal class InsectLegMachineEngine : MultiEntityViewsEngine<InsectLegMachineView, InsectLegView>, IQueryingEntityViewEngine, IEngine
	{
		[Inject]
		internal LegController controller
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

		protected override void Add(InsectLegMachineView entityView)
		{
			controller.Register(entityView.ownerComponent.ownedByMe, entityView.ownerComponent.ownedByAi, entityView.get_ID(), entityView.rigidbodyComponent.rb, entityView.inputComponent.machineInput);
			entityView.rectifyingComponent.onFunctionalsEnabled.NotifyOnValueSet((Action<int, bool>)HandleOnFunctionalsEnabled);
			entityView.stunComponent.machineStunned.subscribers += HandleOnMachineStunned;
		}

		private void HandleOnMachineStunned(IMachineStunComponent machineStunComponent, int machineId)
		{
			controller.SetMachineLegsEnabled(machineId, !machineStunComponent.stunned);
		}

		private void HandleOnFunctionalsEnabled(int machineId, bool active)
		{
			controller.SetMachineLegsEnabled(machineId, active);
		}

		protected override void Remove(InsectLegMachineView entityView)
		{
			controller.Unregister(entityView.get_ID());
			entityView.rectifyingComponent.onFunctionalsEnabled.StopNotify((Action<int, bool>)HandleOnFunctionalsEnabled);
			entityView.stunComponent.machineStunned.subscribers -= HandleOnMachineStunned;
		}

		protected override void Add(InsectLegView entityView)
		{
			entityView.disabledComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnLegEnabled);
			if (entityView.disabledComponent.enabled)
			{
				CubeLeg leg = entityView.insectLegComponent as CubeLeg;
				controller.RegisterLeg(leg, entityView.ownerComponent.machineId);
			}
		}

		protected override void Remove(InsectLegView entityView)
		{
			entityView.disabledComponent.isPartDisabled.StopNotify((Action<int, bool>)OnLegEnabled);
		}

		private void OnLegEnabled(int id, bool enabled)
		{
			InsectLegView insectLegView = default(InsectLegView);
			if (entityViewsDB.TryQueryEntityView<InsectLegView>(id, ref insectLegView))
			{
				int machineId = insectLegView.ownerComponent.machineId;
				CubeLeg leg = insectLegView.insectLegComponent as CubeLeg;
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
	}
}
