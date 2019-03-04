using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class SellRobotDialog : MonoBehaviour, IInitialize, IChainListener
	{
		[SerializeField]
		private UILabel titleLabel;

		[SerializeField]
		private UILabel bodyLabel;

		[SerializeField]
		private UILabel yesButtonLabel;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal SellRobotPresenter sellRobotPresenter
		{
			private get;
			set;
		}

		public event Action sellSelectedGarageSlotRobot = delegate
		{
		};

		public SellRobotDialog()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			sellRobotPresenter.RegisterSellRobotDialog(this);
			this.get_gameObject().SetActive(false);
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.Cancel:
					DismissDialog();
					break;
				case ButtonType.Confirm:
					HandleOnConfirm();
					break;
				}
			}
		}

		public void Show()
		{
			guiInputController.SetShortCutMode(ShortCutMode.OnlyEsc);
			guiInputController.AddFloatingWidget(sellRobotPresenter);
			this.get_gameObject().SetActive(true);
		}

		public void DismissDialog()
		{
			guiInputController.RemoveFloatingWidget(sellRobotPresenter);
			guiInputController.UpdateShortCutMode();
			this.get_gameObject().SetActive(false);
		}

		internal void SetLabels(string titleStr, string bodyStr, string yesButtonStr)
		{
			titleLabel.set_text(titleStr);
			bodyLabel.set_text(bodyStr);
			yesButtonLabel.set_text(yesButtonStr);
		}

		private void HandleOnConfirm()
		{
			this.sellSelectedGarageSlotRobot();
			DismissDialog();
		}
	}
}
