using RCNetwork.Events;
using System.IO;
using System.Text;

internal sealed class RegisterMachineDependency : NetworkDependency
{
	public string name;

	public string displayName;

	public int playerID;

	public RegisterMachineDependency(byte[] data)
		: base(data)
	{
	}

	public RegisterMachineDependency(string _name, string _displayName, int _playerID)
	{
		name = _name;
		displayName = _displayName;
		playerID = _playerID;
	}

	public RegisterMachineDependency(RegisterMachineDependency _dependency)
	{
		name = _dependency.name;
		displayName = _dependency.displayName;
		playerID = _dependency.playerID;
		senderId = _dependency.senderId;
	}

	public override byte[] Serialise()
	{
		byte[] bytes = Encoding.UTF8.GetBytes(displayName);
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(playerID);
				binaryWriter.Write(name.Length);
				binaryWriter.Write(Encoding.ASCII.GetBytes(name));
				binaryWriter.Write(bytes.Length);
				binaryWriter.Write(bytes);
			}
			return memoryStream.ToArray();
		}
	}

	public override void Deserialise(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				playerID = binaryReader.ReadInt32();
				int count = binaryReader.ReadInt32();
				name = Encoding.ASCII.GetString(binaryReader.ReadBytes(count));
				int count2 = binaryReader.ReadInt32();
				displayName = Encoding.UTF8.GetString(binaryReader.ReadBytes(count2));
			}
		}
	}
}
