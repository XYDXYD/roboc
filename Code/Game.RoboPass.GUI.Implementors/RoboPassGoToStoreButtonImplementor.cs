using Game.ECS.GUI.Components;
using Svelto.ECS;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	internal class RoboPassGoToStoreButtonImplementor : MonoBehaviour, IButtonComponent, IUIElementVisibleComponent
	{
		public DispatchOnChange<bool> buttonPressed
		{
			get;
			private set;
		}

		public bool uiElementHidden
		{
			set
			{
				this.get_gameObject().SetActive(!value);
			}
		}

		public RoboPassGoToStoreButtonImplementor()
			: this()
		{
		}

		public void OnClick()
		{
			buttonPressed.set_value(true);
			buttonPressed.set_value(false);
		}

		internal void Initialize(int entityId)
		{
			buttonPressed = new DispatchOnChange<bool>(entityId);
		}
	}
}
