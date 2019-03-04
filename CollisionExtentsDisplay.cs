using Simulation;
using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

internal class CollisionExtentsDisplay : MonoBehaviour
{
	internal IMachineMap map;

	private Rigidbody rb;

	public bool showExtents;

	public CollisionExtentsDisplay()
		: this()
	{
	}

	private void Start()
	{
		rb = this.GetComponent<Rigidbody>();
	}

	private void OnDrawGizmos()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (showExtents)
		{
			ICollection<InstantiatedCube> allInstantiatedCubes = (ICollection<InstantiatedCube>)map.GetAllInstantiatedCubes();
			Gizmos.set_color(Color.get_green());
			foreach (InstantiatedCube item in allInstantiatedCubes)
			{
				Draw(item.gridPos);
				foreach (Byte3 boundsOccupiedCell in item.boundsOccupiedCells)
				{
					Draw(boundsOccupiedCell);
				}
			}
		}
	}

	private void Draw(Byte3 gridCell)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		MachineCell cellAt = map.GetCellAt(gridCell);
		if (cellAt != null)
		{
			Vector3 val = rb.get_position() + rb.get_rotation() * GridScaleUtility.GridToWorld(gridCell, TargetType.Player);
			Gizmos.DrawSphere(val, 0.08f);
		}
	}
}
