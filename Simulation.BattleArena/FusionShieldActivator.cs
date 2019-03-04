using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;

namespace Simulation.BattleArena
{
	internal sealed class FusionShieldActivator : IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private bool _playerCanSeeTheMap;

		[Inject]
		internal FusionShieldsObserver shieldsObserver
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching _worldSwitch
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			shieldsObserver.RegisterShieldStateChanged(SetShieldPowerState);
			_worldSwitch.OnWorldJustSwitched += OnWorldJustSwitched;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			shieldsObserver.UnregisterShieldStateChanged(SetShieldPowerState);
			_worldSwitch.OnWorldJustSwitched -= OnWorldJustSwitched;
		}

		private void OnWorldJustSwitched(WorldSwitchMode obj)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			_playerCanSeeTheMap = true;
			FasterReadOnlyList<FusionShieldEntityView> val = entityViewsDB.QueryEntityViews<FusionShieldEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				IFusionShieldActivable activableComponent = val.get_Item(i).activableComponent;
				activableComponent.visualState = activableComponent.powerState;
			}
		}

		private void SetShieldPowerState(int teamId, bool fullPower)
		{
			FusionShieldEntityView fusionShieldEntityView = entityViewsDB.QueryEntityView<FusionShieldEntityView>(teamId);
			fusionShieldEntityView.activableComponent.powerState = fullPower;
			if (_playerCanSeeTheMap)
			{
				fusionShieldEntityView.activableComponent.visualState = fullPower;
			}
		}

		public void Ready()
		{
		}
	}
}
