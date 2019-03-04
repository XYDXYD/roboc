using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class NewRobotOptionsView : MonoBehaviour, IInitialize, IChainListener
	{
		[Inject]
		private NewRobotOptionsPresenter presenter
		{
			get;
			set;
		}

		public NewRobotOptionsView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void Show(bool enable)
		{
			this.get_gameObject().SetActive(enable);
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				presenter.ButtonClicked((ButtonType)message);
			}
		}
	}
}
