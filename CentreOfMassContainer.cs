using System.Collections.Generic;
using UnityEngine;

internal sealed class CentreOfMassContainer
{
	private Dictionary<int, Vector3> _centresOfMass = new Dictionary<int, Vector3>();

	public void SetCentreOfMass(int player, Vector3 centreOfMass)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_centresOfMass[player] = centreOfMass;
	}

	public Vector3 GetCentreOfMass(int player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _centresOfMass[player];
	}
}
