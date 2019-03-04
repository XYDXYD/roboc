using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeamBaseDestructionAudioTrigger : IInitialize
	{
		public const string IMPACT_AUDIO_GAMEOBJECT = "ImpactAudio";

		private Dictionary<int, GameObject> _teamBases = new Dictionary<int, GameObject>();

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnProtoniumDamageApplied += HandleOnDamageApplied;
			gameObjectPool.Preallocate(1, 20, (Func<GameObject>)(() => gameObjectPool.AddRecycleOnDisableForAudio()));
		}

		public void AddBase(int team, GameObject teamBaseGameObject)
		{
			_teamBases.Add(team, teamBaseGameObject);
			TeamBaseCreated(teamBaseGameObject);
		}

		private void TeamBaseCreated(GameObject teamBaseGameObject)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.TeamBaseLoop), teamBaseGameObject);
		}

		private void HandleOnDamageApplied(DestructionData data)
		{
			if (data.targetType != TargetType.TeamBase)
			{
				return;
			}
			if (data.destroyedCubes.get_Count() > 0)
			{
				GameObject gameObjectForCubePosition = GetGameObjectForCubePosition(data.destroyedCubes.get_Item(0), data.targetType, data.hitRigidbody);
				if (data.destroyedCubes.get_Count() > 1)
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.CrystalMultiDestroyed), gameObjectForCubePosition);
				}
				else
				{
					EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.ProtoniumCubeDestroyed), gameObjectForCubePosition);
				}
			}
			if (data.damagedCubes.get_Count() > 0)
			{
				GameObject gameObjectForCubePosition2 = GetGameObjectForCubePosition(data.damagedCubes.get_Item(0), data.targetType, data.hitRigidbody);
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.ProtoniumCubeHit), gameObjectForCubePosition2);
			}
		}

		private GameObject GetGameObjectForCubePosition(InstantiatedCube cube, TargetType targetType, Rigidbody rigidbody)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectPool.Use(1, (Func<GameObject>)(() => gameObjectPool.AddRecycleOnDisableForAudio()));
			val.SetActive(true);
			val.get_transform().set_position(GridScaleUtility.GetCubeWorldPosition(cube.gridPos, rigidbody, targetType));
			return val;
		}
	}
}
