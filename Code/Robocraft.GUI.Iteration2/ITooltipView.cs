namespace Robocraft.GUI.Iteration2
{
	internal interface ITooltipView : IView
	{
		void ShowTooltip(UIWidget referenceWidget, object content, GenericTooltipArea.Location preferredLocation);
	}
}
