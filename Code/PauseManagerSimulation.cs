using UnityEngine;

internal sealed class PauseManagerSimulation : IPauseManager
{
	public void Pause(bool pause)
	{
		if (!WorldSwitching.IsMultiplayer())
		{
			if (pause)
			{
				Time.set_timeScale(0f);
			}
			else
			{
				Time.set_timeScale(1f);
			}
		}
	}
}
