using Game.ECS.GUI.Components;
using UnityEngine;

namespace Game.ECS.GUI.Implementors
{
	internal class UiElementVisibleImplementor : MonoBehaviour, IUIElementVisibleComponent
	{
		public bool uiElementHidden
		{
			set
			{
				this.get_gameObject().SetActive(!value);
			}
		}

		public UiElementVisibleImplementor()
			: this()
		{
		}
	}
}
