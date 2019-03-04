using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal class BattleLoadProgressSimulation : BattleLoadProgress, IInitialize, IWaitForFrameworkDestruction
	{
		private bool _started;

		[Inject]
		internal MachinePreloader MachinePreloader
		{
			private get;
			set;
		}

		protected override bool MapLoaded => true;

		protected override bool MachinesLoaded => MachinePreloader.IsComplete;

		protected override float MapLoadProgress => 1f;

		protected override float MachineLoadProgress => MachinePreloader.Progress;

		public void OnDependenciesInjected()
		{
			if (_started)
			{
				RequestLoadingProgress();
			}
		}

		public void Start()
		{
			if (base.CommandFactory != null)
			{
				RequestLoadingProgress();
			}
			_started = true;
			StartPollingProgress();
		}

		private void RequestLoadingProgress()
		{
			base.CommandFactory.Build<RequestLoadingProgressAllUsersCommand>().Execute();
		}

		public void OnFrameworkDestroyed()
		{
			if (_monoRunner != null)
			{
				_monoRunner.Dispose();
			}
		}
	}
}
