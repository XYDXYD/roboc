using System.Collections;
using UnityEngine;

internal interface IMachineTeleportAudio
{
	IEnumerator PlayAudio(float initialWait, bool entireMachine, int numCubes, GameObject target);

	void PlayDamagedCubeAudio(GameObject target);

	void PlayExplodeRobotSound(GameObject target);
}
