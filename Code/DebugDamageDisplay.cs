using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

internal sealed class DebugDamageDisplay : MonoBehaviour
{
	internal class DebugDamageData
	{
		internal InstantiatedCube cube;

		internal float endTime;

		internal int hitMachineId;

		internal DebugDamageData(InstantiatedCube c, float t, int p)
		{
			cube = c;
			endTime = t;
			hitMachineId = p;
		}
	}

	private const float displayTime = 6f;

	public static int playerId = 0;

	private static List<DebugDamageData> damagedCubes = new List<DebugDamageData>();

	private static bool _showingDebug = false;

	internal LivePlayersContainer livePlayersContainer;

	internal RigidbodyDataContainer rigidbodyDataContainer;

	internal PlayerMachinesContainer playerMachinesContainer;

	public DebugDamageDisplay()
		: this()
	{
	}

	internal static void CubeDamaged(InstantiatedCube cube, int hitMachineId, int shooter)
	{
		if (_showingDebug && shooter == playerId)
		{
			damagedCubes.Add(new DebugDamageData(cube, Time.get_time() + 6f, hitMachineId));
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(293))
		{
			_showingDebug = !_showingDebug;
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.set_color(Color.get_blue());
		List<DebugDamageData> list = new List<DebugDamageData>();
		foreach (DebugDamageData damagedCube in damagedCubes)
		{
			if (Time.get_time() > damagedCube.endTime)
			{
				list.Add(damagedCube);
			}
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, damagedCube.hitMachineId);
			if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerFromMachineId))
			{
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, damagedCube.hitMachineId);
				Vector3 val = GridScaleUtility.WorldScale(damagedCube.cube.gridPos, TargetType.Player);
				Vector3 val2 = rigidBodyData.get_transform().get_rotation() * val;
				Vector3 val3 = rigidBodyData.get_transform().get_position() + val2;
				Gizmos.DrawSphere(val3, 0.1f);
			}
		}
		foreach (DebugDamageData item in list)
		{
			damagedCubes.Remove(item);
		}
	}
}
