using System;
using System.IO;
using UnityEngine;

internal sealed class MachineMotionDependency
{
	public float timeStamp;

	public int playerId;

	public PlayerMachineMotionHistoryFrame.RigidBodyState rbState;

	public Vector3 targetPoint;

	private static float _lastSentSecondsA = Time.get_realtimeSinceStartup();

	private static DateTime _lastSentSecondsB = DateTime.UtcNow;

	public MachineMotionDependency(int playerId, Rigidbody _rididbody, Vector3 _targetPoint, float _timeStamp)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		this.playerId = playerId;
		timeStamp = _timeStamp;
		rbState = new PlayerMachineMotionHistoryFrame.RigidBodyState(_rididbody);
		targetPoint = _targetPoint;
	}

	public MachineMotionDependency(byte[] data)
	{
		Deserialise(data);
	}

	public static int GetSize()
	{
		return 47;
	}

	public byte[] Serialise()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				UpdateSendTimeStamps(binaryWriter);
				binaryWriter.Write(timeStamp);
				binaryWriter.Write((byte)playerId);
				DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(targetPoint, DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				DataCompressor.V3Compressed v3Compressed2 = DataCompressor.CompressVector3(rbState.position, DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed2.x);
				binaryWriter.Write(v3Compressed2.y);
				binaryWriter.Write(v3Compressed2.z);
				DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(rbState.rotation, DataCompressor.CompressionType.MachineRotation);
				binaryWriter.Write(q3Compressed.x);
				binaryWriter.Write(q3Compressed.y);
				binaryWriter.Write(q3Compressed.z);
				DataCompressor.V3Compressed v3Compressed3 = DataCompressor.CompressVector3(rbState.angularVelocity, DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed3.x);
				binaryWriter.Write(v3Compressed3.y);
				binaryWriter.Write(v3Compressed3.z);
				DataCompressor.V3Compressed v3Compressed4 = DataCompressor.CompressVector3(rbState.centreOfMass, DataCompressor.CompressionType.MachineMaxSizeWorldSpace);
				binaryWriter.Write(v3Compressed4.x);
				binaryWriter.Write(v3Compressed4.y);
				binaryWriter.Write(v3Compressed4.z);
				return memoryStream.ToArray();
			}
		}
	}

	public void Deserialise(byte[] data)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				UpdateReadTimeStamps(binaryReader);
				timeStamp = binaryReader.ReadSingle();
				playerId = binaryReader.ReadByte();
				DataCompressor.V3Compressed data2 = default(DataCompressor.V3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				targetPoint = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				rbState = new PlayerMachineMotionHistoryFrame.RigidBodyState();
				DataCompressor.V3Compressed data3 = default(DataCompressor.V3Compressed);
				data3.x = binaryReader.ReadInt16();
				data3.y = binaryReader.ReadInt16();
				data3.z = binaryReader.ReadInt16();
				rbState.position = DataCompressor.DecompressVector3(data3, DataCompressor.CompressionType.MachinePosition);
				DataCompressor.Q3Compressed data4 = default(DataCompressor.Q3Compressed);
				data4.x = binaryReader.ReadInt16();
				data4.y = binaryReader.ReadInt16();
				data4.z = binaryReader.ReadInt16();
				rbState.rotation = DataCompressor.DecompressQuaternion(data4, DataCompressor.CompressionType.MachineRotation);
				DataCompressor.V3Compressed data5 = default(DataCompressor.V3Compressed);
				data5.x = binaryReader.ReadInt16();
				data5.y = binaryReader.ReadInt16();
				data5.z = binaryReader.ReadInt16();
				rbState.angularVelocity = DataCompressor.DecompressVector3(data5, DataCompressor.CompressionType.MachinePosition);
				DataCompressor.V3Compressed data6 = default(DataCompressor.V3Compressed);
				data6.x = binaryReader.ReadInt16();
				data6.y = binaryReader.ReadInt16();
				data6.z = binaryReader.ReadInt16();
				rbState.centreOfMass = DataCompressor.DecompressVector3(data6, DataCompressor.CompressionType.MachineMaxSizeWorldSpace);
				rbState.worldCOM = rbState.rotation * rbState.centreOfMass + rbState.position;
			}
		}
	}

	private void UpdateReadTimeStamps(BinaryReader br)
	{
		br.ReadSingle();
		br.ReadDouble();
	}

	internal static void OnGameStart()
	{
		_lastSentSecondsA = Time.get_realtimeSinceStartup();
		_lastSentSecondsB = DateTime.UtcNow;
	}

	private static void UpdateSendTimeStamps(BinaryWriter bw)
	{
		bw.Write(Time.get_realtimeSinceStartup() - _lastSentSecondsA);
		bw.Write((DateTime.UtcNow - _lastSentSecondsB).TotalSeconds);
		_lastSentSecondsA = Time.get_realtimeSinceStartup();
		_lastSentSecondsB = DateTime.UtcNow;
	}
}
