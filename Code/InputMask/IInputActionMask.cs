namespace InputMask
{
	internal interface IInputActionMask
	{
		bool InputIsAvailable(UserInputCategory category, int axis);

		void AcceptInputAction(UserInputCategory category, int axis);

		void RejectInputAction(UserInputCategory category, int axis);

		void RejectAllInputByDefault(bool setting);
	}
}
