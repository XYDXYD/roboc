using UnityEngine;

namespace Simulation
{
	internal static class MachineSpawnUtility
	{
		private const float GROUND_CLEARANCE = 0.2f;

		private static readonly Vector3 spawnPositionRaycastOriginOffset = Vector3.get_up() * 5f;

		public static void AdjustSpawnPosition(MachineInfo machineInfo, Rigidbody rb, ref Vector3 spawnPosition, ref Quaternion spawnRotation)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = spawnRotation;
			Vector3 val2 = spawnPosition;
			Vector3 val3 = -machineInfo.MachineCenter;
			RaycastHit val4 = default(RaycastHit);
			if (Physics.Raycast(spawnPosition + spawnPositionRaycastOriginOffset, Vector3.get_down(), ref val4, (float)GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK))
			{
				Quaternion val5 = Quaternion.FromToRotation(Vector3.get_up(), val4.get_normal());
				val = val5 * val;
				val2 = val4.get_point();
			}
			val3.y += machineInfo.MachineSize.y / 2f;
			val3.y += 0.2f;
			spawnPosition = val2 + val * val3;
			spawnRotation = val;
		}
	}
}
