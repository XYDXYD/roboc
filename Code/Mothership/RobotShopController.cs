using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal sealed class RobotShopController : IRobotShopController, IInitialize, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private byte[] _robotData;

		private readonly FasterList<GameObject> _robotGameObjects = new FasterList<GameObject>();

		private readonly FasterList<InstantiatedCube> _robotInstantiatedCubes = new FasterList<InstantiatedCube>();

		private RobotShopMainView _mainView;

		private MothershipPropActivator _motherShipPropActivator;

		private AccountRights _accountRights;

		private ColorPaletteData _colorPalette;

		private ServiceBehaviour _serviceErrorBehaviour;

		private const int _CUBES_TO_BUILD_IN_A_FRAME = 30;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeFactory cubeFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
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
			get;
			private set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool objectPool
		{
			get;
			private set;
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
		internal MachineEditorBatcher batcher
		{
			get;
			private set;
		}

		[Inject]
		internal MaxCosmeticCPUChangedObserver maxCosmeticCPUChangedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal RobotShopObserver observer
		{
			private get;
			set;
		}

		[Inject]
		internal IMothershipPropPresenter mothershipPropPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal RobotDimensionChangedObservable robotDimensionChangedObs
		{
			private get;
			set;
		}

		public event Action<Byte3, uint, byte> OnPreviewAddCubeAt = delegate
		{
		};

		public event Action<Byte3, uint> OnPreviewCubeRemovedAt = delegate
		{
		};

		public bool IsDeveloper()
		{
			return _accountRights.Developer;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			observer.OnShowRobotEvent += ShowRobot;
			observer.OnRobotDeletedEvent += CloseModelView;
			observer.OnRobotFeatureChangeEvent += CloseModelView;
			observer.OnHideRobotEvent += CloseModelView;
			maxCosmeticCPUChangedObserver.AddAction(new ObserverAction<uint>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			ILoadDefaultColorPaletteRequest loadDefaultColorPaletteRequest = serviceFactory.Create<ILoadDefaultColorPaletteRequest>();
			loadDefaultColorPaletteRequest.SetAnswer(new ServiceAnswer<ColorPaletteData>(delegate(ColorPaletteData defaultPalette)
			{
				_colorPalette = defaultPalette;
			}, delegate(ServiceBehaviour behaviour)
			{
				_serviceErrorBehaviour = behaviour;
			}));
			loadDefaultColorPaletteRequest.Execute();
			GetAccountRights();
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			if (_serviceErrorBehaviour != null)
			{
				if (_serviceErrorBehaviour.exceptionThrown != null)
				{
					Debug.LogException(_serviceErrorBehaviour.exceptionThrown);
				}
				else
				{
					Debug.LogException(new Exception("ServiceBehaviour returned from ILoadDefaultColorPaletteRequest was not null"));
				}
				ErrorWindow.ShowServiceErrorWindow(_serviceErrorBehaviour);
				_serviceErrorBehaviour = null;
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			observer.OnShowRobotEvent -= ShowRobot;
			observer.OnRobotDeletedEvent -= CloseModelView;
			observer.OnHideRobotEvent -= CloseModelView;
			maxCosmeticCPUChangedObserver.RemoveAction(new ObserverAction<uint>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public bool IsActive()
		{
			return _mainView.IsActive;
		}

		public void SetupMainView(RobotShopMainView mainView)
		{
			_mainView = mainView;
		}

		public void SetupModelView(RobotShopModelView modelView)
		{
			modelView.OnCloseRequestedEvent += CloseModelView;
		}

		public void SetupMotherShipPropActivator(MothershipPropActivator motherShipPropActivator)
		{
			_motherShipPropActivator = motherShipPropActivator;
		}

		public GUIShowResult Show()
		{
			_mainView.Show();
			observer.OnRobotShopOpened();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			observer.OnHideRobotShop();
			DestroyMachine();
			_mainView.Hide();
			garage.LoadAndBuildRobot();
			EnableBackground(enable: false);
			return true;
		}

		private void ShowRobot(RobotToShow robot)
		{
			machineBuilder.RemoveAllCubesGarage();
			_mainView.SwitchToModelView();
			mothershipPropPresenter.SetRobotShopName(robot.name);
			if (_robotData != robot.robotData)
			{
				DestroyMachine();
				_robotData = robot.robotData;
				MachineModel machineModel = new MachineModel(_robotData);
				machineModel.SetColorData(robot.colorData);
				TaskRunner.get_Instance().Run(BuildMachine(machineModel, robot.name, robot.robotid));
			}
		}

		private void CloseModelView()
		{
			_mainView.SwitchToShowroomView();
		}

		private void CloseModelView(bool refreshData)
		{
			_mainView.SwitchToShowroomView();
			if (refreshData)
			{
				observer.OnShowRobotShopList();
			}
		}

		private void GetAccountRights()
		{
			serviceFactory.Create<IGetAccountRightsRequest>().SetAnswer(new ServiceAnswer<AccountRights>(OnAccountRightsLoaded, delegate
			{
				OnAccountRightsLoaded(new AccountRights(moderator: false, developer: false, admin: false));
			})).Execute();
		}

		private void OnAccountRightsLoaded(AccountRights rights)
		{
			_accountRights = rights;
		}

		private IEnumerator BuildMachine(MachineModel machineModel, string robotName, long robotID)
		{
			loadingPresenter.NotifyLoading("RobotShopMachineBuild");
			yield return null;
			Byte3 smallestGrid = new Byte3(byte.MaxValue, byte.MaxValue, byte.MaxValue);
			Byte3 largestGrid = new Byte3(0, 0, 0);
			FasterListEnumerator<CubeData> enumerator = machineModel.DTO.cubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CubeData current = enumerator.get_Current();
					this.OnPreviewAddCubeAt(current.gridLocation, current.iID, current.rotationIndex);
					if (current.gridLocation.x < smallestGrid.x)
					{
						smallestGrid.x = current.gridLocation.x;
					}
					if (current.gridLocation.y < smallestGrid.y)
					{
						smallestGrid.y = current.gridLocation.y;
					}
					if (current.gridLocation.z < smallestGrid.z)
					{
						smallestGrid.z = current.gridLocation.z;
					}
					if (current.gridLocation.x > largestGrid.x)
					{
						largestGrid.x = current.gridLocation.x;
					}
					if (current.gridLocation.y > largestGrid.y)
					{
						largestGrid.y = current.gridLocation.y;
					}
					if (current.gridLocation.z > largestGrid.z)
					{
						largestGrid.z = current.gridLocation.z;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			Byte3 machineSizeCells = largestGrid - smallestGrid;
			float machineCentreX = (float)(int)machineSizeCells.x / 2f;
			float machineCentreZ = (float)(int)machineSizeCells.z / 2f;
			float bayCentreX = 24.5f;
			float bayCentreZ = 24.5f;
			float machineCentreCurrentX = (float)(int)smallestGrid.x + machineCentreX;
			float machineCentreCurrentZ = (float)(int)smallestGrid.z + machineCentreZ;
			int displacementToApplyX = (int)(bayCentreX - machineCentreCurrentX);
			int displacementToApplyZ = (int)(bayCentreZ - machineCentreCurrentZ);
			bool boundValid = false;
			Bounds bounds = default(Bounds);
			CubeNodeInstance cubeNodeInstance = new CubeNodeInstance();
			int count = 0;
			FasterListEnumerator<CubeData> enumerator2 = machineModel.DTO.cubes.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					CubeData cell = enumerator2.get_Current();
					cell.gridLocation.x = (byte)(cell.gridLocation.x + displacementToApplyX);
					cell.gridLocation.z = (byte)(cell.gridLocation.z + displacementToApplyZ);
					InstantiatedCube instantiatedCube = new InstantiatedCube(cubeNodeInstance, cubeList.CubeTypeDataOf(cell.iID).cubeData, cell.gridLocation, cell.rotationIndex)
					{
						paletteIndex = cell.paletteIndex
					};
					_robotInstantiatedCubes.Add(instantiatedCube);
					GameObject goCube = cubeFactory.BuildCube(cell.iID, GridScaleUtility.GridToWorld(cell.gridLocation, TargetType.Player), CubeData.IndexToQuat(cell.rotationIndex), TargetType.Player);
					goCube.GetComponent<CubeColorUpdater>().SetColor(_colorPalette[cell.paletteIndex]);
					_robotGameObjects.Add(goCube);
					Collider[] colliders = goCube.GetComponentsInChildren<Collider>();
					Collider[] array = colliders;
					foreach (Collider val in array)
					{
						if (boundValid)
						{
							bounds.Encapsulate(val.get_bounds());
						}
						else
						{
							Bounds bounds2 = val.get_bounds();
							bounds.set_center(bounds2.get_center());
							Bounds bounds3 = val.get_bounds();
							bounds.set_extents(bounds3.get_extents());
							Bounds bounds4 = val.get_bounds();
							bounds.set_max(bounds4.get_max());
							Bounds bounds5 = val.get_bounds();
							bounds.set_min(bounds5.get_min());
							Bounds bounds6 = val.get_bounds();
							bounds.set_size(bounds6.get_size());
							boundValid = true;
						}
					}
					count++;
					if (30 <= count)
					{
						count = 0;
						yield return null;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			RobotDimensionDependency dep = new RobotDimensionDependency(bounds.get_min(), bounds.get_max());
			robotDimensionChangedObs.Dispatch(ref dep);
			observer.OnRobotBuilt(robotName);
			batcher.StartBatching(_robotInstantiatedCubes, _robotGameObjects);
			if (Math.Abs(displacementToApplyX) > 1 || Math.Abs(displacementToApplyZ) > 1)
			{
				UpdateShopRobotOffsetDependency param = new UpdateShopRobotOffsetDependency();
				param.cubesOffsetX = displacementToApplyX;
				param.cubesOffsetZ = displacementToApplyZ;
				FasterReadOnlyList<CubeData> cubes = machineModel.DTO.cubes;
				if (cubes.get_Count() == 0)
				{
					RemoteLogger.Error("Error applying offset to a shop robot", "the robot had no cube data in MachineModel.DTO.cubes!", null);
				}
				else
				{
					CubeData cubeData = cubes.get_Item(0);
					param.expectedLocationFirstCubeX = cubeData.gridLocation.x;
					param.expectedLocationFirstCubeY = cubeData.gridLocation.y;
					param.expectedLocationFirstCubeZ = cubeData.gridLocation.z;
					param.robotId = robotID;
					loadingPresenter.NotifyLoading("UpdateShotRobot");
					IUpdateShotRobotOffsetRequest updateShotRobotOffsetRequest = serviceFactory.Create<IUpdateShotRobotOffsetRequest, UpdateShopRobotOffsetDependency>(param);
					updateShotRobotOffsetRequest.SetAnswer(new ServiceAnswer(delegate
					{
						observer.OnRobotInvalidated();
						loadingPresenter.NotifyLoadingDone("UpdateShotRobot");
					}, delegate(ServiceBehaviour behaviour)
					{
						_serviceErrorBehaviour = behaviour;
						RemoteLogger.Error("Error applying offset to a shop robot", "robot id: " + param.robotId, null);
						loadingPresenter.NotifyLoadingDone("UpdateShotRobot");
					}));
					updateShotRobotOffsetRequest.Execute();
				}
			}
			loadingPresenter.NotifyLoadingDone("RobotShopMachineBuild");
		}

		private void DestroyMachine()
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			bool flag = _robotGameObjects.get_Count() > 0;
			for (int i = 0; i < _robotGameObjects.get_Count(); i++)
			{
				GameObject val = _robotGameObjects.get_Item(i);
				objectPool.Recycle(val, val.get_name());
				val.SetActive(false);
			}
			FasterListEnumerator<InstantiatedCube> enumerator = _robotInstantiatedCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.get_Current();
					this.OnPreviewCubeRemovedAt(current.gridPos, current.persistentCubeData.cubeType.ID);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			_robotInstantiatedCubes.FastClear();
			_robotGameObjects.FastClear();
			_robotData = null;
			if (flag)
			{
				batcher.ClearResources();
			}
		}

		public void EnableBackground(bool enable)
		{
		}

		private void UpdateRobotList(ref uint maxCosmeticCPU)
		{
			if (IsActive())
			{
				CloseModelView(refreshData: true);
			}
		}
	}
}
