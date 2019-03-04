using Authentication;
using Robocraft.GUI.Iteration2;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Mothership
{
	internal class AddNewGarageSlotCommand : ICommand
	{
		private const string SEEN_FIRST_BAY_TUTORIAL_PROMPT = "SEEN_FIRST_BAY_TUTORIAL_PROMPT";

		[Inject]
		private GaragePresenter garage
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private ICommandFactory commandFactory
		{
			get;
			set;
		}

		[Inject]
		private CreatedNewRobotObservable_Tencent createdNewRobotObservable
		{
			get;
			set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run(Flow());
		}

		private IEnumerator Flow()
		{
			loadingIconPresenter.NotifyLoading("GarageSlotPurchaseLoadingIcon");
			ILoadGarageSlotLimitRequest request = serviceFactory.Create<ILoadGarageSlotLimitRequest>();
			TaskService<uint> task = new TaskService<uint>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("GarageSlotPurchaseLoadingIcon");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
			}).GetEnumerator();
			if (!task.succeeded)
			{
				yield break;
			}
			uint limit = task.result;
			if (garage.GarageSlotCount >= limit)
			{
				loadingIconPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strAddBay"), string.Format(StringTableBase<StringTable>.Instance.GetString("strGarageMaxSlots"), limit)));
				yield break;
			}
			IBuyGarageSlotRequest buyGarageSlot = serviceFactory.Create<IBuyGarageSlotRequest>();
			TaskService<BuyGarageSlotResponse> buyGarageSlotTask = new TaskService<BuyGarageSlotResponse>(buyGarageSlot);
			yield return new HandleTaskServiceWithError(buyGarageSlotTask, delegate
			{
				loadingIconPresenter.NotifyLoading("GarageSlotPurchaseLoadingIcon");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
			}).GetEnumerator();
			if (!buyGarageSlotTask.succeeded)
			{
				yield break;
			}
			ILoadAllCustomisationInfoRequest allCustomisationsLoadReq = serviceFactory.Create<ILoadAllCustomisationInfoRequest>();
			TaskService<AllCustomisationsResponse> allCustomisationsLoadTask = new TaskService<AllCustomisationsResponse>(allCustomisationsLoadReq);
			yield return allCustomisationsLoadTask;
			AllCustomisationsResponse responseData = allCustomisationsLoadTask.result;
			string defaultSkinName = string.Empty;
			foreach (CustomisationsEntry allSkinCustomisation in responseData.AllSkinCustomisations)
			{
				if (allSkinCustomisation.isDefault)
				{
					defaultSkinName = allSkinCustomisation.id;
				}
			}
			BuyGarageSlotResponse response = buyGarageSlotTask.result;
			GarageSlotDependency newGarageSlot = new GarageSlotDependency(response.garageIndex)
			{
				name = response.newGarageSlotName,
				numberCubes = 0u,
				uniqueSlotId = new UniqueSlotIdentifier(response.newGarageSlotUniqueIdentifier),
				movementCategories = new FasterList<ItemCategory>(),
				masteryLevel = response.masteryLevel,
				baySkinID = defaultSkinName
			};
			garage.AddNewGarageSlot(newGarageSlot);
			loadingIconPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
			bool startTutorial = false;
			string seenFistBayTutorialPromptKey = GetSeenFirstBayTutorialPromptKey();
			bool seenTutorialPrompt = PlayerPrefs.GetInt(seenFistBayTutorialPromptKey, 0) == 1;
			while (garage.isBusyBuilding)
			{
				yield return null;
			}
			if (!seenTutorialPrompt)
			{
				ILoadTutorialStatusRequest tutorialRequest = serviceFactory.Create<ILoadTutorialStatusRequest>();
				TaskService<LoadTutorialStatusData> tutorialTask = new TaskService<LoadTutorialStatusData>(tutorialRequest);
				loadingIconPresenter.NotifyLoading("GarageSlotPurchaseLoadingIcon");
				yield return new HandleTaskServiceWithError(tutorialTask, delegate
				{
					loadingIconPresenter.NotifyLoading("GarageSlotPurchaseLoadingIcon");
				}, delegate
				{
					loadingIconPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
				}).GetEnumerator();
				loadingIconPresenter.NotifyLoadingDone("GarageSlotPurchaseLoadingIcon");
				if (tutorialTask.succeeded)
				{
					LoadTutorialStatusData tutorialState = tutorialTask.result;
					if (!tutorialState.completed && !tutorialState.skipped)
					{
						yield return garage.AskForTutorial(delegate(GenericDialogChoice pChoice)
						{
							startTutorial = (pChoice == GenericDialogChoice.Yes);
						});
						PlayerPrefs.SetInt(seenFistBayTutorialPromptKey, 1);
					}
				}
			}
			CreateNewRobotDependency dep = new CreateNewRobotDependency(CreateNewRobotType.FROM_SCRATCH);
			createdNewRobotObservable.Dispatch(ref dep);
			if (startTutorial)
			{
				commandFactory.Build<StartTutorialFromMothershipCommand>().Execute();
				yield break;
			}
			SwitchWorldDependency dependency = new SwitchWorldDependency("RC_BuildMode", _fastSwitch: false);
			commandFactory.Build<SwitchToBuildModeCommand>().Inject(dependency).Execute();
		}

		private static string GetSeenFirstBayTutorialPromptKey()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SEEN_FIRST_BAY_TUTORIAL_PROMPT");
			stringBuilder.Append("/");
			stringBuilder.Append(User.Username);
			return stringBuilder.ToString();
		}
	}
}
