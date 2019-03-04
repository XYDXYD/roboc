namespace Robocraft.GUI
{
	internal interface IGenericComponentView : IAnchorableUIElement
	{
		void Setup();

		void SetController(IGenericComponent controller);

		void Show();

		void Hide();
	}
}
