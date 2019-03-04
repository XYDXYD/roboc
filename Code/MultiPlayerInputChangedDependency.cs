using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

internal sealed class MultiPlayerInputChangedDependency : NetworkDependency
{
	internal sealed class PlayerInput
	{
		public float horizontal;

		public float vertical;

		public bool jump;

		public bool crouch;

		public bool toggleLights;

		public bool pulseAR;

		public bool strafeLeft;

		public bool strafeRight;

		public PlayerInput()
		{
		}

		public PlayerInput(float h, float v, bool j, bool c, bool l, bool ar, bool sl, bool sr)
		{
			horizontal = h;
			vertical = v;
			jump = j;
			crouch = c;
			toggleLights = l;
			pulseAR = ar;
			strafeLeft = sl;
			strafeRight = sr;
		}
	}

	public Dictionary<int, PlayerInput> singlePlayerInput = new Dictionary<int, PlayerInput>();

	public MultiPlayerInputChangedDependency()
	{
	}

	public MultiPlayerInputChangedDependency(byte[] data)
		: base(data)
	{
	}

	public void AddPlayer(PlayerInputChangedDependency dependency)
	{
		singlePlayerInput[dependency.owner] = new PlayerInput(dependency.latestState.horizontal, dependency.latestState.vertical, dependency.latestState.jump, dependency.latestState.crouch, dependency.latestState.toggleLights, ar: false, dependency.latestState.strafeLeft, dependency.latestState.strafeRight);
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)singlePlayerInput.Count);
				foreach (KeyValuePair<int, PlayerInput> item in singlePlayerInput)
				{
					binaryWriter.Write((byte)item.Key);
					binaryWriter.Write(PackInputData(item.Value));
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
				int num = binaryReader.ReadByte();
				for (int i = 0; i < num; i++)
				{
					int num2 = binaryReader.ReadByte();
					singlePlayerInput[num2] = UnpackInputData(binaryReader.ReadInt16(), num2);
				}
			}
		}
	}

	private short PackInputData(PlayerInput input)
	{
		short num = 0;
		if (input.jump)
		{
			num = (short)(num | 1);
		}
		if (input.crouch)
		{
			num = (short)(num | 2);
		}
		if (input.pulseAR)
		{
			num = (short)(num | 4);
		}
		if (input.toggleLights)
		{
			num = (short)(num | 8);
		}
		if (input.horizontal > 0f)
		{
			num = (short)(num | 0x10);
		}
		else if (input.horizontal < 0f)
		{
			num = (short)(num | 0x20);
		}
		if (input.vertical > 0f)
		{
			num = (short)(num | 0x40);
		}
		else if (input.vertical < 0f)
		{
			num = (short)(num | 0x80);
		}
		if (input.strafeLeft)
		{
			num = (short)(num | 0x100);
		}
		if (input.strafeRight)
		{
			num = (short)(num | 0x200);
		}
		return num;
	}

	private PlayerInput UnpackInputData(short inputData, int owner)
	{
		PlayerInput playerInput = new PlayerInput();
		if ((inputData & 1) != 0)
		{
			playerInput.jump = true;
		}
		if ((inputData & 2) != 0)
		{
			playerInput.crouch = true;
		}
		if ((inputData & 4) != 0)
		{
			playerInput.pulseAR = true;
		}
		if ((inputData & 8) != 0)
		{
			playerInput.toggleLights = true;
		}
		if ((inputData & 0x10) != 0)
		{
			playerInput.horizontal = 1f;
		}
		else if ((inputData & 0x20) != 0)
		{
			playerInput.horizontal = -1f;
		}
		if ((inputData & 0x40) != 0)
		{
			playerInput.vertical = 1f;
		}
		else if ((inputData & 0x80) != 0)
		{
			playerInput.vertical = -1f;
		}
		if ((inputData & 0x100) != 0)
		{
			playerInput.strafeLeft = true;
		}
		if ((inputData & 0x200) != 0)
		{
			playerInput.strafeRight = true;
		}
		return playerInput;
	}
}
