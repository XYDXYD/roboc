using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class AerofoilRemoteGFXEngine : IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
			TaskRunner.get_Instance().Run(Update());
		}

		private void SetThrustGFXRemote(RemoteAerofoilGraphicsNode node)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			Transform transform = node.rigidbodyComponent.rb.get_transform();
			GameObject onSFXGameObject = node.aerofoilGfxComponent.onSFXGameObject;
			GameObject thrustSFXGameObject = node.aerofoilGfxComponent.thrustSFXGameObject;
			if (onSFXGameObject.get_activeSelf())
			{
				onSFXGameObject.SetActive(false);
			}
			if (Mathf.Abs(Vector3.Dot(t.get_forward(), transform.get_forward())) > 0.99f)
			{
				if (!thrustSFXGameObject.get_activeSelf())
				{
					thrustSFXGameObject.SetActive(true);
				}
			}
			else if (thrustSFXGameObject.get_activeSelf())
			{
				thrustSFXGameObject.SetActive(false);
			}
		}

		private void SetFlapOrientationRemote(RemoteAerofoilGraphicsNode node)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			Transform transform = node.rigidbodyComponent.rb.get_transform();
			Vector3 lastPos = node.aerofoilGfxComponent.lastPos;
			Transform flapT = node.aerofoilGfxComponent.flapT;
			if (Mathf.Abs(Vector3.Dot(t.get_forward(), transform.get_forward())) > 0.99f)
			{
				Vector3 val = (flapT.get_position() - lastPos) / Time.get_deltaTime();
				Vector3 val2 = Vector3.get_up() * (1f - 71f / (339f * (float)Math.PI) * Mathf.Clamp(val.get_magnitude(), 0f, 15f));
				val = val.get_normalized() * (71f / (339f * (float)Math.PI)) * Mathf.Clamp(val.get_magnitude(), 0f, 15f);
				Vector3 val3 = val + val2;
				Vector3 normalized = val3.get_normalized();
				float num = Vector3.Dot(normalized, flapT.get_up());
				flapT.Rotate(num * 7.5f, 0f, 0f);
				node.aerofoilGfxComponent.lastPos = flapT.get_position();
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<RemoteAerofoilGraphicsNode> remoteAeroFoils = entityViewsDB.QueryEntityViews<RemoteAerofoilGraphicsNode>();
				for (int i = 0; i < remoteAeroFoils.get_Count(); i++)
				{
					RemoteAerofoilGraphicsNode remoteAerofoilGraphicsNode = remoteAeroFoils.get_Item(i);
					if (!remoteAerofoilGraphicsNode.disabledComponent.disabled)
					{
						SetThrustGFXRemote(remoteAerofoilGraphicsNode);
						SetFlapOrientationRemote(remoteAerofoilGraphicsNode);
					}
				}
				yield return null;
			}
		}
	}
}
