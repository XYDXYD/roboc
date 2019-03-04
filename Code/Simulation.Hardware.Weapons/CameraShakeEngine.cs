using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class CameraShakeEngine : MultiEntityViewsEngine<CameraShakeShootingEntityView, CameraShakeDamageEntityView>, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IEngine
	{
		private Dictionary<int, ShakeMovementData> _shakeData = new Dictionary<int, ShakeMovementData>();

		private float _currentPercent = 1f;

		private int _shakeCount;

		private bool _shakeIsEnabled;

		private ITaskRoutine _tickTask;

		private bool _enabled = true;

		private Transform _transform;

		private CameraShakeDamageEntityView _damageEntityView;

		[Inject]
		internal MachineCpuDataManager cpuManager
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer teamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal CameraSettings cameraSettings
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_shakeIsEnabled = cameraSettings.IsCameraShakeEnabled();
			cameraSettings.OnChangeCameraSettings += HandleEnableCameraShake;
			cpuManager.OnMachineCpuChanged += HandleOnMachineCpuChanged;
			cpuManager.OnMachineCpuInitialized += HandleOnMachineCpuInitialized;
			gameEndedObserver.OnGameEnded += HandleOnGameEnded;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_tickTask.Stop();
			cameraSettings.OnChangeCameraSettings -= HandleEnableCameraShake;
			cpuManager.OnMachineCpuChanged -= HandleOnMachineCpuChanged;
			cpuManager.OnMachineCpuInitialized -= HandleOnMachineCpuInitialized;
			gameEndedObserver.OnGameEnded -= HandleOnGameEnded;
		}

		void IQueryingEntityViewEngine.Ready()
		{
		}

		protected override void Add(CameraShakeDamageEntityView entityView)
		{
			_damageEntityView = entityView;
			_transform = _damageEntityView.cameraShakeDamageComponent.transformToShake;
			InitShakeData();
			_tickTask = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			_tickTask.SetEnumerator(Tick());
			_tickTask.Start((Action<PausableTaskException>)null, (Action)null);
		}

		protected override void Remove(CameraShakeDamageEntityView entityView)
		{
		}

		protected override void Add(CameraShakeShootingEntityView entityView)
		{
			entityView.camShakeComponent.applyShake.NotifyOnValueSet((Action<int, int>)HandleApplyCamShake);
		}

		protected override void Remove(CameraShakeShootingEntityView entityView)
		{
			entityView.camShakeComponent.applyShake.StopNotify((Action<int, int>)HandleApplyCamShake);
		}

		private void StartShake(TranslationCurve translationShootCurves, RotationCurve rotationShootCurves, float duration, float translationMagnitude, float rotationMagnitude)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			if (!_enabled || !_shakeIsEnabled)
			{
				return;
			}
			int validShakeKey = GetValidShakeKey();
			if (validShakeKey >= 0)
			{
				_shakeData[validShakeKey].UpdateValues(Time.get_time(), _transform.get_forward(), translationMagnitude, translationShootCurves, duration, ShakeMovementData.MovementType.Translation, available: false);
				validShakeKey = GetValidShakeKey();
				if (validShakeKey >= 0)
				{
					_shakeData[validShakeKey].UpdateValues(Time.get_time(), _transform.get_forward(), rotationMagnitude, rotationShootCurves, duration, ShakeMovementData.MovementType.Rotation, available: false);
				}
			}
		}

		private void HandleOnMachineCpuChanged(int shooterId, TargetType shooterType, int hitPlayerId, float currentPercent)
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			if (!_enabled || !_shakeIsEnabled || !teamsContainer.IsMe(TargetType.Player, hitPlayerId))
			{
				return;
			}
			if (currentPercent < _currentPercent)
			{
				float num = _currentPercent - currentPercent;
				int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, shooterId);
				Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine);
				int activeMachine2 = playerMachinesContainer.GetActiveMachine(TargetType.Player, hitPlayerId);
				Rigidbody rigidBodyData2 = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine2);
				if (rigidBodyData2 != null && rigidBodyData != null)
				{
					int validShakeKey = GetValidShakeKey();
					if (validShakeKey < 0)
					{
						return;
					}
					Vector3 val = rigidBodyData.get_worldCenterOfMass() - rigidBodyData2.get_worldCenterOfMass();
					_shakeData[validShakeKey].UpdateValues(Time.get_time(), val - _transform.get_forward(), num * _damageEntityView.cameraShakeDamageComponent.translationDamageMagnitudeMultiplier, _damageEntityView.cameraShakeDamageComponent.translationDamageCurves, _damageEntityView.cameraShakeDamageComponent.damageDuration, ShakeMovementData.MovementType.Translation, available: false);
					validShakeKey = GetValidShakeKey();
					if (validShakeKey < 0)
					{
						return;
					}
					_shakeData[validShakeKey].UpdateValues(Time.get_time(), val - _transform.get_forward(), num * _damageEntityView.cameraShakeDamageComponent.rotationDamageMagnitudeMultiplier, _damageEntityView.cameraShakeDamageComponent.rotationDamageCurves, _damageEntityView.cameraShakeDamageComponent.damageDuration, ShakeMovementData.MovementType.Rotation, available: false);
				}
			}
			_currentPercent = currentPercent;
		}

		private void HandleEnableCameraShake(bool enableShake)
		{
			_shakeIsEnabled = enableShake;
		}

		private void InitShakeData()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _damageEntityView.cameraShakeDamageComponent.maxSimultaneouslyShake; i++)
			{
				_shakeData.Add(i, new ShakeMovementData(0f, Vector3.get_zero(), 0f, null, 0f, new Random(i), ShakeMovementData.MovementType.Rotation, available: true));
			}
		}

		private int GetValidShakeKey()
		{
			for (int i = 0; i < _damageEntityView.cameraShakeDamageComponent.maxSimultaneouslyShake; i++)
			{
				if (_shakeData[i].available)
				{
					return i;
				}
			}
			return -1;
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				for (int i = 0; i < _damageEntityView.cameraShakeDamageComponent.maxSimultaneouslyShake; i++)
				{
					if (!_shakeData[i].available)
					{
						if (_shakeData[i].type == ShakeMovementData.MovementType.Translation)
						{
							HandleShakeTranslation(i);
						}
						else
						{
							HandleShakeRotation(i);
						}
					}
				}
				yield return null;
			}
		}

		private void HandleShakeTranslation(int key)
		{
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			float time = Time.get_time() - _shakeData[key].startingTime;
			float? x = MovementCurve.Evaluate(((TranslationCurve)_shakeData[key].curves).x, time, _shakeData[key].duration, _shakeData[key].random);
			float? y = MovementCurve.Evaluate(((TranslationCurve)_shakeData[key].curves).y, time, _shakeData[key].duration, _shakeData[key].random);
			float? z = MovementCurve.Evaluate(((TranslationCurve)_shakeData[key].curves).z, time, _shakeData[key].duration, _shakeData[key].random);
			if (!x.HasValue && !y.HasValue && !z.HasValue)
			{
				_shakeCount--;
				if (_shakeCount <= 0)
				{
					_shakeCount = 0;
					ResetCameraValues();
				}
				_shakeData[key].available = true;
				return;
			}
			Vector3 val = CalculateOffsetTranslation(x, y, z, _transform.get_forward() + _shakeData[key].direction, _shakeData[key].magnitude);
			Vector3 val2 = Vector3.get_zero();
			if (_shakeData[key].previousMovementValues.ContainsKey(_transform.GetInstanceID()))
			{
				val2 = _shakeData[key].previousMovementValues[_transform.GetInstanceID()];
			}
			else
			{
				_shakeCount++;
			}
			Vector3 val3 = val - val2;
			_shakeData[key].previousMovementValues[_transform.GetInstanceID()] = val;
			Transform transform = _transform;
			transform.set_localPosition(transform.get_localPosition() + val3);
		}

		private void HandleShakeRotation(int key)
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			float time = Time.get_time() - _shakeData[key].startingTime;
			float? x = MovementCurve.Evaluate(((RotationCurve)_shakeData[key].curves).x, time, _shakeData[key].duration, _shakeData[key].random);
			float? y = MovementCurve.Evaluate(((RotationCurve)_shakeData[key].curves).y, time, _shakeData[key].duration, _shakeData[key].random);
			if (!x.HasValue && !y.HasValue)
			{
				_shakeCount--;
				if (_shakeCount <= 0)
				{
					_shakeCount = 0;
					ResetCameraValues();
				}
				_shakeData[key].available = true;
				return;
			}
			Vector3 val = Vector2.op_Implicit(CalculateOffsetRotation(x, y, _transform.get_forward() + _shakeData[key].direction, _shakeData[key].magnitude));
			Vector3 val2 = Vector3.get_zero();
			if (_shakeData[key].previousMovementValues.ContainsKey(_transform.GetInstanceID()))
			{
				val2 = _shakeData[key].previousMovementValues[_transform.GetInstanceID()];
			}
			else
			{
				_shakeCount++;
			}
			Vector3 val3 = val - val2;
			_shakeData[key].previousMovementValues[_transform.GetInstanceID()] = val;
			_transform.Rotate(val3.x, val3.y, 0f, 1);
		}

		private void ResetCameraValues()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			_transform.set_localPosition(Vector3.get_zero());
			_transform.set_localRotation(Quaternion.get_identity());
		}

		private Vector3 CalculateOffsetTranslation(float? x, float? y, float? z, Vector3 direction, float magnitude)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			direction *= -1f;
			direction = _transform.InverseTransformDirection(direction);
			direction.Normalize();
			Vector3 val = default(Vector3);
			val.x = ((!x.HasValue) ? 0f : x.Value) * direction.x;
			val.y = ((!y.HasValue) ? 0f : y.Value) * direction.y;
			val.z = ((!z.HasValue) ? 0f : z.Value) * direction.z;
			val *= magnitude;
			return val;
		}

		private Vector2 CalculateOffsetRotation(float? x, float? y, Vector3 direction, float magnitude)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			direction.Normalize();
			float num = Mathf.Clamp(Vector3.Dot(direction, _transform.get_forward()), -1f, 1f);
			float num2 = Mathf.Clamp(Vector3.Dot(direction, _transform.get_right()), -1f, 1f);
			float num3 = ((!x.HasValue) ? 0f : x.Value) * (0f - num) * magnitude;
			float num4 = ((!y.HasValue) ? 0f : y.Value) * num2 * Mathf.Sign(0f - num) * magnitude;
			return new Vector2(num3, num4);
		}

		private void HandleOnGameEnded(bool obj)
		{
			_enabled = false;
		}

		private void HandleOnMachineCpuInitialized(int playerId, uint initialCpu)
		{
			if (teamsContainer.IsMe(TargetType.Player, playerId))
			{
				_currentPercent = 1f;
			}
		}

		private void HandleApplyCamShake(int sender, int weaponId)
		{
			CameraShakeShootingEntityView cameraShakeShootingEntityView = entityViewsDB.QueryEntityView<CameraShakeShootingEntityView>(weaponId);
			ICameraShakeComponent camShakeComponent = cameraShakeShootingEntityView.camShakeComponent;
			if (camShakeComponent.camShakeEnabled)
			{
				StartShake(camShakeComponent.shootCameraTranslationCurves, camShakeComponent.shootCameraRotationCurves, camShakeComponent.cameraShakeDuration, camShakeComponent.cameraShakeTranslationMagnitude, camShakeComponent.cameraShakeRotationMagnitude);
			}
		}
	}
}
