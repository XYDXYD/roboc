using Svelto.ECS;

namespace Game.ECS.GUI.Components
{
	internal interface IButtonComponent
	{
		DispatchOnChange<bool> buttonPressed
		{
			get;
		}
	}
}
