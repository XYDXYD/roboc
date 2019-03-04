using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BrawlDetailsView : MonoBehaviour, IChainRoot, IInitialize
	{
		public GameObject titleLabel;

		public GameObject descriptionLabel;

		public GameObject mainBGImage;

		public GameObject smallMiddleImage;

		public GameObject rulesLabel;

		public GameObject backButton;

		public GameObject playButton;

		public Texture2D fallbackBrawlImage;

		internal SignalChain signalChain;

		[Inject]
		internal BrawlDetailsPresenter presenter
		{
			private get;
			set;
		}

		public BrawlDetailsView()
			: this()
		{
		}

		private void Awake()
		{
			this.get_gameObject().SetActive(false);
		}

		public void OnDependenciesInjected()
		{
			presenter.view = this;
		}

		public void Initialize()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			signalChain = new SignalChain(this.get_transform());
		}

		internal void OnBackButtonClicked()
		{
			presenter.OnBackButtonClicked();
		}

		internal void OnPlayButtonClicked()
		{
			presenter.OnPlayButtonClicked();
		}
	}
}
