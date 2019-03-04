using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class LobbyGameStartView : MonoBehaviour, IInitialize
	{
		public UILabel status;

		public UILabel time;

		public Transform backboard;

		public Transform abuseMessage;

		protected int _lastSeconds;

		[Inject]
		internal LobbyGameStartPresenter presenter
		{
			private get;
			set;
		}

		[Inject]
		internal ICursorMode cursorMode
		{
			private get;
			set;
		}

		[Inject]
		internal ProfanityFilter profanityFilter
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		public LobbyGameStartView()
			: this()
		{
		}

		public void Open()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Close()
		{
			if (this.get_gameObject().get_activeSelf())
			{
				this.get_gameObject().SetActive(false);
			}
		}

		public virtual void SetTimer(float seconds)
		{
			if (this.get_gameObject().get_activeSelf())
			{
				int num = Mathf.RoundToInt(seconds);
				if (num != _lastSeconds)
				{
					time.set_text(Mathf.RoundToInt(seconds).ToString());
					PlayWaitTimeAudio(num);
					_lastSeconds = num;
				}
			}
		}

		protected void PlayWaitTimeAudio(int seconds)
		{
			if (seconds < 4)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.LobbyCountdown03to00), 0);
			}
			else if (seconds < 11)
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.LobbyCountdown10to04), 0);
			}
			else
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.LobbyCountdown30to11), 0);
			}
		}

		void IInitialize.OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		private void Start()
		{
			Open();
		}
	}
}
