using RCNetwork.Events;
using System.IO;

internal sealed class StringCodeDependency : NetworkDependency
{
	public GameServerErrorCodes errorCode;

	public string errorMessage;

	public StringCodeDependency(byte[] data)
		: base(data)
	{
	}

	public StringCodeDependency(GameServerErrorCodes _errorCode)
	{
		errorCode = _errorCode;
	}

	public StringCodeDependency(string _errorMessage)
	{
		errorMessage = _errorMessage;
		errorCode = GameServerErrorCodes.STR_ERR_CUSTOM_STRING;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((int)errorCode);
				if (errorCode == GameServerErrorCodes.STR_ERR_CUSTOM_STRING)
				{
					binaryWriter.Write(errorMessage);
				}
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
				errorCode = (GameServerErrorCodes)binaryReader.ReadInt32();
				if (errorCode == GameServerErrorCodes.STR_ERR_CUSTOM_STRING)
				{
					errorMessage = binaryReader.ReadString();
				}
			}
		}
	}
}
