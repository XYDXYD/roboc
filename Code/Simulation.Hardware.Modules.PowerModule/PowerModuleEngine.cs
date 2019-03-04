using RCNetwork.Events;
using Svelto.Command;
using Svelto.ECS;
using Svelto.IoC;

namespace Simulation.Hardware.Modules.PowerModule
{
	internal sealed class PowerModuleEngine : SingleEntityViewEngine<PowerModuleActivationNode>, IQueryingEntityViewEngine, IInitialize, IEngine
	{
		private PlayerIdDependency _dependency = new PlayerIdDependency(-1);

		private ActivatePowerModuleEffectClientCommand _activationCommand;

		[Inject]
		internal INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
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

		public void OnDependenciesInjected()
		{
			_activationCommand = commandFactory.Build<ActivatePowerModuleEffectClientCommand>();
		}

		protected override void Add(PowerModuleActivationNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.activationComponent.activate.subscribers += HandleModuleActivation;
			}
		}

		protected override void Remove(PowerModuleActivationNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.activationComponent.activate.subscribers -= HandleModuleActivation;
			}
		}

		private void HandleModuleActivation(IModuleActivationComponent arg1, int id)
		{
			PowerModuleActivationNode powerModuleActivationNode = default(PowerModuleActivationNode);
			if (entityViewsDB.TryQueryEntityView<PowerModuleActivationNode>(id, ref powerModuleActivationNode))
			{
				powerModuleActivationNode.confirmActivationComponent.activationConfirmed.Dispatch(ref id);
				_dependency.owner = powerModuleActivationNode.ownerComponent.ownerId;
				_activationCommand.Inject(_dependency).Execute();
				networkEventManager.SendEventToServerUnreliable(NetworkEvent.EnergyModuleActivated, _dependency);
			}
		}
	}
}
