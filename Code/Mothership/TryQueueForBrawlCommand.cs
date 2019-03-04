using Services;
using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class TryQueueForBrawlCommand : ICommand
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

		public void Execute()
		{
			desiredGameMode.DesiredMode = LobbyType.Brawl;
			loadingIconPresenter.NotifyLoading("BrawlParameters");
			TaskRunner.get_Instance().Run(CheckBrawlOverridesAndEnterBrawl());
		}

		private IEnumerator CheckBrawlOverridesAndEnterBrawl()
		{
			IEnumerator enumerator = BrawlOverridePreloader.CheckVersionChange(serviceFactory);
			yield return enumerator;
			BrawlOverridePreloader.VersionCheckResult result = enumerator.Current as BrawlOverridePreloader.VersionCheckResult;
			if (result.failBehaviour == null)
			{
				yield return OnBrawlVersionChecked(result.versionChanged);
			}
			else
			{
				OnVersionCheckFailed(result.failBehaviour);
			}
		}

		private IEnumerator OnBrawlVersionChecked(bool changed)
		{
			loadingIconPresenter.NotifyLoadingDone("BrawlParameters");
			IEnumerator enumerator = BrawlOverridePreloader.PreloadBrawlOverrides(changed, loadingIconPresenter);
			yield return enumerator;
			if ((bool)enumerator.Current)
			{
				loadingIconPresenter.NotifyLoading("BrawlValidation");
				IValidateBrawlRobotRequest validateBrawlRobotReq = serviceFactory.Create<IValidateBrawlRobotRequest>();
				validateBrawlRobotReq.ClearCache();
				TaskService<ValidateRobotForBrawlResult> validateBrawlRobotTS = validateBrawlRobotReq.AsTask();
				yield return validateBrawlRobotTS;
				loadingIconPresenter.NotifyLoadingDone("BrawlValidation");
				if (validateBrawlRobotTS.succeeded)
				{
					OnRobotValidationForPlayComplete(validateBrawlRobotTS.result);
					yield break;
				}
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strErrorValidateBrawlRequestErrorTitle"), StringTableBase<StringTable>.Instance.GetString("strErrorValidateBrawlRequestErrorBody")));
				Console.LogError("validate failed for some reason.");
			}
		}

		private void OnVersionCheckFailed(ServiceBehaviour sb)
		{
			loadingIconPresenter.NotifyLoadingDone("BrawlParameters");
			ErrorWindow.ShowErrorWindow(new GenericErrorData(sb.errorTitle, sb.errorBody));
		}

		private void SwitchToDetailsScreen()
		{
			guiInputController.ToggleScreen(GuiScreens.BrawlDetails);
		}

		private void OnRobotValidationForPlayComplete(ValidateRobotForBrawlResult validationresponse)
		{
			if (validationresponse == ValidateRobotForBrawlResult.CanPlayBrawl)
			{
				guiInputController.ToggleScreen(GuiScreens.BattleCountdown);
				return;
			}
			string key = "strBrawlValidationFailedHeader";
			string key2 = string.Empty;
			switch (validationresponse)
			{
			case ValidateRobotForBrawlResult.BrawlModeLocked:
				key2 = "strBrawlValidationFailedBecauseNowLocked";
				break;
			case ValidateRobotForBrawlResult.CPUTooHigh:
				key2 = "strBrawlValidationFailedBecauseCPUTooHigh";
				break;
			case ValidateRobotForBrawlResult.CPUTooLow:
				key2 = "strBrawlValidationFailedBecauseCPUTooLow";
				break;
			case ValidateRobotForBrawlResult.CubeIDNotAllowed:
			case ValidateRobotForBrawlResult.CubeTypeNotAllowed:
				key2 = "strBrawlValidationFailedBecauseCubeNotAllowed";
				break;
			case ValidateRobotForBrawlResult.PhotonServerErrorOccured:
				key = "strErrorValidateBrawlRequestErrorTitle";
				key2 = "strBrawlValidationFailedBecausePhotonFailed";
				break;
			}
			ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString(key), StringTableBase<StringTable>.Instance.GetString(key2), StringTableBase<StringTable>.Instance.GetString("strOK"), StringTableBase<StringTable>.Instance.GetString("strBrawlSeeDetails"), delegate
			{
				Console.Log("failed to enter queue because robot does not meet requirements");
			}, SwitchToDetailsScreen));
		}
	}
}
