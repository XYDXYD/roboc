using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BrawlButton : MonoBehaviour, IInitialize
	{
		public GameObject[] bonusAdviceLabels;

		public GameObject lockedGameObject;

		public GameObject rosette;

		public GameObject lockMessage;

		public GameObject lockMessageMegabot;

		public GameObject title;

		public Texture2D fallbackBrawlImage;

		public GameObject normalStateBGImage;

		public GameObject highlightStateBGImage;

		public GameObject lockedStateBGImage;

		internal SignalChain signalChain;

		[Inject]
		private BrawlButtonPresenter presenter
		{
			get;
			set;
		}

		public BrawlButton()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void Initialize()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			signalChain = new SignalChain(this.get_transform());
		}

		private void OnClick()
		{
			presenter.OnDetailsClicked();
		}

		private void OnEnable()
		{
			if (presenter != null)
			{
				presenter.OnViewEnable();
			}
		}
	}
}
