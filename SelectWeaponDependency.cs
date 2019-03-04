using RCNetwork.Events;
using System;
using System.IO;

internal sealed class SelectWeaponDependency : NetworkDependency
{
	public int machineId;

	public ItemDescriptor category = new WeaponDescriptor(ItemCategory.NotAFunctionalItem, ItemSize.NotAWeapon);

	public SelectWeaponDependency()
	{
	}

	public SelectWeaponDependency(byte[] data)
		: base(data)
	{
	}

	public void SetParameters(int machineId, ItemDescriptor category)
	{
		this.machineId = machineId;
		this.category = category;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Convert.ToByte(machineId));
				binaryWriter.Write(Convert.ToUInt32((uint)category.itemCategory));
				binaryWriter.Write(Convert.ToUInt32((uint)category.itemSize));
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
				machineId = Convert.ToInt32(binaryReader.ReadByte());
				category.itemCategory = (ItemCategory)binaryReader.ReadUInt32();
				category.itemSize = (ItemSize)binaryReader.ReadUInt32();
			}
		}
	}
}
