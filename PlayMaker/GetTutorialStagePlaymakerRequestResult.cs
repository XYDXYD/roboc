namespace PlayMaker
{
	public class GetTutorialStagePlaymakerRequestResult : IPlaymakerRequestReturnResults
	{
		public int Stage;

		public GetTutorialStagePlaymakerRequestResult()
		{
			Stage = 0;
		}

		public GetTutorialStagePlaymakerRequestResult(int stage_)
		{
			Stage = stage_;
		}

		public void SetDefaultReturnResults()
		{
			Stage = 0;
		}
	}
}
