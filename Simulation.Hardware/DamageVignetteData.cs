using UnityEngine;

namespace Simulation.Hardware
{
	internal sealed class DamageVignetteData
	{
		internal Rigidbody ShooterRB
		{
			get;
			private set;
		}

		internal Rigidbody PlayerRB
		{
			get;
			private set;
		}

		internal DamageVignetteIndicator DamageIndicator
		{
			get;
			private set;
		}

		public DamageVignetteData(Rigidbody shooterRB, Rigidbody playerRB, DamageVignetteIndicator damageIndicator)
		{
			ShooterRB = shooterRB;
			PlayerRB = playerRB;
			DamageIndicator = damageIndicator;
		}
	}
}
