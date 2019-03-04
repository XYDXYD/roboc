using ExitGames.Client.Photon;

public sealed class NetworkConfig
{
	public readonly string NetworkChannelType;

	public readonly ushort MaxSentMessageQueueSize;

	public readonly bool IsAcksLong;

	public readonly byte NetworkDropThreshold;

	public readonly ushort PacketSize;

	public readonly ushort MaxPacketSize;

	public readonly ushort MaxCombinedReliableMessageCount;

	public readonly ushort MaxCombinedReliableMessageSize;

	public readonly uint MinUpdateTimeout;

	public readonly float MaxDelay;

	public readonly byte OverflowThreshold;

	public readonly double ResendDelayBase;

	public readonly double ResendDelayRttMult;

	public readonly int NetworkPeerUpdateInterval;

	public readonly int MaxMillisecondsDelayForBeingDisconnected;

	public NetworkConfig(Hashtable config)
	{
		NetworkChannelType = (string)config.get_Item((object)"NetworkChannelTypes");
		MaxSentMessageQueueSize = (ushort)(long)config.get_Item((object)"MaxSentMessageQueueSize");
		IsAcksLong = ((long)config.get_Item((object)"IsAcksLong") != 0);
		NetworkDropThreshold = (byte)(long)config.get_Item((object)"NetworkDropThreshold");
		PacketSize = (ushort)(long)config.get_Item((object)"PacketSize");
		MaxCombinedReliableMessageCount = (ushort)(long)config.get_Item((object)"MaxCombinedReliableMessageCount");
		MaxCombinedReliableMessageSize = (ushort)(long)config.get_Item((object)"MaxCombinedReliableMessageSize");
		MinUpdateTimeout = (ushort)(long)config.get_Item((object)"MinUpdateTimeout");
		MaxDelay = (long)config.get_Item((object)"MaxDelay");
		OverflowThreshold = (byte)(long)config.get_Item((object)"OverflowThreshold");
		MaxPacketSize = (ushort)(long)config.get_Item((object)"MaxPacketSize");
		ResendDelayBase = (double)config.get_Item((object)"ResendDelayBase");
		ResendDelayRttMult = (double)config.get_Item((object)"ResendDelayRttMult");
		NetworkPeerUpdateInterval = (int)(long)config.get_Item((object)"NetworkPeerUpdateInterval");
		MaxMillisecondsDelayForBeingDisconnected = (int)(long)config.get_Item((object)"MaxMillisecondsDelayForBeingDisconnected");
	}
}
