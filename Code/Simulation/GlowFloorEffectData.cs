using UnityEngine;

namespace Simulation
{
	public struct GlowFloorEffectData
	{
		public int locatorId;

		public Vector3 position;

		public GlowFloorEffectData(int locatorId_, Vector3 position_)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			locatorId = locatorId_;
			position = position_;
		}

		public void SetValues(int locatorId_, Vector3 position_)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			locatorId = locatorId_;
			position = position_;
		}
	}
}
