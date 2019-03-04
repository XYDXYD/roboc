using Game.ECS.GUI.Components;
using Svelto.ECS;
using System;
using UnityEngine;

namespace Game.ECS.GUI.Implementors
{
	internal class ShowImplementor : MonoBehaviour, IShowComponent, IImplementor
	{
		public DispatchOnChange<bool> isShown
		{
			get;
			private set;
		}

		public ShowImplementor()
			: this()
		{
		}

		public void Initialize(int entityId)
		{
			isShown = new DispatchOnChange<bool>(entityId);
			isShown.NotifyOnValueSet((Action<int, bool>)Show);
		}

		private void Show(int entityId, bool show)
		{
			this.get_gameObject().SetActive(show);
		}
	}
}
