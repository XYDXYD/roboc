using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class CubeRaycastInfo
	{
		internal RaycastHit hit
		{
			get;
			private set;
		}

		internal InstantiatedCube hitCube
		{
			get;
			private set;
		}

		public event Action OnInfoUpdated = delegate
		{
		};

		internal void UpdateInfo(RaycastHit hit, InstantiatedCube hitCube)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			this.hit = hit;
			this.hitCube = hitCube;
			this.OnInfoUpdated();
		}
	}
}
