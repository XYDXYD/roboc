using LiteNetLib;
using Network;
using System;
using Utility;

namespace RCNetwork.UNet
{
	public static class ConnectionConfigHelper
	{
		public static void SetupConnection(NetworkConfig networkConfig, byte[] encryptionParams, out ConnectionConfig config, out ConfigData configData)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			string networkChannelType = networkConfig.NetworkChannelType;
			QosType[] array = new QosType[9]
			{
				QosType.Reliable,
				QosType.Unreliable,
				QosType.StateUpdate,
				QosType.ReliableSequenced,
				QosType.AllCostDelivery,
				QosType.ReliableStateUpdate,
				QosType.ReliableFragmented,
				QosType.UnreliableSequenced,
				QosType.UnreliableFragmented
			};
			char[] array2 = networkChannelType.ToCharArray();
			config = new ConnectionConfig();
			configData = new ConfigData();
			for (int i = 0; i < array2.Length; i++)
			{
				int num = int.Parse(array2[i].ToString());
				QosType qosType = array[num];
				if (qosType == QosType.StateUpdate)
				{
					Console.LogError("QosType is StateUpdate - overriding to Unreliable");
					qosType = QosType.Unreliable;
				}
				if (qosType == QosType.ReliableStateUpdate)
				{
					Console.LogError("QosType is ReliableStateUpdate - overriding to Reliable");
					qosType = QosType.Reliable;
				}
				config.AddChannel(qosType);
			}
			if (encryptionParams != null)
			{
				configData.AuthInfo = encryptionParams;
				configData.MaxPacketSize = networkConfig.MaxPacketSize;
				configData.MaxSentMessageQueueSize = networkConfig.MaxSentMessageQueueSize;
				configData.IsAcksLong = networkConfig.IsAcksLong;
				configData.NetworkDropThreshold = networkConfig.NetworkDropThreshold;
				configData.PacketSize = networkConfig.PacketSize;
				configData.MaxCombinedReliableMessageCount = networkConfig.MaxCombinedReliableMessageCount;
				configData.MaxCombinedReliableMessageSize = networkConfig.MaxCombinedReliableMessageSize;
				configData.MinUpdateTimeout = networkConfig.MinUpdateTimeout;
				configData.OverflowDropThreshold = networkConfig.OverflowThreshold;
				configData.ResendDelayBase = networkConfig.ResendDelayBase;
				configData.ResendDelayRttMult = networkConfig.ResendDelayRttMult;
				configData.NetworkPeerUpdateInterval = networkConfig.NetworkPeerUpdateInterval;
				configData.MaxMillisecondsDelayForBeingDisconnected = networkConfig.MaxMillisecondsDelayForBeingDisconnected;
				Console.Log($"ConnectionConfigHelper: ChannelType: {networkConfig.NetworkChannelType} MaxSentMessageQueueSize: {networkConfig.MaxSentMessageQueueSize} IsAcksLong: {networkConfig.IsAcksLong} NetworkDropThreshold: {networkConfig.NetworkDropThreshold} PacketSize: {networkConfig.PacketSize} MaxCombinedReliableMessageCount: {networkConfig.MaxCombinedReliableMessageCount} MaxCombinedReliableMessageSize: {networkConfig.MaxCombinedReliableMessageSize} MinUpdateTimeout: {networkConfig.MinUpdateTimeout} MaxDelay: {networkConfig.MaxDelay} OverflowThreshold: {networkConfig.OverflowThreshold} MaxPacketSize {networkConfig.MaxPacketSize}");
				return;
			}
			throw new Exception("Encryption params must be supplied");
		}
	}
}
