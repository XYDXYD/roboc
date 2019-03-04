using Game.ECS.GUI.Components;
using Svelto.ECS;
using UnityEngine;

namespace Mothership.GUI
{
	internal class StatsHintPopupAreaImplementor : MonoBehaviour, ICubeTypeIDComponent, IMouseOverStateComponent
	{
		public DispatchOnChange<bool> isMouseOver
		{
			get;
			private set;
		}

		public CubeTypeID cubeTypeId
		{
			get;
			set;
		}

		public StatsHintPopupAreaImplementor()
			: this()
		{
		}

		public void Initialize(int entityId)
		{
			isMouseOver = new DispatchOnChange<bool>(entityId);
		}

		private void OnHover(bool isHover)
		{
			isMouseOver.set_value(isHover);
		}
	}
}
