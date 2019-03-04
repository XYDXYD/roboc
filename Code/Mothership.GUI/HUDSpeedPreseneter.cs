using Services.Requests.Interfaces;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;
using System.Collections.Generic;

namespace Mothership.GUI
{
	internal sealed class HUDSpeedPreseneter : IInitialize, IWaitForFrameworkDestruction
	{
		private MachineSpeedUtility _machineSpeedUtility;

		private IDictionary<int, MovementStatsData> _movementStatsData;

		private FasterList<MachineCell> _currentMovementParts = new FasterList<MachineCell>();

		private Dictionary<ItemCategory, int> _partsPerCategory = new Dictionary<ItemCategory, int>();

		private HUDSpeedView _view;

		private int _affectTopSpeedNodeCount;

		private int _baseSpeed;

		private float _speedBoostPercent;

		private bool _useDecimalSystem;

		[Inject]
		internal IServiceRequestFactory serviceFactory
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
		internal MachineMover machineMover
		{
			private get;
			set;
		}

		[Inject]
		internal IMachineBuilder builder
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorBuilder machineBuilder
		{
			private get;
			set;
		}

		internal void SetView(HUDSpeedView hUDSpeedView)
		{
			_view = hUDSpeedView;
		}

		public void OnDependenciesInjected()
		{
			RegisterEventListeners();
		}

		public void OnFrameworkDestroyed()
		{
			UnregisterEventListeners();
		}

		internal void ForceDisplayedHealthToZero(bool lockToZero)
		{
			if (lockToZero)
			{
				UnregisterEventListeners();
				ClearValues();
			}
			else
			{
				RegisterEventListeners();
			}
		}

		private void ClearValues()
		{
			_view.SetBoost("0.00", 0f);
			_view.SetSpeed("0", 0f, _useDecimalSystem);
		}

		private void RegisterEventListeners()
		{
			machineMap.OnAddCubeAt += CubePlaced;
			machineMap.OnRemoveCubeAt += CubeRemoved;
			machineMover.cubesMoved += CubesMoved;
			builder.OnPlaceCube += Builder_OnPlaceCube;
			builder.OnDeleteCube += Builder_OnPlaceCube;
			machineBuilder.OnMachineBuilt += MachineBuilder_OnMachineBuilt;
		}

		private void UnregisterEventListeners()
		{
			machineMap.OnAddCubeAt -= CubePlaced;
			machineMap.OnRemoveCubeAt -= CubeRemoved;
			machineMover.cubesMoved -= CubesMoved;
			builder.OnPlaceCube -= Builder_OnPlaceCube;
			builder.OnDeleteCube -= Builder_OnPlaceCube;
			machineBuilder.OnMachineBuilt -= MachineBuilder_OnMachineBuilt;
		}

		private void MachineBuilder_OnMachineBuilt(uint obj)
		{
			UpdateTopSpeed();
		}

		private void Builder_OnPlaceCube(InstantiatedCube cube)
		{
			UpdateTopSpeed();
		}

		private void CubesMoved(HashSet<InstantiatedCube> obj)
		{
			_currentMovementParts.FastClear();
			_affectTopSpeedNodeCount = 0;
			_partsPerCategory.Clear();
			HashSet<InstantiatedCube>.Enumerator enumerator = obj.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MachineCell cellAt = machineMap.GetCellAt(enumerator.Current.gridPos);
				CubePlaced(enumerator.Current.gridPos, cellAt);
			}
			UpdateTopSpeed();
		}

		private void CubeRemoved(Byte3 arg1, MachineCell cell)
		{
			InstantiatedCube info = cell.info;
			if (info.persistentCubeData.itemType != ItemType.Movement)
			{
				return;
			}
			ItemDescriptor itemDescriptor = info.persistentCubeData.itemDescriptor;
			if (_movementStatsData.TryGetValue(itemDescriptor.GenerateKey(), out MovementStatsData _))
			{
				StoreItemCount(cell, itemDescriptor, adding: false);
				_currentMovementParts.Remove(cell);
				if (itemDescriptor.itemCategory != ItemCategory.Thruster && itemDescriptor.itemCategory != ItemCategory.Propeller)
				{
					_affectTopSpeedNodeCount--;
				}
			}
		}

		private void CubePlaced(Byte3 arg1, MachineCell cell)
		{
			InstantiatedCube info = cell.info;
			if (info.persistentCubeData.itemType != ItemType.Movement)
			{
				return;
			}
			ItemDescriptor itemDescriptor = info.persistentCubeData.itemDescriptor;
			if (_movementStatsData.TryGetValue(itemDescriptor.GenerateKey(), out MovementStatsData _))
			{
				StoreItemCount(cell, itemDescriptor, adding: true);
				_currentMovementParts.Add(cell);
				if (itemDescriptor.itemCategory != ItemCategory.Thruster && itemDescriptor.itemCategory != ItemCategory.Propeller)
				{
					_affectTopSpeedNodeCount++;
				}
			}
		}

		private void StoreItemCount(MachineCell cell, ItemDescriptor descriptor, bool adding)
		{
			ItemCategory itemCategory = descriptor.itemCategory;
			if (itemCategory == ItemCategory.SprinterLeg || itemCategory == ItemCategory.MechLeg)
			{
				itemCategory = ItemCategory.SprinterLeg;
			}
			if (!_partsPerCategory.ContainsKey(itemCategory))
			{
				_partsPerCategory[itemCategory] = 0;
			}
			Dictionary<ItemCategory, int> partsPerCategory;
			ItemCategory key;
			(partsPerCategory = _partsPerCategory)[key = itemCategory] = partsPerCategory[key] + (adding ? 1 : (-1));
		}

		private void UpdateTopSpeed()
		{
			float baseSpeedPercent = 0f;
			_machineSpeedUtility.CalculateSpeed(_currentMovementParts, _partsPerCategory, _affectTopSpeedNodeCount, out _baseSpeed, out baseSpeedPercent, out _speedBoostPercent);
			_view.SetSpeed(_baseSpeed.ToString(), baseSpeedPercent, _useDecimalSystem);
			_view.SetBoost((_speedBoostPercent * 100f).ToString("N2"), _speedBoostPercent);
		}

		public IEnumerator LoadGUIData()
		{
			yield return null;
			ILoadPlatformConfigurationRequest loadPlatformConfigRequest = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> loadPlatformConfigTask = loadPlatformConfigRequest.AsTask();
			loadPlatformConfigTask.Execute();
			_useDecimalSystem = loadPlatformConfigTask.result.UseDecimalSystem;
			ILoadMovementStatsRequest loadWeaponStats = serviceFactory.Create<ILoadMovementStatsRequest>();
			TaskService<MovementStats> taskService = new TaskService<MovementStats>(loadWeaponStats);
			yield return taskService;
			if (taskService.succeeded)
			{
				_machineSpeedUtility = new MachineSpeedUtility(taskService.result, _useDecimalSystem);
				_movementStatsData = taskService.result.data;
			}
		}
	}
}
