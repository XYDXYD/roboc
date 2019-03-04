using System.IO;

namespace RCNetwork.Utilities
{
	internal class RoboUserInfo
	{
		public string _xuid;

		public byte[] _key;

		public RoboUserInfo(string xuid, byte[] key)
		{
			_xuid = xuid;
			_key = key;
		}

		public static RoboUserInfo Create(BinaryReader reader)
		{
			string xuid = reader.ReadString();
			int count = reader.ReadByte();
			byte[] key = reader.ReadBytes(count);
			return new RoboUserInfo(xuid, key);
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(_xuid);
			writer.Write((byte)_key.Length);
			writer.Write(_key);
		}

		public RoboUserInfo Clone()
		{
			return new RoboUserInfo(_xuid, _key);
		}

		public byte[] BuildAuthBytes()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryWriter writer = new BinaryWriter(memoryStream);
				Write(writer);
				return memoryStream.ToArray();
			}
		}

		public static RoboUserInfo CreateFromAuthBytes(byte[] bytes)
		{
			using (MemoryStream input = new MemoryStream(bytes))
			{
				BinaryReader reader = new BinaryReader(input);
				return Create(reader);
			}
		}
	}
}
