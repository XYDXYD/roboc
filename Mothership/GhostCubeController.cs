using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class GhostCubeController
	{
		public GameObject cubeCursor;

		public event Action<GameObject, GameObject> OnChangeGhostCube = delegate
		{
		};

		public event Action<GhostCube> OnGhostCubeInitialized = delegate
		{
		};

		public void GhostCubeChanged(GameObject goCube)
		{
			this.OnChangeGhostCube(goCube, cubeCursor);
		}

		public void GhostCubeInitialized(GhostCube ghostCube)
		{
			this.OnGhostCubeInitialized(ghostCube);
		}
	}
}
