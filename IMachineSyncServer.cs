using UnityEngine;

internal interface IMachineSyncServer
{
	void SendData(BitStream stream);
}
