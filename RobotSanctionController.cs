using Services.Web.Photon;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Text;

public class RobotSanctionController
{
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

	internal IEnumerator CheckAllRobotsSanctions(Action<RobotSanctionData[]> onSuccess = null)
	{
		loadingIconPresenter.NotifyLoading("CheckingRobotSanction");
		ICheckAllRobotsSanctionsRequest request = serviceFactory.Create<ICheckAllRobotsSanctionsRequest>();
		TaskService<RobotSanctionData[]> task = new TaskService<RobotSanctionData[]>(request);
		yield return new HandleTaskServiceWithError(task, delegate
		{
			loadingIconPresenter.NotifyLoading("CheckingRobotSanction");
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("CheckingRobotSanction");
		}).GetEnumerator();
		loadingIconPresenter.NotifyLoadingDone("CheckingRobotSanction");
		if (task.succeeded)
		{
			if (task.result != null)
			{
				for (int i = 0; i < task.result.Length; i++)
				{
					if (!task.result[i].Acknowledged)
					{
						HandleSanctionReceived(task.result[i]);
					}
				}
			}
			onSuccess?.Invoke(task.result);
		}
		else
		{
			ErrorWindow.ShowServiceErrorWindow(task.behaviour);
		}
	}

	internal IEnumerator CheckRobotSanction(string robotUniqueId = "", Action<RobotSanctionData> onSuccess = null, Action customActionOnClick = null)
	{
		loadingIconPresenter.NotifyLoading("CheckingRobotSanction");
		ICheckRobotSanctionRequest request = serviceFactory.Create<ICheckRobotSanctionRequest, string>(robotUniqueId);
		TaskService<RobotSanctionData> task = new TaskService<RobotSanctionData>(request);
		yield return new HandleTaskServiceWithError(task, delegate
		{
			loadingIconPresenter.NotifyLoading("CheckingRobotSanction");
		}, delegate
		{
			loadingIconPresenter.NotifyLoadingDone("CheckingRobotSanction");
		}).GetEnumerator();
		loadingIconPresenter.NotifyLoadingDone("CheckingRobotSanction");
		if (task.succeeded)
		{
			if (task.result != null && !task.result.Acknowledged)
			{
				HandleSanctionReceived(task.result, customActionOnClick);
			}
			onSuccess?.Invoke(task.result);
		}
		else
		{
			ErrorWindow.ShowServiceErrorWindow(task.behaviour);
		}
	}

	private void HandleSanctionReceived(RobotSanctionData sanction, Action customActionOnClick = null)
	{
		string key = "strRobotRemoved";
		string key2 = "strAcknowledge";
		string key3 = "strRobotRemovedDetails";
		Action okClicked = delegate
		{
			AcknowledgeSanction(sanction);
			if (customActionOnClick != null)
			{
				customActionOnClick();
			}
		};
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString(key3, "[ROBOT NAME]", sanction.RobotName));
		stringBuilder.Append("\n");
		if (sanction.Reason != string.Empty)
		{
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strReason", "[REASON]", sanction.Reason));
		}
		key = StringTableBase<StringTable>.Instance.GetString(key);
		key2 = StringTableBase<StringTable>.Instance.GetString(key2);
		GenericErrorData error = new GenericErrorData(key, stringBuilder.ToString(), key2, okClicked);
		ErrorWindow.ShowErrorWindow(error);
	}

	private void AcknowledgeSanction(RobotSanctionData sanction)
	{
		serviceFactory.Create<IAcknowledgeRobotSanctionRequest, RobotSanctionData>(sanction).Execute();
	}
}
