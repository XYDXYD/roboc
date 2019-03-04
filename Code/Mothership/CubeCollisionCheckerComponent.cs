using UnityEngine;

namespace Mothership
{
	internal sealed class CubeCollisionCheckerComponent : MonoBehaviour
	{
		internal MachineEditorCollisionChecker machineCollisionChecker
		{
			private get;
			set;
		}

		public CubeCollisionCheckerComponent()
			: this()
		{
		}

		public void MoveToQueueCheckCollisionLayer()
		{
			Collider[] componentsInChildren = this.GetComponentsInChildren<Collider>(true);
			Collider[] array = componentsInChildren;
			foreach (Collider val in array)
			{
				if (val.get_gameObject().get_layer() == GameLayers.BUILD_COLLISION)
				{
					val.get_gameObject().set_layer(GameLayers.BUILDCOLLISION_UNVERIFIED);
				}
				val.set_isTrigger(true);
			}
		}

		public void MoveToVerifiedCollisionLayer()
		{
			Collider[] componentsInChildren = this.GetComponentsInChildren<Collider>(true);
			Collider[] array = componentsInChildren;
			foreach (Collider val in array)
			{
				if (val.get_gameObject().get_layer() == GameLayers.BUILDCOLLISION_UNVERIFIED)
				{
					val.get_gameObject().set_layer(GameLayers.BUILD_COLLISION);
				}
				val.set_isTrigger(false);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			GameObject gameObject = other.get_gameObject();
			while (gameObject.GetComponent<Rigidbody>() == null)
			{
				if (gameObject.get_transform().get_parent() == null)
				{
					return;
				}
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
			}
			if (gameObject.get_gameObject().get_layer() != GameLayers.GHOST_CUBE && ((1 << other.get_gameObject().get_layer()) & GameLayers.BUILD_COLLISION_VERIFICATION_MASK) != 0 && gameObject.get_activeSelf())
			{
				machineCollisionChecker.SetCollisionBetween(this.get_gameObject(), gameObject, setting: true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			GameObject gameObject = other.get_gameObject();
			while (gameObject.GetComponent<Rigidbody>() == null)
			{
				if (gameObject.get_transform().get_parent() == null)
				{
					return;
				}
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
			}
			if (gameObject.get_gameObject().get_layer() != GameLayers.GHOST_CUBE && ((1 << other.get_gameObject().get_layer()) & GameLayers.BUILD_COLLISION_VERIFICATION_MASK) != 0 && gameObject.get_activeSelf())
			{
				machineCollisionChecker.SetCollisionBetween(this.get_gameObject(), gameObject, setting: false);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			GameObject gameObject = other.get_gameObject();
			while (gameObject.GetComponent<Rigidbody>() == null)
			{
				if (gameObject.get_transform().get_parent() == null)
				{
					return;
				}
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
			}
			if (gameObject.get_gameObject().get_layer() != GameLayers.GHOST_CUBE && ((1 << other.get_gameObject().get_layer()) & GameLayers.BUILD_COLLISION_VERIFICATION_MASK) != 0 && gameObject.get_activeSelf())
			{
				machineCollisionChecker.SetCollisionBetween(this.get_gameObject(), gameObject, setting: true);
			}
		}
	}
}
