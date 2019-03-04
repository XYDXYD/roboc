using System;
using System.Collections;

internal interface ICPUPower
{
	uint MaxCpuPower
	{
		get;
	}

	uint MaxMegabotCpuPower
	{
		get;
	}

	uint TotalActualCPUCurrentRobot
	{
		get;
	}

	uint TotalCPUCurrentRobot
	{
		get;
	}

	uint TotalCosmeticCPUCurrentRobot
	{
		get;
	}

	uint MaxCosmeticCpuPool
	{
		get;
	}

	uint CurrentCosmeticCpuPool
	{
		get;
	}

	void RegisterOnCPULoadChanged(Action<uint> action);

	void UnregisterOnCPULoadChanged(Action<uint> action);

	void RegisterOnCosmeticCPULoadChanged(Action<uint> action);

	void UnregisterOnCosmeticCPULoadChanged(Action<uint> action);

	IEnumerator LoadData();

	IEnumerator IsLoadedEnumerator();

	bool IsLoaded();

	void MothershipFlowCompleted();
}
