using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Mothership
{
	internal sealed class StartTutorialFromMothershipCommand : ICommand
	{
		private const string TEST_MODE_BUTTON = "Test Robot";

		private const string INVENTORY_BUTTON = "Inventory";

		private bool _robotReloaded;

		private bool _robotSwitched;

		private bool _TutorialBegunSuccess;

		private bool _TutorialStageResetSuccess;

		[CompilerGenerated]
		private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache0;

		[Inject]
		internal WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		private GenericInfoDisplay infoDisplay
		{
			get;
			set;
		}

		public void Execute()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			if (CheckTutorialHotkeys(infoDisplay))
			{
				loadingIconPresenter.NotifyLoading("startTutorial");
				SerialTaskCollection val = new SerialTaskCollection();
				val.Add(BeginTutorialRequest());
				val.Add(ResetTutorialStageRequest());
				val.Add(SwitchToTutorialRobotRequest());
				val.Add(ForceReloadCurrentMachineRequest());
				val.Add(ExecuteSwitchToMothershipCommand());
				TaskRunner.get_Instance().Run((IEnumerator)val);
			}
		}

		private IEnumerator BeginTutorialRequest()
		{
			IUpdateTutorialStatusRequest updateTutorialStatusRequest = serviceFactory.Create<IUpdateTutorialStatusRequest>();
			updateTutorialStatusRequest.Inject(new UpdateTutorialStatusData(inProgress_: true, skipped_: false, completed_: false));
			yield return new TaskService(updateTutorialStatusRequest);
			_TutorialBegunSuccess = true;
		}

		private ITask ResetTutorialStageRequest()
		{
			IUpdateTutorialStageRequest updateTutorialStageRequest = serviceFactory.Create<IUpdateTutorialStageRequest>();
			updateTutorialStageRequest.SetAnswer(new ServiceAnswer<bool>(delegate
			{
				_TutorialStageResetSuccess = true;
			}, ErrorWindow.ShowServiceErrorWindow));
			updateTutorialStageRequest.Inject(new UpdateTutorialStageData(0));
			return updateTutorialStageRequest.AsTask();
		}

		private IEnumerator ExecuteSwitchToMothershipCommand()
		{
			while (!_TutorialBegunSuccess && !_TutorialStageResetSuccess && !_robotReloaded && !_robotSwitched)
			{
				yield return null;
			}
			loadingIconPresenter.NotifyLoadingDone("startTutorial");
			worldSwitching.SwitchToMothershipStartTutorialReloadContext();
			yield return null;
		}

		private IEnumerator ForceReloadCurrentMachineRequest()
		{
			ILoadMachineRequest request = serviceFactory.Create<ILoadMachineRequest>();
			request.ForceClearCache();
			TaskService<LoadMachineResult> requestTask = request.AsTask();
			yield return requestTask;
			if (requestTask.succeeded)
			{
				_robotReloaded = true;
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(requestTask.behaviour);
			}
		}

		private IEnumerator SwitchToTutorialRobotRequest()
		{
			ISwitchToTutorialRobotRequest selectTutorialRobotRequest = serviceFactory.Create<ISwitchToTutorialRobotRequest>();
			yield return new TaskService(selectTutorialRobotRequest);
			_robotSwitched = true;
		}

		private static bool CheckTutorialHotkeys(GenericInfoDisplay infoDisplay)
		{
			string unboundButton = string.Empty;
			if (!CheckAllHotKeysBound(ref unboundButton))
			{
				GenericErrorData data = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strHotKeyNotBoundTitle"), string.Format(StringTableBase<StringTable>.Instance.GetString("strHotKeyNotBoundText"), unboundButton));
				infoDisplay.ShowInfoDialogue(data);
				return false;
			}
			return true;
		}

		private static bool CheckAllHotKeysBound(ref string unboundButton)
		{
			if (!TestHotkeyBound("Test Robot"))
			{
				unboundButton = "Test Robot";
				return false;
			}
			if (!TestHotkeyBound("Inventory"))
			{
				unboundButton = "Inventory";
				return false;
			}
			return true;
		}

		private static bool TestHotkeyBound(string action)
		{
			string inputActionKeyMap = InputRemapper.Instance.GetInputActionKeyMap(action);
			if (string.IsNullOrEmpty(inputActionKeyMap))
			{
				return false;
			}
			return true;
		}
	}
}
