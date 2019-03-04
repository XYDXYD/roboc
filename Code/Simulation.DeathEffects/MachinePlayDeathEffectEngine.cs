using Fabric;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.Observer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System.Collections;
using UnityEngine;

namespace Simulation.DeathEffects
{
	internal class MachinePlayDeathEffectEngine : IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private readonly DestructionReporter _destructionReporter;

		private readonly Observable<Kill> _deathAnimationFinishedObservable;

		private readonly PlayerMachinesContainer _playerMachinesContainer;

		private readonly PlayerTeamsContainer _playerTeamsContainer;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public MachinePlayDeathEffectEngine(DestructionReporter destructionReporter, Observable<Kill> deathAnimationFinishedObservable, PlayerMachinesContainer playerMachinesContainer, PlayerTeamsContainer playerTeamsContainer)
		{
			_destructionReporter = destructionReporter;
			_deathAnimationFinishedObservable = deathAnimationFinishedObservable;
			_playerMachinesContainer = playerMachinesContainer;
			_playerTeamsContainer = playerTeamsContainer;
		}

		public void Ready()
		{
			_destructionReporter.OnPlayerSelfDestructs += OnSelfDestruct;
			_destructionReporter.OnMachineKilled += OnMachinedestroyed;
		}

		public void OnFrameworkDestroyed()
		{
			_destructionReporter.OnPlayerSelfDestructs -= OnSelfDestruct;
			_destructionReporter.OnMachineKilled -= OnMachinedestroyed;
		}

		private void OnSelfDestruct(int playerId)
		{
			OnMachinedestroyed(playerId, playerId);
		}

		private void OnMachinedestroyed(int victimId, int shooterId)
		{
			TaskRunner.get_Instance().Run(PlayEffect(victimId, shooterId));
		}

		private IEnumerator PlayEffect(int victimId, int shooterId)
		{
			int machineId = _playerMachinesContainer.GetActiveMachine(TargetType.Player, victimId);
			MachinePlayDeathEffectEntityView entityView = entityViewsDB.QueryEntityView<MachinePlayDeathEffectEntityView>(machineId);
			IDeathEffectComponent deathEffect = entityView.deathEffectComponent;
			IDeathEffectDependenciesComponent robot = entityView.deathEffectDependenciesComponent;
			Transform root = deathEffect.rootTransform;
			root.set_position(robot.rigidbody.get_position() + robot.rigidbody.get_rotation() * robot.machineCenter);
			if (deathEffect.randomRotation)
			{
				root.set_rotation(Random.get_rotationUniform());
			}
			else
			{
				Quaternion rotation = robot.rigidbody.get_transform().get_rotation();
				Vector3 eulerAngles = rotation.get_eulerAngles();
				float y = eulerAngles.y;
				root.set_rotation(Quaternion.AngleAxis(y, Vector3.get_up()));
			}
			Vector3 size = robot.machineSize;
			float longestAxis = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
			root.set_localScale(Vector3.get_one() * longestAxis * deathEffect.scaleFactor);
			robot.root.SetActive(false);
			bool isMe = _playerTeamsContainer.IsMe(TargetType.Player, victimId);
			TaskRunner.get_Instance().Run(PlayAnimation(entityView, isMe));
			if (isMe)
			{
				yield return HandleLocalPlayerDeath(entityView, victimId, shooterId);
			}
		}

		private static IEnumerator PlayAnimation(MachinePlayDeathEffectEntityView entityView, bool isMe)
		{
			IDeathEffectComponent deathEffect = entityView.deathEffectComponent;
			deathEffect.rootGameObject.SetActive(true);
			deathEffect.animation.Play();
			string audioEvent = (!isMe) ? deathEffect.audioEventForOthers : deathEffect.audioEventForPlayer;
			EventManager.get_Instance().PostEvent(audioEvent, 0, (object)null, deathEffect.rootGameObject);
			while (deathEffect.animation.get_isPlaying())
			{
				yield return null;
			}
			deathEffect.rootGameObject.SetActive(false);
		}

		private IEnumerator HandleLocalPlayerDeath(MachinePlayDeathEffectEntityView entityView, int victimId, int shooterId)
		{
			CameraDeathAnimationEntityView camera = entityViewsDB.QueryEntityViews<CameraDeathAnimationEntityView>().get_Item(0);
			camera.deathAnimationBroadcastComponent.isAnimating.set_value(true);
			yield return null;
			TaskRunner.get_Instance().Run(AnimateCamera(entityView, camera));
			yield return (object)new WaitForSecondsEnumerator(entityView.deathEffectComponent.deathEffectsData.deathDuration);
			camera.deathAnimationBroadcastComponent.isAnimating.set_value(false);
			Kill kill = new Kill(victimId, shooterId);
			_deathAnimationFinishedObservable.Dispatch(ref kill);
		}

		private static IEnumerator AnimateCamera(MachinePlayDeathEffectEntityView robotEntityView, CameraDeathAnimationEntityView cameraEntityView)
		{
			IDeathAnimationComponent camera = cameraEntityView.deathAnimationComponent;
			camera.controlScript.set_enabled(false);
			IDeathEffectDependenciesComponent robot = robotEntityView.deathEffectDependenciesComponent;
			Transform robotT = robot.rigidbody.get_transform();
			Vector3 size = robot.machineSize;
			float longestAxis = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
			Vector3 robotPosition = robotT.get_position() + robotT.get_rotation() * robot.machineCenter + Vector3.get_up() * longestAxis * camera.ratioAboveRobot;
			Transform cameraTransform = camera.controlScript.get_transform();
			float timeElapsed = 0f;
			Vector3 startPosition = cameraTransform.get_position();
			float pitch = camera.pitch;
			Quaternion rotation = cameraTransform.get_rotation();
			Vector3 eulerAngles = rotation.get_eulerAngles();
			float yaw = eulerAngles.y;
			float roll = 0f;
			Vector3 lookDirection = Quaternion.Euler(pitch, yaw, roll) * Vector3.get_forward();
			Vector3 targetPosition = robotPosition - lookDirection * camera.distance * longestAxis;
			RaycastHit hit = default(RaycastHit);
			if (Physics.SphereCast(robotPosition, camera.controlScript.cameraCollisionRadius, -lookDirection, ref hit, camera.distance * longestAxis, GameLayers.ENVIRONMENT_LAYER_MASK))
			{
				float num = hit.get_distance() - camera.controlScript.cameraCollisionRadius;
				targetPosition = robotPosition - lookDirection * num;
			}
			Quaternion startRotation = cameraTransform.get_rotation();
			Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
			float inversePositionDuration = 1f / camera.positionDuration;
			float inverseRotationDuration = 1f / camera.rotationDuration;
			bool positionAnimating = true;
			bool rotationAnimating = true;
			while (positionAnimating || rotationAnimating)
			{
				timeElapsed += Time.get_deltaTime();
				positionAnimating = (timeElapsed < camera.positionDuration);
				rotationAnimating = (timeElapsed < camera.rotationDuration);
				if (positionAnimating)
				{
					float num2 = camera.positionCurve.Evaluate(timeElapsed * inversePositionDuration);
					Vector3 position = Vector3.Lerp(startPosition, targetPosition, num2);
					cameraTransform.set_position(position);
				}
				if (rotationAnimating)
				{
					float num3 = camera.rotationCurve.Evaluate(timeElapsed * inverseRotationDuration);
					Quaternion rotation2 = Quaternion.Slerp(startRotation, targetRotation, num3);
					cameraTransform.set_rotation(rotation2);
				}
				yield return null;
			}
		}
	}
}
