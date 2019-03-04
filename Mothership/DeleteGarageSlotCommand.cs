using Services.Analytics;
using Services.Requests.Interfaces;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class DeleteGarageSlotCommand : IInjectableCommand<uint>, ICommand
	{
		private const uint NUMBER_OF_DEFAULT_GARAGE_SLOTS = 1u;

		private uint _garageSlotToDelete;

		private PlayerRobotStats _robotStats;

		[Inject]
		internal SellRobotPresenter sellRobotPresenter
		{
			get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal GaragePresenter garage
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineBuilder machineBuilder
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineMap machineMap
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public ICommand Inject(uint garageId)
		{
			_garageSlotToDelete = garageId;
			return this;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)ShowConfirmationDialog);
		}

		private IEnumerator ShowConfirmationDialog()
		{
			string dismantleRobotText = StringTableBase<StringTable>.Instance.GetString("strDisassembleRobotNoCubesInfo");
			sellRobotPresenter.SetStrings(StringTableBase<StringTable>.Instance.GetString("strDisassembleRobot"), dismantleRobotText, StringTableBase<StringTable>.Instance.GetString("strDismantle"));
			bool okClicked = false;
			yield return sellRobotPresenter.Show(delegate(bool result)
			{
				okClicked = result;
			});
			if (okClicked)
			{
				GetRobotStats();
				if ((long)garage.EditableGarageSlotCount > 1L)
				{
					yield return StartDeleteGarage();
				}
				else
				{
					yield return StartDismantleGarage();
				}
			}
		}

		private void GetRobotStats()
		{
			ILoadMovementStatsRequest service = serviceFactory.Create<ILoadMovementStatsRequest>();
			TaskService<MovementStats> taskService = new TaskService<MovementStats>(service);
			taskService.Execute();
			IGetDamageBoostRequest service2 = serviceFactory.Create<IGetDamageBoostRequest>();
			TaskService<DamageBoostDeserialisedData> taskService2 = new TaskService<DamageBoostDeserialisedData>(service2);
			taskService2.Execute();
			ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> taskService3 = request.AsTask();
			taskService3.Execute();
			TLOG_RobotStatsCalculator_Tencent tLOG_RobotStatsCalculator_Tencent = new TLOG_RobotStatsCalculator_Tencent(taskService.result, taskService2.result, taskService3.result.UseDecimalSystem);
			_robotStats = tLOG_RobotStatsCalculator_Tencent.CalculateRobotStats(machineMap, garage.CurrentRobotIdentifier.ToString(), garage.CurrentRobotMastery);
		}

		private IEnumerator LogPlayerDismantledRobot()
		{
			DismantledRobotDependency dependency = new DismantledRobotDependency(garage.CurrentRobotName, _robotStats);
			ILogPlayerDismantledRobotRequest service = analyticsRequestFactory.Create<ILogPlayerDismantledRobotRequest, DismantledRobotDependency>(dependency);
			TaskService task = new TaskService(service);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Dismantled Robot Log Request failed to send " + task.behaviour.errorBody);
			}
		}

		private IEnumerator StartDeleteGarage()
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			garage.ClearCurrentSlotCrfid();
			machineBuilder.RemoveAllCubes();
			IDeleteGarageRequest deleteGarageRequest = serviceFactory.Create<IDeleteGarageRequest, uint>(_garageSlotToDelete);
			TaskService deleteGarageTask = new TaskService(deleteGarageRequest);
			yield return new HandleTaskServiceWithError(deleteGarageTask, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			}).GetEnumerator();
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			if (deleteGarageTask.succeeded)
			{
				yield return LogPlayerDismantledRobot();
				yield return garage.RefreshGarageData();
				garage.HandleSlotDeleted(_garageSlotToDelete);
				garage.LoadAndBuildRobot();
			}
			else
			{
				yield return RefreshGarageDeleteFailRequest();
			}
		}

		private IEnumerator StartDismantleGarage()
		{
			loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			garage.ClearCurrentSlotCrfid();
			machineBuilder.RemoveAllCubes();
			IDismantleGarageRobotRequest dismantleGarageRequest = serviceFactory.Create<IDismantleGarageRobotRequest, uint>(_garageSlotToDelete);
			TaskService dismantleGarageTask = new TaskService(dismantleGarageRequest);
			yield return new HandleTaskServiceWithError(dismantleGarageTask, delegate
			{
				loadingPresenter.NotifyLoading("RobotShopLoadingScreen");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			}).GetEnumerator();
			loadingPresenter.NotifyLoadingDone("RobotShopLoadingScreen");
			if (dismantleGarageTask.succeeded)
			{
				yield return LogPlayerDismantledRobot();
				yield return garage.RefreshGarageData();
				garage.ShowGarageSlots();
			}
			else
			{
				yield return RefreshGarageDeleteFailRequest();
			}
		}

		private IEnumerator RefreshGarageDeleteFailRequest()
		{
			yield return garage.RefreshGarageData();
			garage.ShowGarageSlots();
			garage.LoadAndBuildRobot();
		}
	}
}
