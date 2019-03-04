using LobbyServiceLayer;
using Svelto.Context;
using Svelto.IoC;

namespace Mothership
{
	internal class BattleLoadProgressMothership : BattleLoadProgress, IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		internal BattleFoundObserver BattleFoundObserver
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching WorldSwitching
		{
			private get;
			set;
		}

		protected override bool MapLoaded => WorldSwitching.IsComplete;

		protected override bool MachinesLoaded => false;

		protected override float MapLoadProgress => WorldSwitching.Progress;

		protected override float MachineLoadProgress => 0f;

		public void OnDependenciesInjected()
		{
			BattleFoundObserver.EnterBattleEvent += OnEnterBattle;
		}

		public void OnFrameworkDestroyed()
		{
			BattleFoundObserver.EnterBattleEvent -= OnEnterBattle;
			if (_monoRunner != null)
			{
				_monoRunner.Dispose();
			}
		}

		private void OnEnterBattle(EnterBattleDependency enterBattleDependency)
		{
			StartPollingProgress();
		}
	}
}
