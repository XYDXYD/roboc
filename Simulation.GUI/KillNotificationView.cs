using Svelto.IoC;
using UnityEngine;

namespace Simulation.GUI
{
	internal class KillNotificationView : MonoBehaviour, IInitialize
	{
		public const string KILL_ANIMATION = "Stream_Kill";

		public const string ASSIST_ANIMATION = "Stream_Assist";

		[SerializeField]
		private Animation _animation;

		[SerializeField]
		private GameObject _killGO;

		[SerializeField]
		private GameObject _assistGO;

		[SerializeField]
		private UILabel _killNameLabel;

		[SerializeField]
		private UILabel _assistNameLabel;

		[Tooltip("In seconds.")]
		[SerializeField]
		private float _timeBetweenNotifications = 0.5f;

		[Inject]
		public KillNotificationController killNotificationController
		{
			private get;
			set;
		}

		public Animation animationComponent => _animation;

		public bool killGOActive
		{
			set
			{
				_killGO.SetActive(value);
			}
		}

		public bool assistGOActive
		{
			set
			{
				_assistGO.SetActive(value);
			}
		}

		public string killName
		{
			set
			{
				_killNameLabel.set_text(value);
			}
		}

		public string assistName
		{
			set
			{
				_assistNameLabel.set_text(value);
			}
		}

		public float downtime => _timeBetweenNotifications;

		public KillNotificationView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			_killGO.SetActive(false);
			_assistGO.SetActive(false);
			killNotificationController.SetView(this);
		}
	}
}
