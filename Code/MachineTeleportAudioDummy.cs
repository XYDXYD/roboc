using System.Collections;
using UnityEngine;

internal sealed class MachineTeleportAudioDummy : IMachineTeleportAudio
{
	public IEnumerator PlayAudio(float initialWait, bool entireMachine, int numCubes, GameObject target)
	{
		yield break;
	}

	public void PlayDamagedCubeAudio(GameObject target)
	{
	}

	public void PlayExplodeRobotSound(GameObject target)
	{
	}
}
