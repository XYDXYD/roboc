using Svelto.Context;
using Svelto.IoC;
using System;

namespace Mothership
{
	internal class BuildInputLock : IInitialize, IWaitForFrameworkDestruction
	{
		private bool _mothershipReady;

		private bool _locked;

		[Inject]
		internal MothershipReadyObserver mothershipReadyObserver
		{
			private get;
			set;
		}

		public bool Locked => !_mothershipReady || _locked;

		public void OnDependenciesInjected()
		{
			mothershipReadyObserver.AddAction((Action)HandleMothershipReady);
		}

		public void OnFrameworkDestroyed()
		{
			mothershipReadyObserver.RemoveAction((Action)HandleMothershipReady);
		}

		private void HandleMothershipReady()
		{
			_mothershipReady = true;
		}

		internal void SetLocked(bool locked)
		{
			_locked = locked;
		}
	}
}
