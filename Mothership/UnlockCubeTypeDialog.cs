using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class UnlockCubeTypeDialog : MonoBehaviour, IInitialize, IChainListener
	{
		[SerializeField]
		private UILabel costLabel;

		[SerializeField]
		private GameObject robitsGO;

		[SerializeField]
		private GameObject cosmeticCreditsGO;

		[Inject]
		internal UnlockCubeTypePresenter unlockCubeTypePresenter
		{
			get;
			private set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public event Action confirmClickedCallback = delegate
		{
		};

		public event Action cancelClickedCallback = delegate
		{
		};

		public UnlockCubeTypeDialog()
			: this()
		{
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.Cancel:
					this.cancelClickedCallback();
					break;
				case ButtonType.Confirm:
					this.confirmClickedCallback();
					break;
				}
			}
		}

		internal void SetCost(uint cost, bool isUnlockedbyGC)
		{
			costLabel.set_text($"{cost:#,#}");
			robitsGO.SetActive(!isUnlockedbyGC);
			cosmeticCreditsGO.SetActive(isUnlockedbyGC);
		}

		void IInitialize.OnDependenciesInjected()
		{
			unlockCubeTypePresenter.RegisterDialog(this);
			this.get_gameObject().SetActive(false);
		}

		public void Show()
		{
			guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
			guiInputController.AddFloatingWidget(unlockCubeTypePresenter);
			this.get_gameObject().SetActive(true);
		}

		public void DismissDialog()
		{
			guiInputController.RemoveFloatingWidget(unlockCubeTypePresenter);
			guiInputController.UpdateShortCutMode();
			this.get_gameObject().SetActive(false);
		}
	}
}
