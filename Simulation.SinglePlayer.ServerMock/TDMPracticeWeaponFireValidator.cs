using Simulation.SinglePlayer.Rewards;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Simulation.SinglePlayer.ServerMock
{
	internal class TDMPracticeWeaponFireValidator : SinglePlayerWeaponFireValidator, IInitialize, IWaitForFrameworkDestruction
	{
		private TDMPracticeGamEndedObserver _TDMPracticeGamEndedObserver;

		[Inject]
		public TDMPracticeGamEndedObservable tdmPracticeGamEndedObservable
		{
			private get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			_TDMPracticeGamEndedObserver = new TDMPracticeGamEndedObserver(tdmPracticeGamEndedObservable);
			_TDMPracticeGamEndedObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleGameEnded(ref int teamId)
		{
			_acceptDamage = false;
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_TDMPracticeGamEndedObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
	}
}
