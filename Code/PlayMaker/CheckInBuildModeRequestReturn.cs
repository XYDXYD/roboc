namespace PlayMaker
{
	public class CheckInBuildModeRequestReturn : IPlaymakerRequestReturnResults
	{
		public bool isInBuildMode;

		public CheckInBuildModeRequestReturn()
		{
			isInBuildMode = false;
		}

		public CheckInBuildModeRequestReturn(bool hasFilledAllLocations_)
		{
			isInBuildMode = hasFilledAllLocations_;
		}

		public void SetDefaultReturnResults()
		{
			isInBuildMode = false;
		}
	}
}
