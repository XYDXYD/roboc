using UnityEngine;

internal sealed class MachineInfo
{
	public Transform centerTransform;

	public Transform cameraPivotTransform;

	public Vector3 MachineSize;

	public Vector3 MachineCenter;

	public float totalMass;

	public Vector3 initialCOM = Vector3.get_zero();

	public int totalHealth;

	public uint totalCpu;
}
