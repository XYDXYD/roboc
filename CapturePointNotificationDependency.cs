using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal class CapturePointNotificationDependency : NetworkDependency
{
	public CapturePointNotification notification;

	public int id;

	public Vector3 position;

	public int defendingTeam;

	public int attackingTeam;

	public CapturePointNotificationDependency(byte[] data)
		: base(data)
	{
	}

	public CapturePointNotificationDependency()
	{
	}

	public void SetParameters(CapturePointNotification notification, int id, Vector3 position, int currentTeam, int attackingTeam)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.notification = notification;
		this.id = id;
		this.position = position;
		defendingTeam = currentTeam;
		this.attackingTeam = attackingTeam;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)notification);
				binaryWriter.Write((byte)id);
				binaryWriter.Write((sbyte)defendingTeam);
				binaryWriter.Write((sbyte)attackingTeam);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				notification = (CapturePointNotification)binaryReader.ReadByte();
				id = binaryReader.ReadByte();
				defendingTeam = binaryReader.ReadSByte();
				attackingTeam = binaryReader.ReadSByte();
			}
		}
	}
}
