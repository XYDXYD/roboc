using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.IoC;
using Utility;

namespace Simulation
{
	internal class TeamBaseLowHealthAudio : IInitialize, IWaitForFrameworkDestruction
	{
		private float[] _teamBaseHealth = new float[2];

		private string[] _lastAudioPlayed = new string[2]
		{
			string.Empty,
			string.Empty
		};

		[Inject]
		public HealthTracker healthTracker
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public VOManager voManager
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			healthTracker.OnEntityHealthChanged += OnEntityHealthChanged;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			healthTracker.OnEntityHealthChanged -= OnEntityHealthChanged;
		}

		private void OnEntityHealthChanged(TargetType type, int playerId, float currentPercent, float deltaHealthPercent)
		{
			if (type == TargetType.TeamBase)
			{
				bool flag = playerTeamsContainer.IsOnMyTeam(type, playerId);
				_teamBaseHealth[playerId] += deltaHealthPercent;
				float num = _teamBaseHealth[playerId];
				string text = string.Empty;
				if (num >= 0.75f)
				{
					text = ((!flag) ? AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_ANN_E_Charged75) : AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_ANN_Charged75));
				}
				else if (num >= 0.5f)
				{
					text = ((!flag) ? AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_ANN_E_Charged50) : AudioFabricEvent.Name(AudioFabricGameEvents.VO_RBA_ANN_Charged50));
				}
				if (text != string.Empty && _lastAudioPlayed[playerId] != text)
				{
					Console.Log(num + " " + text);
					voManager.PlayVO(text);
					_lastAudioPlayed[playerId] = text;
				}
			}
		}
	}
}
