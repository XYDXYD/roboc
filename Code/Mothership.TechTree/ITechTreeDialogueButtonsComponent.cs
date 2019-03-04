using Svelto.ECS;

namespace Mothership.TechTree
{
	internal interface ITechTreeDialogueButtonsComponent
	{
		DispatchOnSet<bool> ConfirmButton
		{
			get;
		}

		DispatchOnSet<bool> CancelButton
		{
			get;
		}

		DispatchOnSet<bool> Dismissed
		{
			get;
		}
	}
}
