using Services.Analytics;
using Services.Requests.Interfaces;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class TLOG_LeftEditModeTracker_Tencent : IInitialize, IWaitForFrameworkDestruction
	{
		private static CreateNewRobotDependency _createdNewRobotDependency;

		private TLOG_RobotStatsCalculator_Tencent _robotStatsCalculator;

		[Inject]
		private WorldSwitching worldSwitching
		{
			get;
			set;
		}

		[Inject]
		private CreatedNewRobotObserver_Tencent createdNewRobotObserver
		{
			get;
			set;
		}

		[Inject]
		private IMachineMap machineMap
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
		private GaragePresenter garagePresenter
		{
			get;
			set;
		}

		[Inject]
		private ICubeList cubeList
		{
			get;
			set;
		}

		[Inject]
		private IEditorCubeFactory cubeFactory
		{
			get;
			set;
		}

		[Inject]
		private SwitchingToTestModeObserver testModeObserver
		{
			get;
			set;
		}

		[Inject]
		private ITutorialController tutorialController
		{
			get;
			set;
		}

		[Inject]
		private IAnalyticsRequestFactory analyticsRequestFactory
		{
			get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			worldSwitching.OnWorldJustSwitched += CheckIfLogPlayerRobot;
			createdNewRobotObserver.AddAction(new ObserverAction<CreateNewRobotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnFrameworkDestroyed()
		{
			worldSwitching.OnWorldJustSwitched -= CheckIfLogPlayerRobot;
			createdNewRobotObserver.RemoveAction(new ObserverAction<CreateNewRobotDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			testModeObserver.RemoveAction((Action)EnteredTestModeFromEditMode);
		}

		public void LoadData()
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
			_robotStatsCalculator = new TLOG_RobotStatsCalculator_Tencent(taskService.result, taskService2.result, taskService3.result.UseDecimalSystem);
			_robotStatsCalculator.SetCubeData(cubeList, cubeFactory);
		}

		private void CheckIfLogPlayerRobot(WorldSwitchMode currentMode)
		{
			if (!tutorialController.IsActive())
			{
				if (_createdNewRobotDependency == null && currentMode == WorldSwitchMode.BuildMode)
				{
					worldSwitching.OnWorldIsSwitching.Add(LogPlayerEditedRobot());
					testModeObserver.AddAction((Action)EnteredTestModeFromEditMode);
				}
				if (_createdNewRobotDependency != null && worldSwitching.SwitchingFrom == WorldSwitchMode.BuildMode && worldSwitching.SwitchingTo != 0)
				{
					TaskRunner.get_Instance().Run((Func<IEnumerator>)LogPlayerCreatedNewRobot);
				}
			}
		}

		private void EnteredTestModeFromEditMode()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LogPlayerEditedRobot);
		}

		private IEnumerator LogPlayerEditedRobot()
		{
			if (!tutorialController.IsActive())
			{
				EditedRobotDependency dependency = new EditedRobotDependency(robotStats_: _robotStatsCalculator.CalculateRobotStats(machineMap, garagePresenter.CurrentRobotIdentifier.ToString(), garagePresenter.CurrentRobotMastery), robotName_: garagePresenter.CurrentRobotName);
				ILogPlayerEditedRobotRequest service = analyticsRequestFactory.Create<ILogPlayerEditedRobotRequest, EditedRobotDependency>(dependency);
				TaskService task = new TaskService(service);
				yield return task;
				if (!task.succeeded)
				{
					Console.LogError("Edited Robot Log Request failed to send " + task.behaviour.errorBody);
				}
				testModeObserver.RemoveAction((Action)EnteredTestModeFromEditMode);
			}
		}

		private IEnumerator LogPlayerCreatedNewRobot()
		{
			CreatedNewRobotDependency dependency = new CreatedNewRobotDependency(robotStats_: (_createdNewRobotDependency.robotCubeData == null) ? _robotStatsCalculator.CalculateRobotStats(machineMap, garagePresenter.CurrentRobotIdentifier.ToString(), garagePresenter.CurrentRobotMastery) : _robotStatsCalculator.CalculateRobotStats(_createdNewRobotDependency.robotCubeData, garagePresenter.CurrentRobotIdentifier.ToString(), garagePresenter.CurrentRobotMastery), robotName_: garagePresenter.CurrentRobotName, createType_: (int)_createdNewRobotDependency.createNewRobotType, robotCount_: garagePresenter.EditableGarageSlotCount);
			ILogPlayerCreatedNewRobotRequest service = analyticsRequestFactory.Create<ILogPlayerCreatedNewRobotRequest, CreatedNewRobotDependency>(dependency);
			TaskService task = new TaskService(service);
			yield return task;
			if (!task.succeeded)
			{
				Console.LogError("Created Robot Log Request failed to send " + task.behaviour.errorBody);
			}
			_createdNewRobotDependency = null;
		}

		private void CachePlayerCreatedNewRobot(ref CreateNewRobotDependency dep)
		{
			_createdNewRobotDependency = dep;
			if (dep.createNewRobotType == CreateNewRobotType.FROM_CRF || dep.createNewRobotType == CreateNewRobotType.GARAGE_COPY)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)LogPlayerCreatedNewRobot);
			}
		}
	}
}
