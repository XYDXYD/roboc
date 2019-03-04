using UnityEngine;

internal interface IMachineSyncClient
{
	void ReceiveData(BitStream stream);
}
