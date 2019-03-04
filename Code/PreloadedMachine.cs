using Services.Simulation;
using Simulation;
using Svelto.DataStructures;
using UnityEngine;

internal class PreloadedMachine
{
	public MachineModel machineModel;

	public FasterList<Transform> allCubeTransforms;

	public Rigidbody rbData;

	public WeaponRaycast weaponRaycast;

	public GameObject machineBoard;

	public IMachineControl inputWrapper;

	public IMachineMap machineMap;

	public MachineInfo machineInfo = new MachineInfo();

	public FasterList<Renderer> allRenderers;

	public int machineId = -1;

	public WeaponOrderSimulation weaponOrder;

	public MachineGraph machineGraph;

	public FasterList<SettingUpCube> batchableCubes;

	public const int NO_ID = -1;
}
