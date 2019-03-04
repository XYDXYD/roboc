using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using System.IO;
using UnityEngine;

internal sealed class FireMissDependency : NetworkDependency
{
	public int shootingMachineId;

	public Vector3 hitPoint;

	public Vector3 hitNormal;

	public ItemDescriptor itemDescriptor = new WeaponDescriptor(ItemCategory.NotAFunctionalItem, ItemSize.NotAWeapon);

	public bool hitSelf;

	public bool hit;

	public TargetType targetType;

	public float timeStamp;

	public FireMissDependency()
	{
	}

	public FireMissDependency(byte[] data)
		: base(data)
	{
	}

	public void SetVariables(int shootingMachineId_, ItemDescriptor _subCategory, Vector3 _hitPoint, Vector3 _hitNormal, bool _hit, bool _hitSelf, float _timeStamp, TargetType _targetType)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		shootingMachineId = shootingMachineId_;
		itemDescriptor = _subCategory;
		hitPoint = _hitPoint;
		hitNormal = _hitNormal;
		hit = _hit;
		hitSelf = _hitSelf;
		timeStamp = _timeStamp;
		targetType = _targetType;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((short)shootingMachineId);
				binaryWriter.Write(DataCompressor.CompressFloat(hitPoint.x, DataCompressor.CompressionType.FireMissRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitPoint.y, DataCompressor.CompressionType.FireMissRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitPoint.z, DataCompressor.CompressionType.FireMissRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitNormal.x, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitNormal.y, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitNormal.z, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(timeStamp);
				byte b = 0;
				b = (byte)(b | (byte)(hit ? 1 : 0));
				b = (byte)(b | (byte)((hitSelf ? 1 : 0) << 1));
				memoryStream.WriteByte(b);
				binaryWriter.Write((byte)targetType);
				binaryWriter.Write((int)itemDescriptor.itemCategory);
				binaryWriter.Write((int)itemDescriptor.itemSize);
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
				shootingMachineId = binaryReader.ReadInt16();
				hitPoint.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.FireMissRange);
				hitPoint.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.FireMissRange);
				hitPoint.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.FireMissRange);
				hitNormal.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitNormal.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitNormal.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				timeStamp = binaryReader.ReadSingle();
				byte b = binaryReader.ReadByte();
				hit = ((b & 1) != 0);
				hitSelf = (((b >> 1) & 1) != 0);
				targetType = (TargetType)binaryReader.ReadByte();
				itemDescriptor.itemCategory = (ItemCategory)binaryReader.ReadInt32();
				itemDescriptor.itemSize = (ItemSize)binaryReader.ReadInt32();
			}
		}
	}
}
