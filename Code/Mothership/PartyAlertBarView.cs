using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class PartyAlertBarView : MonoBehaviour, IInitialize, IChainListener
	{
		public UILabel textLabel;

		public UIWidget greenColorWidget;

		public UIWidget redColorWidget;

		public float queueUpSoundPeriod = 1f;

		[Inject]
		internal PartyAlertBarController controller
		{
			private get;
			set;
		}

		public PartyAlertBarView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			controller.RegisterView(this);
			controller.SetSoundPeriod(queueUpSoundPeriod);
		}

		private void OnDestroy()
		{
			controller.UnregisterView();
		}

		private void Awake()
		{
			SetActive(active: false);
		}

		private void SetActive(bool active)
		{
			textLabel.get_gameObject().SetActive(active);
			greenColorWidget.get_gameObject().SetActive(active);
			redColorWidget.get_gameObject().SetActive(active);
		}

		public void Hide()
		{
			SetActive(active: false);
		}

		public void SetStateGreen(string msg)
		{
			textLabel.get_gameObject().SetActive(true);
			textLabel.set_text(msg);
			greenColorWidget.get_gameObject().SetActive(true);
			redColorWidget.get_gameObject().SetActive(false);
		}

		public void SetStateRed(string msg)
		{
			textLabel.get_gameObject().SetActive(true);
			textLabel.set_text(msg);
			greenColorWidget.get_gameObject().SetActive(false);
			redColorWidget.get_gameObject().SetActive(true);
		}

		public void Listen(object message)
		{
			controller.Listen(message);
		}
	}
}
