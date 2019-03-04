using Mothership;
using SinglePlayerCampaign.GUI.Mothership;
using SinglePlayerServiceLayer;
using SinglePlayerServiceLayer.Requests.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;

public class TryQueueForSinglePlayerCampaign : IInjectableCommand<SelectedCampaignToStart>, ICommand
{
	private SelectedCampaignToStart _selectedCampaign;

	[Inject]
	internal ISinglePlayerRequestFactory singlePlayerRequestFactory
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
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal DesiredGameMode desiredGameMode
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

	public void Execute()
	{
		desiredGameMode.DesiredMode = LobbyType.Solo;
		TaskRunner.get_Instance().Run(CheckOverridesAndEnterCampaign());
	}

	public ICommand Inject(SelectedCampaignToStart dependency)
	{
		_selectedCampaign = dependency;
		return this;
	}

	private IEnumerator CheckOverridesAndEnterCampaign()
	{
		loadingIconPresenter.NotifyLoading("CampaignRobotValidation");
		yield return CampaignJsonOverridesPreloader.PreloadCampaignOverrides(clearCache: true, loadingIconPresenter, _selectedCampaign);
		IValidateCampaignRobotRequest request = singlePlayerRequestFactory.Create<IValidateCampaignRobotRequest, string>(_selectedCampaign.CampaignID);
		TaskService task = new TaskService(request);
		yield return task;
		loadingIconPresenter.NotifyLoadingDone("CampaignRobotValidation");
		if (!task.succeeded)
		{
			string @string = StringTableBase<StringTable>.Instance.GetString("strUnableToValidatePlayerRobotForCampaigns");
			switch (task.behaviour.errorCode)
			{
			case 1:
				@string = StringTableBase<StringTable>.Instance.GetString("strCampaignValidationFailCPUTooHigh");
				break;
			case 2:
				@string = StringTableBase<StringTable>.Instance.GetString("strCampaignValidationFailCPUTooLow");
				break;
			case 4:
			case 5:
				@string = StringTableBase<StringTable>.Instance.GetString("strCampaignValidationFailCubeNotAllowed");
				break;
			case 3:
				@string = StringTableBase<StringTable>.Instance.GetString("strCampaignValidationFailTier");
				break;
			}
			ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strCampaignValidationFailedHeader"), @string, StringTableBase<StringTable>.Instance.GetString("strOK")));
		}
		else
		{
			commandFactory.Build<StartCampaignCommand>().Inject(_selectedCampaign).Execute();
		}
	}
}
