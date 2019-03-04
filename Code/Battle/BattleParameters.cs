using System;
using Utility;

namespace Battle
{
	internal abstract class BattleParameters
	{
		private int _hostPort;

		public string BattleId
		{
			get;
			private set;
		}

		public string MapName
		{
			get;
			protected set;
		}

		public string HostIP
		{
			get;
			protected set;
		}

		public int HostPort
		{
			get
			{
				if (_hostPort == 0)
				{
					throw new Exception("Host port not initialised");
				}
				return _hostPort;
			}
		}

		public DateTime ReceivedEnterBattleTime
		{
			get;
			protected set;
		}

		public NetworkConfig NetworkConfigs
		{
			get;
			protected set;
		}

		public CustomGameConfigOverrides CustomGameConfig
		{
			get;
			protected set;
		}

		public GameModeType GameMode
		{
			get;
			protected set;
		}

		public void Clear()
		{
			MapName = string.Empty;
			HostIP = string.Empty;
			_hostPort = 0;
			ReceivedEnterBattleTime = DateTime.UtcNow;
			NetworkConfigs = null;
			CustomGameConfig = null;
		}

		protected virtual void OnReceivedParameters(BattleParametersData parameters)
		{
			if (parameters == null)
			{
				throw new Exception("battle parameters are null");
			}
			Console.Log("Setting host port to " + parameters.HostPort);
			HostIP = parameters.HostIP;
			_hostPort = parameters.HostPort;
			MapName = parameters.MapName;
			ReceivedEnterBattleTime = parameters.ReceivedEnterBattleTime;
			NetworkConfigs = parameters.NetworkConfigs;
			CustomGameConfig = parameters.CustomGameConfig;
			BattleId = parameters.GameGuid;
			GameModeKey gameModeKey = parameters.GameModeKey;
			GameMode = gameModeKey.type;
		}
	}
}
