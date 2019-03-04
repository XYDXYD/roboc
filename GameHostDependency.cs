using RCNetwork.Events;
using System;
using System.IO;
using System.Text;

internal sealed class GameHostDependency : NetworkDependency
{
	public string ipAddress = "172.16.3.37";

	public int port = 2500;

	public GameHostDependency(string _ipAddress, int _port)
	{
		ipAddress = _ipAddress;
		port = _port;
	}

	public GameHostDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			memoryStream.WriteByte((byte)ipAddress.Length);
			memoryStream.Write(aSCIIEncoding.GetBytes(ipAddress), 0, ipAddress.Length);
			memoryStream.Write(BitConverter.GetBytes(port), 0, 2);
			return memoryStream.ToArray();
		}
	}

	public override void Deserialise(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
				int count = binaryReader.ReadByte();
				ipAddress = aSCIIEncoding.GetString(binaryReader.ReadBytes(count));
				port = binaryReader.ReadInt16();
			}
		}
	}
}
