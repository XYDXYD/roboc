using RCNetwork.Events;
using Simulation;
using System;
using System.IO;
using UnityEngine;

internal sealed class MapPingEventDependency : NetworkDependency
{
	public int sender;

	public int teamId;

	public PingType type;

	public Vector3 location;

	public MapPingEventDependency(int senderId, int teamId, PingType type, Vector3 location)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		sender = senderId;
		this.teamId = teamId;
		this.type = type;
		this.location = location;
	}

	public MapPingEventDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(BitConverter.GetBytes(sender));
				binaryWriter.Write(BitConverter.GetBytes(teamId));
				binaryWriter.Write(BitConverter.GetBytes((int)type));
				binaryWriter.Write(BitConverter.GetBytes((double)location.x));
				binaryWriter.Write(BitConverter.GetBytes((double)location.y));
				binaryWriter.Write(BitConverter.GetBytes((double)location.z));
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				sender = BitConverter.ToInt32(binaryReader.ReadBytes(4), 0);
				teamId = BitConverter.ToInt32(binaryReader.ReadBytes(4), 0);
				type = (PingType)BitConverter.ToInt32(binaryReader.ReadBytes(4), 0);
				double num = BitConverter.ToDouble(binaryReader.ReadBytes(8), 0);
				double num2 = BitConverter.ToDouble(binaryReader.ReadBytes(8), 0);
				double num3 = BitConverter.ToDouble(binaryReader.ReadBytes(8), 0);
				location = new Vector3((float)num, (float)num2, (float)num3);
			}
		}
	}
}
