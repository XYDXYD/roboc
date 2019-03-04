using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal class MapPingsSpawnerBehaviour : MonoBehaviour, IMapPingSpawnerComponent, IComponent
	{
		[SerializeField]
		private GameObject[] mapPingPrefabs;

		[SerializeField]
		private CamPingIndicatorSpawner camPingIndicatorSpawner;

		[SerializeField]
		private float pingLifeTime = 5f;

		[SerializeField]
		private float pingCooldown = 10f;

		[SerializeField]
		private int pingNumber = 2;

		[SerializeField]
		private float colorDecreasingPercentage = 1f;

		[SerializeField]
		private float scalingUpPercentage = 2f;

		[SerializeField]
		private bool showPlayerPingIndicator;

		[Inject]
		internal IGameObjectFactory factory
		{
			private get;
			set;
		}

		[Inject]
		private PlayerNamesContainer playerNamesContainer
		{
			get;
			set;
		}

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		public event Action<float, float, int> InitializePingTimesAndNumber = delegate
		{
		};

		public event Action OnPingDestroyed = delegate
		{
		};

		public MapPingsSpawnerBehaviour()
			: this()
		{
		}

		private void Start()
		{
			for (int i = 0; i < mapPingPrefabs.Length; i++)
			{
				factory.RegisterPrefab(mapPingPrefabs[i], mapPingPrefabs[i].get_name(), this.get_transform().get_parent().get_gameObject());
			}
			this.InitializePingTimesAndNumber(pingLifeTime, pingCooldown, pingNumber);
		}

		public void ShowPingAtLocation(Vector3 location, PingType type, string user, float life)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = factory.Build(mapPingPrefabs[(int)type].get_name());
			val.get_transform().set_position(location);
			MapPingBehaviour component = val.GetComponent<MapPingBehaviour>();
			component.nameLabelNormal.set_text(user);
			component.nameLabelTransparent.set_text(user);
			component.OnDestroy += OnDestroy;
			component.life = life;
			component.colorDecreasingPercentage = colorDecreasingPercentage;
			component.scalingUpPercentage = scalingUpPercentage;
			if (!user.Equals(playerNamesContainer.GetPlayerName(playerTeamsContainer.localPlayerId)) || showPlayerPingIndicator)
			{
				camPingIndicatorSpawner.SpawnCamPingIndicator(val.GetComponent<MapPingBehaviour>(), type);
			}
		}

		public void OnDestroy()
		{
			this.OnPingDestroyed();
		}
	}
}
