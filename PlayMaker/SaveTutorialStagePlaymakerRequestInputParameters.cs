namespace PlayMaker
{
	public class SaveTutorialStagePlaymakerRequestInputParameters : IPlaymakerRequestInputParameters
	{
		public int stage;

		public SaveTutorialStagePlaymakerRequestInputParameters(int stage_)
		{
			stage = stage_;
		}

		void IPlaymakerRequestInputParameters.Clear()
		{
			stage = 0;
		}
	}
}
