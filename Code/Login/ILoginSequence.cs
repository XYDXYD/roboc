using System.Collections;

namespace Login
{
	internal interface ILoginSequence
	{
		void Initialise(GenericLoginController controller);

		IEnumerator UpdateState();

		bool CheckFinished();

		bool UserInputOccured(SplashLoginGUIMessageType inputType);
	}
}
