using UnityEngine;

namespace Mothership
{
	internal class GhostCubeSpecificCollision : MonoBehaviour
	{
		public GameObject regularColliders;

		public GameObject ghostCubeColliders;

		public GhostCubeSpecificCollision()
			: this()
		{
		}

		private void Awake()
		{
			regularColliders.SetActive(true);
			ghostCubeColliders.SetActive(false);
		}

		public void SwitchToGhostCubeColliders()
		{
			ghostCubeColliders.SetActive(true);
			regularColliders.SetActive(false);
		}
	}
}
