using Svelto.ECS;

namespace Game.ECS.GUI.Components
{
	internal interface IShowComponent
	{
		DispatchOnChange<bool> isShown
		{
			get;
		}
	}
}
