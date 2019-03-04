using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class OpenNewGarageCommand : ICommand
	{
		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingPresenter
		{
			get;
			set;
		}

		[Inject]
		private GaragePresenter garagePresenter
		{
			get;
			set;
		}

		[Inject]
		private IGUIInputControllerMothership guiInputController
		{
			get;
			set;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)CheckGarageSlotLimit);
		}

		private IEnumerator CheckGarageSlotLimit()
		{
			ILoadGarageSlotLimitRequest request = serviceFactory.Create<ILoadGarageSlotLimitRequest>();
			TaskService<uint> task = new TaskService<uint>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingPresenter.NotifyLoading("MaxGarageSlots");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("MaxGarageSlots");
			}).GetEnumerator();
			if (!task.succeeded)
			{
				loadingPresenter.NotifyLoadingDone("MaxGarageSlots");
				yield break;
			}
			uint slotLimit = task.result;
			if (garagePresenter.GarageSlotCount >= slotLimit)
			{
				loadingPresenter.NotifyLoadingDone("MaxGarageSlots");
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarning"), StringTableBase<StringTable>.Instance.GetReplaceString("strNewRobotMaxSlots", "{MAX_SLOTS}", slotLimit.ToString())));
			}
			else
			{
				guiInputController.ShowScreen(GuiScreens.NewRobotOptionsScreen);
			}
		}
	}
}
