using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class SwitchToBrawlCommand : ICommand
	{
		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			get;
			private set;
		}

		[Inject]
		private ICPUPower cpuPower
		{
			get;
			set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RequestDataAsTask);
		}

		public IEnumerator RequestDataAsTask()
		{
			if (cpuPower.TotalActualCPUCurrentRobot > cpuPower.MaxCpuPower)
			{
				yield break;
			}
			IGetBrawlParametersRequest service = serviceFactory.Create<IGetBrawlParametersRequest>();
			TaskService<GetBrawlRequestResult> task = new TaskService<GetBrawlRequestResult>(service);
			loadingIconPresenter.NotifyLoading("BrawlParameters");
			yield return task;
			loadingIconPresenter.NotifyLoadingDone("BrawlParameters");
			if (task.succeeded)
			{
				if (!task.result.BrawlParameters.IsLocked)
				{
					TryQueueForBrawlCommand tryQueueForBrawlCommand = commandFactory.Build<TryQueueForBrawlCommand>();
					tryQueueForBrawlCommand.Execute();
				}
				else
				{
					Console.Log("Brawl is locked");
				}
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(task.behaviour);
			}
		}
	}
}
