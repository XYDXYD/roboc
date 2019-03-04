using Fabric;
using Svelto.Command;
using Svelto.ECS;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ShootingAfterEffectsEngine : MultiEntityViewsEngine<ShootingAfterEffectsNode, MachineInputNode>, IQueryingEntityViewEngine, IEngine
	{
		private Dictionary<int, ShootingAfterEffectsNode> _weapons = new Dictionary<int, ShootingAfterEffectsNode>();

		private GameObject _currentMuzzleFlashPrefab;

		private readonly Func<GameObject> _onMuzzleFlashEffectAllocation;

		private IServiceRequestFactory _serviceRequestFactory;

		private IMachineInputComponent _playerInputComponent;

		private ShootingAfterEffectsNode _currentPlayerShootingAfterEffects;

		private PowerBarSettingsData _powerBarSettingsData;

		private bool _shootingStoppedAudioCanPlay;

		private float _playerStartFiringTime;

		private float _playerStopFiringTime;

		private float _currentPlayerMovementParameterValue;

		private float _savedPlayerMovementParameterValue;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public ShootingAfterEffectsEngine(IServiceRequestFactory serviceRequestFactory)
		{
			_serviceRequestFactory = serviceRequestFactory;
			_onMuzzleFlashEffectAllocation = OnMuzzleFlashEffectAllocation;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadPowerBarSettings);
			_shootingStoppedAudioCanPlay = true;
		}

		public void Ready()
		{
		}

		protected override void Add(ShootingAfterEffectsNode entityView)
		{
			_weapons.Add(entityView.get_ID(), entityView);
			entityView.afterEffectsComponent.applyRecoil.subscribers += HandleApplyRecoil;
			entityView.afterEffectsComponent.playMuzzleFlash.subscribers += HandlePlayMuzzleFlash;
			entityView.afterEffectsComponent.playFiringSound.subscribers += HandlePlayFiringSound;
		}

		protected override void Remove(ShootingAfterEffectsNode entityView)
		{
			_weapons.Remove(entityView.get_ID());
			entityView.afterEffectsComponent.applyRecoil.subscribers -= HandleApplyRecoil;
			entityView.afterEffectsComponent.playMuzzleFlash.subscribers -= HandlePlayMuzzleFlash;
			entityView.afterEffectsComponent.playFiringSound.subscribers -= HandlePlayFiringSound;
		}

		protected override void Add(MachineInputNode entityView)
		{
			if (entityView.ownerComponent.ownedByMe)
			{
				_playerInputComponent = entityView.machineInput;
				TaskRunner.get_Instance().Run((Func<IEnumerator>)HandlePlayerInput);
			}
		}

		protected override void Remove(MachineInputNode entityView)
		{
		}

		private void HandleApplyRecoil(IPlayAfterEffectsComponent sender, int weaponId)
		{
			if (_weapons.TryGetValue(weaponId, out ShootingAfterEffectsNode value))
			{
				ApplyRecoil(value);
			}
		}

		private void HandlePlayMuzzleFlash(IPlayAfterEffectsComponent sender, int weaponId)
		{
			if (_weapons.TryGetValue(weaponId, out ShootingAfterEffectsNode value))
			{
				PlayMuzzleFlash(value);
			}
		}

		private void HandlePlayFiringSound(IPlayAfterEffectsComponent sender, int weaponId)
		{
			if (_weapons.TryGetValue(weaponId, out ShootingAfterEffectsNode value))
			{
				PlayFiringSound(value);
			}
		}

		private GameObject OnMuzzleFlashEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForGameObject(_currentMuzzleFlashPrefab);
		}

		private void PreallocateMuzzleFlash(bool isEnemy, IWeaponMuzzleFlash muzzleFlash)
		{
			_currentMuzzleFlashPrefab = ((!isEnemy) ? muzzleFlash.muzzleFlashPrefab : muzzleFlash.muzzleFlashPrefabEnemy);
			gameObjectPool.Preallocate(_currentMuzzleFlashPrefab.get_name(), 2, _onMuzzleFlashEffectAllocation);
		}

		private void PlayFiringSound(ShootingAfterEffectsNode node)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			if (!node.visibilityComponent.inRange)
			{
				return;
			}
			Transform t = node.transformComponent.T;
			string text;
			if (node.weaponOwner.ownedByMe)
			{
				if (_currentPlayerShootingAfterEffects == null)
				{
					_playerStartFiringTime = Time.get_time();
					_savedPlayerMovementParameterValue = _currentPlayerMovementParameterValue;
					_currentPlayerShootingAfterEffects = node;
				}
				text = node.firingAudioComponent.firingAudio;
				PlayShootingStoppedAudioIfNoMoreMana();
			}
			else
			{
				bool flag = false;
				if (node.weaponOwner.isEnemy)
				{
					Vector3 position = node.weaponRotationTransforms.verticalTransform.get_position();
					Vector3 val = node.weaponAimingComponent.targetPoint - position;
					RaycastHit val2 = default(RaycastHit);
					if (Physics.SphereCast(position, 3f, val, ref val2, node.weaponRangeComponent.maxRange, GameLayers.MYSELF_LAYER_MASK))
					{
						flag = true;
					}
				}
				text = ((!flag) ? node.firingAudioComponent.enemyPlayerFiringAudio : node.firingAudioComponent.enemyPlayerFiringMeAudio);
			}
			EventManager.get_Instance().PostEvent(text, 0, (object)null, t.get_gameObject());
		}

		private void PlayShootingStoppedAudioIfNoMoreMana()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			IPowerBarDataComponent powerBarDataComponent = entityViewsDB.QueryEntityViews<PowerBarNode>().get_Item(0).powerBarDataComponent;
			float num = _currentPlayerShootingAfterEffects.weaponFireCostComponent.weaponFireCost;
			float num2 = _currentPlayerShootingAfterEffects.weaponCooldownComponent.weaponCooldown;
			if (_currentPlayerShootingAfterEffects.itemDescriptorComponent.itemDescriptor.itemCategory == ItemCategory.Chaingun)
			{
				num2 = Time.get_deltaTime();
				num *= Time.get_deltaTime();
			}
			float num3 = powerBarDataComponent.powerValue - num;
			if (powerBarDataComponent.progressiveIncrementActive)
			{
				num3 = Math.Min(num3 + (float)(double)_powerBarSettingsData.PowerForAllRobots * _powerBarSettingsData.RefillRatePerSecond * num2, (float)(double)_powerBarSettingsData.PowerForAllRobots);
			}
			if (num3 - num < 0f)
			{
				_shootingStoppedAudioCanPlay = true;
				PlayShootingStoppedAudio();
			}
		}

		private void PlayMuzzleFlash(ShootingAfterEffectsNode node)
		{
			if (node.visibilityComponent.offScreen)
			{
				return;
			}
			Transform t = node.transformComponent.T;
			IWeaponMuzzleFlash muzzleFlashComponent = node.muzzleFlashComponent;
			Transform verticalTransform = node.weaponRotationTransforms.verticalTransform;
			if (t.get_gameObject().get_activeSelf() && !node.zoomedComponent.isZoomed)
			{
				GameObject val = null;
				if (node.weaponOwner.isEnemy && muzzleFlashComponent.muzzleFlashPrefabEnemy != null)
				{
					_currentMuzzleFlashPrefab = muzzleFlashComponent.muzzleFlashPrefabEnemy;
					val = gameObjectPool.Use(_currentMuzzleFlashPrefab.get_name(), _onMuzzleFlashEffectAllocation);
				}
				else if (muzzleFlashComponent.muzzleFlashPrefab != null)
				{
					_currentMuzzleFlashPrefab = muzzleFlashComponent.muzzleFlashPrefab;
					val = gameObjectPool.Use(_currentMuzzleFlashPrefab.get_name(), _onMuzzleFlashEffectAllocation);
				}
				if (val == null)
				{
					Console.LogError("muzzle flash prefab == null");
				}
				else
				{
					PlayEffect(muzzleFlashComponent.muzzleFlashLocations[muzzleFlashComponent.activeMuzzleFlash], verticalTransform, val);
				}
			}
		}

		private void PlayEffect(Transform positionTransform, Transform verticalTransform, GameObject muzzle)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if (positionTransform != null && muzzle != null)
			{
				muzzle.SetActive(true);
				Transform transform = muzzle.get_transform();
				transform.set_parent(verticalTransform);
				transform.set_localPosition(Vector3.get_zero());
				transform.set_localRotation(Quaternion.get_identity());
				transform.set_localScale(Vector3.get_one());
				if (positionTransform != null)
				{
					transform.set_position(positionTransform.get_position());
				}
			}
		}

		private void ApplyRecoil(ShootingAfterEffectsNode node)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (!node.visibilityComponent.offScreen)
			{
				Rigidbody rb = node.rigidBodyComponent.rb;
				if (rb != null)
				{
					rb.AddForceAtPosition(node.weaponRotationTransforms.verticalTransform.get_forward() * node.recoilForceComponent.recoilForce, node.cubePositionComponent.position, 1);
				}
			}
		}

		private IEnumerator HandlePlayerInput()
		{
			while (true)
			{
				if (_currentPlayerShootingAfterEffects != null)
				{
					if (_playerInputComponent.fire1 == 0f)
					{
						PlayShootingStoppedAudio();
						_playerStopFiringTime = Time.get_time();
						_savedPlayerMovementParameterValue = _currentPlayerMovementParameterValue;
					}
					else
					{
						float num = Mathf.Clamp(Time.get_time() - _playerStartFiringTime, 0f, 3f);
						EventManager.get_Instance().SetGlobalParameter("Autofire_Time", num);
						_currentPlayerMovementParameterValue = Mathf.Clamp01(_savedPlayerMovementParameterValue + (Time.get_time() - _playerStartFiringTime) * 10f);
						if (_currentPlayerMovementParameterValue < 1f)
						{
							EventManager.get_Instance().SetGlobalParameter("MovingParts_VolumeDrop", _currentPlayerMovementParameterValue);
						}
						_shootingStoppedAudioCanPlay = true;
					}
				}
				else
				{
					_currentPlayerMovementParameterValue = Mathf.Clamp01(_savedPlayerMovementParameterValue - (Time.get_time() - _playerStopFiringTime));
					if (_currentPlayerMovementParameterValue > 0f)
					{
						EventManager.get_Instance().SetGlobalParameter("MovingParts_VolumeDrop", _currentPlayerMovementParameterValue);
					}
				}
				yield return null;
			}
		}

		private void PlayShootingStoppedAudio()
		{
			if (_shootingStoppedAudioCanPlay)
			{
				_shootingStoppedAudioCanPlay = false;
				Transform t = _currentPlayerShootingAfterEffects.transformComponent.T;
				string stopFiringAudio = _currentPlayerShootingAfterEffects.firingAudioComponent.stopFiringAudio;
				EventManager.get_Instance().PostEvent(stopFiringAudio, 0, (object)null, t.get_gameObject());
				_currentPlayerShootingAfterEffects = null;
			}
		}

		private IEnumerator LoadPowerBarSettings()
		{
			TaskService<PowerBarSettingsData> taskService = _serviceRequestFactory.Create<IGetPowerBarSettingsRequest>().AsTask();
			HandleTaskServiceWithError taskServiceWithErrorHandling = new HandleTaskServiceWithError(taskService, delegate
			{
				RemoteLogger.Error("Unable to load power bar settings", "PowerBarEngine IGetPowerBarSettingsRequest", null);
				Console.LogWarning("Unable to load power bar settings");
			}, null);
			yield return taskServiceWithErrorHandling.GetEnumerator();
			PowerBarSettingsData powerBarSettingsData = _powerBarSettingsData = taskService.result;
		}
	}
}
