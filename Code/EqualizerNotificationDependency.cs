using RCNetwork.Events;
using System.IO;

internal class EqualizerNotificationDependency : NetworkDependency
{
	private EqualizerNotification _equalizerNotification;

	private int _maxHealth;

	private int _health;

	private short _teamID;

	private short _time;

	public int Health
	{
		get
		{
			return _health;
		}
		private set
		{
			_health = value;
		}
	}

	public int MaxHealth
	{
		get
		{
			return _maxHealth;
		}
		private set
		{
			_maxHealth = value;
		}
	}

	public short TeamID
	{
		get
		{
			return _teamID;
		}
		set
		{
			_teamID = value;
		}
	}

	public short Time
	{
		get
		{
			return _time;
		}
		set
		{
			_time = value;
		}
	}

	public EqualizerNotification EqualizerNotific
	{
		get
		{
			return _equalizerNotification;
		}
		set
		{
			_equalizerNotification = value;
		}
	}

	public EqualizerNotificationDependency()
	{
		_maxHealth = -1;
		_health = -1;
	}

	public EqualizerNotificationDependency(byte[] data)
		: base(data)
	{
	}

	public void SetParameters(EqualizerNotification equalizerNotification, short teamId, short time, int maxHealth = -1, int health = -1)
	{
		_equalizerNotification = equalizerNotification;
		_teamID = teamId;
		_time = time;
		_maxHealth = maxHealth;
		_health = health;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)EqualizerNotific);
				binaryWriter.Write(TeamID);
				binaryWriter.Write(Time);
				binaryWriter.Write(_maxHealth);
				binaryWriter.Write(_health);
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
				EqualizerNotific = (EqualizerNotification)binaryReader.ReadByte();
				TeamID = binaryReader.ReadInt16();
				Time = binaryReader.ReadInt16();
				_maxHealth = binaryReader.ReadInt32();
				_health = binaryReader.ReadInt32();
			}
		}
	}
}
