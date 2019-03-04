namespace Mothership
{
	internal class AdditionalClanSectionActivationInfo
	{
		public bool ShouldPushCurrentState;

		public bool ShouldRestorePreviousState;

		public AdditionalClanSectionActivationInfo(bool shouldPushCurrentState_, bool shouldRestorePreviousState_)
		{
			ShouldPushCurrentState = shouldPushCurrentState_;
			ShouldRestorePreviousState = shouldRestorePreviousState_;
		}
	}
}
