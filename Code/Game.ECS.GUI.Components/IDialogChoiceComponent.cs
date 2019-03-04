using Svelto.ECS;

namespace Game.ECS.GUI.Components
{
	internal interface IDialogChoiceComponent
	{
		DispatchOnChange<bool> cancelPressed
		{
			get;
		}

		DispatchOnChange<bool> validatePressed
		{
			get;
		}
	}
}
