using Svelto.ECS;
using System;
using UnityEngine;

namespace Simulation.Hardware
{
	internal class RemoveEntityImplementor : MonoBehaviour, IRemoveEntityComponent, IImplementor
	{
		public Action removeEntity
		{
			get;
			set;
		}

		public RemoveEntityImplementor()
			: this()
		{
		}

		private void OnDestroy()
		{
			if (removeEntity != null)
			{
				removeEntity();
			}
		}
	}
}
