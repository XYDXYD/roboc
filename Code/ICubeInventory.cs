using System;
using System.Collections;
using System.Collections.Generic;

internal interface ICubeInventory
{
	HashSet<uint> OwnedCubes
	{
		get;
	}

	HashSet<uint> NewCubes
	{
		get;
	}

	void RefreshAndForget();

	bool IsCubeOwned(CubeTypeID type);

	IEnumerator RefreshAndWait();

	void RegisterInventoryLoadedCallback(Action callback);

	void DeRegisterInventoryLoadedCallback(Action callback);
}
