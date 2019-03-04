using Fabric;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.SpawnEffects
{
	internal class MachinePlaySpawnEffectEngine : IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private readonly MachineSpawnDispatcher _machineSpawnDispatcher;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public MachinePlaySpawnEffectEngine(MachineSpawnDispatcher machineSpawnDispatcher)
		{
			_machineSpawnDispatcher = machineSpawnDispatcher;
		}

		public void Ready()
		{
			_machineSpawnDispatcher.OnPlayerSpawnedIn += OnPlayerSpawned;
			_machineSpawnDispatcher.OnPlayerRespawnedIn += OnPlayerSpawned;
		}

		public void OnFrameworkDestroyed()
		{
			_machineSpawnDispatcher.OnPlayerRespawnedIn -= OnPlayerSpawned;
			_machineSpawnDispatcher.OnPlayerSpawnedIn -= OnPlayerSpawned;
		}

		private void OnPlayerSpawned(SpawnInParametersPlayer spawnInParameters)
		{
			TaskRunner.get_Instance().Run(PlayEffect(spawnInParameters.machineId, spawnInParameters.isMe, spawnInParameters.isLocal));
		}

		private IEnumerator PlayEffect(int machineId, bool isMe, bool isLocal)
		{
			yield return null;
			MachinePhysicsActivationEntityView localMachine = null;
			if (isLocal)
			{
				localMachine = entityViewsDB.QueryEntityView<MachinePhysicsActivationEntityView>(machineId);
				localMachine.spawnableComponent.isSpawning.set_value(true);
			}
			MachinePlaySpawnEffectEntityView entityView = entityViewsDB.QueryEntityView<MachinePlaySpawnEffectEntityView>(machineId);
			ISpawnEffectComponent spawnEffect = entityView.spawnEffectComponent;
			ISpawnEffectDependenciesComponent dependencies = entityView.spawnEffectDependenciesComponent;
			Rigidbody rb = entityView.spawnEffectDependenciesComponent.rigidbody;
			Vector3 robotPosition = rb.get_position() + rb.get_rotation() * dependencies.machineCenter;
			Vector3 val = robotPosition;
			Vector3 up = Vector3.get_up();
			Vector3 machineSize = dependencies.machineSize;
			Vector3 groundPosition = val - up * machineSize.y * 0.5f;
			spawnEffect.groundTransform.set_position(groundPosition);
			spawnEffect.robotTransform.set_position(robotPosition);
			Quaternion rot = rb.get_transform().get_rotation();
			spawnEffect.groundTransform.set_rotation(rot);
			spawnEffect.robotTransform.set_rotation(rot);
			Vector3 size = dependencies.machineSize;
			float longestAxis = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
			spawnEffect.groundTransform.set_localScale(Vector3.get_one() * longestAxis * spawnEffect.scaleFactor);
			spawnEffect.robotTransform.set_localScale(spawnEffect.groundTransform.get_localScale());
			spawnEffect.rootGameObject.SetActive(true);
			FasterListEnumerator<Renderer> enumerator = entityView.spawnEffectDependenciesComponent.allRenderers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Renderer current = enumerator.get_Current();
					current.set_enabled(false);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			string audioEvent = (!isMe) ? spawnEffect.audioEventForOthers : spawnEffect.audioEventForPlayer;
			EventManager.get_Instance().PostEvent(audioEvent, 0, (object)null, spawnEffect.groundTransform.get_gameObject());
			entityView.spawnEffectDependenciesComponent.robotAnimating = true;
			TaskRunner.get_Instance().Run(PlayAnimation(entityView));
			if (isMe)
			{
				FasterReadOnlyList<CameraSpawnAnimationEntityView> val2 = entityViewsDB.QueryEntityViews<CameraSpawnAnimationEntityView>();
				TaskRunner.get_Instance().Run(AnimateCamera(entityView, val2.get_Item(0)));
			}
			yield return (object)new WaitForSecondsEnumerator(spawnEffect.spawnEffectsData.spawnDuration);
			entityView.spawnEffectDependenciesComponent.robotAnimating = false;
			FasterListEnumerator<Renderer> enumerator2 = entityView.spawnEffectDependenciesComponent.allRenderers.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Renderer current2 = enumerator2.get_Current();
					current2.set_enabled(true);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			if (isLocal)
			{
				localMachine.spawnableComponent.isSpawning.set_value(false);
			}
		}

		private static IEnumerator PlayAnimation(MachinePlaySpawnEffectEntityView entityView)
		{
			ISpawnEffectComponent spawnEffect = entityView.spawnEffectComponent;
			spawnEffect.animation.Play();
			ISpawnEffectDependenciesComponent robot = entityView.spawnEffectDependenciesComponent;
			bool isRobotVisible = !spawnEffect.robotVisibility.get_activeSelf();
			while (spawnEffect.animation.get_isPlaying())
			{
				if (robot.robotAnimating && isRobotVisible != spawnEffect.robotVisibility.get_activeSelf())
				{
					isRobotVisible = spawnEffect.robotVisibility.get_activeSelf();
					FasterListEnumerator<Renderer> enumerator = robot.allRenderers.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Renderer current = enumerator.get_Current();
							current.set_enabled(spawnEffect.robotVisibility.get_activeSelf());
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				yield return null;
			}
			spawnEffect.rootGameObject.SetActive(false);
		}

		private static IEnumerator AnimateCamera(MachinePlaySpawnEffectEntityView robotEntityView, CameraSpawnAnimationEntityView cameraEntityView)
		{
			ISpawnAnimationComponent camera = cameraEntityView.spawnAnimationComponent;
			camera.controlScript.isInputAllowed = false;
			ISpawnEffectDependenciesComponent robot = robotEntityView.spawnEffectDependenciesComponent;
			Vector3 size = robot.machineSize;
			float longestAxis = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
			Transform cameraTransform = camera.controlScript.cameraParent;
			float timeElapsed = 0f;
			Vector3 lookDirection = Quaternion.Euler(camera.pitch, 0f, 0f) * Vector3.get_forward();
			float distance = camera.distance * longestAxis;
			RaycastHit hit = default(RaycastHit);
			if (Physics.SphereCast(cameraTransform.get_position(), camera.controlScript.cameraCollisionRadius, -cameraTransform.TransformDirection(lookDirection), ref hit, distance, GameLayers.ENVIRONMENT_LAYER_MASK))
			{
				distance = hit.get_distance() - camera.controlScript.cameraCollisionRadius;
			}
			Vector3 startPosition = -lookDirection * distance;
			Quaternion startRotation = Quaternion.LookRotation(lookDirection);
			float inversePositionDuration = 1f / camera.positionDuration;
			float inverseRotationDuration = 1f / camera.rotationDuration;
			bool positionAnimating = true;
			bool rotationAnimating = true;
			Vector3 zero = Vector3.get_zero();
			Quaternion identity = Quaternion.get_identity();
			while ((robot.robotAnimating && positionAnimating) || rotationAnimating)
			{
				timeElapsed += Time.get_deltaTime();
				positionAnimating = (timeElapsed < camera.positionDuration);
				rotationAnimating = (timeElapsed < camera.rotationDuration);
				if (positionAnimating)
				{
					float num = camera.positionCurve.Evaluate(timeElapsed * inversePositionDuration);
					cameraTransform.set_localPosition(Vector3.Lerp(startPosition, zero, num));
				}
				if (rotationAnimating)
				{
					float num2 = camera.rotationCurve.Evaluate(timeElapsed * inverseRotationDuration);
					cameraTransform.set_localRotation(Quaternion.Slerp(startRotation, identity, num2));
				}
				yield return null;
			}
			cameraTransform.set_localPosition(zero);
			cameraTransform.set_localRotation(identity);
			camera.controlScript.isInputAllowed = true;
		}
	}
}
