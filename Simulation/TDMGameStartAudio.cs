using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal class TDMGameStartAudio : IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		public VOManager voManager
		{
			private get;
			set;
		}

		[Inject]
		public GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			gameStartDispatcher.Register(OnGameStart);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(OnGameStart);
		}

		private void OnGameStart()
		{
			voManager.PlayVO(AudioFabricEvent.Name(AudioFabricGameEvents.VO_TDM_Intro));
		}
	}
}
