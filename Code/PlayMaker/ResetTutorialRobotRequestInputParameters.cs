namespace PlayMaker
{
	public class ResetTutorialRobotRequestInputParameters : IPlaymakerRequestInputParameters
	{
		public int stage;

		public ResetTutorialRobotRequestInputParameters(int stage_)
		{
			stage = stage_;
		}

		void IPlaymakerRequestInputParameters.Clear()
		{
			stage = 0;
		}
	}
}
