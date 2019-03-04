using Fabric;
using UnityEngine;

public class ROBOAudioFabricAmbTriggerSpotfx : MonoBehaviour
{
	public string AudioFabricEvent = string.Empty;

	public ROBOAudioFabricAmbTriggerSpotfx()
		: this()
	{
	}

	private void Start()
	{
		EventManager.get_Instance().PostEvent(AudioFabricEvent, 0, (object)null, this.get_gameObject());
	}
}
