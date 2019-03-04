using System.Collections;

namespace Login
{
	internal class LoginSequencePlatformNotSupported : LoginSequenceBase
	{
		public override void Initialise(GenericLoginController loginController_)
		{
			base.Initialise(loginController_);
			loginController = loginController_;
		}

		public override bool CheckFinished()
		{
			return false;
		}

		public override bool CheckVideoFinished()
		{
			return false;
		}

		public override void AdvanceToNextStage()
		{
		}

		protected override void FinaliseSharedLoginSequence()
		{
		}

		public override IEnumerator UpdateState()
		{
			loginController.RaiseFatalErrorDialog(StringTableBase<StringTable>.Instance.GetString("strLoginFailure"), StringTableBase<StringTable>.Instance.GetString("strPlatformNotSupported"));
			yield return null;
		}

		public override bool UserInputOccured(SplashLoginGUIMessageType inputType)
		{
			return false;
		}
	}
}
