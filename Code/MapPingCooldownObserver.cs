using System;

internal sealed class MapPingCooldownObserver
{
	public event Action<float> StartCooldown = delegate
	{
	};

	public void BeginCooldown(float cooldownTime)
	{
		this.StartCooldown(cooldownTime);
	}
}
