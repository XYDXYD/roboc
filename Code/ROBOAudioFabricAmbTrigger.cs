using Fabric;
using UnityEngine;
using Utility;

public class ROBOAudioFabricAmbTrigger : MonoBehaviour
{
	public ROBOAudioFabricAmbTrigger()
		: this()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.get_gameObject().GetComponent<CharacterMotorEx>() != null)
		{
			Console.Log("INSIDE ambience Triggered");
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_amb_internal", 0, (object)null, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_amb_external", 1, (object)null, this.get_gameObject());
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Console.Log("OUTSIDE ambience Triggered");
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_amb_external", 0, (object)null, this.get_gameObject());
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_amb_internal", 1, (object)null, this.get_gameObject());
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_amb_external_3D", 1, (object)null, this.get_gameObject());
	}
}
