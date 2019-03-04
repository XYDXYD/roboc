using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class PrebuiltRobotBuilder
	{
		private const int CUBES_TO_BUILD_IN_A_FRAME = 100;

		private Dictionary<string, PrebuiltRobotPart> _prebuiltRobotsData = new Dictionary<string, PrebuiltRobotPart>();

		private FasterList<int> _prebuiltRobotPaletteIndexList = new FasterList<int>();

		private FasterList<string> _defaultRobotPartIds = new FasterList<string>();

		private IMachineMap _prebuiltRobotMachineMap = new MachineMap();

		private Bounds _prebuiltRobotBounds = default(Bounds);

		private PaletteColor[] _currentColourCombination = new PaletteColor[3];

		private PrebuiltRobotRandomColorPicker _prebuiltRobotRandomColorPicker;

		[Inject]
		private ICubeFactory cubeFactory
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
		private IMachineBuilder machineBuilder
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
		private PrebuiltRobotPresenter prebuiltRobotPresenter
		{
			get;
			set;
		}

		[Inject]
		private PremiumMembership premiumMembership
		{
			get;
			set;
		}

		[Inject]
		private RobotDimensionChangedObservable robotDimensionChangedObs
		{
			get;
			set;
		}

		public IMachineMap prebuiltRobotMachineMap => _prebuiltRobotMachineMap;

		public IEnumerator Initialise(Dictionary<string, PrebuiltRobotPart> prebuiltRobotsData, FasterList<string> defaultRobotPartIds)
		{
			_prebuiltRobotsData = prebuiltRobotsData;
			_defaultRobotPartIds = defaultRobotPartIds;
			_prebuiltRobotRandomColorPicker = new PrebuiltRobotRandomColorPicker(serviceFactory, premiumMembership);
			yield return _prebuiltRobotRandomColorPicker.LoadData();
		}

		public void ShowRobotBuilder(bool enable)
		{
			if (enable)
			{
				machineBuilder.RemoveAllCubesGarage();
				NextRandomColor();
				BuildSelectedRobotIds(_defaultRobotPartIds);
			}
			else
			{
				ClearPrebuiltRobot();
				garagePresenter.LoadAndBuildRobot();
			}
		}

		public void NextRandomColor()
		{
			_currentColourCombination = _prebuiltRobotRandomColorPicker.GetRandomColorCombination();
			prebuiltRobotPresenter.ShowColors(_currentColourCombination);
		}

		public void ColorRobot()
		{
			_prebuiltRobotPaletteIndexList.Clear();
			HashSet<InstantiatedCube> allInstantiatedCubes = _prebuiltRobotMachineMap.GetAllInstantiatedCubes();
			using (HashSet<InstantiatedCube>.Enumerator enumerator = allInstantiatedCubes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.Current;
					GameObject cubeAt = _prebuiltRobotMachineMap.GetCubeAt(enumerator.Current.gridPos);
					if (!_prebuiltRobotPaletteIndexList.Contains((int)current.paletteIndex))
					{
						_prebuiltRobotPaletteIndexList.Add((int)current.paletteIndex);
					}
					int num = _prebuiltRobotPaletteIndexList.IndexOf((int)current.paletteIndex);
					if (num >= _currentColourCombination.Length)
					{
						Console.LogError("Invalid no. of colours on robot. >.<");
					}
					else
					{
						PaletteColor paletteColor = current.paletteColor = _currentColourCombination[num];
						current.paletteIndex = paletteColor.paletteIndex;
						cubeAt.GetComponent<CubeColorUpdater>().SetColor(paletteColor);
					}
				}
			}
		}

		public void BuildSelectedRobotIds(FasterList<string> selectedRobotIds)
		{
			TaskRunner.get_Instance().Run(BuildAndColourRobot(selectedRobotIds));
		}

		private IEnumerator BuildAndColourRobot(FasterList<string> selectedRobotIds)
		{
			prebuiltRobotPresenter.ShowLoadingIcon(enable: true);
			ClearPrebuiltRobot();
			for (int i = 0; i < selectedRobotIds.get_Count(); i++)
			{
				PrebuiltRobotPart robotPart = _prebuiltRobotsData[selectedRobotIds.get_Item(i)];
				yield return BuildMachinePart(robotPart);
			}
			ColorRobot();
			RobotDimensionDependency dep = new RobotDimensionDependency(_prebuiltRobotBounds.get_min(), _prebuiltRobotBounds.get_max());
			robotDimensionChangedObs.Dispatch(ref dep);
			prebuiltRobotPresenter.ShowLoadingIcon(enable: false);
		}

		private void ClearPrebuiltRobot()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			HashSet<InstantiatedCube> allInstantiatedCubes = _prebuiltRobotMachineMap.GetAllInstantiatedCubes();
			using (HashSet<InstantiatedCube>.Enumerator enumerator = allInstantiatedCubes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject cubeAt = _prebuiltRobotMachineMap.GetCubeAt(enumerator.Current.gridPos);
					Object.Destroy(cubeAt);
				}
			}
			_prebuiltRobotMachineMap = new MachineMap();
			_prebuiltRobotBounds = default(Bounds);
		}

		private IEnumerator BuildMachinePart(PrebuiltRobotPart robotPart)
		{
			int count = 0;
			FasterReadOnlyList<CubeData> robotCubes = robotPart.machineModel.DTO.cubes;
			for (int i = 0; i < robotCubes.get_Count(); i++)
			{
				CubeData cell = robotCubes.get_Item(i);
				if (!_prebuiltRobotMachineMap.IsPosValid(cell.gridLocation))
				{
					Console.LogError($"Invalid position on {robotPart.id}");
					continue;
				}
				if (_prebuiltRobotMachineMap.IsCellTaken(cell.gridLocation))
				{
					Console.LogError($"Cell already taken {robotPart.id}");
					continue;
				}
				uint cubeId = cell.iID;
				if (cubeList.IsCubeValid(cubeId))
				{
					CubeNodeInstance cubeNodeInstance = new CubeNodeInstance();
					InstantiatedCube instantiatedCube = new InstantiatedCube(cubeNodeInstance, cubeList.CubeTypeDataOf(cubeId).cubeData, cell.gridLocation, cell.rotationIndex);
					instantiatedCube.paletteIndex = cell.paletteIndex;
					cubeNodeInstance.instantiatedCube = instantiatedCube;
					GameObject val = cubeFactory.BuildCube(cell.iID, GridScaleUtility.GridToWorld(cell.gridLocation, TargetType.Player), CubeData.IndexToQuat(cell.rotationIndex), TargetType.Player);
					instantiatedCube.SetParams(val.GetComponent<CubeInstance>());
					_prebuiltRobotMachineMap.SilentlyAddCellToMachineMap(cell.gridLocation, instantiatedCube, val);
					bool flag = false;
					Collider[] componentsInChildren = val.GetComponentsInChildren<Collider>();
					Collider[] array = componentsInChildren;
					foreach (Collider val2 in array)
					{
						if (flag)
						{
							_prebuiltRobotBounds.Encapsulate(val2.get_bounds());
							continue;
						}
						ref Bounds prebuiltRobotBounds = ref _prebuiltRobotBounds;
						Bounds bounds = val2.get_bounds();
						prebuiltRobotBounds.set_center(bounds.get_center());
						ref Bounds prebuiltRobotBounds2 = ref _prebuiltRobotBounds;
						Bounds bounds2 = val2.get_bounds();
						prebuiltRobotBounds2.set_extents(bounds2.get_extents());
						ref Bounds prebuiltRobotBounds3 = ref _prebuiltRobotBounds;
						Bounds bounds3 = val2.get_bounds();
						prebuiltRobotBounds3.set_max(bounds3.get_max());
						ref Bounds prebuiltRobotBounds4 = ref _prebuiltRobotBounds;
						Bounds bounds4 = val2.get_bounds();
						prebuiltRobotBounds4.set_min(bounds4.get_min());
						ref Bounds prebuiltRobotBounds5 = ref _prebuiltRobotBounds;
						Bounds bounds5 = val2.get_bounds();
						prebuiltRobotBounds5.set_size(bounds5.get_size());
						flag = true;
					}
				}
				count++;
				if (count >= 100)
				{
					count = 0;
					yield return null;
				}
			}
		}
	}
}
