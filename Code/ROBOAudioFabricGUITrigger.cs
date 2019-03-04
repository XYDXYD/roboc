using Fabric;
using UnityEngine;

public class ROBOAudioFabricGUITrigger : MonoBehaviour
{
	public string AudioFabricEventGUIOpen = string.Empty;

	public string AudioFabricEventGUIClose = string.Empty;

	public string AudioFabricEventGUISelect = string.Empty;

	public ROBOAudioFabricGUITrigger()
		: this()
	{
	}

	private void DEMO_Audio_Fabric_GUIOpen_Trigger()
	{
		if (AudioFabricEventGUIOpen.Length > 0)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEventGUIOpen, 0, (object)null, this.get_gameObject());
		}
	}

	private void DEMO_Audio_Fabric_GUIClose_Trigger()
	{
		if (AudioFabricEventGUIClose.Length > 0)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEventGUIClose, 0, (object)null, this.get_gameObject());
		}
	}

	private void DEMO_Audio_Fabric_GUISelect_Trigger()
	{
		if (AudioFabricEventGUISelect.Length > 0)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEventGUISelect, 0, (object)null, this.get_gameObject());
		}
	}
}
