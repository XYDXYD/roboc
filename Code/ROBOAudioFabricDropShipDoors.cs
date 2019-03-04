using Fabric;
using UnityEngine;

public class ROBOAudioFabricDropShipDoors : MonoBehaviour
{
	public ROBOAudioFabricDropShipDoors()
		: this()
	{
	}

	private void DEMO_Audio_Fabric_DropShipDoors_Open()
	{
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_prop_DropShip_DoorsOpen", 0, (object)null, this.get_gameObject());
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_amb_external_3D", 0, (object)null, this.get_gameObject());
	}

	private void DEMO_Audio_Fabric_DropShipDoors_Close()
	{
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_prop_DropShip_DoorsClose", 0, (object)null, this.get_gameObject());
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_amb_external_3D", 1, (object)null, this.get_gameObject());
	}
}
