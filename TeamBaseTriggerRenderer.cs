using Simulation.Hardware.Weapons;
using UnityEngine;

internal class TeamBaseTriggerRenderer : MonoBehaviour
{
	internal CentreOfMassContainer centreOfMassContainer;

	internal MachineTransformContainer machineTransformContainer;

	internal int playerId;

	internal TeamBaseTrigger teamBaseTrigger;

	public TeamBaseTriggerRenderer()
		: this()
	{
	}

	private void OnDrawGizmos()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (centreOfMassContainer != null && teamBaseTrigger != null)
		{
			Vector3 val = teamBaseTrigger.minMax[1] - teamBaseTrigger.minMax[0];
			Matrix4x4 val2 = default(Matrix4x4);
			val2.SetTRS(centreOfMassContainer.GetCentreOfMass(playerId), Quaternion.get_identity(), Vector3.get_one());
			Matrix4x4 val3 = default(Matrix4x4);
			val3.SetTRS(machineTransformContainer.GetMachineWorldCOM(TargetType.Player, playerId), machineTransformContainer.GetMachineRotation(TargetType.Player, playerId), Vector3.get_one());
			Gizmos.set_matrix(val3 * val2);
			Gizmos.DrawWireCube(Vector3.get_zero(), val);
		}
	}
}
