namespace PlayMaker
{
	public class CheckTargetRobotDestroyedRequestReturn : IPlaymakerRequestReturnResults
	{
		public bool hasDestroyedTargetRobot;

		public CheckTargetRobotDestroyedRequestReturn()
		{
			hasDestroyedTargetRobot = false;
		}

		public CheckTargetRobotDestroyedRequestReturn(bool hasFilledAllLocations_)
		{
			hasDestroyedTargetRobot = hasFilledAllLocations_;
		}

		public void SetDefaultReturnResults()
		{
			hasDestroyedTargetRobot = false;
		}
	}
}
