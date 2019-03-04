using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class TriggerUploadView : MonoBehaviour, IInitialize, IChainListener
	{
		public UILabel UploadFailedLabel;

		[Inject]
		internal RobotShopSubmissionController submissionController
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public event Action OnTriggerUploadRequestedEvent;

		public TriggerUploadView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			submissionController.SetupTriggerUploadView(this);
		}

		private void OnEnable()
		{
			NGUITools.BringForward(this.get_gameObject());
		}

		public void OnClick()
		{
			if (this.OnTriggerUploadRequestedEvent != null)
			{
				this.OnTriggerUploadRequestedEvent();
			}
		}

		public void ShowUploadError(string error)
		{
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			this.get_gameObject().SetActive(true);
			UploadFailedLabel.set_text(error);
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				if (buttonType == ButtonType.Confirm)
				{
					guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
					this.get_gameObject().SetActive(false);
				}
			}
		}
	}
}
