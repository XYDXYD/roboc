using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System.IO;
using UnityEngine;

namespace Events.Dependencies
{
	internal sealed class MultipleFireMissesDependency : NetworkDependency
	{
		public int numHits;

		public int shootingMachineId;

		public FasterList<Vector3> hitPoints = new FasterList<Vector3>();

		public FasterList<Vector3> hitNormals = new FasterList<Vector3>();

		public FasterList<bool> hitSelfList = new FasterList<bool>();

		public FasterList<bool> hitList = new FasterList<bool>();

		public FasterList<TargetType> targetTypeList = new FasterList<TargetType>();

		public ItemDescriptor itemDescriptor = new WeaponDescriptor(ItemCategory.NotAFunctionalItem, ItemSize.NotAWeapon);

		public float timeStamp;

		public MultipleFireMissesDependency()
		{
		}

		public MultipleFireMissesDependency(byte[] data)
			: base(data)
		{
		}

		public void SetVariables(int numHits_, int shootingMachineId_, ItemDescriptor itemDescriptor_, FasterList<Vector3> hitPoints_, FasterList<Vector3> hitNormals_, FasterList<bool> hitSelfList_, FasterList<bool> hitList_, float timeStamp_, FasterList<TargetType> targetTypeList_)
		{
			numHits = numHits_;
			shootingMachineId = shootingMachineId_;
			hitPoints = hitPoints_;
			hitNormals = hitNormals_;
			hitSelfList = hitSelfList_;
			hitList = hitList_;
			targetTypeList = targetTypeList_;
			itemDescriptor = itemDescriptor_;
			timeStamp = timeStamp_;
		}

		public override byte[] Serialise()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((byte)numHits);
					binaryWriter.Write((short)shootingMachineId);
					for (int num = numHits - 1; num >= 0; num--)
					{
						BinaryWriter binaryWriter2 = binaryWriter;
						Vector3 val = hitPoints.get_Item(num);
						binaryWriter2.Write(DataCompressor.CompressFloat(val.x, DataCompressor.CompressionType.FireMissRange));
						BinaryWriter binaryWriter3 = binaryWriter;
						Vector3 val2 = hitPoints.get_Item(num);
						binaryWriter3.Write(DataCompressor.CompressFloat(val2.y, DataCompressor.CompressionType.FireMissRange));
						BinaryWriter binaryWriter4 = binaryWriter;
						Vector3 val3 = hitPoints.get_Item(num);
						binaryWriter4.Write(DataCompressor.CompressFloat(val3.z, DataCompressor.CompressionType.FireMissRange));
						BinaryWriter binaryWriter5 = binaryWriter;
						Vector3 val4 = hitNormals.get_Item(num);
						binaryWriter5.Write(DataCompressor.CompressFloat(val4.x, DataCompressor.CompressionType.NormalRange));
						BinaryWriter binaryWriter6 = binaryWriter;
						Vector3 val5 = hitNormals.get_Item(num);
						binaryWriter6.Write(DataCompressor.CompressFloat(val5.y, DataCompressor.CompressionType.NormalRange));
						BinaryWriter binaryWriter7 = binaryWriter;
						Vector3 val6 = hitNormals.get_Item(num);
						binaryWriter7.Write(DataCompressor.CompressFloat(val6.z, DataCompressor.CompressionType.NormalRange));
						byte b = 0;
						b = (byte)(b | (byte)(hitList.get_Item(num) ? 1 : 0));
						b = (byte)(b | (byte)((hitSelfList.get_Item(num) ? 1 : 0) << 1));
						memoryStream.WriteByte(b);
						binaryWriter.Write((byte)targetTypeList.get_Item(num));
					}
					binaryWriter.Write(timeStamp);
					binaryWriter.Write((short)itemDescriptor.itemCategory);
					binaryWriter.Write((short)itemDescriptor.itemSize);
					return memoryStream.ToArray();
				}
			}
		}

		public override void Deserialise(byte[] data)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					numHits = binaryReader.ReadByte();
					shootingMachineId = binaryReader.ReadInt16();
					for (int num = numHits - 1; num >= 0; num--)
					{
						Vector3 zero = Vector3.get_zero();
						Vector3 zero2 = Vector3.get_zero();
						zero.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.FireMissRange);
						zero.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.FireMissRange);
						zero.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.FireMissRange);
						zero2.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
						zero2.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
						zero2.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
						byte b = binaryReader.ReadByte();
						bool flag = (b & 1) != 0;
						bool flag2 = ((b >> 1) & 1) != 0;
						TargetType targetType = (TargetType)binaryReader.ReadByte();
						hitPoints.Add(zero);
						hitNormals.Add(zero2);
						hitList.Add(flag);
						hitSelfList.Add(flag2);
						targetTypeList.Add(targetType);
					}
					timeStamp = binaryReader.ReadSingle();
					itemDescriptor.itemCategory = (ItemCategory)binaryReader.ReadInt16();
					itemDescriptor.itemSize = (ItemSize)binaryReader.ReadInt16();
				}
			}
		}
	}
}
