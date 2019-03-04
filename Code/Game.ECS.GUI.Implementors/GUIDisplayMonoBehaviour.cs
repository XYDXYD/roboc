using Svelto.ECS;
using System;
using UnityEngine;

namespace Game.ECS.GUI.Implementors
{
	internal class GUIDisplayMonoBehaviour : MonoBehaviour, IGUIDisplayComponent
	{
		public DispatchOnChange<bool> IsShown
		{
			get;
			set;
		}

		public GUIDisplayMonoBehaviour()
			: this()
		{
		}

		internal void Initialize(int entityId)
		{
			IsShown = new DispatchOnChange<bool>(entityId);
			IsShown.NotifyOnValueSet((Action<int, bool>)ChangeGOVisibility);
			IsShown.set_value(true);
		}

		private void ChangeGOVisibility(int id, bool show)
		{
			this.get_gameObject().SetActive(show);
		}
	}
}
