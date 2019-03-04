using UnityEngine;

namespace Simulation
{
	internal interface ITeleportModuleEffectsComponent
	{
		GameObject teleportStartEffectAlly
		{
			get;
		}

		GameObject teleportTrailEffectAlly
		{
			get;
		}

		GameObject teleportGlowingCenterEffectAlly
		{
			get;
		}

		GameObject teleportEndEffectAlly
		{
			get;
		}

		GameObject teleportStartEffectEnemy
		{
			get;
		}

		GameObject teleportTrailEffectEnemy
		{
			get;
		}

		GameObject teleportGlowingCenterEffectEnemy
		{
			get;
		}

		GameObject teleportEndEffectEnemy
		{
			get;
		}
	}
}
