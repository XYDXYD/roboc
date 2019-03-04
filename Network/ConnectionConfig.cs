using LiteNetLib;
using System;
using System.Collections.Generic;

namespace Network
{
	public class ConnectionConfig
	{
		internal List<SendOptions> _channels = new List<SendOptions>();

		public byte AddChannel(QosType qosType)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			if (_channels.Count > 255)
			{
				throw new ArgumentOutOfRangeException("Channels Count should be less than 256");
			}
			int count = _channels.Count;
			SendOptions item = 0;
			switch (qosType)
			{
			case QosType.Unreliable:
				item = 0;
				break;
			case QosType.UnreliableSequenced:
				item = 2;
				break;
			case QosType.Reliable:
				item = 1;
				break;
			case QosType.ReliableSequenced:
				item = 3;
				break;
			case QosType.StateUpdate:
				item = 5;
				break;
			default:
				if (!Enum.IsDefined(typeof(QosType), qosType))
				{
					throw new ArgumentOutOfRangeException("qos type is not implemented in underlying network layer: " + (int)qosType);
				}
				break;
			}
			_channels.Add(item);
			return (byte)count;
		}
	}
}
