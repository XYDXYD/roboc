using Svelto.ECS;

namespace Game.ECS.GUI.Implementors
{
	internal interface IGUIDisplayComponent
	{
		DispatchOnChange<bool> IsShown
		{
			get;
		}
	}
}
