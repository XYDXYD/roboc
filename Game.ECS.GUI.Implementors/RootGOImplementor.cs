using Game.ECS.GUI.Components;
using UnityEngine;

namespace Game.ECS.GUI.Implementors
{
	internal class RootGOImplementor : MonoBehaviour, IRootGOComponent
	{
		public GameObject rootGO => this.get_gameObject();

		public RootGOImplementor()
			: this()
		{
		}
	}
}
