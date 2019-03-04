using ExitGames.Client.Photon;
using System;

internal sealed class BattleArenaSettingsDependency
{
	public int protoniumHealth;

	public int respawnTimeSeconds;

	public uint[] energyOverTimePerPointOwned;

	public byte[] teamBaseModel;

	public byte[] equalizerModel;

	public int equalizerHealth;

	public uint[] equalizerTriggerTimeSeconds;

	public uint[] equalizerDurationSeconds;

	public uint equalizerWarningSeconds;

	public int[] captureTimeSecondsPerPlayer;

	public int numSegments;

	public int healEscalationTimeSeconds;

	public BattleArenaSettingsDependency()
	{
	}

	public BattleArenaSettingsDependency(Hashtable data)
	{
		protoniumHealth = (int)(long)data.get_Item((object)"protoniumHealth");
		respawnTimeSeconds = (int)(long)data.get_Item((object)"respawnTimeSeconds");
		energyOverTimePerPointOwned = ParseUIntArray((object[])data.get_Item((object)"healOverTimePerTower"));
		teamBaseModel = Convert.FromBase64String((string)data.get_Item((object)"baseMachineMap"));
		equalizerModel = Convert.FromBase64String((string)data.get_Item((object)"equalizerModel"));
		equalizerHealth = (int)(long)data.get_Item((object)"equalizerHealth");
		equalizerTriggerTimeSeconds = ParseUIntArray((object[])data.get_Item((object)"equalizerTriggerTimeSeconds"));
		equalizerWarningSeconds = (uint)(long)data.get_Item((object)"equalizerWarningSeconds");
		equalizerDurationSeconds = ParseUIntArray((object[])data.get_Item((object)"equalizerDurationSeconds"));
		captureTimeSecondsPerPlayer = ParseIntArray((object[])data.get_Item((object)"captureTimeSecondsPerPlayer"));
		numSegments = Convert.ToInt32(data.get_Item((object)"numSegments"));
		healEscalationTimeSeconds = (int)(long)data.get_Item((object)"healEscalationTimeSeconds");
	}

	private static uint[] ParseUIntArray(object[] srcArray)
	{
		uint[] array = new uint[srcArray.Length];
		for (int i = 0; i < srcArray.Length; i++)
		{
			array[i] = (uint)(long)srcArray[i];
		}
		return array;
	}

	private static int[] ParseIntArray(object[] srcArray)
	{
		int[] array = new int[srcArray.Length];
		for (int i = 0; i < srcArray.Length; i++)
		{
			array[i] = (int)(long)srcArray[i];
		}
		return array;
	}
}
