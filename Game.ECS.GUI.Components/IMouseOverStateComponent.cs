using Svelto.ECS;

namespace Game.ECS.GUI.Components
{
	internal interface IMouseOverStateComponent
	{
		DispatchOnChange<bool> isMouseOver
		{
			get;
		}
	}
}
