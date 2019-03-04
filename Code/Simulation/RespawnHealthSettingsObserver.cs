using System;

namespace Simulation
{
	internal sealed class RespawnHealthSettingsObserver
	{
		public event Action<float, float> OnRespawnSettingsReceived = delegate
		{
		};

		public void ApplyRespawnHealSettings(float respawnHealDuration, float respawnFullHealDuration)
		{
			this.OnRespawnSettingsReceived(respawnHealDuration, respawnFullHealDuration);
		}
	}
}
