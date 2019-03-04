using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;

namespace Mothership
{
	internal sealed class SkipTutorialCommand : ICommand
	{
		private bool _exitedTutorial;

		private bool _robotReloaded;

		private bool _robotSwitched;

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

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			loadingIconPresenter.NotifyLoading("ReturningToMothershipFromTutorial");
			SerialTaskCollection val = new SerialTaskCollection();
			val.Add(SwitchToPreviousRobotFromTutorialRobot());
			val.Add(ExitTutorialRequest());
			val.Add(ForceReloadCurrentMachineRequest());
			val.Add(ExecuteSwitchToMothershipCommand());
			TaskRunner.get_Instance().Run((IEnumerator)val);
		}

		private IEnumerator ExecuteSwitchToMothershipCommand()
		{
			while (!_robotSwitched && !_exitedTutorial && !_robotReloaded)
			{
				yield return null;
			}
			loadingIconPresenter.NotifyLoadingDone("ReturningToMothershipFromTutorial");
			worldSwitching.SwitchToLastMothershipGameMode(fastSwitch: true);
		}

		private IEnumerator ExitTutorialRequest()
		{
			IUpdateTutorialStatusRequest updateTutorialStatusRequest = serviceFactory.Create<IUpdateTutorialStatusRequest>();
			updateTutorialStatusRequest.Inject(new UpdateTutorialStatusData(inProgress_: false, skipped_: true, completed_: false));
			yield return new TaskService(updateTutorialStatusRequest);
			_exitedTutorial = true;
		}

		private IEnumerator ForceReloadCurrentMachineRequest()
		{
			ILoadMachineRequest request = serviceFactory.Create<ILoadMachineRequest>();
			request.ForceClearCache();
			TaskService<LoadMachineResult> serviceTask = request.AsTask();
			yield return serviceTask;
			if (serviceTask.succeeded)
			{
				_robotReloaded = true;
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(serviceTask.behaviour);
			}
		}

		private IEnumerator SwitchToPreviousRobotFromTutorialRobot()
		{
			ISwitchToPreviousRobotIfNecessaryRequest selectTutorialRobotRequest = serviceFactory.Create<ISwitchToPreviousRobotIfNecessaryRequest>();
			TaskService task = new TaskService(selectTutorialRobotRequest);
			yield return task;
			if (task.behaviour == null)
			{
				_robotSwitched = true;
			}
		}
	}
}
