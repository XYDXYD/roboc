using Svelto.DataStructures;
using UnityEngine;

namespace Simulation
{
	internal static class PhysicsActivator
	{
		public static RBEntity ActivatePhysicsKinematic()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			GameObject val = new GameObject("SimulationBoard");
			return CreateKinematicRigidBody(val.get_transform());
		}

		private static RBEntity CreateKinematicRigidBody(Transform parent)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			GameObject val = new GameObject("RigidBodyParent__");
			Transform transform = val.get_transform();
			transform.set_parent(parent);
			RBEntity rBEntity = new RBEntity(parent.get_gameObject());
			rBEntity.rigidBody = val.AddComponent<Rigidbody>();
			rBEntity.rigidBody.set_useGravity(true);
			rBEntity.rigidBody.set_isKinematic(true);
			rBEntity.rigidBody.set_interpolation(1);
			return rBEntity;
		}

		internal static void UpdatePhysics(Rigidbody rb, FasterList<InstantiatedCube> destroyedCubes, FasterList<InstantiatedCube> spawnedCubes)
		{
			float num = rb.get_mass();
			if (spawnedCubes != null)
			{
				for (int i = 0; i < spawnedCubes.get_Count(); i++)
				{
					num += spawnedCubes.get_Item(i).mass;
				}
			}
			if (destroyedCubes != null)
			{
				for (int j = 0; j < destroyedCubes.get_Count(); j++)
				{
					num -= destroyedCubes.get_Item(j).mass;
				}
			}
			rb.set_mass(num);
		}
	}
}
