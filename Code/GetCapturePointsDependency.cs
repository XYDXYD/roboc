using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class GetCapturePointsDependency : NetworkDependency
{
	public struct Point
	{
		public readonly Vector3 position;

		public readonly Quaternion rotation;

		public readonly int team;

		public readonly float progress;

		public readonly float maxProgress;

		public Point(Vector3 pPosition, Quaternion pRotation, int pTeam, float pProgress, float pMaxProgress)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			position = pPosition;
			rotation = pRotation;
			team = pTeam;
			progress = pProgress;
			maxProgress = pMaxProgress;
		}
	}

	private const int NUM_CAPTURE_POINTS = 3;

	public readonly Point[] points = new Point[3];

	public GetCapturePointsDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				for (int i = 0; i < points.Length; i++)
				{
					Point point = points[i];
					DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(point.position, DataCompressor.CompressionType.MachinePosition);
					binaryWriter.Write(v3Compressed.x);
					binaryWriter.Write(v3Compressed.y);
					binaryWriter.Write(v3Compressed.z);
					DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(point.rotation, DataCompressor.CompressionType.MachineRotation);
					binaryWriter.Write(q3Compressed.x);
					binaryWriter.Write(q3Compressed.y);
					binaryWriter.Write(q3Compressed.z);
					binaryWriter.Write((sbyte)point.team);
					binaryWriter.Write(DataCompressor.CompressFloatAsByte(point.progress, DataCompressor.CompressionType.MachineRotation));
					binaryWriter.Write(DataCompressor.CompressFloatAsByte(point.maxProgress, DataCompressor.CompressionType.MachineRotation));
				}
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				for (int i = 0; i < points.Length; i++)
				{
					DataCompressor.V3Compressed data2 = default(DataCompressor.V3Compressed);
					data2.x = binaryReader.ReadInt16();
					data2.y = binaryReader.ReadInt16();
					data2.z = binaryReader.ReadInt16();
					DataCompressor.Q3Compressed data3 = default(DataCompressor.Q3Compressed);
					data3.x = binaryReader.ReadInt16();
					data3.y = binaryReader.ReadInt16();
					data3.z = binaryReader.ReadInt16();
					int pTeam = binaryReader.ReadSByte();
					float pProgress = DataCompressor.DecompressFloatAsByte(binaryReader.ReadByte(), DataCompressor.CompressionType.MachineRotation);
					float pMaxProgress = DataCompressor.DecompressFloatAsByte(binaryReader.ReadByte(), DataCompressor.CompressionType.MachineRotation);
					Point point = new Point(DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition), DataCompressor.DecompressQuaternion(data3, DataCompressor.CompressionType.MachineRotation), pTeam, pProgress, pMaxProgress);
					points[i] = point;
				}
			}
		}
	}
}
