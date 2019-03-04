using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;

namespace Simulation.Hardware
{
	internal class ManaDrainingEngine : MultiEntityViewsEngine<ManaDrainNode, PowerBarNode>, ITickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private IPowerBarDataComponent _powerBar;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		private void OnActivateManaDraining(IManaDrainComponent sender, ManaDrainingActivationData data)
		{
			if (data.activate)
			{
				sender.drainRate += data.drainRate;
				sender.draining = true;
				UpdatePowerBarIncrement(data.machineId, active: false);
				return;
			}
			sender.drainRate -= data.drainRate;
			if (sender.drainRate <= 0f)
			{
				sender.drainRate = 0f;
				sender.draining = false;
				UpdatePowerBarIncrement(data.machineId, active: true);
			}
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<ManaDrainNode> val = entityViewsDB.QueryEntityViews<ManaDrainNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				ManaDrainNode manaDrainNode = val.get_Item(i);
				if (manaDrainNode.manaDrainComponent.draining)
				{
					if (_powerBar.powerValue > 0f)
					{
						_powerBar.powerValue -= manaDrainNode.manaDrainComponent.drainRate * deltaSec;
						EventManager.get_Instance().SetParameter("Cloak_Loop", "ManaLeft", 1f - _powerBar.powerPercent, null);
					}
					else
					{
						int value = manaDrainNode.get_ID();
						manaDrainNode.manaDrainComponent.draining = false;
						manaDrainNode.manaDrainComponent.manaDrained.Dispatch(ref value);
					}
				}
			}
		}

		private void UpdatePowerBarIncrement(int machineId, bool active)
		{
			MachineStunNode machineStunNode = default(MachineStunNode);
			if (entityViewsDB.TryQueryEntityView<MachineStunNode>(machineId, ref machineStunNode) && !machineStunNode.stunComponent.stunned)
			{
				_powerBar.progressiveIncrementActive = active;
			}
		}

		public void Ready()
		{
		}

		protected override void Add(ManaDrainNode entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				entityView.manaDrainComponent.activateManaDraining.subscribers += OnActivateManaDraining;
			}
		}

		protected override void Remove(ManaDrainNode entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				entityView.manaDrainComponent.activateManaDraining.subscribers -= OnActivateManaDraining;
			}
		}

		protected override void Add(PowerBarNode entityView)
		{
			_powerBar = entityView.powerBarDataComponent;
		}

		protected override void Remove(PowerBarNode entityView)
		{
			_powerBar = null;
		}
	}
}
