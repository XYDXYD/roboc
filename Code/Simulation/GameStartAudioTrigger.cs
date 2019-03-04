using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Utility;

namespace Simulation
{
	internal class GameStartAudioTrigger : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		[Inject]
		public IServiceRequestFactory requestFactory
		{
			private get;
			set;
		}

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

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			gameStartDispatcher.Register(OnGameStart);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(OnGameStart);
		}

		private void OnGameStart()
		{
			requestFactory.Create<IGetGameStartAudioRequest>().SetAnswer(new ServiceAnswer<string>(OnGetEventName)).Execute();
		}

		private void OnGetEventName(string name)
		{
			voManager.PlayVO(name);
		}

		private void OnGetNameFailed(ServiceBehaviour behaviour)
		{
			Console.LogWarning(behaviour.errorBody);
		}
	}
}
