using RCNetwork.Events;
using System.IO;

internal sealed class PlayerInputChangedDependency : NetworkDependency
{
	public int owner;

	public LatestState latestState;

	public PlayerInputChangedDependency(byte[] data)
		: base(data)
	{
	}

	public PlayerInputChangedDependency(int _owner, LatestState latest)
	{
		owner = _owner;
		latestState = latest;
	}

	public PlayerInputChangedDependency(LatestState latest)
	{
		latestState = latest;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)owner);
				binaryWriter.Write(PackInputData());
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
				owner = binaryReader.ReadByte();
				UnpackInputData(binaryReader.ReadInt16());
			}
		}
	}

	private short PackInputData()
	{
		short num = 0;
		if (latestState.jump)
		{
			num = (short)(num | 1);
		}
		if (latestState.crouch)
		{
			num = (short)(num | 2);
		}
		if (latestState.toggleLights)
		{
			num = (short)(num | 8);
		}
		if (latestState.horizontal > 0f)
		{
			num = (short)(num | 0x10);
		}
		else if (latestState.horizontal < 0f)
		{
			num = (short)(num | 0x20);
		}
		if (latestState.vertical > 0f)
		{
			num = (short)(num | 0x40);
		}
		else if (latestState.vertical < 0f)
		{
			num = (short)(num | 0x80);
		}
		if (latestState.strafeLeft)
		{
			num = (short)(num | 0x100);
		}
		if (latestState.strafeRight)
		{
			num = (short)(num | 0x200);
		}
		return num;
	}

	private void UnpackInputData(short inputData)
	{
		latestState = new LatestState();
		if ((inputData & 1) != 0)
		{
			latestState.jump = true;
		}
		if ((inputData & 2) != 0)
		{
			latestState.crouch = true;
		}
		if ((inputData & 8) != 0)
		{
			latestState.toggleLights = true;
		}
		if ((inputData & 0x10) != 0)
		{
			latestState.horizontal = 1f;
		}
		else if ((inputData & 0x20) != 0)
		{
			latestState.horizontal = -1f;
		}
		if ((inputData & 0x40) != 0)
		{
			latestState.vertical = 1f;
		}
		else if ((inputData & 0x80) != 0)
		{
			latestState.vertical = -1f;
		}
		if ((inputData & 0x100) != 0)
		{
			latestState.strafeLeft = true;
		}
		if ((inputData & 0x200) != 0)
		{
			latestState.strafeRight = true;
		}
	}
}
