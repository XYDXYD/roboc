using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class PriceChangeDialogView : MonoBehaviour, IChainRoot, IChainListener
	{
		[SerializeField]
		private UILabel _dialogHeader;

		[SerializeField]
		private UILabel _dialogBody;

		[Inject]
		internal PriceChangeDialogPresenter presenter
		{
			private get;
			set;
		}

		public PriceChangeDialogView()
			: this()
		{
		}

		public void Listen(object message)
		{
			presenter.HandleMessage(message);
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public bool IsActive()
		{
			return this.get_gameObject().get_activeInHierarchy();
		}

		public void UpdateHeaderText(string text)
		{
			_dialogHeader.set_text(text);
		}

		public void UpdateBodyText(string text)
		{
			_dialogBody.set_text(text);
		}
	}
}
