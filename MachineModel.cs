using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Utility;

internal sealed class MachineModel
{
	private const byte DESTROYEDMASK = 128;

	private const byte ROTATIONMASK = 127;

	private Byte3 gridOffset;

	internal MachineDTO DTO
	{
		get;
		private set;
	}

	public MachineModel()
	{
		DTO = new MachineDTO();
	}

	public MachineModel(MachineModel model)
	{
		DTO = new MachineDTO();
		gridOffset = model.gridOffset;
		int num = 0;
		for (int i = 0; i < model.DTO.Count; i++)
		{
			if (!DTO.Add(new CubeData(model.DTO[i])))
			{
				num++;
			}
		}
		if (num > 0)
		{
			Console.LogError("Invalid robot loadingattempted to load a robot which has " + num + " cubes located in the same cell. The duplicates have been ignored.");
		}
	}

	public MachineModel(byte[] data, Byte3 offset = default(Byte3))
	{
		DTO = new MachineDTO();
		int num = 0;
		gridOffset = offset;
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num2 = binaryReader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					CubeData cubeData = new CubeData();
					cubeData.iID = binaryReader.ReadUInt32();
					cubeData.gridLocation = new Byte3(0, 0, 0);
					cubeData.gridLocation.x = binaryReader.ReadByte();
					cubeData.gridLocation.y = binaryReader.ReadByte();
					cubeData.gridLocation.z = binaryReader.ReadByte();
					cubeData.gridLocation += offset;
					byte b = binaryReader.ReadByte();
					cubeData.rotationIndex = (byte)(b & 0x7F);
					cubeData.isDestroyed = ((b & 0x80) != 0);
					if (!DTO.Add(cubeData))
					{
						num++;
					}
				}
			}
		}
		if (num > 0)
		{
			Console.LogError("Invalid robot loadingattempted to load a robot which has " + num + " cubes located in the same cell. The duplicates have been ignored.");
		}
	}

	[Conditional("DEBUG_CUBES_FOR_MASKS")]
	private void PrintCubesInformation(CubeData cubeData)
	{
		string text = cubeData.iID.ToString("x");
		if (text != "d8ae0c6" && text != "d95c05c")
		{
			Console.LogError("Cube id: " + text + " is at: " + cubeData.gridLocation.x + " , " + cubeData.gridLocation.y + " , " + cubeData.gridLocation.z + " with orientation: " + cubeData.rotationIndex);
		}
	}

	public MemoryStream BuildBinaryStream()
	{
		return BuildBinaryStream(preserveDamage: false);
	}

	public MemoryStream BuildBinaryStream(bool preserveDamage)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(DTO.Count);
		for (int i = 0; i < DTO.Count; i++)
		{
			CubeData cubeData = DTO[i];
			binaryWriter.Write(cubeData.iID);
			Byte3 @byte = cubeData.gridLocation - gridOffset;
			binaryWriter.Write(@byte.x);
			binaryWriter.Write(@byte.y);
			binaryWriter.Write(@byte.z);
			byte b = cubeData.rotationIndex;
			if (cubeData.isDestroyed && preserveDamage)
			{
				b = (byte)(b | 0x80);
			}
			binaryWriter.Write(b);
		}
		return memoryStream;
	}

	public byte[] GetCompresesdRobotData()
	{
		return GetCompresesdRobotData(preserveDamage: false);
	}

	public byte[] GetCompresesdRobotData(bool preserveDamage)
	{
		List<byte> list = new List<byte>();
		byte[] bytes = BitConverter.GetBytes(DTO.Count);
		list.AddRange(bytes);
		for (int i = 0; i < DTO.Count; i++)
		{
			CubeData cubeData = DTO[i];
			bytes = BitConverter.GetBytes(cubeData.iID);
			list.AddRange(bytes);
			Byte3 @byte = cubeData.gridLocation - gridOffset;
			list.Add(@byte.x);
			list.Add(@byte.y);
			list.Add(@byte.z);
			byte b = cubeData.rotationIndex;
			if (cubeData.isDestroyed && preserveDamage)
			{
				b = (byte)(b | 0x80);
			}
			list.Add(b);
		}
		return list.ToArray();
	}

	public void SetColorData(byte[] data)
	{
		if (data != null && data.Length > 0)
		{
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						byte paletteIndex = binaryReader.ReadByte();
						Byte3 @byte = new Byte3(0, 0, 0);
						@byte.x = binaryReader.ReadByte();
						@byte.y = binaryReader.ReadByte();
						@byte.z = binaryReader.ReadByte();
						@byte += gridOffset;
						if (DTO.Contains(@byte))
						{
							DTO[@byte].paletteIndex = paletteIndex;
						}
					}
				}
			}
		}
	}

	public byte[] GetCompressedRobotColorData()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(DTO.Count);
				for (int i = 0; i < DTO.Count; i++)
				{
					CubeData cubeData = DTO[i];
					Byte3 @byte = cubeData.gridLocation - gridOffset;
					binaryWriter.Write(cubeData.paletteIndex);
					binaryWriter.Write(cubeData.gridLocation.x);
					binaryWriter.Write(cubeData.gridLocation.y);
					binaryWriter.Write(cubeData.gridLocation.z);
				}
				return memoryStream.ToArray();
			}
		}
	}
}
