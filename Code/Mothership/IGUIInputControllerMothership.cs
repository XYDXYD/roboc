using System;

namespace Mothership
{
	internal interface IGUIInputControllerMothership : IGUIInputController
	{
		BlurEffectController blurEffectController
		{
			set;
		}

		CurrentToolMode currentToolMode
		{
			set;
		}

		ICursorMode cursorMode
		{
			set;
		}

		event Action<GuiScreens> OnFailShowScreen;

		string GetActiveMainScreenName();

		bool IsMainScreen(GuiScreens screen);

		void MothershipFlowCompleted();
	}
}
