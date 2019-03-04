using Fabric;
using Svelto.Tasks.Enumerators;
using System.Collections;
using UnityEngine;

internal sealed class MachineTeleportAudioFabric : IMachineTeleportAudio
{
	public IEnumerator PlayAudio(float initialWait, bool entireMachine, int numCubes, GameObject target)
	{
		yield return (object)new WaitForSecondsEnumerator(initialWait);
		if (entireMachine)
		{
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_vfx_kube_teleport_seq", 0, (object)null, target);
		}
		else if (numCubes != 1)
		{
			if (numCubes <= 10)
			{
				EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_vfx_kube_teleport_grp_sml", 0, (object)null, target);
			}
			else
			{
				EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_vfx_kube_teleport_grp_lrg", 0, (object)null, target);
			}
		}
	}

	public void PlayDamagedCubeAudio(GameObject target)
	{
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_LaserCannon_Damage_Impact", 0, (object)null, target);
	}

	public void PlayExplodeRobotSound(GameObject target)
	{
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_vfx_kube_robot_explode", 0, (object)null, target);
	}
}
