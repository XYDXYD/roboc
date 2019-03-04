using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class CreatePrebuiltRobotCommand : IInjectableCommand<IMachineMap>, ICommand
	{
		private IMachineMap _prebuiltRobotMachineMap;

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
		private IGUIInputControllerMothership guiInputController
		{
			get;
			set;
		}

		[Inject]
		private ICubeInventory cubesInventory
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
			TaskRunner.get_Instance().Run((Func<IEnumerator>)CreatePrebuiltRobot);
		}

		public ICommand Inject(IMachineMap prebuiltRobotMachineMap)
		{
			_prebuiltRobotMachineMap = prebuiltRobotMachineMap;
			return this;
		}

		private IEnumerator CreatePrebuiltRobot()
		{
			loadingPresenter.NotifyLoading("CreatingPrebuiltRobot");
			MachineModel modelToSave = _prebuiltRobotMachineMap.BuildMachineLayoutModel();
			ICreatePrebuiltRobotRequest request = serviceFactory.Create<ICreatePrebuiltRobotRequest, CreatePrebuiltRobotDependency>(new CreatePrebuiltRobotDependency(modelToSave.GetCompresesdRobotData(), modelToSave.GetCompressedRobotColorData()));
			TaskService task = new TaskService(request);
			yield return task;
			if (!task.succeeded)
			{
				loadingPresenter.NotifyLoadingDone("CreatingPrebuiltRobot");
				switch (task.behaviour.errorCode)
				{
				case 18:
				{
					ILoadGarageSlotLimitRequest slotLimitRequest = serviceFactory.Create<ILoadGarageSlotLimitRequest>();
					TaskService<uint> slotTask = new TaskService<uint>(slotLimitRequest);
					yield return slotTask;
					if (!slotTask.succeeded)
					{
						ErrorWindow.ShowServiceErrorWindow(slotTask.behaviour);
					}
					else
					{
						ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarning"), StringTableBase<StringTable>.Instance.GetReplaceString("strNewRobotMaxSlots", "{MAX_SLOTS}", slotTask.result.ToString())));
					}
					break;
				}
				case 20:
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strWarning"), StringTableBase<StringTable>.Instance.GetString("strPreBuiltRobotLockedCubes")));
					break;
				default:
					ErrorWindow.ShowServiceErrorWindow(task.behaviour);
					break;
				}
			}
			else
			{
				yield return cubesInventory.RefreshAndWait();
				yield return garagePresenter.RefreshGarageData();
				garagePresenter.ShowGarageSlots();
				if (task.succeeded)
				{
					CreateNewRobotDependency createNewRobotDependency = new CreateNewRobotDependency(CreateNewRobotType.BODYBUILDER);
					createdNewRobotObservable.Dispatch(ref createNewRobotDependency);
				}
				loadingPresenter.NotifyLoadingDone("CreatingPrebuiltRobot");
				commandFactory.Build<SwitchToBuildModeCommand>().Inject(new SwitchWorldDependency("RC_BuildMode", _fastSwitch: false)).Execute();
			}
		}
	}
}
