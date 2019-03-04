using System;
using UnityEngine;

namespace Simulation.SinglePlayer.CapturePoints
{
	[Serializable]
	public class AICapturePointData
	{
		public Vector3 Position
		{
			get;
			set;
		}

		public float Radius
		{
			get;
			set;
		}

		public AICapturePointStatus Status
		{
			get;
			set;
		}

		public int OwnedByTeamId
		{
			get;
			set;
		}

		public AICapturePointData(Vector3 position, float radius, AICapturePointStatus status, int ownedByTeamId)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Position = position;
			Radius = radius;
			Status = status;
			OwnedByTeamId = ownedByTeamId;
		}
	}
}
