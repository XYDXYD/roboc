using Mothership;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

public class HUDAdvancedEdit : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
{
	public GameObject comPrefab;

	private float _totalCpu;

	private float _totalMass;

	private Vector3 _com = Vector3.get_zero();

	private GameObject _comIndicator;

	[Inject]
	internal IMachineMap machineMap
	{
		private get;
		set;
	}

	[Inject]
	internal AdvancedRobotEditSettings advancedRobotEditSettings
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

	public GameObject ComIndicator => _comIndicator;

	public HUDAdvancedEdit()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public void OnDependenciesInjected()
	{
		machineMap.OnAddCubeAt += CubePlaced;
		machineMap.OnRemoveCubeAt += CubeRemoved;
		machineMover.cubesMoved += CubesMoved;
		advancedRobotEditSettings.OnToggleCenterOfMass += OnToggleCenterOfMass;
	}

	public void OnFrameworkDestroyed()
	{
		advancedRobotEditSettings.OnToggleCenterOfMass -= OnToggleCenterOfMass;
	}

	private void OnToggleCenterOfMass(bool enable)
	{
		_comIndicator.SetActive(_totalCpu > 0f && enable && WorldSwitching.IsInBuildMode());
	}

	private void CubesMoved(HashSet<InstantiatedCube> obj)
	{
		if (_totalCpu > 0f)
		{
			AllCubesRemoved();
			HashSet<InstantiatedCube>.Enumerator enumerator = obj.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCubePlaced(enumerator.Current);
			}
		}
	}

	private void AllCubesRemoved()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		_totalCpu = 0f;
		_totalMass = 0f;
		_com = Vector3.get_zero();
	}

	private void CubeRemoved(Byte3 position, MachineCell cell)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		InstantiatedCube info = cell.info;
		_totalCpu -= (float)(double)info.persistentCubeData.cpuRating;
		float num = info.mass * info.physcisMassScalar;
		_totalMass -= num;
		Vector3 val = GridScaleUtility.GridToWorld(info.gridPos, TargetType.Player) + GridScaleUtility.WorldScale(CubeData.IndexToQuat(info.rotationIndex) * info.comOffset, TargetType.Player);
		_com -= val * num;
		if (_totalCpu <= 0f)
		{
			_comIndicator.SetActive(false);
			AllCubesRemoved();
		}
		else
		{
			UpdateComMarker();
		}
	}

	private void CubePlaced(Byte3 position, MachineCell cell)
	{
		InstantiatedCubePlaced(cell.info);
	}

	private void InstantiatedCubePlaced(InstantiatedCube cube)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		_totalCpu += (float)(double)cube.persistentCubeData.cpuRating;
		float num = cube.mass * cube.physcisMassScalar;
		_totalMass += num;
		Vector3 val = GridScaleUtility.GridToWorld(cube.gridPos, TargetType.Player) + GridScaleUtility.WorldScale(CubeData.IndexToQuat(cube.rotationIndex) * cube.comOffset, TargetType.Player);
		_com += val * num;
		UpdateComMarker();
		_comIndicator.SetActive(this.get_isActiveAndEnabled() && advancedRobotEditSettings.centerOfMass && WorldSwitching.IsInBuildMode());
	}

	private void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_comIndicator = Object.Instantiate<GameObject>(comPrefab, Vector3.get_zero(), Quaternion.get_identity());
		_comIndicator.SetActive(false);
	}

	private void OnEnable()
	{
		_comIndicator.SetActive(_totalCpu > 0f && advancedRobotEditSettings.centerOfMass && WorldSwitching.IsInBuildMode());
	}

	private void OnDisable()
	{
		if (_comIndicator != null)
		{
			_comIndicator.SetActive(false);
		}
	}

	private void UpdateComMarker()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_comIndicator.get_transform().set_position(_com / _totalMass);
	}
}
