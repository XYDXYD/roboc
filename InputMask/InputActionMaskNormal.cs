namespace InputMask
{
	internal class InputActionMaskNormal : IInputActionMask
	{
		public bool InputIsAvailable(UserInputCategory category, int axis)
		{
			return true;
		}

		public void RejectAllInputByDefault(bool setting)
		{
		}

		public void AcceptInputAction(UserInputCategory category, int axis)
		{
		}

		public void RejectInputAction(UserInputCategory category, int axis)
		{
		}
	}
}
