using Network;
using System.IO;

public class NetworkMessage : MessageBase
{
	public short type;

	public byte[] data;

	public NetworkMessage()
	{
	}

	public NetworkMessage(NetworkMessage nm)
	{
		type = nm.type;
		data = nm.data;
	}

	public override void GhettoSerialize(BinaryWriter bw)
	{
		bw.Write(type);
		bw.Write((ushort)data.Length);
		bw.Write(data);
	}

	public override void GhettoDeserialize(BinaryReader br)
	{
		type = br.ReadInt16();
		int count = br.ReadUInt16();
		data = br.ReadBytes(count);
	}
}
