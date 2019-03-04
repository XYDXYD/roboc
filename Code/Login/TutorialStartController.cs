using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Login
{
	public class TutorialStartController : MonoBehaviour, IChainListener
	{
		public TutorialStartDialogue TutorialStartDialog;

		private bool _hasBeenCompleted;

		private bool _hasBeenSkipped;

		private bool _initialDialogShown;

		private bool _isInprogress;

		private LoginWebservicesRequestFactory _loginWebservicesRequestFactory;

		private Action _onTutorialStartFlowComplete;

		private Action<bool> _ShowLoadingScreenAction;

		public TutorialStartController()
			: this()
		{
		}

		void IChainListener.Listen(object message)
		{
			if (!(message is ButtonType))
			{
				return;
			}
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.Close)
			{
				Console.Log("tutorial start dialog: close button pressed");
				if (!_initialDialogShown)
				{
					_initialDialogShown = true;
					TutorialStartDialog.ShowTutorialExtraHintPanel();
				}
				else
				{
					SaveStatusFlagsAndProceed(skipped: true, inprogress: false);
				}
			}
			if (buttonType == ButtonType.Confirm)
			{
				Console.Log("tutorial start dialog: confirm button pressed");
				SaveStatusFlagsAndProceed(skipped: false, inprogress: true);
			}
		}

		public void Inject(LoginWebservicesRequestFactory loginWebservicesRequestFactory, Action<bool> showLoadingScreen)
		{
			_ShowLoadingScreenAction = showLoadingScreen;
			_loginWebservicesRequestFactory = loginWebservicesRequestFactory;
		}

		public void ShowTutorialStartDialog(Action onTutorialStartFlowComplete)
		{
			_onTutorialStartFlowComplete = onTutorialStartFlowComplete;
			TutorialStartDialog.ShowInitialPanel();
		}

		public IEnumerator RequestData(Action<ServiceBehaviour> onError)
		{
			ILoadTutorialStatusRequest loadRequest = _loginWebservicesRequestFactory.Create<ILoadTutorialStatusRequest>();
			TaskService<LoadTutorialStatusData> requestTask = loadRequest.AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				LoadTutorialStatusData result = requestTask.result;
				_hasBeenSkipped = result.skipped;
				_hasBeenCompleted = result.completed;
				_isInprogress = result.inProgress;
			}
			else
			{
				onError(requestTask.behaviour);
			}
		}

		public bool ShouldShowStartTutorialDialog()
		{
			return !_hasBeenSkipped && !_hasBeenCompleted && !_isInprogress;
		}

		private void SaveStatusFlagsAndProceed(bool skipped, bool inprogress)
		{
			TutorialStartDialog.Hide();
			_ShowLoadingScreenAction(obj: true);
			TaskRunner.get_Instance().Run(SaveStatus(skipped, inprogress));
		}

		private IEnumerator SaveStatus(bool skipped, bool inprogress)
		{
			IUpdateTutorialStatusRequest updateRequest = _loginWebservicesRequestFactory.Create<IUpdateTutorialStatusRequest>();
			updateRequest.Inject(new UpdateTutorialStatusData(inprogress, skipped, completed_: false));
			TaskService requestTask = updateRequest.AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				Console.Log("update tutorial status returned");
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(requestTask.behaviour);
			}
			Console.Log("Save completed succesfully");
			_ShowLoadingScreenAction(obj: false);
			OnFinished();
		}

		private void OnFinished()
		{
			TutorialStartDialog.Hide();
			_onTutorialStartFlowComplete();
			_onTutorialStartFlowComplete = null;
		}
	}
}
