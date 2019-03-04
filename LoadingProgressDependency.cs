using Authentication;
using RCNetwork.Events;
using System;
using System.IO;

internal class LoadingProgressDependency : NetworkDependency
{
	public string UserName
	{
		get;
		private set;
	}

	public float Progress
	{
		get;
		private set;
	}

	public LoadingProgressDependency(float progress)
		: this(User.Username, progress)
	{
	}

	public LoadingProgressDependency(string userName, float progress)
	{
		if (progress < 0f || progress > 1f)
		{
			throw new Exception("Progress must be between 0 and 1");
		}
		UserName = userName;
		Progress = progress;
	}

	public LoadingProgressDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(UserName);
				binaryWriter.Write(Progress);
			}
			return memoryStream.ToArray();
		}
	}

	public override void Deserialise(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				UserName = binaryReader.ReadString();
				Progress = binaryReader.ReadSingle();
			}
		}
	}
}
